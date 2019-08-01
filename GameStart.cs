using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//整个游戏入口
public class GameStart : MonoBehaviour {
    /// <summary>
    /// 是否开始加载
    /// </summary>
    private bool startLoad;
    /// <summary>
    /// 进度条
    /// </summary>
    public Slider progressUI;
    /// <summary>
    /// 文字进度
    /// </summary>
    public Text progressValue;
    private AsyncOperation prog;

    public Text version;
    private void Awake () {
        this.startLoad = false;
    }
    private void Start () {
        Debug.Log ("GameStart -- Start");
        prog = SceneManager.LoadSceneAsync ("Scene"); //异步加载场景
        StartCoroutine ("LoadingScene");
        //相关初始化
        GameManager.InitGame ();
        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.INIT_COMPLETE, this.gameCreateComplete);
        version.text = "" + Global.version;
    }

    //设置滑动条
    private void setProgressValue (int value) {
        // MyLog.Log ("进度条 " + value);
        progressUI.value = value;
        progressValue.text = (value < 100) ? value + "%(加载资源中...)" : value + "%(加载成功，正在进入游戏)";
    }

    private IEnumerator LoadingScene () {
        prog.allowSceneActivation = false; //如果加载完成，也不进入场景

        int toProgress = 0;
        int showProgress = 0;

        //测试了一下，进度最大就是0.9
        while (prog.progress < 0.9f) {
            toProgress = (int) (prog.progress * 100);

            while (showProgress < toProgress) {
                showProgress++;
                setProgressValue (showProgress);

            }
            yield return new WaitForEndOfFrame (); //等待一帧
        }
        //计算0.9---1   其实0.9就是加载好了，我估计真正进入到场景是1  
        toProgress = 100;

        while (showProgress < toProgress) {
            showProgress++;
            setProgressValue (showProgress);
            yield return new WaitForEndOfFrame (); //等待一帧
        }

        prog.allowSceneActivation = true; //如果加载完成，可以进入场景
    }

    // /// <summary>
    // /// 创建世界成功了
    // /// </summary>
    // /// <param name="evt"></param>
    private void gameCreateComplete (CEvent evt) {
        MyLog.Log ("游戏世界创建完成了----------------------------");
    }
}