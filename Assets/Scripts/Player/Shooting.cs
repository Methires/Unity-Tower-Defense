using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Shooting : MonoBehaviour
{
    public Texture2D CrosshairTexture;
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
    private float _reloadTimeCounter;
    private Rect _crosshairPosition;

	void Start ()
	{
        _isReloading = false;
        _reloadTimeCounter = 0.0f;
	    Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _crosshairPosition = new Rect((Screen.width - CrosshairTexture.width) / 2 , 
                             (Screen.height - CrosshairTexture.height) / 2,
                             CrosshairTexture.width, 
                             CrosshairTexture.height);
	    _currentClip = ClipSize;
	    _currentAmmo = MaxAmmo - ClipSize;
       // GetComponent<GUIText>().text = "Clip: " + _currentClip + " | Ammo: " + _currentAmmo;
	}

    void OnGUI()
    {
        GUI.DrawTexture(_crosshairPosition, CrosshairTexture);
    }

    void Update () 
    {
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
	    }

	    if (Input.GetButtonDown("Fire1"))
	    {
	        Shoot();
	    }

        if (Input.GetKeyDown(KeyCode.R) && _currentClip != ClipSize)
        {
            Reload();
        }

        if (_isReloading)
        {
            _reloadTimeCounter += Time.deltaTime * 1.1f;
            if (_reloadTimeCounter > ReloadTime)
            {
                _isReloading = false;
                _reloadTimeCounter = 0.0f;
                //GetComponent<GUIText>().text = "Clip: " + _currentClip + " | Ammo: " + _currentAmmo;
            }
        }
    }

    private void Shoot()
    {
        if (_currentClip > 0 && !_isReloading)
        {
            GetComponent<AudioSource>().clip = ShotSound;
            _currentClip--;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, forward, out hit))
            {
                if (hit.transform.gameObject.tag == "Enemy" && hit.distance < Range)
                {
                    hit.transform.gameObject.GetComponent<EnemyBehaviour>().ReceiveDamage(AttackValue);
                    Debug.Log("You hit enemy");
                }
                else if (hit.distance > Range && hit.transform.gameObject == null)
                {
                    Debug.Log("You hit nothing");
                }
            }
            GetComponent<AudioSource>().Play();
            //GetComponent<GUIText>().text = "Clip: " + _currentClip + " | Ammo: " + _currentAmmo;
        }
        else if (_currentClip == 0)
        {
            Reload();
        }
    }

    private void Reload()
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
            GetComponent<AudioSource>().Play();
        }
        else
        {
            GetComponent<AudioSource>().clip = NoAmmoSound;
            GetComponent<AudioSource>().Play();
        }
    }

    public void RenewAmmo()
    {
        _currentAmmo = MaxAmmo;
        //GetComponent<GUIText>().text = "Clip: " + _currentClip + " | Ammo: " + _currentAmmo;
    }
}
