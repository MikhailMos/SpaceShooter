using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameUIController : MonoBehaviour
{
    [SerializeField] Text _countHealth;
    [SerializeField] Text _countScore;
    [SerializeField] Slider _healthSlider;
    [SerializeField] Text _countScoreWindowGameOver;
    [SerializeField] GameObject _windowGameOver;

    private CompositeDisposable _disposable;

    private void Start()
    {
        _disposable = new CompositeDisposable();
        var controller = Controller.Instance;

        controller.OnGameOver.Subscribe((_) => ShowWindowGameOver()).AddTo(_disposable);
        controller._myShip._health.Subscribe(UpdateBar).AddTo(_disposable);
        controller.Score.Subscribe(UpdateScore).AddTo(_disposable);
    }

    private void UpdateBar(int value)
    {
        _healthSlider.value = ((float)value) / 100;

        if (value < 0)
        {
            value = 0;
        }
        _countHealth.text = value.ToString();
    }

    private void UpdateScore(int score) 
    {
        if (!_windowGameOver.activeSelf)
        {
            _countScore.text = score.ToString();
        }
    }

    public void ShowWindowGameOver()
    {
        _countScoreWindowGameOver.text = Controller.Instance.Score.Value.ToString();
        _windowGameOver.SetActive(true);
    }

    public void ClickToMainMenu()
    {
        LevelManager.PlayScene(Scenes.MainMenu);
        gameObject.SetActive(false);
    }

    public void ClickRestart()
    {
        LevelManager.PlayScene(Scenes.Game);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_disposable != null)
        {
            _disposable.Dispose();
            _disposable = null;
        }
        
    }
}
