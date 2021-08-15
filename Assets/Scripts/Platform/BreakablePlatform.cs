using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : Platform
{
    protected override void Bounce(Player player)
    {
        base.Bounce(player);
        foreach (Transform t in transform) {
            Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
            rb.AddExplosionForce(100, transform.position, 1);
        }
    }
}
