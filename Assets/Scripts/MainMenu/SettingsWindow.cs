using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderEffect;

    private void Awake()
    {
        _sliderMusic.value = Controller.Instance.MusicSource.volume;
        _sliderEffect.value = Controller.Instance.EffectSource.volume;
    }

    public void ChangeValueMusic(float value)
    {
        Controller.Instance.MusicSource.volume = value;
    }

    public void ChangeValueEffect(float value)
    {
        Controller.Instance.EffectSource.volume = value;
    }
}
