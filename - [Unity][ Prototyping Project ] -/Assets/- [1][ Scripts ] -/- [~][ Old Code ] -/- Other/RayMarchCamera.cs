using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RayMarchCamera : MonoBehaviour
{
    [SerializeField] private Shader _rayMarchingShader;

    private Material _rayMarchMaterial;
    public Material RayMarchMaterial
    {
        get
        {
            if (ReferenceEquals(_rayMarchMaterial, null) && !ReferenceEquals(_rayMarchingShader, null))
            {
                _rayMarchMaterial = new Material(_rayMarchingShader);
                _rayMarchMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return _rayMarchMaterial;
        }
    }

    private Camera _rayMarchingCamera;
    public Camera RayMarchingCamera
    {
        get
        {
            if (ReferenceEquals(_rayMarchingCamera, null))
            {
                _rayMarchingCamera = GetComponent<Camera>();
            }
            
            return _rayMarchingCamera;
        }
    }

    
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (ReferenceEquals(RayMarchMaterial, null))
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            
        }
    }
}
