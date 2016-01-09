using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Przechowuje informacje o punktach życiowych, wartości ataku i szybkości przeciwnika. Odpowiada za przemieszczanie i modyfikacje informacji o obiekcie.
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    /// <summary>
    /// Prędkość z jaką obiekt się przemieszcza w przestrzeni gry.
    /// </summary>
    public float Speed;
    /// <summary>
    /// Liczna punktów życia obiektu.
    /// </summary>
    public int HealthPoints;
    /// <summary>
    /// Wartość z jaką obiekt atakuje.
    /// </summary>
    public int AttackValue;
    /// <summary>
    /// Referencja na obiekt, który pojawi się w momencie usunięcia obiektu.
    /// </summary>
    public GameObject ParticleOnDeath;
    /// <summary>
    /// Podstawowy materiał widoczny na obiekcie.
    /// </summary>
    [Header("Materials")]
    public Material BasicMaterial;
    /// <summary>
    /// Materiał widoczny na obiekcie przez pewnien czas od momentu zaatakowania tego obiektu.
    /// </summary>
    public Material DamagedMaterial;
	
    /// <summary>
    /// Zmienna logiczna określąca czy przeciwnik został niedawno zaatakowany.
    /// </summary>
    private bool _wasDamaged;
    /// <summary>
    /// Zmienna odliczająca czas od ostatnich otrzymanych obrażeń.
    /// </summary>
    private float _timeAfterDamaged;
    /// <summary>
    /// Lista referencji na elementy siatki dwuwymiarowej z której składa się trasa.
    /// </summary>
    private List<GameObject> _path;
    /// <summary>
    /// Współrzędne w przestrzeni gry na następny element siatki dwuwymiarowej.
    /// </summary>
    private Vector3 _nextTile;
    /// <summary>
    /// Indeks elementu z listy do którego obecnie przemieszcza się obiekt.
    /// </summary>
    private int _pathPart = 0;
	
    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
    /// </summary>
	void Start ()
	{
	    GetComponent<Renderer>().material = BasicMaterial;
	    _wasDamaged = false;
	    _path = FindObjectOfType<CreatePath>().GetPath();
	    _nextTile = _path[_pathPart].transform.position;
	    _pathPart++;
	}
	
	/// <summary>
    /// Metoda wywoływana co klatkę, gdy skrypt jest aktywny. Przemieszcza w przestrzeni gry obiekt w kierunku punktu końcowego podążając wyznaczoną trasą.
	/// </summary>
	void Update () 
    {
	    if (Vector3.Distance(transform.position, _nextTile) > 0.25f)
	    {
	        transform.position = Vector3.Lerp(transform.position, _nextTile, Speed * Time.deltaTime);
	    }
	    else
	    {
	        if (_pathPart < _path.Count)
	        {
                _nextTile = _path[_pathPart].transform.position;
                _pathPart++;
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
	
    /// <summary>
    /// Metoda zwracająca wartość ataku przypisaną do obiektu będącego przeciwnikiem.
    /// </summary>
    /// <returns></returns>
    public int DealDamage()
    {
        return AttackValue;
    }
	
    /// <summary>
    /// Metoda zmniejszająca punkty życia przeciwnika o wartość value i wywołanie metody usuwającej obiekt, gdy punkty życia osiągną wartość mniejszą lub równą 0.
    /// </summary>
    /// <param name="value">Wartość liczbowa o którą ma zostać zmiejszona liczba punktów życia obiektu.</param>
    public void ReceiveDamage(int value)
    {
        HealthPoints -= value;
        GetComponent<Renderer>().material = DamagedMaterial;
        _timeAfterDamaged = 0.0f;
        _wasDamaged = true;
        if (HealthPoints <= 0)
        {
            var particlePosition = new Vector3(transform.position.x, 0.0f, transform.position.z);
            Instantiate(ParticleOnDeath, particlePosition, Quaternion.identity);
            DestroyEnemy();
        }
    }
	
    /// <summary>
    /// Metoda odpowiedzialna za usunięcie obiektu przeciwnika z gry i zwiększenie zasobów gracza o odpowiednią wartość
    /// </summary>
    public void DestroyEnemy()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateResources(AttackValue * 10);
        Destroy(gameObject);
    }

    /// <summary>
    /// Metoda wywoływana podczas wykrycia kolizji z innym obiektem.
    /// </summary>
    /// <param name="other">Obiekt z którym wykryto kolizję.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerStats>().DecreaseHealth(AttackValue);
        }
    }
}
