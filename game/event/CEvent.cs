public class CEvent {
    public string eventName; //事件名称
    public object eventParams; //事件传的参数
    public object target; //事件抛出者

    public CEvent (string name, object p = null) {
        this.eventName = name;
        this.eventParams = p;
    }
}