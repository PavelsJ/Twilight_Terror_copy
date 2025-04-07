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

    [Header("SFX")] 
    public List<SoundGroup> soundGroups; 
    private Dictionary<SoundType, AudioClip[]> soundDictionary;
    
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
        Chest
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
        AudioClip newMusic = scene.buildIndex is 0 or 1 ? mainThemeMusic : ambientMusic;
        
        if (musicSource.clip != newMusic)
        {
            musicSource.clip = newMusic;
            musicSource.Play();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if(clip == musicSource.clip) return;
        
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
}

[System.Serializable]
public class SoundGroup
{
    public Music_Manager.SoundType type;
    public AudioClip[] clips;
}
