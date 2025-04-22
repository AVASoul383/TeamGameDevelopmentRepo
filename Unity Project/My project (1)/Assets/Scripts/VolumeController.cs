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
    private float baseVol = 0.5f;

    void Awake()
    {
        volumeSlider.onValueChanged.AddListener(HandleSliderValueChange);
        volumeToggle.onValueChanged.AddListener(HandleToggleValueChange);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, volumeSlider.value);
        volumeAmt.text = (volumeSlider.value * 100).ToString("F0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleSliderValueChange(float value)
    {
        audioMixer.SetFloat(volumeParameter, value:Mathf.Log10(value) * multplier);
        disableToggleEvent = true;
        volumeToggle.isOn = volumeSlider.value > volumeSlider.minValue;
        disableToggleEvent = false;
        volumeAmt.text = (volumeSlider.value * 100).ToString("F0");
    }

    private void HandleToggleValueChange(bool enabledSound)
    {
        if (disableToggleEvent)
            return;

        if (enabledSound)       
            volumeSlider.value = baseVol;        
        else       
            volumeSlider.value = volumeSlider.minValue;        
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, volumeSlider.value);
    }

}
