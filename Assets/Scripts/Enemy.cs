using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float hitTime = 0f; //넉백 시간
    [SerializeField] private int damage = 1; //몬스터의 공격력
    [SerializeField] private int haveCoin = 10; //현재 몬스터가 가지고 있는 코인 갯수
    [SerializeField, Tooltip("보스 몬스터일 경우 true")] private bool isBoss = false; //보스 여부
    [SerializeField, Tooltip("보스 몬스터만 사용")] private Image bossHPImage; //보스 체력 이미지

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
            gameManager.PSetBossBattle(true); //보스전 키기
            return;
        }
        spawnColor = new Color(defColor.r, defColor.g, defColor.b, 0.5f);
        spriteRenderer.color = spawnColor;
        bossHPImage = GetComponent<Image>();
    }

    public virtual void Update()
    {
        CheckDeath(); //모든 종류 몬스터에 사용

        if (skillTest || isBoss)
        {
            BossHPUI();
            return;
        }

        if (isSpawnTime)
        {
            Invoke("SpawnTime", 1f);
            return;
        }
        Moving();
        CheckPosition();
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
    /// 필드를 벗어 날려고 할 경우 방지
    /// </summary>
    private void CheckPosition()
    {
        //현재 위치를 체크
        Vector3 checkPosition = transform.position;

        //만약 정해진 영역을 넘으려고 하면 그 영역으로 이동시켜 막기
        if (checkPosition.x <= -21.45f)
        {
            checkPosition.x = -21.45f;
        }

        if (checkPosition.x >= 21.45f)
        {
            checkPosition.x = 21.45f;
        }

        if (checkPosition.y <= -15.3f)
        {
            checkPosition.y = -15.3f;
        }

        if (checkPosition.y >= 15.3f)
        {
            checkPosition.y = 15.3f;
        }

        //조정된 위치를 다시 오브젝트에 적용
        transform.position = checkPosition;
    }

    /// <summary>
    /// 몬스터 죽음 확인
    /// </summary>
    private void CheckDeath()
    {
        if (curHP <= 0f)
        {
            switch (isBoss)
            {
                case false: //보스 몬스터가 아닌 경우
                    VDie(); //일반 적으로 죽이기
                    break;

                case true: //보스 몬스터일 경우
                    VPhaseCheck(); //페이즈 확인 후 판단하기 (페이즈 넘기기 혹은 죽음 판정하기)
                    break;
            }
        }
    }

    /// <summary>
    /// 보스 몬스터일 경우 체력바 이미지를 실시간으로 적용
    /// </summary>
    private void BossHPUI()
    {
        bossHPImage.fillAmount = curHP / maxHP;
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

    /// <summary>
    /// 몬스터 죽음 판정
    /// </summary>
    public virtual void VDie()
    {
        Destroy(gameObject); //오브젝트 파괴

        if (isDeath)
        {
            return; //중복 죽음 판정 방지
        }

        if (isBoss)
        {
            gameManager.PSetBossBattle(false); //보스전 끄기
        }
        gameManager.PSetCoin(haveCoin); //게임 매니저에 코인 등록
        isDeath = true; //죽음 판정
    }

    /// <summary>
    /// BossEnemy 스크립트에 접근하기 위한 버츄얼 함수
    /// -보스 몬스터만 사용하므로 일반 몹은 아무것도 없다.
    /// </summary>
    public virtual void VPhaseCheck()
    {

    }

    /// <summary>
    /// 페이즈를 넘길 때 체력 초기화 하기 위한 버츄얼 함수
    /// </summary>
    public virtual void VSetHP()
    {
        curHP = maxHP;
    }

    public bool PIsSpawnTime()
    {
        return isSpawnTime;
    }
}
