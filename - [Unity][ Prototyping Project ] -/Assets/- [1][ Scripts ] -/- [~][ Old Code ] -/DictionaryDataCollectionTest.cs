using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(DictionaryDataCollectionTest))]
public class DictionaryDataCollectionTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DictionaryDataCollectionTest targetScript = (DictionaryDataCollectionTest) target;
        
        if (GUILayout.Button("Run Test"))
        {
            targetScript.RunTest();
        }
    }
}

public class DictionaryDataCollectionTest : MonoBehaviour
{
    [System.Flags]
    private enum Mask
    {
        Head,
        LeftHand,
        RightHand
    }
    private Mask _mask;

    private int _dataCollectionCount = 100;

    private Dictionary<Mask, List<Vector3>> _collectedData = new Dictionary<Mask, List<Vector3>>();
    private UnityEvent _recordData = new UnityEvent();
    
    
    
    
    
    private void SetEnumFlags()
    {
        _mask = Mask.Head;
    }
    
    
    
    public void RunTest()
    {
        SetEnumFlags();
        
        
        _collectedData = new Dictionary<Mask, List<Vector3>>();
        
        
        if (_mask.HasFlag(Mask.Head))
        {
            _collectedData.Add(Mask.Head, new List<Vector3>());
            _recordData.AddListener(AddHeadData);
        }
        if (_mask.HasFlag(Mask.LeftHand))
        {
            _collectedData.Add(Mask.LeftHand, new List<Vector3>());
            _recordData.AddListener(AddLeftHandData);
        }
        if (_mask.HasFlag(Mask.RightHand))
        {
            _collectedData.Add(Mask.RightHand, new List<Vector3>());
            _recordData.AddListener(AddRightHandData);
        }
        
        
        
        for (int i = 0; i < _dataCollectionCount; i++)
        {
            _recordData.Invoke();
        }
        
        _recordData.RemoveAllListeners();
        Debug.Log($"Dictionary Count: {_collectedData[Mask.Head].Count}");
    }

    
    
    private void AddHeadData() {
        _collectedData[Mask.Head].Add(Vector3.zero); }
    private void AddLeftHandData() {
        _collectedData[Mask.LeftHand].Add(Vector3.zero); }
    private void AddRightHandData() {
        _collectedData[Mask.RightHand].Add(Vector3.zero); }
}
