using UnityEngine;

public class EndpointBehaviour : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            //TO DO: heal player
        }
    }
}
