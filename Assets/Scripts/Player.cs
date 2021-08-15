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

    //MOVEMENT
    [SerializeField]
    private float _speed = 2;
    private float _jumpHeight = 7;
    private float _autoRotationSpeed = 1;

    //AUTO ROTATE
    private bool _isAutoRotate = true;
    private float _autoRotateDelta;
    private Coroutine _autoRotateRoutine;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        transform.position = LevelGenerator.StartingPlatform.position + Vector3.up * 2;
    }

    // Update is called once per frame
    void Update()
    {
        //Process input and move
        Move();

        //Fall
        if (transform.position.y < 1) {
            EnterWater();
        }
    }

    void Move() {
        if (_isDead) return;
        if (Input.GetMouseButtonDown(0))
        {
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
}
