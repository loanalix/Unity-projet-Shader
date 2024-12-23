using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] CharacterController _characterController;
    [SerializeField] float _speed = 6f;
    [SerializeField] float _sprintSpeed = 10f;
    [SerializeField] private InputActionReference _movement, _jump, _sprint, _attack;

    [SerializeField] float _turnSmoothTime = 0.1f;
    [SerializeField] Transform _cam;
    float turnSmoothVelocity;

    [SerializeField] float _gravity = -9.81f;
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _groundDistance;
    [SerializeField] LayerMask _groundMask;
    bool isGrounded = false;
    [SerializeField] float _jumpHeight;
    Vector3 _velocity;


    [SerializeField] Animator _animator;

    [SerializeField] ComboSystem _comboSystem;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(_comboSystem == null)
        {
            _comboSystem = GetComponent<ComboSystem>();
            if (_comboSystem == null)
            {
                Debug.LogWarning("Character has no ComboSystem");
            }
        }
    }

    void Update()
    {
        //// --- Ground Check --- ////
        isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        if (isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f; // force player on the ground
        }
        _animator.SetBool("Grounded", isGrounded);


        //// --- WASD Movement --- ////
        var movementInput = _movement.action.ReadValue<Vector2>();
        Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        if(direction.magnitude >= 0.1f)
        {
            _animator.SetBool("Walking", true);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            if (_sprint.action.ReadValue<float>() > Mathf.Epsilon)
            {
                _characterController.Move(moveDirection.normalized * _sprintSpeed * Time.deltaTime);
            } else
            {
                _characterController.Move(moveDirection.normalized * _speed * Time.deltaTime);
            }
        } else
        {
            _animator.SetBool("Walking", false);
        }

        //// --- Jumping --- ////
        if (_jump.action.ReadValue<float>() > 0.1f && isGrounded) {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            _animator.SetTrigger("Jump");
        }



        //if(_attack.action.ReadValue<float>() > 0.1f && _comboSystem != null)
        //{
        //    _comboSystem.TriggerAttack();
        //    _animator.SetTrigger("Attack");
        //    _animator.SetInteger("AttackAnimationIndex", _comboSystem.GetCurrentAttack().attackAnimationIndex);
        //}

        // Apply velocity
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime); // m/s/s
    }

    private void OnAttack()
    {
        _animator.SetInteger("AttackAnimationIndex", _comboSystem.GetCurrentAttack().attackAnimationIndex);
        _animator.SetTrigger("Attack");       
        //_comboSystem.TriggerAttack();
    }
}
