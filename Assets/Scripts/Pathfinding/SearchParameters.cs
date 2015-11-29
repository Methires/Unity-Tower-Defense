using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public class SearchParameters
    {
        public Vector2 StartLocation { get; set; }
        public Vector2 EndLocation { get; set; }
        public bool[,] Map { get; set; }


        public SearchParameters(Vector2 startLocation, Vector2 endLocation, bool[,] map)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
            Map = map;
        }
    }
}
