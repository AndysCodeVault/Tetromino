using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Presets;

public class Container : MonoBehaviour
{
    private const int MIN_X = -5;
    private const int MAX_X = 6;
    private const int MIN_Y = -10;

    [SerializeField]
    private List<GameObject> m_tetrominoTemplates;

    [SerializeField]
    private GameObject m_blockTemplate;

    [SerializeField]
    private List<AudioClip> m_audioClips;

    private Transform m_blocksTransform;
    private Transform m_tetrominoPlaceholder;
    private Transform m_nextPiecePlaceholder;
    private List<Transform> m_placedBlocksTransform;
    private List<int> m_lineClears;
    private TMP_Text m_scoreText;
    private TMP_Text m_linesText;
    private TMP_Text m_levelText;
    private TMP_Text m_popupText;
    private int m_score;
    private int m_lines;
    private int m_level;
    private AudioSource m_audio;
    private List<ObjectPool> m_tetrominosPools;
    private ObjectPool m_blocksPool;
    private string m_movementNamePrefix;

    [SerializeField]
    private int m_playerNumber = 0;

    private void Awake()
    {
        if (Settings.mode == PlayerMode.Single)
        {
            m_movementNamePrefix = "SinglePlayer";
        }
        else
        {
            m_movementNamePrefix = Settings.GetControlType(m_playerNumber);
        }        

        m_blocksTransform = gameObject.transform.Find("Blocks");
        m_placedBlocksTransform = new List<Transform>();
        m_tetrominoPlaceholder = gameObject.transform.Find("Tetromino");
        m_nextPiecePlaceholder = transform.Find("Menu Next/Next Piece");
        m_scoreText = transform.Find("Menu Score/Canvas/Text (TMP)").GetComponent<TMP_Text>();
        m_linesText = transform.Find("Menu Lines/Canvas/Text (TMP)").GetComponent<TMP_Text>();
        m_levelText = transform.Find("Menu Level/Canvas/Text (TMP)").GetComponent<TMP_Text>();
        m_popupText = transform.Find("Menu Popup/Canvas/Text (TMP)").GetComponent<TMP_Text>();
        m_audio = gameObject.GetComponent<AudioSource>();
        m_audio.volume = 0.25f;
        m_tetrominosPools = new List<ObjectPool>();
        foreach(var tetromino in m_tetrominoTemplates)
        {
            m_tetrominosPools.Add(new ObjectPool(tetromino, transform.Find("ObjectPoolsObjects"), 2));
        }
        m_blocksPool = new ObjectPool(m_blockTemplate, transform.Find("ObjectPoolsObjects"), 120);

        m_scoreText.text = "";
        m_linesText.text = "";
        m_levelText.text = "";
        m_popupText.text = "";
    }

    private void Start()
    {
        Invoke("BeginGame", 1f);
    }

    private void BeginGame()
    {
        m_score = 0;
        m_lines = 0;
        m_level = 1;
        UpdateScore(0);
        CreateNextPiece();
        GetNextPiece();
    }

    private void UpdateScore(int lines)
    {
        m_score += lines * 100;
        if(lines == 4)
        {
            // Play tetris sound
            m_audio.PlayOneShot(m_audioClips[2]);
            m_score += 600;
        }
        m_lines += lines;
        if(m_lines >= m_level * 10)
        {
            // Play level complete sound
            m_audio.PlayOneShot(m_audioClips[0]);
            m_level += 1;
            m_popupText.text = "Level " + m_level;
            Invoke("ClearPopup", 2.5f);
        }
        m_scoreText.text = "Score\n" + m_score;
        m_linesText.text = "Lines\n" + m_lines;
        m_levelText.text = "Level\n" + m_level;
    }

    private void ClearPopup()
    {
        m_popupText.text = "";
    }

