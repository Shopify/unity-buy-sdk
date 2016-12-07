module GraphQLGenerator
  class CSharp
    class Scalar
      attr_reader :csharp_type, :graph_type

      WRAPPER_OBJECT = {
        'int' => 'int',
        'long' => 'long?',
        'double' => 'double?',
        'bool' => 'bool?',
      }
      WRAPPER_OBJECT.default_proc = ->(_, key) { key }

      def initialize(graph_type:, csharp_type:)
        @graph_type = graph_type
        @csharp_type = csharp_type
      end

      def nullable_type
        WRAPPER_OBJECT[@csharp_type]
      end

      def non_nullable_type
        @csharp_type
      end
    end
  end
end
