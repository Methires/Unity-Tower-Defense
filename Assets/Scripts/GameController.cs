using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int StartingResources;
    public GameObject BuildingCamera;
    public GameObject Player;

    private int _resources;
    private int _startingPoint;
    private int _endingPoint;
    private int _waveNumber;
    private float _fpsPhaseEndCounter;
    private bool _isBuildingPhase;
    private bool _hasSpawnedAllEnemies;
    private bool _isTowerMenuOpen;
    private bool _isPauseMenuOpen;
    private bool _isGameOver;
    private Vector2 _currentTile;
    private GameObject _uiBothPhases;
    private GameObject _uiBuildingPhase;
    private GameObject _uiPauseMenu;
    private GameObject _uiTowerMenu;
    private GameObject _uiShooterPhase;
    private GameObject[,] _tiles;

    void Awake()
    {
        _uiBothPhases = GameObject.FindWithTag("BothPhasesView");
        _uiBuildingPhase = GameObject.FindGameObjectWithTag("BuildingPhaseView");
        _uiPauseMenu = GameObject.FindGameObjectWithTag("PauseView");
        _uiTowerMenu = GameObject.FindGameObjectWithTag("TowerMenuView");
        _uiShooterPhase = GameObject.FindGameObjectWithTag("ShooterPhaseView");
    }

    void Start ()
	{
        UpdateResources(StartingResources);
        IncreaseWave();
        _isBuildingPhase = true;
        _uiShooterPhase.SetActive(false);
        _currentTile = new Vector2(0, 0);
        var path = FindObjectOfType<CreatePath>();
        _startingPoint = path.StartingPoint;
        _endingPoint = path.EndingPoint;
        var spawnTiles = FindObjectOfType<SpawnTiles>();
        _tiles = new GameObject[spawnTiles.Lenght, spawnTiles.Width];
        _tiles = spawnTiles.GetAllTiles();
        ChangeMainCamera(true);
        ChangeHighlightedTile(' ');
        OpenPauseMenuUi(false);
        OpenTowerMenuUi(false);
	}
	
	void Update ()
    {
        if (!_isGameOver)
        {      
            if (!_isPauseMenuOpen && !_isTowerMenuOpen)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    OpenPauseMenuUi(true);
                }  
            }
            else if(_isPauseMenuOpen && !_isTowerMenuOpen)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    OpenPauseMenuUi(false);
                }
            }

            if (_isBuildingPhase)
            {
                if (!_isTowerMenuOpen && !_isPauseMenuOpen)
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
                }
                else if (_isTowerMenuOpen)
                {
                    if (Input.GetButtonDown("Cancel"))
                    {
                        OpenTowerMenuUi(false);
                    }
                }
            } 
            else
            {
                if (_hasSpawnedAllEnemies)
                {
                    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    if (enemies.Length == 0)
                    {
                        _fpsPhaseEndCounter += Time.deltaTime;
                        if (_fpsPhaseEndCounter >= 2.0f)
                        {
                            EndWave();
                        }
                    }
                }
            }       
        }
        else
        {
            OpenPauseMenuUi(true);
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
        _uiBothPhases.transform.GetChild(0).GetComponent<Text>().text = "Resources: " + _resources;
    }

    public void IncreaseWave()
    {
        _waveNumber++;
        _uiBothPhases.transform.GetChild(1).GetComponent<Text>().text = "Wave: " + _waveNumber;
    }

    public void UpdateCoreLife(float currentHealthPoints, float maxHealthPoints)
    {
        var temp = Mathf.Round(currentHealthPoints/maxHealthPoints * 100.0f);
       _uiBothPhases.transform.GetChild(2).GetComponent<Text>().text = "Core: " + temp;
    }

    public void GameOver()
    {
        _isGameOver = true;
        BlockPlayer(false);
    }

    private void BlockPlayer(bool unblock)
    {
        Player.GetComponent<FirstPersonController>().BlockMovement = !unblock;
        Player.GetComponent<PlayerStats>().enabled = unblock;
        Player.GetComponent<PlayerStats>().Shooting.enabled = unblock;
    }

    private void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
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
        _isBuildingPhase = false;
        _hasSpawnedAllEnemies = false;
        FindObjectOfType<SpawnTiles>().RenderTiles(false);
        ActivateTowers(true);
        ChangeMainCamera(false);
        var path = FindObjectOfType<CreatePath>().GetPath();
        Player.transform.position = path[path.Count - 2].transform.position;
        _uiBuildingPhase.SetActive(false);
        _uiShooterPhase.SetActive(true);
        ShowCursor(false);
        while (enemiesCounter < enemiesCount)
        {
            enemiesCounter++;
            yield return new WaitForSeconds(1.0f);
            SpawnEnemy();
        }
        _hasSpawnedAllEnemies = true;
    }

    private void EndWave()
    {
        _isBuildingPhase = true;
        _fpsPhaseEndCounter = 0.0f;
        IncreaseWave();
        FindObjectOfType<SpawnTiles>().RenderTiles(true);
        ActivateTowers(false);
        ChangeMainCamera(true);
        if (!_isPauseMenuOpen)
        {
            _uiBuildingPhase.SetActive(true);
            _uiShooterPhase.SetActive(false);
        }
        BlockPlayer(true);
        ShowCursor(true);
    }

    private void ChangeMainCamera(bool isBuildingPhase)
    {
        Player.SetActive(!isBuildingPhase);
        BuildingCamera.SetActive(isBuildingPhase);
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

    public void SpawnTowerWithButton(string towerType)
    {
        switch (towerType)
        {
            case "Wall":
                if (HasEnoughResources(BuildTower.TowerType.Wall))
                {
                    SpawnWallTower();
                }
                break;
            case "AOE":
                if (HasEnoughResources(BuildTower.TowerType.Aoe))
                {
                    SpawnAoeTower();
                }
                break;
            case "Shoot":
                if (HasEnoughResources(BuildTower.TowerType.Shooting))
                {
                    SpawnShootingTower();
                }
                break;
        }
        OpenTowerMenuUi(false);
    }

    public void DestroyTowerWithButton()
    {
        if (HasChildObjects())
        {
            if (FindObjectOfType<CreatePath>().FindPathAfterTowerRemoval((int)_currentTile.x, (int)_currentTile.y))
            {
                DestroyTower();
            }
            OpenTowerMenuUi(false);
        }
    }

    public void StartWaveWithButton()
    {
        StartCoroutine(BeginWave());
    }

    public void OpenTowerMenuUi(bool isVisible)
    {
        _isTowerMenuOpen = isVisible;
        _uiBuildingPhase.SetActive(!isVisible);
        _uiTowerMenu.SetActive(isVisible);
        if (isVisible)
        {
            if (HasChildObjects())
            {
                _uiTowerMenu.transform.GetChild(0).GetComponent<Button>().interactable = false;
                _uiTowerMenu.transform.GetChild(1).GetComponent<Button>().interactable = false;
                _uiTowerMenu.transform.GetChild(2).GetComponent<Button>().interactable = false;
                _uiTowerMenu.transform.GetChild(3).GetComponent<Button>().interactable = true;
            }
            else
            {
                if (CanBuildTower() && FindObjectOfType<CreatePath>().ViablePath((int) _currentTile.x, (int) _currentTile.y))
                {
                    if (HasEnoughResources(BuildTower.TowerType.Wall))
                    {
                        _uiTowerMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        _uiTowerMenu.transform.GetChild(0).GetComponent<Button>().interactable = false;
                    }

                    if (HasEnoughResources(BuildTower.TowerType.Shooting))
                    {
                        _uiTowerMenu.transform.GetChild(1).GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        _uiTowerMenu.transform.GetChild(1).GetComponent<Button>().interactable = false;
                    }
                    if(HasEnoughResources(BuildTower.TowerType.Aoe))
                    {
                        _uiTowerMenu.transform.GetChild(2).GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        _uiTowerMenu.transform.GetChild(2).GetComponent<Button>().interactable = false;
                    }
                }
                else
                {
                    _uiTowerMenu.transform.GetChild(0).GetComponent<Button>().interactable = false;
                    _uiTowerMenu.transform.GetChild(1).GetComponent<Button>().interactable = false;
                    _uiTowerMenu.transform.GetChild(2).GetComponent<Button>().interactable = false;
                }
                _uiTowerMenu.transform.GetChild(3).GetComponent<Button>().interactable = false;
            }
            
        }
        else
        {
            _uiTowerMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;
            _uiTowerMenu.transform.GetChild(1).GetComponent<Button>().interactable = true;
            _uiTowerMenu.transform.GetChild(2).GetComponent<Button>().interactable = true;
            _uiTowerMenu.transform.GetChild(3).GetComponent<Button>().interactable = true;
            _uiTowerMenu.transform.GetChild(4).GetComponent<Button>().interactable = true;
        }
    }

    public void OpenPauseMenuUi(bool isVisible)
    {
        _isPauseMenuOpen = isVisible;
        _uiBothPhases.SetActive(!isVisible);
        _uiPauseMenu.SetActive(isVisible);
        if (_isBuildingPhase)
        {
            _uiBuildingPhase.SetActive(!isVisible);
        }
        else
        {
            BlockPlayer(!isVisible);
            ShowCursor(isVisible);
            _uiShooterPhase.SetActive(!isVisible);
        }
        if (_isGameOver)
        {
            _uiPauseMenu.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
    }

    public void UpdateAmmoOnShooterUi(int currentAmmo, int maxAmmo)
    {
        _uiShooterPhase.transform.GetChild(0).GetComponent<Text>().text = "Ammo: " + currentAmmo + "/" + maxAmmo;
    }

    public void Exit()
    {
        Application.LoadLevel("MainMenu");
    }
}
