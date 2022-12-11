using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoLineRenderer : CustomLineRenderer
{
    [SerializeField] private float _radius;


    
    private void OnDrawGizmos()
    {
        foreach (Vector3 position in points)
        {
            Gizmos.DrawSphere(position, _radius);
        }
    }
}
