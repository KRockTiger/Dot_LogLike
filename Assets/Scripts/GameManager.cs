using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //싱글톤 설정

    private int coin = 0; //현재 코인

    [Header("인터페이스")]
    [SerializeField] private TMP_Text coinText;

    [Header("몬스터 관리")]
    [SerializeField] private List<GameObject> enemies; //등록할 몬스터
    [SerializeField] private float spawnTime; //설정할 스폰 타이머
    [SerializeField] private float spawnTimer; //실시간 타이머
    [SerializeField] private GameObject bossHPUI; //보스 체력 UI 오브젝트    
    [SerializeField] private bool isBossBattle = false; //보스전 유무 판정

    [Header("소환 관리")]
    [SerializeField] private bool useSpawn = false;
    [SerializeField] private Transform objDynamic;
    [SerializeField] private Transform[] spawnPoints; //스폰할 몬스터 구역
    [SerializeField] private int spawnMin = 3; //스폰하는 몬스터 수의 최솟값
    [SerializeField] private int spawnMax = 5; //스폰하는 몬스터 수의 최댓값
    private Vector3 minVector;
    private Vector3 maxVector;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        coinText.text = coin.ToString() + "$";
        BossHPUI();
        if (!useSpawn)
        { return; }
        SpawnCountDown();
    }

    /// <summary>
    /// 몬스터 스폰 쿨타임
    /// </summary>
    private void SpawnCountDown()
    {
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0f)
        {
            SpawnWave();
            spawnTimer = spawnTime;
        }
    }

    /// <summary>
    /// 몬스터 스폰
    /// </summary>
    private void SpawnWave()
    {
        int spawnCount = Random.Range(spawnMin, spawnMax + 1); //랜덤으로 스폰할 몬스터 수
        SpawnField(); //소환 필드 적용

        for (int iNum01 = 0; iNum01 < spawnCount; iNum01++)
        {
            int spawnEnemy = Random.Range(1, enemies.Count); //랜덤으로 스폰할 몬스터
            float xVector = Random.Range(minVector.x, maxVector.x); //x좌표 랜덤 적용
            float yVector = Random.Range(minVector.y, maxVector.y); //y좌표 랜덤 적용
            Vector3 spawnField = new Vector3(xVector, yVector, 0); //소환 좌표 적용

            Instantiate(enemies[spawnEnemy], spawnField, Quaternion.identity, objDynamic);
        }
    }

    /// <summary>
    /// 몬스터를 소환할 필드 설정
    /// </summary>
    /// <returns></returns>
    private (Vector3 _minVector, Vector3 _maxVector) SpawnField()
    {
        minVector = spawnPoints[0].position;
        maxVector = spawnPoints[1].position;

        return (minVector, maxVector);
    }

    /// <summary>
    /// 보스 체력 UI를 담당
    /// </summary>
    private void BossHPUI()
    {
        if (isBossBattle) //보스전일 경우
        {
            bossHPUI.SetActive(true); //보스체력 UI 활성화
        }

        else //보스전이 아닐경우
        {
            bossHPUI.SetActive(false); //보스체력 UI 비활성화
        }
    }

    /// <summary>
    /// 코인 저장
    /// </summary>
    /// <param name="_coin"></param>
    public void PSetCoin(int _coin)
    {
        coin += _coin;
    }

    /// <summary>
    /// 보스 몬스터가 소환될 때 사용
    /// </summary>
    public void PSetBossBattle(bool _bossBattle)
    {
        isBossBattle = _bossBattle;
    }
}

/*
화살을 포물선 그리면서 날리기 코드 (집에서 따로 적어놓으면 삭제 처리)

Rigidbody2D rigid;

void Update()
{
    rigid.MoveRotation(Quaternion.LookRotation(rigid.velocity) * Quaternion.Euler(0,0,-90f));
}
 */
