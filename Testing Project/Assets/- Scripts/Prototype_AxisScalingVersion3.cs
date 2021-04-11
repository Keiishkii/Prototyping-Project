using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_AxisScalingVersion3 : MonoBehaviour
{
    private Mesh _projectionMesh;
    
    [SerializeField] private MeshFilter _projectionMeshFilter;
    [SerializeField] private MeshCollider _projectionColldier;

    [SerializeField] private Transform _armBone;
    
    [SerializeField] private Vector3 _rotation = Vector3.zero;
    [SerializeField] private Vector3 _scale = Vector3.one;


    private void Awake()
    {
        _projectionMesh = _projectionMeshFilter.mesh;
    }


    void FixedUpdate()
    {
        Quaternion armRotation = _armBone.rotation;
        Vector3 armScale = _armBone.localScale;
        
        Matrix4x4 transformationMatrix = Matrix4x4.identity;

        
        transformationMatrix *= Matrix4x4.Rotate(armRotation);
        
        transformationMatrix *= Matrix4x4.Scale(new Vector3((1 / armScale.x), (1 / armScale.y), (1 / armScale.z)));
        
        transformationMatrix *= Matrix4x4.Rotate(Quaternion.Inverse(armRotation));
        
        
        
        Mesh projectionMesh = Mesh.Instantiate(_projectionMesh);
        
        List<Vector3> vertexList = new List<Vector3>();
        projectionMesh.GetVertices(vertexList);
        
        
        for (int i = 0; i < vertexList.Count; i++)
        {
            vertexList[i] = transformationMatrix * vertexList[i];
        }
        
        projectionMesh.SetVertices(vertexList);

        _projectionMeshFilter.mesh = projectionMesh;
        //_projectionColldier.sharedMesh = projectionMesh;
    }
}
