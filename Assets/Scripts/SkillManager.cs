using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum PlayerSkill //�÷��̾�Ը� ����
{
    meteor,
    fireLaser,
    fireBall,
    none, //=>�÷��̾� ��ų�� �ƴϸ� ���
}

public enum BossSkill //�������Ը� ����
{
    fireBomb,
    lavaBoom,
    fireLine,
    none, //=>���� ��ų�� �ƴϸ� ���
}

public class SkillManager : MonoBehaviour
{
    [Header("��ų �ɼ�")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float downSpeed; //���׿� �������� �ӵ�
    [SerializeField] private float damage = 2f;
    //[SerializeField, Tooltip("���ϱ��� ��� true")] private bool oneTarget = false;
    [SerializeField] private PlayerSkill playerSkill;
    [SerializeField] private BossSkill bossSkill;
    [SerializeField] private float setDotTime; //������ ��Ʈ������ �ð�
    [SerializeField] private float curDotTime; //���� ��Ʈ������ �ð�

    private Vector3 direction;
    private Transform playerPosition;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private bool isStop = false; //��ų �̵� ����
    private bool isRight = true;
    private bool isHit = false; //���ϱ��� ��� ��ų �ϳ��� �Ѹ����� �°� ���ִ� bool��

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && bossSkill == BossSkill.none) //�÷��̾��� ����
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (playerSkill == PlayerSkill.fireBall) //���̾� ���� ��
            {
                if (enemy.PIsSpawnTime() || isHit) //���ϱ��� ��� ����
                {
                    return;
                }
            }

            if (playerSkill == PlayerSkill.fireLaser)
            {
                if (enemy.PIsSpawnTime()) //������ ������ ����� ������ ����
                {
                    return;
                }
            }
            isHit = true;
            enemy.PHit(direction, damage);

            if (playerSkill == PlayerSkill.fireBall)
            {
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.tag == "Player" && playerSkill == PlayerSkill.none)
        {
            Player player = collision.GetComponent<Player>();
            if (bossSkill == BossSkill.fireBomb || bossSkill == BossSkill.lavaBoom)
            {
                player.PHit(1);
            }

            if (bossSkill == BossSkill.fireLine)
            {
                curDotTime -= Time.deltaTime;

                if (curDotTime <= 0f)
                {
                    player.PHit(1); //�ð��� ���� ������ �ֱ�
                }
            }
        }
    }

    private void Awake()
    {
        playerPosition = GameObject.Find("Player").transform; //�÷��̾��� ��ġ�� ����

        if (playerSkill == PlayerSkill.fireBall || playerSkill == PlayerSkill.fireLaser || bossSkill == BossSkill.fireBomb || bossSkill == BossSkill.lavaBoom || bossSkill == BossSkill.fireLine)
        {
            boxCollider = GetComponent<BoxCollider2D>(); //�ݸ��� ��������
        }

        else if (playerSkill == PlayerSkill.meteor)
        {
            circleCollider = GetComponent<CircleCollider2D>(); //�ݸ��� ��������
        }
    }

    private void Update()
    {
        CheckRight();
        MovingPlayerSkill(playerSkill);
        MovingBossSkill(bossSkill);
    }

    /// <summary>
    /// �÷��̾� ��ų�� ������
    /// </summary>
    /// <param name="_skill"></param>
    private void MovingPlayerSkill(PlayerSkill _skill) //��ų �������� ǥ��
    {
        if (isStop)
        { return; }

        switch (_skill)
        {
            case PlayerSkill.fireBall: //��Ÿ
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                break;

            case PlayerSkill.meteor: //���׿�
                transform.position += ((transform.localScale.x > 0) ? Vector3.right : Vector3.left).normalized * moveSpeed * Time.deltaTime;
                transform.position += Vector3.down.normalized * downSpeed * Time.deltaTime;
                break;

            case PlayerSkill.fireLaser: //�ұ�� ��
                transform.position = playerPosition.position;
                transform.Rotate(new Vector3(0f, 0f, (isRight == true ? 360f : -360f) * Time.deltaTime * moveSpeed));
                break;

            case PlayerSkill.none:
                break;
        }
    }

    /// <summary>
    /// ���� ��ų�� ������
    /// </summary>
    /// <param name="_skill"></param>
    private void MovingBossSkill(BossSkill _skill)
    {
        if (isStop)
        { return; }

        switch (_skill)
        {
            case BossSkill.fireBomb:
                transform.position += Vector3.down * Time.deltaTime * downSpeed;
                break;

            case BossSkill.none:
                break;
        }
    }

    /// <summary>
    /// ��ų�� ���� ����
    /// </summary>
    private void CheckRight()
    {
        if (transform.localScale.x >= 0f)
        {
            isRight = true;
        }

        else
        {
            isRight = false;
        }
    }

    public void PSetDirection(Vector3 _direction)
    {
        direction = _direction;
    }

    /// <summary>
    /// ��ų�� ���󰡴� ���� ����
    /// </summary>
    /// <returns></returns>
    public Vector3 PGetDirection()
    {
        return direction;
    }

    /// <summary>
    /// �÷��̾� Ȥ�� ���Ϳ� ������ ���ݷ��� ��ų�� �ο�
    /// </summary>
    /// <param name="_damage"></param>
    public void PGetDamage(float _damage)
    {
        damage += _damage;
    }

    /// <summary>
    /// ���� ��ų�� �������� �����ϴ� �̺�Ʈ ��ư
    /// - ���� �ִϸ��̼ǿ� ���
    /// </summary>
    public void BStopMoving()
    {
         isStop = true;
        
        if (playerSkill == PlayerSkill.fireBall)
        {
            boxCollider.enabled = false;
        }

        else if (playerSkill == PlayerSkill.meteor)
        {
            circleCollider.enabled = true;
        }
    }

    public void BGetTrigger()
    {
        boxCollider.enabled = true;
    }

    /// <summary>
    /// ��ų�� ������ �ı��ϴ� �̺�Ʈ ��ư
    /// </summary>
    public void BDestroying()
    {
        Destroy(gameObject);
    }
}
