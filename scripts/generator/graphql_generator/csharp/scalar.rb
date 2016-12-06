module GraphQLGenerator
  class CSharp
    class Scalar
      attr_reader :csharp_type, :graph_type, :imports

      def initialize(graph_type:, csharp_type:, imports: [])
        @graph_type = graph_type
        @csharp_type = csharp_type
        @imports = imports
      end
    end
  end
end
