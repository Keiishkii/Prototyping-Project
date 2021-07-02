using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

#if UNITY_EDITOR
using UnityEditor;
#endif

public struct TransformData
{
    public float3 position;
    public quaternion rotation;
    public float3 scale;
}

[BurstCompile]
public struct RecordTransforms : IJobParallelForTransform
{
    [WriteOnly] public NativeArray<TransformData> dataArray;

    public void Execute(int index, TransformAccess transform)
    {
        TransformData data = new TransformData()
        {
            position = transform.position,
            rotation = transform.rotation
        };
        
        dataArray[index] = data;
    }
}

[BurstCompile]
public struct WriteTransforms : IJobParallelForTransform
{
    [ReadOnly] public NativeArray<TransformData> dataArray;

    public void Execute(int index, TransformAccess transform)
    {
        TransformData data = dataArray[index];
        
        transform.position = data.position;
        transform.rotation = data.rotation;
    }
}

public class DataCollectorTest : MonoBehaviour
{
    [SerializeField] private Transform[] modelRootBones = new Transform[0];
    
    private Transform[] _modelBoneHierarchies = new Transform[0];
    private TransformAccessArray _transformAccessArray;
    
    private TransformData[][] _bonePositionList = new TransformData[0][];

    private RecordTransforms _recordBoneTransformsJob;
    private IEnumerator _record;
    private WriteTransforms _transformPlaybackJob;
    private IEnumerator _play;
    
    
    
    
    private void Awake()
    {
        foreach (Transform modelRoot in modelRootBones)
        {
            GetListOfHierarchy(ref _modelBoneHierarchies, modelRoot);
        }
        
        _transformAccessArray = new TransformAccessArray(_modelBoneHierarchies);
    }

    private void OnDestroy()
    {
        _transformAccessArray.Dispose();
    }

    private static void GetListOfHierarchy(ref Transform[] transforms, Transform parent)
    {
        AddToArray(ref transforms, parent);
        for (int i = 0; i < parent.childCount; i++)
        {
            GetListOfHierarchy(ref transforms, parent.GetChild(i));
        }
    }
    
    
    
    public void StartRecording()
    {
        if (_play != null) StopCoroutine(_play);
        if (_record != null) StopCoroutine(_record);

        _record = RecordTransforms();
        StartCoroutine(_record);
    }
    
    public void StopRecording()
    {
        StopCoroutine(_record);
    }
    
    private IEnumerator RecordTransforms()
    {
        _bonePositionList = new TransformData[0][];
        
        while (Application.isPlaying)
        {
            //Stopwatch recordingStopwatch = Stopwatch.StartNew();
            
            NativeArray<TransformData> transformData = new NativeArray<TransformData>(_transformAccessArray.length, Allocator.TempJob);
            
            _recordBoneTransformsJob = new RecordTransforms() {dataArray = transformData};

            //Debug.Log($"Recording Delay (Creating Job)(Milliseconds): {recordingStopwatch.Elapsed.TotalMilliseconds}");
            //recordingStopwatch.Restart();
            
            JobHandle recordingJobHandle = _recordBoneTransformsJob.Schedule(_transformAccessArray);
            recordingJobHandle.Complete();
            
            //Debug.Log($"Recording Delay (Job)(Milliseconds): {recordingStopwatch.Elapsed.TotalMilliseconds}");
            //recordingStopwatch.Restart();

            AddToArray(ref _bonePositionList, transformData.ToArray());
            
            transformData.Dispose();
            
            yield return null;   
        }
    }

    private static void AddToArray<T>(ref T[] collection, T content)
    {
        int collectionLength = collection.Length;
        T[] tempCollection = new T[collectionLength + 1];

        for (int i = 0; i < collectionLength; i++)
        {
            tempCollection[i] = collection[i];
        }

        tempCollection[collectionLength] = content;
        collection = tempCollection;
    }
    
    
    
    public void StartPlayback()
    {
        if (_play != null) StopCoroutine(_play);
        if (_record != null) StopCoroutine(_record);
        
        _play = PlaybackTransforms();
        StartCoroutine(_play);
    }
    
    private IEnumerator PlaybackTransforms()
    {
        if (_bonePositionList.Length > 0) UnityEngine.Debug.Log($"Bones Captured: {_bonePositionList[0].Length}");
        
        int index = 0;
        while (Application.isPlaying && index < _bonePositionList.Length)
        {
            //Stopwatch playbackStopwatch = Stopwatch.StartNew();
            
            NativeArray<TransformData> transformData = new NativeArray<TransformData>(_bonePositionList[index], Allocator.TempJob);
            
            _transformPlaybackJob = new WriteTransforms() {dataArray = transformData};

            //Debug.Log($"Playback Delay (Creating Job)(Milliseconds): {playbackStopwatch.Elapsed.TotalMilliseconds}");
            //playbackStopwatch.Restart();
            
            JobHandle playbackJobHandle = _transformPlaybackJob.Schedule(_transformAccessArray);
            playbackJobHandle.Complete();

            //UnityEngine.Debug.Log($"Playback Delay (Job)(Milliseconds): {playbackStopwatch.Elapsed.TotalMilliseconds}");
            
            transformData.Dispose();

            
            yield return null;   
            index++;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DataCollectorTest))]
public class DataCollectorTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DataCollectorTest targetScript = (DataCollectorTest) target;

        if (GUILayout.Button("Start Recording"))
        {
            targetScript.StartRecording();
        }
        else if (GUILayout.Button("Stop Recording"))
        {
            targetScript.StopRecording();
        }
        else if (GUILayout.Button("Start Playback"))
        {
            targetScript.StartPlayback();
        }
    }
}
#endif