#if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
    using LSL;
#endif
    using System;
    using System.Collections.Generic;
    using UnityEngine;

namespace _LSL
{
    // This is the base class for LSLInput streams,
    // I've encapsulated the behaviour of reading from the streams every frame inside the script.
    // This covers all supported LSL Stream types, you just need to make your child class / component inherit from LSLInput with the desired type as T,
    // For example: "public static class LSLInputStream : LSLInput<float>"
    public abstract class LSLInput<T> : MonoBehaviour
    {
        #if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        
        public LSLStreamLookup streamLookup = LSLStreamLookup.Name;
        
        public string streamName = "Stream Name";
        public string streamType = "Stream Type";
        
        private StreamInfo[] _streamInfos;
        private StreamInlet _streamInlet;
        private int _channelCount;

        private bool _streamAcquired;
        
        // Tuple List: Item1 = Sample Data, Item2 = Time Stamp
        private readonly List<(T[], double)> _samples = new List<(T[], double)>();
        protected List<(T[], double)> Samples => _samples;
        
        private T[] _lastSampleRecorded = new T[0];
        protected T[] LastSampleRecorded => _lastSampleRecorded;

        
        
        

        protected void Update()
        {
            if (!_streamAcquired) AcquireInletStream();
            if (_streamAcquired) ProcessStreamSamples();
        }

        private void AcquireInletStream()
        {
            _streamInfos = streamLookup switch
            {
                LSLStreamLookup.Name => LSL.LSL.resolve_stream("name", streamName, 1, 0),
                LSLStreamLookup.Type => LSL.LSL.resolve_stream("type", streamType, 1, 0),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_streamInfos.Length <= 0) return;
            
            _streamInlet = new StreamInlet(_streamInfos[0]);
            _channelCount = _streamInlet.info().channel_count();
            _streamInlet.open_stream();

            Debug.Log($"Stream ({streamName}) Found");
            _streamAcquired = true;
        }

        private void ProcessStreamSamples()
        {
            _samples.Clear();

            double timeStamp;
            T[] currentSample = new T[_channelCount];
            while ((timeStamp = PullUnknownSample(_streamInlet, ref currentSample)) != 0)
            {
                _samples.Add((currentSample, timeStamp));
                _lastSampleRecorded = currentSample;
            }
        }

        private static double PullUnknownSample(in StreamInlet streamInlet, ref T[] sample)
        {
            double lastTimeStamp = sample switch
            {
                char[] floatSample => PullSample(streamInlet, ref floatSample),
                double[] floatSample => PullSample(streamInlet, ref floatSample),
                float[] floatSample => PullSample(streamInlet, ref floatSample),
                int[] floatSample => PullSample(streamInlet, ref floatSample),
                short[] floatSample => PullSample(streamInlet, ref floatSample),
                string[] floatSample => PullSample(streamInlet, ref floatSample),
                _ => 0
            };

            return lastTimeStamp;
        }
        
        private static double PullSample(in StreamInlet streamInlet, ref char[] sample) { return streamInlet.pull_sample(sample, 0f); }
        private static double PullSample(in StreamInlet streamInlet, ref double[] sample) { return streamInlet.pull_sample(sample, 0f); }
        private static double PullSample(in StreamInlet streamInlet, ref float[] sample) { return streamInlet.pull_sample(sample, 0f); }
        private static double PullSample(in StreamInlet streamInlet, ref int[] sample) { return streamInlet.pull_sample(sample, 0f); }
        private static double PullSample(in StreamInlet streamInlet, ref short[] sample) { return streamInlet.pull_sample(sample, 0f); }
        private static double PullSample(in StreamInlet streamInlet, ref string[] sample) { return streamInlet.pull_sample(sample, 0f); }
        
        #endif
    }
}