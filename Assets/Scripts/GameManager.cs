using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //�̱��� ����

    private int coin = 0; //���� ����

    [Header("�������̽�")]
    [SerializeField] private TMP_Text coinText;

    [Header("���� ����")]
    [SerializeField] private List<GameObject> enemies; //����� ����
    [SerializeField] private float spawnTime; //������ ���� Ÿ�̸�
    [SerializeField] private float spawnTimer; //�ǽð� Ÿ�̸�
    [SerializeField] private GameObject bossHPUI; //���� ü�� UI ������Ʈ    
    [SerializeField] private bool isBossBattle = false; //������ ���� ����

    [Header("��ȯ ����")]
    [SerializeField] private bool useSpawn = false;
    [SerializeField] private Transform objDynamic;
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

            Instantiate(enemies[spawnEnemy], spawnField, Quaternion.identity, objDynamic);
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
}

/*
ȭ���� ������ �׸��鼭 ������ �ڵ� (������ ���� ��������� ���� ó��)

Rigidbody2D rigid;

void Update()
{
    rigid.MoveRotation(Quaternion.LookRotation(rigid.velocity) * Quaternion.Euler(0,0,-90f));
}
 */
