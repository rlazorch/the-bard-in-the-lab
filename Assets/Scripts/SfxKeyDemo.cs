using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SfxKeyDemo : MonoBehaviour
{
    public AudioClip loop;
    public AudioClip sfx;
    private AudioSource musicPlayer;
    private AudioSource sfxPlayer;
    
    float pitch_scale_next = 1f; // Default value (standard pitch)

    /* The AudioSource.pitch property is essentially the speed at which the clip
    will be played. 0.5 means half the speed, and 2.0 means twice the speed.
    Fitting this to chord changes can be tricky because converting between ratios
    and half-steps is a little messy. This can be done in just intonation using
    ratios from the harmonic series, but some of these may sound out-of-tune
    compared to the music loop. 
    
    One approach is to use the twelfth root of 2, which is the ratio of an
    equal-tempered half-step: */
    double twelfth_root = Math.Pow(2, 1/12d);
    /* If we have a sound effect in G major and want it to fit a C major chord,
    that's a difference of 5 half-steps. So, we multiply by the twelfth root of
    5 times: */
    //double myPitch = Math.Pow(twelfth_root, 5);
    /* This is a value of roughly 1.33484d. The just intonation equivalent of this
    is the much cleaner ratio of 4d/3, which is 1.333333333d.
    
    Here is an example chord progression with 3 chords: */

    // float[] pitch_change_times = {0f, 8f, 12f}; // Start times for each pitch change event. First should always be 0f.
    // float[] pitch_change_events = {1f, 4f/3, 0.8f};

    float[] pitch_change_times = {0f, 6.4f}; // Start times for each pitch change event. First should always be 0f.
    float[] pitch_change_events = {1.05946309436f, 1.0f};

    /*
    When using a sound effect in a major key over a minor chord, consider raising the
    pitch by 3 half-steps.
    */
    
    List<float> pitch_change_times_list;
    List<float> pitch_change_events_list;
    int ind = 0;
    float last_time = -1;

    void set_next_pitch_scale(float n) {
	    pitch_scale_next = n;
    }
    
    void Start()
    {
        pitch_change_times_list = new List<float>(pitch_change_times);
        pitch_change_events_list = new List<float>(pitch_change_events);

        // Create two GameObjects, each with an audio source.
        GameObject child1 = new GameObject("MusicPlayer");
        child1.transform.parent = gameObject.transform;
        musicPlayer = child1.AddComponent<AudioSource>();
        musicPlayer.loop = true;
        musicPlayer.clip = loop;
        musicPlayer.Play();
        
        GameObject child2 = new GameObject("SfxPlayer");
        child2.transform.parent = gameObject.transform;
        sfxPlayer = child2.AddComponent<AudioSource>();
        sfxPlayer.clip = sfx;
        sfxPlayer.volume = 0.5f;

        pitch_change_times_list.Add(loop.length + 1);
        //+ 1 is just a safety precaution; this number just needs to be bigger than the length of the loop
    }

    void Update()
    {
        float time = musicPlayer.time;
        //Position in the loop. This can be made more precise by evaluating how long it takes audio to process

        set_next_pitch_scale(pitch_change_events_list[ind]);
        if (time > pitch_change_times_list[ind + 1])
            ind ++;
        
        if (last_time > time)
            ind = 0;
        
        last_time = time;


        if (Input.GetKeyDown("space"))
        {
            sfxPlayer.pitch = pitch_scale_next;
		    sfxPlayer.PlayOneShot(sfx);
            // Using this code with PlayOneShot() will change alter the pitch of all
            // currently playing clips on this AudioSource when the new clip is
            // played. Generally, this is not desirable. One solution is to decide
            // on a maximum polyphony, create that many AudioSources, and cycle
            // among them so that sounds have time to finish playing. Another option
            // is to create one AudioSource for every chord. This is especially
            // convenient if there are a small number of chords.
        }
    }   
}