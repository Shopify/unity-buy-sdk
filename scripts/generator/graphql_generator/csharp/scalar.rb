module GraphQLGenerator
  class CSharp
    class Scalar
      attr_reader :csharp_type, :graph_type, :imports

      def initialize(graph_type:, csharp_type:, deserialize_expr:, imports: [])
        @graph_type = graph_type
        @csharp_type = csharp_type
        @deserialize_expr = deserialize_expr
        @imports = imports
      end

      def deserialize(expr)
        @deserialize_expr.call(expr)
      end
    end
  end
end
