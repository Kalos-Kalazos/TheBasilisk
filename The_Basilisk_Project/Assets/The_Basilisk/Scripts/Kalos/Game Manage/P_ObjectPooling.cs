using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_ObjectPooling : MonoBehaviour
{
    public static P_ObjectPooling SharedInstance;

    [Header("=== Pool Settings ===")]
    //Enemy Patrol Point EPP
    [SerializeField]
    List<GameObject> pooledEPP;
    [SerializeField]
    GameObject eppToPool;
    [SerializeField]
    int amountToPoolEPP;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledEPP = new List<GameObject>();
        GameObject tmpEPP;
        for (int i = 0; i < amountToPoolEPP; i++)
        {
            tmpEPP = Instantiate(eppToPool);
            tmpEPP.SetActive(false);
            pooledEPP.Add(tmpEPP);
        }
    }

    public GameObject GetPooledTempPP()
    {
        for (int i = 0; i < amountToPoolEPP; i++)
        {
            if (!pooledEPP[i].activeInHierarchy)
            {
                return pooledEPP[i];
            }
        }
        return null;
    }
}
