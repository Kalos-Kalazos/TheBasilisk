using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Damage_Ref : MonoBehaviour
{
    P_AI_Enemy enemyRef;

    void Start()
    {
        enemyRef = GetComponentInParent<P_AI_Enemy>();
    }

    public void DoDamages()
    {
        enemyRef.TryApplyDamage();
    }
}
