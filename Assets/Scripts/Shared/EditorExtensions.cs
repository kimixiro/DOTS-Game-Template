#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace DOTSTemplate
{
    public static class EditorExtensions
    {
        public static bool TryUseKeyDown(this Event e, KeyCode keyCode)
        {
            if (e.type == EventType.KeyDown && e.keyCode == keyCode)
            {
                e.Use();
                return true;
            }

            return false;
        }
        
        public static bool TryUseMouseDown(this Event e, int button)
        {
            if (e.type == EventType.MouseDown && e.button == button)
            {
                GUIUtility.hotControl = 0;
                e.Use();
                return true;
            }

            return false;
        }
        
        public static Rect ToCenteredRect(this Vector2 position, float size)
        {
            var rectSize = new Vector2(size, size);
            return new Rect((Vector2) position - rectSize * 0.5f, rectSize);
        }
        
        public static Rect ToCenteredRect(this Vector3 position, float size)
        {
            var rectSize = new Vector2(size, size);
            return new Rect((Vector2) position - rectSize * 0.5f, rectSize);
        }

        public static GuiScope DrawGUI(this Editor editor)
        {
            Handles.BeginGUI();
            return new GuiScope();
        } 
        
        public struct GuiScope : IDisposable
        {
            public void Dispose()
            {
                Handles.EndGUI();
            }
        }
    }
}

#endif