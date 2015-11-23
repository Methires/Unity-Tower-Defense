using System.Collections.Generic;
using Assets.Scripts.Pathfinding;
using UnityEngine;

public class PathCreation : MonoBehaviour {

    private int _length;
    private int _width;
    private int _startingPoint;
    private int _endingPoint;
    private bool[,] _map;
    private SearchParameters _searchParameters;
    private List<Point> _path;
    private GameObject[,] _tiles;

    void Start()
    {
        _length = FindObjectOfType<SpawnTiles>().Lenght;
        _width = FindObjectOfType<SpawnTiles>().Width;
        _startingPoint = Random.Range(0, _width);
        _endingPoint = Random.Range(0, _width);
        _tiles = FindObjectOfType<SpawnTiles>().GetAllTiles();
        Instantiate(Resources.Load("Prefabs/Start"), _tiles[0, _startingPoint].transform.position, Quaternion.identity);
        Instantiate(Resources.Load("Prefabs/End"), _tiles[_length - 1, _endingPoint].transform.position, Quaternion.identity);
        InitializeMap();
        MarkObstacles();
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

    private void MarkObstacles()
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
    }

    private void ShowPath()
    {
        PathFinder pathFinder = new PathFinder(_searchParameters);
        _path = pathFinder.FindPath();
        foreach (var part in _path)
        {
            _tiles[part.X, part.Y].GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Path") as Material;
        }
    }
	
}
