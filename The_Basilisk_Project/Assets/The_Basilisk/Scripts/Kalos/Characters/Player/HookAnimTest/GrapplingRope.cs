using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    Spring spring;
    LineRenderer lr;
    public P_Character_HookSwing grapplingGun;
    private Vector3 currentGrapplePosition;

    [Header("=== Rope Animation Settings ===")]
    [SerializeField] int quality;
    [SerializeField] float damper;
    [SerializeField] float strength;
    [SerializeField] float velocity;
    [SerializeField] float waveCount;
    [SerializeField] float waveHeight;
    public AnimationCurve affectCurve;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    private void Update()
    {
        if (grapplingGun.activeGrapple)
            lr.enabled = true;
        else 
           lr.enabled = false;
    }

    private void LateUpdate()
    {
        DrawRope();
    }


    public void DrawRope()
    {
        if (!grapplingGun.activeGrapple)
        {
            currentGrapplePosition = grapplingGun.gunTip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
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

        var grapplePoint = grapplingGun.predictionHit.point;
        var gunTipPosition = grapplingGun.gunTip.position;
        var direction = (grapplePoint - gunTipPosition).normalized;
        var up = Quaternion.LookRotation(direction) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (int i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;

            float dynamicWaveHeight = Mathf.Clamp(waveHeight, 0.1f, Mathf.Abs(Vector3.Distance(gunTipPosition, grapplePoint)) / 10f);
            var offset = up * dynamicWaveHeight * Mathf.Sin(delta * waveCount * Mathf.PI * spring.Value * affectCurve.Evaluate(delta));

            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }
}
