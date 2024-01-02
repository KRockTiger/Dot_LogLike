using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum Skill
{
    metemor,
    skillE,
    fireBall,
    skillM1,
}

public class SkillManager : MonoBehaviour
{
    [Header("스킬 옵션")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float damage = 2f;
    //[SerializeField, Tooltip("단일기일 경우 true")] private bool oneTarget = false;
    [SerializeField] private Skill skill;

    private bool isStop = false;
    private Vector3 direction;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private bool isHit = false; //단일기일 경우 스킬 하나에 한마리만 맞게 해주는 bool형

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.PIsSpawnTime() || isHit) //몬스터가 스폰 중일 경우
            {
                return;
            }
            isHit = true;
            enemy.PHit(direction, damage);
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        if (skill == Skill.fireBall)
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        else if (skill == Skill.metemor)
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }
    }

    private void Update()
    {
        Moving(skill);
    }

    private void Moving(Skill _skill)
    {
        if (isStop)
        { return; }

        switch (_skill)
        {
            case Skill.fireBall:
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                break;

            case Skill.metemor:
                transform.position += ((transform.localScale.x > 0) ? Vector3.right : Vector3.left).normalized * moveSpeed * Time.deltaTime;
                transform.position += Vector3.down.normalized * moveSpeed * Time.deltaTime;
                break;
        }
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
        
        if (skill == Skill.fireBall)
        {
            boxCollider.enabled = false;
        }

        else if (skill == Skill.metemor)
        {
            circleCollider.enabled = true;
        }
    }

    public void BDestroying()
    {
        Destroy(gameObject);
    }
}
