using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    public class Builder : EditorWindow
    {
        static List<BaseNode> windows = new List<BaseNode>();
        Vector3 mousePosition;
        bool makeTransition;
        bool clickedOnWindow;
        BaseNode selectedNode;

        public enum UserActions
        {
            ADD_STATE,
            ADD_TRANSITION,
            DELETE_NODE,
            DELETE_TRANSITION,
            COMMENT_NODE
        }

        [MenuItem("Window/FSM Builder")]
        static void ShowBuilder()
        {
            Builder builder = GetWindow<Builder>();
            builder.minSize = new Vector2(800, 600);
        }

        private void OnGUI()
        {
            Event e = Event.current;
            mousePosition = e.mousePosition;
            UserInput(e);
            DrawWindows();
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
            menu.AddItem(new GUIContent("Add State"), false, ContextCallback, UserActions.ADD_STATE);
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
                case UserActions.ADD_STATE:
                {
                    StateNode stateNode = CreateInstance<StateNode>();
                    stateNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 300);
                    stateNode.windowTitle = "State";
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
            windows[id].DrawWindow();
            GUI.DragWindow();
        }

        public void DrawTransitionLines()
        {
            foreach (var window in windows)
            {
                if (window is TransitionNode node)
                {
                    Handles.DrawLine(node.fromState.windowRect.position, node.windowRect.position);
                    Handles.DrawLine(node.windowRect.position, node.toState.windowRect.position);
                }
            }
        }

        //public static void DrawNodeCurve(Rect start, Rect end, Color curveColour, bool left)
        //{
        //    Vector3 startPos = new Vector3
        //    {
        //        x = left ? start.x + start.width : start.x,
        //        y = start.y + (start.height * .5f),
        //        z = 0f
        //    };

        //    Vector3 endPos = new Vector3(end.x + (end.width * .5f), end.y + (end.height * .5f), 0f);

        //    Vector3 startTan = startPos + Vector3.right * 50f;
        //    Vector3 endTan = endPos + Vector3.left * 50f;

        //    for (int i = 0; i < 3; i++)
        //    {
        //        Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColour, null, (i + 1) * .5f);
        //    }

        //    Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColour, null, 1);

        //}

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
    }
}