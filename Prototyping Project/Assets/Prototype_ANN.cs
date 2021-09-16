using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Prototype_ANN))]
public class Prototype_ANN_Editor : Editor
{
    private Texture _texture;
    
    
    public override void OnInspectorGUI()
    {
        Prototype_ANN targetScript = (Prototype_ANN) target;
        
        _texture = (Texture) EditorGUILayout.ObjectField(_texture, typeof(Texture), true);
        
        if (GUILayout.Button("Run"))
        {
            targetScript.TestImage(_texture);
        }
    }
}

public class Prototype_ANN : MonoBehaviour
{
    public void TestImage(Texture texture)
    {
        Matrix<double> matrix = Matrix<double>.Build.Random(3, 4);
        Vector<double> vector = Vector<double>.Build.Dense(4);

        Vector<double> output = matrix.Multiply(vector);
    }
}
