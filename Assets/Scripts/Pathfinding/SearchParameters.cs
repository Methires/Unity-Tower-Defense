using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    /// <summary>
    /// Klasa przechowująca meta dane potrzebne do wyznaczenia trasy, jak punkt początkowy i końcowy.
    /// </summary>
    public class SearchParameters
    {
        /// <summary>
        /// Współrzędne punktu początkowego na siatce dwuwymiarowej.
        /// </summary>
        public Vector2 StartLocation { get; set; }
        /// <summary>
        /// Współrzędne punktu końcowego  na siatce dwuwymiarowej.
        /// </summary>
        public Vector2 EndLocation { get; set; }
        /// <summary>
        /// Zmienna tablicowa przechowująca wartości logiczne określające czy element siatki jest dostępny.
        /// </summary>
        public bool[,] Map { get; set; }

        /// <summary>
        /// Konstruktor klasy SearchParameters.
        /// </summary>
        /// <param name="startLocation">Współrzędne punktu początkowego na siatce dwuwymiarowej.</param>
        /// <param name="endLocation">Współrzędne punktu końcowego na siatce dwuwymiarowej.</param>
        /// <param name="map">Tablica dwuwymiarowa z wartościami logiczymi określającymi czy element siatki jest dostępny.</param>
        public SearchParameters(Vector2 startLocation, Vector2 endLocation, bool[,] map)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
            Map = map;
        }
    }
}
