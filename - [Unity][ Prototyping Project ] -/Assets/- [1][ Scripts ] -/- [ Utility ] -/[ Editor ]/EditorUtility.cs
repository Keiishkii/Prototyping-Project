#if UNITY_EDITOR
    using System;
    using UnityEditor;
    using UnityEngine;

namespace KeiishkiiLib
{
    public static class EditorUtility
    {
        #region Component
        public static void Separator()
        {
            EditorGUILayout.Separator();
        }
        
        public static void Button(in string buttonName, in bool visibleInEditMode, in Action action)
        {
            if ((visibleInEditMode || Application.isPlaying) && GUILayout.Button(buttonName)) action.Invoke();
        }
        
        public static void Foldout(in string foldoutName, ref bool foldoutState, in Action action)
        {
            foldoutState = EditorGUILayout.Foldout(foldoutState, foldoutName);
            if (foldoutState)
            {
                action.Invoke();
            }
        }
        #endregion
        
        #region Scopes
        public static void HorizontalScope(in Action action)
        {
            using GUILayout.HorizontalScope horizontalScope = new GUILayout.HorizontalScope();
            action.Invoke();
        }
        
        public static void VerticalScope(in Action action)
        {
            using GUILayout.VerticalScope verticalScope = new GUILayout.VerticalScope();
            action.Invoke();
        }
        
        public static void IndentedScope(in Action action)
        {
            EditorGUI.indentLevel++;
            action.Invoke();
            EditorGUI.indentLevel--;
        }
        
        public static bool ChangeCheck(in Action action)
        {
            using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();
            
            action.Invoke();
            return changeCheckScope.changed;
        }
        #endregion
    }
}
#endif