using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform trsPlayer;

    [Header("적 몬스터 스탯")]    
    [SerializeField] private float moveSpeed = 5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;        
    }

    private void Start()
    {
        trsPlayer = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        Moving();
    }

    /// <summary>
    /// 몬스터가 플레이어를 추적하는 기능
    /// </summary>
    private void Moving()
    {
        Vector3 direction = trsPlayer.position - transform.position;
        transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;
    }
}
    #region 필요한 기능
    /*
    1. 플레이어의 위치를 탐지하고 추적
    2. 일정거리에 들어오면 일반 공격
    */
    #endregion
