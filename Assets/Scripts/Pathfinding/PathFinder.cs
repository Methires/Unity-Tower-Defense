using System.Collections.Generic;

namespace Assets.Scripts.Pathfinding
{
    public class PathFinder
    {
        private int _width;
        private int _height;
        private Node[,] _nodes;
        private Node _startNode;
        private Node _endNode;
        private SearchParameters _searchParameters;

        public PathFinder(SearchParameters searchParameters)
        {
            this._searchParameters = searchParameters;
            InitializeNodes(searchParameters.Map);
            this._startNode = this._nodes[searchParameters.StartLocation.X, searchParameters.StartLocation.Y];
            this._startNode.State = Node.NodeState.Open;
            this._endNode = this._nodes[searchParameters.EndLocation.X, searchParameters.EndLocation.Y];
        }

        public List<Point> FindPath()
        {
            List<Point> path = new List<Point>();
            bool success = Search(_startNode);
            if (success)
            {
                Node node = this._endNode;
                while (node.ParentNode != null)
                {
                    path.Add(node.Location);
                    node = node.ParentNode;
                }

                path.Reverse();
            }

            return path;
        }

        private void InitializeNodes(bool[,] map)
        {
            this._width = map.GetLength(0);
            this._height = map.GetLength(1);
            this._nodes = new Node[this._width, this._height];
            for (int y = 0; y < this._height; y++)
            {
                for (int x = 0; x < this._width; x++)
                {
                    this._nodes[x, y] = new Node(x, y, map[x, y], this._searchParameters.EndLocation);
                }
            }
        }

        private bool Search(Node currentNode)
        {
            currentNode.State = Node.NodeState.Closed;
            List<Node> nextNodes = GetAdjacentWalkableNodes(currentNode);

            nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
            foreach (var nextNode in nextNodes)
            {
                if (nextNode.Location == this._endNode.Location)
                {
                    return true;
                }
                else
                {
                    if (Search(nextNode))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<Node> GetAdjacentWalkableNodes(Node fromNode)
        {
            List<Node> walkableNodes = new List<Node>();
            IEnumerable<Point> nextLocations = GetAdjacentLocations(fromNode.Location);

            foreach (var location in nextLocations)
            {
                int x = location.X;
                int y = location.Y;

                if (x < 0 || x >= this._width || y < 0 || y >= this._height)
                {
                    continue;
                }

                Node node = this._nodes[x, y];
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

        private static IEnumerable<Point> GetAdjacentLocations(Point fromLocation)
        {
            return new Point[]
            {
                new Point(fromLocation.X - 1, fromLocation.Y - 1),
                new Point(fromLocation.X - 1, fromLocation.Y),
                new Point(fromLocation.X - 1, fromLocation.Y + 1),
                new Point(fromLocation.X, fromLocation.Y + 1),
                new Point(fromLocation.X + 1, fromLocation.Y + 1),
                new Point(fromLocation.X + 1, fromLocation.Y),
                new Point(fromLocation.X + 1, fromLocation.Y - 1),
                new Point(fromLocation.X, fromLocation.Y - 1)
            };
        }
    }
}