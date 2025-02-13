using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_TakeGrapple : MonoBehaviour
{
    public P_GameManager gm;
    P_Trigger_OpenDoors openTrigger;

    public bool openned;

    public GameObject fakeGrapple, originalGrapple, fakeWeapon, originalWeapon, fakeFlames, originalFlames, openDoor, firstDoor;

    private void Start()
    {
        openTrigger = FindAnyObjectByType<P_Trigger_OpenDoors>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("HookTake") && other.CompareTag("Player"))
        {
            originalGrapple = gm.pjHook;
            fakeGrapple.SetActive(false);
            originalGrapple.SetActive(true);
            firstDoor.GetComponent<P_Enviroment_DD>().canBeOpenned = true;
            firstDoor.GetComponent<P_Enviroment_DD>().TriggerDoors();
            other.GetComponent<P_Character_HookSwing>().enabled = true;
            other.GetComponent<P_Character_HookGrab>().enabled = true;
            other.GetComponent<PA_Hook>().enabled = true;
        }

        if (gameObject.CompareTag("WeaponTake") && other.CompareTag("Player"))
        {
            originalWeapon = gm.pjWeapon;
            fakeWeapon.SetActive(false);
            originalWeapon.SetActive(true);
            other.GetComponent<P_Character_Combat>().enabled = true;
            gm.CheckToOpen();
            if (!openned)
            {
                for (int i = 0; i < openTrigger.doorsToOpen.Length; i++)
                {
                    openTrigger.doorsToOpen[i].canBeOpenned = true;
                }
                openDoor.GetComponent<P_Enviroment_DD>().TriggerSingleDoor();
            }

        }

        if (gameObject.CompareTag("FlamesTake") && other.CompareTag("Player"))
        {
            originalFlames = gm.pjFlames;
            fakeFlames.SetActive(false);
            originalFlames.SetActive(true);
            other.GetComponent<P_Character_Combat>().hasFlamethrow = true;
            if (!openned)
                openDoor.GetComponent<P_Enviroment_DD>().canBeOpenned = true;
        }
    }
}
