using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [Header("스킬 옵션")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float damage = 2f;

    private bool isStop = false;
    private Vector3 direction;
    private BoxCollider2D boxCollider;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.PIsSpawnTime()) //몬스터가 스폰 중일 경우
            {
                return;
            }
            enemy.PHit(direction, damage);
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        Moving();
    }

    private void Moving()
    {
        if (isStop)
        { return; }
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
    }

    public void PSetDirection(Vector3 _direction)
    {
        direction = _direction;
    }

    public Vector3 PGetDirection()
    {
        return direction;
    }

    public void BStopMoving()
    {
        isStop = true;
        boxCollider.enabled = false;
    }

    public void BDestroying()
    {
        Destroy(gameObject);
    }
}
