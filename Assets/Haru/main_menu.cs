using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEditor.UI;

public class VolumeControl : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private string volumeParameter = "MasterVolume";

    [Header("Camera Settings")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private string sensitivityParameter = "CameraSensitivity";
    [SerializeField] private float minSensitivity = 10f;
    [SerializeField] private float maxSensitivity = 400;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private Button quitBtn;

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, 0.75f);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        SetVolume(volumeSlider.value);

        if (cameraController == null)
        {
            Debug.LogError("CameraController not found in the scene!");
        }

        float savedSensitivity = PlayerPrefs.GetFloat(sensitivityParameter, 0.5f);
        sensitivitySlider.value = savedSensitivity;
        sensitivitySlider.onValueChanged.AddListener(SetCameraSensitivity);
        SetCameraSensitivity(savedSensitivity);



    }

    public void SetVolume(float volume)
    {
        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat(volumeParameter, dB);

        PlayerPrefs.SetFloat(volumeParameter, volume);
        PlayerPrefs.Save();
    }

    public void SetCameraSensitivity(float normalizedSensitivity)
    {
        float sensitivity = Mathf.Lerp(minSensitivity, maxSensitivity, normalizedSensitivity);

        PlayerPrefs.SetFloat(sensitivityParameter, normalizedSensitivity);
        PlayerPrefs.Save();

        if (cameraController != null)
        {
            cameraController.sensX = sensitivity;
            cameraController.sensY = sensitivity;
        }
        else
        {
            Debug.LogError("CameraController is not assigned!");
        }
    }

}