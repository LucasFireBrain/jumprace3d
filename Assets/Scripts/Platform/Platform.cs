using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    //Linked List
    public Platform Next;

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
        Player player = collision.rigidbody.GetComponent<Player>();
        if (player != null) { 
            Bounce(player);
            player.SetCurrentPlatform(this);
        }
    }
}
