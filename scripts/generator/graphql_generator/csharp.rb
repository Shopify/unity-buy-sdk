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
          nullable_csharp_type: 'string',
        );
      end
    end
    
    ROOT_ERB = erb_for(File.expand_path("../csharp/root.cs.erb", __FILE__))
    TYPE_ERB = erb_for(File.expand_path("../csharp/type.cs.erb", __FILE__))
    ARGUMENTS_ERB = erb_for(File.expand_path("../csharp/arguments.cs.erb", __FILE__))
    INPUT_BASE_ERB = erb_for(File.expand_path("../csharp/input_base.cs.erb", __FILE__))
    INPUT_VALUE_TO_STRING = erb_for(File.expand_path("../csharp/input_value_to_string.cs.erb", __FILE__))
    QUERY_BASE = erb_for(File.expand_path("../csharp/query_base.cs.erb", __FILE__))

    INDENTATION = " " * 4

    RESERVED_WORDS = [
      "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out",  "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string","struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "query"
    ]

    BUILTIN_SCALARS = [
      Scalar.new(
        graph_type: 'Int',
        csharp_type: 'long',
        nullable_csharp_type: 'long?',
      ),
      Scalar.new(
        graph_type: 'Float',
        csharp_type: 'double',
        nullable_csharp_type: 'double?',
      ),
      Scalar.new(
        graph_type: 'String',
        csharp_type: 'string',
        nullable_csharp_type: 'string',
      ),
      Scalar.new(
        graph_type: 'Boolean',
        csharp_type: 'bool',
        nullable_csharp_type: 'bool?',
      ),
      Scalar.new(
        graph_type: 'ID',
        csharp_type: 'string',
        nullable_csharp_type: 'string',
      ),
    ]
    

    def save(path)
      path_types = "#{path}/Types"

      begin
        Dir.mkdir path_types
      rescue Errno::EEXIST
      end

      # output root file
      File.write("#{path}/Root.cs", reformat(ROOT_ERB.result(binding)))

      # output base classes
      File.write("#{path}/Arguments.cs", reformat(ARGUMENTS_ERB.result(binding)))
      File.write("#{path}/InputBase.cs", reformat(INPUT_BASE_ERB.result(binding)))
      File.write("#{path}/InputValueToString.cs", reformat(INPUT_VALUE_TO_STRING.result(binding)))
      File.write("#{path}/QueryBase.cs", reformat(QUERY_BASE.result(binding)))

      schema.types.reject{ |type| type.builtin? || type.scalar? }.each do |type|
        if type.object?
          File.write("#{path_types}/#{type.name}Query.cs", generate_type(type))
        else
          File.write("#{path_types}/#{type.name}.cs", generate_type(type))
        end 
      end
    end 

    def generate_type(type)
      reformat(TYPE_ERB.result(binding))
    end 

    def escape_reserved_word(word)
      return word unless RESERVED_WORDS.include?(word)
      "#{word}Value"
    end

    def reformat(code)
      Reformatter.new(indent: INDENTATION).reformat(code)
    end

    # will return a C# type from a GraphQL type
    def get_arg_type(type, is_non_null: false)
      case type.kind
      when "NON_NULL"
        get_arg_type(type.of_type, is_non_null: true);
      when "SCALAR"
        is_non_null ? scalars[type.name].csharp_type : scalars[type.name].nullable_csharp_type
      when 'LIST'
        # in C# lists cannot be built out of non-null types because the list is already nullable
        "List<#{get_arg_type(type.of_type.unwrap_non_null, is_non_null: true)}>"
      when 'ENUM'
        is_non_null ? type.name : "#{type.name}?"
      when 'INPUT_OBJECT'
        type.classify_name
      else
        raise NotImplementedError, "Unhandled #{type.kind} input type"
      end
    end

    # will return an arg definition from a graphql type
    def get_arg_type_and_name(hasArgs, arg)
      type = get_arg_type(arg.type)

      if arg.type.non_null?
        arg_string = "#{type} #{escape_reserved_word(arg.name)}"
      else
        arg_string = "#{type} #{escape_reserved_word(arg.name)} = null"
      end

      arg_string.prepend(', ') if hasArgs

      arg_string
    end

    def get_field_args(field)
      # we want to setup arguments for queries here
      args = ""
      hasArgs = false

      if field.type.subfields?
        args = "#{args}#{field.type.unwrap.classify_name}Delegate addTo";
        hasArgs = true
      end

      # now we want to setup required args if there are any
      field.required_args.each do |field|
          args = "#{args}#{get_arg_type_and_name(hasArgs, field)}"
          hasArgs = true
      end

      # now handle optional args
      field.optional_args.each do |field|
          args = "#{args}#{get_arg_type_and_name(hasArgs, field)}"
          hasArgs = true
      end

      args
    end
    
    def get_input_args(type)
      # we want to setup arguments for queries here
      args = ""
      hasArgs = false

      # now we want to setup required args if there are any
      type.required_input_fields.each do |field|
          args = "#{args}#{get_arg_type_and_name(hasArgs, field)}"
          hasArgs = true
      end

      # now handle optional args
      type.optional_input_fields.each do |field|
          args = "#{args}#{get_arg_type_and_name(hasArgs, field)}"
          hasArgs = true
      end

      args
    end
  end
end
