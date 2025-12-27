using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button muteButton;

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = MusicManager.Instance != null ? MusicManager.Instance.GetVolume() : 0.5f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (muteButton != null)
        {
            muteButton.onClick.AddListener(OnMuteToggle);
        }
    }

    private void OnVolumeChanged(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(value);
        }
    }

    private void OnMuteToggle()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMute();
        }
    }

    void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }

        if (muteButton != null)
        {
            muteButton.onClick.RemoveListener(OnMuteToggle);
        }
    }
}
