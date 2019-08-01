using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger {
    /// <summary>
    /// 用来委托事件
    /// </summary>
    /// <param name="obj"></param>
    public delegate void VoidDelegate(GameObject obj);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    
}