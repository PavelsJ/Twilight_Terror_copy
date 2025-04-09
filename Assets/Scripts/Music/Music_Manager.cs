using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music_Manager : MonoBehaviour
{
    public static Music_Manager instance;
    
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioClip mainThemeMusic;
    public AudioClip ambientMusic;
    public AudioClip chaseMusic;
    public AudioClip shadowMusic;
    
    private MusicState currentMusicState;
    private MusicState previousMusicState;

    [Header("SFX")] 
    public List<SoundGroup> soundGroups; 
    private Dictionary<SoundType, AudioClip[]> soundDictionary;
    
    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;
    private Coroutine musicTransitionCoroutine;
    
    private enum MusicState
    {
        MainTheme,
        Ambient,
        Chase,
        Shadow
    }
    
    public enum SoundType
    {
        Footsteps,
        Twilight,
        Shake,
        Noise,
        Hurt,
        Warning,
        Hint,
        LightSource,
        LightSwitch,
        PickUp,
        ItemExpire,
        Chest,
        Trap
    }
    
    
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            SceneManager.sceneLoaded += OnSceneChange; 
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeSoundDictionary();
    }

     void Start()
    {
        
        UpdateMusic(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        UpdateMusic(scene, mode);
    }

    private void UpdateMusic(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex is 0 or 1)
            SetMusic(mainThemeMusic, MusicState.MainTheme);
        else
            SetMusic(ambientMusic, MusicState.Ambient);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == musicSource.clip) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySound(SoundType type, float pitch = 1f)
    {
        AudioClip clip = GetRandomClip(type);
        if (clip == null) return;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }

    private void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<SoundType, AudioClip[]>();

        foreach (var group in soundGroups)
        {
            if (!soundDictionary.ContainsKey(group.type))
            {
                soundDictionary.Add(group.type, group.clips);
            }
        }
    }

    private AudioClip GetRandomClip(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out AudioClip[] clips) && clips.Length > 0)
        {
            return clips[Random.Range(0, clips.Length)];
        }
        return null;
    }

    private void SetMusic(AudioClip clip, MusicState state)
    {
        if (musicSource.clip == clip) return;
        
        TransitionToMusic(clip, state);

        previousMusicState = currentMusicState;
        currentMusicState = state;
    }
    
    public void EnterEnemyEncounter()
    {
        if (currentMusicState != MusicState.Shadow && currentMusicState != MusicState.Chase)
        {
            SetMusic(shadowMusic, MusicState.Shadow);
        }
    }
    
    public void ExitEnemyEncounter()
    {
        switch (previousMusicState)
        {
            case MusicState.Ambient:
                SetMusic(ambientMusic, MusicState.Ambient);
                break;
            case MusicState.Chase:
                SetMusic(chaseMusic, MusicState.Chase);
                break;
            default:
                // SetMusic(mainThemeMusic, MusicState.MainTheme);
                break;
        }
    }
    
    public void SetToChaseMusic()
    {
        SetMusic(chaseMusic, MusicState.Chase);
    }

    public void SetToAmbientMusic()
    {
        SetMusic(ambientMusic, MusicState.Ambient);
    }

    public void SetCurrentStateMusic()
    {
        switch (currentMusicState)
        {
            case MusicState.MainTheme:
                SetMusic(mainThemeMusic, MusicState.MainTheme);
                break;
            case MusicState.Ambient:
                SetMusic(ambientMusic, MusicState.Ambient);
                break;
            case MusicState.Chase:
                SetMusic(chaseMusic, MusicState.Chase);
                break;
            case MusicState.Shadow:
                SetMusic(shadowMusic, MusicState.Shadow);
                break;
        }
    }
    
    private void TransitionToMusic(AudioClip newClip, MusicState newState)
    {
        if (musicSource.clip == newClip) return;

        if (musicTransitionCoroutine != null)
            StopCoroutine(musicTransitionCoroutine);

        musicTransitionCoroutine = StartCoroutine(FadeMusic(newClip, newState));
    }
    
    private IEnumerator FadeMusic(AudioClip newClip, MusicState newState)
    {
        float startVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.clip = newClip;
        musicSource.Play();
        currentMusicState = newState;

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }
}

[System.Serializable]
public class SoundGroup
{
    public Music_Manager.SoundType type;
    public AudioClip[] clips;
}
