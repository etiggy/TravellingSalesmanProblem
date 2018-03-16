using GraphLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using TravellingSalesmanProblem.Model;

namespace TravellingSalesmanProblem
{
    class Program
    {
        //Initialisation data for travelling salesman problem
        //Every city is connected to every other in a bi-dreictional fashion
        private readonly static List<object[]> graphInitialisationNodeList = new List<object[]>() {
            new object[] {"A", new Coordinate(05.681818, 63.860370)},
            new object[] {"B", new Coordinate(11.850649, 83.983573)},
            new object[] {"C", new Coordinate(13.798701, 65.092402)},
            new object[] {"D", new Coordinate(16.883117, 40.451745)},
            new object[] {"E", new Coordinate(23.782468, 56.262834)},
            new object[] {"F", new Coordinate(25.000000, 31.211499)},
            new object[] {"G", new Coordinate(29.951299, 41.683778)},
            new object[] {"H", new Coordinate(31.331169, 25.256674)},
            new object[] {"I", new Coordinate(37.175325, 37.577002)},
            new object[] {"J", new Coordinate(39.935065, 19.096509)},
            new object[] {"K", new Coordinate(46.834416, 29.979466)}
        };

        //Graph to hold city network data
        private static Graph<double, Coordinate> graph;

        //Main method
        static void Main(string[] args)
        {
            Initialise();

            PrintOutput();

            Console.WriteLine("\nHit enter to quit");
            Console.ReadLine();
            Environment.Exit(0);
        }

        //Set up graph and populate it with city network data
        private static void Initialise()
        {
            bool loadSuccessful;

            graph = new Graph<double, Coordinate>();

            loadSuccessful = graph.LoadEdgeList(BuildEdgeListFromNodeList(graphInitialisationNodeList));

            if (loadSuccessful)
            {
                Console.WriteLine("Graph data loaded successfully\n");
            }
            else
            {
                Console.WriteLine("Graph data load failed\n");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        //Method to create an edge list from node list. Also calculates the distance between nodes for every edge.
        private static List<object[]> BuildEdgeListFromNodeList(List<object[]> nodeList)
        {
            List<object[]> edgeList = new List<object[]>();

            foreach (var firstNodeData in nodeList)
            {
                foreach (var secondNodeData in nodeList)
                {
                    if (firstNodeData != secondNodeData)
                    {
                        edgeList.Add(new object[] { firstNodeData[0], firstNodeData[1], secondNodeData[0], secondNodeData[1],
                        ((Coordinate)firstNodeData[1]).GetDistance((Coordinate)secondNodeData[1])});
                    }
                }
            }

            return edgeList;
        }

        //Method to initiate graph traversal and print output results
        private static void PrintOutput()
        {
            string[] nodeNameArray = new string[graphInitialisationNodeList.Count];

            for (int i = 0; i < graphInitialisationNodeList.Count; i++)
            {
                nodeNameArray[i] = (string)graphInitialisationNodeList[i][0];
                Coordinate cityCoordinates = (Coordinate)graphInitialisationNodeList[i][1];
                Console.WriteLine("City \"{0}\" at coordinates X: {1} Y: {2}", nodeNameArray[i], cityCoordinates.X.ToString("00.000000"),
                    cityCoordinates.Y.ToString("00.000000"));
            }

            Console.WriteLine("\nCalculating shortest route using a recursive depth-first search algorithm...");
            string routeName = nodeNameArray.First() + "-" + nodeNameArray.Last();

            bool preserveNodeOrder = false;
            bool orderByAscending = true;

            var optimalRoute = graph.GetRoute(nodeNameArray, preserveNodeOrder, orderByAscending, GetRouteBy.EdgeWeight);

            if (optimalRoute.Value != null)
            {
                Console.WriteLine("\nThe shortest route for {0} is through {2}: {1} units", routeName, optimalRoute.Value, optimalRoute.Key);
            }
            else
            {
                Console.WriteLine(optimalRoute.Key);
            }
        }
    }
}
