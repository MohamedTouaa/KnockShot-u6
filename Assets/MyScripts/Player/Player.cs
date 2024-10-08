using SmallHedge.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement firstPersonController;
    private void Awake()
    {
        firstPersonController = GetComponent<PlayerMovement>();
    }
    public void TakeDamage()
    {
        SoundManager.PlaySound(SoundType.Damage, null, 0.5f);
        firstPersonController.enabled= false; 
       
          
    }
}
