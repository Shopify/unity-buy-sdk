require 'erb'

require_relative './reformatter'

module GraphQLGenerator
  class Base
    class << self
      def erb_for(template_filename)
        erb = ERB.new(File.read(template_filename))
        erb.filename = template_filename
        erb
      end
    end

    INDENTATION = " " * 2

    attr_reader :schema_name, :schema

    def initialize(schema, nest_under:, script_name: $0)
      @schema_name = nest_under
      @schema = schema
      @script_name = script_name
    end

    def reformat(code)
      Reformatter.new(indent: self::class::INDENTATION).reformat(code)
    end

    def generator_script
      @script_name
    end
  end
end
