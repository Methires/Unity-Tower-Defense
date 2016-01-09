using System;
using UnityEngine;

/// <summary>
/// Klasa odpowiedzialna za zamianę ruchu myszki na obrót gracza w trakcie fazy obrony. Pochodzi z elementów wchodzących w skład silnika Unity, jednakże musiała zostać odpowiednio modyfikowana.
/// </summary>
[Serializable]
public class MouseLook
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;

    /// <summary>
    /// Zmienna logiczna określają czy gracz może obracać obiekt.
    /// </summary>
    private bool m_BlockLook;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
    }
    /// <summary>
    /// Metoda zamieniająca ruch myszki na obrót obiektu gracza, aby możliwe było rozglądanie się dookoła. Zmodyfikowana metoda z silnika Unity.
    /// </summary>
    /// <param name="character">Pozycja obiektu gracza w przestrzeni gry.</param>
    /// <param name="camera">Pozycja w przestrzeni gry obiektu kamery, która jest wykorzystywana w tej fazie.</param>
    public void LookRotation(Transform character, Transform camera)
    {
        //Modyfikacja w tym miejscu.
        float yRot = Input.GetAxis("Mouse X") * XSensitivity * Convert.ToSingle(!m_BlockLook);
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity * Convert.ToSingle(!m_BlockLook);
        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
        if (clampVerticalRotation)
        {
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
        }
        if (smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
        return q;
    }

    /// <summary>
    /// Metoda typu get i set dla zmiennej określającej czy gracz może rozglądać się wokół podczas fazy obrony.
    /// </summary>
    public bool BlockLook
    {
        set { m_BlockLook = value; }
    }
}