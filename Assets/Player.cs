using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Platform _currentPlatform;

    [SerializeField]
    private float _speed = 2;
    private float _jumpHeight = 7;
    private float _autoRotationSpeed = 1;

    private bool _isAutoRotate = true;
    private float _autoRotateDelta;

    private Vector2 _previousTouchPos;

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
        if (Input.GetMouseButtonDown(0))
        {
            _previousTouchPos = Input.mousePosition;
            _autoRotateDelta = 0;
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
        _isAutoRotate = true;
    }

    public void EnterWater() {
        Camera.main.transform.SetParent(null);
        //Play water particles
        //Die
        //Open Menu
    }
}
