module GraphQLGenerator
  class CSharp
    class Scalar
      attr_reader :csharp_type, :nullable_csharp_type, :graph_type, :imports

      def initialize(graph_type:, csharp_type:, nullable_csharp_type:)
        @graph_type = graph_type
        @csharp_type = csharp_type
        @nullable_csharp_type = nullable_csharp_type
      end
    end
  end
end
