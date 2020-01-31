using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FiniteStateMachineBuilder : EditorWindow
{
    public static FiniteStateMachineBuilder instance;

    FiniteStateMachineBuilder()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    GameObject targetObject;

    public List<FSMBStateWindow> states = new List<FSMBStateWindow>();
    public List<FSMBTransitionWindow> transitions = new List<FSMBTransitionWindow>();
    public List<FSMBConditionWindow> conditions = new List<FSMBConditionWindow>();

    bool makingTransition = false;
    int currentWindowID = 0;
    Vector2 mousePosition;
    Vector2 scrollPosition;

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
        targetObject = EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), allowSceneObjects: true, GUILayout.MinWidth(300)) as GameObject;

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

                    // add transition windows
                    menu.AddItem(new GUIContent("Add Transition"), false, TransitionWindow);

                    menu.DropDown(new Rect(e.mousePosition, new Vector2(0, 0)));
                }
                break;

            default:
                break;
        }

        states.RemoveAll(s => s.id == -1);
        transitions.RemoveAll(t => t.id == -1);
        conditions.RemoveAll(c => c.id == -1);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        //
        // @Todo: Draw Transitions and Conditions
        //

        GUI.BeginGroup(new Rect(0, 0, maxSize.x, maxSize.y));
        BeginWindows();
        // draw transition windows
        foreach (var transition in transitions)
        {
            transition.Draw();
        }

        // draw states
        foreach (var state in states)
        {
            state.Draw();
        }
        EndWindows();
        GUI.EndGroup();

        EditorGUILayout.EndScrollView();
    }

    //
    // @Summary: Builds StateMachine for targetObject
    //
    private void BuildStateMachine()
    {
        if (targetObject == null)
        {
            return;
        }

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
        state.rect = new Rect(mousePosition.x, mousePosition.y, 200f, 200f);
        state.id = NextWindowID();
        states.Add(state);
    }

    private void TransitionWindow()
    {
        var transition = CreateInstance<FSMBTransitionWindow>();
        transition.rect = new Rect(mousePosition.x, mousePosition.y, 200f, 200f);
        transition.id = NextWindowID();
        transitions.Add(transition);
    }
}

public abstract class FSMBWindow : ScriptableObject
{
    public Rect rect;
    public int id;
    protected Vector2 scrollPosition;

    public void Draw()
    {
        rect = GUILayout.Window(id, rect, WindowGUI, string.Format("{0}: {1}", GetName(), id));
    }

    public abstract string GetName();

    //
    // @Note: Make sure this is called at the end of overriding functions
    //
    protected virtual void WindowGUI(int id)
    {
        EditorGUIUtility.labelWidth = 40;
        if (EditorGUILayout.Toggle("Delete", false))
        {
            this.id = -1;
        }

        GUI.DragWindow();
    }
}

public abstract class FSMBStateWindow : FSMBWindow
{
}

public class FSMBPatrolStateWindow : FSMBStateWindow
{
    public float speed;
    public float slowDownDistance = 1f;
    public EnemyPath path;

    public override string GetName()
    {
        return "Patrol";
    }

    protected override void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var speedProperty = so.FindProperty("speed");
        var slowDownDistanceProperty = so.FindProperty("slowDownDistance");
        var pathProperty = so.FindProperty("path");

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.PropertyField(speedProperty);
        EditorGUILayout.PropertyField(slowDownDistanceProperty);
        EditorGUILayout.PropertyField(pathProperty, includeChildren: true);

        so.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();

        base.WindowGUI(id);
    }
}

public class FSMBSeekStateWindow : FSMBStateWindow
{
    public float speed;
    public Transform targetTransform;

    public override string GetName()
    {
        return "Seek";
    }

    protected override void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var speedProperty = so.FindProperty("speed");
        var targetTransformProperty = so.FindProperty("targetTransform");

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.PropertyField(speedProperty);
        EditorGUILayout.PropertyField(targetTransformProperty);

        so.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();

        base.WindowGUI(id);
    }
}

public class FSMBFlyingDodgeStateWindow : FSMBStateWindow
{
    public float dodgeSpeed;
    public float dodgeDistance;
    public float detectionRange;

    public override string GetName()
    {
        return "Flying Dodge";
    }

    protected override void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var dodgeSpeedProperty = so.FindProperty("dodgeSpeed");
        var dodgeDistanceProperty = so.FindProperty("dodgeDistance");
        var detectionRangeProperty = so.FindProperty("detectionRange");

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.PropertyField(dodgeSpeedProperty);
        EditorGUILayout.PropertyField(dodgeDistanceProperty);
        EditorGUILayout.PropertyField(detectionRangeProperty);

        so.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();

        base.WindowGUI(id);
    }
}

public class FSMBWaitStateWindow : FSMBStateWindow
{
    public float waitDuration;

    public override string GetName()
    {
        return "Wait";
    }

    protected override void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var waitDurationProperty = so.FindProperty("waitDuration");

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.PropertyField(waitDurationProperty);

        so.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();

        base.WindowGUI(id);
    }
}

public class FSMBTransitionWindow : FSMBWindow
{
    public int fromState;
    public int toState;
    public List<FSMBConditionWindow> conditions;

    public override string GetName()
    {
        return "Transition";
    }

    protected override void WindowGUI(int id)
    {
        var so = new SerializedObject(this);
        var fromTargetProperty = so.FindProperty("fromState");
        var toStateProperty = so.FindProperty("toState");
        var conditionsProperty = so.FindProperty("conditions");

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.PropertyField(fromTargetProperty);
        EditorGUILayout.PropertyField(toStateProperty);
        EditorGUILayout.PropertyField(conditionsProperty, includeChildren: true);

        so.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();

        // connect from state and to state with line
        Handles.BeginGUI();
        {
            FSMBStateWindow state = null;
            foreach (var s in FiniteStateMachineBuilder.instance.states)
            {
                if (s.id == fromState)
                {
                    state = s;
                }
            }
            if (state != null)
            {
                Handles.DrawLine(state.rect.center, rect.center);
            }
        }
        {
            FSMBStateWindow state = null;
            foreach (var s in FiniteStateMachineBuilder.instance.states)
            {
                if (s.id == toState)
                {
                    state = s;
                }
            }
            if (state != null)
            {
                Handles.DrawLine(state.rect.center, rect.center);
            }
            Handles.EndGUI();
        }

        base.WindowGUI(id);
    }
}

public abstract class FSMBConditionWindow : FSMBWindow
{
}
