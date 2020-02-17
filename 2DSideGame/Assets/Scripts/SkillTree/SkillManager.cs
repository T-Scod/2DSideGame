using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("-- Skill Init --")]
    [Tooltip("This is used to display how many skills the player can equip (capped at 3)")]
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
        // If we have at least 1 skill
        if (skills.Count > 0)
        {
            // intialise the skills available
            foreach (BaseSkill skill in skills)
            {
                skill.Init();
            }
        }

        // Disables the tab
        if (skillTab != null) { skillTab.SetActive(false); }
        tabOpen = false;
    }

    protected void Update()
    {
        // If we have at least 1 equipped skill
        if (equippedSkills.Count > 0)
        {
            // update each skill equipped
            foreach (BaseSkill skillequip in equippedSkills)
            {
                skillequip.Update();
            }
        }

        // if we have at least 1 skill
        if (skills.Count > 0)
        {
            // check if we are able to unlock the skill
            foreach (BaseSkill skill in skills)
            {
                skill.MetRequirements();
            }
        }

        if (skillTab != null)
        {
            // Opens/Closes the Skill Tab
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
            if (equippedSkills.Count > 0)
            {
                // returns cost back to normal
                player.cost += equippedSkills[skillIndex].GetCost();
                equippedSkills[skillIndex].OnUnEquip();
                equippedSkills.RemoveAt(skillIndex);
            }
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
        // If the tab is open
        if (skillTab.activeInHierarchy)
        {
            // Display the information of the skill while the mouse is on the icon
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
                    // Stores the previous icon hovered over
                    prevSkillBox.GetComponent<SkillBox>().Skill.GetSkillBox().SetActive(false);
                    prevSkillBox = null; 
                }
            }          
        }
    }
}
