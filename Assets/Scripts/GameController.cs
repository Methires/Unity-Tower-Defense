using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Odpowiada za kontrolę całej pętli gry podczas rozgrywki w części rozgrywkowej.
/// </summary>
public class GameController : MonoBehaviour
{
    /// <summary>
    /// Zmienna określająca początkową liczbę zasobów gracza.
    /// </summary>
    public int StartingResources;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt z kamerą z której widoczny będzie obraz w trakcie trwania fazy przygotowania.
    /// </summary>
    public GameObject BuildingCamera;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt z kamerą z której widoczny będzie obraz w trakcie trwania fazy obrony.
    /// </summary>
    public GameObject Player;

    /// <summary>
    /// Zmienna określającą obecną liczbę zasobów gracza.
    /// </summary>
    private int _resources;
    /// <summary>
    /// Zmienna przechowującą indeks kolumny z siatki dwuwymiarowej w której znajduje się punkt startowy.
    /// </summary>
    private int _startingPoint;
    /// <summary>
    /// Zmienna przechowującą indeks kolumny z siatki dwuwymiarowej w której znajduje się punkt końcowy.
    /// </summary>
    private int _endingPoint;
    /// <summary>
    /// Zmienna określająca obecny numer fali.
    /// </summary>
    private int _waveNumber;
    /// <summary>
    /// Zmienna będąca licznikiem odliczającym koniec fazy obrony po zniszczeniu wszystkich przeciwników.
    /// </summary>
    private float _fpsPhaseEndCounter;
    /// <summary>
    /// Zmienna logiczna określająca czy obecnie trwa faza przygotowań.
    /// </summary>
    private bool _isBuildingPhase;
    /// <summary>
    /// Zmienna logiczna określająca czy wszyscy przeciwnicy z obecnej fali pojawi się na scenie.
    /// </summary>
    private bool _hasSpawnedAllEnemies;
    /// <summary>
    /// Zmienna logiczna określająca czy widoczne jest menu budowania wieży.
    /// </summary>
    private bool _isTowerMenuOpen;
    /// <summary>
    /// Zmienna logiczna określająca czy widoczne jest menu pauzy.
    /// </summary>
    private bool _isPauseMenuOpen;
    /// <summary>
    /// Zmienna logiczna określająca czy rozgrywka została zakończona.
    /// </summary>
    private bool _isGameOver;
    /// <summary>
    /// Zmienna przechowująca indeksy obecnie wybranego elementu siatki dwuwymiarowej.
    /// </summary>
    private Vector2 _currentTile;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt z widokiem widocznym w trakcie dwóch faz.
    /// </summary>
    private GameObject _uiBothPhases;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt z widokiem widocznym podczas fazy przygotowań.
    /// </summary>
    private GameObject _uiBuildingPhase;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt z memu pauzy.
    /// </summary>
    private GameObject _uiPauseMenu;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt z menu budowania wieży.
    /// </summary>
    private GameObject _uiTowerMenu;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt z widokiem widocznym podczas fazy obrony.
    /// </summary>
    private GameObject _uiShooterPhase;
    /// <summary>
    /// Zmienna przechowująca dwuwymiarową tablicę referencji na obiekty wchodzące w skład siatki dwuwymiarowej.
    /// </summary>
    private GameObject[,] _tiles;
	
    /// <summary>
	/// Metoda wywoływana po załadowaniu sceny na której znaduję się obiekt.
    /// Szuka obiektów zawierających każde z menu i widoków, a następnie przypisuje je do odpowiednich zmiennych klasy.
    /// </summary>
    void Awake()
    {
        _uiBothPhases = GameObject.FindWithTag("BothPhasesView");
        _uiBuildingPhase = GameObject.FindGameObjectWithTag("BuildingPhaseView");
        _uiPauseMenu = GameObject.FindGameObjectWithTag("PauseView");
        _uiTowerMenu = GameObject.FindGameObjectWithTag("TowerMenuView");
        _uiShooterPhase = GameObject.FindGameObjectWithTag("ShooterPhaseView");
    }
	
    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
	/// Ustawia wszystkie zmienne tak, aby możliwie było rozpoczęcie fazy przygotowania dla pierwszej fali.
    /// </summary>
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
	
