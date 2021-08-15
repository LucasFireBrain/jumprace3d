using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platform
{
    private float _delta;
    private bool _isMovingRight;

    protected override void Start() {
        base.Start();
    }

    void Update()
    {
        //Ping Pong move left and right
        if (_delta < -0.5f) _isMovingRight = true;
        if (_delta > 0.5f) _isMovingRight = false;
        if (_isMovingRight) { 
            transform.Translate(Vector3.right * Time.deltaTime / 3);
            _delta += Time.deltaTime / 3;
        }
        else { 
            transform.Translate(Vector3.left * Time.deltaTime / 3);
            _delta -= Time.deltaTime / 3;
        }
    }
}
