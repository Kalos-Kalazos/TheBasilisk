using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class P_SFX_Control : MonoBehaviour
{
    public static P_SFX_Control Instance;

    [Header("General Settings")]
    [Range(0f, 1f)] public float globalVolume = 1f;

    [Header("Audio Clips")]
    public AudioClip[] footstepSounds;
    public AudioClip[] singleDoor;
    public AudioClip[] doubleDoor;
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
            PlaySound(clip, position, 0.7f, 0.8f);
        }
    }
    public void PlaySingleDoorSound(Vector3 position)
    {
        if (footstepSounds.Length > 0)
        {
            AudioClip clip = singleDoor[Random.Range(0, singleDoor.Length)];
            PlaySound(clip, position, 1f, 0.1f);
        }
    }
    public void PlayDoubleDoorSound(Vector3 position)
    {
        if (footstepSounds.Length > 0)
        {
            StartCoroutine(SoundDelay(position));
        }
    }

    private IEnumerator SoundDelay(Vector3 position)
    {
        AudioClip clip1 = doubleDoor[Random.Range(0, doubleDoor.Length)];
        PlaySound(clip1, position);

        yield return new WaitForSeconds(1.2f);

        AudioClip clip2 = doubleDoor[Random.Range(0, doubleDoor.Length)];
        PlaySound(clip2, position);
    }

    public void PlayGunshot(Vector3 position)
    {
        if (gunshotSounds.Length > 0)
        {
            AudioClip clip = gunshotSounds[0];
            PlaySound(clip, position, 1f, 0.3f);
        }
    }

    public void PlayRecharge(Vector3 position)
    {
        if (gunshotSounds.Length > 0)
        {
            AudioClip clip = gunshotSounds[3];
            PlaySound(clip, position, 1f, 0.3f);
        }
    }

    public void PlayImpact(Vector3 position)
    {
        if (impactSounds.Length > 0)
        {
            AudioClip clip = impactSounds[Random.Range(0, impactSounds.Length)];
            PlaySound(clip, position, 0.8f, 0.3f);
        }
    }

    private AudioSource hookLoopSource; // Guardamos el sonido en loop

    public void PlayGunshotHook(Vector3 position)
    {
        if (gunshotSounds.Length > 1)
        {
            AudioClip clip = gunshotSounds[1];
            PlaySound(clip, position, 1f, 0.3f);
        }
    }

    public void PlayLoopHook(Vector3 position)
    {
        if (gunshotSounds.Length > 2)
        {
            AudioClip clip = gunshotSounds[2];
            hookLoopSource = GetAudioSource(); // Guardamos el source
            hookLoopSource.transform.position = position;
            hookLoopSource.clip = clip;
            hookLoopSource.volume = 1f * globalVolume;
            hookLoopSource.loop = true; // Activamos loop
            hookLoopSource.Play();
        }
    }

    public void StopLoopHook()
    {
        if (hookLoopSource != null)
        {
            hookLoopSource.Stop();
            hookLoopSource.loop = false;
            ReturnAudioSource(hookLoopSource); // Devolvemos el source al pool
            hookLoopSource = null;
        }
    }

    public void PlayImpactHook(Vector3 position)
    {
        if (impactSounds.Length > 1)
        {
            AudioClip clip = impactSounds[1];
            PlaySound(clip, position, 1f, 0.3f);
        }
    }

    public void PlayRechargeHook(Vector3 position)
    {
        if (impactSounds.Length > 2)
        {
            AudioClip clip = impactSounds[2];
            PlaySound(clip, position, 1f, 0.3f);
        }
    }
    public void UpdateLoopHookPosition(Vector3 position)
    {
        if (hookLoopSource != null)
        {
            hookLoopSource.transform.position = position;
        }
    }
}