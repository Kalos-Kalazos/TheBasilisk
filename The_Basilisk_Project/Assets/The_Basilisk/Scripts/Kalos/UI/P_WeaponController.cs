using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class P_WeaponController : MonoBehaviour
{
    public int simpleAmmo = 30; //municion del disparo simple
    public int flamethrowerAmmo = 50; //municion para el lanzallamas

    public P_Character_Combat pjCombatManage;

    public int simpleMagazines = 5; //cargadores disponibles para el disparo simple
    public int flamethrowerMagazines = 3; //cargadores disponibles para el lanzallamas

    //UI Texts
    public Text ammoText; //Muestra la municion
    public Text magazinesText; //Muestra los cargadores disponibles 

    public RectTransform image1; // Referencias al RectTransform de la primera imagent

    public RectTransform image2; // Refeferencia de la segunda imagen

    private Vector2 position1; // Posicion inicial de la primera imagen

    private Vector2 position2; // Position inicial de la segunda imagen

    void Start()
    {
        pjCombatManage = GetComponent<P_Character_Combat>();
        position1 = image1.anchoredPosition;
        position2 = image2.anchoredPosition;

        currentAmmoType = AmmoType.Simple;
        simple = true;
    }

    //Enumeracion para el tipo de municion seleccionada 
    public enum AmmoType
    {
        Simple, Flamethrower
    }
    public AmmoType currentAmmoType = AmmoType.Simple; //Tipo de municion actual


    // Update is called once per frame
    void Update()
    {
        UpdateUI(); //Mostrar la informacion de la UI

        simpleAmmo = pjCombatManage.currentAmmoSingle;
        flamethrowerAmmo = pjCombatManage.currentAmmoFlame;
        simpleMagazines = pjCombatManage.ammoSingle;
        flamethrowerMagazines = pjCombatManage.flameMagazine;
    }

    public void Shooting(InputAction.CallbackContext context)
    {
        if (context.started)
            FireWeapon();
    }

    public bool simple;

    public void ChangeAmmo(InputAction.CallbackContext context)
    {
        if (!pjCombatManage.hasFlamethrow) return;

        if (context.performed && !simple)
        {
            currentAmmoType = AmmoType.Simple;
            simple = true;
        }
        else if (context.performed && simple)
        {
            currentAmmoType = AmmoType.Flamethrower;
            simple = false;
        }
        SwapPositions();
    }

    void SwapPositions()
    {

        // Intercambia las posiciones de las imagenes
        Vector2 temp = image1.anchoredPosition;
        image1.anchoredPosition = image2.anchoredPosition;
        image2.anchoredPosition = temp;

    }

    // funcion para disparar el arma
    void FireWeapon()
    {
        if (currentAmmoType == AmmoType.Simple && simpleAmmo > 0)
        {

            simpleAmmo--; //Aqui agregarias la logica de disparo para disparo simple (Crear Projectile)

        }
        else if(currentAmmoType == AmmoType.Flamethrower && flamethrowerAmmo > 0)
        {

            flamethrowerAmmo--; //aqui agregarias la logica del disparo del lanzallamas (Crear llama continua)

        }
        else
        {
            Debug.Log("No hay municion suficiente.");
        }       

    }

    // Actualizar la UI con la municion y cargadores actuales
    void UpdateUI()
    {
        // Verificar si la municion esta por debajo para cambiar el color

        Color ammoColor = (simpleAmmo <= 4) ? Color.red : Color.black;

        Color flamethrowerColor = (flamethrowerAmmo <= 30) ? Color.red : Color.black;

        // Verificar si los cargadores estan por debajo para cambiar el color

        Color magazinesColor = (simpleMagazines <= 12) ? Color.red : Color.black;

        Color flamethrowerMagazinesColor = (flamethrowerMagazines <= 2) ? Color.red : Color.black;


        if (currentAmmoType == AmmoType.Simple)
        {
            ammoText.text = "-" + simpleAmmo + "-";

            ammoText.color = ammoColor;

            magazinesText.text = "-" + simpleMagazines + "-";

            magazinesText.color = magazinesColor;
        }

        else if (currentAmmoType == AmmoType.Flamethrower)
        {
            ammoText.text = "-" + flamethrowerAmmo + "-";

            ammoText.color = flamethrowerColor;

            magazinesText.text = "-" + flamethrowerMagazines + "-";

            magazinesText.color = flamethrowerMagazinesColor;
        }
    }
}
