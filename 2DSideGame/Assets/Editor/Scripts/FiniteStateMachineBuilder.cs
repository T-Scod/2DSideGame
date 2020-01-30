using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FiniteStateMachineBuilder : EditorWindow
{
    GameObject targetObject;

    List<FSMBStateWindow> states = new List<FSMBStateWindow>();
    List<FSMBTransitionWindow> transitions = new List<FSMBTransitionWindow>();
    List<FSMBConditionWindow> conditions = new List<FSMBConditionWindow>();

    bool makingTransition = false;
    int currentWindowID = 0;
    Vector2 mousePosition;

    //
    // @Summary: Creates a window if one isn't already open
    //
    [MenuItem("Window/FiniteStateMachineBuilder")]
    public static void ShowWindow()
    {
        var window = GetWindow<FiniteStateMachineBuilder>("Finite State Machine Builder");
    }

    //
    // @Summary: Window Code
    //
    private void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        // button for setting target object
        EditorGUIUtility.labelWidth = 80;
        targetObject = EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), allowSceneObjects: true) as GameObject;

        if (GUILayout.Button("Build"))
        {
            BuildStateMachine();
        }

        if (GUILayout.Button("Save as Preset"))
        {
            SaveAsPreset();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //
        // @Todo: Draw Transitions and Conditions
        //

        // draw states
        GUI.BeginGroup(new Rect(0, 0, maxSize.x, maxSize.y));
        BeginWindows();
        foreach (var state in states)
        {
            state.Draw();
        }
        EndWindows();
        GUI.EndGroup();

        // handle events
        Event e = Event.current;
        mousePosition = e.mousePosition;
        switch (e.type)
        {
            // if user has right clicked
            case EventType.ContextClick:
                if (!makingTransition)
                {
                    GenericMenu menu = new GenericMenu();

                    // add state windows
                    menu.AddItem(new GUIContent("Add State/Patrol"), false, StateWindowOfType<FSMBPatrolStateWindow>);
                    menu.AddItem(new GUIContent("Add State/Seek"), false, StateWindowOfType<FSMBSeekStateWindow>);
                    menu.AddItem(new GUIContent("Add State/Flying Dodge"), false, StateWindowOfType<FSMBFlyingDodgeStateWindow>);
                    menu.AddItem(new GUIContent("Add State/Wait"), false, StateWindowOfType<FSMBWaitStateWindow>);

                    menu.DropDown(new Rect(e.mousePosition, new Vector2(0, 0)));
                }
                break;

            default:
                break;
        }
    }

    //
    // @Summary: Builds StateMachine for targetObject
    //
    private void BuildStateMachine()
    {
        //
        // @Todo: Transitions and Conditions
        //      

        Dictionary<int, FSM.FiniteState> objectStates = new Dictionary<int, FSM.FiniteState>();
        Dictionary<int, FSM.Transition> objectTransitions = new Dictionary<int, FSM.Transition>();
        Dictionary<int, FSM.Condition> objectConditions = new Dictionary<int, FSM.Condition>();

        // transform builder state information into actual instances of FiniteStates and add states to
        // target object
        foreach (var state in states)
        {
            if (state is FSMBPatrolStateWindow)
            {
                var patrol = state as FSMBPatrolStateWindow;
                var fsmPatrol = targetObject.AddComponent<FSM.PatrolState>();
                fsmPatrol.speed = patrol.speed;
                fsmPatrol.slowDownDistance = patrol.slowDownDistance;
                fsmPatrol.path = patrol.path;
                objectStates.Add(patrol.id, fsmPatrol);
            }
            else if (state is FSMBSeekStateWindow)
            {
                var seek = state as FSMBSeekStateWindow;
                var fsmSeek = targetObject.AddComponent<FSM.SeekState>();
                fsmSeek.speed = seek.speed;
                fsmSeek.targetTransform = seek.targetTransform;
                objectStates.Add(seek.id, fsmSeek);
            }
            else if (state is FSMBFlyingDodgeStateWindow)
            {
                var dodge = state as FSMBFlyingDodgeStateWindow;
                var fsmDodge = targetObject.AddComponent<FSM.FlyingDodgeState>();
                fsmDodge.dodgeSpeed = dodge.dodgeSpeed;
                fsmDodge.dodgeDistance = dodge.dodgeDistance;
                fsmDodge.detectionRange = dodge.detectionRange;
                objectStates.Add(dodge.id, fsmDodge);
            }
            else if (state is FSMBWaitStateWindow)
            {
                var wait = state as FSMBWaitStateWindow;
                var fsmWait = targetObject.AddComponent<FSM.WaitState>();
                fsmWait.waitDuration = wait.waitDuration;
                objectStates.Add(wait.id, fsmWait);
            }
            else
            {
                Debug.LogErrorFormat("Trying to build Finite State Machine with an unhandled state type: {0}", state.GetType().Name);
                return;
            }
        }

    }

    private void SaveAsPreset()
    {
        var preset = CreateInstance<FiniteStateMachinePreset>();

        // setup preset

        //
        // @Bug: Lists not saved properly because of type mismatch between base class and derived
        //      classes
        //

        preset.states = states;
        preset.transitions = transitions;
        preset.conditions = conditions;
        preset.lastWindowID = currentWindowID;

        string path = EditorUtility.SaveFilePanelInProject(title: "Save Preset", defaultName: "FiniteStateMachinePreset", extension: "asset", "");

        if (path.Length != 0)
        {
            // save asset
            AssetDatabase.CreateAsset(preset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }
    }

    private int NextWindowID()
    {
        return currentWindowID++;
    }

    private void StateWindowOfType<T>() where T : FSMBStateWindow
    {
        var state = CreateInstance<T>();
        state.rect = new Rect(mousePosition, new Vector2(200, 200));
        state.id = NextWindowID();
        states.Add(state);
    }
}

