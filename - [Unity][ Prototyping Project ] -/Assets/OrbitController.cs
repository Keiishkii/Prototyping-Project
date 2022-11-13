using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OrbitBodies
{
    public Transform orbitTransform;
    public Vector2 orbitBounds;
    public Vector3 orbitOrientation;
    public float speed;
}

public class OrbitController : MonoBehaviour
{
    [SerializeField] private List<OrbitBodies> _bodies = new List<OrbitBodies>();
    
    
    
    // Update is called once per frame
    void Update()
    {
        float time = Time.realtimeSinceStartup;
        foreach (var body in _bodies)
        {
            if (body.orbitTransform != null)
            {
                Transform orbitTransform = body.orbitTransform;
                orbitTransform.localPosition = Quaternion.Euler(body.orbitOrientation) * new Vector3()
                {
                    x = Mathf.Sin(time * body.speed) * body.orbitBounds.x,
                    z = Mathf.Cos(time * body.speed) * body.orbitBounds.y,
                };
                
                orbitTransform.rotation = Quaternion.LookRotation(-orbitTransform.localPosition - new Vector3(){ y = 1.5f});
            }
        }
    }
}
