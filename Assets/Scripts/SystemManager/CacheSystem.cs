using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheSystem
{
    Dictionary<string, Queue<GameObject>> CacheData = new Dictionary<string, Queue<GameObject>>();

    public Queue<GameObject> GetCache(string name) => CacheData[name];


    public void GenerateCache(GameObject CacheObject, string objectName, int cacheCount)
    {
        if(CacheData.ContainsKey(objectName))
        {
            Debug.LogWarning(objectName + " is already cache generated");
            return;
        }
        Queue<GameObject> queue = new Queue<GameObject>();


        GameObject Folder = new GameObject(objectName);

        for (int i = 0; i < cacheCount; i++)
        {
            GameObject go = Object.Instantiate<GameObject>(CacheObject, Folder.transform);
            go.SetActive(false);
            queue.Enqueue(go);
        }
        CacheData.Add(objectName, queue);
    }

    public GameObject TakeCache(string objectName)
    {
        if(!CacheData.ContainsKey(objectName))
        {
            Debug.LogError(objectName + " is not generate");
            return null;
        }

        if (CacheData[objectName].Count == 0)
        {
            Debug.LogWarning(objectName + "'s genarate Count is zero");
            return null;
        }
        GameObject go = CacheData[objectName].Dequeue();

        return go;
    }

    public void RestoreCache(GameObject UsedObject, string objectName)
    {
        if(!CacheData.ContainsKey(objectName))
        {
            Debug.LogWarning(objectName + "'s cachebox is not generate");
            return;
        }
        UsedObject.SetActive(false);
        CacheData[objectName].Enqueue(UsedObject);
    }
}
