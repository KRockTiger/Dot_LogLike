using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform trsPlayer;

    [Header("�� ���� ����")]    
    [SerializeField] private float moveSpeed = 5f;
    
    private void Start()
    {
        trsPlayer = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        //Moving();
    }

    /// <summary>
    /// ���Ͱ� �÷��̾ �����ϴ� ���
    /// </summary>
    private void Moving()
    {
        Vector3 direction = trsPlayer.position - transform.position;
        transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;
        
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
