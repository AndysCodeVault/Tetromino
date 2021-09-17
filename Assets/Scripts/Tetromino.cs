using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public int Index { get; set; }

    [SerializeField]
    private Color m_color;

    [SerializeField]
    private List<AudioClip> m_audioClips;

    private const float MOVEMENT_DELAY = 0.25f;
    private const float ROTATE_DELAY = 0.5f;
    private const float FAST_FALLING = 0.025f;
    
    private float m_normalFalling = 0.5f;
    private float m_fallSpeed = 0.5f;
    private float m_lastMoveTime = 0f;
    private float m_lastRotateTime = 0f;
    private float m_lastFallTime = 0f;
    private Transform m_modelTransform;
    private Container m_container;
    private List<Transform> m_blockTransforms;
    private bool m_isActive = false;
    private AudioSource m_audio;

    public void ActivateTetromino(Vector3 finalPos, float fallingSpeed)
    {
        m_normalFalling = fallingSpeed;
        StartCoroutine("RunSlide", finalPos);
    }

    public void DeactivateTetromino()
    {
        m_isActive = false;
    }

    private IEnumerator RunSlide(Vector3 finalPos)
    {
        Vector3 posStart = transform.position;
        float duration = 0.5f;
        float timeStart = Time.time;
        float angle = 0f;
        float angleChange = 360f / duration;

        while (Time.time <= timeStart + duration)
        {
            transform.position = Vector3.Slerp(posStart, finalPos, (Time.time - timeStart) / duration);
            angle = 360f / (duration / (Time.time - timeStart));
            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }
        transform.rotation = Quaternion.identity;
        transform.position = finalPos;
        m_isActive = true;
        m_lastFallTime = Time.time;
        if (m_container.IsBlockCollided(GetBlocks(new Vector2Int(0, 0))))
        {
            m_container.GameOver(gameObject);
            //Destroy(gameObject);
        }
    }

    private void Awake()
    {
        m_modelTransform = gameObject.transform.Find("Model");
        m_container = GameObject.Find("Container").GetComponent<Container>();
        m_blockTransforms = new List<Transform>();
        m_blockTransforms.Add(m_modelTransform.GetChild(0));
        m_blockTransforms.Add(m_modelTransform.GetChild(1));
        m_blockTransforms.Add(m_modelTransform.GetChild(2));
        m_blockTransforms.Add(m_modelTransform.GetChild(3));
        m_audio = gameObject.GetComponent<AudioSource>();
        m_audio.volume = 0.25f;

        // Set the color for each block in a shape
        foreach (var r in transform.GetComponentsInChildren<Renderer>())
        {
            r.material.SetColor("_Color", m_color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_isActive)
        {
            return;
        }
        HandleMovement();
        HandleRotation();
        HandleFalling();
    }

    private void HandleMovement()
    {
        if (Time.time > m_lastMoveTime + MOVEMENT_DELAY)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                Move(new Vector3(1, 0, 0));
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                Move(new Vector3(-1, 0, 0));
            }
        }
    }

    private void HandleRotation()
    {
        if(Time.time > m_lastRotateTime + ROTATE_DELAY)
        {
            if(Input.GetAxis("Vertical") > 0)
            {
                Rotate();
            }
        }
    }

    private void HandleFalling()
    {
        if(Input.GetAxis("Vertical") < 0)
        {
            m_fallSpeed = FAST_FALLING;
        }
        else
        {
            m_fallSpeed = m_normalFalling;
        }

        if(Time.time > m_lastFallTime + m_fallSpeed)
        {
            Fall();
        }
    }

    private void Move(Vector3 movement)
    {
        if (!m_container.IsBlockCollided(GetBlocks(new Vector2Int(Mathf.RoundToInt(movement.x), Mathf.RoundToInt(movement.y)))))
        {
            m_audio.PlayOneShot(m_audioClips[0]);
            gameObject.transform.Translate(movement);
            m_lastMoveTime = Time.time;
        }        
    }

    private void Rotate()
    {
        m_modelTransform.Rotate(0, 0, -90f);
        if (m_container.IsBlockCollided(GetBlocks(new Vector2Int(0, 0))))
        {
            m_modelTransform.Rotate(0, 0, 90f);
        }
        else
        {
            m_audio.PlayOneShot(m_audioClips[1]);
            m_lastRotateTime = Time.time;
        }
    }

    private void Fall()
    {
        if(m_container.IsBlockCollided(GetBlocks(new Vector2Int(0, -1))))
        {
            m_container.SetTetromino(GetBlocks(new Vector2Int(0, 0)), m_color, gameObject);
            //GameObject.Destroy(gameObject);
        }
        else
        {
            gameObject.transform.Translate(new Vector3(0, -1, 0));
        }
        m_lastFallTime = Time.time;
    }

    private List<Vector2Int> GetBlocks(Vector2Int tetrominoOffset)
    {
        List<Vector2Int> blocks = new List<Vector2Int>();
        foreach(Transform blockTransform in m_blockTransforms)
        {
            blocks.Add(new Vector2Int(
                Mathf.RoundToInt(blockTransform.position.x) + tetrominoOffset.x,
                Mathf.RoundToInt(blockTransform.position.y) + tetrominoOffset.y));
        }
        return blocks;
    }
}
