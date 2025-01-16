using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] sprites; //Array de Sprites

    public Image targetImage; // Referencia al componente Image

    public float frameRate = 0.2f; // Duracion de cada frame

    private int currentFrame;
    private float timer;


    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {

            timer = 0f; currentFrame = (currentFrame + 1) % sprites.Length; // Ciclo entre 3 sprites

            targetImage.sprite = sprites[currentFrame];

        }



    }
}
