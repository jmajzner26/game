using UnityEngine;
using System.Collections;

/// <summary>
/// Music controller for synthwave/chill electronic soundtrack.
/// Manages adaptive music based on game state.
/// </summary>
public class MusicController : MonoBehaviour
{
    [Header("Music Tracks")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip driftMusic;
    [SerializeField] private AudioClip intenseMusic; // For high-speed sections

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource intenseSource; // Separate track for layering

    [Header("Settings")]
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float crossfadeSpeed = 2f;

    private AudioClip currentClip;
    private float targetVolume = 0f;

    private void Awake()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (intenseSource == null && intenseMusic != null)
        {
            intenseSource = gameObject.AddComponent<AudioSource>();
            intenseSource.loop = true;
            intenseSource.volume = 0f;
        }
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic, false);
        if (intenseSource != null)
        {
            StartCoroutine(FadeOut(intenseSource, 1f));
        }
    }

    public void PlayDriftMusic()
    {
        PlayMusic(driftMusic, false);
        if (intenseSource != null && intenseMusic != null)
        {
            intenseSource.clip = intenseMusic;
            intenseSource.Play();
            StartCoroutine(FadeOut(intenseSource, 1f));
        }
    }

    public void SetIntenseMusic(bool active)
    {
        if (intenseSource != null && intenseMusic != null)
        {
            StartCoroutine(active ? FadeIn(intenseSource, musicVolume * 0.7f, 1f) : FadeOut(intenseSource, 1f));
        }
    }

    private void PlayMusic(AudioClip clip, bool crossfade)
    {
        if (clip == null || clip == currentClip) return;

        if (crossfade && currentClip != null && musicSource.isPlaying)
        {
            StartCoroutine(CrossfadeToClip(clip));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.Play();
            currentClip = clip;
        }
    }

    private IEnumerator CrossfadeToClip(AudioClip newClip)
    {
        float fadeTime = 1f / crossfadeSpeed;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            musicSource.volume = Mathf.Lerp(musicVolume, 0f, t);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();
        currentClip = newClip;

        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t);
            yield return null;
        }
    }

    private IEnumerator FadeIn(AudioSource source, float targetVolume, float duration)
    {
        if (!source.isPlaying)
        {
            source.Play();
        }

        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void Pause()
    {
        musicSource.Pause();
        if (intenseSource != null) intenseSource.Pause();
    }

    public void Resume()
    {
        musicSource.UnPause();
        if (intenseSource != null) intenseSource.UnPause();
    }
}

