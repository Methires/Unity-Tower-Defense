using System;
using UnityEngine;

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
        public Vector2 Location { get; private set; }
        public bool IsWalkable { get; set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public NodeState State { get; set; }

        public float F
        {
            get { return G + H; }
        }

        public Node ParentNode
        {
            get { return _parentNode; }
            set
            {
                _parentNode = value;
                G = _parentNode.G + GetTraversalCost(Location, _parentNode.Location);
            }
        }

        public Node(int x, int y, bool isWalkable, Vector2 endLocation)
        {
            Location = new Vector2(x, y);
            State = NodeState.Untested;
            IsWalkable = isWalkable;
            H = GetTraversalCost(Location, endLocation);
            G = 0;
        }

        internal static float GetTraversalCost(Vector2 location, Vector2 otherLocation)
        {
            float deltaX = otherLocation.x - location.x;
            float deltaY = otherLocation.y - location.y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
