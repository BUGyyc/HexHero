using System;
using System.Collections;
[Serializable]
public class SkillItem {
    public int id;
    public string name;
    public bool isAttack;
    public bool isAoe;
    public int effectId;
    public int delayTime;
    public bool isSpecial;
    public bool isPassive;
    public int value;
    public bool isBuff;
}