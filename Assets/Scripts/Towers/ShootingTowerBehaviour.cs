using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Odpowiada za atakowanie przeciwników przez wieże strzelające.
/// </summary>
public class ShootingTowerBehaviour : MonoBehaviour
{
    /// <summary>
    /// Wartość ataku wieży.
    /// </summary>
    public int AttackValue;
    /// <summary>
    /// Czas pomiędzy atakami wieży.
    /// </summary>
    public float AttackInterval;
    /// <summary>
    /// Referencja na obiekt, który zostanie dodany w przypadku ataku.
    /// </summary>
    public GameObject Bullet;

    /// <summary>
    /// Zmienna służąca do odliczania czasu pomiędzy kolejnymi atakami wieży.
    /// </summary>
    private float _attackTimeCouter;
    /// <summary>
    /// Zmienna logiczna określająca czy trwa faza obrony.
    /// </summary>
    private bool _attackPhase;
    /// <summary>
    /// Lista obiektów przeciwników będących w sąsiedztwie z obiektem.
    /// </summary>
    private List<GameObject> _currentEnemiesInProximity;
   
    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
    /// </summary>
    void Start()
    {
        _attackTimeCouter = 0.0f;
        _attackPhase = false;
        _currentEnemiesInProximity = new List<GameObject>();
	}
	
    /// <summary>
    /// Metoda wywoływana co klatkę, gdy skrypt jest aktywny.
    /// Odlicza czas do kolejnego ataku i obraca wieże w kierunku pierwszego obiektu na liście przeciwników.
	/// </summary>
	void Update()
    {
	    if (_attackPhase)
        {
            ValidateList();
            if (_currentEnemiesInProximity.Count != 0 && _currentEnemiesInProximity[0] != null)
            {
                Vector3 direction = new Vector3(_currentEnemiesInProximity[0].transform.position.x - transform.position.x, 0.0f, _currentEnemiesInProximity[0].transform.position.z - transform.position.z);
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            _currentEnemiesInProximity.Add(other.gameObject);
        }
    }

    /// <summary>
    /// Metoda wywoływana podczas wykrycia kolizji z innym obiektem.
    /// </summary>
    /// <param name="other">Obiekt z którym wykryto kolizję.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            _currentEnemiesInProximity.Remove(other.gameObject);
        }
    }
   
    /// <summary>
    /// Metoda określająca zdolność wieży do atakowania.
    /// </summary>
    /// <param name="value">Wartość argumentu value = true przywraca możliwość atakowania i naliczania czasu między atakami, 
    /// a wartości value = false blokuję możliwość ataku, naliczania czasu i zeruje obecne liczniki.</param>
    public void SetAttackPhase(bool value)
    {
        _attackPhase = value;
        if (!value)
        {
            _currentEnemiesInProximity.Clear();
            _attackTimeCouter = 0.0f;
        }
    }
   
    /// <summary>
    /// Metoda opowiedzialna za sprawdzenie i usuwanie referencji na obiekty z listy. które zostały już zniszczone.
    /// </summary>
    private void ValidateList()
    {
        _currentEnemiesInProximity.RemoveAll(enemy => enemy == null);
    }
}
