using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    private float startAtLevelLoad;
    private float startAtGameStart;

    [ContextMenu("Perform All Tests")]
    private void TestAll()
    {
        PlayTestTest1();
        PlayTestTest2();
        PlayTestTest3();
    }

    [ContextMenu("Perform Test 1")]
    private void PlayTestTest1()
    {
        startAtLevelLoad = Time.timeSinceLevelLoad;
        startAtGameStart = Time.realtimeSinceStartup;

        StartCoroutine(WaitAndTimeTest1());
    }

    private IEnumerator WaitAndTimeTest1()
    {
        yield return new WaitForSeconds(1);

        float durationSinceLevelLoadStart = Time.timeSinceLevelLoad - startAtLevelLoad;
        float durationSinceGameStartStart = Time.realtimeSinceStartup - startAtGameStart;

        Debug.Log($"Test 1 Durations: {durationSinceLevelLoadStart}, {durationSinceGameStartStart}");
    }



    [ContextMenu("Perform Test 2")]
    private void PlayTestTest2()
    {
        startAtLevelLoad = Time.timeSinceLevelLoad;
        startAtGameStart = Time.realtimeSinceStartup;

        StartCoroutine(WaitAndTimeTest2());
    }

    private IEnumerator WaitAndTimeTest2()
    {
        yield return null;

        float durationSinceLevelLoadStart = Time.timeSinceLevelLoad - startAtLevelLoad;
        float durationSinceGameStartStart = Time.realtimeSinceStartup - startAtGameStart;

        Debug.Log($"Test 2 Durations: {durationSinceLevelLoadStart}, {durationSinceGameStartStart}");
    }



    [ContextMenu("Perform Test 3")]
    private void PlayTestTest3()
    {
        StartCoroutine(WaitAndTimeTest3());
    }

    private IEnumerator WaitAndTimeTest3()
    {
        yield return null;

        startAtLevelLoad = Time.timeSinceLevelLoad;
        startAtGameStart = Time.realtimeSinceStartup;

        yield return new WaitForSeconds(1);

        float durationSinceLevelLoadStart = Time.timeSinceLevelLoad - startAtLevelLoad;
        float durationSinceGameStartStart = Time.realtimeSinceStartup - startAtGameStart;

        Debug.Log($"Test 3 Durations: {durationSinceLevelLoadStart}, {durationSinceGameStartStart}");
    }
}