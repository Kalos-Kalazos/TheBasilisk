using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Enviroment_DD : MonoBehaviour
{
    [SerializeField] GameObject doorR, doorL;
        
    [Header("=== Door Settings ===")]
    [SerializeField] bool openned = false;
    public bool canBeOpenned = false;
    [SerializeField] float firstDistance = 0.2f;
    [SerializeField] float finalDistance = 1f;
    [SerializeField] float duration = 4f;
    [SerializeField] float elapsedTime = 0f;
    Vector3 startPositionR;
    Vector3 startPositionL;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            //StopCoroutine(nameof(CloseTheDoor));
            startPositionR = doorR.transform.position;
            startPositionL = doorL.transform.position;
            TriggerDoors();
        }
    }
    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(nameof(OpenTheDoor));
            startPositionR = doorR.transform.position;
            startPositionL = doorL.transform.position;
            AwayFromDoors();
        }
    }
    */

    public void TriggerDoors()
    {
        if (!openned && canBeOpenned)
            StartCoroutine(nameof(OpenTheDoor));
    }
    public void TriggerSingleDoor()
    {
        if (!openned && canBeOpenned)
            StartCoroutine(nameof(OpenTheDoorSingle));
    }
    /*
    void AwayFromDoors()
    {
        if (openned && canBeOpenned)
            StartCoroutine(nameof(CloseTheDoor));
    }
    */
    private IEnumerator OpenTheDoor()
    {
        openned = true;

        //The First movement (Anticipation) position
        Vector3 firstPositionR = startPositionR + doorR.transform.right * firstDistance;
        Vector3 firstPositionL = startPositionL - doorL.transform.right * firstDistance;
        //The final movement positions
        Vector3 finalPositionR = firstPositionR + doorR.transform.right * finalDistance;
        Vector3 finalPositionL = firstPositionL - doorL.transform.right * finalDistance;

        while (elapsedTime < duration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2f);
            doorR.transform.position = Vector3.Lerp(startPositionR, firstPositionR, t);
            doorL.transform.position = Vector3.Lerp(startPositionL, firstPositionL, t);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < duration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2f);
            doorR.transform.position = Vector3.Lerp(firstPositionR, finalPositionR, t);
            doorL.transform.position = Vector3.Lerp(firstPositionL, finalPositionL, t);
            yield return null;
        }
    }

    public IEnumerator OpenTheDoorSingle()
    {
        openned = true;
        float forwardDistance = 0.1f;
        float leftDistance = 1.5f;
        float duration = 4f;
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        Vector3 forwardPosition = startPosition + transform.forward * forwardDistance;
        Vector3 finalPosition = forwardPosition + transform.right * leftDistance;

        while (elapsedTime < duration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2f);
            transform.position = Vector3.Lerp(startPosition, forwardPosition, t);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < duration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2f);
            transform.position = Vector3.Lerp(forwardPosition, finalPosition, t);
            yield return null;
        }
    }

    /*
    private IEnumerator CloseTheDoor()
    {
        openned = false;

        //The First movement (Anticipation) position
        Vector3 firstPositionR = startPositionR - doorR.transform.right * finalDistance;
        Vector3 firstPositionL = startPositionL + doorL.transform.right * finalDistance;
        //The final movement positions
        Vector3 finalPositionR = firstPositionR - doorR.transform.right * firstDistance;
        Vector3 finalPositionL = firstPositionL + doorL.transform.right * firstDistance;

        while (elapsedTime < duration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2f);
            doorR.transform.position = Vector3.Lerp(startPositionR, firstPositionR, t);
            doorL.transform.position = Vector3.Lerp(startPositionL, firstPositionL, t);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < duration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2f);
            doorR.transform.position = Vector3.Lerp(firstPositionR, finalPositionR, t);
            doorL.transform.position = Vector3.Lerp(firstPositionL, finalPositionL, t);
            yield return null;
        }
    }*/
}
