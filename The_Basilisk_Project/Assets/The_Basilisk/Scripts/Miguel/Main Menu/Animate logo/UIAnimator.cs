using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimator : MonoBehaviour
{
    public Sprite[] sprites; //Array de sprites a animar
    public float frameRate = 0.2f; // Duracion de cada frame

    private Image image;
    private int currentFrame = 0;


    // Start is called before the first frame update
    void Start()
    {
        image =
            GetComponent<Image>();

        StartCoroutine(AnimateSprites());
        


    }
    IEnumerator AnimateSprites()
    {

        while (true)// Animacion en loop
        {

            image.sprite = sprites[currentFrame]; //Cambia el sprite

            currentFrame = (currentFrame + 1) % sprites.Length; //Siguiente frame
            yield return new WaitForSeconds(frameRate); //Espera al siguiente frame

        }

    }
}



           
      
