using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_HookGrappling : MonoBehaviour
{
    [Header("=== References ===")]
    [SerializeField] P_Character_Move pm;
    [SerializeField] Rigidbody rb;
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

    //Grapple bools
    private bool grappling, freeze, activeGrapple;

    void Start()
    {
        pm = GetComponent<P_Character_Move>();

        rb = GetComponent<Rigidbody>();
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

        if (freeze)
        {
            pm.playerSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else
            pm.playerSpeed = 10;
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

        freeze = true;

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
        freeze = false;
    }

    private void StopGrapple()
    {
        freeze = false;

        grappling = false;

        grapplingCDTimer = grapplingCD;

        lr.enabled = false;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        rb.velocity = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.y, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ * (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
        +Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
