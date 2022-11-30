#if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
using LSL;
#endif

using UnityEngine;

namespace _LSL
{
    // Example of an LSL Output Stream, this component us used to write position values of a game object to the network.
    public class LSLOutput_PositionVector : LSLOutput<float>
    {
        #if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        
        [SerializeField] private Transform _objectTransform;
        
        
        
        private void Awake()
        {
            // This is the frequency of the sampling, in the case of this script it is writing the samples in fixed update (though this doesn't happen at 50hz, it physics is tied to this) so sampleRate is calculated as 50hz.
            float sampleRate = (1.0f / Time.fixedDeltaTime);

            // The StreamInfo is a setup class used to populate information within the Outlet, this of which is how we write to the network using LSL.
            // The stream contains 3 channels, one for each of the float values in a position.
            StreamInfo streamInfo = new StreamInfo(_streamName, _streamType, 3, sampleRate, channel_format_t.cf_float32);
            XMLElement channels = streamInfo.desc().append_child("channels");
            
            // Here we are assigning the channel names, these names are what each channel will be labeled as on the receiving machine.
            channels.append_child("channel").append_child_value("label", "x");
            channels.append_child("channel").append_child_value("label", "y");
            channels.append_child("channel").append_child_value("label", "z");

            // Finally we populate the outlet, this will now be visible from any machine on the network.
            _outlet = new StreamOutlet(streamInfo);
        }

        private void FixedUpdate()
        {
            // This just checks if the object we are tracking the position of exists. If not, then we leave the function.
            if (ReferenceEquals(_objectTransform, null)) return;
            
            // Here we grab the current position value of the object and store it in a new variable, this is just for performance reasons.
            // Following this, we then use the base classes (LSLOutput<>) PushOutput function to add it to the list of values we want to publish to the network. (See the base class for more information on this)
            Vector3 position = _objectTransform.position;
            PushOutput(new[]
            {
                position.x,
                position.y,
                position.z
            });
        }

        #endif
    }
};