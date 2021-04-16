using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_AxisScalingVersion2 : MonoBehaviour
{
    private Mesh _projectionMesh;
    
    [SerializeField] private MeshFilter _projectionMeshFilter;
    [SerializeField] private MeshCollider _projectionColldier;
    
    [SerializeField] private Vector3 _rotation = Vector3.zero;
    [SerializeField] private Vector3 _scale = Vector3.one;


    private void Awake()
    {
        _projectionMesh = _projectionMeshFilter.mesh;
    }


    void FixedUpdate()
    {
        Matrix4x4 transformationMatrix = Matrix4x4.identity;

        transformationMatrix *= Matrix4x4.Rotate(Quaternion.Euler(_rotation));
        transformationMatrix *= Matrix4x4.Scale(_scale);
        transformationMatrix *= Matrix4x4.Rotate(Quaternion.Inverse(Quaternion.Euler(_rotation)));
        
        Mesh projectionMesh = Mesh.Instantiate(_projectionMesh);
        
        List<Vector3> vertexList = new List<Vector3>();
        projectionMesh.GetVertices(vertexList);
        
        for (int i = 0; i < vertexList.Count; i++)
        {
            vertexList[i] = transformationMatrix * vertexList[i];
        }
        
        projectionMesh.SetVertices(vertexList);

        _projectionMeshFilter.mesh = projectionMesh;
        _projectionColldier.sharedMesh = projectionMesh;
    }
}
