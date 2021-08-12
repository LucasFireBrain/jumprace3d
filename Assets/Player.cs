using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _speed = 1;

    private Vector2 _previousTouchPos;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move() {
        if (Input.GetMouseButtonDown(0)) {
            _previousTouchPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0)) {
            //move forward
            _rigidbody.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
            //Rotate
            float xDelta = Input.mousePosition.x - _previousTouchPos.x;
            _previousTouchPos = Input.mousePosition;
            transform.Rotate(Vector3.up, xDelta);
        }
    }

    void Rotate() {
    }

    public void BounceUp() {
        //Apply force upwards.
        _rigidbody.velocity = _rigidbody.velocity.withY(7);
    }
}
