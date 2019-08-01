using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPooler : MonoBehaviour {
    public List<GameObject> pooledText;
    public GameObject textToPool;
    public int amountToPool;
    // Start is called before the first frame update
    void Start () {
        pooledText = new List<GameObject> ();
        for (int i = 0; i < amountToPool; i++) {
            GameObject obj = (GameObject) Instantiate (textToPool);
            obj.transform.SetParent (gameObject.transform, false);
            obj.SetActive (false);
            pooledText.Add (obj);
        }
    }

    /// <summary>
    /// 从对象池中取
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledText () {
        for (int i = 0; i < pooledText.Count; i++) {
            if (!pooledText[i].activeInHierarchy) {
                return pooledText[i];
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update () {

    }
}