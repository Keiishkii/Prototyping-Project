using UnityEngine;

public class OrbitController : MonoBehaviour
{
    public Vector3 orbitOriginVector;
    public Transform orbitOriginTransform;
    
    public Vector3 orbitOrientation;
    public Vector2 orbitBounds;
    public float speed;

    public bool useTransformAsOrigin;
    public bool rotateToLookAtOrigin;
    
    private Transform _transform;

    
    
    
    
    private void Awake()
    {
        _transform = transform;
    }

    
    
    private void FixedUpdate()
    {
        float time = Time.realtimeSinceStartup;

        Vector3 origin = ((useTransformAsOrigin) ? (orbitOriginTransform.position) : (orbitOriginVector));
        Vector3 position = origin + (Quaternion.Euler(orbitOrientation) * new Vector3()
        {
            x = Mathf.Sin(time * speed) * orbitBounds.x,
            z = Mathf.Cos(time * speed) * orbitBounds.y,
        });

        _transform.position = position;
        
        if (rotateToLookAtOrigin) 
            _transform.rotation = Quaternion.LookRotation(Vector3.Normalize(origin - position));
    }
}
