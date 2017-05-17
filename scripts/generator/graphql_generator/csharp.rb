require 'erb'
require 'ostruct'
require 'fileutils'
require_relative './csharp/scalar'
require_relative './reformatter'

module GraphQLGenerator
  UNKNOWN_ENUM = OpenStruct.new
  UNKNOWN_ENUM.name = "UNKNOWN"
  UNKNOWN_ENUM.description = "If the SDK is not up to date with the schema in the Storefront API, it is possible\n" \
                             "to have enum values returned that are unknown to the SDK. In this case the value\n" \
                             "will actually be UNKNOWN."

  class CSharp
    class << self
      def erb_for(template_filename)
        erb = ERB.new(File.read(template_filename))
        erb.filename = template_filename
        erb
      end
    end

    attr_reader :scalars, :schema, :namespace, :version

    def initialize(schema, namespace:, custom_scalars: [], script_name: $0)
      @schema = schema
      @namespace = namespace
      @script_name = script_name

      @version = File.read(File.expand_path("../../../version", __FILE__)).strip

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
      path_graphql = "#{path}/GraphQL"
    
      begin
        Dir.mkdir(path_graphql)
      rescue Errno::EEXIST
      end

      Dir["#{path}/**/*.cs"].reject{ |f| f[%r{Vendor/}] }.each do |file|
        FileUtils.rm(file)
      end

      %w(
        ShopifyBuy
        Cart
        SDK/Arguments
        SDK/InputBase
        SDK/InputValueToString
        SDK/CastUtils
        SDK/CartLineItems
        SDK/ValidationUtils
        SDK/AbstractResponse
        SDK/NoQueryException
        SDK/ObservableDictionary
        SDK/AliasException
        SDK/InvalidServerResponseException
        SDK/DefaultQueries
        SDK/DefaultQueriesProducts
        SDK/DefaultQueriesCollections
        SDK/DefaultQueriesCheckout
        SDK/QueryLoader
        SDK/ConnectionLoader
        SDK/UnityLoader
        SDK/TopLevelResponse
        SDK/MutationResponse
        SDK/QueryResponse
        SDK/ILoader
        SDK/Log
      ).each do |class_file_name|
        directory = "#{path}/#{File.dirname(class_file_name)}"

        unless File.directory?(directory)
          Dir.mkdir(directory)
        end

        erb = CSharp::erb_for(File.expand_path("../csharp/#{class_file_name}.cs.erb", __FILE__))
        File.write("#{path}/#{class_file_name}.cs", reformat(erb.result(binding)))
      end

      # output type definitions
      schema.types.reject{ |type| type.builtin? || type.scalar? }.each do |type|
        # output
        if type.object? || type.interface?
          File.write("#{path_graphql}/#{type.name}Query.cs", reformat(TYPE_ERB.result(binding)))
          File.write("#{path}/#{type.name}.cs", reformat(TYPE_RESPONSE_ERB.result(binding)))
        elsif type.input_object? || type.kind == 'ENUM'
          File.write("#{path}/#{type.name}.cs", reformat(TYPE_ERB.result(binding)))
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
        "(#{graph_type_to_csharp_type(type)}) #{value}"
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
        args << "#{field.type.unwrap.classify_name}Delegate buildQuery";
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

    def enum_values(type)
      type.enum_values.clone.unshift(UNKNOWN_ENUM).map{ |enum| "#{deprecation_doc_for(enum)}#{summary_doc(enum.description)}\n#{enum.name}" }.join(",\n")
    end

    def pretty_doc(documentation)
      "/// #{documentation.split("\n").map{|part| part.strip }.join("\n/// ")}"
    end

    def deprecation_doc_for(item)
      "/// \\deprecated #{item.deprecation_reason}\n" if item.deprecated?
    end

    def summary_doc(description)
      if description
        "/// <summary>\n" \
        "#{pretty_doc(description)}\n" \
        "/// </summary>"
      else
        ""
      end
    end

    def param_doc(param_name, param_description)
      if param_description
        return "/// <param name=\"#{param_name}\">\n" \
               "#{pretty_doc(param_description)}\n" \
               "/// </param>"
      else
        return ""
      end
    end

    def params_doc(arguments)
      arguments.reject{|arg| arg.description == nil}.map{|arg| param_doc(arg.name, arg.description)}.join("\n")
    end

    def docs_query_object(type)
      if schema.root_name?(type.name)
        if type.name == schema.query_root_name
          return summary_doc(
            "<see cref=\"#{type.classify_name}Query\" /> is the root query builder. All Storefront API queries" \
            " are built off of <see cref=\"#{type.classify_name}Query\" />."
          )
        else
          return summary_doc(
            "<see cref=\"#{type.classify_name}Query\" /> is the root mutation builder. All Storefront API mutation queries" \
            " are built off of <see cref=\"#{type.classify_name}Query\" />."
          )
        end
      else
        return summary_doc(type.description)
      end
    end

    def docs_possible_types(type)
      return "<list type=\"bullet\">\n#{type.possible_types.map{|type| "<item><description><see cref=\"#{type.classify_name}\" /></description></item>" }.join("\n")}\n</list>"
    end

    def docs_response_object(type)
      if type.interface?
        return summary_doc(
          "#{"Unknown" if type.interface?}#{type.classify_name} is a response object.\n" \
          "With <see cref=\"#{"Unknown" if type.interface?}#{type.classify_name}.Create\" /> you'll be able instantiate objects implementing #{type.classify_name}.\n" \
          "<c>#{"Unknown" if type.interface?}#{type.classify_name}.Create</c> will return one of the following types:\n#{docs_possible_types(type)}"
        )
      elsif connection?(type)
        if type.description
          return summary_doc("#{type.description}. #{type.classify_name} can be cast to <c>List<#{node_type_from_connection_type(type).classify_name}></c>.")
        else
          return summary_doc("#{type.classify_name} is a response object. #{type.classify_name} can be cast to <c>List<#{node_type_from_connection_type(type).classify_name}></c>.")
        end
      else
        if type.description
          return summary_doc(type.description)
        else
          return summary_doc("#{type.classify_name} is a response object.")
        end
      end
    end

    def docs_input_object(type)
      if type.description
        return summary_doc(type.description)
      else
        return summary_doc("#{type.classify_name} is an input object.")
      end
    end

    def docs_input_field(field)
      return summary_doc(field.description)
    end

    def docs_enum(type)
      return summary_doc(type.description)
    end

    def docs_interface(type)
      return summary_doc(type.description)
    end

    def docs_query_field(field)
      params_doc = params_doc(field.args);

      if field.description
        return "#{deprecation_doc_for(field)}#{summary_doc(field.description)}\n#{params_doc}"
      elsif params_doc != ""
        return "#{deprecation_doc_for(field)}#{params_doc}"
      else
        return "#{deprecation_doc_for(field)}"
      end
    end

    def docs_response_field(field)
      if field.args.any?
        alias_doc = param_doc("alias", "If the original field queried was queried using an alias, then pass the matching string.")
      else
        alias_doc = ""
      end

      if field.description
        return "#{deprecation_doc_for(field)}#{summary_doc(field.description)}\n#{alias_doc}"
      else
        return "#{deprecation_doc_for(field)}#{alias_doc}"
      end
    end
  end
end
