using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_SpeedTestCouroutine : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        while (Application.isPlaying)
        {
            for (int i = 0; i < Prototype_SpeedTest.iterations; i++)
            {
                Prototype_SpeedTest.TestFunction();
            }

            yield return null;
        }
    }
}
