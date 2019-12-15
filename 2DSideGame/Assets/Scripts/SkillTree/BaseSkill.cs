using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill : ScriptableObject
{
    public abstract void Init();
    public abstract void Update();
    public abstract string GetName();
    public abstract string GetDescription();
    public abstract int GetCost();
    public abstract GameObject GetSkillBox();
    public abstract void OnEquip();
    public abstract void OnUnEquip();
}
