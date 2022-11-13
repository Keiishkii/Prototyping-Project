#if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using LSL;
#endif

using UnityEngine;

namespace _LSL
{
    public class LSLOutput_PositionVector : LSLOutput<float>
    {
        #if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        
        [SerializeField] private Transform _objectTransform;
        
        
        
        private void Awake()
        {
            float sampleRate = (1.0f / Time.fixedDeltaTime);

            StreamInfo streamInfo = new StreamInfo(_streamName, _streamType, 3, sampleRate, channel_format_t.cf_float32);
            XMLElement channels = streamInfo.desc().append_child("channels");
            channels.append_child("channel").append_child_value("label", "x");
            channels.append_child("channel").append_child_value("label", "y");
            channels.append_child("channel").append_child_value("label", "z");

            _outlet = new StreamOutlet(streamInfo);
            _currentSample = new float[3];
        }

        private void FixedUpdate()
        {
            if (_objectTransform == null) return;
            
            Vector3 position = _objectTransform.position;
            _currentSample = new[]
            {
                position.x,
                position.y,
                position.z
            };

            PushOutput(_outlet, _currentSample);
        }

        #endif
    }
};