using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blades : MonoBehaviour
{

    private float _rotationSpeed = -50f;
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.forward, _rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Player player = collision.rigidbody.GetComponent<Player>();
        if (player != null)
        {
            Camera.main.transform.SetParent(null);
            player.Die(collision.GetContact(0).point);
        }
    }
}
