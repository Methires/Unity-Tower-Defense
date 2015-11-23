using System;

namespace Assets.Scripts.Pathfinding
{
    public class Node
    {
        public enum NodeState
        {
            Untested,
            Open,
            Closed

        }

        private Node _parentNode;
        public Point Location { get; private set; }
        public bool IsWalkable { get; set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public NodeState State { get; set; }

        public float F
        {
            get { return this.G + this.H; }
        }

        public Node ParentNode
        {
            get { return this._parentNode; }
            set
            {
                this._parentNode = value;
                this.G = this._parentNode.G + GetTraversalCost(this.Location, this._parentNode.Location);
            }
        }

        public Node(int x, int y, bool isWalkable, Point endLocation)
        {
            this.Location = new Point(x, y);
            this.State = NodeState.Untested;
            this.IsWalkable = isWalkable;
            this.H = GetTraversalCost(this.Location, endLocation);
            this.G = 0;
        }

        internal static float GetTraversalCost(Point location, Point otherLocation)
        {
            float deltaX = otherLocation.X - location.X;
            float deltaY = otherLocation.Y - location.Y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
