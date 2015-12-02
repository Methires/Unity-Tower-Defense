using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int StartingResources;
    private int _resources;
    private int _startingPoint;
    private int _endingPoint;
    private int _waveNumber;
    private int _enemiesCounter;
    private bool _buildingTurn;
    private bool _gameOver;
    private Vector2 _currentTile;
    private GameObject[,] _tiles;
	void Start ()
	{
	    _resources = 0;
	    _waveNumber = 0;
        _buildingTurn = true;
	    _gameOver = false;
        _currentTile = new Vector2(0, 0);
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
        if (!_gameOver)
        {
            if (_buildingTurn)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (_currentTile.x < FindObjectOfType<SpawnTiles>().Lenght - 1)
                    {
                        ChangeHighlightedTile('W');
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (_currentTile.x > 0)
                    {
                        ChangeHighlightedTile('S');
                    }
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (_currentTile.y > 0)
                    {
                        ChangeHighlightedTile('A');
                    }
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (_currentTile.y < FindObjectOfType<SpawnTiles>().Width - 1)
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
                            if (FindObjectOfType<CreatePath>().ViablePath((int)_currentTile.x, (int)_currentTile.y))
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
                            if (FindObjectOfType<CreatePath>().ViablePath((int)_currentTile.x, (int)_currentTile.y))
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
                            if (FindObjectOfType<CreatePath>().ViablePath((int)_currentTile.x, (int)_currentTile.y))
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
                        if (FindObjectOfType<CreatePath>().FindPathAfterTowerRemoval((int)_currentTile.x, (int)_currentTile.y))
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
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }
    }

    private void ChangeHighlightedTile(char pressedKey)
    {
        switch (pressedKey)
        {
            case 'W':
                _currentTile.x = _currentTile.x + 1;
                _currentTile.y = _currentTile.y;
                break;
            case 'S':
                _currentTile.x = _currentTile.x - 1;
                _currentTile.y = _currentTile.y;
                break;
            case 'A':
                _currentTile.x = _currentTile.x;
                _currentTile.y = _currentTile.y - 1;
                break;
            case 'D':
                _currentTile.x = _currentTile.x;
                _currentTile.y = _currentTile.y + 1;
                break;
            default:
                _currentTile.x = _currentTile.x;
                _currentTile.y = _currentTile.y;
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

    public void UpdateCoreLife(float currentHealthPoints, float maxHealthPoints)
    {
        var temp = Mathf.Round(currentHealthPoints/maxHealthPoints * 100.0f);
        GameObject.FindGameObjectWithTag("CoreText").GetComponent<GUIText>().text = "Core: " + temp + "%";
    }

    public void GameOver()
    {
        _gameOver = true;
    }

    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
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
        _tiles[(int)_currentTile.x, (int)_currentTile.y].GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Highlighted") as Material;
    }

    private IEnumerator BeginWave()
    {
        int enemiesCount = 3 * _waveNumber;
        int enemiesCounter = 0;
        _enemiesCounter = enemiesCount;
        _buildingTurn = false;
        FindObjectOfType<SpawnTiles>().RenderTiles(false);
        ActivateTowers(true);
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
        ActivateTowers(false);
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
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Wall, _tiles[(int)_currentTile.x, (int)_currentTile.y]));
        ApplyMaterialOnTiles();
    }

    private void SpawnShootingTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Shooting, _tiles[(int) _currentTile.x, (int) _currentTile.y]));
        ApplyMaterialOnTiles();
    }

    private void SpawnAoeTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Aoe, _tiles[(int) _currentTile.x, (int) _currentTile.y]));
        ApplyMaterialOnTiles();
    }

    private void DestroyTower()
    {
        UpdateResources(FindObjectOfType<BuildTower>().DestroyTower(_tiles[(int) _currentTile.x, (int) _currentTile.y].transform.GetChild(0).gameObject));
        ApplyMaterialOnTiles();
    }

    private bool CanBuildTower()
    {
        return ((_currentTile.x != 0 || _currentTile.y != _startingPoint) && (_currentTile.x != FindObjectOfType<SpawnTiles>().Lenght - 1 || _currentTile.y != _endingPoint));
    }

    private bool HasChildObjects()
    {
        if (_tiles[(int) _currentTile.x, (int) _currentTile.y].transform.childCount != 0)
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

    private void ActivateTowers(bool value)
    {
        var aoeTowers = FindObjectsOfType<AoeTowerBehaviour>();
        foreach (var aTower in aoeTowers)
        {
            aTower.GetComponent<AoeTowerBehaviour>().SetAttackPhase(value);
        }
        var shootingTowers = FindObjectsOfType<ShootingTowerBehaviour>();
        foreach (var sTower in shootingTowers)
        {
            sTower.GetComponent<ShootingTowerBehaviour>().SetAttackPhase(value);
        }
    }
}
