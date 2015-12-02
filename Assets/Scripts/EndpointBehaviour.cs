using UnityEngine;

public class EndpointBehaviour : MonoBehaviour
{
    public int MaxHealth;
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
            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            //TO DO: heal player
        }
    }

    private void DealDamage(int value)
    {
        _currentHealth -= value;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
        }
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateCoreLife(_currentHealth, MaxHealth);
        if (_currentHealth == 0)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GameOver();
        }
    }
}
