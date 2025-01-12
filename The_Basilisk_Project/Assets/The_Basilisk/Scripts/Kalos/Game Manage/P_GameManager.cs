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

    bool doorsOpenned, azazelInteracted;

    P_Azazel_Talk azazel;

    public P_Enviroment_DD basiliskDoor, elevatorDoor, azazelDoor;

    [SerializeField] GameObject UI_GameOver;

    void Start()
    {
        actualScene = ActualSceneID();

        Time.timeScale = 1f;

        pjFlames = player.GetComponent<P_Character_Combat>().flameThrowTank;

        azazel = FindAnyObjectByType<P_Azazel_Talk>();

        if (actualScene == 1)
        {
            //Scripts deactivated
            player.GetComponent<P_Character_HookSwing>().enabled = false;
            player.GetComponent<P_Character_Combat>().hasFlamethrow = false;
            player.GetComponent<P_Character_Combat>().enabled = false;
            player.GetComponent<P_Character_HookGrab>().enabled = false;
            player.GetComponent<PA_Hook>().enabled = false;

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

    private void Update()
    {
        if (azazel != null)
        {
            if (!azazel.talking && !azazel.ended && azazel.onRange)
            {
                Interaction(player);
            }

            if (azazelInteracted && !azazel.talking && !azazel.ended)
            {
                azazel.talking = true;
                azazel.AzazelTalks();
            }
            if (azazelInteracted && azazel.canNext && !azazel.ended)
            {
                azazel.NextLine();
            }
        }
    }
    void Interaction(GameObject player)
    {
        azazelInteracted = player.GetComponent<P_DoorInteraction>().performed;
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
                if (azazel.ended)
                {
                    azazelDoor.canBeOpenned = true;
                    azazelDoor.TriggerDoors();
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
    public void GameOver_UI()
    {
        if (UI_GameOver != null)
        {
            UI_GameOver.SetActive(true);
            Time.timeScale = 0f;
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
