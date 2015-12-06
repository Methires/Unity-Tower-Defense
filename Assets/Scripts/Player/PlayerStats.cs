using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("GUI Bar Textures")]
    public Texture2D HpTexture2D;
    public Texture2D StaminaTexture2D;
    [Header("Statistics")]
    public float MaxHealth;
    public float MaxStamina;
    public float DeathTimePenalty;
    
    private float _currentHealth = 100;
    private float _currentStamina = 100;
    private float _barWidth;
    private float _barHeight;
    private float _staminaRegenarationDelay;
    private float _staminaDelayValue = 2.0f;
    private float _deathTimeCounter;
    private bool _canRegenerateStamina;
    private bool _isDead;
    private Vector3 _lastPosition;
    private GameObject _gun;
    private CharacterController _characterController;
    private FirstPersonController _firstPersonController;
    private Shooting _shooting;

    void Start()
    {
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

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha7)) DecreaseHealth(75);
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

    public void IncreaseHealth(float value)
    {
		    _currentHealth += value;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth); 
    }

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

    void OnDisable()
    {
        EndFpsPhase();
    }

    public Shooting Shooting
    {
        get { return _shooting; }
        private set { _shooting = value; }
    }
}
