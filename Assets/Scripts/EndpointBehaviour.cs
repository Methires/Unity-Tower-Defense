using UnityEngine;

public class EndpointBehaviour : MonoBehaviour
{
    public int Health;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            DealDamage(other.GetComponent<EnemyBehaviour>().DealDamage(false));
            other.GetComponent<EnemyBehaviour>().DeleteEnemy();
        }
        else if (other.tag == "Player")
        {
            //TO DO: heal player
        }
    }

    private void DealDamage(int value)
    {
        Health -= value;
        //TO DO: Play some kind of sound
        if (Health <= 0)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GameOver();
        }
    }
}
