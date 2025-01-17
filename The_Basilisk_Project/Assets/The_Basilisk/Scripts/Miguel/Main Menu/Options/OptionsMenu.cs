using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider effectsSlider;
    public Slider musicSlider;

    [Header("Audio Sources")]
    public AudioSource effectAudioSource; //Audio para efectos
    public AudioSource musicAudioSource; // Audio para musica


    [Header("Back Button")]
    public GameObject mainMenu; //Referencia al Menu principal
    public GameObject optionsMenu; // Referencia al menu de opciones


    // Start is called before the first frame update
    void Start()
    {
        // Cargar Valores guardados o inicializamos valores por defecto

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        // Asignar valores Iniciales

        UpdateMasterVolume();
        UpdateEffectsVolume();
        UpdateMusicVolume();

    }

    public void UpdateMasterVolume()
    {

        float volume = masterSlider.value;
        AudioListener.volume = volume;

        PlayerPrefs.SetFloat("MasterVolume", volume); //Guarda el valor


    }

    public void UpdateEffectsVolume()
    {

        float volume = effectsSlider.value;

        if( effectAudioSource !=null) 
        
            effectAudioSource.volume = volume;

        PlayerPrefs.SetFloat("EffectsVolume", volume); //Guarda el valor

        

    }


   public void UpdateMusicVolume()
   {

        float volume = musicSlider.value;

        if (musicAudioSource != null)

            musicAudioSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume); //Guarda el valor


   }


    public void BackToMainMenu()
    {
        // Guarda los valores de los sliders antes de salir
        PlayerPrefs.Save();



        // Cambia al menu principal

        optionsMenu.SetActive(false);

        mainMenu.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
