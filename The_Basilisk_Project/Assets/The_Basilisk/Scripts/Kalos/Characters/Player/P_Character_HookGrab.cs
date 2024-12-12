using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_HookGrab : MonoBehaviour
{
    [Header("=== Grab Settings ===")]
    private P_Character_HookSwing playerSwing;

    public Vector3 grabPoint;

    [SerializeField] LayerMask canGrab;

    public bool shortenCable;

    private SpringJoint joint;

    private void Start()
    {
        playerSwing = gameObject.GetComponent<P_Character_HookSwing>();
    }

    private void Update()
    {
        if (shortenCable)
        {
            float distanceFromPoint = Vector3.Distance(transform.position, grabPoint);
            joint.maxDistance = Mathf.Max(joint.maxDistance - playerSwing.shortenSpeed * Time.deltaTime, distanceFromPoint * 0.5f);
        }

        CheckForGrabPoints();
    }

    public void GrabObject(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartGrab();
        }
        else if (context.canceled)
        {
            StopGrab();
        }
    }

    public void TakeCloseObject(InputAction.CallbackContext context)
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

    void CheckForGrabPoints()
    {
        if (joint != null) return;

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
            playerSwing.predictionPoint.gameObject.SetActive(true);
            playerSwing.predictionPoint.position = realHitPoint;
        }
        else
        {
            playerSwing.predictionPoint.gameObject.SetActive(false);
            playerSwing.predictionPoint.position = playerSwing.cam.position + playerSwing.cam.forward * playerSwing.maxSwingDistance;
        }

        playerSwing.predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    void StartGrab()
    {
        if (playerSwing.predictionHit.point == Vector3.zero || playerSwing.grapplingCDTimer > 0) return;

        playerSwing.hasGrabbed = true;

        playerSwing.activeGrapple = true;

        grabPoint = playerSwing.predictionHit.point;

        playerSwing.predictionPoint.gameObject.SetActive(false);

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grabPoint;

        float distanceFromPoint = Vector3.Distance(transform.position, grabPoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.1f;

        joint.spring = 20f;
        joint.damper = 1f;
        joint.massScale = 0.1f;
    }

    void StopGrab()
    {
        //lr.enabled = false;
        playerSwing.activeGrapple = false;
        playerSwing.hasGrabbed = false;

        if (joint != null)
        {
            Destroy(joint);
        }

        playerSwing.grapplingCDTimer = playerSwing.grapplingCD;
    }
}
