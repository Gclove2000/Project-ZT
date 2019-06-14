﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;
    public static ObjectPool Instance
    {
        get
        {
            if (!instance || !instance.gameObject)
                instance = FindObjectOfType<ObjectPool>();
            return instance;
        }
    }

    private Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    [SerializeField]
#if UNITY_EDITOR
    [DisplayName("池子")]
#endif
    private Transform poolRoot;//用于放置失效对象
    [SerializeField]
#if UNITY_EDITOR
    [DisplayName("清理周期(秒)")]
#endif
    private float cleanDelayTime = 600.0f;//池子东西放多放久了臭，要按时排掉

    public void Put(GameObject gameObject)
    {
        if (!gameObject)
        {
            return;
        }
        MyUtilities.SetActive(gameObject, false);
        gameObject.transform.SetParent(poolRoot, false);
        string name = gameObject.name;
        if (pool.ContainsKey(name))
        {
            pool[name].Add(gameObject);
        }
        else
        {
            pool.Add(name, new List<GameObject>() { gameObject });
        }
    }

    public void Put(GameObject gameObject, float delayTime)
    {
        StartCoroutine(PutDelay(gameObject, delayTime));
    }

    public GameObject Get(GameObject prefab, Transform parent = null, bool worldPositonStays = false)
    {
        string goName = prefab.name + "(Clone)";
        if (pool.ContainsKey(goName) && pool[goName].Count > 0)
        {
            GameObject go = pool[goName][0];
            pool[goName].Remove(go);
            if (pool[goName].Count < 1) pool.Remove(goName);
            go.transform.SetParent(parent, worldPositonStays);
            MyUtilities.SetActive(go, true);
            return go;
        }
        else
        {
            GameObject go = Instantiate(prefab, parent, worldPositonStays) as GameObject;
            return go;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = false)
    {
        string goName = prefab.name + "(Clone)";
        if (pool.ContainsKey(goName) && pool[goName].Count > 0)
        {
            GameObject go = pool[goName][0];
            go.transform.position = position;
            go.transform.rotation = rotation;
            pool[goName].Remove(go);
            go.transform.SetParent(parent, worldPositionStays);
            MyUtilities.SetActive(go, true);
            return go;
        }
        else
        {
            GameObject go = Instantiate(prefab, position, rotation) as GameObject;
            if (parent) go.transform.SetParent(parent, worldPositionStays);
            return go;
        }
    }

    private void Awake()
    {
        if (!poolRoot) poolRoot = transform;
        StartCoroutine(CleanDelay());
    }

    IEnumerator PutDelay(GameObject gameObject, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Put(gameObject);
    }

    IEnumerator CleanDelay()
    {
        yield return new WaitForSeconds(cleanDelayTime);
        Clean();
        StartCoroutine(CleanDelay());
    }

    public void Clean()
    {
        if (pool.Count > 0)
        {
            List<string> keys = new List<string>(pool.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                for (int j = pool[keys[i]].Count - 1; j >= 0; j--)
                {
                    Destroy(pool[keys[i]][j]);
                }
            }
            pool.Clear();
            System.GC.Collect();
        }
    }
}