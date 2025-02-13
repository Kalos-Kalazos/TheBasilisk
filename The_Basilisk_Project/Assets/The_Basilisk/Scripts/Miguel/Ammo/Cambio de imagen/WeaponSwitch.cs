using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{

    public RectTransform image1; // Referencias al RectTransform de la primera imagent

    public RectTransform image2; // Refeferencia de la segunda imagen

    private Vector2 position1; // Posicion inicial de la primera imagen

    private Vector2 position2; // Position inicial de la segunda imagen


    // Start is called before the first frame update
    void Start()
    {

        // Guardamos las posiciones iniciales

        position1 = image1.anchoredPosition;
        position2 = image2.anchoredPosition;

    }

    // Update is called once per frame
    void Update()
    {
        
        // Cambiar de arma con las

    }
}
