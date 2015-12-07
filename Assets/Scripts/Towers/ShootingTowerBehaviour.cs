using UnityEngine;
using System.Collections.Generic;

public class ShootingTowerBehaviour : MonoBehaviour
{
    public int AttackValue;
    public float AttackInterval;
    public GameObject Bullet;

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
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    720.0f*Time.deltaTime);
                if (_attackTimeCouter > AttackInterval)
                {
                    var bulletPosition = new Vector3(transform.position.x, 11.0f, transform.position.z);
                    GameObject bullet = Instantiate(Bullet, bulletPosition, Quaternion.identity) as GameObject;
                    bullet.GetComponent<NanobotBehaviour>().AcquireTarget(_currentEnemiesInProximity[0], AttackValue);
                   _attackTimeCouter = 0.0f;
                }
            }
            _attackTimeCouter += Time.deltaTime;
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
        if (!value)
        {
            _currentEnemiesInProximity.Clear();
            _attackTimeCouter = 0.0f;
        }
    }

    private void ValidateList()
    {
        _currentEnemiesInProximity.RemoveAll(enemy => enemy == null);
    }
}
