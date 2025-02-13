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

    private AudioClip lastFootstep;
    private float footstepCooldown = 0.2f;
    private float lastFootstepTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitchVariation = 0f)
    {
        if (clip == null) return;
        GameObject audioObject = new GameObject("TempAudio");
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume * globalVolume;
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.spatialBlend = 1f; // 3D Sound
        source.Play();
        Destroy(audioObject, clip.length);
    }

    public void PlayFootstep(Vector3 position)
    {
        if (Time.time - lastFootstepTime < footstepCooldown) return;

        if (footstepSounds.Length > 0)
        {
            AudioClip clip;
            do
            {
                clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            } while (clip == lastFootstep);

            lastFootstep = clip;
            lastFootstepTime = Time.time;
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
