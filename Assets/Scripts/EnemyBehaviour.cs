using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float Speed;
    public int HealthPoints;
    public int AttackValue;
    [Header("Materials")] 
    public Material BasicMaterial;
    public Material DamagedMaterial;

    private bool _wasDamaged;
    private float _timeAfterDamaged;
    private List<GameObject> _path;
    private Vector3 _nextTile;
    private int i = 0;

	void Start ()
	{
	    GetComponent<Renderer>().material = BasicMaterial;
	    _wasDamaged = false;
	    _path = FindObjectOfType<CreatePath>().GetPath();
	    _nextTile = _path[i].transform.position;
	    i++;
	}
	
	void Update () 
    {
	    if (Vector3.Distance(transform.position, _nextTile) > 0.25f)
	    {
	        transform.position = Vector3.Lerp(transform.position, _nextTile, Speed * Time.deltaTime);
	    }
	    else
	    {
	        if (i < _path.Count)
	        {
                _nextTile = _path[i].transform.position;
                i++;
	        }
	    }

	    if (_wasDamaged)
	    {
	        _timeAfterDamaged += 1.5f*Time.deltaTime;
	        if (_timeAfterDamaged > 1.0f)
	        {
	            GetComponent<Renderer>().material = BasicMaterial;
	            _wasDamaged = false;
	        }
	    }
    }

    public int DealDamage()
    {
        return AttackValue;
    }

    public void ReceiveDamage(int value)
    {
        HealthPoints -= value;
        GetComponent<Renderer>().material = DamagedMaterial;
        _timeAfterDamaged = 0.0f;
        _wasDamaged = true;
        if (HealthPoints <= 0)
        {
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateResources(AttackValue * 10);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerStats>().DecreaseHealth(AttackValue);
        }
    }

    void OnDestroy()
    {
        //TO DO: maybe creating some kind of particle emitter
    }
}
