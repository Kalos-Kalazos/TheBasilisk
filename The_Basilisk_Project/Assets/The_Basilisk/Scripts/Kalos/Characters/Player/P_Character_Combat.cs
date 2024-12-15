using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_Combat : MonoBehaviour
{
    #region Non visible variables 
    P_WeaponController wpControl;
    #endregion

    [Header("Combat Settings")]
    public int currentAmmoSingle;
    public int ammoSingle;
    [SerializeField] float fireRate;
    [SerializeField] float shootingRange;
    public int flameMagazine;
    public int currentAmmoFlame;
    public int maxAmmoFlame;
    [SerializeField] float fireCooldown;
    [SerializeField] float flameRate;
    [SerializeField] float flameRange;
    [SerializeField] float flameDistance;
    public int damage;
    [SerializeField] Transform shootingPoint;
    [SerializeField] GameObject flameThrowTrigger;
    public GameObject flameThrowTank;
    [SerializeField] float impulsitoBala;
    public bool hasFlamethrow = false;
    [SerializeField] float burnDamagePerSecond, burnDuration;

    [SerializeField] LayerMask EnemyLayer;

    Transform cam;

    private void Start()
    {
        wpControl = GetComponent<P_WeaponController>();
        cam = Camera.main?.transform;
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
        if (!wpControl.simple) return;

        if (context.started && currentAmmoSingle > 0 && fireCooldown <= 0)
        {
            Ray ray = new Ray(cam.position, cam.forward);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, shootingRange))
            {
                Debug.DrawRay(ray.origin, ray.direction * shootingRange, Color.red, fireRate);

                if(hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    P_AI_Enemy enemy = hit.collider.GetComponent<P_AI_Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
                //else VFX Bujero de bala

            }

            currentAmmoSingle--;
            fireCooldown = fireRate;

            //Sound that warn enemies
            P_GameManager.OnGunshot(transform.position, "Player");

        }
    }

    public void Recharge(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (wpControl.currentAmmoType == P_WeaponController.AmmoType.Simple)
            {
                if (ammoSingle > 0)
                {
                    int ammoNeeded = 12 - currentAmmoSingle;

                    if (ammoSingle >= ammoNeeded)
                    {
                        ammoSingle -= ammoNeeded;
                        currentAmmoSingle = 12;
                    }
                    else
                    {
                        currentAmmoSingle += ammoSingle;
                        ammoSingle = 0;
                    }
                }
                else
                {
                    //No ammo
                }
            }
            else if (wpControl.currentAmmoType == P_WeaponController.AmmoType.Flamethrower)
            {
                if (flameMagazine > 0 && currentAmmoFlame < maxAmmoFlame)
                {
                    int ammoNeeded = maxAmmoFlame - currentAmmoFlame;
                    currentAmmoFlame += ammoNeeded;
                    flameMagazine--;
                }
                else if (flameMagazine <= 0)
                {
                    //No ammo
                }
                else
                {
                    //amo full
                }
            }

        }
    }

    public void FlameThrow(InputAction.CallbackContext context)
    {
        if (!hasFlamethrow || wpControl.simple) return;

        if (context.started && currentAmmoFlame > 0 && fireCooldown <= 0)
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

        while(currentAmmoFlame > 0)
        {
            DealFlameDamage();
            currentAmmoFlame--;
            yield return new WaitForSeconds(flameRate);
        }

        flameThrowTrigger.SetActive(false);
    }

    void DealFlameDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(flameThrowTrigger.transform.position, flameRange, EnemyLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                P_AI_Enemy enemy = hitCollider.GetComponent<P_AI_Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    ApplyBurnEffect(hitCollider);
                }
            }
        }
    }

    private void ApplyBurnEffect(Collider hitCollider)
    {
        P_AI_Enemy enemy = hitCollider.GetComponent<P_AI_Enemy>();
        if (enemy != null)
        {
            enemy.ApplyBurnEffect(burnDuration, burnDamagePerSecond);
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
