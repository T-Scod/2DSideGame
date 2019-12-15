using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive Skills/Damage Boost")]
public class DamageBoost : BaseSkill
{
    [Header("--Skill Init--")]
    public string skillName;
    public string skillDescription;
    public int skillCost;
    public string skillBox;

    [Header("--Skill Stats--")]
    public int boostAmount;

    private string skillType;
    private int originalDamage;
    private GameObject skillBoxObj;
    private TempPlayer PlayerObj;

    public override void Init()
    {
        skillType = "Passive";
        PlayerObj = FindObjectOfType<TempPlayer>();
        skillBoxObj = GameObject.Find(skillBox);

        if (PlayerObj != null) { originalDamage = PlayerObj.damage; }
    }

    #region Skill Intialisation
    public override void Update() { }

    public override int GetCost() { return skillCost; }

    public override string GetDescription() { return skillDescription; }

    public override string GetName() { return skillName; }

    public override GameObject GetSkillBox() { return skillBoxObj; }

    public override void OnEquip()
    {
        PlayerObj.damage += boostAmount;
    }

    public override void OnUnEquip()
    {
        PlayerObj.damage = originalDamage;
    }
    #endregion
}
