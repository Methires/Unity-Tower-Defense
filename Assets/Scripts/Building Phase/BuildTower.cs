using UnityEngine;

public class BuildTower : MonoBehaviour
{
    public enum TowerType
    {
        Wall,
        Shooting,
        Aoe
    }
    public struct Tower
    {
        public TowerType Type;
        public int Cost;
    }
    public Tower WallTower;
    public Tower ShootingTower;
    public Tower AoeTower;

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
