using RPGCharacterAnims.Lookups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _maxHp = 10;
    private float _hp;
    [SerializeField] private Animator _animator;
    [SerializeField] private Material _material;
    [SerializeField] private FloatingText _damageText;

    void Start()
    {
        _hp = _maxHp;

        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        
        if(_material == null)
        {
            _material = GetComponent<Renderer>().sharedMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // check if sword
        if(other.TryGetComponent<MeleeWeapon>(out var weapon))
        {
            // check if attacking
            if (weapon.isAttacking())
            {
                // take damage
                Debug.Log("OUch");

                TakeDamage(weapon.GetDamage());
                _material.SetInt("Effect", 1);
                if (_hp > 0)
                {
                    _animator.SetTrigger("Hit");
                }
                else
                {
                    _animator.SetTrigger("Die");
                }

                // stop weapon attack
                weapon.StopAttack(0);

                StartCoroutine(stopEffect());
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        FloatingText damageText = Instantiate(_damageText, transform.position, Quaternion.identity, transform);
        //damageText.transform.LookAt(Camera.main.transform);
        damageText.SetText(damage.ToString());

        damageText.ClearColors();
        damageText.SetColor(Color.blue);
        damageText.SetColor(Color.white, 1);
        damageText.SetColor(Color.red, 2);

        if (_hp > 0)
        {
            _animator.SetTrigger("Hit");
        }
        else
        {
            _animator.SetTrigger("Die");
        }
    }

    private IEnumerator stopEffect()
    {
        yield return new WaitForSeconds(1f);
        _material.SetInt("Effect", 0);
    }
}
