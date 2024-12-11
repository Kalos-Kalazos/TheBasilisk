using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class P_GameManager : MonoBehaviour
{
    [Header("=== Game Manager Settings ===")]

    public int actualScene;

    GameObject player, pjWeapon, pjHook;

    void Start()
    {
        actualScene = ActualSceneID();

        player = GameObject.FindWithTag("Player");
        pjWeapon = GameObject.FindWithTag("WeaponPJ");
        pjHook = GameObject.FindWithTag("HookPJ");

        if (actualScene == 1)
        {
            //Scripts deactivated
            player.GetComponent<P_Character_HookSwing>().enabled = false;
            player.GetComponent<P_Character_Combat>().enabled = false;

            //Objects deactivated
            pjWeapon.SetActive(false);
            pjHook.SetActive(false);
        }
    }
    int ActualSceneID()
    {
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("GL_LvL1")))
        {
            return 1;
        }
        else
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_Level1")))
        {
            return 2;
        }
        else
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_LevelBoss")))
        {
            return 3;
        }
        else return 0;
    }
}
