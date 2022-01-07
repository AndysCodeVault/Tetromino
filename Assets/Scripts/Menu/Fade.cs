using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private float m_duration = 1f;
    private float m_startTime;
    private Color m_startColor;
    private Color m_endColor;
    private Image m_image;
    private bool m_isFadeIn;

    private void Start()
    {        
        m_image = gameObject.GetComponent<Image>();
        BeginFadeIn();
    }

    public void BeginFadeIn()
    {
        m_isFadeIn = true;
        m_startTime = Time.time;
        m_startColor = m_image.color;
        m_endColor = new Color(m_startColor.r, m_startColor.g, m_startColor.b, 0f);
    }

    public void BeginFadeOut()
    {
        m_isFadeIn = false;
        m_startTime = Time.time;
        m_startColor = m_image.color;
        m_endColor = new Color(m_startColor.r, m_startColor.g, m_startColor.b, 1f);
        gameObject.SetActive(true);
    }

    private void Update()
    {
        float elapsedTime = Time.time - m_startTime;
        float pctComplete = elapsedTime / m_duration;
        m_image.color = Color.Lerp(m_startColor, m_endColor, pctComplete);

        if(pctComplete >= 1 && m_isFadeIn)
        {            
            gameObject.SetActive(false);
        }
    }
}
