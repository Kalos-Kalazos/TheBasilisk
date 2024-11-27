using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_ObjectPooling : MonoBehaviour
{
    public static P_ObjectPooling SharedInstance;

    [Header("Pool Settings")]
    //Bullets
    [SerializeField]
    List<GameObject> pooledBullets;
    [SerializeField]
    GameObject bulletToPool;
    [SerializeField]
    int amountToPoolB;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledBullets = new List<GameObject>();
        GameObject tmpB;
        for (int i = 0; i < amountToPoolB; i++)
        {
            tmpB = Instantiate(bulletToPool);
            tmpB.SetActive(false);
            pooledBullets.Add(tmpB);
        }

    }
    public GameObject GetPooledBullet()
    {
        for (int i = 0; i < amountToPoolB; i++)
        {
            if (!pooledBullets[i].activeInHierarchy)
            {
                return pooledBullets[i];
            }
        }
        return null;
    }
}
