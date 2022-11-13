#if UNITY_EDITOR
    using KeiishkiiLib;
    using UnityEditor;
    using UnityEngine;
    using static KeiishkiiLib.EditorUtility;

[CustomEditor(typeof(OrbitController))]
public class OrbitController_Editor : CustomEditor<OrbitController>
{
    protected override void OnInspectorRender()
    {
        TextLabel("Orbit Controls:");
        
        BooleanField("Look At Origin:", ref _targetScript.rotateToLookAtOrigin);
        HorizontalScope((() =>
        {
            BooleanField("Use Transform as Origin:", ref _targetScript.useTransformAsOrigin);
            if (_targetScript.useTransformAsOrigin)
            {
                ObjectField<Transform>("", ref _targetScript.orbitOriginTransform);
            }
            else
            {
                VectorField("", ref _targetScript.orbitOriginVector);
            }
        }));
        
        Separator();
        
        VectorField("Bounds:", ref _targetScript.orbitBounds);
        VectorField("Orientation:", ref _targetScript.orbitOrientation);
        
        Separator();
        
        FloatField("Orbit Speed:", ref _targetScript.speed);
    }
}
#endif
