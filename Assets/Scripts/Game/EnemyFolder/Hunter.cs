using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Hunter : BaseEnemyShip
{
    [SerializeField] private Bullet _bulletPref;
    [SerializeField] private float _coolDown = 1.3f; // скорость стрельбы
    private float _coolDownCurrent = 10;
    private StageShip _currentStage;

    private Subject<(Transform, Bullet)> _fire = new Subject<(Transform, Bullet)> ();
    public IObservable<(Transform, Bullet)> Fire => _fire;

    private IEnumerator LocalUpdate()
    {
        while (_currentStage == StageShip.Wait)
        {
            if (_coolDownCurrent < _coolDown)
            {
                _coolDownCurrent += Time.deltaTime;
            } else
            {
                _coolDownCurrent = 0;
                _fire.OnNext((transform, _bulletPref));
            }
            yield return null;
        }
    }

    protected override void UpdateStage(StageShip stage)
    {
        _currentStage = stage;
        if (stage == StageShip.Wait)
        {
            StartCoroutine(LocalUpdate());
        }
    }
}
