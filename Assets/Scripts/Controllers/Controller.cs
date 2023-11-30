using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Controller : MonoBehaviour
{
    public HealthBonus _healBonusPref;
    public int _procentBounsHealth = 30;
    
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _effectSource;
    [SerializeField] private AudioClip _clipShot;

    public ReactiveProperty<int> Score = new ReactiveProperty<int>();
    public AudioSource MusicSource => _musicSource;
    public AudioSource EffectSource => _effectSource;

    public static Controller Instance;
    public PlayerShip _myShip;

    private Vector3 _leftDownPoint;
    private Vector3 _rightDownPoint;
    private Vector3 _leftUpPoint;
    private Vector3 _rightUpPoint;
    private Vector2 _centerCam;

    public Vector3 LeftDowndPoint => _leftDownPoint;
    public Vector3 RightDownPoint => _rightDownPoint;
    public Vector3 LeftUpPoint => _leftUpPoint;
    public Vector3 RightUpPoint => _rightUpPoint;
    public Vector2 CenterCam => _centerCam;

    private Subject<Unit>_gameOver = new Subject<Unit>();
    public IObservable<Unit> OnGameOver => _gameOver;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCameraSettings()
    {
        var cameraMain = Camera.main;

        if (cameraMain != null)
        {
            _centerCam = cameraMain.transform.position;

            float distance = cameraMain.farClipPlane;
            _leftDownPoint = cameraMain.ScreenToWorldPoint(new Vector3(0, 0, distance));
            _leftUpPoint = cameraMain.ScreenToWorldPoint(new Vector3(0, cameraMain.pixelHeight, distance));
            _rightUpPoint = cameraMain.ScreenToWorldPoint(new Vector3(cameraMain.pixelWidth, cameraMain.pixelHeight, distance));
            _rightDownPoint = cameraMain.ScreenToWorldPoint(new Vector3(cameraMain.pixelWidth, 0, distance));
        }
    }

    public void PlayAudioShot()
    {
        _effectSource.PlayOneShot(_clipShot);
    }

    public void GameOver()
    {
        _gameOver.OnNext(Unit.Default);
    }
}
