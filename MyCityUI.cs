using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyCityUI : MonoBehaviour {

    /// <summary>
    /// 英雄页面
    /// </summary>
    public GameObject myHeroPage;
    /// <summary>
    /// 我的仓库
    /// </summary>
    public GameObject myWarehousePage;
    /// <summary>
    /// 黑市
    /// </summary>
    public GameObject mallPage;
    /// <summary>
    /// 酒馆
    /// </summary>
    public GameObject barPage;

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }
    private AsyncOperation prog;
    //切换到游戏场景
    public void OnBackWorldScene () {
        prog = SceneManager.LoadSceneAsync ("Welcome"); //异步加载场景
        StartCoroutine ("LoadingScene");
    }

    IEnumerator LoadingScene () {
        prog.allowSceneActivation = false;
        int toProgress = 0;
        int showProgress = 0;
        while (prog.progress < 0.9f) {
            toProgress = (int) (prog.progress * 100);
            while (showProgress < toProgress) {
                showProgress++;
            }
            yield return new WaitForEndOfFrame (); //等待一帧
        }
        //计算0.9---1   其实0.9就是加载好了，我估计真正进入到场景是1  
        toProgress = 100;
        while (showProgress < toProgress) {
            showProgress++;
            yield return new WaitForEndOfFrame (); //等待一帧
        }
        prog.allowSceneActivation = true; //如果加载完成，可以进入场景
    }
}