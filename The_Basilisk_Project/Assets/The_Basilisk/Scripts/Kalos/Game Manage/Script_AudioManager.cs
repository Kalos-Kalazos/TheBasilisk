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
    public AudioSource organMusic;
    public AudioSource metalMusic;
    public AudioMixer audioMixer;

    public bool isInCombat = false;
    public bool basiliskInCombat = false;
    public bool isInRangeB = false;


    private void Start()
    {
        ambienceMusic.Play();
        audioMixer.SetFloat("OrganMusic", -40f);
        organMusic.Play();
        audioMixer.SetFloat("CombatMusic", -40f);
        combatMusic.Play();
        audioMixer.SetFloat("BasiliskMusic", -80f);
        metalMusic.Play();
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
            StartCoroutine(FadeMusic("NormalMusic", "CombatMusic", 0.5f));
            audioMixer.SetFloat("OrganMusic", -40f);
        }
    }

    public void BasiliskCombat()
    {
        if (!basiliskInCombat && isInRangeB)
        {
            basiliskInCombat = true;
            StopAllCoroutines();
            StartCoroutine(FadeMusic("OrganMusic", "BasiliskMusic", 0.5f));
            audioMixer.SetFloat("CombatMusic", -10f);
        }
    }
    public void BasiliskExitCombat()
    {
        if (basiliskInCombat && isInRangeB)
        {
            basiliskInCombat = false;
            StopAllCoroutines();
            StartCoroutine(FadeMusic("BasiliskMusic", "OrganMusic", 0.5f));
            audioMixer.SetFloat("NormalMusic", -20f);
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
            StartCoroutine(FadeMusic("CombatMusic", "NormalMusic", 0.5f));
            audioMixer.SetFloat("BasiliskMusic", -80f);
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
            float newVolumeOut = Mathf.Lerp(startVolume, -40f, timer / duration);
            float newVolumeIn = Mathf.Lerp(endVolume, 0f, timer / duration);

            audioMixer.SetFloat(fadeOutGroup, newVolumeOut);
            audioMixer.SetFloat(fadeInGroup, newVolumeIn);

            timer += Time.deltaTime;
            yield return null;
        }

        audioMixer.SetFloat(fadeOutGroup, -40f);
        audioMixer.SetFloat(fadeInGroup, 0f);
    }

    #region Music Methods
    #endregion

    #region SFX Methods


    #endregion
}