public abstract class FSMBWindow : ScriptableObject
{
    public Rect rect;
    public int id;
}

public abstract class FSMBStateWindow : FSMBWindow
{
    public abstract void Draw();
}

public class FSMBPatrolStateWindow : FSMBStateWindow
{
    public float speed;
    public float slowDownDistance = 1f;
    public EnemyPath path;

    public override void Draw()
    {
        GUI.Window(id, rect, WindowGUI, string.Format("Patrol: {0}", id));
    }

    private void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var speedProperty = so.FindProperty("speed");
        var slowDownDistanceProperty = so.FindProperty("slowDownDistance");
        var pathProperty = so.FindProperty("path");

        EditorGUILayout.PropertyField(speedProperty);
        EditorGUILayout.PropertyField(slowDownDistanceProperty);
        EditorGUILayout.PropertyField(pathProperty, includeChildren: true);

        so.ApplyModifiedProperties();
    }
}

public class FSMBSeekStateWindow : FSMBStateWindow
{
    public float speed;
    public Transform targetTransform;

    public override void Draw()
    {
        GUI.Window(id, rect, WindowGUI, string.Format("Seek: {0}", id));
    }

    private void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var speedProperty = so.FindProperty("speed");
        var targetTransformProperty = so.FindProperty("targetTransform");

        EditorGUILayout.PropertyField(speedProperty);
        EditorGUILayout.PropertyField(targetTransformProperty);

        so.ApplyModifiedProperties();
    }
}

public class FSMBFlyingDodgeStateWindow : FSMBStateWindow
{
    public float dodgeSpeed;
    public float dodgeDistance;
    public float detectionRange;

    public override void Draw()
    {
        GUI.Window(id, rect, WindowGUI, string.Format("Flying Dodge: {0}", id));
    }

    private void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var dodgeSpeedProperty = so.FindProperty("dodgeSpeed");
        var dodgeDistanceProperty = so.FindProperty("dodgeDistance");
        var detectionRangeProperty = so.FindProperty("detectionRange");

        EditorGUILayout.PropertyField(dodgeSpeedProperty);
        EditorGUILayout.PropertyField(dodgeDistanceProperty);
        EditorGUILayout.PropertyField(detectionRangeProperty);

        so.ApplyModifiedProperties();
    }
}

public class FSMBWaitStateWindow : FSMBStateWindow
{
    public float waitDuration;

    public override void Draw()
    {
        GUI.Window(id, rect, WindowGUI, string.Format("Wait: {0}", id));
    }

    private void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var waitDurationProperty = so.FindProperty("waitDuration");

        EditorGUILayout.PropertyField(waitDurationProperty);

        so.ApplyModifiedProperties();
    }
}

public abstract class FSMBTransitionWindow : FSMBWindow
{
    public FSMBStateWindow fromState;
    public FSMBStateWindow toState;
    public List<FSMBConditionWindow> conditions;
}

public abstract class FSMBConditionWindow : FSMBWindow
{
}
