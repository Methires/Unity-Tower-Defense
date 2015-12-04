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
    
    private float _currentHealth = 100;
    private float _currentStamina = 100;
    private float _barWidth;
    private float _barHeight;
    private bool _canRegenerateStamina;
    private float _staminaRegenarationDelay;
    private float _DelayValue = 2.0f;
    private Vector3 _lastPosition;
    private CharacterController _characterController;
    private FirstPersonController _firstPersonController;

    void Awake()
    {
        _barHeight = Screen.height * 0.04f;
        _barWidth = _barHeight * 10.0f;
        _characterController = FindObjectOfType<CharacterController>().GetComponent<CharacterController>();
        _firstPersonController = FindObjectOfType<FirstPersonController>().GetComponent<FirstPersonController>();
        _lastPosition = transform.position;
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

    void FixedUpdate()
    {
        if (_characterController.isGrounded && Input.GetKey(KeyCode.LeftShift) && _lastPosition != transform.position && _currentStamina > 0)
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
            _staminaRegenarationDelay += 0.5f * Time.deltaTime;
            if (_staminaRegenarationDelay >= _DelayValue)
            {
                _staminaRegenarationDelay = _DelayValue;
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

    void DecreaseHealth(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
        if (_currentHealth < 0)
        {
            //Game Over
        }
    }

    void IncreaseHealth(float value)
    {
        _currentHealth += value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
    }

}
