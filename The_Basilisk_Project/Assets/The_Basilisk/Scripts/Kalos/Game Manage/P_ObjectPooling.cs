using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_ObjectPooling : MonoBehaviour
{
    public static P_ObjectPooling SharedInstance;

    [Header("=== Pool Settings ===")]
    //Enemy
    [SerializeField]
    List<GameObject> pooledEnemies;
    [SerializeField]
    GameObject enemyToPool;
    [SerializeField]
    int amountToPoolE;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledEnemies = new List<GameObject>();
        GameObject tmpE;
        for (int i = 0; i < amountToPoolE; i++)
        {
            tmpE = Instantiate(enemyToPool);
            tmpE.SetActive(false);
            pooledEnemies.Add(tmpE);
        }

    }
    public GameObject GetPooledBullet()
    {
        for (int i = 0; i < amountToPoolE; i++)
        {
            if (!pooledEnemies[i].activeInHierarchy)
            {
                return pooledEnemies[i];
            }
        }
        return null;
    }
}
