using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    // Referenciar a las armas estas deberian estar asignadas en el inspector de unity
    public P_WeaponController weaponController;

    //Variables para el incremento de cargadores
    public int simpleAmmoIncrease = 1;// Cargadores de arma simple a aumentar

    public int flamethrowerAmmoIncrease = 1; //Cargadores de lanzallamas a aumentar

    private bool isPickedUp = false; //variable para saber si el objeto a sido recogido

    private void OnTriggerEnter(Collider other) // Funcion que se llama cuando el jugador entra en el trigger
    {

        if (other.CompareTag("Player") && !isPickedUp) //Verifica que el objeto que entro es el Player
        {

            if (weaponController != null)
            {

                weaponController.pjCombatManage.ammoSingle += simpleAmmoIncrease;//Aumenta cargadores del arma simple

                weaponController.pjCombatManage.flameMagazine += flamethrowerAmmoIncrease; //Aumenta cargadores lanzallamas

                isPickedUp = true; // marca el Pick Up como recogido

                Destroy(gameObject); // destrulle el Pick Up despues de ser recogido

            }

        }

    }

}
