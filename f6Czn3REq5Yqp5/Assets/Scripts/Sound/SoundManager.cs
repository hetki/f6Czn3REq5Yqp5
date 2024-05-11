using Hetki.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Sound Manager
/// </summary>
public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance;

    private static AudioSource sfx;

    [SerializeField]
    private static Dictionary<string, AudioClip> audioClips;

    /// <summary>
    /// SoundManager Pre-Initialization sequence
    /// </summary>
    private void Awake()
    {
        GetInstance();
    }

    /// <summary>
    /// Get the SoundManager instance
    /// </summary>
    /// <returns>SoundManager</returns>
    public static SoundManager GetInstance()
    {
        if (Instance != null)
            return Instance;
        else
        {
            try
            {
                Instance = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
                Instance.Initialize();
                return Instance;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Initialize SoundManager
    /// </summary>
    private void Initialize()
    {
        sfx = transform.Find("SFX").GetComponent<AudioSource>();
        AudioClip[] loadedClips = Resources.LoadAll<AudioClip>("Sounds/");
        //Load into dictionary to prevent unnecessary iteration
        audioClips = loadedClips.Select((value, index) => new { value, index })
                      .ToDictionary(pair => pair.value.name, pair => pair.value);
        //Keep GameManager 'alive' for persistence
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Play sound on sfx audioSource
    /// </summary>
    /// <param name="sounds"></param>
    public void PlaySound(Sounds sounds) 
    {
        sfx.PlayOneShot(audioClips[sounds.ToString()]);
    }

}

/// <summary>
/// SoundClip names as enums to prevent code errors
/// </summary>
public enum Sounds 
{
    CardFlip,
    Match,
    Mismatch,
    GameOver
}
