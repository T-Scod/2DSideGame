using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    [System.Serializable]
    public enum StateType
    {
        WAIT,
        SEEK,
    }

    [System.Serializable]
    public enum ConditionType
    {
        PLAYER_PROXIMITY,
        PROJECTILE_THREAT,
        STATE_COMPLETE,
    }

    public class Builder : EditorWindow
    {
        static List<BaseNode> windows = new List<BaseNode>();
        static Enemy targetObject;
        Vector3 mousePosition;
        bool makeTransition;
        bool clickedOnWindow;
        BaseNode selectedNode;

        public enum UserActions
        {
            ADD_WAIT_STATE,
            ADD_SEEK_STATE,
            MAKE_TRANSITION_START,
            MAKE_TRANSITION_END,
            DELETE_NODE,
            DELETE_TRANSITION,
            COMMENT_NODE
        }

        [MenuItem("Window/FSM Builder")]
        static void ShowBuilder()
        {
            Builder builder = GetWindow<Builder>("Finite State Machine Builder");
            builder.minSize = new Vector2(800, 600);
        }

        private void OnGUI()
        {
            Event e = Event.current;
            mousePosition = e.mousePosition;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Clear"))
            {
                windows.Clear();
            }

            if (GUILayout.Button("Save"))
            {
                SaveAsPreset();
            }

            if (GUILayout.Button("Open"))
            {
                OpenPreset();
            }

            if (GUILayout.Button("Apply"))
            {
                ApplyToTargetObject();
            }

            targetObject = (Enemy)EditorGUILayout.ObjectField("", targetObject, typeof(Enemy), true);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginScrollView(new Vector2(0, 0));

            UserInput(e);
            DrawWindows();

            GUILayout.EndScrollView();
        }

        private void OnEnable()
        {

        }

        void UserInput(Event e)
        {
            if (e.button == 1 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    RightClick(e);
                }
            }

            if (e.button == 0 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    //
                    // @Todo: Left Click
                    //
                }
            }
        }

        void RightClick(Event e)
        {
            selectedNode = null;
            clickedOnWindow = false;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(e.mousePosition))
                {
                    selectedNode = windows[i];
                    clickedOnWindow = true;
                    break;
                }
            }

            if (!clickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyNode(e);
            }
        }

        void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add State/Wait"), false, ContextCallback, UserActions.ADD_WAIT_STATE);
            menu.AddItem(new GUIContent("Add State/Seek"), false, ContextCallback, UserActions.ADD_SEEK_STATE);
            menu.AddItem(new GUIContent("Add Comment"), false, ContextCallback, UserActions.COMMENT_NODE);
            menu.ShowAsContext();
            e.Use();
        }

        void ModifyNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            if (selectedNode is StateNode)
            {
                menu.AddItem(new GUIContent("Add Transition"), false, ContextCallback, UserActions.MAKE_TRANSITION_START);
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.DELETE_NODE);
            }
            else if (selectedNode is CommentNode)
            {
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.DELETE_NODE);
            }

            menu.ShowAsContext();
            e.Use();
        }

        void ContextCallback(object o)
        {
            UserActions a = (UserActions)o;

            switch (a)
            {
                case UserActions.ADD_WAIT_STATE:
                {
                    StateNode stateNode = CreateInstance<WaitStateNode>();
                    stateNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 75);
                    stateNode.windowTitle = "Wait";
                    windows.Add(stateNode);
                    break;
                }

                case UserActions.ADD_SEEK_STATE:
                {
                    StateNode stateNode = CreateInstance<SeekStateNode>();
                    stateNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 75);
                    stateNode.windowTitle = "Seek";
                    windows.Add(stateNode);
                    break;
                }

                case UserActions.MAKE_TRANSITION_START:
                {
                    if (selectedNode is StateNode from)
                    {
                        // Transition transition = from.AddTransition();
                    }
                    break;
                }

                case UserActions.MAKE_TRANSITION_END:
                {
                    // @Todo:
                    // this is to set the toState for the transition
                    //
                    break;
                }

                case UserActions.COMMENT_NODE:
                {
                    CommentNode commentNode = CreateInstance<CommentNode>();
                    commentNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100);
                    commentNode.windowTitle = "Comment";
                    windows.Add(commentNode);
                    break;
                }

                case UserActions.DELETE_NODE:
                {
                    if (selectedNode != null)
                    {
                        if (selectedNode is StateNode stateNode)
                        {
                            // @Todo: 
                            // Delete child transitions
                            // 
                        }
                        windows.Remove(selectedNode);
                    }
                    break;
                }

                case UserActions.DELETE_TRANSITION:
                    break;

                default:
                    break;
            }
        }

        void DrawWindows()
        {
            BeginWindows();

            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].DrawCurve();
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }

            EndWindows();
        }

        void DrawNodeWindow(int id)
        {
            if (id >= windows.Count)
                return;

            windows[id].DrawWindow();
            GUI.DragWindow();
        }

        public void DrawTransitionLines()
        {
            foreach (var window in windows)
            {
                if (window is TransitionNode node)
                {
                    //* if we go for transitions being a separate window
                    Handles.DrawAAPolyLine(10f, node.fromState.windowRect.position, node.toState.windowRect.position);
                    //*/

                    /* if we go for transitions being nodes 
                    DrawNodeCurve(node.fromState.windowRect, node.windowRect, Color.red, false);
                    DrawNodeCurve(node.windowRect, node.toState.windowRect, Color.red, true);
                    //*/
                }
            }
        }

        private static void DrawNodeCurve(Rect start, Rect end, Color curveColour, bool left)
        {
            Vector3 startPos = new Vector3
            {
                x = left ? start.x + start.width : start.x,
                y = start.y + (start.height * .5f),
                z = 0f
            };

            Vector3 endPos = new Vector3(end.x + (end.width * .5f), end.y + (end.height * .5f), 0f);

            Vector3 startTan = startPos + Vector3.right * 50f;
            Vector3 endTan = endPos + Vector3.left * 50f;

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColour, null, (i + 1) * .5f);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColour, null, 1);

        }

        //public static TransitionNode AddTransitionNode(int index, Transition transition, StateNode from)
        //{
        //    Rect fromRect = from.windowRect;
        //    fromRect.x += 50;
        //    float targetY = fromRect.y - fromRect.height;

        //    if (from.finiteState != null)
        //    {
        //        targetY += (index * 100);
        //    }

        //    fromRect.y = targetY;

        //    TransitionNode transitionNode = CreateInstance<TransitionNode>();
        //    transitionNode.Init(from, transition);
        //    transitionNode.windowRect = new Rect(fromRect.x + 200 + 100, fromRect.y + (fromRect.y * .7f), 200, 80);
        //    transitionNode.windowTitle = "Condition Check";
        //    windows.Add(transitionNode);
        //    return transitionNode;
        //}

        public static void ApplyToTargetObject()
        {
            if (targetObject == null)
                return;

            // remove old stuff
            while (true)
            {
                FiniteState state = targetObject.gameObject.GetComponent<FiniteState>();
                if (state == null)
                    break;
                DestroyImmediate(state);
            }

            while (true)
            {
                Condition condition = targetObject.gameObject.GetComponent<Condition>();
                if (condition == null)
                    break;
                DestroyImmediate(condition);
            }
            
            // add new stuff
            var stateMapping = new Dictionary<BaseNode, FiniteState>();
            var newStates = new List<FiniteState>();

            // add states 
            foreach (var node in windows)
            {
                if (node is WaitStateNode waitNode)
                {
                    var state = targetObject.gameObject.AddComponent<WaitState>();
                    state.waitDuration = waitNode.duration;

                    stateMapping.Add(node, state);
                    newStates.Add(state);
                }
                else if (node is SeekStateNode seekNode)
                {
                    var state = targetObject.gameObject.AddComponent<SeekState>();
                    state.speed = seekNode.speed;
                    state.targetTransform = seekNode.targetTransform;

                    stateMapping.Add(node, state);
                    newStates.Add(state);
                }
            }

            // add transitions and conditions
            foreach (var node in windows)
            {
                if (node is TransitionNode transNode)
                {
                    var fromState = stateMapping[transNode.fromState];
                    // var toState = stateMapping[transNode.toState];

                    Transition transition = new Transition();
                    fromState.transitions.Add(transition);

                    foreach (var condition in transNode.conditions)
                    {
                        switch (condition.type)
                        {
                            case ConditionType.PLAYER_PROXIMITY:
                                var playerProximityCondition = targetObject.gameObject.AddComponent<PlayerProximityCondition>();
                                var playerProximityNode = condition as PlayerProximityConditionNode;

                                playerProximityCondition.not = playerProximityNode.not;
                                playerProximityCondition.distance = playerProximityNode.distance;
                                playerProximityCondition.targetTransform = playerProximityNode.targetTransform;

                                transition.conditions.Add(playerProximityCondition);
                                break;

                            case ConditionType.PROJECTILE_THREAT:
                                var projectileThreatCondition = targetObject.gameObject.AddComponent<ProjectileThreatCondition>();
                                var projectileThreatNode = condition as ProjectileThreatConditionNode;

                                projectileThreatCondition.not = projectileThreatNode.not;
                                projectileThreatCondition.distance = projectileThreatNode.distance;
                                projectileThreatCondition.parentTransform = projectileThreatNode.parentTransform;

                                break;

                            case ConditionType.STATE_COMPLETE:
                                var stateCompleteCondition = targetObject.gameObject.AddComponent<StateCompleteCondition>();
                                var stateCompleteNode = condition as StateCompleteConditionNode;

                                stateCompleteCondition.not = stateCompleteNode.not;
                                stateCompleteCondition.state = stateMapping[stateCompleteNode.state];

                                break;
                        }
                    }
                }
            }

            // link up with targetObject's FiniteStateMachine
            targetObject.stateMachine.states = newStates.ToArray();
        }

        public static void SaveAsPreset()
        {
            var asset = CreateInstance<FSMAsset>();
            var stateIndexes = new Dictionary<StateNode, int>();
            
            // add all states and comments
            foreach (var window in windows)
            {
                if (window is StateNode state)
                {
                    if (state is WaitStateNode waitState)
                    {
                        FSMAsset.State stateAsset = new FSMAsset.State
                        {
                            type = StateType.WAIT,
                            windowRect = waitState.windowRect,
                            duration = waitState.duration
                        };
                        stateIndexes.Add(waitState, asset.states.Count);
                        asset.states.Add(stateAsset);

                    }
                    else if (state is SeekStateNode seekState)
                    {
                        FSMAsset.State stateAsset = new FSMAsset.State
                        {
                            type = StateType.SEEK,
                            windowRect = seekState.windowRect,
                            speed = seekState.speed,
                            targetTransform = seekState.targetTransform
                        };
                        stateIndexes.Add(seekState, asset.states.Count);
                        asset.states.Add(stateAsset);
                    }
                }
                else if (window is CommentNode comment)
                {
                    FSMAsset.Comment commentAsset = new FSMAsset.Comment
                    {
                        windowRect = comment.windowRect,
                        text = comment.comment
                    };
                    asset.comments.Add(commentAsset);
                }
            }

            // add all transitions with conditions
            foreach (var window in windows)
            {
                if (window is TransitionNode transition)
                {
                    FSMAsset.Transition transitionAsset = new FSMAsset.Transition
                    {
                        fromState = stateIndexes[transition.fromState],
                        toState = stateIndexes[transition.toState]
                    };

                    // add conditions
                    foreach (var condition in transition.conditions)
                    {
                        FSMAsset.Condition conditionAsset = new FSMAsset.Condition
                        {
                            type = condition.type,
                            not = condition.not
                        };

                        switch (condition.type)
                        {
                            case ConditionType.PLAYER_PROXIMITY:
                            {
                                var derived = condition as PlayerProximityConditionNode;
                                conditionAsset.distance = derived.distance;
                                conditionAsset.targetTransform = derived.targetTransform;
                                break;
                            }

                            case ConditionType.PROJECTILE_THREAT:
                            {
                                var derived = condition as ProjectileThreatConditionNode;
                                conditionAsset.distance = derived.distance;
                                conditionAsset.parentTransform = derived.parentTransform;
                                break;
                            }

                            case ConditionType.STATE_COMPLETE:
                            {
                                var derived = condition as StateCompleteConditionNode;
                                conditionAsset.state = stateIndexes[derived.state];
                                break;
                            }
                        }

                        transitionAsset.conditions.Add(conditionAsset);
                    }
                }
            }

            // save asset
            string assetPath = EditorUtility.SaveFilePanelInProject("Save FSM Asset", "FSMAsset.asset", "asset", "");
            if (assetPath.Length != 0)
            {
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        public static void OpenPreset()
        {
            // remove current windows
            windows.Clear();

            string assetPath = EditorUtility.OpenFilePanel("Open Asset", Application.dataPath, "asset");

            if (assetPath.Length != 0)
            {
                // make assetPath relative to project path
                if (assetPath.StartsWith(Application.dataPath))
                {
                    assetPath = "Assets" + assetPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    Debug.LogErrorFormat("Couldn't load asset at path: {0}", assetPath);
                    return;
                }

                FSMAsset asset = AssetDatabase.LoadAssetAtPath<FSMAsset>(assetPath);

                if (asset)
                {
                    LoadPreset(asset);
                }
                else
                {
                    Debug.LogErrorFormat("Couldn't load asset at path: {0}", assetPath);
                }
            }
        }

        private static void LoadPreset(FSMAsset asset)
        {
            // load in states
            foreach (var state in asset.states)
            {
                switch (state.type)
                {
                    case StateType.SEEK:
                    {
                        SeekStateNode node = CreateInstance<SeekStateNode>();
                        node.windowRect = state.windowRect;
                        node.windowTitle = "Seek";
                        node.speed = state.speed;
                        node.targetTransform = state.targetTransform;
                        windows.Add(node);
                        break;
                    }

                    case StateType.WAIT:
                    {
                        WaitStateNode node = CreateInstance<WaitStateNode>();
                        node.windowRect = state.windowRect;
                        node.windowTitle = "Wait";
                        node.duration = state.duration;
                        windows.Add(node);
                        break;
                    }

                    default:
                        Debug.LogErrorFormat("Couldn't load in state of type: {0}", state.type);
                        break;
                }
            }

            // load in transitions and their conditions
            foreach (var transition in asset.transitions)
            {
                TransitionNode node = CreateInstance<TransitionNode>();
                node.windowRect = transition.windowRect;
                node.windowTitle = "Transition";
                node.fromState = windows[transition.fromState] as StateNode;
                node.toState = windows[transition.toState] as StateNode;

                // @Todo:
                // load in conditions
                //

                windows.Add(node);
            }

            // load in comments
            foreach (var comment in asset.comments)
            {
                CommentNode node = CreateInstance<CommentNode>();
                node.windowRect = comment.windowRect;
                node.windowTitle = "Comment";
                node.comment = comment.text;
                windows.Add(node);
            }
        }
    }
}