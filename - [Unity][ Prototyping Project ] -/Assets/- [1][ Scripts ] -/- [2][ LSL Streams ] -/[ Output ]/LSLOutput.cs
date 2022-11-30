#if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
    using System.Collections.Generic;
    using LSL;
    using UnityEngine.Rendering;
    using UnityEngine;
#endif

namespace _LSL
{
    // Abstract class used for writing LSL streams. Stores the values it will be writing, and the name of the stream and its description.
    public abstract class LSLOutput<T> : MonoBehaviour
    {
        #if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        
        [SerializeField] protected string _streamName = "Stream Name";
        [SerializeField] protected string _streamType = "Stream Type";

        protected StreamOutlet _outlet;
        private readonly List<T[]> _streamOutputData = new List<T[]>();

        
        
        
        
        // Subscribes the OnFrameRenderComplete function to the URPs endFrameRendering Event
        private void OnEnable()
        {
            RenderPipelineManager.endFrameRendering += OnFrameRenderComplete;
        }
        
        // Unsubscribes the OnFrameRenderComplete function to the URPs endFrameRendering Event
        private void OnDisable()
        {
            RenderPipelineManager.endFrameRendering -= OnFrameRenderComplete;
        }

        

        // Adds the sample to the stream output list
        protected void PushOutput(in T[] sample)
        {
            _streamOutputData.Add(sample);
        }

        // On Rendering to the screen at the end of the frame, pushes all samples over the network using the outlet.
        private void OnFrameRenderComplete(ScriptableRenderContext arg1, Camera[] arg2)
        {
            switch (_outlet.info().channel_format())
            {
                case channel_format_t.cf_string: {
                    foreach (var sample in _streamOutputData)
                    {
                        if (sample is string[] castSample) _outlet.push_sample(castSample);
                    }
                } break;
                case channel_format_t.cf_double64: {
                    foreach (var sample in _streamOutputData)
                    {
                        if (sample is double[] castSample) _outlet.push_sample(castSample);
                    }
                } break;
                case channel_format_t.cf_float32: {
                    foreach (var sample in _streamOutputData)
                    {
                        if (sample is float[] castSample) _outlet.push_sample(castSample);
                    }
                } break;
                case channel_format_t.cf_int8:
                case channel_format_t.cf_int16:
                case channel_format_t.cf_int32:
                case channel_format_t.cf_int64: {
                    foreach (var sample in _streamOutputData)
                    {
                        if (sample is int[] castSample) _outlet.push_sample(castSample);
                    }
                } break;
                case channel_format_t.cf_undefined:
                {
                    if (_streamOutputData.Count == 0) break;
                    Debug.LogError("Channel Type Undefined for sample push");
                } break;
                default:
                {
                    Debug.LogError("Channel Type was not recognised");
                } break;
            }
            
            _streamOutputData.Clear();
        }
        
        #endif
    }
}