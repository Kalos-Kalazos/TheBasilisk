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

    bool interacted;
    bool checkStatus;
    GameObject player;
    [SerializeField] GameObject interactImage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            checkStatus = true;
            if (canBeOpenned)
            {
                interactImage.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkStatus = false;
            interactImage.SetActive(false);
        }
    }

    private void Update()
    {
        if (checkStatus && player != null)
        {
            Interaction(player);
        }

        if (checkStatus && interacted)
        {
            if (CompareTag("SingleDoor"))
            {
                TriggerSingleDoor();
                checkStatus = false;
            }
            else
            {
                startPositionR = doorR.transform.position;
                startPositionL = doorL.transform.position;
                TriggerDoors();
                checkStatus = false;
            }
        }
    }

    void Interaction(GameObject player)
    {
        interacted = player.GetComponent<P_DoorInteraction>().performed;
    }

    public void TriggerDoors()
    {
        if (!openned && canBeOpenned)
        {
            P_SFX_Control.Instance.PlaySingleDoorSound(transform.position);

            StartCoroutine(nameof(OpenTheDoor));
        }
    }

    public void TriggerSingleDoor()
    {
        if (!openned && canBeOpenned)
        {
            P_SFX_Control.Instance.PlayDoubleDoorSound(transform.position);

            StartCoroutine(nameof(OpenTheDoorSingle));
        }
    }

    private IEnumerator OpenTheDoor()
    {
        openned = true;
        elapsedTime = 0f;

        Vector3 firstPositionR = startPositionR + doorR.transform.right * firstDistance;
        Vector3 firstPositionL = startPositionL - doorL.transform.right * firstDistance;
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
}