using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flank : MonoBehaviour
{
    public Transform playerPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("change position 1");
            other.gameObject.GetComponent<EnnemyMovement>().StopFlanking(playerPosition);
           
        }
    }

}
