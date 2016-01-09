using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. 
/// Odpowiada za kontrolę całej pętli gry podczas przebywania w głównym menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Zmienna logiczna określająca czy widoczne jest menu wyboru typu informacji.
    /// </summary>
    private bool _isInfoMenuOpen;
    /// <summary>
    /// Zmienna logiczna określająca czy widoczne jest menu wyboru informacji o sposobach leczenia.
    /// </summary>
    private bool _isTowerInfoMenuOpen;
    /// <summary>
    /// Zmienna logiczna określająca czy widoczne jest menu wyboru informacji o chorobach nowotworowych.
    /// </summary>
    private bool _isEnemyInfoMenuOpen;
    /// <summary>
    /// Zmienna logiczna określająca czy widoczne jest widok przedstawiania wybranej informacji.
    /// </summary>
    private bool _isTextMenuOpen;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt zawierący menu główne.
    /// </summary>
    private GameObject _mainMenuUi;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt zawierący menu wyboru typu informacji.
    /// </summary>
    private GameObject _infoMenuUi;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt zawierący menu wyboru informacji o sposobach leczenia.
    /// </summary>
    private GameObject _towerInfoMenuUi;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt zawierący menu wyboru informacji o chorobach nowotworowych.
    /// </summary>
    private GameObject _enemyInfoMenuUi;
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt zawierący widok przedstawiania wybranej informacji.
    /// </summary>
    private GameObject _textMenuUi;
	
    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
	/// Szuka obiektów zawierających każde z menu i widoków, a następnie przypisuje je do odpowiednich zmiennych klasy.
    /// </summary>
	void Start () 
    {
        _mainMenuUi = GameObject.Find("Canvas: Main Menu");
        _infoMenuUi = GameObject.Find("Canvas: Info Menu");
        _towerInfoMenuUi = GameObject.Find("Canvas: Tower Info");
        _enemyInfoMenuUi = GameObject.Find("Canvas: Enemy Info");
        _textMenuUi = GameObject.Find("Canvas: Text");
        OpenMenu("Main");
	}
	
	/// <summary>
	/// Metoda wywoływana co klatkę, gdy skrypt jest aktywny.
	/// Sprawdza czy naciśnięty został klawisz ESC, którego wciśnięcie zamyka kolejne menu, aż do powrotu do menu głównego.
	/// </summary>
	void Update () 
    {
	    if (Input.GetButtonDown("Cancel"))
	    {
	        if (_isInfoMenuOpen)
	        {
                OpenMenu("Main");
            }
            else if (_isTowerInfoMenuOpen || _isEnemyInfoMenuOpen || _isTextMenuOpen)
            {
                OpenMenu("Info");
            }
	    }
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za otworzenie odpowiedniego menu 
	/// z wszystkich możliwych menu dostępnych przed rozpoczęciem części rozrywkowej gry.
    /// </summary>
    /// <param name="menuType">Argument typu string określący, które menu ma zostać pokazane</param>
    public void OpenMenu(string menuType)
    {
        switch (menuType)
        {
            case "Main": _mainMenuUi.SetActive(true);
                _infoMenuUi.SetActive(false);
                _isInfoMenuOpen = false;
                _towerInfoMenuUi.SetActive(false);
                _isTowerInfoMenuOpen = false;
                _enemyInfoMenuUi.SetActive(false);
                _isEnemyInfoMenuOpen = false;
                _textMenuUi.SetActive(false);
                _isTextMenuOpen = false;
                break;
            case "Info": _mainMenuUi.SetActive(false);
                _infoMenuUi.SetActive(true);
                _isInfoMenuOpen = true;
                _towerInfoMenuUi.SetActive(false);
                _isTowerInfoMenuOpen = false;
                _enemyInfoMenuUi.SetActive(false);
                _isEnemyInfoMenuOpen = false;
                _textMenuUi.SetActive(false);
                _isTextMenuOpen = false;
                break;
            case "Tower":
                _mainMenuUi.SetActive(false);
                _infoMenuUi.SetActive(false);
                _isInfoMenuOpen = false;
                _towerInfoMenuUi.SetActive(true);
                _isTowerInfoMenuOpen = true;
                _enemyInfoMenuUi.SetActive(false);
                _isEnemyInfoMenuOpen = false;
                _textMenuUi.SetActive(false);
                _isTextMenuOpen = false;
                break;
            case "Enemy":
                _mainMenuUi.SetActive(false);
                _infoMenuUi.SetActive(false);
                _isInfoMenuOpen = false;
                _towerInfoMenuUi.SetActive(false);
                _isTowerInfoMenuOpen = false;
                _enemyInfoMenuUi.SetActive(true);
                _isEnemyInfoMenuOpen = true;
                _textMenuUi.SetActive(false);
                _isTextMenuOpen = false;
                break;
        }
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za wyświetlenie odpowiedniego tekstu 
	/// i nagłówka w widoku przedstawiania informacji w zależności od wybranej informacji.
    /// </summary>
    /// <param name="textFileName">Argument typu string określający nazwę pliku, który zawiera tekst z informacjami.</param>
    public void OpenTextMenu(string textFileName)
    {
        _mainMenuUi.SetActive(false);
        _infoMenuUi.SetActive(false);
        _isInfoMenuOpen = false;
        _towerInfoMenuUi.SetActive(false);
        _isTowerInfoMenuOpen = false;
        _enemyInfoMenuUi.SetActive(false);
        _isEnemyInfoMenuOpen = false;
        _textMenuUi.SetActive(true);
        _isTextMenuOpen = true;
        _textMenuUi.transform.GetChild(1).GetComponent<ScrollRect>().verticalNormalizedPosition = 1.0f;
        TextAsset textFile;

        switch (textFileName)
        {
            case "Gun":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Cyberknife";
                textFile = Resources.Load("Texts/Defences/Gun") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                     
                break;
            case "Shoot":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Nanotechnologia";
                textFile = Resources.Load("Texts/Defences/Shoot") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
            case "AOE":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Chemioterapia";
                textFile = Resources.Load("Texts/Defences/AOE") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
            case "Type1":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Rak nerki";
                textFile = Resources.Load("Texts/Enemies/Type1") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
            case "Type2":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Rak pluc";
                textFile = Resources.Load("Texts/Enemies/Type2") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
            case "Type3":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Chloniaki nieziarnicze ";
                textFile = Resources.Load("Texts/Enemies/Type3") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
        }
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za rozpoczęcie części rozgrywkowej gry.
    /// </summary>
    public void StartGame()
    {
        Application.LoadLevel("MainGame");
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za zamknięcie programu.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
