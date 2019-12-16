using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Used to Setup the skill box that represents everything the skill does
 */
public class SkillBox : MonoBehaviour
{
    [Header("-- Init --")]
    public BaseSkill Skill;
    public Text Name;
    public Text Description;
    public Text Type;
    public Text Cost;
    public Text Requirement;

    void Start()
    {
        if (Skill != null && gameObject != null &&
            Name != null && Description != null &&
            Type != null && Cost != null && Requirement != null)
        {
            Name.text = Skill.GetName();
            Description.text = Skill.GetDescription();
            Type.text = Skill.GetSkillType();
            Cost.text = Skill.GetCost().ToString();
            Requirement.text = Skill.GetRequirements().ToString();
        }
    }
}
