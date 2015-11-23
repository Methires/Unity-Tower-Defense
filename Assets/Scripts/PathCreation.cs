using System.Collections.Generic;
using Assets.Scripts.Pathfinding;
using UnityEngine;

public class PathCreation : MonoBehaviour {

    private int _length;
    private int _width;
    private int _startingPoint { get; set; }
    private int _endingPoint { get; set; }
    private bool[,] _map;
    private SearchParameters _searchParameters;
    private List<Point> _path;
    private GameObject[,] _tiles;

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
        ShowPath();
    }

    private void InitializeMap()
    {
        this._map = new bool[_length, _width];
        for (int i = 0; i < _length; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                _map[i, j] = true;
            }
        }
        var startLocation = new Point(0, _startingPoint);
        var endLocation = new Point(_length -1, _endingPoint);
        this._searchParameters = new SearchParameters(startLocation, endLocation, _map);
    }

    private void MarkObstacles(int x = -1, int y = -1)
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
            _map[x, y] = false;
        }
    }

    private void FindPath()
    {
        PathFinder pathFinder = new PathFinder(_searchParameters);
        _path = pathFinder.FindPath();
    }

    private void ShowPath()
    {
        foreach (var part in _path)
        {
            _tiles[part.X, part.Y].GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Path") as Material;
        }
    }

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
            ShowPath();
            return false;
        }
        else
        {
            ShowPath();
            return true;
        }
    }

    public List<GameObject> GetPath()
    {
        List<GameObject> goPath = new List<GameObject>();

        foreach (var tile in _path)
        {
            goPath.Add(_tiles[tile.X,tile.Y]);
        }
        return goPath;
    }

    public int StartingPoint
    {
        get { return _startingPoint; }
        private set { this._startingPoint = value; }
    }

    public int EndingPoint
    {
        get { return _endingPoint; }
        private set { this._endingPoint = value; }
    }
}
