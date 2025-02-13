using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Trigger_NextLVL : MonoBehaviour
{
    public P_GameManager gm;
    public GameObject fadeIn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gm.OpenDoorsLVL2();

            if (gm.generatorsPowered == 3)
            {
                gm.elevatorDoor.canBeOpenned = true;
            }
            if (gameObject.CompareTag("NextLVL"))
            {
                StartCoroutine(nameof(FadeIn));
            }
        }
    }

    private IEnumerator FadeIn()
    {
        fadeIn.SetActive(true);

        yield return new WaitForSeconds(2f);

        gm.NextLevel();
    }
}
