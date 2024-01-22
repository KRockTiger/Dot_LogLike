using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //�̱��� ����

    private int coin = 0; //���� ����

    [SerializeField] private GameObject keyManual;
    private bool gameStart = false;
    private bool gameEnd = false;
    private bool gamePause = false;
    private bool isStart = false;

    [Header("�������̽�")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject gamePauseUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameClearUI;
    private bool isInventory = false;

    [Header("���� ����")]
    [SerializeField] private List<GameObject> enemies; //����� ����
    [SerializeField] private List<GameObject> phases; //������ �̹���
    [SerializeField] private float spawnTime; //������ ���� Ÿ�̸�
    [SerializeField] private float spawnTimer; //�ǽð� Ÿ�̸�
    [SerializeField] private GameObject bossEnemy; //���� ������Ʈ
    [SerializeField] private GameObject bossHPUI; //���� ü�� UI ������Ʈ
    [SerializeField] private GameObject startBossKey; //���� ��ȯ Ű
    [SerializeField] private bool isBossBattle = false; //������ ���� ����
    [SerializeField] private bool isEliteSpawn = false; //����Ʈ ���� ��ȯ ����
    [SerializeField] private bool isEliteDie = false; //����Ʈ ���� ���� ����
    [SerializeField] private float setEliteTime; //������ ���� �ð�
    [SerializeField] private float curEliteTime; //���� ���� �ð�

    [Header("��ȯ ����")]
    [SerializeField] private bool useSpawn = false; //���� ����
    [SerializeField] private Transform objDynamic;
    [SerializeField] private Transform objEnemy;
    [SerializeField] private Transform[] spawnPoints; //������ ���� ����
    [SerializeField] private int spawnMin = 3; //�����ϴ� ���� ���� �ּڰ�
    [SerializeField] private int spawnMax = 5; //�����ϴ� ���� ���� �ִ�

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
        GameCheck(); //���� ���� Ȯ���Ͽ� ���� ����
        WaitEliteSpawn(); //����Ʈ ���� ��ȯ ���
        InventoryUI();
        coinText.text = coin.ToString() + "$";
        BossHPUI();
        if (!useSpawn)
        { return; }
        SpawnCountDown();
    }
    /// <summary>
    /// ����Ʈ ���� ���� ���
    /// </summary>
    private void WaitEliteSpawn()
    {
        if (isEliteSpawn || !gameStart) //���� �������̳� ����Ʈ ���Ͱ� ��ȯ�Ǿ��� ��� ����
        {
            return;
        }

        if (isStart) //���� �������ڸ��� �ð� �����ϰ� �ٷβ���
        {
            curEliteTime = setEliteTime;
            isStart = false;
        }

        curEliteTime -= Time.deltaTime;

        if (curEliteTime <= 0f) //�����ð��� ������ �� ����� ����Ʈ ���� ��ȯ
        {
            Instantiate(enemies[0], Vector3.zero, Quaternion.identity);
            isEliteSpawn = true;
        }
    }

    /// <summary>
    /// ������ ������ ��� ���߱�
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
    /// �κ��丮UI Ű
    /// </summary>
    private void InventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.B)) //Ű�� ���������� ����
        {
            gamePause = !gamePause;
            isInventory = !isInventory;
        }
        inventoryUI.SetActive(gamePause);
    }

    /// <summary>
    /// ���� ���� ��Ÿ��
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
    /// ���� ����
    /// </summary>
    private void SpawnWave()
    {
        int spawnCount = Random.Range(spawnMin, spawnMax + 1); //�������� ������ ���� ��
        SpawnField(); //��ȯ �ʵ� ����

        for (int iNum01 = 0; iNum01 < spawnCount; iNum01++)
        {
            int spawnEnemy = Random.Range(1, enemies.Count); //�������� ������ ����
            float xVector = Random.Range(minVector.x, maxVector.x); //x��ǥ ���� ����
            float yVector = Random.Range(minVector.y, maxVector.y); //y��ǥ ���� ����
            Vector3 spawnField = new Vector3(xVector, yVector, 0); //��ȯ ��ǥ ����

            Instantiate(enemies[spawnEnemy], spawnField, Quaternion.identity, objEnemy);
        }
    }

    /// <summary>
    /// ���͸� ��ȯ�� �ʵ� ����
    /// </summary>
    /// <returns></returns>
    private (Vector3 _minVector, Vector3 _maxVector) SpawnField()
    {
        minVector = spawnPoints[0].position;
        maxVector = spawnPoints[1].position;

        return (minVector, maxVector);
    }

    /// <summary>
    /// ���� ü�� UI�� ���
    /// </summary>
    private void BossHPUI()
    {
        if (isBossBattle) //�������� ���
        {
            bossHPUI.SetActive(true); //����ü�� UI Ȱ��ȭ
        }

        else //�������� �ƴҰ��
        {
            bossHPUI.SetActive(false); //����ü�� UI ��Ȱ��ȭ
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
    /// ��� ���� ����
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
    /// ���� ����
    /// </summary>
    /// <param name="_coin"></param>
    public void PSetCoin(int _coin)
    {
        coin += _coin;
    }

    /// <summary>
    /// ���� ���Ͱ� ��ȯ�� �� ���
    /// </summary>
    public void PSetBossBattle(bool _bossBattle)
    {
        isBossBattle = _bossBattle;
    }

    /// <summary>
    /// ���� ����
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
    /// ����Ʈ ���Ͱ� ��ȯ�� �Ǹ� ���
    /// </summary>
    public void PSetEndElite()
    {
        useSpawn = false;
        isEliteDie = true;
    }

    /// <summary>
    /// Ű Ȱ��ȭ
    /// </summary>
    public void PSetBossKey()
    {
        startBossKey.SetActive(true);
    }

    /// <summary>
    /// ���� Ȱ��ȭ
    /// </summary>
    public void PSetBoss()
    {
        bossEnemy.SetActive(true);
    }

    /// <summary>
    /// ���� ���� ó��
    /// </summary>
    public void PSetEndGame()
    {
        gameEnd = true;
    }

    /// <summary>
    /// ���� ����ȭ�� UIǥ��
    /// </summary>
    public void PPlayerDie()
    {
        gameOverUI.SetActive(true);
    }
    
    /// <summary>
    /// ���� Ŭ����ȭ�� UIǥ��
    /// </summary>
    public void PClearGame()
    {
        gameClearUI.SetActive(true);
    }

    /// <summary>
    /// ��ư : ���Ӿ� �ٽ� �ҷ�����
    /// </summary>
    public void BTryGame()
    {
        SceneManager.LoadSceneAsync("PlayScene"); //"PlayScene"
    }
}

