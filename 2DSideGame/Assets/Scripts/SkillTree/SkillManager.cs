using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("-- Skill Init --")]
    [Tooltip("This is used to display how many skills the player can equip")]
    public List<BaseSkill> equippedSkills = new List<BaseSkill>();
    [Tooltip("This is used to show how many skills in the players has acquired")]
    public List<BaseSkill> skills = new List<BaseSkill>();
    [Tooltip("UI Skill Tab that opens on KeyPress")]
    public GameObject skillTab;

    public TempPlayer player;
    public int skillAmount;

    private Ray ray;
    private bool tabOpen;
    private GameObject prevSkillBox;

    protected void Awake()
    {
        if (skills.Count > 0)
        {
            foreach (BaseSkill skill in skills)
            {
                skill.Init();
            }
        }

        if (skillTab != null) { skillTab.SetActive(false); }
        tabOpen = false;
    }

    protected void Update()
    {
        if (equippedSkills.Count > 0)
        {
            foreach (BaseSkill skillequip in equippedSkills)
            {
                skillequip.Update();
            }
        }

        if (skills.Count > 0)
        {
            foreach (BaseSkill skill in skills)
            {
                skill.MetRequirements();
            }
        }

        if (skillTab != null)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!skillTab.activeInHierarchy && !tabOpen)
                {
                    tabOpen = true;
                    skillTab.SetActive(true);
                }
                else if (skillTab.activeInHierarchy && tabOpen)
                {
                    tabOpen = false;
                    skillTab.SetActive(false);
                }
            }
             OnHover();
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
            // Calculate if player has enough cost to equip
            int costLeft = player.cost - skills[skillIndex].GetCost();
            if (costLeft > 0 && skills[skillIndex].UnlockedSkill() == true && !equippedSkills.Contains(skills[skillIndex]))
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
            // returns cost back to normal
            player.cost += equippedSkills[skillIndex].GetCost();
            equippedSkills[skillIndex].OnUnEquip();
            equippedSkills.RemoveAt(skillIndex);
        }
    }

    /// <summary>
    /// Used to acquire a skill
    /// </summary>
    /// <param name="skill"></param>
    public void AcquireSkill(BaseSkill skill)
    {
        skills.Add(skill);
    }

    /// <summary>
    /// Manages Skillboxes when hovering over specified skill
    /// </summary>
    protected void OnHover()
    {
        if (skillTab.activeInHierarchy)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Skill"))
            {
                prevSkillBox = hit.transform.gameObject;
                prevSkillBox.GetComponent<SkillBox>().Skill.GetSkillBox().SetActive(true);
            }
            else
            {
                if (prevSkillBox != null)
                {
                    prevSkillBox.GetComponent<SkillBox>().Skill.GetSkillBox().SetActive(false);
                    prevSkillBox = null; 
                }
            }          
        }
    }
}
