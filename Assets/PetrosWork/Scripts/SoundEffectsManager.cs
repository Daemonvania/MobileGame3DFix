using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance;
    
    [SerializeField] private AudioSource soundObjectPrefab; // Prefab with AudioSource component]
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; }
    }
    
    public void PlaySoundFXClip(AudioClip clip, Transform sourceTransform, float volume = 1.0f)
    {
        if (clip == null) return;

        AudioSource audioSource = Instantiate(soundObjectPrefab, sourceTransform.position, Quaternion.identity);
        
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource, clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
