using System.Collections;
using UnityEngine;

public class HealthBonus : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turboSpeed = 15;
    [SerializeField] private float _stopDistance = 0.5f;
    [SerializeField] private int _addHealth;

    private Transform _healthBar;
    private Vector3 _goTo;
    public int Health => _addHealth;

    private void OnEnable()
    {
        _healthBar = GameObject.Find("BgIcon").transform;
        Controller controller = Controller.Instance;
        _goTo = new Vector3(0, controller.RightDownPoint.y - 2, 0);
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (transform.position.y > _goTo.y)
        {
            transform.position -= new Vector3(0, Time.deltaTime * _speed, 0);
            yield return null;
        }
        
        Destroy(gameObject);
    }

    public void CallMoveToBar()
    {
        StopAllCoroutines();
        
        _goTo = _healthBar.transform.position;
        GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(MoveToBar());
    }

    private IEnumerator MoveToBar()
    {
        var tr = transform;
        var position = tr.position;
        var absoluteDir = position - _goTo;
        var dir = absoluteDir / absoluteDir.magnitude;
        while (Vector3.Distance(transform.position, _goTo) > _stopDistance)
        {
            transform.position -= dir * (Time.deltaTime * _turboSpeed);
            yield return null;
        }
        Destroy(gameObject);
    }

}
