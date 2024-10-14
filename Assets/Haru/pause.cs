using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class pause : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Canvas scoreCanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(pauseThing());
        }
    }


    private IEnumerator pauseThing()
    {
        pauseCanvas.gameObject.SetActive(!pauseCanvas.gameObject.activeSelf);
        yield return new WaitForSeconds(0.5f);
        scoreCanvas.gameObject.SetActive(!scoreCanvas.gameObject.activeSelf);
        Time.timeScale = Mathf.Abs(Time.timeScale - 1);
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
