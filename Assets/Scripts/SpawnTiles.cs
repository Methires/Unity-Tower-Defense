using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    public GameObject Tile;
    [Header("Sizes")] 
    public int Lenght;
    public int Width;

    private GameObject[,] _tiles;
	void Awake ()
    {
        _tiles = new GameObject[Lenght,Width];
	    for (int i = 0; i < Lenght; i++)
	    {
	        for (int j = 0; j < Width; j++)
	        {
	            GameObject tile = Instantiate(Tile);
	            tile.transform.parent = transform;
                Vector3 position = new Vector3
                {
                    x = j * 1.05f,
                    y = 0.0f,
                    z = i * 1.05f
                };
	            tile.transform.localPosition = position;
	            _tiles[i, j] = tile;
	        }
	    }
    }

    public GameObject[,] GetAllTiles()
    {
        return _tiles;
    }
}
