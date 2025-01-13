using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class P_Character_HookSwing : MonoBehaviour
{
    [Header("=== References ===")]
    private P_Character_Move_v2 pm; //Player Move
    private P_Character_Combat pc;  //Player Combat
    private PA_Hook hookAnim;
    private CharacterController controller;
    Rigidbody rb;

    [SerializeField] HookCooldown hC;
    [SerializeField] Transform orientation;
    public Transform cam;
    public Transform gunTip;
    [SerializeField] LayerMask isGrappleable;
    [SerializeField] LineRenderer lr;

    [Header("=== Swing Settings ===")]
    public float maxSwingDistance = 20f;
    [SerializeField] float horizontalForce = 10f;
    [SerializeField] float forwardForce = 10f;
    public float shortenSpeed = 5f;
    [SerializeField] float overshootYAxis = 1f;
    public float grapplingCD = 0.1f;
    public float grapplingCDTimer;

    [Header("=== Aereal movement Settings ===")]
    [SerializeField] float maxSpeed = 8f;

    [Header("=== Prediction ===")]
    public RaycastHit predictionHit;
    public float predictionRadius = 1f;
    public Transform predictionPoint;

    private SpringJoint joint;
    private Vector3 currentGrapplePosition;
    public Vector3 swingPoint, grapplePoint;

    public bool activeGrapple;
    public bool swinging;
    public bool shortenCable;

    public bool hasGrabbed;

    private bool shouldEnableController = true;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Start()
    {
        pm = GetComponent<P_Character_Move_v2>();
        pc = GetComponent<P_Character_Combat>();
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        hookAnim = GetComponent<PA_Hook>();
        cam = Camera.main?.transform;

        if (!cam || !gunTip || !predictionPoint || !lr)
        {
            Debug.LogError("error");
        }
    }

    void Update()
    {
        #region --GrapplingCooldown
        if (grapplingCDTimer > 0 && hookAnim.retracted)
        {
            grapplingCDTimer -= Time.deltaTime;
        }

        if (grapplingCDTimer < 0) grapplingCDTimer = 0;
        #endregion

        #region --ReenableController
        if (!shouldEnableController)
        {
            controller.enabled = false;
            rb.isKinematic = false;
        }
        else
        {
            controller.enabled = true;
            rb.isKinematic = true;
        }

        if (pm.isGrounded && !swinging && !activeGrapple)
        {
            controller.enabled = true;
            rb.isKinematic = true;
        }

        #endregion

        if (swinging)
        {
            pm.playerSpeed = pm.swingSpeed;
        }
        else
        {
            pm.playerSpeed = 10;
        }

        CheckForSwingPoints();

        hC.currentCooldownTime = grapplingCDTimer;
    }

    private void FixedUpdate()
    {
        if (swinging)
        {
            AirMovement();
        }
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
            if (swinging)
            {
                ResetRestrictions();
                StopSwing();
            }
        }
    }

    /*
    public void Retract(InputAction.CallbackContext context)
    {
        if (predictionHit.point == Vector3.zero) return;

        if (context.performed)
        {
            Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

            float grapplePointRelativeYPos = predictionHit.point.y - lowestPoint.y;
            float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

            if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

            //JumpToPosition(predictionHit.point, highestPointOnArc);
        }
    }
    */

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

        if (Vector3.Distance(transform.position, realHitPoint) > 1f)
        {
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

        //Vector3.Distance(transform.position, swingPoint)
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

        LimitMovement();
    }
    private void LimitMovement()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void StartSwing()
    {
        if (predictionHit.point == Vector3.zero || grapplingCDTimer > 0 || !hasGrabbed && activeGrapple) return;

        ResetRestrictions();

        hC.UseHook();

        Invoke(nameof(ExecuteGrapple), 0.2f);

        pc.SoundEmitter();

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
        joint.minDistance = distanceFromPoint * 0.2f;

        joint.spring = 15f;
        joint.damper = 2f;
        joint.massScale = 15f;
        /*
        lr.enabled = true;
        lr.positionCount = 2;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);*/

        grapplingCDTimer = grapplingCD;
    }

    private void ExecuteGrapple()
    {
        if (predictionHit.point == Vector3.zero || grapplingCDTimer > 0 || !hasGrabbed && activeGrapple) return;

        controller.enabled = false;
        rb.isKinematic = false;
        grapplePoint = predictionHit.point;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(ResetRestrictions), 1f);
    }

    
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        //Invoke(nameof(ResetRestrictions), 1f);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!swinging || !activeGrapple)
        {
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

    void StopSwing()
    {
        //lr.enabled = false;
        activeGrapple = false;

        if (joint != null)
        {
            Destroy(joint);
        }
        StopCoroutine(ReenableCharacterController());
    }


    /*public void DrawRope()
    {
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 5f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }*/
}