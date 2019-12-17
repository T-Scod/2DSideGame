using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField]
    private GameObject m_prefab;

    private Queue<GameObject> m_availableObjects = new Queue<GameObject>();

    public static ObjectPooling Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(m_prefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        m_availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if(m_availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = m_availableObjects.Dequeue();
        instance.SetActive(true);

        return instance;
    }
}
