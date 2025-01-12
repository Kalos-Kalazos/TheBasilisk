using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PA_Combined_Hook : MonoBehaviour
{
    [Header("=== Hook Settings ===")]
    public Transform hookHead;
    public Transform originPoint;
    public LayerMask canGrab;
    public float launchSpeed = 10f;
    public float returnSpeed = 10f;
    public float grabDistanceThreshold = 1f;
    public float launchPower = 5f;

    private Rigidbody headRB;
    private Rigidbody grabbedObjectRB;
    private SpringJoint joint;
    private Vector3 targetPoint;

    private bool isProcessingAction;
    private bool launched;
    private bool retracted = true;
    private bool grabbed;
    private bool canGrabObject;

    // Referencia a swingControl que se mantiene
    public P_Character_HookSwing swingControl;

    void Start()
    {
        headRB = hookHead.GetComponent<Rigidbody>();
        headRB.isKinematic = true;
    }

    void Update()
    {
        if (!isProcessingAction)
        {
            // Determinar si lanzar o retraer el gancho
            if (Input.GetButtonDown("Fire1") && !launched)
            {
                DetermineTargetPoint();
                StartCoroutine(LaunchHook());
            }
            else if (Input.GetButtonUp("Fire1") && launched)
            {
                StartCoroutine(ReturnHook());
            }
        }

        // Verificar si se puede agarrar un objeto
        if (grabbedObjectRB != null && !grabbed)
        {
            float distanceToHook = Vector3.Distance(hookHead.position, grabbedObjectRB.position);
            if (distanceToHook < grabDistanceThreshold)
            {
                AttachObjectToHook();
                grabbed = true;
            }
        }
    }

    private void DetermineTargetPoint()
    {
        // Usar raycast o spherecast para buscar objetos que se puedan agarrar
        RaycastHit hit;
        if (Physics.SphereCast(hookHead.position, 0.5f, hookHead.forward, out hit, swingControl.maxSwingDistance, canGrab))
        {
            canGrabObject = hit.rigidbody != null;
            targetPoint = canGrabObject ? hit.point : swingControl.swingPoint;
            grabbedObjectRB = canGrabObject ? hit.rigidbody : null;
        }
        else
        {
            canGrabObject = false;
            targetPoint = swingControl.swingPoint;
            grabbedObjectRB = null;
        }
    }

    private IEnumerator LaunchHook()
    {
        isProcessingAction = true;
        launched = true;
        retracted = false;

        while (Vector3.Distance(hookHead.position, targetPoint) > 0.2f)
        {
            headRB.isKinematic = true;
            hookHead.SetParent(null);
            hookHead.position = Vector3.MoveTowards(hookHead.position, targetPoint, launchSpeed * Time.deltaTime);
            yield return null;
        }

        // Conectar el gancho a un objeto si es necesario
        if (canGrabObject && grabbedObjectRB != null)
        {
            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grabbedObjectRB.position;
            joint.spring = 20f;
            joint.damper = 1f;
            joint.massScale = 0.1f;
        }

        isProcessingAction = false;
    }

    private IEnumerator ReturnHook()
    {
        isProcessingAction = true;
        launched = false;

        while (Vector3.Distance(hookHead.position, originPoint.position) > 0.2f)
        {
            hookHead.position = Vector3.MoveTowards(hookHead.position, originPoint.position, returnSpeed * Time.deltaTime);
            yield return null;
        }

        ResetHook();
        isProcessingAction = false;
    }

    private void AttachObjectToHook()
    {
        grabbedObjectRB.isKinematic = true;
        grabbedObjectRB.transform.SetParent(hookHead);
    }

    private void ResetHook()
    {
        retracted = true;
        grabbed = false;

        if (grabbedObjectRB != null)
        {
            grabbedObjectRB.isKinematic = false;
            grabbedObjectRB.transform.SetParent(null);
            grabbedObjectRB = null;
        }

        if (joint != null)
        {
            Destroy(joint);
        }

        hookHead.SetParent(originPoint);
        hookHead.localPosition = Vector3.zero;
        headRB.isKinematic = true;
    }
}
