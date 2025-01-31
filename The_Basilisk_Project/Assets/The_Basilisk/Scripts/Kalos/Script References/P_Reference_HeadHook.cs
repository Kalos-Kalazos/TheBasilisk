using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Reference_HeadHook : MonoBehaviour
{
    public bool isHooked = false;
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 6 && !isHooked)
        {
            collision.transform.SetParent(transform);
            isHooked = true;
        }
    }
    */
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == 6 && !isHooked)
        {
            collision.transform.SetParent(transform);
            isHooked = true;
        }
    }
}
