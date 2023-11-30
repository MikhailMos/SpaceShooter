using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _effects;
    [SerializeField] private float _speed = 15;
    [SerializeField] private float _coolDown = 0.1f;    // время между выстрелами
    public int _maxHealth = 100;
    [SerializeField] private float _shipRollEuler = 45; // на сколько градусов поварачивает корабль при повороте или движении
    [SerializeField] private float _shipRollSpeed = 80; // скорость вращения корабля
    [SerializeField] private float _smothness = 1.2f;   // плавность движения корабля

    private Subject<Unit> _fireClick = new Subject<Unit>();
    public IObservable<Unit> FireClick => _fireClick;

    private Rigidbody2D _rigidbody;         // хранит риджитбоди корабля
    private float _coolDownCurrent = 10;    // текущее время между выстрелами
    private MeshRenderer _mR;               // отображение 3х мерных объектов (в данном случае корабля)
    private Vector3 _sizeWorldShip;         // размеры корабля по 3м осям
    private Controller _controller;         // хранит контроллер для удобства, чтоб не обращаться через точку (Controller.Instance)

    [HideInInspector] public ReactiveProperty<int> _health = new ReactiveProperty<int>();

    private void Awake()
    {
        if (Controller.Instance == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        _rigidbody = GetComponent<Rigidbody2D>();
        _mR = GetComponent<MeshRenderer>();
        _controller = Controller.Instance;
        _controller._myShip = this;
        _sizeWorldShip = _mR.bounds.extents;
    }

    private void Start()
    {
        _controller.UpdateCameraSettings();
        _health.Value = _maxHealth;
    }

    private void Update()
    {
        UpdateKey();
        FireButtonClick();
    }

    private void FireButtonClick()
    {
        if (Input.GetMouseButton(0))
        {
            if (_coolDownCurrent >= _coolDown) {
                _coolDownCurrent = 0;
                _fireClick.OnNext(Unit.Default);
            }
        }

        if (_coolDownCurrent < _coolDown)
        {
            _coolDownCurrent += Time.deltaTime;
        }
    }

    private void UpdateKey()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // летим вперед
        if (moveVertical > 0)
        {
            _effects[4].Play();
        } else
        {
            _effects[4].Stop();
        }
        
        // летим назад
        if (moveVertical < 0)
        {
            _effects[2].Play();
            _effects[3].Play();
        } else 
        { 
            _effects[2].Stop(); 
            _effects[3].Stop(); 
        }
        
        // летим влево
        if (moveHorizontal < 0)
        {
            _effects[0].Play();
        }
        else
        {
            _effects[0].Stop();
        }
        
        // летим вправо
        if (moveHorizontal > 0) 
        {
            _effects[1].Play();
        } else
        {
            _effects[1].Stop();
        }

        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, new Vector2(moveHorizontal * _speed * 1.2f, moveVertical * _speed), _smothness);

        transform.position = CheckBoardWorld();

        var targetRotation = Quaternion.Euler(0, 180 + (-moveHorizontal * _shipRollEuler), 0);
        // для плавного вращения
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _shipRollSpeed * Time.deltaTime);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            LevelManager.PlayScene(Scenes.MainMenu);
        }
    }

    private Vector3 CheckBoardWorld()
    {
        var pos = transform.position;
        var x = pos.x;
        var y = pos.y;
        
        x = Mathf.Clamp(x, _controller.LeftDowndPoint.x + _sizeWorldShip.x, _controller.RightDownPoint.x - _sizeWorldShip.x);
        y = Mathf.Clamp(y, _controller.LeftDowndPoint.y + _sizeWorldShip.y, _controller.LeftUpPoint.y - _sizeWorldShip.y);

        return new Vector3(x, y, 0);
    }

    public void DamageMe(int damage)
    {
        _health.Value -= damage;
        if (_health.Value <=  0 ) 
        {
            var tr = transform;
            var position = tr.position;
            gameObject.SetActive(false);
            _controller.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("EnemyBullet")) 
        {
            var bull = obj.GetComponent<Bullet>();
            DamageMe(bull._damage);
            bull.HitMe();
        }

        if (obj.CompareTag("AddHealth"))
        {
            var bonus = obj.GetComponent<HealthBonus>();
            bonus.CallMoveToBar();
            _health.Value += bonus.Health;
            if (_health.Value > _maxHealth ) 
            { 
                _health.Value = _maxHealth;
            }
        }
    }
}
