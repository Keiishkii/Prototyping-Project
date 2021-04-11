using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_AxisScaling : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform _projection;
    
    
    
    void Start()
    {
        Matrix4x4 transformationMatrix = _transform.localToWorldMatrix;
        
        _projection.position = transformationMatrix.MultiplyPoint(Vector3.zero);
        _projection.rotation = transformationMatrix.rotation;
        _projection.localScale = transformationMatrix.lossyScale;
    }
}
