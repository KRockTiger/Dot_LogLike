using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //싱글톤 설정

    private int coin = 0; //현재 코인

    [SerializeField] private GameObject keyManual;
    private bool gameStart = false;
    private bool gameEnd = false;
    private bool gamePause = false;
    private bool isStart = false;

    [Header("인터페이스")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject gamePauseUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameClearUI;
    private bool isInventory = false;

    [Header("몬스터 관리")]
    [SerializeField] private List<GameObject> enemies; //등록할 몬스터
    [SerializeField] private List<GameObject> phases; //페이즈 이미지
    [SerializeField] private float spawnTime; //설정할 스폰 타이머
    [SerializeField] private float spawnTimer; //실시간 타이머
    [SerializeField] private GameObject bossEnemy; //보스 오브젝트
    [SerializeField] private GameObject bossHPUI; //보스 체력 UI 오브젝트
    [SerializeField] private GameObject startBossKey; //보스 소환 키
    [SerializeField] private bool isBossBattle = false; //보스전 유무 판정
    [SerializeField] private bool isEliteSpawn = false; //엘리트 몬스터 소환 여부
    [SerializeField] private bool isEliteDie = false; //엘리트 몬스터 죽음 여부
    [SerializeField] private float setEliteTime; //설정할 게임 시간
    [SerializeField] private float curEliteTime; //현재 게임 시간

    [Header("소환 관리")]
    [SerializeField] private bool useSpawn = false; //스폰 관리
    [SerializeField] private Transform objDynamic;
    [SerializeField] private Transform objEnemy;
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
        gameEnd = false;
        isEliteSpawn = false;
        useSpawn = false;
    }

    private void Start()
    {
        gamePause = false;
        gameOverUI.SetActive(false);
        gameClearUI.SetActive(false);
        isInventory = false;
    }

    private void Update()
    {
        if (gameStart)
        {
            keyManual.SetActive(false);
        }

        if (isEliteDie)
        {
            DestroyAllEnemy();
        }
        GameCheck(); //게임 상태 확인하여 멈춤 설정
        WaitEliteSpawn(); //엘리트 보스 소환 대기
        InventoryUI();
        coinText.text = coin.ToString() + "$";
        BossHPUI();
        if (!useSpawn)
        { return; }
        SpawnCountDown();
    }
    /// <summary>
    /// 엘리트 몬스터 스폰 대기
    /// </summary>
    private void WaitEliteSpawn()
    {
        if (isEliteSpawn || !gameStart) //게임 시작전이나 엘리트 몬스터가 소환되었을 경우 리턴
        {
            return;
        }

        if (isStart) //게임 시작하자마자 시간 적용하고 바로끄기
        {
            curEliteTime = setEliteTime;
            isStart = false;
        }

        curEliteTime -= Time.deltaTime;

        if (curEliteTime <= 0f) //일정시간이 지나면 맵 가운데에 엘리트 몬스터 소환
        {
            Instantiate(enemies[0], Vector3.zero, Quaternion.identity);
            isEliteSpawn = true;
        }
    }

    /// <summary>
    /// 게임이 끝났을 경우 멈추기
    /// </summary>
    private void GameCheck()
    {
        if (gameEnd || gamePause)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
        }

        else if (!gameEnd && !gamePause)
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// 인벤토리UI 키
    /// </summary>
    private void InventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.B)) //키를 누를때마다 변경
        {
            gamePause = !gamePause;
            isInventory = !isInventory;
        }
        inventoryUI.SetActive(gamePause);
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

            Instantiate(enemies[spawnEnemy], spawnField, Quaternion.identity, objEnemy);
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

    public void PBossPhaseUI(int _phase)
    {
        switch(_phase)
        {
            case 1:
                for (int iNum01 = 0; iNum01 < phases.Count; iNum01++)
                {
                    phases[iNum01].SetActive(true);
                }
                break;

            case 2:
                for (int iNum02 = 0;iNum02 < phases.Count; iNum02++)
                {
                    if (iNum02 != phases.Count - 1)
                    {
                        phases[iNum02].SetActive(true);
                    }

                    else
                    {
                        phases[iNum02].SetActive(false);
                    }
                }
                break;

            case 3:
                for (int iNum03 = 0; iNum03 < phases.Count; iNum03++)
                {
                    if (iNum03 == 0)
                    {
                        phases[iNum03].SetActive(true);
                    }

                    else
                    {
                        phases[iNum03].SetActive(false);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 모든 몬스터 삭제
    /// </summary>
    private void DestroyAllEnemy()
    {
        //int count = objEnemy.childCount;
        //for (int iNum = count; iNum > -1; iNum--)
        //{
        //    Transform objEnem = objEnemy.GetChild(iNum);
        //    Destroy(objEnem);
        //}
        GameObject enemyObj = GameObject.Find("ObjEnemy");
        Destroy(enemyObj);
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

    /// <summary>
    /// 게임 시작
    /// </summary>
    /// <param name="_start"></param>
    public void PSetStartGame(bool _start)
    {
        useSpawn = _start;
        gameStart = _start;
        isStart = _start;
        keyManual.SetActive(!_start);
    }

    /// <summary>
    /// 엘리트 몬스터가 소환이 되면 등록
    /// </summary>
    public void PSetEndElite()
    {
        useSpawn = false;
        isEliteDie = true;
    }

    /// <summary>
    /// 키 활성화
    /// </summary>
    public void PSetBossKey()
    {
        startBossKey.SetActive(true);
    }

    /// <summary>
    /// 보스 활성화
    /// </summary>
    public void PSetBoss()
    {
        bossEnemy.SetActive(true);
    }

    /// <summary>
    /// 게임 엔드 처리
    /// </summary>
    public void PSetEndGame()
    {
        gameEnd = true;
    }

    /// <summary>
    /// 게임 오버화면 UI표시
    /// </summary>
    public void PPlayerDie()
    {
        gameOverUI.SetActive(true);
    }
    
    /// <summary>
    /// 게임 클리어화면 UI표시
    /// </summary>
    public void PClearGame()
    {
        gameClearUI.SetActive(true);
    }

    /// <summary>
    /// 버튼 : 게임씬 다시 불러오기
    /// </summary>
    public void BTryGame()
    {
        SceneManager.LoadSceneAsync("PlayScene"); //"PlayScene"
    }
}

