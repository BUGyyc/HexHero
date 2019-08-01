using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 小提示框
/// </summary>
public class Tips : MonoBehaviour {
    private void Start () {
        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.SHOW_TIP, this.showTip);
    }

    private void OnDestroy() {
        CEventDispatcherObj.cEventDispatcher.removeEventListener(CEventName.SHOW_TIP,this.showTip);
    }

    private void showTip (CEvent evt) {
        
    }
}