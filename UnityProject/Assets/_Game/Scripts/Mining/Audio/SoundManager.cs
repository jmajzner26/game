using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Sounds")]
    [SerializeField] private List<SoundEffect> soundEffects = new List<SoundEffect>();
    [SerializeField] private AudioClip backgroundMusic;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.7f;
    
    private Dictionary<string, SoundEffect> soundDictionary = new Dictionary<string, SoundEffect>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSounds();
        }
chess        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        PlayBackgroundMusic();
    }
    
    private void InitializeSounds()
    {
        soundDictionary.Clear();
        foreach (var sound in soundEffects)
        {
            if (!string.IsNullOrEmpty(sound.name))
            {
                soundDictionary[sound.name] = sound;
            }
        }
        
        if (musicSource != null)
            musicSource.volume = musicVolume;
        
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    public void PlaySound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName) && sfxSource != null)
        {
            SoundEffect sound = soundDictionary[soundName];
            if (sound.clip != null)
            {
                sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume);
            }
        }
    }
    
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume * sfxVolume);
        }
    }
    
    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    public void AddSoundEffect(SoundEffect sound)
    {
        if (!string.IsNullOrEmpty(sound.name))
        {
            soundEffects.Add(sound);
            soundDictionary[sound.name] = sound;
        }
    }
}
