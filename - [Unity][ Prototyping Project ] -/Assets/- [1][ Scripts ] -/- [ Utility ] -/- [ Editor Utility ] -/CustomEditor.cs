#if UNITY_EDITOR
    using UnityEditor;
    using static KeiishkiiLib.EditorUtility;

namespace KeiishkiiLib
{
    public abstract class CustomEditor<T> : Editor
    {
        protected T _targetScript;
        private bool _showBaseInspector;
        
        
        
        private void OnEnable()
        {
            if (target is T targetScript)
            {
                _targetScript = targetScript;
            }
        } 
        
        protected abstract void OnInspectorRender();
        
        public override void OnInspectorGUI()
        {
            OnInspectorRender();
            
            Separator();
            Foldout($"{((_showBaseInspector) ? ("Hide") : ("Show"))} {typeof(T).ToString()} Base Inspector:", ref _showBaseInspector, () =>
            {
                base.OnInspectorGUI();
            });
        }
    }
}
#endif