using UnityEngine;
using UnityEngine.VFX;

public class DestroyIT : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject,5f);
    }
}
