using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_TakeGrapple : MonoBehaviour
{
    public GameObject fakeGrapple, originalGrapple;

    private void Start()
    {
        originalGrapple = GameObject.FindWithTag("HookPJ");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fakeGrapple.SetActive(false);
            originalGrapple.SetActive(true);
            other.GetComponent<P_Character_HookSwing>().enabled = true;
        }
    }
}
