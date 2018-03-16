namespace GraphLibrary.Model
{
    //Class representation of an edge between nodes in a graph
    internal class Edge<TEdge, TNode>
    {
        //Public properties with private setters
        internal TEdge weightOfEdge { get; private set; }

        //Reference to the node the edge point towards to
        internal Node<TEdge, TNode> nodeB { get; private set; }

        //Default constructor with node to connect to and edge weight parameter
        public Edge(Node<TEdge, TNode> nodeB, TEdge weightOfEdge)
        {
            this.nodeB = nodeB;
            this.weightOfEdge = weightOfEdge;
        }

        //Chained constructor with node to connect parameter only
        public Edge(Node<TEdge, TNode> nodeB) : this(nodeB, default(TEdge)) { }
    }
}
