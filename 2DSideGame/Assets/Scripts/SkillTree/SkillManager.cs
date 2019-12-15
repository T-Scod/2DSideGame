using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<BaseSkill> equippedSkills = new List<BaseSkill>();
    public List<BaseSkill> skills = new List<BaseSkill>();
    public TempPlayer player;
    public int skillAmount;

    void Awake()
    {
        if (skills.Count > 0)
        {
            foreach (BaseSkill skill in skills)
            {
                skill.Init();
            }
        }
    }

    void Update()
    {
        if (equippedSkills.Count > 0)
        {
            foreach (BaseSkill skillequip in equippedSkills)
            {
                skillequip.Update();
            }
        }
    }

    /// <summary>
    /// Equips the skill to the eqipped list
    /// </summary>
    /// <param name="skillIndex"> Skill being used </param>
    public void EquipSkill(int skillIndex)
    {
        if (equippedSkills.Count != skillAmount && player != null)
        {
            int costLeft = player.cost - skills[skillIndex].GetCost();
            if (costLeft > 0)
            {
                player.cost -= skills[skillIndex].GetCost(); 
                equippedSkills.Add(skills[skillIndex]);
                equippedSkills[skillIndex].OnEquip();
            }
        }
    }

    /// <summary>
    /// Removes the skill from the equipped list
    /// </summary>
    /// <param name="skillIndex"> Skill Being Used </param>
    public void UnequipSkill(int skillIndex)
    {
        if (equippedSkills.Count != skillAmount && player != null)
        {
            player.cost += equippedSkills[skillIndex].GetCost();
            equippedSkills[skillIndex].OnUnEquip();
            equippedSkills.RemoveAt(skillIndex);
        }
    }

}
