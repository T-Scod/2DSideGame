using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive Skills/Damage Boost")]
public class DamageBoost : BaseSkill
{
    [Header("--Skill Init--")]
    public string skillName;
    public string skillDescription;
    public string skillType;
    public int skillCost;
    public string skillRequirement;
    [Tooltip("Name of the Skill Box in the Canvas")]
    public string skillBox;
    [Tooltip("Name of the Skill Icon in the Canvas")]
    public string skillIcon;

    [Header("--Skill Stats--")]
    public int boostAmount;

    private int originalDamage;
    private GameObject skillBoxObj;
    private GameObject skillIconObj;
    private TempPlayer PlayerObj;
    private bool isUnlocked;

    public override void Init()
    {
        PlayerObj = FindObjectOfType<TempPlayer>();
        skillBoxObj = GameObject.Find(skillBox);
        skillIconObj = GameObject.Find(skillIcon);
        isUnlocked = false;

        if (PlayerObj != null) { originalDamage = PlayerObj.damage; }
        if (skillBoxObj != null) { skillBoxObj.SetActive(false); }
    }

    #region Skill Intialisation
    public override void Update() {  }

    public override int GetCost() { return skillCost; }

    public override string GetDescription() { return skillDescription; }

    public override string GetName() { return skillName; }

    public override string GetSkillType() { return skillType; }

    public override string GetRequirements() { return skillRequirement;  }

    public override GameObject GetSkillBox() { return skillBoxObj; }

    public override void OnEquip() { PlayerObj.damage += boostAmount; }

    public override void OnUnEquip() { PlayerObj.damage = originalDamage; }

    public override GameObject GetSkillIcon() { return skillIconObj; }

    public override bool UnlockedSkill() { return isUnlocked; }

    public override void MetRequirements()
    {
        //TODO: Replace with better requirement
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerObj.gold -= 20;
            isUnlocked = true;
        }
    }
    #endregion
}
