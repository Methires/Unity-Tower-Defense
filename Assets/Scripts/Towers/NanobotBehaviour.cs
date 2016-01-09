using UnityEngine;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Odpowiada za przemieszczanie się i atakowanie obiektów pojawiających się w ramach ataku wieży strzelającej.
/// </summary>
public class NanobotBehaviour : MonoBehaviour
{
    /// <summary>
    /// Wartość obrażeń jakie zostaną zadane przeciwnikowi podczas kontaku z nim.
    /// </summary>
    private int _attackValue;
    /// <summary>
    /// Referencja na obiekt, w kierunku którego przemieszczać będzię się obiekt tej klasy.
    /// </summary>
    private GameObject _target;

	/// <summary>
    /// Metoda wywoływana co klatkę, gdy skrypt jest aktywny.
    /// Przemieszcza w przestrzeni gry obiekt w kierunku obiektu ustawionego jako cel albo usuwa obiekt, jeżeli cel przestał istnieć.
	/// </summary>
	void Update() 
    {
	    if (_target != null)
	    {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_target.transform.position - transform.position), 720.0f * Time.deltaTime);
	        transform.position = Vector3.Lerp(transform.position, _target.transform.position, 2.5f * Time.deltaTime);
	    }
	    else
	    {
	        Destroy(gameObject);
	    }
	}

    /// <summary>
    /// Metoda wywoływana podczas wykrycia kolizji z innym obiektem.
    /// </summary>
    /// <param name="other">Obiekt z którym wykryto kolizję.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyBehaviour>().ReceiveDamage(_attackValue);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metoda określająca cel, do którego ma podążać obiekt i wartość ataku, jaki zostanie zadany podczas kontaktu z jakimkolwiek przeciwnikiem.
    /// </summary>
    /// <param name="target">Obiekt, w kierunku którego przemieszczać będzię się ten obiekt.</param>
    /// <param name="attackValue">Wartość obrażeń jakie zostaną zadane przeciwnikowi podczas kontaku z nim.</param>
    public void AcquireTarget(GameObject target, int attackValue)
    {
        _target = target;
        _attackValue = attackValue;
    }
}
