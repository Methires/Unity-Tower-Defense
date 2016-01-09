using UnityEngine;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Odpowiedzialna za budowę i destrukcję wież.
/// </summary>
public class BuildTower : MonoBehaviour
{
    /// <summary>
    /// Typ wyliczeniowy określający typ wieży.
    /// </summary>
    public enum TowerType
    {
        /// <summary>
        /// Typ określający wieżę typu ściana.
        /// </summary>
        Wall,
        /// <summary>
        /// Typ określający wieżę strzelającą.
        /// </summary>
        Shooting,
        /// <summary>
        /// Typ określający wieżę obszarową.
        /// </summary>
        Aoe
    }
    /// <summary>
    /// Struktura przechowująca informację o typie i koszcie wybudowania wieży.
    /// </summary>
    public struct Tower
    {
        /// <summary>
        /// Zmienna przechowująca informację o typie wieży.
        /// </summary>
        public TowerType Type;
        /// <summary>
        /// Zmienna przechowująca informację o koszcie wybudowania wieży.
        /// </summary>
        public int Cost;
    }
    /// <summary>
    /// Zmienna typu Tower przechowująca informacje dla wieży typu ściana.
    /// </summary>
    public Tower WallTower;
    /// <summary>
    /// Zmienna typu Tower przechowująca informacje dla wieży strzelającej.
    /// </summary>
    public Tower ShootingTower;
    /// <summary>
    /// Zmienna typu Tower przechowująca informacje dla wieży obszarowej.
    /// </summary>
    public Tower AoeTower;
	
    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
	/// Ustala odpowieni typ wyliczenowy i koszt dla każdej z wież.
    /// </summary>
    void Start()
    {
        WallTower = new Tower
        {
            Type = TowerType.Wall,
            Cost = 100
        };
        ShootingTower = new Tower
        {
            Type = TowerType.Shooting,
            Cost = 150
        };
        AoeTower = new Tower
        {
            Type = TowerType.Aoe,
            Cost = 250
        };
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za dodawanie odpowiedniego obiektu wieży, definiowanego przez argument type, w miejscu i jako dziecko obiektu parent. Zwraca koszt wieży.
    /// </summary>
    /// <param name="type">Argument typu TowerType określająca typ wieży, jak ma zostać utworzona.</param>
    /// <param name="parent">Argument typu GameObject, którego dzieckiem stanie się nowa wieża.</param>
    /// <returns>Wzraca koszt wybudowania wieży.</returns>
    public int SpawnTower(TowerType type, GameObject parent)
    {
        Tower tempTower = new Tower();
        GameObject tempGameObject = null;
        switch (type)
        {
                case TowerType.Wall:
                    tempTower = WallTower;
                    tempGameObject = Resources.Load("Prefabs/Towers/Wall") as GameObject;
                    break;
                case TowerType.Shooting:
                    tempTower = ShootingTower;
                    tempGameObject = Resources.Load("Prefabs/Towers/Shooting") as GameObject;
                    break;
                case TowerType.Aoe:
                    tempTower = AoeTower;
                    tempGameObject = Resources.Load("Prefabs/Towers/Aoe") as GameObject;
                    break;
        }
        GameObject tower = Instantiate(tempGameObject, parent.transform.position, Quaternion.identity) as GameObject;
        tower.transform.parent = parent.transform;
        tower.transform.localPosition = new Vector3(tower.transform.localPosition.x, -(2.0f/parent.transform.localScale.y), tower.transform.localPosition.z);
        
        return tempTower.Cost;
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za usuwanie obiektu wieży tower. Zwraca koszt wieży.
    /// </summary>
    /// <param name="tower">Argument typu GameObject, którego dzieckiem jest wieża, która ma zostać zniszczona.</param>
    /// <returns>Wzraca koszt wybudowania wieży</returns>
    public int DestroyTower(GameObject tower)
    {
        Tower tempTower = new Tower();
        if (tower.name.Contains("Wall"))
        {
            tempTower = WallTower;
        }
        else if (tower.name.Contains("Shooting"))
        {
            tempTower = ShootingTower;
        }
        else if (tower.name.Contains("Aoe"))
        {
            tempTower = AoeTower;
        }
        Destroy(tower.gameObject);

        return tempTower.Cost;
    }
}
