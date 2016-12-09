require 'erb'
require_relative './csharp/scalar'
require_relative './reformatter'

module GraphQLGenerator
  class CSharp
    class << self
      def erb_for(template_filename)
        erb = ERB.new(File.read(template_filename))
        erb.filename = template_filename
        erb
      end
    end

    attr_reader :scalars, :schema, :namespace
  
    def initialize(schema, namespace:, custom_scalars: [], script_name: $0)
      @schema = schema
      @namespace = namespace
      @script_name = script_name

      @scalars = (BUILTIN_SCALARS + custom_scalars).reduce({}) { |hash, scalar| hash[scalar.graph_type] = scalar; hash }
      @scalars.default_proc = proc do |hash,key|
        Scalar.new(
          graph_type: key,
          csharp_type: 'string',
        );
      end
    end
    
    ERBS = {
      "Root.cs" => erb_for(File.expand_path("../csharp/root.cs.erb", __FILE__)),
      "Arguments.cs" => erb_for(File.expand_path("../csharp/arguments.cs.erb", __FILE__)),
      "InputBase.cs" => erb_for(File.expand_path("../csharp/input_base.cs.erb", __FILE__)),
      "InputValueToString.cs" => erb_for(File.expand_path("../csharp/input_value_to_string.cs.erb", __FILE__)),
      "QueryBase.cs" => erb_for(File.expand_path("../csharp/query_base.cs.erb", __FILE__)),
      "TopLevelResponse.cs" => erb_for(File.expand_path("../csharp/top_level_response.cs.erb", __FILE__)),
      "AbstractResponse.cs" => erb_for(File.expand_path("../csharp/abstract_response.cs.erb", __FILE__)),
    }

    TYPE_ERB = erb_for(File.expand_path("../csharp/type.cs.erb", __FILE__))
    TYPE_RESPONSE_ERB = erb_for(File.expand_path("../csharp/type_response.cs.erb", __FILE__))

    INDENTATION = " " * 4

    RESERVED_WORDS = [
      "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out",  "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string","struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "query"
    ]

    BUILTIN_SCALARS = [
      Scalar.new(
        graph_type: 'Int',
        csharp_type: 'long',
      ),
      Scalar.new(
        graph_type: 'Float',
        csharp_type: 'double',
      ),
      Scalar.new(
        graph_type: 'String',
        csharp_type: 'string',
      ),
      Scalar.new(
        graph_type: 'Boolean',
        csharp_type: 'bool',
      ),
      Scalar.new(
        graph_type: 'ID',
        csharp_type: 'string',
      ),
    ]
    

    def save(path)
      path_types = "#{path}/Types"

      begin
        Dir.mkdir path_types
      rescue Errno::EEXIST
      end

      # output classes on root
      ERBS.each do |key,erb|
        File.write("#{path}/#{key}", reformat(erb.result(binding)))
      end

      # output type definitions
      schema.types.reject{ |type| type.builtin? || type.scalar? }.each do |type|
        # output 
        if type.object? || type.interface?
          File.write("#{path_types}/#{type.name}Query.cs", reformat(TYPE_ERB.result(binding)))
          File.write("#{path_types}/#{type.name}.cs", reformat(TYPE_RESPONSE_ERB.result(binding)))
        elsif type.input_object?
          File.write("#{path_types}/#{type.name}.cs", reformat(TYPE_ERB.result(binding)))
        end 
      end
    end 

    def escape_reserved_word(word)
      return word unless RESERVED_WORDS.include?(word)
      "#{word}Value"
    end

    def reformat(code)
      Reformatter.new(indent: INDENTATION).reformat(code)
    end

    def get_response_type(type, isTopLevel: true)
      case type.kind
      when "NON_NULL"
        get_response_type(type.of_type);
      when "SCALAR"
        scalars[type.name].nullable_type
      when 'LIST'
        # in C# lists cannot be built out of non-null types because the list is already nullable
        "List<#{get_response_type(type.of_type.unwrap_non_null, isTopLevel: false)}>"
      when 'ENUM'
        "#{type.name}?"
      when 'OBJECT', 'INTERFACE'
        type.classify_name
      else
        raise NotImplementedError, "Unhandled #{type.kind} response type"
      end
    end

    # will return a C# type from a GraphQL type
    def get_arg_type(type, is_non_null: false)
      case type.kind
      when "NON_NULL"
        get_arg_type(type.of_type, is_non_null: true);
      when "SCALAR"
        is_non_null ? scalars[type.name].non_nullable_type : scalars[type.name].nullable_type
      when 'LIST'
        "List<#{get_arg_type(type.of_type)}>"
      when 'ENUM'
        is_non_null ? type.name : "#{type.name}?"
      when 'INPUT_OBJECT'
        type.classify_name
      else
        raise NotImplementedError, "Unhandled #{type.kind} input type"
      end
    end

    # will return an arg definition from a graphql type
    def get_arg_type_and_name(arg)
      type = get_arg_type(arg.type)

      arg_string = "#{type} #{escape_reserved_word(arg.name)}"
      arg_string << " = null" unless arg.type.non_null?
      
      arg_string
    end

    def get_field_args(field)
      # we want to setup arguments for queries here
      args = []
  
      if field.type.subfields?
        args << "#{field.type.unwrap.classify_name}Delegate addTo";
      end

      # now we want to setup required args if there are any
      field.required_args.each do |field|
          args << "#{get_arg_type_and_name(field)}"
      end

      # now handle optional args
      field.optional_args.each do |field|
          args << "#{get_arg_type_and_name(field)}"
      end

      args.join(",")
    end
    
    def get_input_args(type)
      # we want to setup arguments for queries here
      args = []

      # now we want to setup required args if there are any
      type.required_input_fields.each do |field|
          args << "#{get_arg_type_and_name(field)}"
      end

      # now handle optional args
      type.optional_input_fields.each do |field|
          args << "#{get_arg_type_and_name(field)}"
      end

      args.join(",")
    end

    def get_response_init_object_interface(field)
      type = field.type.unwrap_non_null

      "_#{field.name} = new #{type.classify_name}((Dictionary<string,object>) GetJSON(\"#{field.name}\"));"
    end

    def get_response_init_scalar(field)
      type = field.type.unwrap_non_null

      "_#{field.name} = (#{get_response_type(type)}) GetJSON(\"#{field.name}\");"
    end

    def get_response_init_enum(field)
      type = field.type.unwrap_non_null

      "try {\n" \
      "   _#{field.name} = (#{field.type.unwrap_non_null.classify_name}) Enum.Parse(typeof(#{field.type.unwrap_non_null.classify_name}), (string) GetJSON(\"#{field.name}\"));\n" \
      "} catch(ArgumentException) {\n" \
      "   _#{field.name} = #{field.type.unwrap_non_null.classify_name}.UNKNOWN;\n" \
      "}\n"
    end

    def get_response_inits(type)
      out = ""

      type.fields.each do |field|
        type = field.type.unwrap_non_null

        out << "if (GetJSON(\"#{field.name}\") != null) {\n" \

        case type.kind
        when "INTERFACE", "OBJECT"     
          out << "    #{get_response_init_object_interface(field)}\n"
        when "LIST"
          out << "    // TODO: this is a list\n"
        when "ENUM"
          out << "    #{get_response_init_enum(field)}\n"
        when "SCALAR"
          out << "    #{get_response_init_scalar(field)}\n"
        else
          raise NotImplementedError, "Unhandled #{type.kind} init for type"
        end

        out << "}\n\n"               
      end

      out
    end
  end
end
