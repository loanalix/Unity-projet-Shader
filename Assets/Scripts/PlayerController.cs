using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private InputActionReference _movement, _attack, _pointerPosition, _sprint, _jump;
    [SerializeField] private Transform _feetPosition;
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _sprintSpeed = 40f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _gravity = 9.3f;
    [SerializeField] private MeleeWeapon _meleeWeapon;
    //private float _rotationSPeed = 100000f;
    private GameObject _camera;
    private Rigidbody _rb;
    private Animator _animator;
    private bool _isGrounded;

    //public Vector3 _velocity;

    void Start()
    {
        // lock mouse
        //Cursor.lockState = CursorLockMode.Locked;
        _isGrounded = true;
        Cursor.visible = false;
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _camera = GameObject.FindGameObjectWithTag("MainCamera");

        if(_meleeWeapon == null)
        {
            Debug.LogWarning("The player has no melee weapon");
        }
        if(_feetPosition == null)
        {
            Debug.LogWarning("Player feet position was not assigned, using transform instead");
            _feetPosition = transform;
        }
        //_velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = isGrounded();
        if (_isGrounded == false)
        {
            _rb.AddForce(new Vector3(0, -_gravity, 0), ForceMode.Acceleration);
        }
        _animator.SetBool("Grounded", _isGrounded);

        // Movement
        var movementInput = _movement.action.ReadValue<Vector2>(); 
        var mouseInput = _pointerPosition.action.ReadValue<Vector2>();
        float sprinting = _sprint.action.ReadValue<float>(); // utiliser started ?

        float speed = _speed;
        if (sprinting > math.EPSILON)
        {
            speed = _sprintSpeed;
        }

        Vector3 dir = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        // move player
        if(dir.magnitude > math.EPSILON)
        {
            _animator.SetBool("IsWalking", true);
            _rb.AddForce(dir * Time.deltaTime * speed * 10000);
        }
        else
        {
            _animator.SetBool("IsWalking", false);
        }

        // jump
        if (_jump.action.ReadValue<float>() > math.EPSILON && _isGrounded) {
            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
            _animator.SetTrigger("Jump");
        }

        if (_attack.action.ReadValue<float>() > math.EPSILON && _meleeWeapon != null && _meleeWeapon.isAttacking() == false)
        {
            _animator.SetTrigger("Attack");
            _meleeWeapon.StartAttack();
        }

        // get rotation toward movement
        transform.LookAt(transform.position + dir);


        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position + dir),1);

        // rotate camera
        //_camera.transform.rotation = Quaternion.Euler(new Vector3(-mouseInput.y * _sensitivity, mouseInput.x * _sensitivity, 0));
    }

    bool isGrounded()
    {
        Debug.DrawRay(_feetPosition.position + new Vector3(0, 0.2f, 0), -Vector3.up, Color.red, 2f);
        return Physics.Raycast(_feetPosition.position + new Vector3(0,0.1f,0), -Vector3.up, 0.2f);
    }
}
