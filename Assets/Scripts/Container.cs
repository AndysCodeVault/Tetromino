using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    private const int MIN_X = -5;
    private const int MAX_X = 6;
    private const int MIN_Y = -10;

    [SerializeField]
    private List<GameObject> m_tetrominoTemplates;

    [SerializeField]
    private GameObject m_blockTemplate;
    private Transform m_blocksTransform;
    private Transform m_tetrominoPlaceholder;

    private List<Transform> m_placedBlocksTransform;

    private void Awake()
    {
        m_blocksTransform = gameObject.transform.Find("Blocks");
        m_placedBlocksTransform = new List<Transform>();
        m_tetrominoPlaceholder = gameObject.transform.Find("Tetromino");
    }

    public bool IsBlockCollided(List<Vector2Int> blocks)
    {
        foreach(Vector2Int block in blocks)
        {
            if(block.x <= MIN_X || block.x >= MAX_X)
            {
                return true;
            }            
            if(block.y <= MIN_Y)
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

    public void SetTetromino(List<Vector2Int> blocks, Color color)
    {
        var yList = new HashSet<int>();

        foreach(Vector2Int block in blocks)
        {
            var newBlock = GameObject.Instantiate(m_blockTemplate, m_blocksTransform);
            newBlock.transform.position = new Vector3(block.x, block.y, 0);
            newBlock.GetComponent<Renderer>().material.SetColor("_Color", color);
            m_placedBlocksTransform.Add(newBlock.transform);
            yList.Add(block.y);
        }

        CheckLineClears(yList);
        GetNextPiece();
    }

    private void CheckLineClears(HashSet<int> yList)
    {
        var lineClears = new List<int>();

        foreach(int y in yList)
        {
            bool isLineClear = true;
            Vector2Int pos = new Vector2Int();
            pos.y = y;
            for(pos.x = MIN_X + 1; pos.x < MAX_X; pos.x += 1)
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
            ClearLines(lineClears);
        }
    }

    private void ClearLines(List<int> lineClears)
    {
        // Destroy all the cubes in the lineClears
        foreach (int y in lineClears)
        {
            Vector2Int pos = new Vector2Int();
            pos.y = y;
            for (pos.x = MIN_X + 1; pos.x < MAX_X; pos.x += 1)
            {                
                var block = GetBlockAtPos(pos);
                m_placedBlocksTransform.Remove(block);                
                GameObject.Destroy(block.gameObject);
            }
        }

        ShiftBlocksUpAfterLineClear(lineClears);
    }

    private void ShiftBlocksUpAfterLineClear(List<int> lineClears)
    {
        // Move all the cubes above a line down
        foreach (Transform t in m_placedBlocksTransform)
        {
            // Count how many cubes to move down
            int moveCount = 0;
            foreach (int y in lineClears)
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

    private void GetNextPiece()
    {
        var index = Mathf.FloorToInt(Random.Range(0, m_tetrominoTemplates.Count));
        var template = m_tetrominoTemplates[index];
        var newTetromino = GameObject.Instantiate(template, m_tetrominoPlaceholder);
        newTetromino.transform.position = new Vector3(0, 11, 0);
    }
}
