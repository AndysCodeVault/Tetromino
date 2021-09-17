using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    private float m_speed;
    private bool m_isFading = false;
    private Renderer m_renderer;
    private float m_startTime;
    private Color m_startColor;
    private Color m_finalColor;

    private void Awake()
    {
        m_renderer = gameObject.GetComponent<Renderer>();
    }

    public void BeginFade(float delay, float speed)
    {       
        m_speed = speed;
        Invoke("StartFade", delay);
    }

    private void StartFade()
    {
        m_isFading = true;        
        m_startTime = Time.time;
        m_startColor = new Color(m_renderer.material.color.r, m_renderer.material.color.g, m_renderer.material.color.b, 1f);
        m_finalColor = new Color(m_renderer.material.color.r, m_renderer.material.color.g, m_renderer.material.color.b, 0f);
    }

    private void Update()
    {
        if (!m_isFading)
        {
            return;
        }
        float pctComplete = (Time.time - m_startTime) / m_speed;
        Color newColor = Color.Lerp(m_startColor, m_finalColor, pctComplete);
        m_renderer.material.SetColor("_Color", newColor);
        if(pctComplete >= 1)
        {
            m_isFading = false;
        }
    }

}
