public class CEventListener {
    public CEventListener () { }
    //委托
    public delegate void CEventListenerDelegate (CEvent evt);

    public event CEventListenerDelegate OnEvent;

    public void Execute (CEvent evt) {
        if (OnEvent != null) {
            this.OnEvent (evt);
        }
    }
}