using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveFadeOut : MonoBehaviour
{
    public GameObject fadeOut;

    public void Deactivate()
    {
        fadeOut.SetActive(false);
    }
}
