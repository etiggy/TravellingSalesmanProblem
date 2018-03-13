using GraphLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLibrary
{
    //Class representation of a graph
    public class Graph
    {
        //Collection of all the nodes in the graph
        private List<Node> listOfNodes { get; set; }

        //Private collection to store all possible routes from onwards the traversal process
        private List<List<Node>> listOfPossibleRoutes { get; set; }

        //Default constructor to create graph
        public Graph()
        {
            listOfNodes = new List<Node>();
        }

        //Method to create a node in the graph with node name and node value parameter. Checks before creation for node name clashes to 
        //circumvent duplication.
        //Returns boolean true on success and false on failure.
        public bool CreateNode(string nameOfNode, object valueOfNode)
        {
            bool createdOK = false;

            if (GetNode(nameOfNode) == null)
            {
                listOfNodes.Add(new Node(nameOfNode, valueOfNode));
                createdOK = true;
            }

            return createdOK;
        }

        //Overload for method to create a node in the graph with node name parameter. Checks before creation for node name clashes to circumvent 
        //duplication.
        //Returns boolean true on success and false on failure.
        public bool CreateNode(string nameOfNode)
        {
            return CreateNode(nameOfNode, 0);
        }

        //Overload for method to create a node in the graph. Extra parameters to add name and value to nodes in an array of string format.
        //Returns boolean true on success and false on failure.
        public bool CreateNode(object[] nodeData)
        {
            switch (nodeData.Count())
            {
                case 1:
                    return CreateNode((string)nodeData[0]);
                case 2:
                    return CreateNode((string)nodeData[0], nodeData[1]);
                default:
                    return false;
            }
        }

        //Internal method to return reference of node from a custom list of nodes based on node name parameter 
        private Node GetNode(string nameOfNode, List<Node> listOfNode)
        {
            return listOfNode.SingleOrDefault(m => m.nameOfNode.Equals(nameOfNode));
        }

        //Overload for internal method to return reference of node from the graph based on a node name parameter
        private Node GetNode(string nameOfNode)
        {
            return GetNode(nameOfNode, listOfNodes);
        }

        //Method to connect two nodes with an edge. Extra parameters to add value to nodes, weight to edge and to set up bi-directional edges.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(string nameOfNodeA, object valueOfNodeA, string nameOfNodeB, object valueOfNodeB, object weightOfEdge, bool biDirectionalEdge)
        {
            bool connectionSuccessful = false;

            CreateNode(nameOfNodeA, valueOfNodeA);
            Node nodeA = GetNode(nameOfNodeA);

            CreateNode(nameOfNodeB, valueOfNodeB);
            Node nodeB = GetNode(nameOfNodeB);

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
        public bool ConnectNodes(string nameOfNodeA, string nameOfNodeB, object weightOfEdge)
        {
            return ConnectNodes(nameOfNodeA, 0, nameOfNodeB, 0, weightOfEdge, false);
        }

        //Overload for method to connect two nodes with a bi-directional edge. Extra parameter to add weight to edge and set edge mode to bi-directional.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(string nameOfNodeA, string nameOfNodeB, object weightOfEdge, bool biDirectionalEdge)
        {
            return ConnectNodes(nameOfNodeA, 0, nameOfNodeB, 0, weightOfEdge, biDirectionalEdge);
        }

        //Overload for method to connect two nodes with a directional edge. Extra parameter to add value to nodes and weight to edge.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(string nameOfNodeA, object valueOfNodeA, string nameOfNodeB, object valueOfNodeB, object weightOfEdge)
        {
            return ConnectNodes(nameOfNodeA, valueOfNodeA, nameOfNodeB, valueOfNodeB, weightOfEdge, false);
        }

        //Overload for method to connect two nodes with an edge. Extra parameters to add value to nodes and weight to an optionally bidirectional edge 
        //in an array of string format.
        //Returns boolean true on success and false on failure.
        public bool ConnectNodes(object[] nodeData)
        {
            switch (nodeData.Count())
            {
                case 3:
                    return ConnectNodes((string)nodeData[0], (string)nodeData[1], nodeData[2]);
                case 4:
                    return ConnectNodes((string)nodeData[0], (string)nodeData[1], nodeData[2], (bool)nodeData[3]);
                case 5:
                    return ConnectNodes((string)nodeData[0], nodeData[1], (string)nodeData[2], nodeData[3], nodeData[4]);
                case 6:
                    return ConnectNodes((string)nodeData[0], nodeData[1], (string)nodeData[2], nodeData[3], nodeData[4], (bool)nodeData[5]);
                default:
                    return false;
            }
        }

        //Method to populate graph with nodes and edges through an edge list. Accepts a list of object arrays as edges and batch processes them
        //through the ConnectNodes() method.
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

        //Method to initiate the traversal of the graph. Accepts string array as parameter with list of node names to include in the path search.
        //The first item from the array is selected as the starting point and the last as the destination. The inclusion of ntermediary nodes is 
        //checked by a separate method after the initial run of the traversal algorithm.
        //Returns boolean true if at least one possible route is found and false if none.
        public bool TraverseNodes(string[] nodeNameArray, bool preserveNodeOrder)
        {
            listOfPossibleRoutes = new List<List<Node>>();

            Node nodeA = GetNode(nodeNameArray.First());
            Node nodeB = GetNode(nodeNameArray.Last());

            if (nodeA != null && nodeB != null)
            {
                List<Node> nodesOnRoute = new List<Node>();
                DepthFirstRecursive(nodesOnRoute, nodeA, nodeB);
            }

            IntermediaryNodeInclusionFilter(nodeNameArray, preserveNodeOrder);

            return (listOfPossibleRoutes.Count > 0) ? true : false;
        }

        //Recursive method to traverse graph using a depth-first search algorithm. Takes a route tracing list, a starting and a destination 
        //node as parameters.
        private void DepthFirstRecursive(List<Node> nodesOnRoute, Node nodeA, Node nodeB)
        {
            nodesOnRoute.Add(nodeA);

            foreach (var edge in nodeA.listOfEdges)
            {
                Node nextNode = edge.nodeB;

                if (!nodesOnRoute.Contains(nextNode))
                {
                    if (nextNode == nodeB)
                    {
                        List<Node> tempNodesOnRoute = new List<Node>();

                        foreach (var node in nodesOnRoute)
                        {
                            tempNodesOnRoute.Add(node);
                        }

                        tempNodesOnRoute.Add(nextNode);
                        listOfPossibleRoutes.Add(tempNodesOnRoute);
                    }
                    else
                    {
                        DepthFirstRecursive(nodesOnRoute, nextNode, nodeB);
                    }
                }
            }

            nodesOnRoute.Remove(nodesOnRoute.Last());
        }

        //Method to filter out possible routes that do not contain the intermediary nodes. The preservation of the order of intermediary nodes 
        //depends on the bool parameter preserveNodeOrder.
        private void IntermediaryNodeInclusionFilter(string[] nodeNameArray, bool preserveNodeOrder)
        {
            List<List<Node>> newListOfPossibleRoutes = new List<List<Node>>();

            foreach (var route in listOfPossibleRoutes)
            {
                bool routeContainsAllIntermediaryNodes = (preserveNodeOrder) ?
                    NodeInclusionInOrder(nodeNameArray, route) : NodeInclusionNotInOrder(nodeNameArray, route);

                if (routeContainsAllIntermediaryNodes)
                {
                    newListOfPossibleRoutes.Add(route);
                }
            }

            listOfPossibleRoutes = newListOfPossibleRoutes;
        }

        //Checks for nodes in the route passed by as a parameter and returns a bool depending on if the route passes the check. In-order verification.
        private bool NodeInclusionInOrder(string[] nodeNameArray, List<Node> route)
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
        private bool NodeInclusionNotInOrder(string[] nodeNameArray, List<Node> route)
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

        //Method to summarise the weight of edges in all possible routes found and return the route with the lowest or highest value as a key-value pair
        public KeyValuePair<string, object> GetRouteByEdgeWeight(bool orderByAscending)
        {
            try
            {
                Dictionary<string, object> dictionaryOfWeightedPossibleRoutes = new Dictionary<string, object>();

                foreach (var route in listOfPossibleRoutes)
                {
                    StringBuilder currentRouteNodes = new StringBuilder();
                    double currentRouteEdgeWeights = 0;

                    for (int i = 0; i < route.Count - 1; i++)
                    {
                        Node currentNode = route[i];
                        Node nextNode = route[i + 1];

                        currentRouteNodes.Append(currentNode.nameOfNode);
                        currentRouteNodes.Append('-');
                        currentRouteEdgeWeights += Convert.ToDouble(currentNode.listOfEdges.SingleOrDefault(m => m.nodeB == nextNode).weightOfEdge);
                    }

                    currentRouteNodes.Append(route.Last().nameOfNode);
                    dictionaryOfWeightedPossibleRoutes.Add(currentRouteNodes.ToString(), currentRouteEdgeWeights);
                }

                return ReturnTopElementFromDictionary(orderByAscending, dictionaryOfWeightedPossibleRoutes);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<string, object>("Invalid edge weight data type\n" + ex.Message, null);
            }
        }

        //Method to summarise the value of nodes in all possible routes found and return the route with the lowest or highest value as a key-value pair
        public KeyValuePair<string, object> GetRouteByNodeValue(bool orderByAscending)
        {
            try
            {
                Dictionary<string, object> dictionaryOfWeightedPossibleRoutes = new Dictionary<string, object>();

                foreach (var route in listOfPossibleRoutes)
                {
                    StringBuilder currentRouteNodes = new StringBuilder();
                    double currentRouteNodeValues = 0;

                    for (int i = 0; i < route.Count; i++)
                    {
                        Node currentNode = route[i];

                        currentRouteNodes.Append(currentNode.nameOfNode);
                        currentRouteNodeValues += Convert.ToDouble(currentNode.valueOfNode);

                        if (i < route.Count - 1)
                        {
                            currentRouteNodes.Append('-');
                        }
                    }

                    dictionaryOfWeightedPossibleRoutes.Add(currentRouteNodes.ToString(), currentRouteNodeValues);
                }

                return ReturnTopElementFromDictionary(orderByAscending, dictionaryOfWeightedPossibleRoutes);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<string, object>("Invalid node value data type\n" + ex.Message, null);
            }
        }

        //Method to summarise the value of nodes and weight of edges in all possible routes found and return the route with the lowest or highest 
        //value as a key-value pair
        public KeyValuePair<string, object> GetRouteByCombinedValue(bool orderByAscending)
        {
            try
            {
                Dictionary<string, object> dictionaryOfWeightedPossibleRoutes = new Dictionary<string, object>();

                foreach (var route in listOfPossibleRoutes)
                {
                    StringBuilder currentRouteNodes = new StringBuilder();
                    double currentRouteEdgeWeights = 0;
                    double currentRouteNodeValues = 0;

                    for (int i = 0; i < route.Count; i++)
                    {
                        Node currentNode = route[i];

                        currentRouteNodes.Append(currentNode.nameOfNode);
                        currentRouteNodeValues += Convert.ToDouble(currentNode.valueOfNode);

                        if (i < route.Count - 1)
                        {
                            Node nextNode = route[i + 1];

                            currentRouteNodes.Append('-');
                            currentRouteEdgeWeights += Convert.ToDouble(currentNode.listOfEdges.SingleOrDefault(m => m.nodeB == nextNode).weightOfEdge);
                        }
                    }

                    dictionaryOfWeightedPossibleRoutes.Add(currentRouteNodes.ToString(), currentRouteNodeValues);
                }

                return ReturnTopElementFromDictionary(orderByAscending, dictionaryOfWeightedPossibleRoutes);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<string, object>("Invalid edge weight/node value data type\n" + ex.Message, null);
            }
        }

        //Method to summarise the number of hops in all possible routes found and return the route with the lowest or highest value as a key-value pair
        public KeyValuePair<string, object> GetRouteByNumberOfNodes(bool orderByAscending)
        {
            Dictionary<string, object> dictionaryOfWeightedPossibleRoutes = new Dictionary<string, object>();

            foreach (var route in listOfPossibleRoutes)
            {
                StringBuilder currentRouteNodes = new StringBuilder();

                for (int i = 0; i < route.Count; i++)
                {
                    Node currentNode = route[i];

                    currentRouteNodes.Append(currentNode.nameOfNode);

                    if (i < route.Count - 1)
                    {
                        currentRouteNodes.Append('-');
                    }
                }

                dictionaryOfWeightedPossibleRoutes.Add(currentRouteNodes.ToString(), route.Count);
            }

            return ReturnTopElementFromDictionary(orderByAscending, dictionaryOfWeightedPossibleRoutes);
        }

        //Method to order a dictionary passed by a parameter in descending or ascending order based on a bool parameter and return the first element 
        //as a key-value pair
        private KeyValuePair<string, object> ReturnTopElementFromDictionary(bool orderByAscending, Dictionary<string, object> dictionaryOfWeightedPossibleRoutes)
        {
            if (orderByAscending)
            {
                dictionaryOfWeightedPossibleRoutes = dictionaryOfWeightedPossibleRoutes.OrderBy(m => m.Value).ToDictionary(n => n.Key, n => n.Value);
            }
            else
            {
                dictionaryOfWeightedPossibleRoutes = dictionaryOfWeightedPossibleRoutes.OrderByDescending(m => m.Value).ToDictionary(n => n.Key, n => n.Value);
            }

            return dictionaryOfWeightedPossibleRoutes.FirstOrDefault();
        }
    }
}
