﻿using UnityEngine;

public class NanobotBehaviour : MonoBehaviour
{
    private int _attackValue;
    private GameObject _target;

	void Start() 
    {	
	}
	
	void Update() 
    {
	    if (_target != null)
	    {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_target.transform.position - transform.position), 720.0f * Time.deltaTime);
	        transform.position = Vector3.Lerp(transform.position, _target.transform.position, 1.5f * Time.deltaTime);
	    }
	    else
	    {
	        Destroy(gameObject);
	    }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyBehaviour>().ReceiveDamage(_attackValue);
        }
    }

    public void AcquireTarget(GameObject target, int attackValue)
    {
        _target = target;
        _attackValue = attackValue;
    }
}