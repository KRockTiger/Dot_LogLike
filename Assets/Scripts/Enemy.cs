using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform trsPlayer;

    [Header("적 몬스터 스탯")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private float curHP;
    [SerializeField] private float attack;
    
    private void Start()
    {
        trsPlayer = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        //Moving();
    }

    /// <summary>
    /// 몬스터가 플레이어를 추적하는 기능
    /// </summary>
    private void Moving()
    {
        Vector3 direction = trsPlayer.position - transform.position;
        transform.position += new Vector3(direction.x, direction.y, 0).normalized * moveSpeed * Time.deltaTime;
        
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void PHit(Vector3 _direction)
    {
        transform.position += _direction.normalized * 7f * Time.deltaTime;
    }
}
