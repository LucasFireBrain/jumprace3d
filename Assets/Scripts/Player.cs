using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Platform _currentPlatform;

    //LOGIC
    private Vector2 _previousTouchPos;
    private bool _isDead;
    private bool _isStarted;
    private bool _hasMoved;

    //MOVEMENT
    private float _baseSpeed = 5;
    private float _speed = 2;
    private float _baseJumpHeight = 7;
    private float _jumpHeight;
    private float _autoRotationSpeed = 1;

    //AUTO ROTATE
    private bool _isAutoRotate = true;
    private float _autoRotateDelta;
    private Coroutine _autoRotateRoutine;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        transform.position = LevelGenerator.StartingPlatform.position + Vector3.up * 0.5f;
        _rigidbody.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Process input and move only after starter or if is not dead
        if(_isStarted && !_isDead) Move();

        if (!_isStarted) {
            if (Input.GetMouseButtonUp(0)) {
                TapToStart();
            }
        }

        //Fall
        if (transform.position.y < -0.5f) {
            EnterWater();
        }
    }

    void TapToStart() {
        _isStarted = true;
        _rigidbody.useGravity = true;
        GameController.Main.UIController.StartPanel.SetActive(false);   
        GameController.Main.UIController.InstructionFade(true);             //Show instructions
    }

    void Move() {
        if (Input.GetMouseButtonDown(0))
        {
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
        else if (Input.GetMouseButton(0))
        {
            //move forward
            _rigidbody.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
            //Rotate
            float xDelta = Input.mousePosition.x - _previousTouchPos.x;
            _previousTouchPos = Input.mousePosition;
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

    public void BounceUp() {
        //Apply force upwards.
        _rigidbody.velocity = Vector3.up * _jumpHeight;
    }

    public void SetCurrentPlatform(Platform platform) {
        _currentPlatform = platform;
        _autoRotateRoutine = StartCoroutine(SetAutoRotateRoutine());
    }
    IEnumerator SetAutoRotateRoutine() {
        yield return new WaitForSeconds(0.25f);
        _isAutoRotate = true;
    }

    public void EnterWater() {
        Camera.main.transform.SetParent(null);
        //Play water particles
        //Die
        GameController.Main.GameOver();
    }

    public void Die(Vector3 contactPoint) {
        //Set Ragdoll
        _isDead = true;
        _rigidbody.AddExplosionForce(20, contactPoint, 4);
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        Platform platform = collision.transform.GetComponentInParent<Platform>();
        if (platform != null)
        {
            if (platform is BottomPlatform)
            {
                _jumpHeight = _baseJumpHeight * 4;
                _speed = _baseSpeed / 3;
            }
            else {
                Debug.Log("Not bottom");
                _jumpHeight = _baseJumpHeight;
                _speed = _baseSpeed;
            }
            BounceUp();
            platform.Bounce();
        }
    }
}
