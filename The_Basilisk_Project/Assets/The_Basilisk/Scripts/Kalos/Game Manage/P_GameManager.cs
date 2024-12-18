using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class P_GameManager : MonoBehaviour
{
    [Header("=== Game Manager Settings ===")]

    public int actualScene, deadCount;

    public int generatorsPowered;

    public GameObject player, pjWeapon, pjHook, pjFlames;

    public static P_GameManager Instance;

    [SerializeField] P_Enviroment_DD[] doorsToOpen;

    bool doorsOpenned;

    public P_Enviroment_DD basiliskDoor, elevatorDoor;

    void Start()
    {
        actualScene = ActualSceneID();

        pjFlames = player.GetComponent<P_Character_Combat>().flameThrowTank;

        if (actualScene == 1)
        {
            //Scripts deactivated
            player.GetComponent<P_Character_HookSwing>().enabled = false;
            player.GetComponent<P_Character_Combat>().hasFlamethrow = false;
            player.GetComponent<P_Character_Combat>().enabled = false;
            player.GetComponent<P_Character_HookGrab>().enabled = false;

            //Objects deactivated
            pjWeapon.SetActive(false);
            pjHook.SetActive(false);
            pjFlames.SetActive(false);
        }
        else if (actualScene == 2)
        {
            //Scripts Deactivated
            player.GetComponent<P_Character_Combat>().hasFlamethrow = false;

            //Objects deactivated
            pjFlames.SetActive(false);
        }
    }

    public void CheckToOpen()
    {
        switch (actualScene)
        {
            case 1:
                if (deadCount >= 1)
                {
                    elevatorDoor.canBeOpenned = true;
                }
                break;
            case 2:
                if (deadCount >= 6)
                {
                    basiliskDoor.canBeOpenned = true;
                    basiliskDoor.TriggerDoors();
                }
                break;
        }
    }


    public void NextLevel()
    {
        switch (actualScene)
        {
            case 1:
                SceneManager.LoadScene("GL_LvL2");
                break;
            case 2:
                SceneManager.LoadScene("GL_LvL3");
                break;
        }
    }

    public void OpenDoorsLVL2()
    {
        if (!doorsOpenned)
        {
            for (int i = 0; i < doorsToOpen.Length; i++)
            {
                doorsToOpen[i].canBeOpenned = true;
            }

            doorsOpenned = true;
        }
    }


    int ActualSceneID()
    {
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("GL_LvL1")))
        {
            return 1;
        }
        else
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("GL_LvL2")))
        {
            return 2;
        }
        else
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("GL_LvL3")))
        {
            return 3;
        }
        else return 0;
    }
}
