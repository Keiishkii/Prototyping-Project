using System.Collections;
using System.Collections.Generic;
using KeiishkiiLib;
using UnityEditor;
using UnityEngine;
using EditorUtility = KeiishkiiLib.EditorUtility;

[CustomEditor(typeof(PhotoCapture))]
public class PhotoCapture_Editor : CustomEditor<PhotoCapture>
{
    protected override void OnInspectorRender()
    {
        EditorUtility.Button($"{((_targetScript.recording) ? ("Stop Capture") : ("Start Capture"))}", false, () =>
        {
            if (!_targetScript.recording)
            {
                _targetScript.recording = true;
                _targetScript.StartCoroutine(_targetScript.Capture());
            }
            else
            {
                _targetScript.recording = false;
            }
        });
    }
}
