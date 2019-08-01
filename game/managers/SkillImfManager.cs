using System;
using System.Collections;
using System.Collections.Generic;
public class SkillImfManager {
    public static SkillImf skillImf;
    public static void InitSkillManager () {
        loadAllSkillImf ();
    }

    private static void loadAllSkillImf () {
        skillImf = ConfigUtils.loadAllSkillConfig ();
    }

    /// <summary>
    /// 通过ID获取技能
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static SkillItem getSkillItemById (int id) {
        foreach (var item in skillImf.skillItems) {
            if (item.id == id) {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 获得特效ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static int getSkillEffectIdById (int id) {
        SkillItem item = getSkillItemById (id);
        if (item == null) return -1;
        return item.effectId;
    }

    /// <summary>
    /// 获得技能名称
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string getSkillNameById (int id) {
        SkillItem item = getSkillItemById (id);
        if (item == null) return "";
        return item.name;
    }
}