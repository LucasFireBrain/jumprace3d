using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _speed = 1;

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

    public void Move() {
        if (Input.GetMouseButton(0)) {
            _rigidbody.MovePosition(transform.position + transform.forward * _speed);
        }
    }

    public void BounceUp() {
        //Apply force upwards.
        _rigidbody.velocity = _rigidbody.velocity.withY(10);
    }
}
