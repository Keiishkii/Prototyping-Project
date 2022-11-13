using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

[BurstCompile]
public struct CalculationJob : IJobParallelFor
{
    public NativeArray<float> inputOne;
    public NativeArray<float> inputTwo;
    public NativeArray<float> result;

    public void Execute(int index)
    {
        float3 start = new float3(inputOne[index], inputOne[index], inputOne[index]);
        float3 controlPointOne = new float3(inputOne[index], inputOne[index], inputOne[index]);
        float3 controlPointTwo = new float3(inputOne[index], inputOne[index], inputOne[index]);
        float3 controlPointThree = new float3(inputOne[index], inputOne[index], inputOne[index]);
        float3 controlPointFour = new float3(inputOne[index], inputOne[index], inputOne[index]);
        float3 controlPointFive = new float3(inputOne[index], inputOne[index], inputOne[index]);
        float3 end = new float3(inputOne[index], inputOne[index], inputOne[index]);

        float weightSubtraction = (1 - inputTwo[index]);

        float3 position =
            ((math.pow(weightSubtraction, 6)) * start) +
            ((6 * math.pow(weightSubtraction, 5) * math.pow(inputTwo[index], 1)) * controlPointOne) +
            ((15 * math.pow(weightSubtraction, 4) * math.pow(inputTwo[index], 2)) * controlPointTwo) +
            ((20 * math.pow(weightSubtraction, 3) * math.pow(inputTwo[index], 3)) * controlPointThree) +
            ((15 * math.pow(weightSubtraction, 2) * math.pow(inputTwo[index], 4)) * controlPointFour) +
            ((6 * math.pow(weightSubtraction, 1) * math.pow(inputTwo[index], 5)) * controlPointFive) +
            ((math.pow(inputTwo[index], 6)) * end);

        float3 position2 =
            ((math.pow(weightSubtraction, 6)) * start) +
            ((6 * math.pow(weightSubtraction, 5) * math.pow(inputTwo[index], 1)) * controlPointOne) +
            ((15 * math.pow(weightSubtraction, 4) * math.pow(inputTwo[index], 2)) * controlPointTwo) +
            ((20 * math.pow(weightSubtraction, 3) * math.pow(inputTwo[index], 3)) * controlPointThree) +
            ((15 * math.pow(weightSubtraction, 2) * math.pow(inputTwo[index], 4)) * controlPointFour) +
            ((6 * math.pow(weightSubtraction, 1) * math.pow(inputTwo[index], 5)) * controlPointFive) +
            ((math.pow(inputTwo[index], 6)) * end);

        float3 position3 =
            ((math.pow(weightSubtraction, 6)) * start) +
            ((6 * math.pow(weightSubtraction, 5) * Mathf.Pow(inputTwo[index], 1)) * controlPointOne) +
            ((15 * math.pow(weightSubtraction, 4) * Mathf.Pow(inputTwo[index], 2)) * controlPointTwo) +
            ((20 * math.pow(weightSubtraction, 3) * Mathf.Pow(inputTwo[index], 3)) * controlPointThree) +
            ((15 * math.pow(weightSubtraction, 2) * Mathf.Pow(inputTwo[index], 4)) * controlPointFour) +
            ((6 * math.pow(weightSubtraction, 1) * Mathf.Pow(inputTwo[index], 5)) * controlPointFive) +
            ((math.pow(inputTwo[index], 6)) * end);

        result[index] = math.distance(new float3(0, 0, 0), position + position2 + position3);
    }
}

public class Prototype_UnityJobs : MonoBehaviour
{
    public bool testing = true;
    public int testLength = 100;

    private int samplingDelay = 0;



    private void Awake()
    {
        samplingDelay = Application.targetFrameRate * 2;
        //StartCoroutine(TimeMainThread());
        StartCoroutine(TimeJobs());
    }




