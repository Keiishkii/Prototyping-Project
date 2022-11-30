#if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
    using _LSL;
    using LSL;
#endif
    using UnityEngine;

public class LSLOutputStream_FloatArray : LSLOutput<float>
{
    #if (PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
    
    private void Awake()
    {
        // This is the frequency of the sampling, in the case of this script it is writing the samples in fixed update (though this doesn't happen at 50hz, it physics is tied to this) so sampleRate is calculated as 50hz.
        float sampleRate = (1.0f / Time.fixedDeltaTime);
            
        // The StreamInfo is a setup class used to populate information within the Outlet, this of which is how we write to the network using LSL.
        // The stream contains 2 channels, this is completely arbitrary and is just used in this case as a demonstration.
        StreamInfo streamInfo = new StreamInfo(_streamName, _streamType, 2, sampleRate, channel_format_t.cf_float32);
        XMLElement channels = streamInfo.desc().append_child("channels");
        
        // Here we are assigning the channel names, these names are what each channel will be labeled as on the receiving machine.
        channels.append_child("channel").append_child_value("label", "value_1");
        channels.append_child("channel").append_child_value("label", "value_2");

        // Finally we populate the outlet, this will now be visible from any machine on the network.
        _outlet = new StreamOutlet(streamInfo);
    }

    private void FixedUpdate()
    {
        // Using the base classes (LSLOutput<>) PushOutput function we can add the values below to the list that will be published to the network. (See the base class for more information on this)
        PushOutput(new[]
        {
            1f,
            2f
        });
    }
    
    #endif
}
