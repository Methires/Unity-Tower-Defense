using System;
using UnityEngine;

/// <summary>
/// Klasa dziedzicząca po klasie MonoBehaviour. Przechowuje wszystkie informacje o postaci gracza, która wykorzystywana jest podczas fazy obrony i odpowiada za modyfikację tych informacji.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    /// <summary>
    /// Tekstura wykorzystywana do tworzenia paska życia gracza.
    /// </summary>
    [Header("GUI Bar Textures")]
    public Texture2D HpTexture2D;
    /// <summary>
    /// Tekstura wykorzystywana do tworzenia paska wytrzymałości gracza.
    /// </summary>
    public Texture2D StaminaTexture2D;
    /// <summary>
    /// Maksymalna liczba punktów życia.
    /// </summary>
    [Header("Statistics")]
    public float MaxHealth;
    /// <summary>
    /// Maksymalna liczna punktów wytrzymałości.
    /// </summary>
    public float MaxStamina;
    /// <summary>
    /// Czas odliczany po rozpoczęciu kary w przypadku śmierci.
    /// </summary>
    public float DeathTimePenalty;
    
    /// <summary>
    /// Obecna liczba punktów życia.
    /// </summary>
    private float _currentHealth;
    /// <summary>
    /// Obecna liczna punktów wytrzymałości.
    /// </summary>
    private float _currentStamina;
    /// <summary>
    /// Szerokość pasków życia i wytrzymałości.
    /// </summary>
    private float _barWidth;
    /// <summary>
    /// Wysokość pasków życia i wytrzymałości.
    /// </summary>
    private float _barHeight;
    /// <summary>
    /// Zmienna odliczająca czas, który upłynął od ostatniego zmniejszenia punktów wytrzymałości.
    /// </summary>
    private float _staminaRegenarationDelay;
    /// <summary>
    /// Czas po upływie którego punkty wytrzymałości zaczną się odnawiać.
    /// </summary>
    private float _staminaDelayValue = 2.0f;
    /// <summary>
    /// Zmienna odliczająca czas, który upłynął od rozpoczęcią kary za śmierć.
    /// </summary>
    private float _deathTimeCounter;
    /// <summary>
    /// Zmienna logiczna określająca czy możliwe jest zwiększanie punktów wytrzymałości.
    /// </summary>
    private bool _canRegenerateStamina;
    /// <summary>
    /// Zmienna logiczna określajaca czy trwa kara za śmierć.
    /// </summary>
    private bool _isDead;
    /// <summary>
    /// Współrzędne z ostatnią pozycją gracza w przestrzeni gry.
    /// </summary>
    private Vector3 _lastPosition;
    /// <summary>
    /// Referencja na obiekt z modelem broni.
    /// </summary>
    private GameObject _gun;
    /// <summary>
    /// Referencja na komponent CharacterController.
    /// </summary>
    private CharacterController _characterController;
    /// <summary>
    /// Referencja na skrypt FirstPersonController.
    /// </summary>
    private FirstPersonController _firstPersonController;
    /// <summary>
    /// Referencja na skrypt Shooting.
    /// </summary>
    private Shooting _shooting;
    
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        _currentHealth = MaxHealth;
        _currentStamina = MaxStamina;
        _barHeight = Screen.height * 0.04f;
        _barWidth = _barHeight * 10.0f;
        _characterController = FindObjectOfType<CharacterController>().GetComponent<CharacterController>();
        _firstPersonController = FindObjectOfType<FirstPersonController>().GetComponent<FirstPersonController>();
        _lastPosition = transform.position;
        Shooting = GetComponentInChildren<Shooting>();
        _gun = Shooting.gameObject;
        _isDead = false;
        _deathTimeCounter = 0.0f;
    }
    
    /// <summary>
    /// Metoda dopowiedzialna na rysowanie interfejsu gracza. Metoda wykorzystywana przed zaminami wprowadzonymi w Unity 4.6.
    /// </summary>
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width - _barWidth - 10,
                                 Screen.height - _barHeight - 10,
                                 _currentHealth * _barWidth / MaxHealth,
                                 _barHeight), 
                                 HpTexture2D);
        GUI.DrawTexture(new Rect(Screen.width - _barWidth - 10,
                                 Screen.height - _barHeight * 2 - 20,
                                 _currentStamina * _barWidth / MaxStamina,
                                 _barHeight),
                                 StaminaTexture2D);
    }
    
    /// <summary>
    /// Metoda wywoływana co klatkę, gdy skrypt jest aktywny. Odpowiada za blokowanie możliwości sprintu i odliczanie czasu między karami.
    /// </summary>
    void Update()
    {
        if (!_isDead)
        {
            if (_characterController.isGrounded && Input.GetButton("Sprint") && _lastPosition != transform.position &&
                _currentStamina > 0)
            {
                _lastPosition = transform.position;
                _currentStamina -= 0.75f;
                _currentStamina = Mathf.Clamp(_currentStamina, 0, MaxStamina);
                _canRegenerateStamina = false;
            }
            else
            {
                _canRegenerateStamina = true;
            }


            if (_canRegenerateStamina)
            {
                _staminaRegenarationDelay += 0.5f*Time.deltaTime;
                if (_staminaRegenarationDelay >= _staminaDelayValue)
                {
                    _staminaRegenarationDelay = _staminaDelayValue;
                    if (Math.Abs(_currentStamina - MaxStamina) > 0.1f)
                    {
                        _currentStamina += 1.0f;
                        _currentStamina = Mathf.Clamp(_currentStamina, 0, MaxStamina);
                    }
                    else
                    {
                        _canRegenerateStamina = false;
                        _staminaRegenarationDelay = 0.0f;
                    }
                }
            }

            _firstPersonController.CanRun = _currentStamina > 0;
        }
        else
        {
            _deathTimeCounter += Time.deltaTime;
            if (_deathTimeCounter >= DeathTimePenalty)
            {
                DeathPenalty(true);
                _deathTimeCounter = 0.0f;
            }
        }
        
    }
    
    /// <summary>
    /// Metoda zmniejszająca obecne punkty życia o wartość damage i rozpoczynająca karę czasową, gdy ilość tych punktów jest mniejsza, bądź równa 0.
    /// </summary>
    /// <param name="damage">Wartość o jaką mają zostać zmniejszone punkty życia gracza.</param>
    public void DecreaseHealth(float damage)
    {
        if (!_isDead)
        {
            _currentHealth -= damage;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
            if (_currentHealth <= 0)
            {
                DeathPenalty(false);
            } 
        }
    }
    
    /// <summary>
    /// Metoda zwiększająca obecne punkty życia o wartość value, z zabezpieczeniem zapewniającym nie przekroczenie wartości początkowej.
    /// </summary>
    /// <param name="value">Wartość o jaką mają zostać zwiększone punkty życia gracza.</param>
    public void IncreaseHealth(float value)
    {
		    _currentHealth += value;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth); 
    }
   
    /// <summary>
    /// Metoda rozpoczynająca albo kończąca karę związaną ze śmiercią gracza.
    /// </summary>
    /// <param name="undo">Wartość undo = true, gdy kara się kończy, a wartość undo = false, gdy kara się rozpoczyna.</param>
    private void DeathPenalty(bool undo)
    {
        _isDead = !undo;
        _characterController.enabled = undo;
        _firstPersonController.enabled = undo;
        GetComponent<CapsuleCollider>().enabled = undo;
        Shooting.enabled = undo;
        _gun.SetActive(undo);
        _currentStamina = 0.0f;
        _canRegenerateStamina = undo;
        if (undo)
        {
            IncreaseHealth(MaxHealth);
            _currentStamina = MaxStamina;
        }
    }
   
    /// <summary>
    /// Metoda przywracająca odpowiednie artybuty do ich początkowej wartości pod koniec fazy obrony.
    /// </summary>
    private void EndFpsPhase()
    {
        if (_isDead)
        {
            DeathPenalty(true);
        }
        else
        {
            _currentStamina = MaxStamina;
            _currentHealth = MaxHealth;
        }
        _deathTimeCounter = 0.0f;
    }
    
    /// <summary>
    /// Metoda wywoływana, gdy obiekt staje się nieaktywny.
    /// </summary>
    void OnDisable()
    {
        EndFpsPhase();
    }
    
    /// <summary>
    /// Metoda typu get dla zwracająca referencje na obiekt klasy Shooting.
    /// </summary>
    public Shooting Shooting
    {
        get { return _shooting; }
        private set { _shooting = value; }
    }
}
