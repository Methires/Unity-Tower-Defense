using UnityEngine;
using System.Collections.Generic;

public class ShootingTowerBehaviour : MonoBehaviour
{
    public int AttackValue;
    public float AttackInterval;
    private float _attackTimeCouter;
    private bool _attackPhase;
    private List<GameObject> _currentEnemiesInProximity;

    void Start()
    {
        _attackTimeCouter = 0.0f;
        _attackPhase = false;
        _currentEnemiesInProximity = new List<GameObject>();
	}
	
	void Update()
    {
	    if (_attackPhase)
        {
            ValidateList();
            if (_currentEnemiesInProximity.Count != 0 && _currentEnemiesInProximity[0] != null)
            {
                Vector3 direction = _currentEnemiesInProximity[0].transform.position - transform.position;
                Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    720.0f*Time.deltaTime);
                transform.rotation = rotation;

                if (_attackTimeCouter > AttackInterval)
                {
                    _currentEnemiesInProximity[0].GetComponent<EnemyBehaviour>().ReceiveDamage(AttackValue);
                    //GameObject bullet = Instantiate(Resources.Load("Prefabs/Towers/Bullet")) as GameObject;
                    //bullet.GetComponent<BulletBehaviour>().SetGoal(_currentEnemiesInProximity[0]);
                   _attackTimeCouter = 0.0f;
                }
            }
            _attackTimeCouter += 1.5f*Time.deltaTime;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            _currentEnemiesInProximity.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            _currentEnemiesInProximity.Remove(other.gameObject);
        }
    }

    public void SetAttackPhase(bool value)
    {
        _attackPhase = value;
    }

    private void ValidateList()
    {
        _currentEnemiesInProximity.RemoveAll(enemy => enemy == null);
    }
}
