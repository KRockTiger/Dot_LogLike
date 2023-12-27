using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //�̱��� ����

    private int coin = 0;

    [Header("�������̽�")]
    [SerializeField] private TMP_Text coinText;

    [Header("���� ����")]
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private float spawnTime;
    [SerializeField] private float spawnTimer;

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
    }

    private void SpawnCountDown()
    {
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnTime;
        }
    }

    private void SpawnWave()
    {
        
    }

    public void PSetCoin(int _coin)
    {
        coin += _coin;
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
