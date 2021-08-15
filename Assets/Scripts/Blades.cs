using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blades : MonoBehaviour
{

    private float _rotationSpeed = -50f;
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,Time.deltaTime * _rotationSpeed);
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
