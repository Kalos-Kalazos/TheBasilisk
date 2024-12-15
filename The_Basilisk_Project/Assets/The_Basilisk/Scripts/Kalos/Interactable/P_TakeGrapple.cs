using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_TakeGrapple : MonoBehaviour
{
    public P_GameManager gm;

    bool openned;

    public GameObject fakeGrapple, originalGrapple, fakeWeapon, originalWeapon, fakeFlames, originalFlames, openDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("HookTake") && other.CompareTag("Player"))
        {
            originalGrapple = gm.pjHook;
            fakeGrapple.SetActive(false);
            originalGrapple.SetActive(true);
            other.GetComponent<P_Character_HookSwing>().enabled = true;
            other.GetComponent<P_Character_HookGrab>().enabled = true;
        }

        if (gameObject.CompareTag("WeaponTake") && other.CompareTag("Player"))
        {
            originalWeapon = gm.pjWeapon;
            fakeWeapon.SetActive(false);
            originalWeapon.SetActive(true);
            other.GetComponent<P_Character_Combat>().enabled = true;
            if(!openned)
                StartCoroutine(nameof(OpenTheDoor));
        }

        if (gameObject.CompareTag("FlamesTake") && other.CompareTag("Player"))
        {
            originalFlames = gm.pjFlames;
            fakeFlames.SetActive(false);
            originalFlames.SetActive(true);
            other.GetComponent<P_Character_Combat>().hasFlamethrow = true;
        }
    }

    private IEnumerator OpenTheDoor()
    {
        openned = true;
        float forwardDistance = 0.1f;
        float leftDistance = 1.5f;
        float duration = 4f;
        float elapsedTime = 0f;

        Vector3 startPosition = openDoor.transform.position;
        Vector3 forwardPosition = startPosition + openDoor.transform.forward * forwardDistance;
        Vector3 finalPosition = forwardPosition + openDoor.transform.right * leftDistance;

        while (elapsedTime < duration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2f);
            openDoor.transform.position = Vector3.Lerp(startPosition, forwardPosition, t);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < duration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2f);
            openDoor.transform.position = Vector3.Lerp(forwardPosition, finalPosition, t);
            yield return null;
        }
    }
}