    private int mainThreadLoopsSampled;
    private float mainThreadTimePassed;
    public IEnumerator TimeMainThread()
    {
        while (testing)
        {
            List<Vector3> start = new List<Vector3>();
            List<Vector3> controlPointOne = new List<Vector3>();
            List<Vector3> controlPointTwo = new List<Vector3>();
            List<Vector3> controlPointThree = new List<Vector3>();
            List<Vector3> controlPointFour = new List<Vector3>();
            List<Vector3> controlPointFive = new List<Vector3>();
            List<Vector3> end = new List<Vector3>();

            float weight = 0.5f;
            List<float> resultList = new List<float>();
            for (int i = 0; i < testLength; i++)
            {
                start.Add(new Vector3(i, i, i));
                controlPointOne.Add(new Vector3(i, i, i));
                controlPointTwo.Add(new Vector3(i, i, i));
                controlPointThree.Add(new Vector3(i, i, i));
                controlPointFour.Add(new Vector3(i, i, i));
                controlPointFive.Add(new Vector3(i, i, i));
                end.Add(new Vector3(i, i, i));
            }


            float startTime = Time.realtimeSinceStartup;
            for (int i = 0; i < testLength; i++)
            {
                float weightSubtraction = (1 - weight);

                Vector3 position =
                    ((Mathf.Pow(weightSubtraction, 6)) * start[i]) +
                    ((6 * Mathf.Pow(weightSubtraction, 5) * Mathf.Pow(weight, 1)) * controlPointOne[i]) +
                    ((15 * Mathf.Pow(weightSubtraction, 4) * Mathf.Pow(weight, 2)) * controlPointTwo[i]) +
                    ((20 * Mathf.Pow(weightSubtraction, 3) * Mathf.Pow(weight, 3)) * controlPointThree[i]) +
                    ((15 * Mathf.Pow(weightSubtraction, 2) * Mathf.Pow(weight, 4)) * controlPointFour[i]) +
                    ((6 * Mathf.Pow(weightSubtraction, 1) * Mathf.Pow(weight, 5)) * controlPointFive[i]) +
                    ((Mathf.Pow(weight, 6)) * end[i]);

                Vector3 position2 =
                    ((Mathf.Pow(weightSubtraction, 6)) * start[i]) +
                    ((6 * Mathf.Pow(weightSubtraction, 5) * Mathf.Pow(weight, 1)) * controlPointOne[i]) +
                    ((15 * Mathf.Pow(weightSubtraction, 4) * Mathf.Pow(weight, 2)) * controlPointTwo[i]) +
                    ((20 * Mathf.Pow(weightSubtraction, 3) * Mathf.Pow(weight, 3)) * controlPointThree[i]) +
                    ((15 * Mathf.Pow(weightSubtraction, 2) * Mathf.Pow(weight, 4)) * controlPointFour[i]) +
                    ((6 * Mathf.Pow(weightSubtraction, 1) * Mathf.Pow(weight, 5)) * controlPointFive[i]) +
                    ((Mathf.Pow(weight, 6)) * end[i]);

                Vector3 position3 =
                    ((Mathf.Pow(weightSubtraction, 6)) * start[i]) +
                    ((6 * Mathf.Pow(weightSubtraction, 5) * Mathf.Pow(weight, 1)) * controlPointOne[i]) +
                    ((15 * Mathf.Pow(weightSubtraction, 4) * Mathf.Pow(weight, 2)) * controlPointTwo[i]) +
                    ((20 * Mathf.Pow(weightSubtraction, 3) * Mathf.Pow(weight, 3)) * controlPointThree[i]) +
                    ((15 * Mathf.Pow(weightSubtraction, 2) * Mathf.Pow(weight, 4)) * controlPointFour[i]) +
                    ((6 * Mathf.Pow(weightSubtraction, 1) * Mathf.Pow(weight, 5)) * controlPointFive[i]) +
                    ((Mathf.Pow(weight, 6)) * end[i]);

                resultList.Add(Vector3.Magnitude(position + position2 + position3));
            }

            float endTime = Time.realtimeSinceStartup;

            mainThreadTimePassed += endTime - startTime;
            mainThreadLoopsSampled++;

            Debug.Log($"MAIN: {1 / endTime - startTime} - Target: {Application.targetFrameRate}");
            Debug.Log("List Length: " + resultList.Count);
            yield return new WaitForSeconds(0.25f);
        }

        yield return null;
    }




    private int jobLoopsSampled;
    private float jobTimePassed;
    public IEnumerator TimeJobs()
    {
        while (testing)
        {
            NativeArray<float> jobInputOne = new NativeArray<float>(testLength, Allocator.TempJob);
            NativeArray<float> jobInputTwo = new NativeArray<float>(testLength, Allocator.TempJob);
            NativeArray<float> jobResult = new NativeArray<float>(testLength, Allocator.TempJob);

            for (int i = 0; i < testLength; i++)
            {
                jobInputOne[i] = i;
                jobInputTwo[i] = 0.5f;
            }

            CalculationJob calculationJob = new CalculationJob();
            calculationJob.inputOne = jobInputOne;
            calculationJob.inputTwo = jobInputTwo;
            calculationJob.result = jobResult;

            float startTime = Time.realtimeSinceStartup;
            JobHandle handle = calculationJob.Schedule(jobResult.Length, 1);
            handle.Complete();
            float endTime = Time.realtimeSinceStartup;

            List<float> jobResultList = jobResult.ToList();

            jobInputOne.Dispose();
            jobInputTwo.Dispose();
            jobResult.Dispose();


            jobTimePassed += endTime - startTime;
            jobLoopsSampled++;

            Debug.Log($"Jobs: {1 / (endTime - startTime)} - Target: {Application.targetFrameRate}");
            Debug.Log("List Length: " + jobResultList.Count);
            yield return new WaitForSeconds(0.25f);
        }

        yield return null;
    }
}
