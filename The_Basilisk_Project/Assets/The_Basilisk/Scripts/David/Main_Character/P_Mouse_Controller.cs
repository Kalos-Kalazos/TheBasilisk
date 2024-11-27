using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Mouse_Controller : MonoBehaviour
{
    public float sentitiviy = 1.5f;
    public float smoothing = 1.5f;

    private float xMousePos;
    private float smoothedMousePos;

    private float currentLookingPos;
    void Start()
    {
        Cursor.lockState =CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ModifyInput();
        MovePlayer();
    }

    void GetInput()
    {
        xMousePos = Input.GetAxisRaw("Mouse X");

    }
    void ModifyInput()
    {
        xMousePos*= sentitiviy * smoothing;
        smoothedMousePos = Mathf.Lerp(a: smoothedMousePos, b: xMousePos, t: 1f / smoothing);

    }

    void MovePlayer()

    {
        currentLookingPos += smoothedMousePos;
        transform.localRotation = Quaternion.AngleAxis(currentLookingPos, transform.up);
    }

}
