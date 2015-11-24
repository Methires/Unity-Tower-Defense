using System.Collections;
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
	    _startingPoint = FindObjectOfType<CreatePath>().StartingPoint;
	    _endingPoint = FindObjectOfType<CreatePath>().EndingPoint;
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

	        if (Input.GetKeyDown(KeyCode.Alpha1))
	        {
                if (CanBuildTower())
                {
                    if (!HasChildObjects())
                    {
                        if (FindObjectOfType<CreatePath>().ViablePath(_currentTile.X, _currentTile.Y))
                        {
                            SpawnWallTower();
                        }
                    }
                }
	        }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (CanBuildTower())
	            {
	                if (!HasChildObjects())
	                {
	                    if (FindObjectOfType<CreatePath>().ViablePath(_currentTile.X, _currentTile.Y))
	                    {
	                        SpawnShootingTower();
	                    }
	                }
	            }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (CanBuildTower())
	            {
	                if (!HasChildObjects())
	                {
	                    if (FindObjectOfType<CreatePath>().ViablePath(_currentTile.X, _currentTile.Y))
	                    {
	                        SpawnAoeTower();
	                    }
	                }
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (HasChildObjects())
                {
                    if (FindObjectOfType<CreatePath>().FindPathAfterTowerRemoval(_currentTile.X,_currentTile.Y))
                    {
                        DestroyTower();
                    }
                }
            }

	    if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(StartWave());
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
        var path = FindObjectOfType<CreatePath>().GetPath();
        foreach (var pathPart in path)
        {
            pathPart.GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Path") as Material;
        }
        _tiles[_currentTile.X, _currentTile.Y].GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Highlighted") as Material;
    }

    private IEnumerator StartWave()
    {
        int enemyCount = 5;
        int enemyCounter = 0;
        while (enemyCounter < enemyCount)
        {
            yield return new WaitForSeconds(1.0f);
            SpawnEnemy();
            enemyCounter++;
        }
    }

    private void SpawnEnemy()
    {
        Instantiate(Resources.Load("Prefabs/Enemies/Type1"), _tiles[0, _startingPoint].transform.position, Quaternion.identity);
    }

    public void SpawnWallTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Wall, _tiles[_currentTile.X, _currentTile.Y]));
        ApplyMaterialOnTiles();
    }

    public void SpawnShootingTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Shooting, _tiles[_currentTile.X, _currentTile.Y]));
        ApplyMaterialOnTiles();
    }

    public void SpawnAoeTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Aoe, _tiles[_currentTile.X, _currentTile.Y]));
        ApplyMaterialOnTiles();
    }

    public void DestroyTower()
    {
        UpdateResources(FindObjectOfType<BuildTower>().DestroyTower(_tiles[_currentTile.X, _currentTile.Y].transform.GetChild(0).gameObject));
        ApplyMaterialOnTiles();
    }

    public bool CanBuildTower()
    {
        if (((_currentTile.X == 0 && _currentTile.Y == _startingPoint) || (_currentTile.X == FindObjectOfType<SpawnTiles>().Lenght - 1 && _currentTile.Y == _endingPoint)))
        {
            return false;
        }
        return true;
    }

    public bool HasChildObjects()
    {
        if (_tiles[_currentTile.X, _currentTile.Y].transform.childCount != 0)
        {
            return true;
        }
        return false;

    }
}
