using System.Collections;
using UnityEngine;
using Assets.Scripts.Pathfinding;

public class GameController : MonoBehaviour
{
    public int StartingResources;
    private int _resources;
    private int _startingPoint;
    private int _endingPoint;
    private int _waveNumber;
    private int _enemiesCounter;
    private bool _buildingTurn;
    //TO CONSIDER: Replace custom class Point with build-in Unity class Vector2
    private Point _currentTile;
    private GameObject[,] _tiles;
	void Start ()
	{
	    _resources = 0;
	    _waveNumber = 0;
        _buildingTurn = true;
        _currentTile = new Point(0, 0);
        _startingPoint = FindObjectOfType<CreatePath>().StartingPoint;
        _endingPoint = FindObjectOfType<CreatePath>().EndingPoint;
        _tiles = new GameObject[FindObjectOfType<SpawnTiles>().Lenght, FindObjectOfType<SpawnTiles>().Width];
        _tiles = FindObjectOfType<SpawnTiles>().GetAllTiles();
        _enemiesCounter = 0;
        ChangeHighlightedTile(' ');
        UpdateResources(StartingResources);
        IncreaseWave();
	}
	
	void Update ()
    {
        if (_buildingTurn)
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
                            if (HasEnoughResources(BuildTower.TowerType.Wall))
                            {
                                SpawnWallTower();
                            }
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
                            if (HasEnoughResources(BuildTower.TowerType.Shooting))
                            {
                                SpawnShootingTower();
                            }
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
                            if (HasEnoughResources(BuildTower.TowerType.Aoe))
                            {
                                SpawnAoeTower();
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (HasChildObjects())
                {
                    if (FindObjectOfType<CreatePath>().FindPathAfterTowerRemoval(_currentTile.X, _currentTile.Y))
                    {
                        DestroyTower();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(BeginWave());
            }  
        }
        else
        {
            if (_enemiesCounter == 0)
            {
                EndWave();
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

    public void UpdateResources(int value)
    {
        _resources += value;
        GameObject.FindGameObjectWithTag("ResourcesText").GetComponent<GUIText>().text = "Resources: " + _resources;
    }

    public void IncreaseWave()
    {
        _waveNumber++;
        GameObject.FindGameObjectWithTag("WaveText").GetComponent<GUIText>().text = "Wave: " + _waveNumber;
    }

    public void GameOver()
    {
        //TO DO: menu with two buttons, restart and return to main menu?
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

    private IEnumerator BeginWave()
    {
        int enemiesCount = 3 * _waveNumber;
        int enemiesCounter = 0;
        _enemiesCounter = enemiesCount;
        _buildingTurn = false;
        FindObjectOfType<SpawnTiles>().RenderTiles(false);
        while (enemiesCounter < enemiesCount)
        {
            enemiesCounter++;
            yield return new WaitForSeconds(1.0f);
            SpawnEnemy();
        }
    }

    private void EndWave()
    {
        _buildingTurn = true;
        IncreaseWave();
        FindObjectOfType<SpawnTiles>().RenderTiles(true);
    }

    private void SpawnEnemy()
    {
        float temp = Random.Range(0.0f, 10.0f);
        if (temp >= 0.0f && temp < 3.3f)
        {
            Instantiate(Resources.Load("Prefabs/Enemies/Type1"), _tiles[0, _startingPoint].transform.position, Quaternion.identity);
        }
        else if (temp >= 3.3f && temp < 6.6f)
        {
            Instantiate(Resources.Load("Prefabs/Enemies/Type2"), _tiles[0, _startingPoint].transform.position, Quaternion.identity);
        }
        else if (temp >= 6.6f)
        {
            Instantiate(Resources.Load("Prefabs/Enemies/Type3"), _tiles[0, _startingPoint].transform.position, Quaternion.identity);
        }
    }

    public void DecreaseEnemyCounter()
    {
        if (!_buildingTurn)
        {
            _enemiesCounter--;
        }
    }

    private void SpawnWallTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Wall, _tiles[_currentTile.X, _currentTile.Y]));
        ApplyMaterialOnTiles();
    }

    private void SpawnShootingTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Shooting, _tiles[_currentTile.X, _currentTile.Y]));
        ApplyMaterialOnTiles();
    }

    private void SpawnAoeTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Aoe, _tiles[_currentTile.X, _currentTile.Y]));
        ApplyMaterialOnTiles();
    }

    private void DestroyTower()
    {
        UpdateResources(FindObjectOfType<BuildTower>().DestroyTower(_tiles[_currentTile.X, _currentTile.Y].transform.GetChild(0).gameObject));
        ApplyMaterialOnTiles();
    }

    private bool CanBuildTower()
    {
        return ((_currentTile.X != 0 || _currentTile.Y != _startingPoint) && (_currentTile.X != FindObjectOfType<SpawnTiles>().Lenght - 1 || _currentTile.Y != _endingPoint));
    }

    private bool HasChildObjects()
    {
        if (_tiles[_currentTile.X, _currentTile.Y].transform.childCount != 0)
        {
            return true;
        }
        return false;
    }

    private bool HasEnoughResources(BuildTower.TowerType type)
    {
        int temp = 0;
        switch (type)
        {
            case BuildTower.TowerType.Wall:
                temp = FindObjectOfType<BuildTower>().WallTower.Cost;
                break;
            case BuildTower.TowerType.Shooting:
                temp = FindObjectOfType<BuildTower>().ShootingTower.Cost;
                break;
            case BuildTower.TowerType.Aoe:
                temp = FindObjectOfType<BuildTower>().AoeTower.Cost;
                break;
        }
        return _resources - temp >= 0;
    }
}
