using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_HookGrab : MonoBehaviour
{
    [Header("=== Grab Settings ===")]
    private P_Character_HookSwing playerSwing;
    public PA_Hook playerPA_Hook;
    public float launchPower = 40f;
    public Rigidbody grabbedObjectRB;

    public Vector3 grabPoint, targetPosition, hitPoint;
    public Transform hookOrigin;

    [SerializeField] LayerMask canGrab;

    public bool shortenCable, grabbed;

    public RaycastHit predictionHitObject;
    public Transform predictionPointObject;

    public SpringJoint joint;

    [SerializeField] bool inputPressed;

    private void Start()
    {
        playerSwing = gameObject.GetComponent<P_Character_HookSwing>();
        playerPA_Hook = gameObject.GetComponent<PA_Hook>();
    }

    private void Update()
    {
        if (playerSwing.swinging) return;

        //MoveGrabbedObject();
        CheckForGrabPoints();

        if (grabbedObjectRB != null && !grabbed)
        {
            float distanceToHook = Vector3.Distance(hookOrigin.position, grabbedObjectRB.position);
            if (distanceToHook < 0.5f)
            {
                StopGrab();
                AlignWithHook();
                grabbed = true;
            }
        }
    }

    void AlignWithHook()
    {
        grabbedObjectRB.isKinematic = true;

        Collider objectCollider = grabbedObjectRB.GetComponent<Collider>();

        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }

        grabbedObjectRB.velocity = Vector3.zero;
        grabbedObjectRB.angularVelocity = Vector3.zero;
        grabbedObjectRB.transform.position = hookOrigin.position;
        grabbedObjectRB.transform.rotation = hookOrigin.rotation;
        grabbedObjectRB.transform.SetParent(hookOrigin);

        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }
    }

    public void GrabObject(InputAction.CallbackContext context)
    {
        if (context.started && !grabbed)
        {
            StartGrab();
        }
        else if (context.canceled)
        {
            if (grabbed && playerPA_Hook.retracted && !playerPA_Hook.returning)
                ReleaseGrab();
            else
                StopGrab();
        }
    }

    void ReleaseGrab()
    {
        grabbed = false;
        if (grabbedObjectRB != null)
        {
            playerPA_Hook.hookHead.GetComponent<P_Reference_HeadHook>().isHooked = false;
            grabbedObjectRB.gameObject.transform.SetParent(null);
            grabbedObjectRB.isKinematic = false;
            LaunchObject();
        }
    }

    void LaunchObject()
    {
        grabbedObjectRB.AddForce(playerSwing.cam.forward * launchPower, ForceMode.Impulse);
        Debug.Log("Launching object wow so far ma men");
        grabbedObjectRB = null;

    }

    void CheckForGrabPoints()
    {
        if (joint != null && joint.connectedAnchor == playerSwing.swingPoint) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(playerSwing.cam.position, playerSwing.predictionRadius, playerSwing.cam.forward, out sphereCastHit, playerSwing.maxSwingDistance, canGrab);

        RaycastHit raycastHit;
        Physics.Raycast(playerSwing.cam.position, playerSwing.cam.forward, out raycastHit, playerSwing.maxSwingDistance, canGrab);

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
            predictionPointObject.gameObject.SetActive(true);
            predictionPointObject.position = realHitPoint;
        }
        else
        {
            predictionPointObject.gameObject.SetActive(false);
            predictionPointObject.position = playerSwing.cam.position + playerSwing.cam.forward * playerSwing.maxSwingDistance;
        }

        predictionHitObject = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    void StartGrab()
    {
        if (predictionHitObject.point == Vector3.zero || playerSwing.grapplingCDTimer > 0) return;

        playerSwing.hasGrabbed = true;

        playerSwing.activeGrapple = true;

        grabPoint = predictionHitObject.point;

        predictionPointObject.gameObject.SetActive(false);

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grabPoint;

        float distanceFromPoint = Vector3.Distance(transform.position, grabPoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.1f;

        joint.spring = 20f;
        joint.damper = 1f;
        joint.massScale = 0.1f;

        grabbedObjectRB = predictionHitObject.rigidbody;

        if (grabbedObjectRB != null)
        {
            grabbedObjectRB.velocity = Vector3.zero;

            if (Vector3.Distance(targetPosition, grabbedObjectRB.position) < 1f)
            {
                grabbedObjectRB.gameObject.transform.SetParent(hookOrigin);
                grabbedObjectRB.isKinematic = true;
                grabbed = true;
            }
        }

        playerSwing.grapplingCDTimer = playerSwing.grapplingCD;
    }

    /*
    void MoveGrabbedObject()
    {
        if (grabbedObjectRB != null && !grabbed && joint != null)
        {
            joint.connectedAnchor = grabbedObjectRB.position;
            targetPosition = hookOrigin.position;
            grabbedObjectRB.position = Vector3.MoveTowards(grabbedObjectRB.position, targetPosition, objectMoveSpeed * Time.deltaTime);
        }
    }*/

    void StopGrab()
    {
        //lr.enabled = false;
        playerSwing.activeGrapple = false;
        playerSwing.hasGrabbed = false;
        playerPA_Hook.hookHead.GetComponent<P_Reference_HeadHook>().isHooked = false;

        if (joint != null)
        {
            Destroy(joint);
        }
    }
}
