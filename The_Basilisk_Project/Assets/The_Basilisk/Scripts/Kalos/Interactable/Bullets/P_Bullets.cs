using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Bullets : MonoBehaviour
{
    [Header("=== Bullet Settings ===")]
    [SerializeField] float speed;
    [SerializeField] float timeToDeactivate;
    [SerializeField] public float damageBullet;

    Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        StartCoroutine(DeactivateAfterTime());
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(timeToDeactivate);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
