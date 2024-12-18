using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Trigger_NextLVL : MonoBehaviour
{
    public P_GameManager gm;

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
                gm.NextLevel();
            }
        }
    }
}
