using Hetki.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance;

    private static AudioSource sfx;

    [SerializeField]
    private static Dictionary<string, AudioClip> audioClips;

    private void Awake()
    {
        GetInstance();
    }

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
            catch (System.Exception ex)
            {
                //Could not find GO
                MonoHelper.LogError("GetInstance() Error: " + ex.Message + "\n" + ex.StackTrace);
                return null;
            }
        }
    }

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

    public void PlaySound(Sounds sounds) 
    {
        sfx.PlayOneShot(audioClips[sounds.ToString()]);
    }

}

public enum Sounds 
{
    CardFlip,
    Match,
    Mismatch,
    GameOver
}
