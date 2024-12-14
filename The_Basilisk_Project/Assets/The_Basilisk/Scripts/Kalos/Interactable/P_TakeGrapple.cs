using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_TakeGrapple : MonoBehaviour
{
    public P_GameManager gm;

    public GameObject fakeGrapple, originalGrapple, fakeWeapon, originalWeapon, fakeFlames, originalFlames;

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("HookTake"))
        {
            if (other.CompareTag("Player"))
            {
                if (gameObject.CompareTag("HookTake"))
                    originalGrapple = gm.pjHook;
                fakeGrapple.SetActive(false);
                originalGrapple.SetActive(true);
                other.GetComponent<P_Character_HookSwing>().enabled = true;
                other.GetComponent<P_Character_HookGrab>().enabled = true;
            }
        }

        if (gameObject.CompareTag("WeaponTake"))
        {
            if (other.CompareTag("Player"))
            {
                if (gameObject.CompareTag("WeaponTake"))
                    originalWeapon = gm.pjWeapon;
                fakeWeapon.SetActive(false);
                originalWeapon.SetActive(true);
                other.GetComponent<P_Character_Combat>().enabled = true;
            }
        }

        if (gameObject.CompareTag("FlamesTake"))
        {
            if (other.CompareTag("Player"))
            {
                if (gameObject.CompareTag("FlamesTake"))
                    originalFlames = gm.pjFlames;
                fakeFlames.SetActive(false);
                originalFlames.SetActive(true);
                other.GetComponent<P_Character_Combat>().hasFlamethrow = true;
            }
        }
    }
}
