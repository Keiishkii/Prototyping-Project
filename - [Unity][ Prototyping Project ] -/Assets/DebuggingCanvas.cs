using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DebuggingCanvas : MonoBehaviour
{
    [SerializeField] private Transform _UIElementSpinner;
    [SerializeField] private float _rotationSpeed;

    [SerializeField] private TMP_Text _currentFramerateLabel;
    [SerializeField] private TMP_Text _averageFramerateLabel;
    
    
    
    
    
    private void Start()
    {
        UISpinnerAsync();
        FramerateCheckAsync();
    }


    private async void UISpinnerAsync()
    {
        int spinnerRotationPhase = 0;
        float spinnerRotationStep = (360f * Time.deltaTime);
        
        while (Application.isPlaying)
        {
            _UIElementSpinner.transform.rotation = Quaternion.Euler(0, 0, spinnerRotationPhase * spinnerRotationStep * _rotationSpeed);
            spinnerRotationPhase++;

            await Task.Yield();
        }
    }

    private async void FramerateCheckAsync()
    {
        float framerateCount = Time.deltaTime;
        float frames = 1;

        while (Application.isPlaying)
        {
            float framerate = (1f / Time.deltaTime);
            
            _currentFramerateLabel.text = $"Current Framerate: {framerate}";
            _averageFramerateLabel.text = $"Average: {(framerateCount / frames)}";

            framerateCount += framerate;
            frames++;

            await Task.Yield();
        }
    }
}
