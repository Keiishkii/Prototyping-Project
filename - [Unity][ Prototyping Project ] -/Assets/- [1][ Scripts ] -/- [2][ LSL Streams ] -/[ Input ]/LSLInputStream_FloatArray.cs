using System.Collections;
using System.Collections.Generic;
using _LSL;
using LSL;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace _LSL
{
    // The LSLInputStream_FloatArray class is a child of LSLInput, this class is designed to access LSL streams broadcasting float data in there channels.
    // On doing so, this class will collect and store the data sent via the stream, in a public read only function. 
    public class LSLInputStream_FloatArray : LSLInput<float>
    {
        [SerializeField] private bool _showLastRecordedSampleInLog;
        [SerializeField] private bool _showSamplesInLog;
        
        
        
        private void Update()
        {
            // If your using the Update function, you will need to manually call the bases Update function too, that's where the sample fetching logic resides.
            base.Update();

            // LastSampleRecorded will return the last value received from the stream.
            if (_showLastRecordedSampleInLog) {
                if (LastSampleRecorded.Length > 0)
                {
                    string sampleLog = "";

                    if (LastSampleRecorded.Length > 0) sampleLog += $"{LastSampleRecorded[0]}";
                    for (int i = 1; i < LastSampleRecorded.Length; i++)
                    {
                        sampleLog += $", {LastSampleRecorded[i]}";
                    }

                    Debug.Log($"Last Sample Recorded: ({sampleLog})");
                }
            }

            // Samples will return the samples received on this frame.
            // This could be none or it could be many as the stream will not be tied to the exact same timings as Unity's Update function.
            if (_showSamplesInLog) {
                for (int sampleIndex = 0; sampleIndex < Samples.Count; sampleIndex++)
                {
                    string sampleLog = "";

                    if (Samples[sampleIndex].Item1.Length > 0) sampleLog += $"{Samples[sampleIndex].Item1[0]}";
                    for (int i = 1; i < Samples[sampleIndex].Item1.Length; i++)
                    {
                        sampleLog += $", {Samples[sampleIndex].Item1[i]}";
                    }

                    Debug.Log($"Sample [{sampleIndex}]: ({sampleLog})");
                }
            }
        }
    }
}