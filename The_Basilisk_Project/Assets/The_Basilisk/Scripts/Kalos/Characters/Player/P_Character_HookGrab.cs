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
    public Transform hookHeadTransform;  // La cabeza del gancho con animación

    [SerializeField] LayerMask canGrab;

    public bool shortenCable, grabbed;

    public RaycastHit predictionHitObject;
    public Transform predictionPointObject;

    [SerializeField] bool inputPressed;

    private void Start()
    {
        playerSwing = gameObject.GetComponent<P_Character_HookSwing>();
        playerPA_Hook = gameObject.GetComponent<PA_Hook>();
    }

    private void Update()
    {
        if (playerSwing.swinging) return;

        CheckForGrabPoints();

        if (grabbedObjectRB != null && !grabbed)
        {
            float distanceToHook = Vector3.Distance(hookOrigin.position, grabbedObjectRB.position);
            if (distanceToHook < 0.5f)
            {
                StopGrab();
                grabbed = true;
            }
        }

        // Sincronización con la cabeza del gancho (sin alinear estrictamente el objeto)
        if (grabbed && grabbedObjectRB != null && hookHeadTransform != null)
        {
            grabbedObjectRB.velocity = Vector3.zero;  // Evitar movimientos no deseados
            grabbedObjectRB.angularVelocity = Vector3.zero;

            // Mantenemos el objeto pegado a la cabeza del gancho, pero no lo alineamos estrictamente
            grabbedObjectRB.transform.position = hookHeadTransform.position;
            grabbedObjectRB.transform.rotation = hookHeadTransform.rotation;
        }
    }

    public void GrabObject(InputAction.CallbackContext context)
    {
        if (context.started && !grabbed)
        {
            StartGrab();
        }
        else if (context.started)
        {
            if (grabbed && playerPA_Hook.retracted && !playerPA_Hook.returning)
                ReleaseGrab();
            else
                StopGrab();
        }
    }

    void ReleaseGrab()
    {
        if (grabbedObjectRB == null) return;

        grabbed = false;
        grabbedObjectRB.transform.SetParent(null);  // Dejamos de ser hijo de la cabeza del gancho

        // Desactivamos temporalmente las colisiones para evitar que se empuje al soltar
        Collider grabbedCollider = grabbedObjectRB.GetComponent<Collider>();
        if (grabbedCollider != null)
        {
            grabbedCollider.enabled = false;
        }

        // Aguardamos un pequeño tiempo para permitir que el objeto se libere correctamente
        StartCoroutine(ReleaseAfterDelay(0.1f));

        // Eliminar las restricciones de posición y rotación al soltar el objeto
        grabbedObjectRB.constraints = RigidbodyConstraints.None;  // Eliminamos las restricciones físicas

        LaunchObject();  // Lanzamos el objeto
    }

    // Corutina para liberar el objeto después de un pequeño retraso
    IEnumerator ReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Hacemos que el objeto no se quede atrapado por colisiones inmediatamente después de soltarlo
        Collider grabbedCollider = grabbedObjectRB.GetComponent<Collider>();
        if (grabbedCollider != null)
        {
            grabbedCollider.enabled = true;
        }
    }

    void LaunchObject()
    {
        if (grabbedObjectRB != null)
        {
            grabbedObjectRB.AddForce(playerSwing.cam.forward * launchPower, ForceMode.Impulse);  // Añadimos fuerza al objeto
        }
    }

    void CheckForGrabPoints()
    {
        if (grabbed) return;

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

        grabbedObjectRB = predictionHitObject.rigidbody;

        if (grabbedObjectRB != null)
        {
            grabbedObjectRB.velocity = Vector3.zero;

            grabbedObjectRB.transform.SetParent(hookHeadTransform);  // Lo hacemos hijo de la cabeza del gancho
            grabbedObjectRB.useGravity = true;  // Dejamos la gravedad activada

            // Congelamos el movimiento y la rotación mientras está enganchado
            grabbedObjectRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

            grabbed = true;
        }

        playerSwing.grapplingCDTimer = playerSwing.grapplingCD;
    }

    void StopGrab()
    {
        playerSwing.activeGrapple = false;
        playerSwing.hasGrabbed = false;
        playerPA_Hook.hookHead.GetComponent<P_Reference_HeadHook>().isHooked = false;

        grabbedObjectRB = null;
    }
}