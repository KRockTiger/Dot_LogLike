using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //싱글톤 설정

    private int coin = 0;

    [Header("인터페이스")]
    [SerializeField] private TMP_Text coinText;

    [Header("몬스터 관리")]
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
화살을 포물선 그리면서 날리기 코드 (집에서 따로 적어놓으면 삭제 처리)

Rigidbody2D rigid;

void Update()
{
    rigid.MoveRotation(Quaternion.LookRotation(rigid.velocity) * Quaternion.Euler(0,0,-90f));
}
 */
