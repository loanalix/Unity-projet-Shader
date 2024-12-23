using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] int _damage = 5;
    [SerializeField] int _attackTime = 5;
    [SerializeField] List<Slash> slashes;
    private bool _attacking = false;

    public void StartAttack()
    {
        _attacking = true;
        StartCoroutine(Attack());
        StartCoroutine(StopAttack(_attackTime));
    }
    public void StartAttack(float attackTime)
    {
        _attacking = true;
        StartCoroutine(Attack());
        StartCoroutine(StopAttack(attackTime));
    }
    

    public int GetDamage()
    {
        return _damage;
    }

    public IEnumerator Attack()
    {
        for (int i = 0; i < slashes.Count; i++) {
            yield return new WaitForSeconds(slashes[i].delay);
            slashes[i].slashObj.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        DisableSlashes();
    }

    public IEnumerator StopAttack(float time)
    {
        yield return new WaitForSeconds(time);
        _attacking = false;
    }

    void DisableSlashes()
    {
        for (int i = 0; i < slashes.Count; i++)
        {
            slashes[i].slashObj.SetActive(false);
        }
    }

    public bool isAttacking()
    {
        return _attacking;
    }
}

[System.Serializable]
public class Slash
{
    public GameObject slashObj;
    public float delay;
}