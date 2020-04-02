using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Active Skills/Raging Fury")]
public class RageSkill : BaseSkill
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

    [Header("--Skill Mechanics--")]
    public KeyCode activeKey;
    public int boostAmount;
    public int debuffAmount;
    private float duration;
    public float maxDuration;

    private int originalDamage;
    private int originalHealth;
    private float originalSpeed;
    private GameObject skillBoxObj;
    private GameObject skillIconObj;
    private TempPlayer playerObj;
    private bool isUnlocked;

    public override void Init()
    {
        playerObj = FindObjectOfType<TempPlayer>();
        skillBoxObj = GameObject.Find(skillBox);
        skillIconObj = GameObject.Find(skillIcon);
        isUnlocked = false;

        if (playerObj != null) { originalDamage = playerObj.damage; }
        if (skillBoxObj != null) { skillBoxObj.SetActive(false); }
    }

    #region Skill Initialisation
    public override void Update()
    {
        if (Input.GetKeyDown(activeKey))
        {

        }
    }

    public override int GetCost() { return skillCost; }

    public override string GetDescription() { return skillDescription; }

    public override string GetName() { return skillName; }

    public override string GetSkillType() { return skillType; }

    public override string GetRequirements() { return skillRequirement; }

    public override GameObject GetSkillBox() { return skillBoxObj; }

    public override void OnEquip() {  }

    public override void OnUnEquip() { }

    public override GameObject GetSkillIcon() { return skillIconObj; }

    public override bool UnlockedSkill() { return isUnlocked;  }

    public override void MetRequirements()
    {
        //TODO: Replace with better requirement
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerObj.gold -= 20;
            isUnlocked = true;
        }
    }
    #endregion
}
