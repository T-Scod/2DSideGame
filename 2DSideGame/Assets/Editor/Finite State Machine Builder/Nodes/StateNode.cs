﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FSM.Builder
{
    public abstract class StateNode : BaseNode
    {
        public abstract void DrawContents();
        public override void DrawWindow()
        {
            DrawContents();
        }
    }
}