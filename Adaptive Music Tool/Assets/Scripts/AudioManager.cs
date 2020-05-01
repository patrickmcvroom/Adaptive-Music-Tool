using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Audio;
using System;

[RequireComponent(typeof(AudioSource))]

public class AudioManager : MonoBehaviour
{
    public HeartRateManager heartRateManager;

    public List<AudioClip> trackOne;
    public List<AudioClip> trackTwo;
    public List<AudioClip> trackThree;

    public Button trackOneButton;
    public Button trackTwoButton;
    public Button trackThreeButton;
    public Button stopButton;

    private AudioSource source;
    private int currentIntensity;
    private Tracks currentTrack;

    private enum Tracks {One, Two, Three };

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        trackOneButton.onClick.AddListener(() =>
        {
            PlayMusic(Tracks.One);
        });
        trackTwoButton.onClick.AddListener(() =>
        {
            PlayMusic(Tracks.Two);
        });
        trackThreeButton.onClick.AddListener(() =>
        {
            PlayMusic(Tracks.Three);
        });

        stopButton.onClick.AddListener(source.Stop);
    }

    private void OnDestroy()
    {
        trackOneButton.onClick.RemoveAllListeners();
        trackTwoButton.onClick.RemoveAllListeners();
        trackThreeButton.onClick.RemoveAllListeners();

        stopButton.onClick.RemoveAllListeners();
    }

    private void PlayMusic(Tracks trackName, bool restart = true)
    {
        AudioClip clip = null;

        switch(trackName)
        {
            case Tracks.One:
                if(trackOne.Count == 0)
                {
                    return;
                }
                clip = trackOne[currentIntensity];
                break;
            case Tracks.Two:
                if (trackTwo.Count == 0)
                {
                    return;
                }
                clip = trackTwo[currentIntensity];
                break;
            case Tracks.Three:
                if (trackThree.Count == 0)
                {
                    return;
                }
                clip = trackThree[currentIntensity];
                break;
        }

        currentTrack = trackName;

        PlayAudioClip(clip, restart);
        Debug.Log($"Playing {currentTrack}");
    }

    private void PlayAudioClip(AudioClip clip, bool restart = false)
    {
        if(clip == null)
        {
            return;
        }

        var currentTime = 0f;

        if (source.isPlaying && !restart)
        {
            currentTime = source.time;
            source.Stop();
        }

        source.clip = clip;

        source.time = currentTime;
        source.loop = true;
        source.Play();


    }

    private void Update()
    {
        if (currentIntensity == heartRateManager.intensity)
        {
            return;
        }

        currentIntensity = heartRateManager.intensity;

        PlayMusic(currentTrack, false);
    }
}
