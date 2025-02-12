using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Audio;

public class Script_AudioManager : MonoBehaviour
{
    #region SingleTon
    private static Script_AudioManager instance;
    public static Script_AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No hay AudioManager!");
            }
            return instance;
        }
    }

    #endregion

    //Declaracion de todos los valores de la base de datos (Todos los valores han de ser PUBLIC)

    //Llamada sin referencia: Script_AudioManager.Instance.PlaySFX(1);

    [Header("=== Audio Source References ===")]
    public AudioSource ambienceMusic;
    public AudioSource combatMusic;
    public AudioMixer audioMixer;

    public bool isInCombat = false;

    [Range(0f, 1f)] public float globalSFXVolume = 1.0f;

    private void Start()
    {
        ambienceMusic.Play();
    }
    public void EnterCombat()
    {
        if (!combatMusic.isPlaying) 
        {
            combatMusic.Play();
        }

        if (!isInCombat)
        {
            isInCombat = true;
            StopAllCoroutines();
            StartCoroutine(FadeMusic("NormalMusic", "CombatMusic", 1.5f));
        }
    }

    public void ExitCombat()
    {
        if (!ambienceMusic.isPlaying)
        {
            ambienceMusic.Play();
        }

        if (isInCombat)
        {
            isInCombat = false;
            StopAllCoroutines();
            StartCoroutine(FadeMusic("CombatMusic", "NormalMusic", 2f));
        }
    }

    private IEnumerator FadeMusic(string fadeOutGroup, string fadeInGroup, float duration)
    {
        float startVolume, endVolume;
        audioMixer.GetFloat(fadeOutGroup, out startVolume);
        audioMixer.GetFloat(fadeInGroup, out endVolume);

        float timer = 0;
        while (timer < duration)
        {
            float newVolumeOut = Mathf.Lerp(startVolume, -80f, timer / duration);
            float newVolumeIn = Mathf.Lerp(endVolume, 0f, timer / duration);

            audioMixer.SetFloat(fadeOutGroup, newVolumeOut);
            audioMixer.SetFloat(fadeInGroup, newVolumeIn);

            timer += Time.deltaTime;
            yield return null;
        }

        audioMixer.SetFloat(fadeOutGroup, -80f);
        audioMixer.SetFloat(fadeInGroup, 0f);

        if (isInCombat)
        {
            ambienceMusic.Stop();
        }
        else
        {
            combatMusic.Stop();
        }
    }

    //En un Singleton podemos declarar cualquier ACCION LLAMABLE siempre y cuando sea PUBLIC
    #region Music Methods

    #endregion

    #region SFX Methods


    #endregion
}
