using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapGeneratorWindow : EditorWindow
{
    private const string _editorSODirectory = "Assets/- [~][ Editor Windows ] - /";
    private const string _editorSOFilename = "Map Generator Window.asset";
    
    private MapGeneratorWindow_SO _mapGeneratorWindowSO;
        
    
    
    
    
    [MenuItem("Keiishkii/Echo Testing/Map Generator")]
    private static void Initialise()
    {
        MapGeneratorWindow window = GetWindow<MapGeneratorWindow>();
        window.Show();
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        
        if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID($"{_editorSODirectory}{_editorSOFilename}")))
        {
            _mapGeneratorWindowSO = CreateInstance<MapGeneratorWindow_SO>();
            AssetDatabase.CreateAsset(_mapGeneratorWindowSO, $"{_editorSODirectory}{_editorSOFilename}");
        }
        else
        {
            _mapGeneratorWindowSO = AssetDatabase.LoadAssetAtPath<MapGeneratorWindow_SO>($"{_editorSODirectory}{_editorSOFilename}");
        }
    }

    
    

    private void OnGUI()
    {
        EditorGUILayout.LabelField($"{((hasUnsavedChanges) ? ("Has Unsaved Changes") : ("Saved"))}");
        
        if (GUI.changed) {  }
    }

    private void Save()
    {
        
    }
}
