using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchVariedSFX : MonoBehaviour
{
    [SerializeField] private bool usesPlayOneShot = true;
    [SerializeField] private int polyphony = 8;
    [SerializeField] private bool hasPitchVariation = true;

    [Range(0f, .3f)] [SerializeField] private float deviation = 0.0f;

    private AudioSource audioSource;
    private AudioSource sfxPlayer;
    private AudioClip audioClip;
    private List<AudioSource> sources = new List<AudioSource>();
    int playerIndex = 0;
    void UpdateAudioSettings() {
        for (int i = 0; i < gameObject.transform.childCount; i ++) {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }

        audioClip = audioSource.clip;

        sources.Clear();
        if (polyphony > 1) {
            for (int i = 0; i < polyphony; i ++) {
                GameObject childPlayer = new GameObject("SfxPlayer");
                childPlayer.transform.parent = gameObject.transform;
                sfxPlayer = childPlayer.AddComponent<AudioSource>();
                sfxPlayer.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
                sfxPlayer.volume = audioSource.volume;
                sfxPlayer.clip = audioClip;

                sources.Add(sfxPlayer);
            }
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
        }
    }

    public void PlaySFX()
    {
        if (polyphony == 1) {
            if (!usesPlayOneShot) {
                SetPitchVariation(-1);
                audioSource.Play();
            }
            else {
                SetPitchVariation(-1);
                audioSource.PlayOneShot(audioClip);
            }
        }
        else {
            if (!usesPlayOneShot) {
                SetPitchVariation(playerIndex);
                sources[playerIndex].Play();
                //sfxPlayer.pitch = pitch_scale_next;
            }
            else {
                SetPitchVariation(playerIndex);
                sources[playerIndex].PlayOneShot(audioClip);
            }

            playerIndex ++;
            if (playerIndex >= sources.Count) {
                playerIndex = 0;
            }
        }
    }

    void SetPitchVariation(int i) {
        if (!hasPitchVariation) {
            return;
        }

        if (i == -1) {
            audioSource.pitch = 1 + Random.Range(-deviation, deviation);
        }
        else {
            sources[i].pitch = 1 + Random.Range(-deviation, deviation);
        }
    }
}
