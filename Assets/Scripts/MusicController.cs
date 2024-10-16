// This demo is based on sample code from https://docs.unity3d.com/ScriptReference/AudioSource.PlayScheduled.html

using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip myClip;
    
    float paddingTime = 2f;
    // This is the time we allow for Unity to load the
    // AudioClip. It should probably be at least 1f.
    
    private double adjustMP3 = 0.01f;
    // MP3 files have a small amount of silence, around
    // 10ms to 50ms, at the beginning because of how they
    // are compressed. As such, it makes seamless looping
    // difficult. However, if we account for this each time
    // we schedule a new iteration of the loop, we can smooth
    // this over cleanly. Some experimentation may be
    // required to find the right value.
    
    private int numberOfSources = 2;
    // This is the number of audio sources Unity is
    // actually using. For a basic looping setup,
    // this should be more than sufficient.
    
    private int index = 0;
    private double nextEventTime;
    private AudioSource[] audioSources = new AudioSource[2];
    
    void SetNextTimeEvent(double from) {
        nextEventTime = from + myClip.length - adjustMP3;
        // Depending on your file format and the quality of the
        // trimming on your audio, myClip.length may be slightly
        // longer than is desired. One way to calculate the duration
        // more accurately is to use the tempo and number of beats
        // in the loop:
        //     (60.0 / bpm) * numBeatsPerSegment
        // You can also make this adjustment using adjustMP3
        // (See the variable definition above for details).
    }
    
    void Start()
    {
        // Create two GameObjects, each with an audio source.
        for (int i = 0; i < numberOfSources; i++)
        {
            GameObject child = new GameObject("Player");
            child.transform.parent = gameObject.transform;
            audioSources[i] = child.AddComponent<AudioSource>();
        }

        // We set the music to begin playing in 1 second.
        nextEventTime = AudioSettings.dspTime + 1f;
        
        // If your loop has an intro, you can also play that
        // on a separate AudioSource and schedule the loop
        // proper to begin playing so that the intro and loop
        // seamlessly dovetail. If that is the case, replace
        // 1f with the duration of this intro (assuming the
        // intro begins immediately).
    }

    void Update()
    {
        double time = AudioSettings.dspTime;
        // For audio, which typically operates at 44.1 kHz or above, Time.time (which
        // is a single-precision float) is not precise enough. For more details, see:
        // https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html

        if (time + paddingTime > nextEventTime)
        {
            // We are now paddingTime seconds before the time at which the sound should
            // play, so we will schedule it now in order for the system to have enough
            // time to prepare the audio for playback.
            
            audioSources[index].clip = myClip;
            audioSources[index].PlayScheduled(nextEventTime);

            SetNextTimeEvent(nextEventTime);
            //Debug.Log("Scheduled source " + index + " to start at time " + nextEventTime);

            // Cycle to the next audio source so that the loading process of one does
            // not interfere with the one that's playing currently.
            index = 1 - index;
        }
    }
}