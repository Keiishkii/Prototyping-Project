using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectSettings : MonoBehaviour
{
    public bool ApplyTargetFramerate = false;
    public int TargetFramerate = 60;

    private void Awake()
    {
        ApplyProjectSettings();
    }

    [ContextMenu("Apply Project Settings")]
    private void ApplyProjectSettings()
    {
        if (ApplyTargetFramerate) Application.targetFrameRate = TargetFramerate;
        else QualitySettings.vSyncCount = 1;
    }
}
