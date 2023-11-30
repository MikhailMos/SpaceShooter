using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;

public enum StageShip
{
    In, Wait, Out
}

public abstract class BaseEnemyShip : MonoBehaviour
{
    [Header("Base Fields")]
    [SerializeField] private float _normalSpeed = 8;
    [SerializeField] private float _delayTurbo = 2; // время (сек) через которое корабль будет улетать либо в игрока, либо за пределы экрана
    [SerializeField] private float _turboSpeed = 5; // скорость корабаля, когда он покидает видимую зону камеры
    [SerializeField] private float _speedRotation = 0.01f; // скорость перемещения?
    [SerializeField] private int _collisionDamage = 10; // урон наносмый кораблем при столкновении с игроком
    [SerializeField] private int _maxhealth = 2; // макс. жизнь врага
    [SerializeField] private int _costPointesScore = 5; // очки зачисляемые игроку, при уничтожении вражеского корабля
    [SerializeField] private ParticleSystem _fireEngin; // анимация огня двигателя
    [SerializeField] private GameObject _destroyEffect; // эфект взрыва

    public int CostPointerScore => _costPointesScore;
    
    [HideInInspector] public PlayerShip _player;
    [HideInInspector] public Transform _myRoot;
    [HideInInspector] public Vector3 _playerLastPos = Vector3.up;   // последняя позиция игрока

    private Subject<MonoBehaviour> _putMe = new Subject<MonoBehaviour>();
    public IObservable<MonoBehaviour> PutMe => _putMe;

    private Vector3 DirectionToPlayer => transform.position - new Vector3(_playerLastPos.x, _playerLastPos.y, 0);
    private int _health = 100; // текущее значение жизни корабля
    private float _goTo; // конечная точка движения вражеского корабля
    private float _goToPointTurbo; // значние точки, где вражеский корабль должен остановиться и следить за кораблем игрока
    private float _timerDelay; // текущее показание таймера

    private IEnumerator Core()
    {
        UpdateStage(StageShip.In);
        _fireEngin.Play();
        while (transform.position.y > _goToPointTurbo)
        {
            transform.position -= new Vector3(0, Time.deltaTime * _normalSpeed, 0);
            Look(new Vector3(0, _goToPointTurbo, 0));
            // пропускаем кадр
            yield return null;
        }

        UpdateStage(StageShip.Wait);
        _fireEngin.Stop();
        while (_timerDelay < _delayTurbo) 
        {
            _timerDelay += Time.deltaTime;
            // пропускаем кадр
            yield return null;
        }

        UpdateStage(StageShip.Out);
        _fireEngin.Play();
        if (_playerLastPos != Vector3.up)
        {
            // поведение камикадзе
            var dir = DirectionToPlayer / DirectionToPlayer.magnitude;
            while (transform.position.y > _goTo && transform.position.y < -_goTo)
            {
                Look(dir);
                transform.position -= dir * (Time.deltaTime * _turboSpeed);
                // пропускаем кадр
                yield return null;
            }
        } else
        {
            while (transform.position.y > _goTo)
            {
                transform.position -= new Vector3(0, Time.deltaTime * _turboSpeed, 0);
                // пропускаем кадр
                yield return null;
            }
        }

        _putMe.OnNext(this);
    }

    private void OnEnable()
    {
        _timerDelay = 0;
        var controller = Controller.Instance;
        _goTo = controller.RightDownPoint.y - 2;
        _goToPointTurbo = UnityEngine.Random.Range((controller.CenterCam.y + 1), (controller.LeftUpPoint.y - 1));
        _health = _maxhealth;
        
        StartCoroutine(Core());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected void Look(Vector3 dir, bool lerp = false, bool invertion = false)
    {
        float signedAngle = Vector2.SignedAngle(Vector2.down, dir);
        if (invertion)
        {
            signedAngle += 180;
        }
        if (Mathf.Abs(signedAngle) >= 1e-3f)
        {
            var angles = transform.eulerAngles;
            angles.z = signedAngle;
            if (lerp)
            {
                // поворачиваем объект
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, angles, _speedRotation);
            } else
            {
                transform.eulerAngles = angles;

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Bullet"))
        {
            var bull = obj.GetComponent<Bullet>();
            bull.HitMe();
            DamageMe(bull._damage, this);
            return;
        }
        if (obj.CompareTag("Player"))
        {
            obj.GetComponent<PlayerShip>().DamageMe(_collisionDamage);
            Controller.Instance.Score.Value += (_costPointesScore / 2);
            SpawnDestroyEffect();
            _putMe.OnNext(this);
        }
    }

    private void SpawnBonus()
    {
        var random = UnityEngine.Random.Range(0, 100);
        if (random < Controller.Instance._procentBounsHealth)
        {
            Instantiate(Controller.Instance._healBonusPref, transform.position, new Quaternion(0, 0, 0, 0));
        }
    }

    private void SpawnDestroyEffect()
    {
        var pos = transform.position;
        Instantiate(_destroyEffect, new Vector3(pos.x, pos.y, -2), transform.rotation);
    }

    private void DamageMe(int damage, BaseEnemyShip baseEnemy)
    {
        _health -= damage;
        if (_health <= 0)
        {
            _health = _maxhealth;
            SpawnBonus();
            SpawnDestroyEffect();
            Controller.Instance.Score.Value += _costPointesScore;
            _putMe.OnNext(this);
        }
    }

    protected abstract void UpdateStage(StageShip stage);
}
