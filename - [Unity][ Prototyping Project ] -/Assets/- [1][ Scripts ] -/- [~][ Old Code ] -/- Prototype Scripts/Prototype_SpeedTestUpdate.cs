using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_SpeedTestUpdate : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Prototype_SpeedTest.iterations; i++)
        {
            Prototype_SpeedTest.TestFunction();
        }
    }
}
