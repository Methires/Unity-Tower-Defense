using UnityEngine;

public class EndpointBehaviour : MonoBehaviour
{
    public int MaxHealth;
    public GameObject ParticleOnDamage;

    private int _currentHealth;

    void Start()
    {
        _currentHealth = MaxHealth;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateCoreLife(_currentHealth, MaxHealth);
    }

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

    private void DealDamage(int value)
    {
        Instantiate(ParticleOnDamage, transform.position, Quaternion.Euler(0.0f,0.0f,0.0f));
        _currentHealth -= value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateCoreLife(_currentHealth, MaxHealth);
        if (_currentHealth == 0)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GameOver();
        }
    }
}
