using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Debug = UnityEngine.Debug;
using File = System.IO.File;
using Object = UnityEngine.Object;

[CustomEditor(typeof(InternetSpeedDataCollector))]
public class InternetSpeedDataCollectorEditor : Editor
{
    [Serializable]
    private class PingData
    {
        public List<int> pingResults;
        public List<float> timeSinceStart;
        public List<string> timeOfResults;
    }
    
    private TextAsset _textAsset;
    
    
    
    
    
    public override void OnInspectorGUI()
    {
        _textAsset = (TextAsset) EditorGUILayout.ObjectField("Json Data: ", _textAsset, typeof(TextAsset), false);
        
        if (GUILayout.Button("Convert JSON file to CSV"))
        {
            string directory = Application.dataPath + "/Data Collection/";
            string fileName = $"InternetData.csv";

            string fileContents = "";
            
            PingData internetPingData = JsonUtility.FromJson<PingData>(_textAsset.text);


            fileContents += $"TimeOfTest, TimeSinceTestStart, Ping \n";
            for (int i = 0; i < internetPingData.pingResults.Count; i++)
            {
                fileContents += $"{internetPingData.timeOfResults[i]}, {internetPingData.timeSinceStart[i]}, {internetPingData.pingResults[i]} \n";
            }
            
            File.WriteAllText(directory + fileName, fileContents);
        }
    }
}

public class InternetSpeedDataCollector : MonoBehaviour
{
    [Serializable]
    private class PingData
    {
        public List<int> pingResults;
        public List<float> timeSinceStart;
        public List<string> timeOfResults;
    }
    
    private Ping _ping;

    private string _timeOfTestStart;
    private readonly List<int> _pingResults = new List<int>();
    private readonly List<float> _timeSinceStart = new List<float>();
    private readonly List<string> _timeOfResults = new List<string>();
    
    private void OnEnable()
    {
        _ping = new Ping("8.8.8.8");

        _timeOfTestStart = DateTime.Now.ToLongTimeString().Replace(":", "-");
        StartCoroutine(WaitForPingEnd());
    }

    private void OnDisable()
    {
        PingData pingData = new PingData { pingResults = _pingResults, timeSinceStart = _timeSinceStart, timeOfResults = _timeOfResults };
        string jsonData = JsonUtility.ToJson(pingData, true);

        string directory = Application.dataPath + "/Data Collection/";
        string fileName = $"Internet Data At Time {_timeOfTestStart}.json";
        
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        File.WriteAllText(directory + fileName, jsonData);
        
        /*
        for (int i = 0; i < _pingResults.Count; i++)
        {
            Debug.Log ($"Ping {i}: {_pingResults[i]}, at {_timeOfResults[i]}");
        }
        */
    }



    private IEnumerator WaitForPingEnd()
    {
        float timeOfTest = Time.realtimeSinceStartup;
        string timeOfTestString = DateTime.Now.ToLongTimeString();
        
        bool breakCondition = _ping.isDone;
        while (!breakCondition)
        {
            yield return new WaitForSeconds(1f);
            
            breakCondition = _ping.isDone;
        }
        
        _pingResults.Add(_ping.time);
        _timeSinceStart.Add(timeOfTest);
        _timeOfResults.Add(timeOfTestString);
        
        _ping = new Ping("8.8.8.8");
        StartCoroutine(WaitForPingEnd());
    }
    
    
}