    public bool IsBlockCollided(List<Vector2Int> blocks)
    {
        int minX = Mathf.RoundToInt(transform.position.x) + MIN_X;
        int maxX = Mathf.RoundToInt(transform.position.x) + MAX_X;
        int minY = Mathf.RoundToInt(transform.position.y) + MIN_Y;

        foreach (Vector2Int block in blocks)
        {
            if(block.x <= minX || block.x >= maxX)
            {
                return true;
            }            
            if(block.y <= minY)
            {
                return true;
            }

            foreach(var placedBlock in m_placedBlocksTransform)
            {
                if(placedBlock.position.x == block.x && placedBlock.position.y == block.y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetTetromino(List<Vector2Int> blocks, Color color, GameObject tetromino)
    {
        var yList = new HashSet<int>();

        foreach(Vector2Int block in blocks)
        {            
            var newBlock = m_blocksPool.Get();
            newBlock.transform.parent = m_blocksTransform;
            newBlock.transform.position = new Vector3(block.x, block.y, 0);
            newBlock.GetComponent<Renderer>().material.SetColor("_Color", color);
            m_placedBlocksTransform.Add(newBlock.transform);
            yList.Add(block.y);
        }

        CheckLineClears(yList);
        GetNextPiece();

        //Destroy tetromino
        var tetrominoScript = tetromino.GetComponent<Tetromino>();
        tetrominoScript.DeactivateTetromino();
        m_tetrominosPools[tetrominoScript.Index].Return(tetromino);
    }

    private void CheckLineClears(HashSet<int> yList)
    {
        var lineClears = new List<int>();

        int minX = Mathf.RoundToInt(transform.position.x) + MIN_X;
        int maxX = Mathf.RoundToInt(transform.position.x) + MAX_X;

        foreach(int y in yList)
        {
            bool isLineClear = true;
            Vector2Int pos = new Vector2Int();
            pos.y = y;
            for(pos.x = minX + 1; pos.x < maxX; pos.x += 1)
            {
                if(!IsBlockAtPos(pos))
                {
                    isLineClear = false;
                    break;
                }
            }
            if(isLineClear)
            {
                lineClears.Add(pos.y);
            }
        }

        if(lineClears.Count > 0)
        {
            // Play line clear sound
            m_audio.PlayOneShot(m_audioClips[1]);
            UpdateScore(lineClears.Count);
            ClearLines(lineClears);
        }
    }

    private void ClearLines(List<int> lineClears)
    {
        int minX = Mathf.RoundToInt(transform.position.x) + MIN_X;
        int maxX = Mathf.RoundToInt(transform.position.x) + MAX_X;

        // Destroy all the cubes in the lineClears
        foreach (int y in lineClears)
        {
            Vector2Int pos = new Vector2Int();
            pos.y = y;
            for (pos.x = minX + 1; pos.x < maxX; pos.x += 1)
            {                
                var block = GetBlockAtPos(pos);
                m_placedBlocksTransform.Remove(block);

                float delay = Mathf.Abs((float)pos.x - transform.position.x) / 10f;
                var fadeScript = block.GetComponent<FadeOut>();
                fadeScript.BeginFade(delay, 0.5f);

                m_blocksPool.Return(block.gameObject, 1.05f);
            }
        }

        m_lineClears = lineClears;
        Invoke("ShiftBlocksUpAfterLineClear", 1f);
    }

    private void ShiftBlocksUpAfterLineClear()
    {
        // Move all the cubes above a line down
        foreach (Transform t in m_placedBlocksTransform)
        {
            // Count how many cubes to move down
            int moveCount = 0;
            foreach (int y in m_lineClears)
            {
                if (t.position.y > y)
                {
                    moveCount += 1;
                }
            }

            if (moveCount > 0)
            {
                t.Translate(new Vector3(0, -moveCount));
            }
        }
    }

    private bool IsBlockAtPos(Vector2Int pos)
    {
        foreach (Transform t in m_placedBlocksTransform)
        {
            if (Mathf.FloorToInt(t.position.x) == pos.x && Mathf.FloorToInt(t.position.y) == pos.y)
            {
                return true;
            }
        }
        return false;
    }

    private Transform GetBlockAtPos(Vector2Int pos)
    {
        foreach (Transform t in m_placedBlocksTransform)
        {
            if (Mathf.FloorToInt(t.position.x) == pos.x && Mathf.FloorToInt(t.position.y) == pos.y)
            {
                return t;
            }
        }
        return null;
    }

    private void CreateNextPiece()
    {
        var index = Mathf.FloorToInt(Random.Range(0, m_tetrominoTemplates.Count));
        var template = m_tetrominoTemplates[index];
        var newTetromino = m_tetrominosPools[index].Get();
        newTetromino.transform.parent = m_nextPiecePlaceholder;
        newTetromino.transform.Find("Model").rotation = template.transform.Find("Model").rotation;
        newTetromino.GetComponent<Tetromino>().Index = index;
        if(index == 0 || index == 3)
        {
            newTetromino.transform.localPosition = new Vector3(-0.75f, 0, 0);
        }
        else if(index == 1)
        {
            newTetromino.transform.localPosition = new Vector3(0, -0.65f, 0);
        }
        else
        {
            newTetromino.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    private void GetNextPiece()
    {
        var nextPiece = m_nextPiecePlaceholder.GetChild(0);
        nextPiece.transform.parent = m_tetrominoPlaceholder;
        float fallingSpeed = 1f / (m_level + 1f);
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y + 11, transform.position.z);
        nextPiece.GetComponent<Tetromino>().ActivateTetromino(startPos, fallingSpeed, m_movementNamePrefix, gameObject);
        Invoke("CreateNextPiece", 0.25f);
    }

    public void GameOver(GameObject lastTetromino)
    {
        m_popupText.text = "Game Over";
        for(int i = 0; i < m_blocksTransform.childCount; i++)
        {
            var block = m_blocksTransform.GetChild(i);
            block.gameObject.AddComponent<Rigidbody>();
            float forceValue = 1000f;
            var force = new Vector3(
                Random.Range(-forceValue, forceValue), 
                Random.Range(-forceValue, forceValue), 
                Random.Range(-forceValue, forceValue));
            block.GetComponent<Rigidbody>().AddForce(force);
        }

        m_tetrominosPools[lastTetromino.GetComponent<Tetromino>().Index].Return(lastTetromino);
    }
}
