using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private GameObject _target;

	void Update()
	{
	    transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, 10.0f * Time.deltaTime);
	}

    public void SetGoal(GameObject target)
    {
        _target = target;
    }
}
