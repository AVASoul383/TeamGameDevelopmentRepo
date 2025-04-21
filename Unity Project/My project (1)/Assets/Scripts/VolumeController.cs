using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] float multplier = 20f;
    [SerializeField] Toggle volumeToggle;
    [SerializeField] TMP_Text volumeAmt;

    private bool disableToggleEvent;
    private float baseVol;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        volumeSlider.onValueChanged.AddListener(HandleSliderValueChange);
        volumeToggle.onValueChanged.AddListener(HandleToggleValueChange);
    }

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, volumeSlider.value);
        volumeAmt.text = (volumeSlider.value * 100).ToString("F0");
        baseVol = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleSliderValueChange(float value)
    {
        audioMixer.SetFloat(volumeParameter, Mathf.Log10(value) * multplier);
        disableToggleEvent = true;
        volumeToggle.isOn = volumeSlider.value > volumeSlider.minValue;
        disableToggleEvent = false;
        volumeAmt.text = (volumeSlider.value * 100).ToString("F0");
    }

    private void HandleToggleValueChange(bool enabled)
    {
        if (disableToggleEvent)
            return;

        if (enabled)
        {
            volumeSlider.value = baseVol;
        }
        else
        {
            volumeSlider.value = volumeSlider.minValue;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, volumeSlider.value);
    }

    public float volume { get { return volumeSlider.value;} }
}
