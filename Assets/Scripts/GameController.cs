using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private int _startingPoint;
    private int _endingPoint;
    private int [] _currentXYIndex;
    private int[,] _aValues;
    private GameObject[,] _tiles;
	void Start ()
	{
	    int length = FindObjectOfType<SpawnTiles>().Lenght;
	    int width = FindObjectOfType<SpawnTiles>().Width;
	    _startingPoint = Random.Range(0, length - 1);
        _endingPoint = Random.Range(0, length - 1);
	    _currentXYIndex = new[] {0, 0};
	    _tiles = new GameObject[length,width];
        _tiles = FindObjectOfType<SpawnTiles>().GetAllTiles();
	    Instantiate(Resources.Load("Prefabs/Start"), _tiles[0, _startingPoint].transform.position, Quaternion.identity);
        Instantiate(Resources.Load("Prefabs/End"), _tiles[length - 1, _endingPoint].transform.position, Quaternion.identity);
        _aValues = new int[length,width];
	    for (int i = 0; i < length; i++)
	    {
	        _aValues[_startingPoint, i] = i;
	    }
	    for (int i = 0; i < length; i++)
	    {
	        for (int j = 0; j < width; j++)
	        {
	            if (_startingPoint + j < length)
	            {
	                _aValues[i, _startingPoint + j] = j + i;
	            }
                if (_startingPoint - j > 0)
                {
                    _aValues[i, _startingPoint - j] = j + i;
                }
	        }
	    }
        ChangeHighlightedTile(' ');
	}
	
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.W))
	    {
	        if (_currentXYIndex[0] < FindObjectOfType<SpawnTiles>().Lenght - 1)
	        {
	            ChangeHighlightedTile('W');
	        }
	    }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_currentXYIndex[0] > 0)
            {
                ChangeHighlightedTile('S');
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (_currentXYIndex[1] > 0)
            {
                ChangeHighlightedTile('A');
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (_currentXYIndex[1] < FindObjectOfType<SpawnTiles>().Width - 1)
            {
                ChangeHighlightedTile('D');
            }
        }
        
	    if (Input.GetKeyDown(KeyCode.B))
	    {
            if (!((_currentXYIndex[0] == 0 && _currentXYIndex[1] == _startingPoint) || (_currentXYIndex[0] == FindObjectOfType<SpawnTiles>().Lenght - 1 && _currentXYIndex[1] == _endingPoint)))
            {
                if (_tiles[_currentXYIndex[0], _currentXYIndex[1]].transform.childCount == 0)
                {
                    GameObject baseTower = Instantiate(Resources.Load("Prefabs/Towers/Base"), _tiles[_currentXYIndex[0], _currentXYIndex[1]].transform.position, Quaternion.identity) as GameObject;
                    if (baseTower != null)
                    {
                        baseTower.transform.parent = _tiles[_currentXYIndex[0], _currentXYIndex[1]].transform;
                    }
                } 
            }
	    }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!((_currentXYIndex[0] == 0 && _currentXYIndex[1] == _startingPoint) || (_currentXYIndex[0] == FindObjectOfType<SpawnTiles>().Lenght - 1 && _currentXYIndex[1] == _endingPoint)))
            {
                if (_tiles[_currentXYIndex[0], _currentXYIndex[1]].transform.childCount == 0)
                {
                    GameObject baseTower = Instantiate(Resources.Load("Prefabs/Towers/Shooting"), _tiles[_currentXYIndex[0], _currentXYIndex[1]].transform.position, Quaternion.identity) as GameObject;
                    if (baseTower != null)
                    {
                        baseTower.transform.parent = _tiles[_currentXYIndex[0], _currentXYIndex[1]].transform;
                    }
                }
            }
        }

	    if (Input.GetKeyDown(KeyCode.X))
	    {
	        if (_tiles[_currentXYIndex[0], _currentXYIndex[1]].transform.childCount != 0)
	        {
	            Destroy(_tiles[_currentXYIndex[0], _currentXYIndex[1]].transform.GetChild(0).gameObject);
	        }
	    }
    }

    private void ChangeHighlightedTile(char pressedKey)
    {
        int[] newXY = new int[2];
        switch (pressedKey)
        {
            case 'W':
                newXY[0] = _currentXYIndex[0] + 1;
                newXY[1] = _currentXYIndex[1];
                break;
            case 'S':
                newXY[0] = _currentXYIndex[0] - 1;
                newXY[1] = _currentXYIndex[1];
                break;
            case 'A':
                newXY[0] = _currentXYIndex[0];
                newXY[1] = _currentXYIndex[1] - 1;
                break;
            case 'D':
                newXY[0] = _currentXYIndex[0];
                newXY[1] = _currentXYIndex[1] + 1;
                break;
            default:
                newXY[0] = _currentXYIndex[0];
                newXY[1] = _currentXYIndex[1];
                break;
        }
        _tiles[_currentXYIndex[0],_currentXYIndex[1]].GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Normal") as Material;
        _tiles[newXY[0],newXY[1]].GetComponent<Renderer>().material = Resources.Load("Materials/Tile/Highlighted") as Material;
        _currentXYIndex = newXY;
    }
}
