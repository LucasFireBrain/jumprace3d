using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour, IPlayer {
    public static int AiIndex;

    public ParticleSystem _dustParticles;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private Platform _currentPlatform;

    private float _progress;
    private float _bounceCount;

    //MOVEMENT
    private float _baseSpeed = 7f;
    private float _speed = 7;
    private float _baseJumpHeight = 7;
    private float _jumpHeight;
    private float _autoRotationSpeed = 1;

    //AUTO ROTATE
    private bool _isAutoRotate = true;
    private float _autoRotateDelta;
    private Coroutine _autoRotateRoutine;

    // Start is called before the first frame update
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();

        _currentPlatform = LevelGenerator.StartingPlatform.GetComponent<Platform>();
        for (int i = 0; i <= AiIndex; i++) {
            _currentPlatform = _currentPlatform.Next;
        }

        transform.position = _currentPlatform.transform.position + Vector3.up * 0.8f;

        //_rigidbody.useGravity = false;

        _animator = GetComponentInChildren<Animator>();
        //Set random color
        GetComponentInChildren<Renderer>().material.SetColor("_Color", Random.ColorHSV());
        AiIndex++;

        //Add to player list
        GameController.Main.Players.Add(this);
    }

    public void BounceUp() {
        //Apply force upwards.
        _rigidbody.velocity = Vector3.up * _jumpHeight;
        _animator.Play("Flip_01");
    }

    // Update is called once per frame
    void Update() {

        transform.LookAt(_currentPlatform.Next.transform.position.withY(transform.position.y)); //Look forward at the next platform

        //Move after a number of bounces
        if (_bounceCount > Random.Range(1, 3)) {
            //Check distance between AI and Next Platform on XY Plane
            if (Vector3.Distance(transform.position, _currentPlatform.Next.transform.position.withY(transform.position.y)) > 0.1f) {
                _rigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * _speed);
            }
        }
    }
    void OnCollisionEnter(Collision collision) {
        Platform platform = collision.transform.GetComponentInParent<Platform>();
        if (platform != null) {
            //Reset bounce count
            if (platform != _currentPlatform) {
                _bounceCount = 0;
                if (platform is MovingPlatform) _bounceCount += 2; //Don't stay on moving platforms
            }
            else {
                _bounceCount++;
            }

            _currentPlatform = platform;
            if (platform is BottomPlatform) {
                _jumpHeight = _baseJumpHeight * 4;
                _speed = _baseSpeed / 3;
            }
            else if (platform is GoalPlatform) {
                _jumpHeight = 0;
                _speed = 0;
                //Win
            }
            else {
                _progress = platform.Progress;
                _jumpHeight = _baseJumpHeight;
                _speed = _baseSpeed;
            }
            _dustParticles.Play();

            BounceUp();
        }
    }

    public float GetProgress() {
        return _progress;
    }

    public string GetName() {
        return name;
    }
}
