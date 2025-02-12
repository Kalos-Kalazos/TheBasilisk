using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

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
    [SerializeField] int damage;
    [SerializeField] float damageFlames;
    [SerializeField] Transform shootingPoint;
    public GameObject flameThrowTank;
    [SerializeField] float impulsitoBala;
    public bool hasFlamethrow = false;
    [SerializeField] float burnDamagePerSecond, burnDuration, soundRadius;

    [SerializeField] GameObject muzzleVFX;
    [SerializeField] GameObject hitPointVFX;
    [SerializeField] GameObject hitBloodVFX;
    [SerializeField] VisualEffect flamesVFX;
    [SerializeField] VisualEffect smallFlameVFX;
    [SerializeField] Transform flamesPivot;

    [SerializeField] LayerMask EnemyLayer;
    [SerializeField] Animator animator;

    [SerializeField] bool recharged;

    Transform cam;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }
    private void OnEnable()
    {
        flamesVFX.Stop();
        smallFlameVFX.Stop();
    }

    private void Start()
    {
        wpControl = GetComponent<P_WeaponController>();
        cam = Camera.main?.transform;
        recharged = true;
        flamesVFX.Stop();
        smallFlameVFX.Stop();
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

        if (wpControl.simple)
            smallFlameVFX.Stop();
        else
            smallFlameVFX.Play();
    }

    public void Shooting(InputAction.CallbackContext context)
    {
        if (!this.enabled) return;

        if (!wpControl.simple || !recharged) return;

        if (context.started && currentAmmoSingle > 0 && fireCooldown <= 0)
        {
            Ray ray = new Ray(cam.position, cam.forward);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, shootingRange))
            {
                Debug.DrawRay(ray.origin, ray.direction * shootingRange, Color.red, fireRate);

                GameObject muzzleTemp = Instantiate(muzzleVFX, shootingPoint.position, shootingPoint.rotation);
                muzzleTemp.transform.SetParent(shootingPoint);

                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    GameObject hitBloodTemp = Instantiate(hitBloodVFX, hit.point, shootingPoint.rotation);

                    P_AI_Enemy enemy = hit.collider.GetComponent<P_AI_Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        enemy.pivotVFX.position = hit.point;
                    }

                    Destroy(hitBloodTemp, 2);
                }
                else
                {
                    GameObject hitPointTemp = Instantiate(hitPointVFX, hit.point, shootingPoint.rotation);

                    Destroy(hitPointTemp, 5);
                }

                Destroy(muzzleTemp, 1.5f);
            }

            SoundEmitter();

            currentAmmoSingle--;
            fireCooldown = fireRate;
        }
    }

    public void SoundEmitter()
    {
        //Sound that warn enemies
        Collider[] hitColliders = Physics.OverlapSphere(shootingPoint.position, soundRadius, EnemyLayer);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                P_AI_Enemy enemy = hitCollider.GetComponent<P_AI_Enemy>();
                if (enemy != null)
                {
                    enemy.OnGunshotHeard(transform.position, "Player");
                }
            }
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
                        recharged = false;
                    }
                    else
                    {
                        currentAmmoSingle += ammoSingle;
                        ammoSingle = 0;
                        recharged = false;
                    }
                    if (animator != null)
                    {
                        animator.SetTrigger("Reload");
                    }
                }
                else
                {
                    //No ammo
                }
            }

            if (wpControl.currentAmmoType == P_WeaponController.AmmoType.Flamethrower)
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

    public void RechargedBoolean()
    {
        recharged = true;
    }

    public void FlameThrow(InputAction.CallbackContext context)
    {
        if (!hasFlamethrow || wpControl.simple || !recharged) return;

        if (context.started && currentAmmoFlame > 0 && fireCooldown <= 0)
        {
            SoundEmitter();
            StartCoroutine(Flames());
        }
        else if(context.canceled)
        {
            StopAllCoroutines();
            flamesVFX.Stop();
            smallFlameVFX.Stop();
        }
            
    }

    IEnumerator Flames()
    {
        flamesVFX.Play();
        Debug.Log("Se activan las llamas");

        while (currentAmmoFlame > 0)
        {
            DealFlameDamage();
            currentAmmoFlame--;
            yield return new WaitForSeconds(flameRate);
        }

        flamesVFX.Stop();
    }

    void DealFlameDamage()
    {
        Collider[] hitColliders = Physics.OverlapCapsule(shootingPoint.position, flamesPivot.position, flameRange, EnemyLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                P_AI_Enemy enemy = hitCollider.GetComponent<P_AI_Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damageFlames);
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

        Gizmos.DrawWireSphere(shootingPoint.position, soundRadius);

        Gizmos.color = originalColor;

        Gizmos.DrawWireSphere(flamesPivot.position, flameRange);
    }
}
