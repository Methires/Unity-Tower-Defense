using UnityEngine;

public class GameController : MonoBehaviour
{
    private int _startingPoint;
    private int _endingPoint;
    private int[,] _aValues;
    private GameObject[,] _tiles;
	void Start ()
	{
	    int length = FindObjectOfType<SpawnTiles>().Lenght;
	    int width = FindObjectOfType<SpawnTiles>().Width;
	    _startingPoint = Random.Range(0, length - 1);
        _endingPoint = Random.Range(0, length - 1);
	    _tiles = FindObjectOfType<SpawnTiles>().GetAllTiles();
        _aValues = new int[length,width];
	}
	
	void Update ()
    {	
	}
}
