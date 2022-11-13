#if UNITY_EDITOR
    using System;
    using UnityEditor;
    using UnityEngine;
    using static UnityEditor.EditorGUILayout;
    using Object = UnityEngine.Object;

namespace KeiishkiiLib
{
    public static class EditorUtility
    {
        #region Component
        // - - - 
            public static void Separator()
            {
                EditorGUILayout.Separator();
            }

            public static void TextLabel(in string text)
            {
                LabelField(text);
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
        // - - - 
        #endregion

        #region Fields
        // - - -
            public static void BooleanField(in string booleanName, ref bool booleanReference)
            {
                booleanReference = Toggle(booleanName, booleanReference);
            }

            public static void FloatField(in string floatName, ref float floatReference)
            {
                floatReference = EditorGUILayout.FloatField(floatName, floatReference);
            }
            
            public static void ObjectField<T>(in string objectName, ref T objectReference) where T : Object
            {
                objectReference = (T) EditorGUILayout.ObjectField(objectName, objectReference, typeof(T));
            }
            
            public static void VectorField(in string vectorName, ref Vector2 vectorReference)
            {
                vectorReference = Vector2Field(vectorName, vectorReference);
            }
            
            public static void VectorField(in string vectorName, ref Vector3 vectorReference)
            {
                vectorReference = Vector3Field(vectorName, vectorReference);
            }
            
            public static void VectorField(in string vectorName, ref Vector4 vectorReference)
            {
                vectorReference = Vector4Field(vectorName, vectorReference);
            }
        // - - - 
        #endregion
        
        #region Scopes
        // - - - 
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
        // - - - 
        #endregion
    }
}
#endif