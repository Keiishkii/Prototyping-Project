using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Prototype_FloodEcho))]
public class Prototype_FloodEcho_Editor : CustomEditor_Interface
{
    private float _strength = 1;
    private Vector2Int _position;
    
    protected override void DrawInspectorGUI()
    {
        Prototype_FloodEcho targetScript = (Prototype_FloodEcho) target;
        
        _strength = EditorGUILayout.FloatField("Strength: ", _strength);
        _position = EditorGUILayout.Vector2IntField("Position: ", _position);
        
        if (GUILayout.Button("Activate Cell") && Application.isPlaying)
        {
            if (_position.x >= 0 && _position.x < targetScript.boardBounds.x && _position.y >= 0 && _position.y < targetScript.boardBounds.y)
            {
                Debug.Log("Ping");
                targetScript.gameBoard[_position.x][_position.y] = new SoundCell()
                {
                    strength = _strength,
                    age = 0,
                    live = true,
                    
                    sourceID = SoundCell.sourceIDCounter++
                };
            }
        }
    }
}



public class SoundCell
{
    public static uint sourceIDCounter = 0;
    
    public float strength = 0;
    public int age = 0;
    public int soundDisplacement = 0;
    public bool live = false;

    public bool wall = false;
    public uint sourceID;
}



public class Prototype_FloodEcho : MonoBehaviour
{
    public Vector2Int boardBounds;
    [HideInInspector] public readonly List<List<SoundCell>> gameBoard = new List<List<SoundCell>>();

    private readonly float _decayRate = 0.85f;
    private readonly float _spreadRate = 0.95f;
    private readonly int _oldestAge = 2;
    
    
    
    private void Awake()
    {
        for (int i = 0; i < boardBounds.x; i++)
        {
            gameBoard.Add(new List<SoundCell>());
            for (int j = 0; j < boardBounds.y; j++)
            {
                //if (j < 8 && i == 5 && j != 2) 
                //    gameBoard[i].Add(new SoundCell() {wall = true});
                //else 
                    gameBoard[i].Add(new SoundCell() {strength = 0.0f, age = 0, live = false});
            }
        }

        StartCoroutine(ProcessByTime(0.05f));
    }


    IEnumerator ProcessByTime(float waitTime)
    {
        while (Application.isPlaying)
        {
            yield return new WaitForSeconds(waitTime);
            Simulate();
        }
    }

    
    
    
    
    private void Simulate()
    {
        List<List<SoundCell>> newBoard = new List<List<SoundCell>>();
        for (int i = 0; i < boardBounds.x; i++)
        {
            newBoard.Add(new List<SoundCell>());
            for (int j = 0; j < boardBounds.y; j++)
            {
                SimulateCell(out SoundCell cell, new Vector2Int(i, j));
                newBoard[i].Add(cell);
            }
        }
        
        SetBoard(ref newBoard);
    }

    private void SimulateCell(out SoundCell cell, Vector2Int position)
    {
        SoundCell currentData = gameBoard[position.x][position.y];
        if (!currentData.wall)
        {
            int age = currentData.age + 1;
            int soundDisplacement = (currentData.strength > 0.1f) ? currentData.soundDisplacement : -1;
            bool live = (currentData.live && currentData.age < _oldestAge);
            float strength = currentData.strength;
            uint sourceID = currentData.sourceID;


            if (position.x > 0)
            {
                SoundCell westCell = gameBoard[position.x - 1][position.y];

                if (!westCell.wall && westCell.live && westCell.strength > ((strength / _spreadRate)))
                {
                    if (sourceID != westCell.sourceID || soundDisplacement < 0 || Math.Abs(westCell.soundDisplacement - soundDisplacement) > 2)
                    {
                        live = true;
                        strength = westCell.strength * _spreadRate;
                        age = 0;
                        soundDisplacement = westCell.soundDisplacement + 1;
                        sourceID = westCell.sourceID;
                    }
                }
            }

            if (position.x < boardBounds.x - 1)
            {
                SoundCell eastCell = gameBoard[position.x + 1][position.y];

                if (!eastCell.wall && eastCell.live && eastCell.strength > ((strength / _spreadRate)))
                {
                    if (sourceID != eastCell.sourceID || soundDisplacement < 0 || Math.Abs(eastCell.soundDisplacement - soundDisplacement) > 2)
                    {
                        live = true;
                        strength = eastCell.strength * _spreadRate;
                        age = 0;
                        soundDisplacement = eastCell.soundDisplacement + 1;
                        sourceID = eastCell.sourceID;
                    }
                }
            }

            if (position.y > 0)
            {
                SoundCell southCell = gameBoard[position.x][position.y - 1];

                if (!southCell.wall && southCell.live && southCell.strength > ((strength / _spreadRate)))
                {
                    if (sourceID != southCell.sourceID || soundDisplacement < 0 || Math.Abs(southCell.soundDisplacement - soundDisplacement) > 2)
                    {
                        live = true;
                        strength = southCell.strength * _spreadRate;
                        age = 0;
                        soundDisplacement = southCell.soundDisplacement + 1;
                        sourceID = southCell.sourceID;
                    }
                }
            }

            if (position.y < boardBounds.y - 1)
            {
                SoundCell northCell = gameBoard[position.x][position.y + 1];

                if (!northCell.wall && northCell.live && northCell.strength > ((strength / _spreadRate)))
                {
                    if (sourceID != northCell.sourceID || soundDisplacement < 0 || Math.Abs(northCell.soundDisplacement - soundDisplacement) > 2)
                    {
                        live = true;
                        strength = northCell.strength * _spreadRate;
                        age = 0;
                        soundDisplacement = northCell.soundDisplacement + 1;
                        sourceID = northCell.sourceID;
                    }
                }
            }
            

            if (!live) strength *= _decayRate;
            
            cell = new SoundCell()
            {
                strength = strength,
                age = age,
                live = live,
                soundDisplacement = soundDisplacement,
                sourceID = sourceID
            };
        }
        else
        {
            cell = new SoundCell()
            {
                wall = true
            };
        }
    }

    
    
    
    
    private void SetBoard(ref List<List<SoundCell>> newBoard)
    {
        for (int i = 0; i < boardBounds.x; i++)
        {
            gameBoard.Add(new List<SoundCell>());
            for (int j = 0; j < boardBounds.y; j++)
            {
                gameBoard[i][j] = newBoard[i][j];
            }
        }
    }
    
    
    
    
    
    
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        for (int i = -1; i < boardBounds.x + 1; i++)
        {
            for (int j = -1; j < boardBounds.y + 1; j++)
            {
                Vector3 position = new Vector3(i, 0, j);
                Vector3 center = new Vector3((boardBounds.x - 1) / 2.0f, 0, (boardBounds.y - 1) / 2.0f);
                
                if (i < 0 || i >= boardBounds.x || j < 0 || j >= boardBounds.y)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(position - center, Vector3.one);
                }
                else
                {
                    SoundCell cell = gameBoard[i][j];
                    if (cell.wall)
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(position - center, Vector3.one);
                    }
                    else if (cell.strength > 0.15f)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(position - center, new Vector3(1, 0.25f, 1) * Mathf.Clamp01(cell.strength));
                    }
                }
            }
        }
    }
}
