using System;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    /// <summary>
    /// Klasa reprezentująca węzeł i przechowywująca informacje potrzebne do wyznaczenia trasy.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Typ wyliczeniowy określający stan elementu siatki.
        /// </summary>
        public enum NodeState
        {
            /// <summary>
            /// Element niesprawdzony przez algorytm.
            /// </summary>
            Untested,
            /// <summary>
            /// Element dostępny przy wyznaczaniu trasy.
            /// </summary>
            Open,
            /// <summary>
            /// Element odrzucony przy wyznaczaniu trasy
            /// </summary>
            Closed
        }

        /// <summary>
        /// Zmienna przechowująca referencję na obiekt poprzedzający dany podczas wyznaczania trasy.
        /// </summary>
        private Node _parentNode;
        /// <summary>
        /// Współrzędne elementu
        /// </summary>
        public Vector2 Location { get; private set; }
        /// <summary>
        /// Zmienna logiczna określająca czy element jest dostępny przy wyznaczaniu trasy.
        /// </summary>
        public bool IsWalkable { get; set; }
        /// <summary>
        /// Wartość G z algorytmu A*, czyli długość ścieżki prowadzącej z punktu początkowego do aktualnego pola.
        /// </summary>
        public float G { get; private set; }
        /// <summary>
        /// Wartość G z algorytmu A*, czyli szacunkowa długość ścieżki prowadząca z aktualnego pola do punktu końcowego
        /// </summary>
        public float H { get; private set; }
        /// <summary>
        /// Stan elementu.
        /// </summary>
        public NodeState State { get; set; }

        /// <summary>
        /// Metoda typu get i set dla wartości F potrzebnej do określania ścieżki.
        /// </summary>
        public float F
        {
            get { return G + H; }
        }

        /// <summary>
        /// Metoda typu get i set zmiennej przechowującej rodzica danego węzła. 
        /// </summary>
        public Node ParentNode
        {
            get { return _parentNode; }
            set
            {
                _parentNode = value;
                G = _parentNode.G + GetTraversalCost(Location, _parentNode.Location);
            }
        }

        /// <summary>
        /// Konstruktor klasy Node.
        /// </summary>
        /// <param name="x">Pierwsza współrzędna elementu.</param>
        /// <param name="y">Druga współrzędna elementu.</param>
        /// <param name="isWalkable">Określa czy element jest dostępny przy wyznaczaniu trasy.</param>
        /// <param name="endLocation">Współrzędne końcowego punktu.</param>
        public Node(int x, int y, bool isWalkable, Vector2 endLocation)
        {
            Location = new Vector2(x, y);
            State = NodeState.Untested;
            IsWalkable = isWalkable;
            H = GetTraversalCost(Location, endLocation);
            G = 0;
        }

        /// <summary>
        /// Metoda zwracają odległość euklidesową między dwoma elementami.
        /// </summary>
        /// <param name="location">Pierwszy element.</param>
        /// <param name="otherLocation">Drugi element.</param>
        /// <returns>Zwaraca wartość będącą odległość euklidesową między dwoma argumentami.</returns>
        internal static float GetTraversalCost(Vector2 location, Vector2 otherLocation)
        {
            float deltaX = otherLocation.x - location.x;
            float deltaY = otherLocation.y - location.y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
