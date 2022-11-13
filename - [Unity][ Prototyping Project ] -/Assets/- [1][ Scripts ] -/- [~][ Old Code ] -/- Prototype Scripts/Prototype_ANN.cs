using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEditor;
using UnityEngine;

public class Bounds : Object
{
    public float minimum, maximum;
}

[CustomEditor(typeof(Prototype_ANN))]
public class Prototype_ANN_Editor : Editor
{ private delegate void PlotGraph(Rect renderRect);
    private PlotGraph graphPlotFunction;
    
    private float _theta = 0.2f;
    private float _minimumXBounds = -1, _maximumXBounds = 1;
    private float _minimumYBounds = -1, _maximumYBounds = 1;
    
    private Material mat;
    
    
    
    private void OnEnable()
    {
        var shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);
    }
    
    public override void OnInspectorGUI()
    {
        Prototype_ANN targetScript = (Prototype_ANN) target;

        Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
        if (Event.current.type == EventType.Repaint)
        {
            Debug.Log("EventRender");

            GUI.BeginClip(rect);
            GL.Clear(true, false, Color.black);
            mat.SetPass(0);

            graphPlotFunction?.Invoke(rect);

            GUI.EndClip();
        }



        GUILayout.Space(25);
        
        
        
        GUILayout.Label("X Bounds"); 
        
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        
        _minimumXBounds = EditorGUILayout.FloatField("Min", _minimumXBounds);
        _maximumXBounds = EditorGUILayout.FloatField("Max:", _maximumXBounds);
        
        GUILayout.EndHorizontal();
        
        GUILayout.Label("Y Bounds"); 
        
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        
        _minimumYBounds = EditorGUILayout.FloatField("Min", _minimumYBounds);
        _maximumYBounds = EditorGUILayout.FloatField("Max:", _maximumYBounds);
        
        GUILayout.EndHorizontal();
        
        
        _theta = EditorGUILayout.FloatField("Theta:", _theta);
        
        
        
        if (GUILayout.Button("Plot Signed Graph"))
            graphPlotFunction = PlotSignGraph;
        
        if (GUILayout.Button("Plot Step Graph"))
            graphPlotFunction = PlotStepGraph;
        
        if (GUILayout.Button("Plot Sigmoid Graph"))
            graphPlotFunction = PlotSigmoidGraph;
    }

    private void DrawGraphBackground(Rect renderRect)
    {
        GL.Begin(GL.QUADS);
        GL.Color(new Color(0.25f, 0.25f, 0.25f));
        
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(renderRect.width, 0, 0);
        GL.Vertex3(renderRect.width, renderRect.height, 0);
        GL.Vertex3(0, renderRect.height, 0);
        
        GL.End();
    }

    private void PlotSignGraph(Rect renderRect)
    {
        DrawGraphBackground(renderRect);

        GL.Begin(GL.LINES);
        GL.Color(new Color(0.5f, 1, 0.8f));

        float previousX = _minimumXBounds, previousResult = Prototype_ANN.CalculateSign(previousX, _theta);
        for (float x = _minimumXBounds + 0.001f; x <= _maximumXBounds; x += 0.001f)
        {
            float result = Prototype_ANN.CalculateSign(x, _theta);
            
            float scaledPreviousX = Mathf.Lerp(0, renderRect.width, Mathf.InverseLerp(_minimumXBounds, _maximumXBounds, previousX));
            float scaledPreviousResult = renderRect.height * 0.025f + Mathf.Lerp(0, renderRect.height * 0.95f, Mathf.Clamp01(Mathf.InverseLerp(_minimumYBounds, _maximumYBounds, previousResult * -1f)));
            GL.Vertex3(scaledPreviousX, scaledPreviousResult, 0);
            
            float scaledX = Mathf.Lerp(0, renderRect.width, Mathf.InverseLerp(_minimumXBounds, _maximumXBounds, x));
            float scaledResult = renderRect.height * 0.025f + Mathf.Lerp(0, renderRect.height * 0.95f, Mathf.Clamp01(Mathf.InverseLerp(_minimumYBounds, _maximumYBounds, result * -1f)));
            GL.Vertex3(scaledX, scaledResult, 0);

            previousX = x;
            previousResult = result;
        }
        
        GL.End();
    }

    public void PlotStepGraph(Rect renderRect)
    {
        DrawGraphBackground(renderRect);

        GL.Begin(GL.LINES);
        GL.Color(new Color(0.5f, 1, 0.8f));

        float previousX = _minimumXBounds, previousResult = Prototype_ANN.CalculateStep(previousX, _theta);
        for (float x = _minimumXBounds + 0.001f; x <= _maximumXBounds; x += 0.001f)
        {
            float result = Prototype_ANN.CalculateStep(x, _theta);
            
            float scaledPreviousX = Mathf.Lerp(0, renderRect.width, Mathf.InverseLerp(_minimumXBounds, _maximumXBounds, previousX));
            float scaledPreviousResult = renderRect.height * 0.025f + Mathf.Lerp(0, renderRect.height * 0.95f, Mathf.Clamp01(Mathf.InverseLerp(_minimumYBounds, _maximumYBounds, previousResult * -1f)));
            GL.Vertex3(scaledPreviousX, scaledPreviousResult, 0);
            
            float scaledX = Mathf.Lerp(0, renderRect.width, Mathf.InverseLerp(_minimumXBounds, _maximumXBounds, x));
            float scaledResult = renderRect.height * 0.025f + Mathf.Lerp(0, renderRect.height * 0.95f, Mathf.Clamp01(Mathf.InverseLerp(_minimumYBounds, _maximumYBounds, result * -1f)));
            GL.Vertex3(scaledX, scaledResult, 0);

            previousX = x;
            previousResult = result;
        }
        
        GL.End();
    }

    public void PlotSigmoidGraph(Rect renderRect)
    {
        DrawGraphBackground(renderRect);

        GL.Begin(GL.LINES);
        GL.Color(new Color(0.5f, 1, 0.8f));

        float previousX = _minimumXBounds, previousResult = Prototype_ANN.CalculateSigmoid(previousX, _theta);
        for (float x = _minimumXBounds + 0.001f; x <= _maximumXBounds; x += 0.001f)
        {
            float result = Prototype_ANN.CalculateSigmoid(x, _theta);
            
            float scaledPreviousX = Mathf.Lerp(0, renderRect.width, Mathf.InverseLerp(_minimumXBounds, _maximumXBounds, previousX));
            float scaledPreviousResult = renderRect.height * 0.025f + Mathf.Lerp(0, renderRect.height * 0.95f, Mathf.Clamp01(Mathf.InverseLerp(_minimumYBounds, _maximumYBounds, previousResult * -1f)));
            GL.Vertex3(scaledPreviousX, scaledPreviousResult, 0);
            
            float scaledX = Mathf.Lerp(0, renderRect.width, Mathf.InverseLerp(_minimumXBounds, _maximumXBounds, x));
            float scaledResult = renderRect.height * 0.025f + Mathf.Lerp(0, renderRect.height * 0.95f, Mathf.Clamp01(Mathf.InverseLerp(_minimumYBounds, _maximumYBounds, result * -1f)));
            GL.Vertex3(scaledX, scaledResult, 0);

            previousX = x;
            previousResult = result;
        }
        
        GL.End();
    }
}

public class Prototype_ANN : MonoBehaviour
{
    public void RunTest(Texture texture)
    {
        //Matrix<double> matrix = Matrix<double>.Build.Random(3, 4);
        //Vector<double> vector = Vector<double>.Build.Dense(4);

        //Vector<double> output = matrix.Multiply(vector);
    }

    public static float CalculateSign(float input, float theta)
    {
        return (input >= theta) ? 1f : -1f;
    }

    public static float CalculateStep(float input, float theta)
    {
        return (input >= theta) ? 1f : 0f;
    }

    public static float CalculateSigmoid(float input, float theta)
    {
        return 1f / (1f + Mathf.Exp(-(input - theta)));
    }
}
