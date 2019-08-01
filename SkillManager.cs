using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {
    /// <summary>
    /// 挂载了ObjectPool脚本的对象
    /// </summary>
    public GameObject PoolObject;
    /// <summary>
    /// 普通火焰效果
    /// </summary>     
    public GameObject pNormalFirePrefab;

    private List<GameObject> pool = new List<GameObject> ();

    /// <summary>
    /// 根据技能Id释放技能
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <param name="delayTime"></param>
    public void onPlayEffectById (int id, Vector3 pos, float delayTime) {
        GameObject obj = getParticleGameObjectById (id);
        if (obj == null) return;
        GameObject gameObject = getGameObjectFromPool (obj);
        if (gameObject == null) return;
        gameObject.transform.position = pos;
        gameObject.GetComponent<ParticleSystem> ().Play ();
        //定时销毁
        StartCoroutine (onDestoryParticle (gameObject, delayTime));
    }

    /// <summary>
    /// 从对象池中取
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    private GameObject getGameObjectFromPool (GameObject prefab) {
        return PoolObject.GetComponent<ObjectPool> ().RequestCacheGameObject (prefab);
    }

    /// <summary>
    /// 通过技能ID来获取prefab
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private GameObject getParticleGameObjectById (int id) {
        switch (id) {
            case 1:
                return pNormalFirePrefab;
        }
        return null;
    }

    IEnumerator onDestoryParticle (GameObject obj, float delayTime) {
        yield return new WaitForSeconds (delayTime);
        // obj.SetActive (false);
        //回收特效
        PoolObject.GetComponent<ObjectPool> ().ReturnCacheGameObject (obj);
    }

}