using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform trsPlayer;

    [Header("�� ���� ����")]    
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
    /// ���Ͱ� �÷��̾ �����ϴ� ���
    /// </summary>
    private void Moving()
    {
        Vector3 direction = trsPlayer.position - transform.position;
        transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;
    }
}
    #region �ʿ��� ���
    /*
    1. �÷��̾��� ��ġ�� Ž���ϰ� ����
    2. �����Ÿ��� ������ �Ϲ� ����
    */
    #endregion
