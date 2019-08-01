using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 英雄的行为类
/// </summary>
public class HeroBehaviours : MonoBehaviour {
    private Animator animator;

    public int HeroId = 0;

    private HeroItem heroItem;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
        animator = this.GetComponent<Animator> ();
        SetAnimatorState ("isIdle");
    }

    public void setHeroId (int id) {
        HeroId = id;
    }

    public void setHeroItem (HeroItem item) {
        heroItem = item;
    }

    public HeroItem getHeroItem () {
        return heroItem;
    }

    /// <summary>
    /// 设置动画状态
    /// </summary>
    /// <param name="str"></param>
    public void SetAnimatorState (string str) {
        int state = 0;
        switch (str) {
            case "isIdle":
                state = 0;
                break;
            case "isRun":
                state = 1;
                break;
            case "isAttack":
                state = 2;
                break;
            case "isDamage":
                state = 3;
                break;
            case "isDied":
                state = 4;
                break;
        }
        animator.SetInteger ("heroState", state);
    }

}