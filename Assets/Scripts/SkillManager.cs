using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum Skill
{
    metemor,
    fireLaser,
    fireBall,
}

public class SkillManager : MonoBehaviour
{
    [Header("��ų �ɼ�")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float downSpeed; //���׿� �������� �ӵ�
    [SerializeField] private float damage = 2f;
    //[SerializeField, Tooltip("���ϱ��� ��� true")] private bool oneTarget = false;
    [SerializeField] private Skill skill;

    private Vector3 direction;
    private Transform playerPosition;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private bool isStop = false;
    private bool isHit = false; //���ϱ��� ��� ��ų �ϳ��� �Ѹ����� �°� ���ִ� bool��

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.PIsSpawnTime() || isHit) //���Ͱ� ���� ���� ���
            {
                if (skill == Skill.fireBall) //���ϱ��� ��� ����
                {
                    return;
                }
            }
            isHit = true;
            enemy.PHit(direction, damage);

            if (skill == Skill.fireBall)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Awake()
    {
        playerPosition = GameObject.Find("Player").transform; //�÷��̾��� ��ġ�� ����

        if (skill == Skill.fireBall || skill == Skill.fireLaser)
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
                transform.position += Vector3.down.normalized * downSpeed * Time.deltaTime;
                break;

            case Skill.fireLaser:
                transform.position = playerPosition.position;
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

    /// <summary>
    /// ��ų�� ������ �ı��ϴ� �̺�Ʈ ��ư
    /// </summary>
    public void BDestroying()
    {
        Destroy(gameObject);
    }
}
