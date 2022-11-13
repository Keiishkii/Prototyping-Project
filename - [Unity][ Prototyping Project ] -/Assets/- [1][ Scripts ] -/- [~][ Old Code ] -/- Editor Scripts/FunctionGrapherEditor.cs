using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MoonSharp.Interpreter;

[CustomEditor(typeof(FunctionGrapher))]
public class FunctionGrapherEditor : Editor
{
    private struct Bounds
    {
        public float minimumXValue, maximumXValue;
        public float minimumYValue, maximumYValue;
        public float minimumTValue, maximumTValue;

        public Bounds(float minimumXValue, float maximumXValue, float minimumYValue, float maximumYValue, float minimumTValue, float maximumTValue)
        {
            this.minimumXValue = minimumXValue;
            this.maximumXValue = maximumXValue;
            this.minimumYValue = minimumYValue;
            this.maximumYValue = maximumYValue;
            this.minimumTValue = minimumTValue;
            this.maximumTValue = maximumTValue;
        }
    }
    private enum EquationType
    {
        Default,
        Parametric
    }

    private static GUIStyle _styleInstance;
    private static GUIStyle _style
    {
        get { return _styleInstance ??= new GUIStyle {richText = true}; }
    }
    
    private static Material _materialInstance;
    private static Material _material
    {
        get { return _materialInstance ?? (_materialInstance = new Material(Shader.Find("Hidden/Internal-Colored"))); }
    }
 
    private string _stringEquationOne, _stringEquationTwo;
    
    private List<Vector2> _plottedCoordinates = new List<Vector2>();
    
    private Bounds _bounds = new Bounds(-10, 10, -10, 10, -10, 10);
    private EquationType _equationTypeDropDown = EquationType.Default;
    private EquationType _equationType = EquationType.Default;
    
    
    
    
    public override void OnInspectorGUI()
    {
        Rect graphUIBounds = GUILayoutUtility.GetRect(10, 1000, 200, 200);
        GUILayout.Space(5);
        
        PlotGUIElements();
        GUILayout.Space(5);
        
        BoundsGUIElements();
        
        RenderGraph(graphUIBounds);
    }


