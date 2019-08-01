using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroGameUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 关闭城池UI
    /// </summary>
    public void OnCloseTownUI(){
        CEventDispatcherObj.cEventDispatcher.dispatchEvent (new CEvent (CEventName.CLOSE_TOWN_UI), this);
    }
}
