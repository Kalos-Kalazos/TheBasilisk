using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_SFX_Control : MonoBehaviour
{
    public static P_SFX_Control Instance;

    [Header("General Settings")]
    [Range(0f, 1f)] public float globalVolume = 1f;

    [Header("Audio Clips")]
    public AudioClip[] footstepSounds;
    public AudioClip[] gunshotSounds;
    public AudioClip[] impactSounds;

    private Queue<AudioSource> audioPool = new Queue<AudioSource>();
    private int poolSize = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject audioObject = new GameObject("PooledAudio");
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.spatialBlend = 1f; // 3D Sound
            source.playOnAwake = false;
            audioObject.transform.SetParent(transform);
            audioPool.Enqueue(source);
            audioObject.SetActive(false);
        }
    }

    private AudioSource GetAudioSource()
    {
        if (audioPool.Count > 0)
        {
            AudioSource source = audioPool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }
        else
        {
            // Si se necesitan más sonidos, se crean adicionales (pero evita spam infinito)
            GameObject audioObject = new GameObject("ExtraAudio");
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.spatialBlend = 1f;
            return source;
        }
    }

    private void ReturnAudioSource(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
        if (audioPool.Count < poolSize) // Evitar sobrecargar el pool
        {
            audioPool.Enqueue(source);
        }
        else
        {
            Destroy(source.gameObject); // Si hay demasiados, eliminamos los extra
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitchVariation = 0f)
    {
        if (clip == null) return;

        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume * globalVolume;
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.Play();

        StartCoroutine(ReturnToPool(source, clip.length));
    }

    private IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnAudioSource(source);
    }

    public void PlayFootstep(Vector3 position)
    {
        if (footstepSounds.Length > 0)
        {
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            PlaySound(clip, position, 0.7f, 0.1f);
        }
    }

    public void PlayGunshot(Vector3 position)
    {
        if (gunshotSounds.Length > 0)
        {
            AudioClip clip = gunshotSounds[Random.Range(0, gunshotSounds.Length)];
            PlaySound(clip, position, 1f);
        }
    }

    public void PlayImpact(Vector3 position)
    {
        if (impactSounds.Length > 0)
        {
            AudioClip clip = impactSounds[Random.Range(0, impactSounds.Length)];
            PlaySound(clip, position, 0.8f);
        }
    }
}