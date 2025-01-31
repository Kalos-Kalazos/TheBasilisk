using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Trigger_OpenDoors : MonoBehaviour
{
    public P_Enviroment_DD[] doorsToOpen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < doorsToOpen.Length; i++)
            {
                if (doorsToOpen[i].canBeOpenned)
                {
                    doorsToOpen[i].TriggerSingleDoor();
                }
            }
        }
    }
}
