using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_HookSwing : MonoBehaviour
{
    [Header("=== References ===")]
    private P_Character_Move pm;
    private CharacterController controller;
    Rigidbody rb;

    [SerializeField] Transform orientation;
    [SerializeField] Transform cam;
    public Transform gunTip;
    [SerializeField] LayerMask isGrappleable;
    [SerializeField] LineRenderer lr;

    [Header("=== Swing Settings ===")]
    [SerializeField] float maxSwingDistance = 20f;
    [SerializeField] float horizontalForce = 10f;
    [SerializeField] float forwardForce = 10f;
    [SerializeField] float shortenSpeed = 5f;
    [SerializeField] float overshootYAxis = 1f;
    [SerializeField] float grapplingCD = 0.1f;
    float grapplingCDTimer;

    [Header("=== Prediction ===")]
    public RaycastHit predictionHit;
    [SerializeField] float predictionRadius = 1f;
    public Transform predictionPoint;

    private SpringJoint joint;
    private Vector3 currentGrapplePosition;
    public Vector3 swingPoint;

    public bool activeGrapple;
    public bool swinging;
    public bool shortenCable;
    public bool shootGrapple;

    private bool shouldEnableController = true;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Start()
    {
        pm = GetComponent<P_Character_Move>();
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        if (!cam || !gunTip || !predictionPoint || !lr)
        {
            Debug.LogError("error");
        }
    }

    void Update()
    {
        #region --GrapplingCooldown
        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }

        if (grapplingCDTimer < 0) grapplingCDTimer = 0;
        #endregion

        if (!shouldEnableController)
        {
            controller.enabled = false;
        }

        if (swinging)
        {
            pm.playerSpeed = pm.swingSpeed;
        }
        else
        {
            pm.playerSpeed = 10;
        }

        if (swinging)
        {
            AirMovement();
        }

        CheckForSwingPoints();
    }

    /*private void LateUpdate()
    {
        DrawRope();
    }*/

    public void LaunchSwing(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartSwing();
        }
        else if (context.canceled)
        {
            StopSwing();
        }
    }

    public void Retract(InputAction.CallbackContext context)
    {
        if (predictionHit.point == Vector3.zero) return;

        if (context.performed)
        {
            Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

            float grapplePointRelativeYPos = predictionHit.point.y - lowestPoint.y;
            float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

            if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

            JumpToPosition(predictionHit.point, highestPointOnArc);
        }
    }

    public void ShortenCable(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            shortenCable = true;
        }
        else if (context.canceled)
        {
            shortenCable = false;
        }
    }

    void CheckForSwingPoints()
    {
        if (joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionRadius, cam.forward, out sphereCastHit, maxSwingDistance, isGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, isGrappleable);

        Vector3 realHitPoint = Vector3.zero;

        if (raycastHit.point != Vector3.zero)
        {
            realHitPoint = raycastHit.point;
        }
        else if (sphereCastHit.point != Vector3.zero)
        {
            realHitPoint = sphereCastHit.point;
        }

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
            predictionPoint.position = cam.position + cam.forward * maxSwingDistance;
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    void AirMovement()
    {
        if (!activeGrapple) return;

        if (swinging)
        {
            if (pm.moveInput.x > 0)
                rb.AddForce(orientation.right * horizontalForce, ForceMode.Impulse);
            if (pm.moveInput.x < 0)
                rb.AddForce(-orientation.right * horizontalForce, ForceMode.Impulse);

            if (pm.moveInput.y > 0)
                rb.AddForce(orientation.forward * forwardForce, ForceMode.Impulse);
            if (pm.moveInput.y < 0)
                rb.AddForce(-orientation.forward * forwardForce, ForceMode.Impulse);
        }

        if (shortenCable)
        {
            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);
            joint.maxDistance = Mathf.Max(joint.maxDistance - shortenSpeed * Time.deltaTime, distanceFromPoint * 0.5f);
        }
    }

    void StartSwing()
    {
        if (predictionHit.point == Vector3.zero || grapplingCDTimer > 0) return;

        ResetRestrictions();

        activeGrapple = true;
        swinging = true;
        controller.enabled = false;
        rb.isKinematic = false;

        swingPoint = predictionHit.point;
        predictionPoint.gameObject.SetActive(false);

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 15f;
        joint.damper = 2f;
        joint.massScale = 4f;
        /*
        lr.enabled = true;
        lr.positionCount = 2;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);*/
    }
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 2f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        rb.velocity = velocityToSet;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ * (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
        + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public void ResetRestrictions()
    {
        swinging = false;

        shouldEnableController = false;
    }

    void StopSwing()
    {
        //lr.enabled = false;
        activeGrapple = false;

        if (joint != null)
        {
            Destroy(joint);
        }

        grapplingCDTimer = grapplingCD;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (swinging && collision.collider.CompareTag("Ground"))
        {
            ResetRestrictions();

            StartCoroutine(ReenableCharacterController());
        }
    }

    private IEnumerator ReenableCharacterController()
    {
        yield return new WaitForSeconds(0.1f);

        shouldEnableController = true;
        controller.enabled = true;
        rb.isKinematic = true;
        StopSwing();
    }

    /*public void DrawRope()
    {
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 5f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }*/
}