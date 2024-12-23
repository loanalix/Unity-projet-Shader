using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnShieldRipples : MonoBehaviour
{
    public GameObject shieldRipple;
    
    private VisualEffect shieldRippleVFX;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            print("a");
            GameObject ripples = Instantiate(shieldRipple, transform);
            shieldRippleVFX = ripples.GetComponent<VisualEffect>();
            shieldRippleVFX.SetVector3("SphereCenter", other.GetContact(0).point);
            
            Destroy(shieldRipple, 5f);
        }
    }
}
