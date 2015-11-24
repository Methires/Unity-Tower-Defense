using System.Collections.Generic;
using UnityEngine;

public class MovingToEndpoint : MonoBehaviour
{
    public float Speed;
    private List<GameObject> _path;
    private Vector3 _nextTile;
    private int i = 0;

	void Start ()
	{
	    _path = FindObjectOfType<CreatePath>().GetPath();
	    _nextTile = _path[i].transform.position;
	    i++;
	}
	
	void Update () 
    {
	    if (Vector3.Distance(transform.position, _nextTile) > 0.5f)
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
    }
}
