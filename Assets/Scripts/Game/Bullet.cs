using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 14; // скорость пули
    [SerializeField] private GameObject _destroyEffec; // эфект уничтожения
    public int _damage = 3;     // урон

    private Subject<MonoBehaviour> _putMe = new Subject<MonoBehaviour>(); // событие для подписи SpawnManager
    public IObservable<MonoBehaviour> PutMe => _putMe;
    private float _goTo;    // положение в которое нужно прилететь
    public bool _isEnemy;   // для понимания, чья это пуля

    private void OnEnable()
    {
        var controller = Controller.Instance;
        _goTo = controller.LeftUpPoint.y + 2;
        StartCoroutine(Move());
    }

    // Корутина - используется для параллельной работы
    private IEnumerator Move()
    {
        if (_isEnemy)
        {
            while (transform.position.y > -_goTo)
            {
                transform.position -= new Vector3(0, Time.deltaTime * _speed, 0);
                // пропустим кадр (чтоб не зависало)
                yield return null;
            }
        }
        else
        {
            while (transform.position.y < _goTo)
            {
                transform.position += new Vector3(0, Time.deltaTime * _speed, 0);
                // пропустим кадр (чтоб не зависало)
                yield return null;
            }
        }

        // рассылаем событие
        _putMe.OnNext(this);
    }

    public void HitMe()
    {
        var pos = transform.position;
        Instantiate(_destroyEffec, new Vector3(pos.x, pos.y, -2), transform.rotation);

        _putMe.OnNext(this);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
