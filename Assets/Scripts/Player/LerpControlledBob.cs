using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Klasa pomocnicza dla klasy CurvedControllerBob. Opowiadająca za wyznaczanie offsetu potrzebnego do uzyskania bardziej realnej animacji, której celem jest imitacja ruch głowy.
/// Pochodzi z elementów wchodzących w skład silnika Unity.
/// </summary>
[Serializable]
public class LerpControlledBob
{
    public float BobDuration;
    public float BobAmount;

    private float m_Offset = 0f;

    public float Offset()
    {
        return m_Offset;
    }

    public IEnumerator DoBobCycle()
    {
        float t = 0f;
        while (t < BobDuration)
        {
            m_Offset = Mathf.Lerp(0f, BobAmount, t / BobDuration);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        t = 0f;
        while (t < BobDuration)
        {
            m_Offset = Mathf.Lerp(BobAmount, 0f, t / BobDuration);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        m_Offset = 0f;
    }
}