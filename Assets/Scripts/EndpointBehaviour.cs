using UnityEngine;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Przechowuje punkty życiowe obiektu będącego reprezentacją punktu końcowego i odpowiada za jego funkcjonalności.
/// </summary>
public class EndpointBehaviour : MonoBehaviour
{
    /// <summary>
    /// Maksymalna liczba punktów życia punktu końcowego.
    /// </summary>
    public int MaxHealth;
    /// <summary>
    /// Referencja na obiekt, który pojawi się w przypadku wykrycia kolizji z obiektem przeciwnika.
    /// </summary>
    public GameObject ParticleOnDamage;
   
    /// <summary>
    /// Obecna liczba punktów życia punktu końcowego.
    /// </summary>
    private int _currentHealth;
   
    /// <summary>
    /// Metoda wywoływana co klatkę, gdy skrypt jest aktywny.
    /// </summary>
    void Start()
    {
        _currentHealth = MaxHealth;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateCoreLife(_currentHealth, MaxHealth);
    }

    /// <summary>
    /// Metoda wywoływana podczas wykrycia kolizji z innym obiektem.
    /// </summary>
    /// <param name="other">Obiekt z którym wykryto kolizję.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            DealDamage(other.GetComponent<EnemyBehaviour>().DealDamage());
            other.GetComponent<EnemyBehaviour>().DestroyEnemy();
        }
        else if (other.tag == "Player")
        {
            other.GetComponent<PlayerStats>().IncreaseHealth(other.GetComponent<PlayerStats>().MaxHealth);
            other.GetComponent<PlayerStats>().Shooting.RenewAmmo();
        }
    }
   
    /// <summary>
    /// Metoda odpowiedzialna za odejmowanie punktów życia punktu końcowego.
    /// </summary>
    /// <param name="value">Wartość liczbowa jaka ma zostać odjęta od liczby punktów życia punktu końcowego.</param>
    private void DealDamage(int value)
    {
        var particlePosition = new Vector3(transform.position.x, 0.0f, transform.position.z);
        GameObject particle = Instantiate(ParticleOnDamage);
        particle.transform.position = particlePosition;
        _currentHealth -= value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateCoreLife(_currentHealth, MaxHealth);
        if (_currentHealth == 0)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GameOver();
        }
    }
}