	/// <summary>
	/// Metoda wywoływana co klatkę, gdy skrypt jest aktywny.
	/// Sprawdza czy klawisz naciśniety przez gracza powinien wywołać jakąś metodę i w takim przypadku wywołuje ją.
	/// </summary>
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
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if (_currentTile.x < FindObjectOfType<SpawnTiles>().Lenght - 1)
                        {
                            ChangeHighlightedTile('W');
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        if (_currentTile.x > 0)
                        {
                            ChangeHighlightedTile('S');
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (_currentTile.y > 0)
                        {
                            ChangeHighlightedTile('A');
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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
	
    /// <summary>
    /// Metoda odpowiedzialna za zmianę obecnie wybranego przez gracza elementu siatki dwuwymiarowej.
    /// </summary>
    /// <param name="pressedKey">Argument typu char na podstawie, którego określany jest nowy wybrany element.</param>
    private void ChangeHighlightedTile(char pressedKey)
    {
        switch (pressedKey)
        {
            case 'W':
                _currentTile.x = _currentTile.x + 1;
                break;
            case 'S':
                _currentTile.x = _currentTile.x - 1;
                break;
            case 'A':
                _currentTile.y = _currentTile.y - 1;
                break;
            case 'D':
                _currentTile.y = _currentTile.y + 1;
                break;
        }
        ApplyMaterialOnTiles();
    }
    
	/// <summary>
    /// Metoda zmieniająca wartość zasobów gracza o wartość value i aktualizująca tej informacji na ekranie.
    /// </summary>
    /// <param name="value">Argument typu int określający o ile musi zostać zwiększona liczba zasobów.</param>
    public void UpdateResources(int value)
    {
        _resources += value;
        _uiBothPhases.transform.GetChild(0).GetComponent<Text>().text = "Zasoby: " + _resources;
    }
    
	/// <summary>
    /// Metoda odpowiedzialna za zwiększenie numeru obecnej fali.
    /// </summary>
    public void IncreaseWave()
    {
	    if (_waveNumber == 10)
	    {
            GameOver();
	    }
	    else
	    {
	        _waveNumber++;
	        _uiBothPhases.transform.GetChild(1).GetComponent<Text>().text = "Fala: " + _waveNumber;
	    }
    }
    
	/// <summary>
    /// Metoda odpowiedzialna za obliczanie procentowej reprezentacji punktów życia punktu końcowego i aktualizacji tej informacji na ekranie.
    /// </summary>
    /// <param name="currentHealthPoints">Argument typu float określający obecną liczbę punktów życia punktu końcowego.</param>
    /// <param name="maxHealthPoints">Argument typu float określający maksymalną liczbę punktów życia punktu końcowego.</param>
    public void UpdateCoreLife(float currentHealthPoints, float maxHealthPoints)
    {
        var temp = Mathf.Round(currentHealthPoints/maxHealthPoints * 100.0f);
       _uiBothPhases.transform.GetChild(2).GetComponent<Text>().text = "Rdzen: " + temp;
    }
    
	/// <summary>
    /// Metoda odpowiedzialna akcje wywoływane na zakończenie gry.
    /// </summary>
    public void GameOver()
    {
        _isGameOver = true;
        BlockPlayer(false);
    }
    
	/// <summary>
    /// Metoda odpowiedzialna za blokowanie i odblokowywanie możliwości ruchu, obrotu i strzelania gracza oraz jego interfejs podczas fazy obrony.
    /// </summary>
    /// <param name="unblock">Argument typu bool, który określa czy zablokować gracza, gdy unblock = false albo odblokować, gdy unblock = true.</param>
    private void BlockPlayer(bool unblock)
    {
        Player.GetComponent<FirstPersonController>().BlockMovement = !unblock;
        Player.GetComponent<PlayerStats>().enabled = unblock;
        Player.GetComponent<PlayerStats>().Shooting.enabled = unblock;
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za ukrywanie i pokazywanie kursora myszy.
    /// </summary>
    /// <param name="show">Argument typu bool, który określa czy kursor myszy ma być widoczny, gdy show = true albo niewidoczny, gdy show = false.</param>
    private void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za przeładowanie gry celem jej ponownego rozpoczęcia.
    /// </summary>
    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
	
    /// <summary>
    /// Metoda odpowiedzialna na przypisywanie odpowiedniego materiału do wszystkich elementów z których składa się siatka dwuwymiarowa.
    /// </summary>
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
	
    /// <summary>
    /// Metoda odpowiedzialna za rozpoczęcie fazy obrony dla obecnej fali przeciwników.
    /// </summary>
    /// <returns>IEnumerator pozwalający na wykorzystanie mechanizmu yield.</returns>
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
	
    /// <summary>
    /// Metoda kończąca fazę obrony dla obecnej fali przeciwników.
    /// </summary>
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
	
    /// <summary>
    /// Metoda odpowiedzialna na aktywację obiektu, który posiada komponent kamery przez którą gracz będzie widział rozgrywkę w zależności o fazy.
    /// </summary>
    /// <param name="isBuildingPhase">Argument typu bool, który określa czy obecnie rozpocznie się faza przygotowania, gdy isBuildingPhase = true albo faza obrony, 
	/// gdy isBuildingPhase = false </param>
    private void ChangeMainCamera(bool isBuildingPhase)
    {
        Player.SetActive(!isBuildingPhase);
        BuildingCamera.SetActive(isBuildingPhase);
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za dodanie nowego przeciwnika na scenę.
    /// </summary>
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
	
    /// <summary>
    /// Metoda odpowiadająca za wywołanie odpowiednej metody w skrypcie BuildTower tworzącej wieżę typu ściana.
    /// </summary>
    private void SpawnWallTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Wall, _tiles[(int)_currentTile.x, (int)_currentTile.y]));
        ApplyMaterialOnTiles();
    }
	
    /// <summary>
    /// Metoda odpowiadająca za wywołanie odpowiednej metody w skrypcie BuildTower tworzącej wieżę strzelającą.
    /// </summary>
    private void SpawnShootingTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Shooting, _tiles[(int) _currentTile.x, (int) _currentTile.y]));
        ApplyMaterialOnTiles();
    }
	
    /// <summary>
    /// Metoda odpowiadająca za wywołanie odpowiednej metody w skrypcie BuildTower tworzącej wieżę obszarową.
    /// </summary>
    private void SpawnAoeTower()
    {
        UpdateResources(-FindObjectOfType<BuildTower>().SpawnTower(BuildTower.TowerType.Aoe, _tiles[(int) _currentTile.x, (int) _currentTile.y]));
        ApplyMaterialOnTiles();
    }
	
    /// <summary>
    /// Metoda odpowiadająca za wywołanie odpowiednej metody w skrypcie BuildTower usuwającej wieżę z obecniej wybranego elementu siatki dwuwymiarowej.
    /// </summary>
    private void DestroyTower()
    {
        UpdateResources(FindObjectOfType<BuildTower>().DestroyTower(_tiles[(int) _currentTile.x, (int) _currentTile.y].transform.GetChild(0).gameObject));
        ApplyMaterialOnTiles();
    }
	
    /// <summary>
    /// Metoda określająca czy na obecnie wybranym elemencie siatki dwuwymiarowej można utowrzyć jakąkolwiek wieżę. 
    /// </summary>
    /// <returns>Zwraca wartość bool = true, gdy na obecnie wybranym elemencie siatki dwuwymiarowej można utowrzyć wieżę
	///	albo false, gdy na obecnie wybranym elemencie siatki dwuwymiarowej nie można utworzyć wieży.</returns>
    private bool CanBuildTower()
    {
        return ((_currentTile.x != 0 || _currentTile.y != _startingPoint) && (_currentTile.x != FindObjectOfType<SpawnTiles>().Lenght - 1 || _currentTile.y != _endingPoint));
    }
	
    /// <summary>
    /// Metoda sprawdzająca czy obecnie wybrany element siatki dwuwymiarowej jest rodzicem.
    /// </summary>
    /// <returns>Zwraca wartość bool = true, gdy obecnie wybrany element siatki dwuwymiarowej jest rodzicem albo false, gdy obecnie wybrany element siatki dwuwymiarowej nie jest rodzicem.</returns>
    private bool HasChildObjects()
    {
        if (_tiles[(int) _currentTile.x, (int) _currentTile.y].transform.childCount != 0)
        {
            return true;
        }
        return false;
    }
	
    /// <summary>
    /// Metoda sprawdzająca czy gracz posiada odpowiednią liczbę zasobów potrzebną do wybudowania danej wieży.
    /// </summary>
    /// <param name="type">Argument typu BuildTower.TowerType określający, który koszt wieży ma zostać sprawdzony.</param>
    /// <returns>Zwraca wartość bool = true, gdy gracz posiada wystarczającą liczbę zasobów albo false, gdy gracz nie posida wystarczającej liczby zasobów.</returns>
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
	
    /// <summary>
    /// Metoda określająca stan aktywności wszystkich wież obecnych na scenie.
    /// </summary>
    /// <param name="value">Argument typu bool określający, czy wieże mają być aktywne, gdy value = true albo nieaktywne, gdy value = false.</param>
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
	
    /// <summary>
    /// Metoda przypisana pod przyciski w menu budowania. Odpowiada za wywołanie metody tworzącej obiekt wieży odpowiednej w zależności o argumentu metody.
    /// </summary>
    /// <param name="towerType">Argument typu string określący jaka wieża ma zostać wybudowana.</param>
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
	
    /// <summary>
    /// Metoda przypisana pod przycisk w menu budowania. Odpowiada za wywołanie metody niszczącej obiekt wieży.
    /// </summary>
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
	
    /// <summary>
    /// Metoda przypisana pod przycisk podczas fazy przygotowania. Odpowiada za rozpoczęcie fazy obrony.
    /// </summary>
    public void StartWaveWithButton()
    {
        StartCoroutine(BeginWave());
    }
	
    /// <summary>
    /// Metoda przypisana pod przycisk podczas fazy przygotowania. Otwiera albo zamyka menu budowania i usuwania wież.
    /// </summary>
    /// <param name="isVisible">Agrument typu bool określący czy menu ma być widoczne, gdy isVisible = true albo niewidoczne, gdy isVisible = false.</param>
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
	
    /// <summary>
    /// Metoda odpowiedzialna za otwieranie albo zamykanie menu pauzy podczas każdej z faz.
    /// </summary>
    /// <param name="isVisible">Agrument typu bool określący czy menu ma być widoczne, gdy isVisible = true albo niewidoczne, gdy isVisible = false.</param>
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
	
    /// <summary>
    /// Metoda odpowiedzialna za zaktualizowanie informacji na ekranie odpowiadającej liczbie amunicji w broni i pozostałej amunicji do wykorzystania.
    /// </summary>
    /// <param name="currentAmmo">Argument typu int, który określa ile amunicji jest obecnie w magazyku broni.</param>
    /// <param name="maxAmmo">Argument typu int, który określa liczbę amunicji do wykorzystania.</param>
    public void UpdateAmmoOnShooterUi(int currentAmmo, int maxAmmo)
    {
        _uiShooterPhase.transform.GetChild(0).GetComponent<Text>().text = "Amunicja: " + currentAmmo + "/" + maxAmmo;
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za przejście do menu głównego podczas trwania gry rozrywkowej.
    /// </summary>
    public void Exit()
    {
        Application.LoadLevel("MainMenu");
    }
}
