using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] List<Attack> _combo;

    [SerializeField] float _maxTimeBetweenAttacks; // Combo is cancelled if the player doesnt start another attack after this time
    int _comboIndex;
    float remainingTime;
    Attack _currentAttack;
    Volume mainCamVolume;
    [SerializeField] CinemachineFreeLook cam;
    [SerializeField] GameObject hitEffect;

    float targetVignetteIntensity;
    float currentVignetteIntensity;

    float defaultFov;
    float targetFov;
    float currentFov;

    private void Start()
    {
        if(_combo.Count == 0)
        {
            Debug.LogWarning("Combo has no attacks");
        } else
        {
            _currentAttack = _combo[0];
        }
        _comboIndex = 0;

        mainCamVolume = Camera.main.GetComponent<Volume>();

        if (cam == null)
            Debug.LogWarning("No virtual camera");
        defaultFov = cam.m_Lens.FieldOfView;
    }

    private void Update()
    {
        remainingTime -= Time.deltaTime;
        if(remainingTime < 0)
        {
            // cancel combo
            EndCombo();
        }

        // Update vignette
        if (mainCamVolume.profile.TryGet(out Vignette vignette))
        {
            targetVignetteIntensity = Mathf.Lerp(targetVignetteIntensity, 0, _currentAttack.vignetteEffect.recoverySpeed * Time.deltaTime);
            currentVignetteIntensity = Mathf.Lerp(currentVignetteIntensity, targetVignetteIntensity, _currentAttack.vignetteEffect.snapiness * Time.fixedDeltaTime);

            vignette.intensity.value = currentVignetteIntensity;
        }

        // Update fov
        targetFov = Mathf.Lerp(targetFov, defaultFov, _currentAttack.fovEffect.recoverySpeed * Time.deltaTime);
        currentFov = Mathf.Lerp(currentFov, targetFov, _currentAttack.vignetteEffect.snapiness * Time.fixedDeltaTime);

        cam.m_Lens.FieldOfView = currentFov;
    }

    public void TriggerAttack()
    {
        remainingTime = _maxTimeBetweenAttacks;

        StartCoroutine(DealDamage(_currentAttack));
        StartCoroutine(AttackEffect(_currentAttack));
        StartCoroutine(StartVignetteEffect(_currentAttack));
        StartCoroutine(StartFovEffect(_currentAttack));

        _comboIndex++;
        if (_comboIndex >= _combo.Count)
            EndCombo();

        _currentAttack = _combo[_comboIndex];
    }

    IEnumerator DealDamage(Attack attack)
    {
        yield return new WaitForSeconds(attack.attackDelay);

        // check if an enemy is colliding with the attack hitbox
        BoxCollider boxCollider = attack.hitbox as BoxCollider;
        Vector3 center = boxCollider.transform.position + boxCollider.center;
        Vector3 halfExtents = boxCollider.size / 2;

        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, boxCollider.transform.rotation);
        foreach(Collider hitCollider in hitColliders)
        {

            if (hitCollider.TryGetComponent<Enemy>(out var enemy))
            {
                // Deal damage
                enemy.TakeDamage(attack.damage);
                
                // effect
                GameObject newEffect = Instantiate(hitEffect);

                Vector3 playerDir = (gameObject.transform.position - enemy.transform.position).normalized;

                newEffect.transform.position = new Vector3(enemy.transform.position.x + playerDir.x * 0.5f, enemy.transform.position.y + 1, enemy.transform.position.z + playerDir.z * 0.5f);
            }
        }
    }

    IEnumerator StartVignetteEffect(Attack attack)
    {
        yield return new WaitForSeconds(attack.vignetteEffect.delay);
        targetVignetteIntensity = attack.vignetteEffect.strenght;
    }
    IEnumerator StartFovEffect(Attack attack)
    {
        yield return new WaitForSeconds(attack.fovEffect.delay);
        targetFov = defaultFov - attack.fovEffect.strenght;
    }

    IEnumerator AttackEffect(Attack attack)
    {
        yield return new WaitForSeconds(attack.effectDelay);
        attack.effect.SetActive(true);

        yield return new WaitForSeconds(0.4f);
        attack.effect.SetActive(false);
    }

    void EndCombo()
    {
        //Debug.Log("Combo ended");
        _comboIndex = 0;
        _currentAttack = _combo[0];
    }

    public Attack GetCurrentAttack()
    {
        return _currentAttack;
    }

    [System.Serializable]
    public class Attack
    {
        public int attackAnimationIndex = 0;
        public float damage = 10;
        public float attackDelay = 0f;
        
        public Collider hitbox;

        public VignetteEffect vignetteEffect = new VignetteEffect();
        public FovEffect fovEffect = new FovEffect();

        public GameObject effect;
        public float effectDelay = 0.5f;
    }

    [System.Serializable]
    public class VignetteEffect
    {
        public float strenght = 0.6f;
        public float delay = 0.4f;
        public float snapiness = 10;
        public float recoverySpeed = 2;
    }

    [System.Serializable]
    public class FovEffect
    {
        public float strenght = 5f;
        public float delay = 0.4f;
        public float snapiness = 10;
        public float recoverySpeed = 2;
    }
}
