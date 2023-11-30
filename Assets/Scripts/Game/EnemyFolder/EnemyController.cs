using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

// Контроллирует, сколько врагов и когда они оживут
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _minDelay = 2; // минималь промежуток появления врагов (сек)
    [SerializeField] private float _maxDelay = 4; // максимальный промежутов появления врагов (сек)
    [SerializeField] private int _maxCountOneSpawn = 5; // макс. кол-во врагов за один спавн
    private float _timeDelay;   // текущее время таймера
    private int _countOnPull;   // текущее кол-во кораблей
    private SpawnManager _spawnManager; // ссылка на SpawnManager
    private CompositeDisposable _disposablesEnemy = new CompositeDisposable();
    private Coroutine _coroutine;

    private void OnEnable()
    {
        _disposablesEnemy = new CompositeDisposable();
        _coroutine = StartCoroutine(SpawnEnemy());
    }

    private void Awake()
    {
        _spawnManager = GetComponent<SpawnManager>();
        _timeDelay = Random.Range(_minDelay, _maxDelay);

    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            _timeDelay -= Time.deltaTime;
            if (_timeDelay < 0 )
            {
                _countOnPull = Random.Range(1, _maxCountOneSpawn);
                _timeDelay = Random.Range(_minDelay, _maxDelay);

                for (int i = 0; i < _countOnPull; i++)
                {
                    var hunter = _spawnManager.SpawnEnemy();
                    
                    if (hunter != null)
                    {
                        hunter.Fire.Subscribe((param) => Fire(param.Item1, param.Item2)).AddTo(_disposablesEnemy);
                    }

                    yield return null;
                }
                _countOnPull = Random.Range(1, _maxCountOneSpawn);
            }

            yield return null;
        }
        
    }

    private void Fire(Transform tr, Bullet bullet)
    {
        _spawnManager.SpawnBullet(tr, bullet);
    }

    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _disposablesEnemy.Dispose();
        _disposablesEnemy = null;
    }
}
