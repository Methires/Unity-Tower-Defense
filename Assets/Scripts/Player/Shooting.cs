using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Shooting : MonoBehaviour
{
    public Texture2D CrosshairTexture;
    public GameObject GunTip;
    [Header("Sounds")]
    public AudioClip ShotSound;
    public AudioClip ReloadSound;
    public AudioClip NoAmmoSound;
    [Header("Weapon Params")]
    public int AttackValue;
    public int MaxAmmo;
    public int ClipSize;
    public float Range;
    public float ReloadTime;

    private int _currentAmmo;
    private int _currentClip;
    private bool _isReloading;
    private bool _isShooting;
    private float _reloadTimeCounter;
    private float _shootingTimeCounter;
    private float _laserTimeCounter;
    private float _flareTimeCounter;
    private Rect _crosshairPosition;
    private LineRenderer _line;
    private Light _light;


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

        // GetComponent<GUIText>().text = "Clip: " + _currentClip + " | Ammo: " + _currentAmmo;
    }

    void OnGUI()
    {
        GUI.DrawTexture(_crosshairPosition, CrosshairTexture);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
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
                //GetComponent<GUIText>().text = "Clip: " + _currentClip + " | Ammo: " + _currentAmmo;
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
            //GetComponent<GUIText>().text = "Clip: " + _currentClip + " | Ammo: " + _currentAmmo;
        }
        else if (_currentClip == 0 && !_isShooting)
        {
            Reload();
        }
    }

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

    public void RenewAmmo()
    {
        _currentAmmo = MaxAmmo;
        //GetComponent<GUIText>().text = "Clip: " + _currentClip + " | Ammo: " + _currentAmmo;
    }

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
    }

    void OnDisable()
    {
        EndFpsPhase();
    }
}
