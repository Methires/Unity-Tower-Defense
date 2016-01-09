using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Pathfinding;
using UnityEngine;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Odpowiedzialna za wszelkie funkcjonalności związane z wyznaczeniem i zaznaczaniem trasy, po której poruszają się przeciwnicy.
/// </summary>
public class CreatePath : MonoBehaviour 
{
    /// <summary>
    /// Zmienna przechowująca informację o wysokości siatki dwuwymiarowej.
    /// </summary>
    private int _length;
    /// <summary>
    /// Zmienna przechowująca informację o szerokości siatki dwuwymiarowej.
    /// </summary>
    private int _width;
    /// <summary>
    /// Zmienna przechowującą indeks kolumny z siatki dwuwymiarowej w której znajduje się punkt początkowy.
    /// </summary>
    private int _startingPoint;
    /// <summary>
    /// Zmienna przechowującą indeks kolumny z siatki dwuwymiarowej w której znajduje się punkt końcowy.
    /// </summary>
    private int _endingPoint;
    /// <summary>
    /// Zmienna tablicowa przechowująca wartości logiczne określające czy element siatki posiada dzieci czy nie.
    /// </summary>
    private bool[,] _map;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt klasy SearchParameters.
    /// </summary>
    private SearchParameters _searchParameters;
    /// <summary>
    /// Zmienna przechowująca listę indeksów elementów siatki dwuwymiarowej, które składają sią na trasę dla przeciwników.
    /// </summary>
    private List<Vector2> _path;
    /// <summary>
    /// Zmienna przechowująca dwuwymiarową tablicę referencji na obiekty wchodzące w skład siatki dwuwymiarowej.
    /// </summary>
    private GameObject[,] _tiles;
	
    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
	/// Odnajduje odpowiednie obiekty i przypisuje referencje na nie do odpowiednich zmiennych klasy.
	/// Tworzy punkt początkowy i końcowy, a następnie wywołuje metodę odpowiedzialną za wyznaczanie trasy między nimi.
    /// </summary>
    void Start()
    {
        _length = FindObjectOfType<SpawnTiles>().Lenght;
        _width = FindObjectOfType<SpawnTiles>().Width;
        StartingPoint = Random.Range(0, _width);
        EndingPoint = Random.Range(0, _width);
        _tiles = FindObjectOfType<SpawnTiles>().GetAllTiles();
        Instantiate(Resources.Load("Prefabs/Start"), _tiles[0, _startingPoint].transform.position, Quaternion.identity);
        Instantiate(Resources.Load("Prefabs/End"), _tiles[_length - 1, _endingPoint].transform.position, Quaternion.identity);
        InitializeMap();
        MarkObstacles();
        FindPath();
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za ustalenie początkowych wartości dla siatki dwuwymiarowej, celem przygotowania jej do wyszukiwania trasy.
    /// </summary>
    private void InitializeMap()
    {
        _map = new bool[_length, _width];
        for (int i = 0; i < _length; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                _map[i, j] = true;
            }
        }
        var startLocation = new Vector2(0, _startingPoint);
        var endLocation = new Vector2(_length -1, _endingPoint);
        _searchParameters = new SearchParameters(startLocation, endLocation, _map);
    }
	
    /// <summary>
    /// Metoda odpowiedzialna na oznaczanie na dwywymiarowej tablicy przechowującej wartości logiczne, czy element siatki posiada dzieci czy nie.
    /// </summary>
    /// <param name="x">Indeks wiersza z elementem siatki dwuwymiarowej na którym ma powstać nowa wieża albo zostać usunięta obecna.</param>
    /// <param name="y">Indeks kolumny z elementem siatki dwuwymiarowej na którym ma powstać nowa wieża albo zostać usunięta obecna.</param>
    /// <param name="delete">Argument typu bool określający czy wieżna na zostać usunięta czy będzie dodana.</param>
    private void MarkObstacles(int x = -1, int y = -1, bool delete = false)
    {
        for (int i = 0; i < _length; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                if (_tiles[i, j].transform.childCount != 0)
                {
                    _map[i, j] = false;
                }
            }
        }

        if (x >= 0 && y >= 0 && x<_length && y<_width)
        {
            if (!delete)
            {
                _map[x, y] = false;
            }
            else
            {
                _map[x, y] = true;
            }
        }
    }
	
    /// <summary>
    /// Metoda wywołująca odnajdywanie metody odpowiedzialnej za odnajdywanie ścieżki.
    /// </summary>
    private void FindPath()
    {
        PathFinder pathFinder = new PathFinder(_searchParameters);
        _path = pathFinder.FindPath();
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za określanie czy możliwe jest wybudowanie przez gracza wieży na polu siatki dwuwymiarowej o współrzędnych x i y, tak, aby nie blokować ścieżki.
    /// </summary>
    /// <param name="x">Indeks wiersza z elementem siatki dwuwymiarowej na którym ma powstać nowa wieża.</param>
    /// <param name="y">Indeks kolumny z elementem siatki dwuwymiarowej na którym ma powstać nowa wieża.</param>
    /// <returns>Zwraca wartość logiczną true, gdy ścieżka będzie istnieć albo false, gdy ścieżka nie będzie mogła powstać.</returns>
    public bool ViablePath(int x, int y)
    {
        var previousPath = _path;
        var previousMap = _map;
        InitializeMap();
        MarkObstacles(x,y);
        FindPath();
        if (_path.Count == 0)
        {
            _map = previousMap;
            _path = previousPath;
            return false;
        }
        return true;
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za odnajdywanie ścieżki po usunięciu wieży przez gracza. Jeżeli zwróci coś innego, niż prawda, został popełniony błąd podczas wykonywania metody.
    /// </summary>
    /// <param name="x">Indeks wiersza z elementem siatki dwuwymiarowej z którego usunięta ma być wieża.</param>
    /// <param name="y">Indeks kolumny z elementem siatki dwuwymiarowej z którego usunięta ma być wieża.</param>
    /// <returns>Zwraca wartość logiczną true. Wartość false oznacza błąd.</returns>
    public bool FindPathAfterTowerRemoval(int x, int y)
    {
        InitializeMap();
        MarkObstacles(x,y, true);
        FindPath();
        return _path.Count != 0;
    }
	
    /// <summary>
    /// Metoda zwracająca listę pól będących ścieżką dla przeciwników.
    /// </summary>
    /// <returns>Zwraca listę indeksów elementów siatki dwuwymiarowej, które składają sią na trasę dla przeciwników.</returns>
    public List<GameObject> GetPath()
    {
        return _path.Select(tile => _tiles[(int)tile.x, (int)tile.y]).ToList();
    }
	
    /// <summary>
    /// Metoda typu get i set dla drugiej współrzędnej punktu początkowego.
    /// </summary>
    public int StartingPoint
    {
        get { return _startingPoint; }
        private set { _startingPoint = value; }
    }
	
    /// <summary>
    /// Metoda typu get i set dla drugiej współrzędnej punktu początkowego.
    /// </summary>
    public int EndingPoint
    {
        get { return _endingPoint; }
        private set { _endingPoint = value; }
    }
}
