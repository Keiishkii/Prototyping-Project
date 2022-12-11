using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class SpriteLineRenderer : CustomLineRenderer
{
    [SerializeField] private float _ringMotionSpeed;
    [SerializeField] private float _ringDisplacement;
    
    [SerializeField] private float _wavePropagationSpeed;
    [SerializeField] private float _waveDisplacement;
    
    [SerializeField] private float _minimumWaveSize;
    [SerializeField] private float _maximumWaveSize;

    private MeshFilter _meshFilter;


    
    
    
    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    
    
    private void Update()
    {
        SpriteLineRendererMeshGenerator.DisplacementFromStartUntilRingSizeFull = 1;
        
        SpriteLineRendererMeshGenerator.RingDisplacement = _ringDisplacement;
        SpriteLineRendererMeshGenerator.RingMotionSpeed = _ringMotionSpeed;
        
        SpriteLineRendererMeshGenerator.WaveDisplacement = _waveDisplacement;
        SpriteLineRendererMeshGenerator.WavePropagationSpeed = _wavePropagationSpeed;
        
        SpriteLineRendererMeshGenerator.MinWaveSize = _minimumWaveSize;
        SpriteLineRendererMeshGenerator.MaxWaveSize = _maximumWaveSize;
        
        
        
        _meshFilter.mesh = SpriteLineRendererMeshGenerator.GenerateLineMesh(points);
    }
    
    /*
    private void OnDrawGizmos()
    {
        float growthDistance = 1;
        
        float distance = 0;
        for (int i = 1; i < meshPoints.Count; i++)
        {
            distance += Vector3.Distance(meshPoints[i - 1], meshPoints[i]);
            
            float iLerp = Mathf.InverseLerp(0, growthDistance, Mathf.Clamp01(distance));
            float radiusMultiplier = Mathf.Lerp(0, 1, iLerp);

            float waveSizeMultiplier = Mathf.Lerp(0.75f, 1.25f, (Mathf.Sin(((_wavePropagationSpeed * Time.timeSinceLevelLoad) + (distance * _waveDisplacement)) * (Mathf.PI * 2f)) / 2f) + 0.5f);
            
            
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(meshPoints[i], _radius * radiusMultiplier * waveSizeMultiplier);
        }
    }
    */
}

public static class SpriteLineRendererMeshGenerator
{
    public static float RingMotionSpeed;
    public static float RingDisplacement;

    public static float DisplacementFromStartUntilRingSizeFull;
    
    public static float WavePropagationSpeed;
    public static float WaveDisplacement;
    
    public static float MinWaveSize;
    public static float MaxWaveSize;





    public static List<Vector3> GenerateMeshPoints(in List<Vector3> inputPositions)
    {
        List<Vector3> outputList = new List<Vector3>();
        if (inputPositions.Count == 0 || RingDisplacement < 0.01f) return outputList;

        outputList.Add(inputPositions[0]);

        float displacementElapsed = (Time.timeSinceLevelLoad * RingMotionSpeed) % RingDisplacement;
        Vector3 displacementOrigin = inputPositions[0];
        for (int i = 1; i < inputPositions.Count; i++)
        {
            Vector3 nextInputPoint = inputPositions[i];
            while (Vector3.SqrMagnitude(displacementOrigin - nextInputPoint) > Mathf.Pow(RingDisplacement - displacementElapsed, 2f))
            {
                Vector3 position = displacementOrigin + Vector3.Normalize(nextInputPoint - displacementOrigin) * (RingDisplacement - displacementElapsed);
                outputList.Add(position);

                displacementOrigin = position;
                displacementElapsed = 0;
            }

            displacementElapsed += Vector3.Distance(displacementOrigin, nextInputPoint);
            displacementOrigin = nextInputPoint;
        }

        return outputList;
    }



    public static Mesh GenerateLineMesh(in List<Vector3> inputPositions)
    {
        List<Vector3> meshPointList = GenerateMeshPoints(inputPositions);
        if (meshPointList.Count == 0) return null;

        List<int> triangleList = new List<int>();
        List<Vector2> uvList = new List<Vector2>();
        List<Vector3> vertexList = new List<Vector3>();
        List<Vector3> normalList = new List<Vector3>();
        
        

        float totalDistance = 0;
        for (int i = 1; i < meshPointList.Count; i++)
        {
            totalDistance += Vector3.Distance(meshPointList[i - 1], meshPointList[i]);
            
            float radiusMultiplier = Mathf.InverseLerp(0, DisplacementFromStartUntilRingSizeFull, Mathf.Clamp01(totalDistance));
            float waveSizeMultiplier = Mathf.Lerp(MinWaveSize, MaxWaveSize, (Mathf.Sin(((WavePropagationSpeed * Time.timeSinceLevelLoad) + (totalDistance * WaveDisplacement)) * (Mathf.PI * 2f)) / 2f) + 0.5f);

            float ringScale = radiusMultiplier * waveSizeMultiplier;

            GenerateTexturePlane(meshPointList, i, triangleList, uvList, vertexList, normalList, ringScale);
        }



        Mesh mesh = new Mesh()
        {
            vertices = vertexList.ToArray(),
            normals = normalList.ToArray(),
            triangles = triangleList.ToArray(),
            uv = uvList.ToArray()
        };

        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    private static void GenerateTexturePlane(in List<Vector3> inputLine, in int index, in List<int> triangleList, in List<Vector2> uvList, in List<Vector3> vertexList, in List<Vector3> normalList, in float ringScale)
    {
        Vector3 position = inputLine[index];
        Vector3 scale = Vector3.one * ringScale;
        Vector3 direction = (index < inputLine.Count - 1)
            ? Vector3.Normalize(inputLine[index] - inputLine[index - 1]) / 2f + Vector3.Normalize(inputLine[index + 1] - inputLine[index]) / 2f
            : Vector3.Normalize(inputLine[index] - inputLine[index - 1]);

        Matrix4x4 transformationMatrix = Matrix4x4.TRS(position, Quaternion.LookRotation(direction), scale);
        List<Vector3> vertices = new List<Vector3>()
        {
            new(0.5f, -0.5f),
            new(0.5f, 0.5f),
            new(-0.5f, -0.5f),
            new(-0.5f, 0.5f)
        };

        
        
        // Adding vertices and normals to Mesh:
        int subMeshStartingIndex = vertexList.Count;
        foreach (Vector3 vertex in vertices)
        {
            Vector4 vertexAsVector4 = vertex;
            vertexAsVector4.w = 1;
            
            vertexList.Add(transformationMatrix * vertexAsVector4);
            normalList.Add(direction);
        }

        // Adding UV's to Mesh:
        uvList.Add(new Vector2(1, 0));
        uvList.Add(new Vector2(1, 1));
        uvList.Add(new Vector2(0, 0));
        uvList.Add(new Vector2(0, 1));
        
        
        // Adding Triangle 1 to Mesh:
        triangleList.Add(subMeshStartingIndex + 0);
        triangleList.Add(subMeshStartingIndex + 1);
        triangleList.Add(subMeshStartingIndex + 2);

        // Adding Triangle 2 to Mesh:
        triangleList.Add(subMeshStartingIndex + 1);
        triangleList.Add(subMeshStartingIndex + 3);
        triangleList.Add(subMeshStartingIndex + 2);
    }
}