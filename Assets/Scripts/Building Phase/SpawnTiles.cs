using UnityEngine;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Odpowiedzialna za tworzenie pól siatki dwuwymiarowej na początku rozgrywki, przechowująca wszystkie obiekty wchodzące w skład dwuwymiarowej siatki.
/// </summary>
public class SpawnTiles : MonoBehaviour
{
    /// <summary>
    /// Zmienna przechowująca referencję na obiekt z którego składać będzie się siatka dwuwymiarowa.
    /// </summary>
    public GameObject Tile;
    /// <summary>
    /// Zmienna określająca wysokość siatki dwuwymiarowej.
    /// </summary>
    [Header("Sizes")]
    public int Lenght;
    /// <summary>
    /// Zmienna określająca szerokość siatki duwymiarowej.
    /// </summary>
    public int Width;

    /// <summary>
    /// Zmienna przechowująca dwuwymiarową tablicę referencji na obiekty wchodzące w skład siatki dwuwymiarowej.
    /// </summary>
    private GameObject[,] _tiles;
	
    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
	/// Tworzy elemnenty składające się na dwuwymiarową siatkę.
    /// </summary>
	void Start ()
    {
        _tiles = new GameObject[Lenght, Width];
        for (int i = 0; i < Lenght; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                GameObject tile = Instantiate(Tile);
                tile.transform.parent = transform;
                Vector3 position = new Vector3
                {
                    x = j * 10.5f,
                    y = 0.0f,
                    z = i * 10.5f
                };
                tile.transform.localPosition = position;
                _tiles[i, j] = tile;
            }
        }
    }
	
    /// <summary>
    /// Metoda zwracająca wszystkie obiekty będące polami dwuwymiarowej siatki.
    /// </summary>
    /// <returns>Tablica dwuwymiarowa z referencjami na obiekty wchodzę w skład siatki dwuwymiarowej</returns>
    public GameObject[,] GetAllTiles()
    {
        return _tiles;
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za włączanie, dla argumentu value = true, albo wyłączanie, dla argumentu value = false, Renderera wszystkich pól siatki dwuwymiarowej.
    /// </summary>
    /// <param name="value">Argument typu bool, który określa czy elementy mają być widoczne, gdy value = true albo niewidoczne, gdy value = false.</param>
    public void RenderTiles(bool value)
    {
        foreach (var tile in _tiles)
        {
            tile.GetComponent<Renderer>().enabled = value;
        }
    }
}
