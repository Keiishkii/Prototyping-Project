using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(EchoNode))]
public class EchoNode_Editor : Editor
{
    private static float strength = 1;

    public override void OnInspectorGUI()
    {
        EchoNode targetScript = (EchoNode) target;

        strength = EditorGUILayout.FloatField("Strength", strength);
        
        if (GUILayout.Button("Ping"))
        {
            targetScript.Ping(strength);
        }
    }
}


public class EchoNode : MonoBehaviour
{
    private static float speedOfSoundSpeed = 4f;
    
    [HideInInspector] public List<EchoNode> connectedNodes = new List<EchoNode>();
    private Transform _transform;


    
    
    
    private void Awake()
    {
        _transform = transform;
    }

    
    
    

    [ContextMenu("Ping")]
    public void Ping(float strength)
    {
        Ping(_transform, strength);
    }
    
    private void Ping(Transform source, float strength)
    {
        if (connectedNodes.Count == 1)
        {
            StartCoroutine(SoundWave(connectedNodes[0].transform, strength));
        }
        else
        {
            foreach (EchoNode node in connectedNodes)
            {
                if (node.gameObject.GetInstanceID() != source.gameObject.GetInstanceID())
                {
                    StartCoroutine(SoundWave(node.transform, strength));
                }
            }
        }
    }

    private IEnumerator SoundWave(Transform nextNode, float strength)
    {
        Vector3 targetPosition = nextNode.position, startingPosition = _transform.position; 
        
        float distance = Vector3.Distance(targetPosition, startingPosition);
        float travelDuration = distance / speedOfSoundSpeed;

        GameObject sound = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Transform soundTransform =sound.transform;
            
        soundTransform.position = startingPosition;
        soundTransform.localScale = Vector3.one * Mathf.Pow(strength, (1f / 3f));
        
        float timeElapsed = 0;
        while (timeElapsed < travelDuration && strength > 0)
        {
            float deltaTime = Time.deltaTime;

            timeElapsed += deltaTime;
            strength -= deltaTime;

            if (strength > 0)
            {
                float tValue = Mathf.InverseLerp(0, travelDuration, timeElapsed);
                Vector3 position = Vector3.Lerp(startingPosition, targetPosition, tValue);

                soundTransform.position = position;
                soundTransform.localScale = Vector3.one * (0.5f * Mathf.Pow(strength, (1f / 3f)));

                yield return null;
            }
        }

        Destroy(sound);
        
        if (strength > 0)
        {
            nextNode.GetComponent<EchoNode>().Ping(_transform, strength);
        }

        yield return null;
    }
    
    
    
    
    

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(position, 0.125f);

        foreach (EchoNode node in connectedNodes)
        {
            Vector3 otherPosition = node.transform.position;
            
            Gizmos.DrawLine(position, otherPosition);
        }
    }
}
