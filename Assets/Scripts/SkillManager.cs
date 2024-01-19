using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum PlayerSkill //플레이어에게만 적용
{
    meteor,
    fireLaser,
    fireBall,
    none, //=>플레이어 스킬이 아니면 등록
}

public enum BossSkill //보스에게만 적용
{
    fireBomb,
    lavaBoom,
    fireLine,
    none, //=>보스 스킬이 아니면 등록
}

public class SkillManager : MonoBehaviour
{
    [Header("스킬 옵션")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float downSpeed; //메테오 떨어지는 속도
    [SerializeField] private float damage = 2f;
    //[SerializeField, Tooltip("단일기일 경우 true")] private bool oneTarget = false;
    [SerializeField] private PlayerSkill playerSkill;
    [SerializeField] private BossSkill bossSkill;

    private Vector3 direction;
    private Transform playerPosition;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private bool isStop = false;
    private bool isRight = true;
    private bool isHit = false; //단일기일 경우 스킬 하나에 한마리만 맞게 해주는 bool형

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && bossSkill == BossSkill.none) //플레이어의 공격
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.PIsSpawnTime() || isHit) //몬스터가 스폰 중일 경우
            {
                if (playerSkill == PlayerSkill.fireBall) //단일기일 경우 막기
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
    }

    private void Awake()
    {
        playerPosition = GameObject.Find("Player").transform; //플레이어의 위치를 저장

        if (playerSkill == PlayerSkill.fireBall || playerSkill == PlayerSkill.fireLaser)
        {
            boxCollider = GetComponent<BoxCollider2D>(); //콜리더 가져오기
        }

        else if (playerSkill == PlayerSkill.meteor)
        {
            circleCollider = GetComponent<CircleCollider2D>(); //콜리더 가져오기
        }
    }

    private void Update()
    {
        CheckRight();
        MovingPlayerSkill(playerSkill);
        MovingBossSkill(bossSkill);
    }

    /// <summary>
    /// 플레이어 스킬의 움직임
    /// </summary>
    /// <param name="_skill"></param>
    private void MovingPlayerSkill(PlayerSkill _skill) //스킬 움직임을 표현
    {
        if (isStop)
        { return; }

        switch (_skill)
        {
            case PlayerSkill.fireBall: //평타
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                break;

            case PlayerSkill.meteor: //메테오
                transform.position += ((transform.localScale.x > 0) ? Vector3.right : Vector3.left).normalized * moveSpeed * Time.deltaTime;
                transform.position += Vector3.down.normalized * downSpeed * Time.deltaTime;
                break;

            case PlayerSkill.fireLaser: //불기둥 샷
                transform.position = playerPosition.position;
                transform.Rotate(new Vector3(0f, 0f, (isRight == true ? 360f : -360f) * Time.deltaTime * moveSpeed));
                break;

            case PlayerSkill.none:
                break;
        }
    }

    /// <summary>
    /// 보스 스킬의 움직임
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
    /// 스킬의 방향 설정
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
    /// 스킬이 날라가는 방향 설정
    /// </summary>
    /// <returns></returns>
    public Vector3 PGetDirection()
    {
        return direction;
    }

    /// <summary>
    /// 보스 스킬의 움직임을 제한하는 이벤트 버튼
    /// - 일정 애니메이션에 사용
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

    /// <summary>
    /// 스킬이 끝나면 파괴하는 이벤트 버튼
    /// </summary>
    public void BDestroying()
    {
        Destroy(gameObject);
    }
}
