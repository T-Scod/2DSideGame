using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Finite State Machine Preset", menuName = "Finite State Machine Presets/Preset")]
public class FiniteStateMachinePreset : ScriptableObject
{
    public List<FSMBStateWindow> states;
    public List<FSMBTransitionWindow> transitions;
    public List<FSMBConditionWindow> conditions;
    public int lastWindowID;
}
