using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{

    public int simpleAmmo = 30; //municion del disparo simple
    public int flamethrowerAmmo = 50; //municion para el lanzallamas
    

    public int simpleMagazines = 5; //cargadores disponibles para el disparo simple
    public int flamethrowerMagazines = 3; //cargadores disponibles para el lanzallamas

    //UI Texts
    public Text ammoText; //Muestra la municion
    public Text magazinesText; //Muestra los cargadores disponibles 

    //Enumeracion para el tipo de municion seleccionada 
    public enum AmmoType
    {
        Simple, Flamethrower
    }
    public AmmoType currentAmmoType = AmmoType.Simple; //Tipo de municion actual

    //Variables para el disparo Continuo
    private bool isFiring = false; //Indicador de si el lanzallamas esta disparando
    public float flamethrowerFireRate = 0.1f; //frecuencia de disparo (tiempo entre disparos)

    private float nextFireTime = 0f; // Tiempo cuando se podra disparar nuevamente

    public RectTransform image1; // Referencias al RectTransform de la primera imagent

    public RectTransform image2; // Refeferencia de la segunda imagen

    private Vector2 position1; // Posicion inicial de la primera imagen

    private Vector2 position2; // Position inicial de la segunda imagen

    void Start()
    {

        //simpleAmmo = player.GetComponent<P_Character_Combat>().currentAmmoSingle;

        // Guardamos las posiciones iniciales

        position1 = image1.anchoredPosition;
        position2 = image2.anchoredPosition;

    }

    

    // Update is called once per frame
    void Update()
    {
        UpdateUI(); //Mostrar la informacion de la UI

        if (Input.GetKeyDown(KeyCode.Tab)) //Cambiar entre los tipos de municion con la tecla tabulador
        {
            SwapPositions();
            if (currentAmmoType == AmmoType.Simple) currentAmmoType = AmmoType.Flamethrower;
            if (currentAmmoType == AmmoType.Flamethrower) currentAmmoType = AmmoType.Simple;
            

        }
        if (Input.GetButtonDown("Fire1"))//Disparar dependiendo del tipo de municion
        {
            //Por defecto el boton disparo es el clic izquierdo del raton
            FireWeapon();

        }

        // Activar o desactivar disparo continuo para el lanzallamas
        if (currentAmmoType == AmmoType.Flamethrower)
        {

            if (Input.GetButton("Fire1") && Time.time >= nextFireTime && flamethrowerAmmo > 0)
            {

                FireFlamethrower();

            }
            if (Input.GetButtonUp("Fire1")) //Si se suelta el boton de disparo, deja de disparar
            {

                isFiring = false;

            }

        }

        if (Input.GetKeyDown(KeyCode.R))//Recargar cargadores con R
        {

            Reload();

        }

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

    void FireFlamethrower() //Funcion para disparar el lanzallamas de forma continua
    {

        if (flamethrowerAmmo > 0)
        {

            isFiring = true; //Iniciar el disparo Continuo

            flamethrowerAmmo--; //Consumir municion del lanzallamas

            nextFireTime = Time.time + flamethrowerFireRate; //Logica de disparo Continuo cada vez que se cumpla la tasa de disparo

            //Aqui puedes aguegrar la logica visual del disparo del lanzallamas (crear llama o emitir particulas


            Debug.Log("Disparando lanzallamas" + flamethrowerAmmo);
        }
        else
        {

            Debug.Log("No hay municion del lanzallamas");

        }



    }

    //Funcion de recarga
    void Reload()
    {

        if (currentAmmoType == AmmoType.Simple && simpleMagazines > 0)
        {

            if (simpleAmmo < 30) // Capacidad maxima de municion)
            {

                int ammoToReload = Mathf.Min(30 - simpleAmmo, 30); //Recarga hasta 30 balas o el maximo posible
                simpleAmmo += ammoToReload;

                simpleMagazines--; //Usar un cargador

                Debug.Log("Recargando" + ammoToReload);
            
            }
            else
            {

                Debug.Log("La municion esta llena");

            }
            

        }
        else if (currentAmmoType == AmmoType.Flamethrower && flamethrowerMagazines > 0)
        {

            if (flamethrowerAmmo < 50)// capacidad maxima de municion para lanzallamas
            {

                int ammoToReload = Mathf.Min(50 - flamethrowerAmmo, 10); // Recarga hasta 10 unidades o el maximo posible

                flamethrowerAmmo += ammoToReload;

                flamethrowerMagazines--; // Usar un cargador

                Debug.Log("Recargado Lanzallamas" + ammoToReload);


            }
            else
            {

                Debug.Log("No hay cargadores");

            }

        }

    }

    // Actualizar la UI con la municion y cargadores actuales
    void UpdateUI()
    {
        // Verificar si la municion esta por debajo o igual a 10 para cambiar el color

        Color ammoColor = (simpleAmmo <= 10) ? Color.red : Color.black;

        Color flamethrowerColor = (flamethrowerAmmo <= 10) ? Color.red : Color.black;

        // Verificar si los cargadores estan por debajo o igual a 3 para cambiar el color

        Color magazinesColor = (simpleMagazines <= 3) ? Color.red : Color.black;

        Color flamethrowerMagazinesColor = (flamethrowerMagazines <= 3) ? Color.red : Color.black;


        if (currentAmmoType == AmmoType.Simple)
        {

            ammoText.text = "0" + simpleAmmo;

            ammoText.color = ammoColor;

            magazinesText.text = "0" + simpleMagazines;

            magazinesText.color = magazinesColor;

        }
        else
        {

            ammoText.text = "0" + flamethrowerAmmo;

            ammoText.color = flamethrowerColor;

            magazinesText.text = "0" + flamethrowerMagazines;

            magazinesText.color = flamethrowerMagazinesColor;

        }


    }



}
