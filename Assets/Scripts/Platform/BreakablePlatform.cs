using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : Platform {
    public MeshCollider MeshCollider;
    public override void Bounce() {
        //Disable Platform
        MeshCollider.enabled = false;

        //Add phyisics to pieces
        foreach (Transform t in transform) {
            Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
            rb.AddExplosionForce(100, transform.position, 1);
        }
       
        //Keep Linked List of platforms
        Platform temp = LevelGenerator.StartingPlatform;
        while (temp.Next != this) {
            temp = temp.Next;
        }
        temp.Next = this.Next;

        //Destroy gameobject after 4 seconds
        GameObject.Destroy(this.gameObject, 4);
    }
}
