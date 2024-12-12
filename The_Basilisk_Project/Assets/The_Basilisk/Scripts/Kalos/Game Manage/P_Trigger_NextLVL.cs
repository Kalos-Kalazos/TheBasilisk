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
            gm.NextLevel();
        }
    }
}
