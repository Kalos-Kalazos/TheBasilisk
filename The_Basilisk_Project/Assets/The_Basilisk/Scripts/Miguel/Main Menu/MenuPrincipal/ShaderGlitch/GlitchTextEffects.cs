using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GlitchTextEffect : MonoBehaviour
{
    public Text legacyText; // Referencia al texto Legacy
    private Material textMaterial; // Material del texto

    public float glitchFrequency = 0.1f;
    public float glitchDuration = 0.05f;
    public string originalText = "Texto de Glitch";

    private string currentText = ""; // Texto que se muestra

    private void Start()
    {
        legacyText.text = originalText;
        currentText = originalText;
        textMaterial = legacyText.material; // Obtener el material del texto
        StartCoroutine(ApplyGlitchEffect());
    }

    private IEnumerator ApplyGlitchEffect()
    {
        while (true)
        {
            // Aplicar el glitch con frecuencia definida
            if (UnityEngine.Random.value < glitchFrequency)
            {
                StartCoroutine(GlitchText());
            }

            yield return null;
        }
    }

    private IEnumerator GlitchText()
    {
        // Activar el glitch en el shader
        textMaterial.SetFloat("_GlitchAmount", UnityEngine.Random.Range(0.1f, 0.5f));
        textMaterial.SetFloat("_TimeSpeed", UnityEngine.Random.Range(1.0f, 3.0f));

        // Cambiar el texto aleatoriamente
        string tempText = GetGlitchedText();
        legacyText.text = tempText;

        yield return new WaitForSeconds(glitchDuration);

        // Devolver al estado original
        textMaterial.SetFloat("_GlitchAmount", 0);
        textMaterial.SetFloat("_TimeSpeed", 1.0f);
        legacyText.text = currentText;
    }

    private string GetGlitchedText()
    {
        char[] glitchedTextArray = new char[originalText.Length];
        for (int i = 0; i < originalText.Length; i++)
        {
            if (UnityEngine.Random.value < 0.3f)
            {
                glitchedTextArray[i] = (char)UnityEngine.Random.Range(33, 126); // Caracter aleatorio
            }
            else
            {
                glitchedTextArray[i] = originalText[i]; // Mantener la letra original
            }
        }

        return new string(glitchedTextArray);
    }
}
