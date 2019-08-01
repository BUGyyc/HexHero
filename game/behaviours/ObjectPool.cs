using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPool : MonoBehaviour {
    private GameObject CachePanel;
    private Dictionary<string, Queue<GameObject>> m_Pool = new Dictionary<string, Queue<GameObject>> ();
    private Dictionary<GameObject, string> m_GoTag = new Dictionary<GameObject, string> ();

    public void ClearCachePool () {
        m_Pool.Clear ();
        m_GoTag.Clear ();
    }

    /// <summary>
    /// 从缓存中取
    /// </summary>
    /// <param name="g"></param>
    public void ReturnCacheGameObject (GameObject g) {
        if (CachePanel == null) {
            CachePanel = new GameObject ();
            CachePanel.name = "CachePanel";
            GameObject.DontDestroyOnLoad (CachePanel);
        }

        if (g == null) {
            return;
        }

        //先保存起来
        g.transform.parent = CachePanel.transform;
        g.SetActive (false);

        if (m_GoTag.ContainsKey (g)) {
            string tag = m_GoTag[g];
            RemoveOutMark (g);

            if (!m_Pool.ContainsKey (tag)) {
                m_Pool[tag] = new Queue<GameObject> ();
            }

            m_Pool[tag].Enqueue (g);
        }
    }

    public GameObject RequestCacheGameObject (GameObject prefab) {
        string tag = prefab.GetInstanceID ().ToString ();
        GameObject obj = GetFromPool (tag);
        //如果没取到
        if (obj == null) {
            obj = GameObject.Instantiate<GameObject> (prefab);
            obj.name = prefab.name + Time.time;
        }

        MarkAsOut (obj, tag);
        return obj;
    }

    private GameObject GetFromPool (string tag) {
        if (m_Pool.ContainsKey (tag) && m_Pool[tag].Count > 0) {
            GameObject obj = m_Pool[tag].Dequeue ();
            obj.SetActive (true);
            return obj;
        }
        return null;
    }

    private void MarkAsOut (GameObject gameObject, string tag) {
        m_GoTag.Add (gameObject, tag);
    }

    private void RemoveOutMark (GameObject go) {
        if (m_GoTag.ContainsKey (go)) {
            m_GoTag.Remove (go);
        } else {
            Debug.LogError ("remove out err!!!!");
        }
    }
}