using UnityEngine;
using Assets.Scripts.Pathfinding;

public class GameController : MonoBehaviour
{
    private int _resources;
    private int _startingPoint;
    private int _endingPoint;
    private Point _currentTile;
    private GameObject[,] _tiles;
	void Start ()
	{
        _currentTile = new Point(0,0);
	    _startingPoint = FindObjectOfType<PathCreation>().StartingPoint;
	    _endingPoint = FindObjectOfType<PathCreation>().EndingPoint;
        _tiles = new GameObject[FindObjectOfType<SpawnTiles>().Lenght, FindObjectOfType<SpawnTiles>().Width];
        _tiles = FindObjectOfType<SpawnTiles>().GetAllTiles();
        ChangeHighlightedTile(' ');
        UpdateResources(100000);
	}
	
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.W))
	    {
	        if (_currentTile.X < FindObjectOfType<SpawnTiles>().Lenght - 1)
	        {
	            ChangeHighlightedTile('W');
	        }
	    }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_currentTile.X > 0)
            {
                ChangeHighlightedTile('S');
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (_currentTile.Y > 0)
            {
                ChangeHighlightedTile('A');
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (_currentTile.Y < FindObjectOfType<SpawnTiles>().Width - 1)
            {
                ChangeHighlightedTile('D');
            }
        }
        
	    if (Input.GetKeyDown(KeyCode.B))
	    {
            if (!((_currentTile.X == 0 && _currentTile.Y == _startingPoint) || (_currentTile.X == FindObjectOfType<SpawnTiles>().Lenght - 1 && _currentTile.Y == _endingPoint)))
            {
                if (_tiles[_currentTile.X, _currentTile.Y].transform.childCount == 0)
                {
                    if (_resources - 150 >= 0)
                    {
                        if (FindObjectOfType<PathCreation>().ViablePath(_currentTile.X,_currentTile.Y))
                        {
                            SpawnTower("Base", 150); 
                        }
                    }
                } 
            }
	    }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!((_currentTile.X == 0 && _currentTile.Y == _startingPoint) || (_currentTile.X == FindObjectOfType<SpawnTiles>().Lenght - 1 && _currentTile.Y == _endingPoint)))
            {
                if (_tiles[_currentTile.X, _currentTile.Y].transform.childCount == 0)
                {
                    if (_resources - 250 >= 0)
                    {
                        if (FindObjectOfType<PathCreation>().ViablePath(_currentTile.X, _currentTile.Y))
                        {
                            SpawnTower("Shooting", 250);
                        }
                    }
                }
            }
        }

	    if (Input.GetKeyDown(KeyCode.X))
	    {
	        if (_tiles[_currentTile.X, _currentTile.Y].transform.childCount != 0)
	        {
	            DestroyTower();
	        }
	    }
    }

    private void ChangeHighlightedTile(char pressedKey)
    {
        switch (pressedKey)
        {
            case 'W':
                _currentTile.X = _currentTile.X + 1;
                _currentTile.Y = _currentTile.Y;
                break;
            case 'S':
                _currentTile.X = _currentTile.X - 1;
                _currentTile.Y = _currentTile.Y;
                break;
            case 'A':
                _currentTile.X = _currentTile.X;
                _currentTile.Y = _currentTile.Y - 1;
                break;
            case 'D':
                _currentTile.X = _currentTile.X;
                _currentTile.Y = _currentTile.Y + 1;
                break;
            default:
                _currentTile.X = _currentTile.X;
                _currentTile.Y = _currentTile.Y;
                break;
        }
        ApplyMaterialOnTiles();
    }

    private void SpawnTower(string towerName, int towerCost)
    {
        GameObject baseTower = Instantiate(Resources.Load("Prefabs/Towers/" + towerName), _tiles[_currentTile.X, _currentTile.Y].transform.position, Quaternion.identity) as GameObject;
        if (baseTower != null)
        {
            baseTower.transform.parent = _tiles[_currentTile.X, _currentTile.Y].transform;
            UpdateResources(-towerCost);
        } 
        ApplyMaterialOnTiles();
    }

    private void DestroyTower()
    {
        if (_tiles[_currentTile.X, _currentTile.Y].transform.GetChild(0).gameObject.name.Contains("Base"))
	    {
		    UpdateResources(150);
        }
        else if (_tiles[_currentTile.X, _currentTile.Y].transform.GetChild(0)
            .gameObject.name.Contains("Shooting"))
        {
            UpdateResources(250);
        }
        Destroy(_tiles[_currentTile.X, _currentTile.Y].transform.GetChild(0).gameObject);
        FindObjectOfType<PathCreation>().ViablePath(-1, -1);
        ApplyMaterialOnTiles();
    }

    public void UpdateResources(int value)
    {
        _resources += value;
        GameObject.FindGameObjectWithTag("ResourcesText").GetComponent<GUIText>().text = "Resources: " + _resources;
    }

    private void ApplyMaterialOnTiles()
    {
        foreach (var tile in _tiles)
        {
            tile.GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Normal") as Material;
        }
        var path = FindObjectOfType<PathCreation>().GetPath();
        foreach (var pathPart in path)
        {
            pathPart.GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Path") as Material;
        }
        _tiles[_currentTile.X, _currentTile.Y].GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Highlighted") as Material;
    }
}
