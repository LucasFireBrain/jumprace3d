using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IPlayer, ITouchHandler {

    //REFERENCES
    private Animator _animator;
    private Platform _currentPlatform;
    private Rigidbody _rigidbody;
  
    //PARTICLES
    public ParticleSystem DustParticles;
    public ParticleSystem[] Rockets;
    public GameObject WaterParticles;   //Used by enabling game object


    //INPUT
    private float _dragFactor = 200;
    private Vector2 _previousTouchPos;

    //GAME LOGIC
    private bool _isDead;
    private bool _isEnteredWater;
    private bool _isStarted;
    private bool _hasMoved;
    private float _progress;
    private int _bounceCount;

    //MOVEMENT
    private float _baseSpeed = 5;
    private float _speed = 2;
    private float _baseJumpHeight = 7;
    private float _jumpHeight;

    //AUTO ROTATE
    private Coroutine _autoRotateRoutine;
    private float _autoRotationSpeed = 1;
    private float _autoRotateDelta;
    private bool _isAutoRotate = true;


    // Start is called before the first frame update
    void Start() {

        //Initialize
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _animator = GetComponentInChildren<Animator>();
        transform.position = LevelGenerator.StartingPlatform.position + Vector3.up * 0.8f;

        //Add to Input Manager
        InputManager.TouchHandlers.Add(this);

        //Add to player list
        GameController.Main.Players.Add(this);
    }

    // Update is called once per frame
    void Update() {
#if UNITY_EDITOR    //For testing on editor with mouse
        //{ //Process input and move only after started or if player is not dead
        //    if (_isStarted && !_isDead) MoveAndRotate();
        //    if (!_isStarted) {
        //        if (Input.GetMouseButtonUp(0)) {
        //            OnTap(new Touch());
        //        }
        //    }
        //}
#endif
        //Fall
        if (!_isEnteredWater && transform.position.y < -0.1f) {
             EnterWater();
        }
    }
    public void OnTouch(Touch touch) {
        if (_isStarted && !_isDead) MoveAndRotate(touch);
    }
    public void OnTap(Touch touch) {
        if (!_isStarted) {
            _isStarted = true;
            _rigidbody.useGravity = true;
            GameController.Main.UIController.StartPanel.SetActive(false);
            GameController.Main.UIController.InstructionFade(true);             //Show instructions
        }
    }

    void MoveAndRotate(Touch touch) {
        if (touch.phase == TouchPhase.Began) {
            if (!_hasMoved) {   //Hide Instructions
                _hasMoved = true;
                GameController.Main.UIController.InstructionFade(false);    
            }
            _previousTouchPos = touch.position;
            _autoRotateDelta = 0;

            if (_autoRotateRoutine != null) {     //Stop auto rotate
                StopCoroutine(_autoRotateRoutine);
            }
            _isAutoRotate = false;
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
            //move forward
            _rigidbody.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
            //Rotate
            float xDelta = (touch.position.x - _previousTouchPos.x) / Screen.width * _dragFactor;
            _previousTouchPos = touch.position;
            transform.Rotate(Vector3.up, xDelta);
        }
        else if (_isAutoRotate && _currentPlatform != null) {
            float currentAngle = transform.eulerAngles.y;
            float targetAngle = _currentPlatform.transform.eulerAngles.y;
            _autoRotateDelta += Time.deltaTime * _autoRotationSpeed;
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, _autoRotateDelta);
            transform.eulerAngles = transform.eulerAngles.withY(angle);
        }
    }

#if UNITY_EDITOR
    void MoveAndRotate() {   //Move with mouse, for editor testing or web version

        if (Input.GetMouseButtonDown(0)) {
            if (!_hasMoved) {
                _hasMoved = true;
                GameController.Main.UIController.InstructionFade(false);    //Hide Instructions
            }
            _previousTouchPos = Input.mousePosition;
            _autoRotateDelta = 0;
            //Stop auto rotate
            if (_autoRotateRoutine != null) {
                StopCoroutine(_autoRotateRoutine);
            }
            _isAutoRotate = false;
        }
        else if (Input.GetMouseButton(0)) {
            //move forward
            _rigidbody.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
            //Rotate
            float xDelta = (Input.mousePosition.x - _previousTouchPos.x) / Screen.width * _dragFactor;
            _previousTouchPos = Input.mousePosition;
            transform.Rotate(Vector3.up, xDelta);
        }
        else if (_isAutoRotate && _currentPlatform != null ) {
            float currentAngle = transform.eulerAngles.y;
            float targetAngle = _currentPlatform.transform.eulerAngles.y;
            _autoRotateDelta += Time.deltaTime * _autoRotationSpeed;
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, _autoRotateDelta);
            transform.eulerAngles = transform.eulerAngles.withY(angle);
        }
    }
#endif

    void BounceUp() {
        //Apply force upwards.
        _rigidbody.velocity = Vector3.up * _jumpHeight;
        _animator.Play("Flip_01");
    }

    void FireRockets() {
        foreach (ParticleSystem rocket in Rockets) {
            rocket.Play();
        }
    }

    void SetCurrentPlatform(Platform platform) {
        _currentPlatform = platform;
        _bounceCount = 0;
        _autoRotateRoutine = StartCoroutine(SetAutoRotateRoutine());
    }
    IEnumerator SetAutoRotateRoutine() {
        yield return new WaitForSeconds(0.25f);
        _isAutoRotate = true;
    }

    void EnterWater() {
        WaterParticles.SetActive(true);
        Die();
    }

    void OnCollisionEnter(Collision collision) {
        //Collision with other players

        //Collision with Platform
        Platform platform = collision.transform.GetComponentInParent<Platform>();
        if (platform != null) {

            if (_currentPlatform != platform) {
                SetCurrentPlatform(platform);
            }

            if (platform is BottomPlatform) {
                //Normal Speed;
                _jumpHeight = _baseJumpHeight * 4;
                _speed = _baseSpeed / 3;
            }

            else if (platform is GoalPlatform) {
                _jumpHeight = 0;
                _speed = 0;
                //Win
                GameController.Main.GameOver(true);
                _animator.Play("Idle");
            }
            else {  //platform is NormalPlatforms
                //Bonus Speed
                if (_bounceCount == 0 && transform.position.DistanceXY(platform.transform.position) < 0.2f) {
                    Debug.Log("PERFECT");
                    _jumpHeight = _baseJumpHeight * 1.3f;
                    _speed = _baseSpeed * 1.3f;
                    //Rocket shoes particles
                    FireRockets();
                }
                else if (_bounceCount == 0 && transform.position.DistanceXY(platform.transform.position) < 0.3f) {
                    Debug.Log("GOOD");
                    _jumpHeight = _baseJumpHeight * 1.15f;
                    _speed = _baseSpeed * 1.15f;
                }
                else {
                    //Normal Speed;
                    _jumpHeight = _baseJumpHeight;
                    _speed = _baseSpeed;
                }
                _progress = platform.Progress;
                GameController.Main.UIController.SetProgress(_progress);
            }

            _bounceCount++;
            DustParticles.Play();

            if (!(platform is GoalPlatform)) { 
                BounceUp();
            }
            platform.Bounce();

            //Update ranks
            GameController.Main.UpdatePlayerRank();
        }
    }

    public float GetProgress() {
        return _progress;
    }

    public string GetName() {
        return name;
    }
    public void Die() {
        //Set Ragdoll
        Camera.main.transform.SetParent(null);
        _progress = 0;
        _isDead = true;
        GameController.Main.GameOver(false);
    }

}
