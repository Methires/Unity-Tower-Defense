using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private bool _isInfoMenuOpen;
    private bool _isTowerInfoMenuOpen;
    private bool _isEnemyInfoMenuOpen;
    private bool _isTextMenuOpen;
    private GameObject _mainMenuUi;
    private GameObject _infoMenuUi;
    private GameObject _towerInfoMenuUi;
    private GameObject _enemyInfoMenuUi;
    private GameObject _textMenuUi;

	void Start () 
    {
        _mainMenuUi = GameObject.Find("Canvas: Main Menu");
        _infoMenuUi = GameObject.Find("Canvas: Info Menu");
        _towerInfoMenuUi = GameObject.Find("Canvas: Tower Info");
        _enemyInfoMenuUi = GameObject.Find("Canvas: Enemy Info");
        _textMenuUi = GameObject.Find("Canvas: Text");
        OpenMenu("Main");
	}
	
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
                    = "Laser Gun";
                textFile = Resources.Load("Texts/Defences/Gun") as TextAsset;
                Debug.Log(_textMenuUi.transform.GetChild(1).transform.GetChild(0).name);
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                     
                break;
            case "Shoot":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Nanobots";
                textFile = Resources.Load("Texts/Defences/Shoot") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
            case "AOE":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Chemotheraphy AOE";
                textFile = Resources.Load("Texts/Defences/AOE") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
            case "Type1":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Cancer cell 1";
                textFile = Resources.Load("Texts/Enemies/Type1") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
            case "Type2":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Cancer cell 2";
                textFile = Resources.Load("Texts/Enemies/Type2") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
            case "Type3":
                _textMenuUi.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text
                    = "Cancer cell 3";
                textFile = Resources.Load("Texts/Enemies/Type3") as TextAsset;
                _textMenuUi.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = textFile.text;
                break;
        }
    }

    public void StartGame()
    {
        Application.LoadLevel("MainGame");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
