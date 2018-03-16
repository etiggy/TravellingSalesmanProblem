using System.Collections.Generic;
using System.Linq;

namespace GraphLibrary.Model
{
    //Class representation of a node in a graph
    internal class Node<TEdge, TNode>
    {
        //Public properties with private setters
        internal string nameOfNode { get; private set; }
        internal TNode valueOfNode { get; private set; }

        //Collection of directional edges starting out from the node
        internal List<Edge<TEdge, TNode>> listOfEdges { get; private set; }

        //Default constructor with node name and node value parameter
        public Node(string nameOfNode, TNode valueOfNode)
        {
            listOfEdges = new List<Edge<TEdge, TNode>>();
            this.nameOfNode = nameOfNode;
            this.valueOfNode = valueOfNode;
        }

        //Chained constructor with node name parameter only 
        public Node(string nameOfNode) : this(nameOfNode, default(TNode)) { }

        //Method to add a directional edge to a node without edge weight parameter
        internal void ConnectTo(Node<TEdge, TNode> nodeB)
        {
            ConnectTo(nodeB, default(TEdge));
        }

        //Method to add a directional edge to a node with edge weight parameter
        internal void ConnectTo(Node<TEdge, TNode> nodeB, TEdge weightOfEdge)
        {
            listOfEdges.Add(new Edge<TEdge, TNode>(nodeB, weightOfEdge));
        }

        //Method with boolean return value to check if a node is connected to another node specified in the parameter
        internal bool IsConnectedTo(Node<TEdge, TNode> nodeB)
        {
            return (listOfEdges.Find(m => m.nodeB == nodeB) != null) ? true : false;
        }
    }
}
