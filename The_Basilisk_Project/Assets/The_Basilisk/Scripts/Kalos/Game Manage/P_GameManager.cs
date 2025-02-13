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
    [SerializeField] P_AI_Enemy[] enemiesOnScene;

    bool doorsOpenned, azazelInteracted;
    public bool isCombatActive;
    public bool isCombatBasilisk;

    P_Azazel_Talk azazel;

    public P_Enviroment_DD basiliskDoor, elevatorDoor, azazelDoor;

    [SerializeField] GameObject UI_GameOver;

    [SerializeField] Script_AudioManager musicManager;

    void Start()
    {
        actualScene = ActualSceneID();

        pjFlames = player.GetComponent<P_Character_Combat>().flameThrowTank;

        enemiesOnScene = FindObjectsOfType<P_AI_Enemy>();

        Time.timeScale = 1f;
                
        if (actualScene == 2)
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
        else if (actualScene == 3)
        {
            azazel = FindAnyObjectByType<P_Azazel_Talk>();
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

    public void CheckOnCombat()
    {
        isCombatActive = false;
        isCombatBasilisk = false;

        for (int i = 0; i < enemiesOnScene.Length; i++)
        {
            if (enemiesOnScene[i].onCombat && enemiesOnScene[i].health < 200)
            {
                isCombatActive = true;
                break;
            }
            else if (enemiesOnScene[i].onCombat && enemiesOnScene[i].health > 200)
            {
                isCombatBasilisk = true;
                break;
            }
        }

        if (isCombatActive)
        {
            musicManager.EnterCombat();
        }
        else
        {
            musicManager.ExitCombat();
        }

        if (isCombatBasilisk)
        {
            musicManager.BasiliskCombat();
        }
        else
        {
            musicManager.BasiliskExitCombat();
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
            case 2:
                if (player.GetComponent<P_Character_Combat>().enabled == true)
                {
                    elevatorDoor.canBeOpenned = true;
                    for (int i = 0; i < doorsToOpen.Length; i++)
                    {
                        doorsToOpen[i].TriggerDoors();
                    }
                }
                break;
            case 3:
                if (deadCount >= 4)
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
            case 2:
                SceneManager.LoadScene("Def_LvL2");
                break;
            case 3:
                SceneManager.LoadScene("MainMenu");
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
            //UI_GameOver.SetActive(true);
            //Time.timeScale = 0f;
        }
    }

    int ActualSceneID()
    {
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Def_LvL1")))
        {
            return 2;
        }
        else
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Def_LvL2")))
        {
            return 3;
        }
        else
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("MainMenu")))
        {
            return 0;
        }
        else return 1;
    }
}
