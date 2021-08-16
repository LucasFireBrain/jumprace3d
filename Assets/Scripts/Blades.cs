using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blades : MonoBehaviour {

    private float _rotationSpeed = -50f;

    // Update is called once per frame
    void Update() {
        transform.Rotate(0, 0, Time.deltaTime * _rotationSpeed);
    }

    IEnumerator OnCollisionEnter(Collision collision) {
        Player player = collision.rigidbody.GetComponent<Player>();
        if (player != null) {
            collision.rigidbody.AddExplosionForce(20, collision.GetContact(0).point, 4);
            yield return new WaitForSeconds(1f);
            player.Die();
        }
    }
}
