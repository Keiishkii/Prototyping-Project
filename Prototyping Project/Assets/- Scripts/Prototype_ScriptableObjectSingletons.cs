using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Prototype_ScriptableObjectSingletons : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"Radius: {BallData.instance.BallRadius}");
    }
}