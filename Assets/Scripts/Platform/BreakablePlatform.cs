using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : Platform {
    public override void Bounce() {
        GetComponent<MeshCollider>().enabled = false;
        foreach (Transform t in transform) {
            Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
            rb.AddExplosionForce(100, transform.position, 1);
        }
    }
}
