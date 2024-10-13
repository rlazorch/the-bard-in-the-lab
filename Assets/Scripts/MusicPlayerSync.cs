using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class MusicPlayerSync : MonoBehaviour
{
    public AudioClip supertrack;
    public AudioClip death;
    private AudioSource supersource;
    public AudioClip[] subtracks;
    public float defaultVolume = 0.5f;
    public AudioMixerSnapshot[] snapshots;
    public float transitionTime_Death = 0.1f;
    public float transitionTime = 0.3f;
    private List<AudioSource> subsources = new List<AudioSource>();
    public bool stopAllCoroutines = true;
    public bool forceSync = true;
    AudioSource audioSource;
    AudioSource musicSource;
    private float syncTime = 0;
    private bool onFrameOne = true;
    private bool dead = false;
    void UpdateAudioSettings() {
        for (int i = 0; i < gameObject.transform.childCount; i ++) {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }
        subsources.Clear();
        
        GameObject childPlayer;
        
        childPlayer = new GameObject("MusicPlayerSuper");
        childPlayer.transform.parent = gameObject.transform;
        musicSource = childPlayer.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        musicSource.volume = defaultVolume;
        musicSource.clip = supertrack;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        supersource = musicSource;
        

        for (int i = 0; i < subtracks.Length; i ++) {
            childPlayer = new GameObject("MusicPlayer");
            childPlayer.transform.parent = gameObject.transform;
            musicSource = childPlayer.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            musicSource.volume = 0f;
            musicSource.clip = subtracks[i];
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            subsources.Add(musicSource);
        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        UpdateAudioSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateAudioSettings();
            onFrameOne = true;
        }
        if (onFrameOne) {
            supersource.Play();
            for (int i = 0; i < subsources.Count; i ++) {
                subsources[i].Play();
            }
            onFrameOne = false;
        }
        
        syncTime += Time.deltaTime;
        if (syncTime > 0.5)
            syncTime = 0;

        

        if (forceSync) {
            if (syncTime == 0) {
                for (int i = 0; i < subsources.Count; i ++) {
                    subsources[i].timeSamples = supersource.timeSamples;
                }            
            }
        }
    }

    public void playOnDeath() {
        if (!dead) {
            dead = true;
            stopEverything();
            gameObject.GetComponent<AudioSource>().PlayOneShot(death);
        }
    }

    public void swapToSuper() {
        if (subtracks.Length == 0) {
            return;
        }
        if (stopAllCoroutines) {
            StopAllCoroutines();
        }
        for (int i = 0; i < subsources.Count; i ++) {
            StartCoroutine(LerpValueOverTime(subsources[i], 0, transitionTime));
        }
        StartCoroutine(LerpValueOverTime(supersource, defaultVolume, transitionTime));
        //snapshots[0].TransitionTo(transitionTime);
    }
    public void swapToSub(int sub) {
        if (subtracks.Length == 0) {
            return;
        }
        if (stopAllCoroutines) {
            StopAllCoroutines();
        }
        for (int i = 0; i < subsources.Count; i ++) {
            if (i != sub) {
                StartCoroutine(LerpValueOverTime(subsources[i], 0, transitionTime));
            }
        }
        StartCoroutine(LerpValueOverTime(supersource, 0, transitionTime));
        StartCoroutine(LerpValueOverTime(subsources[sub], defaultVolume, transitionTime));
        //snapshots[1].TransitionTo(transitionTime);
    }

    public void stopEverything() {
        if (stopAllCoroutines) {
            StopAllCoroutines();
        }
        for (int i = 0; i < subsources.Count; i ++) {
            StartCoroutine(LerpValueOverTime(subsources[i], 0, 0.1f));
        }
        StartCoroutine(LerpValueOverTime(supersource, 0, 0.1f));
    }

    IEnumerator LerpValueOverTime(AudioSource source, float end, float duration) {
        float startVol = source.volume;
        for (float t = 0f; t < duration; t += Time.deltaTime) {
            source.volume = Mathf.Lerp(startVol, end, t / duration);
            yield return null; // Waits until next frame
        }
        source.volume = end;
    }
}