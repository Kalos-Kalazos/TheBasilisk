using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkController : MonoBehaviour
{
    public ParticleSystem sparkParticles; //Arrastra aqui tu sistema de particulas en el Inspector
    private bool isTouchingGround = false;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground")) //Asegurarse de que esta en el suelo
        {
            isTouchingGround = true;
            sparkParticles.Play(); //Activa las chispas


        }

    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {

            isTouchingGround = false;
            sparkParticles.Stop(); //Detiene las chispas

        }

    }

    // Update is called once per frame
    void Update()
    {

        if (isTouchingGround && GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {

            if (! sparkParticles.isPlaying)
            sparkParticles.Play();

            

        }
        else
        {

            if (!sparkParticles.isPlaying)
                sparkParticles.Stop();

        }


    }
}
