using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBarController : MonoBehaviour
{
    public Slider slider; //referencia al Slider
    public Image barImage; //Referencia al componente Image
    public Sprite[] barSprites; //Array de sprites vacia a llena

    private int currentIndex = 0; //Indice actual del sprite

    // Start is called before the first frame update
    void Start()
    {

        //Configurar el Slider para que actualice la barra automaticamente
        slider.minValue = 0;
        slider.maxValue = barSprites.Length - 1;
        slider.wholeNumbers = true; // Para evitar valores decimales

        slider.onValueChanged.AddListener(UpdateBar);

        //Iniciar la barra
        UpdateBar(slider.value);

    }

    private void UpdateBar(float value)
    {

        currentIndex =
        Mathf.Clamp((int)value, 0, barSprites.Length - 1);//Asegurarse de que el indice sea Valido
        barImage.sprite = barSprites[currentIndex]; //Cambiar el sprite

    }

    public void OnSpriteClicked()
    {

        //Incrementar el indice
        currentIndex = (currentIndex + 1) % barSprites.Length; //Cambiar al siguiente sprite en bucle

        slider.value = currentIndex; //Sincronizar con el slider

        barImage.sprite = barSprites[currentIndex]; //Actualizar la barra


    }
}
