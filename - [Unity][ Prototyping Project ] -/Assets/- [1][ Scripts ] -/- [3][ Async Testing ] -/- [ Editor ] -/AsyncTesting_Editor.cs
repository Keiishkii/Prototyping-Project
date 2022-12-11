using System.Collections;
using System.Collections.Generic;
using KeiishkiiLib;
using static KeiishkiiLib.EditorUtility;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AsyncTesting))]
public class AsyncTesting_Editor : CustomEditor<AsyncTesting>
{
    protected override void OnInspectorRender()
    {
        Button("Delay Call Test:", false, AsyncTesting.TestDelayAsync);
        
        Button("Coroutine Comparison:", false, () =>
        {
            _targetScript.StartCoroutine(AsyncTesting.LoopCoroutine());
            AsyncTesting.LoopAsync();
        });
        
        Button("Halt Test:", false, () =>
        {
            AsyncTesting.HaltTest().Wait(10000);
        });
    }
}
