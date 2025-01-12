using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    Spring spring;
    LineRenderer lr;
    P_Character_HookSwing grapplingGun;
    P_Character_HookGrab grabManager;
    PA_Hook hookHeadAnim;
    private Vector3 currentGrapplePosition;
    private Vector3 grapplePoint;

    [Header("=== Rope Animation Settings ===")]
    [SerializeField] int quality;
    [SerializeField] float damper;
    [SerializeField] float strength;
    [SerializeField] float velocity;
    [SerializeField] float waveCount;
    [SerializeField] float waveHeight;
    public AnimationCurve affectCurve;

    // Par�metro para hacer que la cuerda se actualice m�s r�pido en clics r�pidos
    [SerializeField] float fastUpdateMultiplier = 1.5f; // Multiplicador para velocidad de actualizaci�n
    private bool isFastClick = false;
    private float clickTime = 0.3f; // El tiempo m�ximo para considerar un clic r�pido (en segundos)
    private float clickTimer = 0f; // Temporizador para medir la duraci�n del clic

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        grapplingGun = GetComponent<P_Character_HookSwing>();
        grabManager = GetComponent<P_Character_HookGrab>();
        hookHeadAnim = GetComponent<PA_Hook>();

        spring = new Spring();
        spring.SetTarget(0);
    }

    private void Update()
    {
        if (grapplingGun.activeGrapple || hookHeadAnim.returning)
        {
            if (!lr.enabled)
                lr.enabled = true;
        }
        else if (!hookHeadAnim.returning && hookHeadAnim.retracted)
        {
            ResetRope();
        }

        // Aqu� detectamos si el clic fue r�pido y pasamos la informaci�n a la cuerda
        isFastClick = clickTimer < clickTime;  // Depende de c�mo manejes el clic
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void ResetRope()
    {
        currentGrapplePosition = grapplingGun.gunTip.position;
        spring.Reset();
        if (lr.positionCount > 0)
            lr.positionCount = 0;
    }

    public void DrawRope()
    {
        if (!grapplingGun.activeGrapple && !hookHeadAnim.returning)
        {
            ResetRope();
            return;
        }

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        grapplePoint = hookHeadAnim.hookHead.position;

        var gunTipPosition = grapplingGun.gunTip.position;
        currentGrapplePosition = grapplePoint;

        var direction = (grapplePoint - gunTipPosition).normalized;
        var up = Quaternion.LookRotation(direction) * Vector3.up;

        // Si es un clic r�pido, multiplicamos la frecuencia de actualizaci�n de la cuerda
        float updateSpeed = isFastClick ? fastUpdateMultiplier : 1f;

        for (int i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;

            float dynamicWaveHeight = Mathf.Clamp(waveHeight, 0.1f, Vector3.Distance(gunTipPosition, grapplePoint) / 10f);
            var offset = up * dynamicWaveHeight * Mathf.Sin(delta * waveCount * Mathf.PI * spring.Value * affectCurve.Evaluate(delta));

            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta * updateSpeed) + offset); // Aplicamos updateSpeed
        }
    }
}