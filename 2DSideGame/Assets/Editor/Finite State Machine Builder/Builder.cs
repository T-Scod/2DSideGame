using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
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
            ADD_TRANSITION,
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
                menu.AddItem(new GUIContent("Add Transition"), false, ContextCallback, UserActions.ADD_TRANSITION);
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

                case UserActions.ADD_TRANSITION:
                {
                    if (selectedNode is StateNode from)
                    {
                        // Transition transition = from.AddTransition();
                    }
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
                            //
                            // @Todo: Delete child transitions
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
                    //Handles.DrawLine(node.fromState.windowRect.position, node.windowRect.position);
                    //Handles.DrawLine(node.windowRect.position, node.toState.windowRect.position);

                    DrawNodeCurve(node.fromState.windowRect, node.windowRect, Color.red, false);
                    DrawNodeCurve(node.windowRect, node.toState.windowRect, Color.red, true);
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
            Debug.Log("Applying to Target Object");

            if (targetObject == null)
            {
                return;
            }

            // apply shit
        }

        public static void SaveAsPreset()
        {
            FSMAsset asset = CreateInstance<FSMAsset>();

            // add all states and comments
            foreach (var window in windows)
            {
                if (window is StateNode state)
                {
                    if (state is WaitStateNode waitState)
                    {
                        FSMAsset.State stateAsset = new FSMAsset.State
                        {
                            type = FSMAsset.StateType.WAIT,
                            windowRect = waitState.windowRect,
                            duration = waitState.duration
                        };
                        asset.states.Add(stateAsset);

                    }
                    else if (state is SeekStateNode seekState)
                    {
                        FSMAsset.State stateAsset = new FSMAsset.State
                        {
                            type = FSMAsset.StateType.SEEK,
                            windowRect = seekState.windowRect,
                            speed = seekState.speed,
                            targetTransform = seekState.targetTransform
                        };
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
                    FSMAsset.Transition transitionAsset = new FSMAsset.Transition();
                    // add conditions
                    foreach (var condition in transition.conditions)
                    {
                        FSMAsset.Condition conditionAsset = new FSMAsset.Condition
                        {
                            not = condition.not,
                        };
                        transitionAsset.conditions.Add(conditionAsset);
                    }

                    // @Todo:
                    // Implement adding fromState and toState into transition asset
                    //
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
            string assetPath = EditorUtility.OpenFilePanel("Open Asset", "../Assets", "asset");

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
            // load in comments
            foreach (var comment in asset.comments)
            {
                CommentNode node = CreateInstance<CommentNode>();
                node.windowRect = comment.windowRect;
                node.windowTitle = "Comment";
                node.comment = comment.text;
                windows.Add(node);
            }

            // load in states
            foreach (var state in asset.states)
            {
                switch (state.type)
                {
                    case FSMAsset.StateType.SEEK:
                    {
                        SeekStateNode node = CreateInstance<SeekStateNode>();
                        node.windowRect = state.windowRect;
                        node.windowTitle = "Seek";
                        node.speed = state.speed;
                        node.targetTransform = state.targetTransform;
                        windows.Add(node);
                        break;
                    }

                    case FSMAsset.StateType.WAIT:
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

            // @Todo:
            // load in transitions and their conditions
            //
        }
    }
}