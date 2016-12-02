require_relative './base.rb'
require_relative './csharp/scalar'

module GraphQLGenerator
  class CSharp < Base
    attr_reader :scalars

    def initialize(schema, custom_scalars: [], **options)
      super(schema, **options)

      @scalars = (BUILTIN_SCALARS + custom_scalars).reduce({}) { |hash, scalar| hash[scalar.graph_type] = scalar; hash }
    end

    ROOT_ERB = erb_for(File.expand_path("../csharp/root.cs.erb", __FILE__))
    TYPE_ERB = erb_for(File.expand_path("../csharp/type.cs.erb", __FILE__))
    ID_ERB = erb_for(File.expand_path("../csharp/id.cs.erb", __FILE__))
    ARGUMENTS_ERB = erb_for(File.expand_path("../csharp/arguments.cs.erb", __FILE__))
    INPUT_BASE_ERB = erb_for(File.expand_path("../csharp/input_base.cs.erb", __FILE__))
    INPUT_VALUE_TO_STRING = erb_for(File.expand_path("../csharp/input_value_to_string.cs.erb", __FILE__))
    QUERY_BASE = erb_for(File.expand_path("../csharp/query_base.cs.erb", __FILE__))

    IDENTATION = " " * 4

    RESERVED_WORDS = [
      "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out",  "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string","struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "query"
    ]

    BUILTIN_SCALARS = [
      Scalar.new(
        graph_type: 'Int',
        csharp_type: 'int',
        deserialize_expr: ->(expr) { "jsonAsInteger(#{expr}, key)" },
      ),
      Scalar.new(
        graph_type: 'Float',
        csharp_type: 'double',
        deserialize_expr: ->(expr) { "jsonAsDouble(#{expr}, key)" },
      ),
      Scalar.new(
        graph_type: 'String',
        csharp_type: 'string',
        deserialize_expr: ->(expr) { "jsonAsString(#{expr}, key)" },
      ),
      Scalar.new(
        graph_type: 'Boolean',
        csharp_type: 'bool',
        deserialize_expr: ->(expr) { "jsonAsBoolean(#{expr}, key)" },
      ),
      Scalar.new(
        graph_type: 'ID',
        csharp_type: 'ID',
        deserialize_expr: ->(expr) { "new ID(jsonAsString(#{expr}, key))" },
        imports: ['com.shopify.mobile.lib.GraphQL.ID'],
      ),
    ]

    def save(path)
      path_types = "#{path}/Types"

      # create the Types directory if it doesn't exist
      begin
        Dir.mkdir path_types
      rescue
      end

      # output root file
      File.write("#{path}/Root.cs", reformat(ROOT_ERB.result(binding)))

      # output base classes
      File.write("#{path}/Arguments.cs", reformat(ARGUMENTS_ERB.result(binding)))
      File.write("#{path}/InputBase.cs", reformat(INPUT_BASE_ERB.result(binding)))
      File.write("#{path}/InputValueToString.cs", reformat(INPUT_VALUE_TO_STRING.result(binding)))
      File.write("#{path}/QueryBase.cs", reformat(QUERY_BASE.result(binding)))


      # output the id type
      File.write("#{path_types}/ID.cs", reformat(ID_ERB.result(binding)))

      schema.types.reject{ |type| type.name.start_with?('__') || type.scalar? }.each do |type|
        if type.object? then
          File.write("#{path_types}/#{type.name}Query.cs", generate_type(type))
        elsif type.input_object? then
          File.write("#{path_types}/#{type.name}Input.cs", generate_type(type))
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

    # will return a C# type from a GraphQL type
    def get_arg_type(type, isOptional=false)
        type = type.unwrap_non_null
        outType = ""

        case type.kind
        when "SCALAR"
          if isOptional && type.name != "String" && type.name != "ID" then
            outType = "#{scalars[type.name].csharp_type}?"
          else
            outType = scalars[type.name].csharp_type
          end
        when 'LIST'
          outType = "List<#{get_arg_type(type.of_type)}>"
        when 'ENUM'
          if isOptional then
            outType = "#{type.name}?"
          else
            outType = type.name
          end
        when 'OBJECT', 'INTERFACE', 'UNION'
          outType = type.name
        when 'INPUT_OBJECT'
          outType = "#{type.classify_name}Input"
        else
          raise NotImplementedError, "Unhandled #{type.kind} response type"
        end

        outType
    end

    # will return an arg definition from a graphql type
    def get_arg_type_and_name(hasArgs, arg, isOptional=false)
        type = get_arg_type(arg.type, isOptional)

        if isOptional then
            arg_string = "#{type} #{escape_reserved_word(arg.name)} = null"
        else
            arg_string = "#{type} #{escape_reserved_word(arg.name)}"
        end

        arg_string.prepend(', ') if hasArgs

        arg_string
    end
  end
end
