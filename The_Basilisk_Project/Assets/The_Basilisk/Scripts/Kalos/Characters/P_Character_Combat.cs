using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_Combat : MonoBehaviour
{
    #region Non visible variables 
    PlayerInput playerInput;
    #endregion

    [Header("Combat Settings")]
    [SerializeField] int currentAmmoSingle;
    [SerializeField] int ammoSingle;
    [SerializeField] int maxAmmoSingle;
    [SerializeField] int ammoFlame;
    [SerializeField] int maxAmmoFlame;
    [SerializeField] float fireCooldown;
    [SerializeField] float fireRate;
    [SerializeField] float flameRate;
    [SerializeField] float flameRange;
    [SerializeField] int damage;
    [SerializeField] Transform shootingPoint;
    [SerializeField] GameObject flameThrowTrigger;

    void Start()
    {
        
    }

    void Update()
    {
        #region --FireCooldown
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        if (fireCooldown < 0) fireCooldown = 0;
        #endregion

        #region --Keep variable limits

        if (fireCooldown < 0) fireCooldown = 0;
        if (currentAmmoSingle < 0) currentAmmoSingle = 0;
        if (ammoSingle < 0) ammoSingle = 0;

        #endregion
    }

    public void Shooting(InputAction.CallbackContext context)
    {
        if (context.started && currentAmmoSingle > 0 && fireCooldown <= 0)
        {
            damage = 10;
            GameObject bullet = P_ObjectPooling.SharedInstance.GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position = shootingPoint.transform.position;
                bullet.transform.rotation = shootingPoint.transform.rotation;
                bullet.GetComponent<P_Bullets>().damageBullet = damage;
                bullet.SetActive(true);
            }
            currentAmmoSingle--;
            fireCooldown = fireRate;
        }
    }

    public void Recharge(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ammoSingle -= 30 - currentAmmoSingle;
            currentAmmoSingle = 30;
        }
    }

    public void FlameThrow(InputAction.CallbackContext context)
    {
        if (context.started && ammoFlame > 0 && fireCooldown <= 0)
        {
            StartCoroutine(Flames());
        }
        else if(context.canceled)
        {
            StopAllCoroutines();
            flameThrowTrigger.SetActive(false);
        }
            
    }

    IEnumerator Flames()
    {
        flameThrowTrigger.SetActive(true);

        while(ammoFlame > 0)
        {
            DealFlameDamage();
            ammoFlame--;
            yield return new WaitForSeconds(flameRate);
        }

        flameThrowTrigger.SetActive(false);
    }

    void DealFlameDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(flameThrowTrigger.transform.position, flameRange);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                //hit.GetComponent<P_Enemy>().TakeDamage(damage);
            }
        }

    }
    private void OnDrawGizmosSelected()
    {
        Color originalColor = Gizmos.color;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(flameThrowTrigger.transform.position, flameRange);

        Gizmos.color = originalColor;
    }
}
