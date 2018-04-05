using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BattleSoundsManager : MonoBehaviour
{
    static private BattleSoundsManager _instance;

    static public BattleSoundsManager instance
    {
        get
        {
            if (!initialized)
            {
                _instance = GameObject.FindObjectOfType<BattleSoundsManager>();
                initialized = true;
            }
            return _instance;
        }
    }

    private static bool initialized = false;
    private static bool audioSourceInitialized = false;
    public List<AudioClip> walkingSound = new List<AudioClip>();
    public List<AudioClip> damageSounds = new List<AudioClip>();
    public List<AudioClip> swingSounds = new List<AudioClip>();
    public List<AudioClip> deathSounds = new List<AudioClip>();
    private AudioSource _soundEffectPlayer;

    private void Awake()
    {
        initialized = false;
        audioSourceInitialized = false;
    }

    public void playSound(AudioClip clip, float value, int pitch)
    {
        soundEffectPlayer.pitch = pitch;
        soundEffectPlayer.PlayOneShot(clip, value);
    }

    public void playSound(AudioClip clip, float value)
    {
        soundEffectPlayer.pitch = 1;
        soundEffectPlayer.PlayOneShot(clip, value);
    }

    public AudioSource soundEffectPlayer
    {
        get
        {
            if (!audioSourceInitialized)
            {
                _soundEffectPlayer = GetComponent<AudioSource>();
                audioSourceInitialized = true;
            }
            return _soundEffectPlayer;
        }
    }
}