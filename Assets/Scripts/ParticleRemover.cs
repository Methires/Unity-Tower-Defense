using UnityEngine;
using System.Collections;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Odpowiada za usuwanie obiektu po upływie czasu, jaki potrzeby był emiterowi cząsteczek do ukończenia swojego działania.
/// </summary>
public class ParticleRemover : MonoBehaviour 
{
    /// <summary>
    /// Zmienna określająca czas po jakim czasie obiekt ma zostać usunięty.
    /// </summary>
    public float Duration;
	
    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
	/// Wywołuje metodę odpowiedzialną za usunięcie obiektu.
    /// </summary>
	void Start()
	{
	    StartCoroutine(DestroyOnParticleEnd());
	}
	
    /// <summary>
    /// Metoda odpowiedzialna za zniszczenie obiektu po upływie określonego czasu.
    /// </summary>
    /// <returns>IEnumerator pozwalający na wykorzystanie mechanizmu yield.</returns>
    private IEnumerator DestroyOnParticleEnd()
    {
        yield return new WaitForSeconds(Duration);
        Destroy(gameObject);
    }
}
