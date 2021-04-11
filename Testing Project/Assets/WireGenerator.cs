using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = System.Drawing.Color;

[Serializable]
public class WireSegment
{
    public Transform controlPointOne;
    public Transform controlPointTwo;
    public Transform controlPointThree;
    public Transform controlPointFour;
    public Transform controlPointFive;
    public Transform wireSegmentEnd;
}



[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WireGenerator : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private readonly int _vertexCount = 6;
    private readonly float _wireRadius = 0.025f;
    private readonly float _increment = 0.025f;

    public Transform wireStart;
    public List<WireSegment> wireSegments = new List<WireSegment>();

    private int _dubugTestCount = 0;
    private int _totalTime = 0;

    private void Awake()
    {
        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;

        GenerateMeshWithTimer();
    }





    private void Update()
    {
        GenerateMeshWithTimer();
    }





    private void GenerateMeshWithTimer()
    {
        DateTime start = DateTime.Now;
        GenerateMesh();
        DateTime end = DateTime.Now;

        _totalTime += end.Subtract(start).Milliseconds;
        _dubugTestCount++;

        float averageTimeElapsed = ((float)_totalTime / (float)_dubugTestCount);

        Debug.Log($"Average Time Taken: {averageTimeElapsed}");
    }

    private void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 segmentStartingPosition = wireStart.localPosition;

        int segmentStartVertexIndex = 0;
        Quaternion lookRotation = Quaternion.identity;

        for (int segment = 0; segment < wireSegments.Count; segment++)
        {
            Vector3 controlOne = wireSegments[segment].controlPointOne.localPosition;
            Vector3 controlTwo = wireSegments[segment].controlPointTwo.localPosition;
            Vector3 controlThree = wireSegments[segment].controlPointThree.localPosition;
            Vector3 controlFour = wireSegments[segment].controlPointFour.localPosition;
            Vector3 controlFive = wireSegments[segment].controlPointFive.localPosition;
            Vector3 segmentEndingPosition = wireSegments[segment].wireSegmentEnd.localPosition;

            GenerateWireSegment(ref vertices, ref normals, ref triangles, segment, ref lookRotation, segmentStartingPosition, controlOne, controlTwo, controlThree, controlFour, controlFive, segmentEndingPosition);

            if (segment != 0)
            {
                GenerateWireSegmentLinks(ref triangles, segmentStartVertexIndex);
            }            

            segmentStartVertexIndex = vertices.Count;
            segmentStartingPosition = segmentEndingPosition;
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

        //Debug.Log($"Vertex Count: {vertices.Count}, Triangles: {triangles.Count / 3}");
    }

    private void GenerateWireSegment(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<int> triangles, int segmentIndex, ref Quaternion previousRotation, Vector3 startingPosition, Vector3 controlOne, Vector3 controlTwo, Vector3 controlThree, Vector3 controlFour, Vector3 controlFive, Vector3 endingPosition)
    {
        List<List<Vector3>> vertexRings = new List<List<Vector3>>();
        int prevousSegmentVertexCount = vertices.Count;

        Vector3 centerPosition = CalculatePositionOnBezierCurve(startingPosition, controlOne, controlTwo, controlThree, controlFour, controlFive, endingPosition, 0);
        Vector3 directionPosition = CalculatePositionOnBezierCurve(startingPosition, controlOne, controlTwo, controlThree, controlFour, controlFive, endingPosition, _increment);

        if (segmentIndex == 0)
        {
            previousRotation = Quaternion.LookRotation(directionPosition - centerPosition, Vector3.up);
        }

        Quaternion lookRotation = Quaternion.identity;

        for (float i = 0; i < 1; i += _increment)
        {
            if (i + _increment < 1)
            {
                centerPosition = CalculatePositionOnBezierCurve(startingPosition, controlOne, controlTwo, controlThree, controlFour, controlFive, endingPosition, i);
                directionPosition = CalculatePositionOnBezierCurve(startingPosition, controlOne, controlTwo, controlThree, controlFour, controlFive, endingPosition, (i + _increment));

                Quaternion positiveLookRotation = Quaternion.LookRotation(directionPosition - centerPosition, Vector3.up);
                Quaternion negativeLookRotation = Quaternion.LookRotation(directionPosition - centerPosition, Vector3.down);

                lookRotation = (Vector3.Dot(previousRotation * Vector3.up, positiveLookRotation * Vector3.up) > 0) ? positiveLookRotation : negativeLookRotation;
                previousRotation = lookRotation;
            }
            else
            {
                centerPosition = CalculatePositionOnBezierCurve(startingPosition, controlOne, controlTwo, controlThree, controlFour, controlFive, endingPosition, 1);
                directionPosition = CalculatePositionOnBezierCurve(startingPosition, controlOne, controlTwo, controlThree, controlFour, controlFive, endingPosition, (1 - _increment));

                Quaternion positiveLookRotation = Quaternion.LookRotation(centerPosition - directionPosition, Vector3.up);
                Quaternion negativeLookRotation = Quaternion.LookRotation(centerPosition - directionPosition, Vector3.down);

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

        int segmentMultiplyer = segmentIndex + 1;

        for (int vertexRing = 1; vertexRing < vertexRings.Count; vertexRing++)
        {
            for (int vertex = 0; vertex < _vertexCount; vertex++)
            {
                int vertexIndexA = prevousSegmentVertexCount + (vertexRing * _vertexCount) + vertex;
                int vertexIndexB = prevousSegmentVertexCount + (vertexRing * _vertexCount) + ((vertex + 1 < _vertexCount) ? (vertex + 1) : 0);
                int vertexIndexC = prevousSegmentVertexCount + ((vertexRing - 1) * _vertexCount) + vertex;
                int vertexIndexD = prevousSegmentVertexCount + ((vertexRing - 1) * _vertexCount) + ((vertex + 1 < _vertexCount) ? (vertex + 1) : 0);

                triangles.Add(vertexIndexA);
                triangles.Add(vertexIndexB);
                triangles.Add(vertexIndexC);

                triangles.Add(vertexIndexB);
                triangles.Add(vertexIndexD);
                triangles.Add(vertexIndexC);
            }
        }
    }

    private void GenerateWireSegmentLinks(ref List<int> triangles, int segmentStartVertexIndex)
    {
        for (int vertex = segmentStartVertexIndex; vertex < segmentStartVertexIndex + _vertexCount; vertex++)
        {
            int vertexIndexA = vertex;
            int vertexIndexB = ((vertex + 1 != segmentStartVertexIndex + _vertexCount) ? (vertex + 1) : segmentStartVertexIndex);
            int vertexIndexC = vertex - _vertexCount;
            int vertexIndexD = ((vertex - _vertexCount + 1 != segmentStartVertexIndex) ? (vertex - _vertexCount + 1) : segmentStartVertexIndex - _vertexCount);

            triangles.Add(vertexIndexA);
            triangles.Add(vertexIndexB);
            triangles.Add(vertexIndexC);

            triangles.Add(vertexIndexB);
            triangles.Add(vertexIndexD);
            triangles.Add(vertexIndexC);
        }
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

    private Vector3 CalculatePositionOnBezierCurve(Vector3 start, Vector3 controlPointOne, Vector3 controlPointTwo, Vector3 controlPointThree, Vector3 controlPointFour, Vector3 controlPointFive, Vector3 end, float weight)
    {
        float weightSubtraction = (1 - weight);

        Vector3 position =
            ((Mathf.Pow(weightSubtraction, 6)) * start) +
            ((6 * Mathf.Pow(weightSubtraction, 5) * Mathf.Pow(weight, 1)) * controlPointOne) +
            ((15 * Mathf.Pow(weightSubtraction, 4) * Mathf.Pow(weight, 2)) * controlPointTwo) +
            ((20 * Mathf.Pow(weightSubtraction, 3) * Mathf.Pow(weight, 3)) * controlPointThree) +
            ((15 * Mathf.Pow(weightSubtraction, 2) * Mathf.Pow(weight, 4)) * controlPointFour) +
            ((6 * Mathf.Pow(weightSubtraction, 1) * Mathf.Pow(weight, 5)) * controlPointFive) +
            ((Mathf.Pow(weight, 6)) * end);

        return position;
    }
}
