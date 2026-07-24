using System.Collections.Generic;
using UnityEngine;

// Generic, prefab tabanlı object pool.
// Pool boşaldığında otomatik olarak yeni instance oluşturur (Instantiate).
// T mutlaka bir Component (Prefab üzerindeki script) olmalı.
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Queue<T> inactiveObjects = new Queue<T>();

    public ObjectPool(T prefab, int prewarmCount = 0)
    {
        this.prefab = prefab;

        for (int i = 0; i < prewarmCount; i++)
        {
            T obj = CreateNew();
            obj.gameObject.SetActive(false);
            inactiveObjects.Enqueue(obj);
        }
    }

    private T CreateNew()
    {
        T obj = Object.Instantiate(prefab);
        return obj;
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj = inactiveObjects.Count > 0 ? inactiveObjects.Dequeue() : CreateNew();

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        inactiveObjects.Enqueue(obj);
    }

    public int InactiveCount => inactiveObjects.Count;
}