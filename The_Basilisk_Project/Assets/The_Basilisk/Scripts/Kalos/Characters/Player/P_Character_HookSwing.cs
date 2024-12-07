/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_HookSwing : MonoBehaviour
{
    [Header("=== References ===")]
    P_Character_Move pm;
    P_Character_HookGrappling pGrapple;
    Rigidbody rb;
    Transform orientation;
    CharacterController controller;
    [SerializeField] Transform cam;
    [SerializeField] Transform gunTip;
    [SerializeField] LayerMask isGrappleable;
    [SerializeField] LineRenderer lr;

    [Header("=== Swing Settings ===")]
    [SerializeField] float maxSwingDistance;
    [SerializeField] float horizontalForce;
    [SerializeField] float forwardForce;
    [SerializeField] float extendSpeed;

    [Header("=== Prediction ===")]
    public RaycastHit predictionHit;
    [SerializeField] float predictionRadius;
    public Transform predictionPoint;

    SpringJoint joint;

    private Vector3 swingPoint, currentGrapplePosition;

    //Grapple bools
    public bool activeGrapple, swinging, shortenCable;
    [SerializeField] InputAction shortenCableInput;

    private bool shouldEnableController = true;

    void Start()
    {
        pm = GetComponent<P_Character_Move>();

        rb = GetComponent<Rigidbody>();

        controller = GetComponent<CharacterController>();

        orientation = GetComponent<Transform>();

        pGrapple = GetComponent<P_Character_HookGrappling>();

        shortenCableInput.performed += ctx => shortenCable = true;
        shortenCableInput.canceled += ctx => shortenCable = false;
    }

    void Update()
    {
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
            pm.swingSpeed = pm.playerSpeed;
        }

        if (joint != null)
            AirMovement();

        ChecForSwingPoints();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

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

    void ChecForSwingPoints()
    {
        if (joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionRadius, cam.forward, out sphereCastHit, maxSwingDistance, isGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, isGrappleable);

        Vector3 realHitPoint;

        // Direct hit
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;

        // Indirect (predicted) hit
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;

        // Miss
        else realHitPoint = Vector3.zero;

        // realHitPoint found
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        // realHitPoint not found
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    void AirMovement()
    {
        if (swinging)
        {
            //right
            if (pm.moveInput.x > 0) rb.AddForce(orientation.right * horizontalForce * Time.deltaTime);
            //left
            if (pm.moveInput.x < 0) rb.AddForce(-orientation.right * horizontalForce * Time.deltaTime);

            //forward
            if (pm.moveInput.x > 0) rb.AddForce(orientation.forward * forwardForce * Time.deltaTime);
        }

        //shorten cable
        if (shortenCable)
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }

        //extend cable
        if (pm.moveInput.x < 0)
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }

    void StartSwing()
    {
        if (predictionHit.point == Vector3.zero) return;

        ResetRestrictions();

        swinging = true;
        controller.enabled = false;
        rb.isKinematic = false;

        swingPoint = predictionHit.point;
        predictionPoint.gameObject.SetActive(false);

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = true;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

    public void ResetRestrictions()
    {
        swinging = false;
    }

    void StopSwing()
    {
        swinging = false;

        if (joint != null)
        {
            Destroy(joint);
        }

        shouldEnableController = false;
        StartCoroutine(ReenableCharacterController());
    }
    private IEnumerator ReenableCharacterController()
    {
        yield return new WaitForSeconds(1f);
        shouldEnableController = true;

        if(!pGrapple.activeGrapple && !swinging)
        {
            controller.enabled = true;
            rb.isKinematic = true;
        }
    }

    void DrawRope()
    {
        //no grappling no draw
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }
}
*/



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

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask isGrappleable;
    [SerializeField] private LineRenderer lr;

    [Header("=== Swing Settings ===")]
    [SerializeField] private float maxSwingDistance = 20f;
    [SerializeField] private float horizontalForce = 10f;
    [SerializeField] private float forwardForce = 10f;
    [SerializeField] private float shortenSpeed = 5f;

    [Header("=== Prediction ===")]
    public RaycastHit predictionHit;
    [SerializeField] private float predictionRadius = 1f;
    public Transform predictionPoint;

    private SpringJoint joint;
    private Vector3 swingPoint, currentGrapplePosition;

    public bool activeGrapple;
    public bool swinging;
    public bool shortenCable;

    private bool shouldEnableController = true;

    void Start()
    {
        pm = GetComponent<P_Character_Move>();
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        if (!cam || !gunTip || !predictionPoint || !lr)
        {
            Debug.LogError("error");
        }

        lr.enabled = false;
    }

    void Update()
    {
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

    private void LateUpdate()
    {
        DrawRope();
    }

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
        if (context.performed)
        {
            JumpToPosition();
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
        if (predictionHit.point == Vector3.zero) return;

        ResetRestrictions();

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

        joint.spring = 10f;
        joint.damper = 4f;
        joint.massScale = 4f;

        lr.enabled = true;
        lr.positionCount = 2;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }
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
        lr.enabled = false;

        if (joint != null)
        {
            Destroy(joint);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (swinging)
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
    }

    public void DrawRope()
    {
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 5f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }
}
