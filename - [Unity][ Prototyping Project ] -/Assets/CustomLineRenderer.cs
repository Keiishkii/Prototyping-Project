using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomLineRenderer : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    
    
    
    protected void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            points.Add(new Vector3()
            {
                y = Mathf.Sin(i * 0.1f) * 1f,
                z = i * 0.1f
            });
        }
    }
}
