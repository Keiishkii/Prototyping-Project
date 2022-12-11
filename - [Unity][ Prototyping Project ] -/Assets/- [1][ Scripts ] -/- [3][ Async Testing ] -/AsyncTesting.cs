using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTesting : MonoBehaviour
{
    public static async void TestDelayAsync()
    {
        Debug.Log("Start");
        
        await Task.Delay(5000);
        
        Debug.Log("End");
    }

    public static async void LoopAsync()
    {
        int frame = 0;
        while (Application.isPlaying)
        {
            Debug.Log($"Async - Frame: {frame++}");
            await Task.Yield();
        }
    }

    public static IEnumerator LoopCoroutine()
    {
        int frame = 0;
        while (Application.isPlaying)
        {
            Debug.Log($"Coroutine - Frame: {frame++}");
            yield return null;
        }
    }

    public static async Task HaltTest()
    {
        await Task.Delay(5000);
    }
}
