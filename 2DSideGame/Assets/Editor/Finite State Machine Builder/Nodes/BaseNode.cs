using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Builder
{
    public abstract class BaseNode : ScriptableObject
    {
        public Rect windowRect;
        public string windowTitle;
        protected Vector2 scrollPosition;

        public virtual void DrawWindow()
        {

        }

        public virtual void DrawCurve()
        {

        }
    }
}
