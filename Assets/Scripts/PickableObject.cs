using UnityEngine;
using UnityEngine.Events;

public class PickableObject : MonoBehaviour
{
    [SerializeField] UnityEvent _onPickup;

    private void OnTriggerEnter(Collider other)
    {
        // check if its a player

        //if(other.CompareTag("Player")) // better than == because it gives error if typo

        //var playerController =; // var validé par le prof
        //if (other.GetComponent<PlayerController>() != null) // il vaut mieux check si le composant existe car on ne peut pas mettre plusieur tags
        //{
        //    _particleSystem.Play();
        //}

        if (other.TryGetComponent<PlayerController>(out var pC)) // renvoie true si le component existe et le store dans pC
        {
            _onPickup.Invoke();
            //Destroy(gameObject);
            Destroy(gameObject, 1.2f); // kms (mais après la particule)
        }
    }
}
