using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void Bounce(Player player) {
        player.BounceUp();
        Debug.Log("Collided with " + player.name);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody.GetComponent<Player>() != null) { 
            Bounce(collision.rigidbody.GetComponent<Player>());
        }
    }
}
