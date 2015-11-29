using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AoeTowerBehaviour : MonoBehaviour
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
		    if (_attackTimeCouter > AttackInterval)
            {
                ValidateList();
                foreach (var enemy in _currentEnemiesInProximity)
                {
                    enemy.GetComponent<EnemyBehaviour>().ReceiveDamage(AttackValue);
                }
                _attackTimeCouter = 0.0f;
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
        if (value == false)
        {
            _currentEnemiesInProximity.Clear();
        }
    }

    private void ValidateList()
    {
        _currentEnemiesInProximity.RemoveAll(enemy => enemy == null);
    }
}
