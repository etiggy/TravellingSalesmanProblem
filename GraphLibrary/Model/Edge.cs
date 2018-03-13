namespace GraphLibrary.Model
{
    //Class representation of an edge between nodes in a graph
    internal class Edge
    {
        //Public properties with private setters
        internal object weightOfEdge { get; private set; }

        //Reference to the node the edge point towards to
        internal Node nodeB { get; private set; }

        //Default constructor with node to connect to and edge weight parameter
        public Edge(Node nodeB, object weightOfEdge)
        {
            this.nodeB = nodeB;
            this.weightOfEdge = weightOfEdge;
        }

        //Chained constructor with node to connect parameter only
        public Edge(Node nodeB) : this(nodeB, 0) { }
    }
}
