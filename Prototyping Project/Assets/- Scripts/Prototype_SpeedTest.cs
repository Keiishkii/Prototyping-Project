using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_SpeedTest : MonoBehaviour
{
    public static int iterations = 5000;
    public static int counter;



    public static void TestFunction()
    {
        float num1 = Mathf.Sqrt(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(Mathf.Exp(100) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3) / 123 * 0.34f * 3);
        counter++;
    }




    // Update is called once per frame
    private void Update()
    {
        counter = 0;
    }

    private void LateUpdate()
    {
        Debug.Log($"<b>Counter: {counter}</b>");
    }
}
