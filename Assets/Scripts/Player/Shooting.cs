using UnityEngine;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Odpowiedzialna za przechowywanie informacji o amunicji i wykonywanie funkcjonalności związanych z atakowaniem przeciwników przez gracza.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Shooting : MonoBehaviour
{
    /// <summary>
    /// Tekstura która wykorzystwana jest jako celownik do broni.
    /// </summary>
    public Texture2D CrosshairTexture;
    /// <summary>
    /// Referencja na obiekt, który znajduję się na końcu broni.
    /// </summary>
    public GameObject GunTip;
    /// <summary>
    /// Plik dźwiękowy z dźwiękiem strzału z broni.
    /// </summary>
    [Header("Sounds")]
    public AudioClip ShotSound;
    /// <summary>
    /// Plik dźwiękowy z dźwiękiem przeładowania broni.
    /// </summary>
    public AudioClip ReloadSound;
    /// <summary>
    /// Plik dźwiękowy z dźwiękiem braku amunicji w broni.
    /// </summary>
    public AudioClip NoAmmoSound;
    /// <summary>
    /// Wartość obrażeń zadawanych podczas strzału z broni.
    /// </summary>
    [Header("Weapon Params")]
    public int AttackValue;
    /// <summary>
    /// Maksymalna liczba amunicji.
    /// </summary>
    public int MaxAmmo;
    /// <summary>
    /// Liczba strzałów w magazynku.
    /// </summary>
    public int ClipSize;
    /// <summary>
    /// Zasięg broni.
    /// </summary>
    public float Range;
    /// <summary>
    /// Czas potrzebny na przeładowanie broni.
    /// </summary>
    public float ReloadTime;

    /// <summary>
    /// Obecna liczba całkowitej amunicji.
    /// </summary>
    private int _currentAmmo;
    /// <summary>
    /// Obecna liczba amunicji w magazynku.
    /// </summary>
    private int _currentClip;
    /// <summary>
    /// Zmienna logiczna określająca czy trwa przeładowanie broni.
    /// </summary>
    private bool _isReloading;
    /// <summary>
    /// Zmienna logiczna określająca czy trwa strzał z broni.
    /// </summary>
    private bool _isShooting;
    /// <summary>
    /// Zmienna odliczająca czas od rozpoczęcia przeładowania.
    /// </summary>
    private float _reloadTimeCounter;
    /// <summary>
    /// Zmienna odliczająca czas od rozpoczęcia strzału.
    /// </summary>
    private float _shootingTimeCounter;
    /// <summary>
    /// Zmienna odliczająca czas od rozpoczęcia pokazywania lasera.
    /// </summary>
    private float _laserTimeCounter;
    /// <summary>
    /// Zmienna odliczająca czas od rozpoczęcia pokazywania flary.
    /// </summary>
    private float _flareTimeCounter;
    /// <summary>
    /// Zmienna określająca rozmiar i pozycje celownika.
    /// </summary>
    private Rect _crosshairPosition;
    /// <summary>
    /// Referencja na komponent LineRenderer.
    /// </summary>
    private LineRenderer _line;
    /// <summary>
    /// Referencja na komponent LineRenderer.
    /// </summary>
    private Light _light;

    /// <summary>
    /// Metoda wywoływana tylko raz przy pierwszej klatce w której skrypt jest aktywny.
    /// </summary>
    void Start()
    {
        _isReloading = false;
        _reloadTimeCounter = 0.0f;
        _crosshairPosition = new Rect((Screen.width - CrosshairTexture.width) / 2,
                             (Screen.height - CrosshairTexture.height) / 2,
                             CrosshairTexture.width,
                             CrosshairTexture.height);
        _currentClip = ClipSize;
        _currentAmmo = MaxAmmo - ClipSize;
        _line = GunTip.GetComponent<LineRenderer>();
        _line.enabled = false;
        _light = GunTip.GetComponent<Light>();
        _light.intensity = 0.0f;
        _light.enabled = false;
        UpdateAmmoText();
    }
    
    /// <summary>
    /// Metoda dopowiedzialna na rysowanie interfejsu gracza. Metoda wykorzystywana przed zaminami wprowadzonymi w Unity 4.6.
    /// </summary>
    void OnGUI()
    {
        GUI.DrawTexture(_crosshairPosition, CrosshairTexture);
    }
   
    /// <summary>
    /// Metoda wywoływana co klatkę, gdy skrypt jest aktywny. Sprawdza czy klawisz naciśniety przez gracza powinien wywołać jakąś metodę i w takim przypadku wywołuje ją.
    /// Odlicza czas dla odpowiednich wartości.
    /// </summary>
    void Update()
    {
        if (Input.GetButtonDown("Fire"))
        {
            Shoot();
        }

        if (Input.GetButtonDown("Reload") && _currentClip != ClipSize)
        {
            Reload();
        }

        if (_isReloading)
        {
            _reloadTimeCounter += Time.deltaTime;
            if (_reloadTimeCounter > ReloadTime)
            {
                _isReloading = false;
                _reloadTimeCounter = 0.0f;
                UpdateAmmoText();
            }
        }

        if (_isShooting)
        {
            _shootingTimeCounter += Time.deltaTime;
            _laserTimeCounter += Time.deltaTime;
            _flareTimeCounter += Time.deltaTime;

            if (_laserTimeCounter > 0.1f)
            {
                _line.enabled = false;
            }

            if (_flareTimeCounter < 0.125f)
            {
                _light.intensity += 10.0f * Time.deltaTime;
            }
            else if(_flareTimeCounter > 0.125f && _flareTimeCounter < 0.25f)
            {
                _light.intensity -= 10.0f * Time.deltaTime;
            }

            if (_shootingTimeCounter > 1.0f)
            {
                _isShooting = false;
                _shootingTimeCounter = 0.0f;
                _laserTimeCounter = 0.0f;
                _flareTimeCounter = 0.0f;
                _light.intensity = 0.0f;
                _light.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Metoda wywoływana w momencie oddania strzału z broni.
    /// </summary>
    private void Shoot()
    {
        if (_currentClip > 0 && !_isReloading && !_isShooting)
        {
            GetComponent<AudioSource>().clip = ShotSound;
            _isShooting = true;
            _line.enabled = true;
            _light.enabled = true;
            _light.intensity = 0.0f;
            _currentClip--;
            UpdateAmmoText();
            Ray ray = new Ray(GunTip.transform.position, GunTip.transform.forward);
            RaycastHit hit;
            _line.SetPosition(0, ray.origin);
            if (Physics.Raycast(ray, out hit))
            {
                _line.SetPosition(1, hit.point);
                if (hit.transform.gameObject.tag == "Enemy" && hit.distance < Range)
                {
                    hit.transform.gameObject.GetComponent<EnemyBehaviour>().ReceiveDamage(AttackValue);
                }
            }
            else
            {
                _line.SetPosition(1, ray.GetPoint(Range));
            }
            GetComponent<AudioSource>().Play();
        }
        else if (_currentClip == 0 && !_isShooting)
        {
            Reload();
        }
    }
    
    /// <summary>
    /// Metoda wywoływana w momencie przeładowania broni.
    /// </summary>
    private void Reload()
    {
        if (!_isShooting)
        {
            GetComponent<AudioSource>().clip = ReloadSound;
            if (_currentAmmo > 0 && !_isReloading)
            {
                _isReloading = true;
                if (_currentAmmo >= ClipSize)
                {
                    _currentAmmo -= ClipSize - _currentClip;
                    _currentClip = ClipSize;
                }
                else
                {
                    _currentClip = _currentAmmo;
                    _currentAmmo = 0;
                }
            }
            else
            {
                GetComponent<AudioSource>().clip = NoAmmoSound;
            }
            GetComponent<AudioSource>().Play();
        }
    }
   
    /// <summary>
    /// Metoda odpowiedzialna za zwiększenie obecnej liczby amunicji do wartości maksymalnej.
    /// </summary>
    public void RenewAmmo()
    {
        _currentAmmo = MaxAmmo;
        UpdateAmmoText();
    }
    
    /// <summary>
    /// Metoda wywołująca metodę odpowiedzialną za zmianę informacji o liczbie amunicji na ekranie gracza.
    /// </summary>
    private void UpdateAmmoText()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateAmmoOnShooterUi(_currentClip, _currentAmmo);
    }
    
    /// <summary>
    /// Metoda przywracająca odpowiednie artybuty do ich początkowej wartości pod koniec fazy obrony.
    /// </summary>
    private void EndFpsPhase()
    {
        _line.enabled = false;
        _light.enabled = false;
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = null;
        _isShooting = false;
        _isReloading = false;
        _reloadTimeCounter = 0.0f;
        _shootingTimeCounter = 0.0f;
        _laserTimeCounter = 0.0f;
        _flareTimeCounter = 0.0f;
        UpdateAmmoText();
    }
   
    /// <summary>
    /// Metoda wywoływana, gdy obiekt staje się nieaktywny.
    /// </summary>
    void OnDisable()
    {
        EndFpsPhase();
    }
}
