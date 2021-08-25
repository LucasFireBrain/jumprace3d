using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IPlayer {

    //REFERENCES
    private Animator _animator;
    private Platform _currentPlatform;
    private Rigidbody _rigidbody;
  
    //PARTICLES
    public GameObject WaterParticles;   //Used by enabling game object
    public ParticleSystem DustParticles;
    public ParticleSystem[] Rockets;


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
    private float _autoRotateSpeed = 150;
    private float _autoRotateDelta;
    private bool _isAutoRotate;


    // Start is called before the first frame update
    void Start() {
        //Initialize
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _animator = GetComponentInChildren<Animator>();
        transform.position = LevelGenerator.StartingPlatform.position + Vector3.up * 0.8f;

        //Add to player list
        GameController.Main.Players.Add(this);
    }

    // Update is called once per frame
    void Update() {
        if (_isAutoRotate) AutoRotate();                                    //Auto Rotate
        if (Input.GetMouseButton(0)) StopAutoRotate();                      //Stop Auto Rotate
        if (!_isEnteredWater && transform.position.y < -0.1f) EnterWater(); //Fall and die
        if (_isStarted && !_isDead) Rotate();                               //Rotate
    }

    void FixedUpdate() {
        if (_isStarted && !_isDead) Move();
    }

    public void StartGame() {
        if (!_isStarted) {
            _isStarted = true;
            _rigidbody.useGravity = true;
        }
    }
    void AutoRotate() {
        if (_currentPlatform != null && _currentPlatform.Next != null) {
            Debug.Log("AUTOROTATE");
            Vector3 forward = (_currentPlatform.Next.transform.position - _currentPlatform.transform.position).withY(0);
            Quaternion targetRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _autoRotateSpeed * Time.deltaTime);
        }
    }
    void StopAutoRotate() {
        if (!_isAutoRotate) return;
        if (_autoRotateRoutine != null) StopCoroutine(_autoRotateRoutine);
        _isAutoRotate = false;
    }
    IEnumerator StartAutoRotateRoutine() {
        yield return new WaitForSeconds(0.25f);
        _isAutoRotate = true;
    }
    void Move() {
        //TOUCH INPUT
        if (Input.touchCount > 0) {
            if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary) {
                //move forward
                _rigidbody.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
            }
        }
        //MOUSE INPUT
        else {
            if (Input.GetMouseButton(0)) {
                //move forward
                _rigidbody.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
            }
        }
    }
    void Rotate() {
        //TOUCH INPUT
        if (Input.touchCount > 0) {
            if (Input.touches[0].phase == TouchPhase.Began) {
                _previousTouchPos = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary) {
                //Rotate
                float xDelta = (Input.touches[0].position.x - _previousTouchPos.x) / Screen.width * _dragFactor;
                _previousTouchPos = Input.touches[0].position;
                transform.Rotate(Vector3.up, xDelta);
            }
        }
        //MOUSE INPUT
        else {
            if (Input.GetMouseButtonDown(0)) {
                _previousTouchPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0)) {
                //Rotate
                float xDelta = (Input.mousePosition.x - _previousTouchPos.x) / Screen.width * _dragFactor;
                _previousTouchPos = Input.mousePosition;
                transform.Rotate(Vector3.up, xDelta);
            }
        }
    }

    void BounceUp() {
        //Apply force upwards.
        _rigidbody.velocity = Vector3.up * _jumpHeight;
        if (Random.Range(0, 2) == 0) { 
            _animator.Play("Flip_01");
        }
        else _animator.Play("Flip_02");
    }

    void FireRockets() {
        foreach (ParticleSystem rocket in Rockets) {
            rocket.Play();
        }
    }
    void SetCurrentPlatform(Platform platform) {
        _currentPlatform = platform;
        _bounceCount = 0;
        _autoRotateRoutine = StartCoroutine(StartAutoRotateRoutine());
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

            if (_currentPlatform != null 
                && _currentPlatform.Next != null 
                && platform != _currentPlatform 
                && platform != _currentPlatform.Next) {
                //LONG JUMP
                GameController.Main.UIController.ShowPowerup(2);
            }

            if (_currentPlatform != platform) {
                SetCurrentPlatform(platform);
            }

            if (platform is BottomPlatform) {
                _jumpHeight = _baseJumpHeight * 3.5f;
                _speed = _baseSpeed / 2.5f;
            }

            else if (platform is GoalPlatform) {
                _jumpHeight = 0;
                _speed = 0;
                _progress = 1;
                GameController.Main.UIController.SetProgress(_progress);
                //Win
                GameController.Main.GameOver(true);
                _animator.Play("Idle");
            }
            else {  //platform is Normal Platform
                //Bonus Speed
                if (_bounceCount == 0 && transform.position.DistanceXY(platform.transform.position) < 0.2f) {
                    GameController.Main.UIController.ShowPowerup(0);
                    _jumpHeight = _baseJumpHeight * 1.3f;
                    _speed = _baseSpeed * 1.3f;
                    FireRockets();  //Rocket shoes particles
                }
                else if (_bounceCount == 0 && transform.position.DistanceXY(platform.transform.position) < 0.3f) {
                    GameController.Main.UIController.ShowPowerup(1);
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
        Camera.main.transform.SetParent(null);
        _progress = 0;
        _isDead = true;
        GameController.Main.GameOver(false);

        //Ragdoll
        GetComponentInChildren<RagDollController>().EnableRagdoll();
    }

}
