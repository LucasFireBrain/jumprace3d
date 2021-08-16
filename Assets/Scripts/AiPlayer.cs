using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour
{
    public ParticleSystem _dustParticles;
    
    private Rigidbody _rigidbody;
    private Animator _animator;
    private Platform _currentPlatform;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        transform.position = LevelGenerator.StartingPlatform.position + Vector3.up * 0.5f;
        _rigidbody.useGravity = false;
        _animator = GetComponentInChildren<Animator>();
    }

    public void BounceUp()
    {
        //Apply force upwards.
        _rigidbody.velocity = Vector3.up * _jumpHeight;
        _animator.Play("Flip_01");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
