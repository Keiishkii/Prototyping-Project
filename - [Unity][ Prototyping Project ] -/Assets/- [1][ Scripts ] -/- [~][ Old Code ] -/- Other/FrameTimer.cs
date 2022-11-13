using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameTimer : MonoBehaviour
{
    private int samplingDelay = 0;
    private int framesSampled = 0;
    private float timePassed = 0;



    private void Awake()
    {
        samplingDelay = Application.targetFrameRate * 2;
        StartCoroutine(WriteFramerate());
    }

    private void Update()
    {
        if (samplingDelay < 0)
        {
            framesSampled++;
            timePassed += Time.deltaTime;
        }
        else
        {
            samplingDelay--;
        }
    }

    private IEnumerator WriteFramerate()
    {
        while (Application.isPlaying)
        {
            Debug.ClearDeveloperConsole();
            if (framesSampled > 0) Debug.Log($"<b>Current FPS: {1 / Time.deltaTime}</b>, ~FPS: {framesSampled / timePassed} - Target: {Application.targetFrameRate}");
            yield return new WaitForSeconds(0.25f);
        }
    }
}
