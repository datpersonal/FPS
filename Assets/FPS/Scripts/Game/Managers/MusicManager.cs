using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance; // Singleton instance
    private AudioSource audioSource;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
