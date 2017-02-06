module GraphQLGenerator
  class CSharp
    class Scalar
      attr_reader :csharp_type, :graph_type

      WRAPPER_OBJECT = {
        'int' => 'int?',
        'long' => 'long?',
        'double' => 'double?',
        'bool' => 'bool?',
      }
      WRAPPER_OBJECT.default_proc = ->(_, key) { key }

      def initialize(graph_type:, csharp_type:, custom_cast: nil)
        @graph_type = graph_type
        @csharp_type = csharp_type
        @custom_cast = custom_cast
      end

      def nullable_type
        WRAPPER_OBJECT[@csharp_type]
      end

      def non_nullable_type
        @csharp_type
      end

      def cast_value(value, is_non_null = true)
        if @custom_cast
          @custom_cast.call(value)
        elsif is_non_null
          "(#{non_nullable_type}) #{value}"
        else
          "(#{nullable_type}) #{value}"
        end
      end
    end
  end
end
