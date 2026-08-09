using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    public static AudioManager Instance { get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(SoundDataSO soundData)
    {
        if(soundData == null || soundData.clip == null) return;

        switch (soundData.type)
        {
            case AudioType.SFX:
                sfxSource.PlayOneShot(soundData.clip, soundData.volume);
                break;
            case AudioType.Music:
                musicSource.clip = soundData.clip;
                musicSource.volume = soundData.volume;
                musicSource.Play();
                break;
        }

    }
}