    private void PlotGUIElements()
    {
        GUILayout.Label("<b><color=#FFF>Plot:</color></b>", _style);
        
        _equationTypeDropDown = (EquationType) EditorGUILayout.EnumPopup(_equationTypeDropDown);

        switch (_equationTypeDropDown)
        {
            case EquationType.Default:
            {
                GUILayout.Label("<b><color=#FFF>    Y:</color></b>", _style);
                _stringEquationOne = EditorGUILayout.TextArea(_stringEquationOne);
            } break;
            case EquationType.Parametric:
            {
                GUILayout.Label("<b><color=#FFF>    X:</color></b>", _style);
                _stringEquationOne = EditorGUILayout.TextArea(_stringEquationOne);
                
                GUILayout.Label("<b><color=#FFF>    Y:</color></b>", _style);
                _stringEquationTwo = EditorGUILayout.TextArea(_stringEquationTwo);
            } break;
        }
        
        if (GUILayout.Button("Plot"))
        {
            _equationType = _equationTypeDropDown;
            
            switch (_equationType)
            {
                case EquationType.Default:
                {
                    Script luaScript = new Script();
                    luaScript.DoString(_stringEquationOne);
            
                    DynValue luaFunction = luaScript.Globals.Get("equate");
                    
                    
                    
                    if (luaFunction != null)
                    {
                        _plottedCoordinates.Clear();
                        for (float x = _bounds.minimumXValue; x <= _bounds.maximumXValue; x += 0.01f)
                        {
                            DynValue yResult = luaScript.Call(luaFunction, x);
                        
                            _plottedCoordinates.Add(new Vector2(x, (float) yResult.Number));
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to plot the equation");   
                    }
                } break;
                case EquationType.Parametric:
                {
                    Script luaXScript = new Script();
                    luaXScript.DoString(_stringEquationOne);
            
                    DynValue luaXFunction = luaXScript.Globals.Get("equate");
                    
                    Script luaYScript = new Script();
                    luaYScript.DoString(_stringEquationTwo);
            
                    DynValue luaYFunction = luaYScript.Globals.Get("equate");
                    
                    
                    
                    if (luaXFunction != null || luaYFunction != null)
                    {
                        _plottedCoordinates.Clear();
                        for (float t = _bounds.minimumTValue; t <= _bounds.maximumTValue; t += 0.01f)
                        {
                            DynValue xResult = luaXScript.Call(luaXFunction, t);
                            DynValue yResult = luaYScript.Call(luaYFunction, t);
                        
                            _plottedCoordinates.Add(new Vector2((float) xResult.Number, (float) yResult.Number));
                        }
                    }
                } break;
            }
        }
    }
    
    private void BoundsGUIElements()
    {
        GUILayout.Label("<b><color=#FFF>Bounds:</color></b>", _style);

        if (_equationType == EquationType.Parametric)
        {
            GUILayout.BeginHorizontal();
            {
                _bounds.minimumTValue = Mathf.Min(EditorGUILayout.FloatField("Min T: ", _bounds.minimumTValue), (_bounds.maximumTValue - 0.1f));
                _bounds.maximumTValue = Mathf.Max(EditorGUILayout.FloatField("Max T: ", _bounds.maximumTValue), (_bounds.minimumTValue + 0.1f));
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        {
            _bounds.minimumXValue = Mathf.Min(EditorGUILayout.FloatField("Min X: ", _bounds.minimumXValue), (_bounds.maximumXValue - 0.1f));
            _bounds.maximumXValue = Mathf.Max(EditorGUILayout.FloatField("Max X: ", _bounds.maximumXValue), (_bounds.minimumXValue + 0.1f));
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        {
            _bounds.minimumYValue = Mathf.Min(EditorGUILayout.FloatField("Min Y: ", _bounds.minimumYValue), (_bounds.maximumYValue - 0.1f));
            _bounds.maximumYValue = Mathf.Max(EditorGUILayout.FloatField("Max Y: ", _bounds.maximumYValue), (_bounds.minimumYValue + 0.1f));
        }
        GUILayout.EndHorizontal();
    }
    
    private void RenderGraph(Rect graphUIBounds)
    {
        GUI.BeginClip(graphUIBounds);
        GL.Clear(true, false, Color.black);
        _material.SetPass(0);
        
        RenderBackground(graphUIBounds);
        RenderGrid(graphUIBounds);
        RenderPlot(graphUIBounds);

        GUI.EndClip();
    }

    private void RenderPlot(Rect graphUIBounds)
    {
        if (_plottedCoordinates.Count > 0)
        {
            GL.Begin(GL.LINES);
            GL.Color(new Color(0.2f, 0.95f, 0.375f));
            Vector2 previousCoordinate = _plottedCoordinates[0];
            for (int i = 1; i < _plottedCoordinates.Count; i++)
            {
                Vector2 currentCoordinate = _plottedCoordinates[i];

                if ((((previousCoordinate.y < _bounds.maximumYValue && previousCoordinate.y > _bounds.minimumYValue) && (previousCoordinate.x < _bounds.maximumXValue && previousCoordinate.x > _bounds.minimumXValue)) || 
                    ((currentCoordinate.y < _bounds.maximumYValue && currentCoordinate.y > _bounds.minimumYValue) && (currentCoordinate.x < _bounds.maximumXValue && currentCoordinate.x > _bounds.minimumXValue))) && 
                    Vector2.SqrMagnitude(currentCoordinate - previousCoordinate) < 1000)
                {
                    Vector2 inverseScaledCurrentCoordinate = new Vector2(
                        Mathf.InverseLerp(_bounds.minimumXValue, _bounds.maximumXValue, currentCoordinate.x),
                        Mathf.InverseLerp(_bounds.minimumYValue, _bounds.maximumYValue, currentCoordinate.y));

                    Vector2 boundsScaledCurrentCoordinate = new Vector2(
                        Mathf.LerpUnclamped(0, graphUIBounds.width, inverseScaledCurrentCoordinate.x),
                        Mathf.LerpUnclamped(graphUIBounds.height, 0, inverseScaledCurrentCoordinate.y));

                    GL.Vertex3(boundsScaledCurrentCoordinate.x, boundsScaledCurrentCoordinate.y, 0);


                    Vector2 inverseScaledPreviousCoordinate = new Vector2(
                        Mathf.InverseLerp(_bounds.minimumXValue, _bounds.maximumXValue, previousCoordinate.x),
                        Mathf.InverseLerp(_bounds.minimumYValue, _bounds.maximumYValue, previousCoordinate.y));

                    Vector2 boundsScaledPreviousCoordinate = new Vector2(
                        Mathf.LerpUnclamped(0, graphUIBounds.width, inverseScaledPreviousCoordinate.x),
                        Mathf.LerpUnclamped(graphUIBounds.height, 0, inverseScaledPreviousCoordinate.y));

                    GL.Vertex3(boundsScaledPreviousCoordinate.x, boundsScaledPreviousCoordinate.y, 0);
                }

                previousCoordinate = currentCoordinate;
            }

            GL.End();
        }
    }
    
    private void RenderBackground(Rect graphUIBounds)
    {
        GL.Begin(GL.QUADS);
        GL.Color(new Color(0.125f, 0.125f, 0.125f));
        
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(graphUIBounds.width, 0, 0);
        GL.Vertex3(graphUIBounds.width, graphUIBounds.height, 0);
        GL.Vertex3(0, graphUIBounds.height, 0);

        GL.End();
    }

    private void RenderGrid(Rect graphUIBounds)
    {
        GL.Begin(GL.LINES);
        
        for (int verticalLine = Mathf.FloorToInt(_bounds.minimumXValue); verticalLine <= Mathf.CeilToInt(_bounds.maximumXValue); verticalLine++)
        {
            GL.Color((verticalLine == 0) ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.175f, 0.175f, 0.175f));
            
            float inverseValue = Mathf.InverseLerp(_bounds.minimumXValue, _bounds.maximumXValue, verticalLine);
            float positionInBounds = Mathf.Lerp(0, graphUIBounds.width, inverseValue);

            GL.Vertex3(positionInBounds, 0, 0);
            GL.Vertex3(positionInBounds, graphUIBounds.height, 0);
        }
        
        for (int horizontalLine = Mathf.FloorToInt(_bounds.minimumYValue); horizontalLine < Mathf.CeilToInt(_bounds.maximumYValue); horizontalLine++)
        {
            GL.Color((horizontalLine == 0) ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.175f, 0.175f, 0.175f));
            
            float inverseValue = Mathf.InverseLerp(_bounds.minimumYValue, _bounds.maximumYValue, horizontalLine);
            float positionInBounds = Mathf.LerpUnclamped(graphUIBounds.height, 0, inverseValue);

            GL.Vertex3(0, positionInBounds, 0);
            GL.Vertex3(graphUIBounds.width, positionInBounds, 0);
            
        }

        GL.End();
    }
}
