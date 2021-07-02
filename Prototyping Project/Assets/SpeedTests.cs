using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SpeedTests : MonoBehaviour
{
    [SerializeField] private int iterations;
    [SerializeField] private GameObject _gameObject;

    private void Start()
    {
        List<int> listOne = new List<int>();
        listOne.Add(1);
        listOne.Add(1);
        listOne.Add(1);
        listOne.Add(1);

        object Object = (object) listOne;
        List<int> list = (List<int>)Object;
        
        int count = 0;
        foreach (var variable in list)
        {
            count++;
        }
        
        
        Debug.Log(count);
    }

    void Update()
    {
        //Debug.Log($"Time Elapsed - (Test 1): {Test1()}, (Test 2): {Test2()}");
    }

    private float Test1()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        float testNum = 16.275f;
        for (int i = 0; i < iterations; i++)
        {
            float num = (float)(testNum - Math.Truncate(testNum));
        }
        
        return (float) stopwatch.Elapsed.TotalMilliseconds;
    }

    private float Test2()
    {
        Dictionary<string, int> intDictionary = new Dictionary<string, int>();
        //IEnumerable iterator = intDictionary.Where(WhereFunction);

        new List<int>();
        
        object Object = (object) new List<int>();
        List<int> list = (List<int>)Object;
        
        int count = 0;
        foreach (var variable in list)
        {
            count++;
        }
        
        
        Debug.Log(count);

        return 0;
    }
}
