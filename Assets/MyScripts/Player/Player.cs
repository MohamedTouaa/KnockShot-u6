using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement firstPersonController;
    [SerializeField]
    private GameObject deathScreen;
    [SerializeField]
    private Volume globalVolume;
    [SerializeField]
    private Shotgun shotgun;

    private Vignette vignette;  
    private float deathVignetteTargetIntensity = 0.5f; 
    private float vignetteLerpDuration = 1f; 

    private void Awake()
    {
        firstPersonController = GetComponent<PlayerMovement>();

        // Access the Vignette component in the Volume profile
        if (globalVolume.profile.TryGet(out vignette))
        {
            Debug.Log("Vignette effect found.");
        }
        else
        {
            Debug.LogError("Vignette effect not found in Volume profile.");
        }
    }

    public void Die()
    {
        StartCoroutine(Death());
    }

  
    public IEnumerator Death()
    {
        SoundManager.PlaySound(SoundType.Damage, null, 1);
        firstPersonController.enabled = false;

       
        

      
        yield return StartCoroutine(FadeVignetteAndPause());

    
    }

    private IEnumerator FadeVignetteAndPause()
    {
        float elapsedTime = 0f;
        float initialIntensity = vignette.intensity.value;

       
        while (elapsedTime < vignetteLerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(initialIntensity, deathVignetteTargetIntensity, elapsedTime / vignetteLerpDuration);
            vignette.intensity.Override(newIntensity);
            yield return null; 
        }

        shotgun.enabled = false;    
        vignette.intensity.Override(deathVignetteTargetIntensity);

        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; 
    }
}
