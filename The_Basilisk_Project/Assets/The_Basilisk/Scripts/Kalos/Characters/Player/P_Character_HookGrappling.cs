using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_HookGrappling : MonoBehaviour
{
    [Header("=== References ===")]
    [SerializeField] P_Character_Move pm;
    [SerializeField] Transform cam;
    [SerializeField] Transform gunTip;
    [SerializeField] LayerMask isGrappleable;
    [SerializeField] LineRenderer lr;

    [Header("=== Grappling Settings ===")]
    [SerializeField] float maxGrappleDistance;
    [SerializeField] float grappleDelay;
    [SerializeField] float grapplingCD;
    private float grapplingCDTimer;

    private Vector3 grapplePoint;

    private bool grappling;

    void Start()
    {
        pm = GetComponent<P_Character_Move>();
    }

    private void Update()
    {
        #region --GrapplingCooldown
        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }

        if (grapplingCDTimer < 0) grapplingCDTimer = 0;
        #endregion

    }

    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, gunTip.position);
    }
    public void LaunchHook(InputAction.CallbackContext context)
    {
        StartGrapple();
    }

    /*public void PullHook(InputAction.CallbackContext context)
    {
        ExecuteGrapple();
    }
    public void ReleaseHook(InputAction.CallbackContext context)
    {
        StopGrapple();
    }*/

    private void StartGrapple()
    {
        if (grapplingCDTimer > 0) return;

        grappling = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, isGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelay);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {

    }

    private void StopGrapple()
    {
        grappling = false;

        grapplingCDTimer = grapplingCD;

        lr.enabled = false;
    }
}
