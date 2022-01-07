using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class ObjectPool
{
    private GameObject m_prefab;
    private Transform m_parentTransform;
    private Queue<GameObject> m_objectsAvailable = new Queue<GameObject>();

    public ObjectPool(GameObject prefab, Transform parentTransform, int count)
    {
        m_prefab = prefab;
        m_parentTransform = parentTransform;
        for(int i = 0; i < count; i++)
        {
            Return(AddObj());
        }
    }

    private GameObject AddObj()
    {
        var obj = GameObject.Instantiate(m_prefab, m_parentTransform);
        return obj;
    }

    public GameObject Get()
    {
        if (m_objectsAvailable.Count == 0)
        {
            return AddObj();
        }
        GameObject obj = m_objectsAvailable.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.parent = m_parentTransform;
        m_objectsAvailable.Enqueue(obj);
    }

    public async void Return(GameObject obj, float delay)
    {        
        await Task.Delay(TimeSpan.FromSeconds(delay));
        Return(obj);
    }

}
