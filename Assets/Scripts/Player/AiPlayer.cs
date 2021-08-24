using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour, IPlayer {
    public static int AiIndex;

    public ParticleSystem _dustParticles;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private Platform _currentPlatform;

    private bool _isStarted;
    private float _progress;
    private float _bounceCount;

    //MOVEMENT
    private float _baseSpeed = 3f;
    private float _speed = 3;
    private float _baseJumpHeight = 7;
    private float _jumpHeight;
    private float _autoRotationSpeed = 1;

    //AUTO ROTATE
    private bool _isAutoRotate = true;
    private float _autoRotateDelta;
    private float _autoRotateSpeed;
    private Coroutine _autoRotateRoutine;

    // Start is called before the first frame update
    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;

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
        if (GameController.Main.IsStarted) {
            if (_currentPlatform != null && _currentPlatform.Next != null) {
                transform.LookAt(_currentPlatform.Next.transform.position.withY(transform.position.y));
                //Move after a number of bounces
                if (_bounceCount > Random.Range(0, 2)) {
                    //Check distance between AI and Next Platform on XY Plane
                    if (transform.position.DistanceXY(_currentPlatform.Next.transform.position) > 0.05f) {
                        Vector3 targetPos = _currentPlatform.Next.transform.position.withY(transform.position.y);
                        Vector3 moveVector = Vector3.Lerp(transform.position, targetPos, _speed * Time.deltaTime);
                        _rigidbody.MovePosition(moveVector);
                    }
                }
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

    public void StartGame() {
        if (!_isStarted) {
            _isStarted = true;
            _rigidbody.useGravity = true;
        }
    }
}
