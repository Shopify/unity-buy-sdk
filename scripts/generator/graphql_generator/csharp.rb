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
    
    TYPE_ERB = erb_for(File.expand_path("../csharp/type.cs.erb", __FILE__))
    TYPE_RESPONSE_ERB = erb_for(File.expand_path("../csharp/type_response.cs.erb", __FILE__))

    INDENTATION = " " * 4
    ALIAS_SEPARATOR = "___"

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
      path_sdk = "#{path}/SDK"

      begin
        Dir.mkdir path_types
      rescue Errno::EEXIST
      end

      begin
        Dir.mkdir path_sdk
      rescue Errno::EEXIST
      end

      # output classes on root
      %w(
        Root
        Arguments
        InputBase
        InputValueToString
        AbstractResponse
        CastUtils
        ValidationUtils
        NoQueryException
        InvalidServerResponseException
        AliasException
        SDK/ShopifyBuy
        SDK/DefaultQueries
        SDK/QueryLoader
        SDK/ConnectionLoader
        SDK/UnityLoader
        SDK/TopLevelResponse
        SDK/MutationResponse
        SDK/QueryResponse
        SDK/ILoader
      ).each do |class_file_name|
        erb = CSharp::erb_for(File.expand_path("../csharp/#{class_file_name}.cs.erb", __FILE__))
        File.write("#{path}/#{class_file_name}.cs", reformat(erb.result(binding)))
      end

      # output type definitions
      schema.types.reject{ |type| type.builtin? || type.scalar? }.each do |type|
        # output 
        if type.object? || type.interface?
          File.write("#{path_types}/#{type.name}Query.cs", reformat(TYPE_ERB.result(binding)))
          File.write("#{path_types}/#{type.name}.cs", reformat(TYPE_RESPONSE_ERB.result(binding)))
        elsif type.input_object? || type.kind == 'ENUM'
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

    def graph_type_to_csharp_type(type, is_non_null: false)
      case type.kind
      when "NON_NULL"
        graph_type_to_csharp_type(type.of_type, is_non_null: true);
      when "SCALAR"
        is_non_null ? scalars[type.name].non_nullable_type : scalars[type.name].nullable_type
      when 'LIST'
        "List<#{graph_type_to_csharp_type(type.of_type)}>"
      when 'ENUM'
        is_non_null ? type.name : "#{type.name}?"
      when 'INPUT_OBJECT', 'OBJECT', 'INTERFACE'
        type.classify_name
      else
        raise NotImplementedError, "Unhandled #{type.kind} input type"
      end
    end

    def graph_type_to_csharp_cast(type, value, is_non_null = false)
      case type.kind
      when "NON_NULL"
        graph_type_to_csharp_cast(type.of_type, value, is_non_null: true);
      when "SCALAR"
        scalars[type.name].cast_value(value)
      else
        "(#{graph_type_to_csharp_type(type)})"
      end
    end

    # will return an arg definition from a graphql type
    def arg_type_and_name(arg)
      type = graph_type_to_csharp_type(arg.type)

      arg_string = "#{type} #{escape_reserved_word(arg.name)}"
      arg_string << " = null" unless arg.type.non_null?
      
      arg_string
    end

    def field_args(field)
      # we want to setup arguments for queries here
      args = []
  
      if field.type.subfields?
        args << "#{field.type.unwrap.classify_name}Delegate addTo";
      end

      # now we want to setup required args if there are any
      field.required_args.each do |field|
          args << "#{arg_type_and_name(field)}"
      end

      # now handle optional args
      field.optional_args.each do |field|
          args << "#{arg_type_and_name(field)}"
      end

      if field.args.any?
        args << 'string alias = null'
      end

      args.join(",")
    end
    
    def input_args(type)
      # we want to setup arguments for queries here
      args = []

      # now we want to setup required args if there are any
      type.required_input_fields.each do |field|
          args << "#{arg_type_and_name(field)}"
      end

      # now handle optional args
      type.optional_input_fields.each do |field|
          args << "#{arg_type_and_name(field)}"
      end

      args.join(",")
    end

    def response_init_object(field)
      type = field.type.unwrap_non_null

      "new #{type.classify_name}((Dictionary<string,object>) dataJSON[key])"
    end

    def response_init_interface(field)
      type = field.type.unwrap_non_null

      "Unknown#{type.classify_name}.Create((Dictionary<string,object>) dataJSON[key])"
    end

    def response_init_scalar(field)
      graph_type_to_csharp_cast(field.type, "dataJSON[key]")
    end

    def response_init_enum(field)
      type = field.type.unwrap_non_null
      enum_type_name = field.type.unwrap_non_null.classify_name

      "CastUtils.GetEnumValue<#{enum_type_name}>(dataJSON[key])"
    end

    def response_init_list(field)
      type = field.type.unwrap_non_null

      "CastUtils.CastList<List<#{graph_type_to_csharp_type(type.of_type)}>>((IList) dataJSON[key])"
    end

    def type_from_name(type_name)
      schema.types.find { |type| type.name == type_name }
    end

    def field_from_type(type, field_name)
      type.fields.find { |field| field.name == field_name }
    end

    def connection?(type)
      type.name.end_with?("Connection")
    end

    def edges_type_from_connection_type(type)
      if connection?(type)
        edges = field_from_type(type, "edges")

        type_from_name(edges.type.unwrap.name)
      end
    end

    def node_type_from_connection_type(type)
      if connection?(type)
        edge_type = edges_type_from_connection_type(type)
        
        type_from_name(edge_type.name)

        node = field_from_type(edge_type, "node")

        type_from_name(node.type.unwrap.name)
      end
    end
  end
end
