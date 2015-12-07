using UnityEngine;
using System.Collections;

public class ParticleRemover : MonoBehaviour 
{
    public float Duration;

	void Start()
	{
	    StartCoroutine(DestroyOnParticleEnd());
	}

    private IEnumerator DestroyOnParticleEnd()
    {
        yield return new WaitForSeconds(Duration);
        Destroy(gameObject);
    }
}
