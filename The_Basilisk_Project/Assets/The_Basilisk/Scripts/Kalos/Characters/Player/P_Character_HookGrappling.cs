using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_HookGrappling : MonoBehaviour
{
    [Header("=== References ===")]
    private P_Character_Move pm;
    private Rigidbody rb;
    private Transform orientation;
    private CharacterController controller;
    [SerializeField] Transform cam;
    [SerializeField] Transform gunTip;
    [SerializeField] LayerMask isGrappleable;
    P_Character_HookSwing pSwing;

    [Header("=== Grappling Settings ===")]
    [SerializeField] float maxGrappleDistance;
    [SerializeField] float grappleDelay;
    [SerializeField] float grapplingCD;
    [SerializeField] float overshootYAxis;
    private float grapplingCDTimer;

    public Vector3 grapplePoint;

    //Grapple bools
    private bool grappling, freeze;
    public bool activeGrapple;


    void Start()
    {
        pm = GetComponent<P_Character_Move>();

        rb = GetComponent<Rigidbody>();

        controller = GetComponent<CharacterController>();

        orientation = GetComponent<Transform>();

        pSwing = GetComponent<P_Character_HookSwing>();
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
        }
        else
        {
            pm.playerSpeed = 10;
        }
    }

    private void LateUpdate()
    {
        pSwing.DrawRope();
    }

    public void LaunchGrapple(InputAction.CallbackContext context)
    {
        if (context.performed)
            StartGrapple();
    }

    private void StartGrapple()
    {
        if (grapplingCDTimer > 0) return;

        grappling = true;

        freeze = true;

        /*RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, isGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelay);
            pSwing.predictionPoint.gameObject.SetActive(false);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelay);
            pSwing.predictionPoint.gameObject.SetActive(false);
        }*/
        grapplePoint = pSwing.predictionHit.point;
        Invoke(nameof(ExecuteGrapple), grappleDelay);
        pSwing.predictionPoint.gameObject.SetActive(false);
    }

    private void ExecuteGrapple()
    {
        freeze = false;
        controller.enabled = false;
        rb.isKinematic = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
        Invoke(nameof(ResetRestrictions), 1f);
    }

    private void StopGrapple()
    {
        freeze = false;
        grappling = false;

        grapplingCDTimer = grapplingCD;

        controller.enabled = true;

        rb.isKinematic = true;
    }

    private bool enableMovementOnNextTouch;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 2f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            StopGrapple();
        }
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ * (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
        +Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
