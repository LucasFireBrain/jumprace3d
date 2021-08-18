using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollController : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody[] Rigidbodies;
    
    public void EnableRagdoll() {
        //Create parent reference
        GameObject parentGo = transform.parent.gameObject;

        //Detach from parent
        transform.SetParent(null);

        //Disable parent
        parentGo.SetActive(false);

        //Disable Animator
        Animator.enabled = false;

        //Enable physics on ragdoll parts
        for (int i = 0; i < Rigidbodies.Length; i++) {
            Rigidbody rb = Rigidbodies[i];
            rb.isKinematic = false;
            if(rb.GetComponent<Collider>() != null) rb.GetComponent<Collider>().enabled = true;
            //Add random force to each part
            rb.AddForce(Vector3.zero.RandomOffset(10, 10, 10) * 30);
        }
    }
}
