using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SoundSource))]
public class SoundSource_Editor : Editor
{
    private float strength, frequency;
    
    public override void OnInspectorGUI()
    {
        SoundSource targetScript = (SoundSource) target;

        strength = EditorGUILayout.FloatField("Strength: ", strength);
        frequency = EditorGUILayout.FloatField("Frequency: ", frequency);
        
        if (GUILayout.Button("Ping"))
        {
            targetScript.Ping(strength, frequency);
        }
        
        EditorGUILayout.Space();
        
        base.OnInspectorGUI();
    }
}


public class SoundSource : MonoBehaviour
{
    [SerializeField] private Transform _echoNodeFolder;
    private readonly List<EchoNode> _echoNodes = new List<EchoNode>();

    private Transform _transform;
    
    
    private void Awake()
    {
        _transform = transform;
        _echoNodes.Clear();
        
        int childCount = _echoNodeFolder.childCount;
        for (int i = 0; i < childCount; i++)
        {
            _echoNodes.Add(_echoNodeFolder.GetChild(i).GetComponent<EchoNode>());
        }
    }

    

    public void Ping(float strength, float freqeuncy)
    {
        Vector3 position = transform.position;

        if (_echoNodes.Count == 1)
        {
            //Gizmos.DrawLine(position, _echoNodes[0].transform.position);
        }
        if (_echoNodes.Count >= 2)
        {
            int closestIndex = 0, 
                secondClosestIndex = 1;
            
            float 
                closestDistance = Vector3.Distance(position, _echoNodes[closestIndex].transform.position), 
                secondClosestDistance = Vector3.Distance(position, _echoNodes[secondClosestIndex].transform.position);
            
            for (int i = 1; i < _echoNodes.Count; i++)
            {
                float distance = Vector3.Distance(position, _echoNodes[i].transform.position);
                if (distance < secondClosestDistance)
                {
                    if (distance < closestDistance)
                    {
                        secondClosestDistance = closestDistance;
                        secondClosestIndex = closestIndex;
                        
                        closestDistance = distance;
                        closestIndex = i;
                        
                    }
                    else
                    {
                        secondClosestDistance = distance;
                        secondClosestIndex = i;
                    }
                }
            }
            
            //Gizmos.DrawLine(position, _echoNodes[closestIndex].transform.position);
            //Gizmos.DrawLine(position, _echoNodes[secondClosestIndex].transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;

        if (_echoNodes.Count == 1)
        {
            Gizmos.DrawLine(position, _echoNodes[0].transform.position);
        }
        if (_echoNodes.Count >= 2)
        {
            int closestIndex = 0, 
                secondClosestIndex = 1;
            
            float 
                closestDistance = Vector3.Distance(position, _echoNodes[closestIndex].transform.position), 
                secondClosestDistance = Vector3.Distance(position, _echoNodes[secondClosestIndex].transform.position);
            
            for (int i = 1; i < _echoNodes.Count; i++)
            {
                float distance = Vector3.Distance(position, _echoNodes[i].transform.position);
                if (distance < secondClosestDistance)
                {
                    if (distance < closestDistance)
                    {
                        secondClosestDistance = closestDistance;
                        secondClosestIndex = closestIndex;
                        
                        closestDistance = distance;
                        closestIndex = i;
                        
                    }
                    else
                    {
                        secondClosestDistance = distance;
                        secondClosestIndex = i;
                    }
                }
            }
            
            Gizmos.DrawLine(position, _echoNodes[closestIndex].transform.position);
            Gizmos.DrawLine(position, _echoNodes[secondClosestIndex].transform.position);
        }
    }
}
