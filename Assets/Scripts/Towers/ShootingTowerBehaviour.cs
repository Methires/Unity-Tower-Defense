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
}
