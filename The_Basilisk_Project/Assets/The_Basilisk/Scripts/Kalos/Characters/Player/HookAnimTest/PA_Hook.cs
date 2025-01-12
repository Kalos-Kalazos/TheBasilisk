using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PA_Hook : MonoBehaviour
{
    P_Character_HookSwing hookControl;
    P_Character_HookGrab grabControl;

    [Header("=== Anim settings ===")]

    public bool launched;
    public bool launch;
    public bool returning;
    public bool retracted;
    private bool isProcessingAction;
    [SerializeField] float speed;
    [SerializeField] Transform originPoint;
    [SerializeField] Vector3 targetPoint;
    Vector3 targetPosition;

    public Transform hookHead;

    Rigidbody headRB;

    void Start()
    {
        hookControl = GetComponent<P_Character_HookSwing>();
        grabControl = GetComponent<P_Character_HookGrab>();

        headRB = hookHead.GetComponent<Rigidbody>();

        retracted = true;

        headRB.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabControl.grabbed) return;

        launch = hookControl.activeGrapple;

        if (hookControl.hasGrabbed) 
            targetPoint = grabControl.joint.connectedAnchor;
        else if(launch)
            targetPoint = hookControl.swingPoint;

        if (!isProcessingAction)
        {
            if (launch && !launched)
            {
                StartCoroutine(LaunchHead());
            }
            else if (!launch && launched)
            {
                StartCoroutine(ReturnHook());
            }
        }
    }

    void AlignWithHook()
    {
        hookHead.transform.SetPositionAndRotation(originPoint.position, originPoint.rotation);
        hookHead.transform.SetParent(originPoint);
        hookHead.GetComponent<BoxCollider>().enabled = false;
    }

    private IEnumerator LaunchHead()
    {
        if (targetPoint == Vector3.zero) StopCoroutine(LaunchHead());

        isProcessingAction = true;
        launched = true;
        retracted = false;

        hookHead.GetComponent<BoxCollider>().enabled = true;

        float distance = Vector3.Distance(hookHead.position, targetPoint);

        while (distance > 0.2f)
        {
            headRB.isKinematic = true;

            var step = speed * Time.deltaTime;
            hookHead.transform.SetParent(null);

            hookHead.position = Vector3.MoveTowards(hookHead.position, targetPoint, step);

            distance = Vector3.Distance(hookHead.position, targetPoint);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f); // Espera breve para estabilidad
        isProcessingAction = false; // Desbloquear acciones
    }

    private IEnumerator ReturnHook()
    {
        isProcessingAction = true;
        returning = true;
        headRB.isKinematic = false;

        yield return new WaitForSeconds(0.7f);

        float step = speed * Time.deltaTime;
        targetPosition = originPoint.position;

        while (Vector3.Distance(hookHead.position, targetPosition) > 0.2f)
        {
            hookHead.position = Vector3.MoveTowards(hookHead.position, targetPosition, step);
            yield return null;
        }

        StopGrab();
        AlignWithHook();
        isProcessingAction = false;
    }

    void StopGrab()
    {
        returning = false;
        launched = false;
        retracted = true;

        headRB.isKinematic = true;
    }
}