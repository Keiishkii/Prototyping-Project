using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Jobs;

[BurstCompile]
public struct CalculationJob_2 : IJob
{
    public void Execute()
    {
        for (int i = 0; i < 50000; i++)
        {
            float result = math.exp10(i);
        }
    }
}









public class Prototype_UnityJobsV2 : MonoBehaviour
{
    private void Awake()
    {

    }



    private void Update()
    {
        float start = Time.realtimeSinceStartup;

        JobHandle jobHandle = scheduleJob();
        jobHandle.Complete();

        float end = Time.realtimeSinceStartup;
        float duration = end - start;

        Debug.Log($"Duration: {duration * 1000}");
    }


    private JobHandle scheduleJob()
    {
        CalculationJob_2 job = new CalculationJob_2();
        return job.Schedule();
    }
}
