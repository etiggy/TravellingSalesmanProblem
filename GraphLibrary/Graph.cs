using GraphLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphLibrary
{
    //Class representation of a graph
    public class Graph<TEdge, TNode>
    {
        //Constants to store error messages
        private const string noRouteExistString = "No route exist";
        private const string invalidDataTypeString = "Invalid data type";
        private const string noDataTypeString = "Nodes don't exist";

        //Collection of all the nodes in the graph
        private List<Node<TEdge, TNode>> listOfNodes { get; set; }

        //KeyValue pair variables declared as dynamic to store minimum and maximum result for route weighting methods  
        private dynamic minValue;
        private dynamic maxValue;

        //Properties to store initialisation parameters for traversal
        private string[] nodeNameArray { get; set; }
        private bool preserveNodeOrder { get; set; }
        private bool orderByAscending { get; set; }
        private GetRouteBy goalOfTraversal { get; set; }

        //Default constructor to create graph
        public Graph()
        {
            listOfNodes = new List<Node<TEdge, TNode>>();
        }

        //Method to create a node in the graph with node name and node value parameter. Checks before creation for node name clashes to 
        //circumvent duplication.
        //Returns boolean true on success and false on failure.
        public bool CreateNode(string nameOfNode, TNode valueOfNode)
        {
            bool createdOK = false;

            if (GetNode(nameOfNode) == null)
            {
                listOfNodes.Add(new Node<TEdge, TNode>(nameOfNode, valueOfNode));
                createdOK = true;
            }

            return createdOK;
        }

        //Overload for method to create a node in the graph with node name parameter. Checks before creation for node name clashes to circumvent 
        //duplication.
        //Returns boolean true on success and false on failure.
        public bool CreateNode(string nameOfNode)
        {
            return CreateNode(nameOfNode, default(TNode));
        }

        //Overload for method to create a node in the graph. Extra parameters to add name and value to nodes in an array of object format.
        //Returns boolean true on success and false on failure.
        public bool CreateNode(object[] nodeData)
        {
            try
            {
                switch (nodeData.Count())
                {
                    case 1:
                        return CreateNode((string)nodeData[0]);
                    case 2:
                        return CreateNode((string)nodeData[0], (TNode)nodeData[1]);
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Internal method to return reference of node from a custom list of nodes based on node name parameter 
        private Node<TEdge, TNode> GetNode(string nameOfNode, List<Node<TEdge, TNode>> listOfNode)
        {
            return listOfNode.Find(m => m.nameOfNode.Equals(nameOfNode));
        }

        //Overload for internal method to return reference of node from the graph's listofnodes collection based on a node name parameter
        private Node<TEdge, TNode> GetNode(string nameOfNode)
        {
            return GetNode(nameOfNode, listOfNodes);
        }

        //Method to connect two nodes with an edge. Extra parameters to add value to nodes, weight to edge and to set up bi-directional edges.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(string nameOfNodeA, TNode valueOfNodeA, string nameOfNodeB, TNode valueOfNodeB, TEdge weightOfEdge, bool biDirectionalEdge)
        {
            bool connectionSuccessful = false;

            CreateNode(nameOfNodeA, valueOfNodeA);
            Node<TEdge, TNode> nodeA = GetNode(nameOfNodeA);

            CreateNode(nameOfNodeB, valueOfNodeB);
            Node<TEdge, TNode> nodeB = GetNode(nameOfNodeB);

            if (biDirectionalEdge)
            {
                if (!nodeA.IsConnectedTo(nodeB) && !nodeB.IsConnectedTo(nodeB))
                {
                    nodeA.ConnectTo(nodeB, weightOfEdge);
                    nodeB.ConnectTo(nodeA, weightOfEdge);
                    connectionSuccessful = true;
                }
            }
            else
            {
                if (!nodeA.IsConnectedTo(nodeB))
                {
                    nodeA.ConnectTo(nodeB, weightOfEdge);
                    connectionSuccessful = true;
                }
            }

            return connectionSuccessful;
        }

        //Overload for method to connect two nodes with a directional edge. Extra parameter to add weight to edge.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(string nameOfNodeA, string nameOfNodeB, TEdge weightOfEdge)
        {
            return ConnectNodes(nameOfNodeA, default(TNode), nameOfNodeB, default(TNode), weightOfEdge, false);
        }

        //Overload for method to connect two nodes with a bi-directional edge. Extra parameter to add weight to edge and set edge mode to bi-directional.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(string nameOfNodeA, string nameOfNodeB, TEdge weightOfEdge, bool biDirectionalEdge)
        {
            return ConnectNodes(nameOfNodeA, default(TNode), nameOfNodeB, default(TNode), weightOfEdge, biDirectionalEdge);
        }

        //Overload for method to connect two nodes with a directional edge. Extra parameter to add value to nodes and weight to edge.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(string nameOfNodeA, TNode valueOfNodeA, string nameOfNodeB, TNode valueOfNodeB, TEdge weightOfEdge)
        {
            return ConnectNodes(nameOfNodeA, valueOfNodeA, nameOfNodeB, valueOfNodeB, weightOfEdge, false);
        }

        //Overload for method to connect two nodes with an edge. Extra parameters to add value to nodes and weight to an optionally bidirectional edge 
        //in an array of object format.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(object[] nodeData)
        {
            try
            {
                switch (nodeData.Count())
                {
                    case 3:
                        return ConnectNodes((string)nodeData[0], (string)nodeData[1], (TEdge)nodeData[2]);
                    case 4:
                        return ConnectNodes((string)nodeData[0], (string)nodeData[1], (TEdge)nodeData[2], (bool)nodeData[3]);
                    case 5:
                        return ConnectNodes((string)nodeData[0], (TNode)nodeData[1], (string)nodeData[2], (TNode)nodeData[3], (TEdge)nodeData[4]);
                    case 6:
                        return ConnectNodes((string)nodeData[0], (TNode)nodeData[1], (string)nodeData[2], (TNode)nodeData[3], (TEdge)nodeData[4], (bool)nodeData[5]);
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Method to populate graph with nodes and edges through an edge list. Accepts a list of object arrays as edges and batch processes them
        //through the ConnectNodes(object[] nodeData) method.
        //Returns boolean true on success and false on failure.
        public bool LoadEdgeList(List<object[]> edgeList)
        {
            bool loadSuccessful = false;

            foreach (var edge in edgeList)
            {
                loadSuccessful = ConnectNodes(edge);

                if (!loadSuccessful)
                {
                    listOfNodes.Clear();
                    break;
                }
            }

            return loadSuccessful;
        }

        //Method to initiate the traversal of the graph. Accepts string array as parameter with list of node names to include in the path search, 
        //boolean variable to flag if order of intermediary nodes need to be preserved, boolean variable to note if sorting should be ascending or
        //descending, a GetRouteBy enum value to decide on the specific route weighting to be used.
        //The first item from the array is selected as the starting point and the last as the destination. 
        //Returns a KeyValue pair defined as a dynamic variable.
        public dynamic GetRoute(string[] nodeNameArray, bool preserveNodeOrder, bool orderByAscending, GetRouteBy currentGoal)
        {
            this.nodeNameArray = nodeNameArray;
            this.preserveNodeOrder = preserveNodeOrder;
            this.orderByAscending = orderByAscending;

            goalOfTraversal = currentGoal;


            Node<TEdge, TNode> nodeA = GetNode(nodeNameArray.First());
            Node<TEdge, TNode> nodeB = GetNode(nodeNameArray.Last());

            if (nodeA != null && nodeB != null)
            {
                if (CurrentGoalSetup())
                {
                    List<Node<TEdge, TNode>> nodesOnRoute = new List<Node<TEdge, TNode>>();
                    DepthFirstRecursive(nodesOnRoute, nodeA, nodeB);
                }
                else
                {
                    minValue = maxValue = new KeyValuePair<string, object>(invalidDataTypeString, null);
                }
            }
            else
            {
                minValue = maxValue = new KeyValuePair<string, object>(noDataTypeString, null);
            }

            return orderByAscending ? minValue : maxValue;
        }

        //Method to set up specific types for the minimum and maximum global variables and to run some arithmetic tests to check if the specific type
        //assigned to the generic values of Edge/Nodes has <, > and += operators defined.
        private bool CurrentGoalSetup()
        {
            TEdge edgeType = listOfNodes.Find(m => m.listOfEdges.Count > 0).listOfEdges[0].weightOfEdge;
            TNode nodeType = listOfNodes[0].valueOfNode;

            switch (goalOfTraversal)
            {
                case GetRouteBy.EdgeWeight:
                    {
                        minValue = maxValue = new KeyValuePair<string, TEdge>(noRouteExistString, default(TEdge));

                        return TypeArithmeticTest<TEdge>(edgeType);
                    }
                case GetRouteBy.NodeValue:
                    {
                        minValue = maxValue = new KeyValuePair<string, TNode>(noRouteExistString, default(TNode));

                        return TypeArithmeticTest<TNode>(nodeType);
                    }
                case GetRouteBy.CombinedValue:
                    {
                        minValue = maxValue = new KeyValuePair<string, TNode>(noRouteExistString, default(TNode));

                        return (TypeArithmeticTest<TEdge>(edgeType) && TypeArithmeticTest<TNode>(nodeType));
                    }
                case GetRouteBy.NumberOfNodes:
                    {
                        minValue = maxValue = new KeyValuePair<string, int>(noRouteExistString, default(int));

                        return true;
                    }
                default:
                    return false;
            }
        }

        //Method to run basic arithmetics tests on generics to see if operators <, > and += are defined
        private bool TypeArithmeticTest<T>(dynamic type)
        {
            try
            {
                T value = default(T);

                bool comparison = (value < type) && (value > type);
                value += type;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Recursive method to traverse graph using a depth-first search algorithm. Takes a route tracing list, a starting and a destination 
        //node as parameters. Upon finding a valid router it calls the appropriate method based on the global goalOfTraversal variable using reflection.
        private void DepthFirstRecursive(List<Node<TEdge, TNode>> nodesOnRoute, Node<TEdge, TNode> nodeA, Node<TEdge, TNode> nodeB)
        {
            nodesOnRoute.Add(nodeA);

            foreach (var edge in nodeA.listOfEdges)
            {
                Node<TEdge, TNode> nextNode = edge.nodeB;

                if (!nodesOnRoute.Contains(nextNode))
                {
                    if (nextNode == nodeB)
                    {
                        List<Node<TEdge, TNode>> tempNodesOnRoute = nodesOnRoute.ToList();

                        tempNodesOnRoute.Add(nextNode);

                        bool routeContainsAllIntermediaryNodes = (preserveNodeOrder) ?
                    NodeInclusionInOrder(tempNodesOnRoute) : NodeInclusionNotInOrder(tempNodesOnRoute);

                        if (routeContainsAllIntermediaryNodes)
                        {
                            string methodName = "CalculateRouteValueBy" + goalOfTraversal;
                            MethodInfo methodInfo = this.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                            methodInfo.Invoke(this, new object[] { tempNodesOnRoute });
                        }
                    }
                    else
                    {
                        DepthFirstRecursive(nodesOnRoute, nextNode, nodeB);
                    }
                }
            }

            nodesOnRoute.Remove(nodesOnRoute.Last());
        }

        //Method to summarise the number of nodes in the route passed to it as a parameter and call SetMinAndMaxValue with the result
        private void CalculateRouteValueByNumberOfNodes(List<Node<TEdge, TNode>> currentRoute)
        {
            StringBuilder currentRouteNodes = new StringBuilder();

            for (int i = 0; i < currentRoute.Count; i++)
            {
                Node<TEdge, TNode> currentNode = currentRoute[i];

                currentRouteNodes.Append(currentNode.nameOfNode);

                if (i < currentRoute.Count - 1)
                {
                    currentRouteNodes.Append('-');
                }
            }

            SetMinAndMaxValue<int>(currentRouteNodes.ToString(), currentRoute.Count);
        }

        //Method to summarise the weight of edges and the value of nodes in the route passed to it as a parameter and call SetMinAndMaxValue 
        //with the result
        private void CalculateRouteValueByCombinedValue(List<Node<TEdge, TNode>> currentRoute)
        {
            StringBuilder currentRouteNodes = new StringBuilder();
            dynamic currentRouteEdgeWeights = default(TEdge);
            dynamic currentRouteNodeValues = default(TNode);

            for (int i = 0; i < currentRoute.Count; i++)
            {
                Node<TEdge, TNode> currentNode = currentRoute[i];

                currentRouteNodes.Append(currentNode.nameOfNode);
                currentRouteNodeValues += currentNode.valueOfNode;

                if (i < currentRoute.Count - 1)
                {
                    Node<TEdge, TNode> nextNode = currentRoute[i + 1];

                    currentRouteNodes.Append('-');
                    currentRouteEdgeWeights += currentNode.listOfEdges.Find(m => m.nodeB == nextNode).weightOfEdge;
                }
            }

            SetMinAndMaxValue<TNode>(currentRouteNodes.ToString(), currentRouteNodeValues);
        }

        //Method to summarise the value of nodes in the route passed to it as a parameter and call SetMinAndMaxValue with the result
        private void CalculateRouteValueByNodeValue(List<Node<TEdge, TNode>> currentRoute)
        {
            StringBuilder currentRouteNodes = new StringBuilder();
            dynamic currentRouteNodeValues = default(TNode);

            for (int i = 0; i < currentRoute.Count; i++)
            {
                Node<TEdge, TNode> currentNode = currentRoute[i];

                currentRouteNodes.Append(currentNode.nameOfNode);
                currentRouteNodeValues += currentNode.valueOfNode;

                if (i < currentRoute.Count - 1)
                {
                    currentRouteNodes.Append('-');
                }
            }

            SetMinAndMaxValue<TNode>(currentRouteNodes.ToString(), currentRouteNodeValues);
        }

        //Method to summarise the weight of edges in the route passed to it as a parameter and call SetMinAndMaxValue with the result
        private void CalculateRouteValueByEdgeWeight(List<Node<TEdge, TNode>> currentRoute)
        {
            StringBuilder currentRouteNodes = new StringBuilder();
            dynamic currentRouteEdgeWeights = default(TEdge);

            for (int i = 0; i < currentRoute.Count - 1; i++)
            {
                Node<TEdge, TNode> currentNode = currentRoute[i];
                Node<TEdge, TNode> nextNode = currentRoute[i + 1];

                currentRouteNodes.Append(currentNode.nameOfNode);
                currentRouteNodes.Append('-');
                currentRouteEdgeWeights += currentNode.listOfEdges.Find(m => m.nodeB == nextNode).weightOfEdge;
            }

            currentRouteNodes.Append(currentRoute.Last().nameOfNode);

            //SetMinAndMaxValue<TEdge>(currentRouteNodes.ToString(), currentRouteEdgeWeights);
            SetMinAndMaxValue<TEdge>(currentRouteNodes.ToString(), currentRouteEdgeWeights);
        }

        //Helper method to compare a value in a KeyValue pair to previous min and max results and save it in case it is smaller than min or 
        //bigger than max 
        private void SetMinAndMaxValue<T>(string key, dynamic value)
        {
            if (value < minValue.Value || minValue.Value == default(T))
            {
                minValue = new KeyValuePair<string, T>(key, value);
            }

            if (value > maxValue.Value || maxValue.Value == default(T))
            {
                maxValue = new KeyValuePair<string, T>(key, value);
            }
        }

        //Checks for nodes in the route passed by as a parameter and returns a bool depending on if the route passes the check. In-order verification.
        private bool NodeInclusionInOrder(List<Node<TEdge, TNode>> route)
        {
            int nameOfNodeCharIndexInCurrentRoute = 0;

            foreach (var nameOfNode in nodeNameArray)
            {
                int currentNodeIndex = route.IndexOf(GetNode(nameOfNode, route));

                if (((currentNodeIndex == 0) && (currentNodeIndex == nameOfNodeCharIndexInCurrentRoute))
                    || (currentNodeIndex > nameOfNodeCharIndexInCurrentRoute))
                {
                    nameOfNodeCharIndexInCurrentRoute = currentNodeIndex;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        //Checks for nodes in the route passed by as a parameter and returns a bool depending on if the route passes the check. Not in-order verification.
        private bool NodeInclusionNotInOrder(List<Node<TEdge, TNode>> route)
        {
            foreach (var nameOfNode in nodeNameArray)
            {
                if (GetNode(nameOfNode, route) == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
