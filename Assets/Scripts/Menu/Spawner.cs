using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Vector3 m_startPosition;
    private Vector3 m_endPosition;
    private float m_startTime;
    private GameObject m_tetromino;

    [SerializeField]
    private float m_duration = 3f;

    [SerializeField]
    private List<GameObject> m_tetrominoes;

    private void Awake()
    {
        m_startPosition = transform.Find("Start Position").position;
        m_endPosition = transform.Find("End Position").position;
        m_startTime = Time.time - Random.Range(0, m_duration);
        SelectRandomTetromino();
    }

    private void SelectRandomTetromino() 
    {
        int index = Random.Range(0, m_tetrominoes.Count);
        m_tetromino = GameObject.Instantiate(m_tetrominoes[index], transform);
        m_tetromino.transform.position = m_startPosition;
        Invoke("SetTetrominoColor", 0.01f);
    }

    private void SetTetrominoColor()
    {
        var renderers = m_tetromino.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            var color = renderer.material.color;
            var newColor = new Color(color.r, color.g, color.b, 0.35f);
            renderer.material.color = newColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.time - m_startTime;
        float pctComplete = elapsedTime / m_duration;       
        m_tetromino.transform.position = Vector3.Lerp(m_startPosition, m_endPosition, pctComplete);
        if(pctComplete > 1)
        {
            m_startTime = Time.time;
            GameObject.Destroy(m_tetromino);
            SelectRandomTetromino();
        }
    }
}
