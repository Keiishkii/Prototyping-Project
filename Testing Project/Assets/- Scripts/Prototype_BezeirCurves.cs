using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = System.Drawing.Color;

[RequireComponent(typeof(MeshFilter))]
public class Prototype_BezeirCurves : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private readonly int _vertexCount = 8;
    private readonly float _wireRadius = 0.025f;
    private readonly float _increment = 0.01f;
    
    public Transform lineStart;
    public Transform controlPointOne;
    public Transform controlPointTwo;
    public Transform controlPointThree;
    public Transform controlPointFour;
    public Transform controlPointFive;
    public Transform lineEnd;


    private void Awake()
    {
        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;

        DateTime start = DateTime.Now;
        GenerateMesh();
        DateTime end = DateTime.Now;
        Debug.Log($"Time Taken: {end.Subtract(start).Milliseconds}");
    }

    private void GenerateMesh()
    {
        Vector3 start = lineStart.position;
        Vector3 controlPointOnePosition = controlPointOne.position;
        Vector3 controlPointTwoPosition = controlPointTwo.position;
        Vector3 controlPointThreePosition = controlPointThree.position;
        Vector3 controlPointFourPosition = controlPointFour.position;
        Vector3 controlPointFivePosition = controlPointFive.position;
        Vector3 end = lineEnd.position;
        
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        
        List<List<Vector3>> vertexRings = new List<List<Vector3>>();

        
        
        Vector3 pos = CalculatePositionOnBezierCurve(start, controlPointOnePosition, controlPointTwoPosition, controlPointThreePosition, controlPointFourPosition, controlPointFivePosition, end, 0);
        Vector3 nPos = CalculatePositionOnBezierCurve(start, controlPointOnePosition, controlPointTwoPosition, controlPointThreePosition, controlPointFourPosition, controlPointFivePosition, end, (_increment));
        
        Quaternion previousRotation = Quaternion.LookRotation(pos - nPos, Vector3.up);
        
        for (float i = 0; i < 1; i+=_increment)
        {
            Vector3 centerPosition;
            Quaternion lookRotation;

            if (i + _increment < 1)
            {
                centerPosition = CalculatePositionOnBezierCurve(start, controlPointOnePosition, controlPointTwoPosition, controlPointThreePosition, controlPointFourPosition, controlPointFivePosition, end, i);
                Vector3 nextPosition = CalculatePositionOnBezierCurve(start, controlPointOnePosition, controlPointTwoPosition, controlPointThreePosition, controlPointFourPosition, controlPointFivePosition, end, (i + _increment));
                
                Quaternion positiveLookRotation = Quaternion.LookRotation(nextPosition - centerPosition, Vector3.up);
                Quaternion negativeLookRotation = Quaternion.LookRotation(nextPosition - centerPosition, Vector3.down);

                lookRotation = (Vector3.Dot(previousRotation * Vector3.up, positiveLookRotation * Vector3.up) > 0) ? positiveLookRotation : negativeLookRotation;
                previousRotation = lookRotation;
            }
            else
            {
                centerPosition = CalculatePositionOnBezierCurve(start, controlPointOnePosition, controlPointTwoPosition, controlPointThreePosition, controlPointFourPosition, controlPointFivePosition, end, 1);
                Vector3 previousPosition = CalculatePositionOnBezierCurve(start, controlPointOnePosition, controlPointTwoPosition, controlPointThreePosition, controlPointFourPosition, controlPointFivePosition, end, (1 - _increment));
                
                Quaternion positiveLookRotation = Quaternion.LookRotation(centerPosition - previousPosition, Vector3.up);
                Quaternion negativeLookRotation = Quaternion.LookRotation(centerPosition - previousPosition, Vector3.down);

                lookRotation = (Vector3.Dot(previousRotation * Vector3.up, positiveLookRotation * Vector3.up) >= 0) ? positiveLookRotation : negativeLookRotation;
                previousRotation = lookRotation;
            }
            

            List<Vector3> vertexRing = GenerateRing(centerPosition, lookRotation);
            vertexRings.Add(vertexRing);
            
            for (int vertex = 0; vertex < vertexRing.Count; vertex++)
            {
                vertices.Add(vertexRing[vertex]);
                normals.Add(Vector3.Normalize(vertexRing[vertex] - centerPosition));
            }
        }

        for (int vertexRing = 1; vertexRing < vertexRings.Count; vertexRing++)
        {
            for (int vertex = 0; vertex < _vertexCount; vertex++)
            {
                int vertexIndexA = (vertexRing * _vertexCount) + vertex;
                int vertexIndexB = (vertexRing * _vertexCount) + ((vertex + 1 < _vertexCount) ? (vertex + 1) : 0);
                int vertexIndexC = ((vertexRing - 1) * _vertexCount) + vertex;
                int vertexIndexD = ((vertexRing - 1) * _vertexCount) + ((vertex + 1 < _vertexCount) ? (vertex + 1) : 0);
                
                triangles.Add(vertexIndexA);
                triangles.Add(vertexIndexB);
                triangles.Add(vertexIndexC);

                triangles.Add(vertexIndexB);
                triangles.Add(vertexIndexD);
                triangles.Add(vertexIndexC);
            }
        }

        for (int i = 1; i < _vertexCount - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        
        for (int i = 1; i < _vertexCount - 1; i++)
        {
            triangles.Add(vertices.Count - _vertexCount);
            triangles.Add(vertices.Count - _vertexCount + i + 1);
            triangles.Add(vertices.Count - _vertexCount + i);
        }
        
        _mesh.Clear();
        
        _mesh.SetVertices(vertices);
        _mesh.SetNormals(normals);
        _mesh.SetTriangles(triangles, 0);
        _mesh.subMeshCount = 1;
        
        _mesh.RecalculateBounds();
    }

    private List<Vector3> GenerateRing(Vector3 centerPosition, Quaternion lookRotation)
    {
        List<Vector3> vertices = new List<Vector3>();

        for (int vertex = 0; vertex < _vertexCount; vertex++)
        {
            float radians = Mathf.Lerp(0, (2 * Mathf.PI), ((float)vertex / (float)_vertexCount));
            
            Vector3 position = new Vector3(Mathf.Sin(radians) * _wireRadius, Mathf.Cos(radians) * _wireRadius, 0);

            position = Matrix4x4.Rotate(lookRotation) * position;
            position += centerPosition;
            
            vertices.Add(position);
        }

        return vertices;
    }

    private int _dubugTestCount = 0;
    private int _totalTime = 0;
    private void Update()
    {
        DateTime start = DateTime.Now;
        GenerateMesh();
        DateTime end = DateTime.Now;

        _totalTime += end.Subtract(start).Milliseconds;
        _dubugTestCount++;
        
        float averageTimeElapsed = ((float)_totalTime / (float)_dubugTestCount);
        
        Debug.ClearDeveloperConsole();
        Debug.Log($"Average Time Taken: {averageTimeElapsed}");
    }
    
    private Vector3 CalculatePositionOnBezierCurve(Vector3 start, Vector3 controlPointOne, Vector3 controlPointTwo, Vector3 controlPointThree, Vector3 controlPointFour, Vector3 controlPointFive, Vector3 end, float weight)
    {
        float weightSubtraction = (1 - weight);
        
        Vector3 position = 
            ((     Mathf.Pow(weightSubtraction, 6)) * start) + 
            ((6  * Mathf.Pow(weightSubtraction, 5) * Mathf.Pow(weight, 1)) * controlPointOne) + 
            ((15 * Mathf.Pow(weightSubtraction, 4) * Mathf.Pow(weight, 2)) * controlPointTwo) + 
            ((20 * Mathf.Pow(weightSubtraction, 3) * Mathf.Pow(weight, 3)) * controlPointThree) + 
            ((15 * Mathf.Pow(weightSubtraction, 2) * Mathf.Pow(weight, 4)) * controlPointFour) + 
            ((6  * Mathf.Pow(weightSubtraction, 1) * Mathf.Pow(weight, 5)) * controlPointFive) + 
            ((     Mathf.Pow(weight, 6)) * end);    
        
        return position;
    }
    
    
    private void OnDrawGizmos()
    {
        Vector3 start = lineStart.position;
        Vector3 controlPointOnePosition = controlPointOne.position;
        Vector3 controlPointTwoPosition = controlPointTwo.position;
        Vector3 controlPointThreePosition = controlPointThree.position;
        Vector3 controlPointFourPosition = controlPointFour.position;
        Vector3 controlPointFivePosition = controlPointFive.position;
        Vector3 end = lineEnd.position;
        
        Gizmos.color = new UnityEngine.Color(0.7f, 0.45f, 0.25f);
        Gizmos.DrawLine(start, controlPointOnePosition);
        Gizmos.DrawLine(controlPointOnePosition, controlPointTwoPosition);
        Gizmos.DrawLine(controlPointTwoPosition, controlPointThreePosition);
        Gizmos.DrawLine(controlPointThreePosition, controlPointFourPosition);
        Gizmos.DrawLine(controlPointFourPosition, controlPointFivePosition);
        Gizmos.DrawLine(controlPointFivePosition, end);
    }
    
    /*
    private void GizmoGenerateRing(Vector3 centerPosition, Quaternion lookRotation)
    {
        List<Vector3> vertices = new List<Vector3>();

        for (int vertex = 0; vertex < _vertexCount; vertex++)
        {
            float radians = Mathf.Lerp(0, (2 * Mathf.PI), ((float)vertex / (float)_vertexCount));
            
            Vector3 position = new Vector3(Mathf.Sin(radians) * _wireRadius, Mathf.Cos(radians) * _wireRadius, 0);

            position = Matrix4x4.Rotate(lookRotation) * position;

            position += centerPosition;
            
            Gizmos.color = Color.grey;
            Gizmos.DrawSphere(position, 0.0125f);
            
            vertices.Add(position);
        }

        for (int vertex = 0; vertex < vertices.Count; vertex++)
        {
            Vector3 position = vertices[vertex];
            Vector3 nextPosition = vertices[((vertex + 1 < vertices.Count) ? (vertex + 1) : 0)];
            
            Gizmos.DrawLine(position, nextPosition);
        }
    }
    */
}
