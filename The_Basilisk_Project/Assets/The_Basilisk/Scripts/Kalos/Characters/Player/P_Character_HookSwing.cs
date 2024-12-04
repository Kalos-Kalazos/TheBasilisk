using System.Collections;
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
    [SerializeField] RaycastHit predictionHit;
    [SerializeField] float predictionRadius;
    [SerializeField] Transform predictionPoint;

    SpringJoint joint;

    private Vector3 swingPoint, currentGrapplePosition;

    //Grapple bools
    public bool activeGrapple, swinging, shortenCable;
    [SerializeField] InputAction shortenCableInput;

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

        pGrapple.grapplePoint = predictionHit.point;
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
        //right
        if (pm.moveInput.x >= 0) rb.AddForce(orientation.right * horizontalForce * Time.deltaTime);
        //left
        if (pm.moveInput.x < 0) rb.AddForce(-orientation.right * horizontalForce * Time.deltaTime);

        //forward
        if (pm.moveInput.x >= 0) rb.AddForce(orientation.forward * forwardForce * Time.deltaTime);

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
