using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;

    private Transform trsPlayer; //플레이어 위치
    private bool isHitting = false; //넉백 확인
    private bool isDeath = false; //사망 확인
    [SerializeField] private bool isSpawnTime = true; //스폰 확인
    private Vector3 enemyScale;
    private Vector3 hitDirection; //넉백 방향
    private SpriteRenderer spriteRenderer;
    private Color defColor; //현 몬스터의 컬러
    private Color hitColor = Color.red; //피격 당했을 때 나타내는 컬러
    private Color spawnColor; //스폰타임일 때 컬러

    [Header("적 몬스터 스탯")]
    [SerializeField] private float moveSpeed = 5f; //이동속도
    [SerializeField] private float maxHP = 10f; //적용시킬 몬스터 체력
    [SerializeField] private float curHP; //현재 체력
    [SerializeField] private float damage = 2f; //몬스터의 공격력
    [SerializeField] private float hitTime = 0f; //넉백 시간
    [SerializeField] private int haveCoin = 10; //현재 몬스터가 가지고 있는 코인 갯수
    [SerializeField] private bool isBoss = false; //보스 여부

    [Header("테스트용")]
    [SerializeField] private bool skillTest = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player sc = collision.GetComponent<Player>();
            sc.PHit(damage);
        } //=> 플레이어 태그를 감지하고 스크립트에 접근하여 데미지 주기
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyScale = transform.localScale;
        curHP = maxHP;

        if (isBoss)
        {
            return;
        }

        //스폰될 때 랜덤으로 바라볼 방향을 결정
        int randDirenction = Random.Range(0, 2);
        if (randDirenction == 0)
        {
            transform.localScale = new Vector3(-enemyScale.x, enemyScale.y, 0);
        }

        else
        {
            transform.localScale = new Vector3(enemyScale.x, enemyScale.y, 0);
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        trsPlayer = GameObject.Find("Player").transform;
        defColor = spriteRenderer.color;

        if (!isSpawnTime || isBoss)
        {
            return;
        }
        spawnColor = new Color(defColor.r, defColor.g, defColor.b, 0.5f);
        spriteRenderer.color = spawnColor;
    }

    private void Update()
    {
        if (skillTest || isBoss)
        { return; }

        if (isSpawnTime)
        {
            Invoke("SpawnTime", 1f);
            return;
        }
        Moving();
        CheckDeath();
    }

    /// <summary>
    /// 스폰하는 시간
    /// </summary>
    private void SpawnTime()
    {
        spriteRenderer.color = defColor;
        isSpawnTime = false;
    }

    /// <summary>
    /// 몬스터가 플레이어를 추적하는 기능
    /// </summary>
    private void Moving()
    {
        Vector3 direction = trsPlayer.position - transform.position;

        if (!isHitting) //피격 중이 아닌 경우
        {
            transform.position += new Vector3(direction.x, direction.y, 0).normalized * moveSpeed * Time.deltaTime;

            if (direction.x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-enemyScale.x, enemyScale.y, 1);
            }

            else if (direction.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(enemyScale.x, enemyScale.y, 1);
            }
        }

        else //피격 중인 경우
        {
            spriteRenderer.color = hitColor; //현재 스프라이트를 피격 컬러로 설정
            transform.position += hitDirection.normalized * moveSpeed * 1.2f * Time.deltaTime; //넉백

            if (hitDirection.x < 0) //피격 방향이 왼쪽이면 오른쪽 방향으로 바라보기
            {
                transform.localScale = new Vector3(enemyScale.x, enemyScale.y, 1);
            }

            else if (hitDirection.x > 0) //피격 방향이 오른쪽이면 왼쪽 방향으로 바라보기
            {
                transform.localScale = new Vector3(-enemyScale.x, enemyScale.y, 1);
            }

            hitTime -= Time.deltaTime; //넉백 시간
            if (hitTime <= 0f)
            {
                spriteRenderer.color = defColor; //스프라이트 복구
                isHitting = false; //피격 판정 끄기
            }
        }
    }

    /// <summary>
    /// 몬스터 죽음 확인
    /// </summary>
    private void CheckDeath()
    {
        if (curHP <= 0f)
        {
            Destroy(gameObject);

            if (isDeath)
            {
                return;
            }

            gameManager.PSetCoin(haveCoin);
            isDeath = true;
        }
    }

    /// <summary>
    /// 몬스터 피격
    /// </summary>
    /// <param 피격 방향="_direction">피격을 당한 방향을 나타냄</param>
    public void PHit(Vector3 _direction, float _damage)
    {
        if (skillTest)
        {
            Debug.Log("몬스터가 스킬 영향을 받았습니다.");
            return;
        }

        curHP -= _damage;
        hitDirection = _direction; //피격 방향 저장
        hitTime = 0.2f; //피격 시간
        isHitting = true; //피격 판정 키기
    }

    public bool PIsSpawnTime()
    {
        return isSpawnTime;
    }
}
