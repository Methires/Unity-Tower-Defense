using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    /// <summary>
    /// 
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        /// /
        /// </summary>
        private int _width;
        /// <summary>
        /// 
        /// </summary>
        private int _height;
        /// <summary>
        /// 
        /// </summary>
        private Node[,] _nodes;
        /// <summary>
        /// 
        /// </summary>
        private Node _startNode;
        /// <summary>
        /// 
        /// </summary>
        private Node _endNode;
        /// <summary>
        /// 
        /// </summary>
        private SearchParameters _searchParameters;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParameters"></param>
        public PathFinder(SearchParameters searchParameters)
        {
            _searchParameters = searchParameters;
            InitializeNodes(searchParameters.Map);
            _startNode = _nodes[(int)searchParameters.StartLocation.x, (int)searchParameters.StartLocation.y];
            _startNode.State = Node.NodeState.Open;
            _endNode = _nodes[(int)searchParameters.EndLocation.x, (int)searchParameters.EndLocation.y];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Vector2> FindPath()
        {
            List<Vector2> path = new List<Vector2>();
            bool success = Search(_startNode);
            if (success)
            {
                Node node = _endNode;
                while (node.ParentNode != null)
                {
                    path.Add(node.Location);
                    node = node.ParentNode;
                }

                path.Reverse();
            }

            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        private void InitializeNodes(bool[,] map)
        {
            _width = map.GetLength(0);
            _height = map.GetLength(1);
            _nodes = new Node[_width, _height];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _nodes[x, y] = new Node(x, y, map[x, y], _searchParameters.EndLocation);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private bool Search(Node currentNode)
        {
            currentNode.State = Node.NodeState.Closed;
            List<Node> nextNodes = GetAdjacentWalkableNodes(currentNode);

            nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
            foreach (var nextNode in nextNodes)
            {
                if (nextNode.Location == _endNode.Location)
                {
                    return true;
                }
                if (Search(nextNode))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromNode"></param>
        /// <returns></returns>
        private List<Node> GetAdjacentWalkableNodes(Node fromNode)
        {
            List<Node> walkableNodes = new List<Node>();
            IEnumerable<Vector2> nextLocations = GetAdjacentLocations(fromNode.Location);

            foreach (var location in nextLocations)
            {
                int x = (int)location.x;
                int y = (int)location.y;

                if (x < 0 || x >= _width || y < 0 || y >= _height)
                {
                    continue;
                }

                Node node = _nodes[x, y];
                if (!node.IsWalkable)
                {
                    continue;
                }
                if (node.State == Node.NodeState.Closed)
                {
                    continue;
                }
                if (node.State == Node.NodeState.Open)
                {
                    float traversalCost = Node.GetTraversalCost(node.Location, node.ParentNode.Location);
                    float gTemp = fromNode.G + traversalCost;
                    if (gTemp < node.G)
                    {
                        node.ParentNode = fromNode;
                        walkableNodes.Add(node);
                    }
                }
                else
                {
                    node.ParentNode = fromNode;
                    node.State = Node.NodeState.Open;
                    walkableNodes.Add(node);
                }
            }

            return walkableNodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <returns></returns>
        private static IEnumerable<Vector2> GetAdjacentLocations(Vector2 fromLocation)
        {
            return new Vector2[]
            {
                new Vector2(fromLocation.x - 1, fromLocation.y - 1), 
                new Vector2(fromLocation.x - 1, fromLocation.y), 
                new Vector2(fromLocation.x - 1, fromLocation.y + 1), 
                new Vector2(fromLocation.x, fromLocation.y + 1), 
                new Vector2(fromLocation.x + 1, fromLocation.y + 1),
                new Vector2(fromLocation.x + 1, fromLocation.y),
                new Vector2(fromLocation.x + 1, fromLocation.y - 1),
                new Vector2(fromLocation.x, fromLocation.y - 1)
            };
        }
    }
}