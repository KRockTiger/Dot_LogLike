using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;

    private Transform trsPlayer; //�÷��̾� ��ġ
    private bool isHitting = false; //�˹� Ȯ��
    private bool isDeath = false; //��� Ȯ��
    private bool isSpawnTime = true; //���� Ȯ��
    private Vector3 enemyScale;
    private Vector3 hitDirection; //�˹� ����
    private SpriteRenderer spriteRenderer;
    private Color defColor; //�� ������ �÷�
    private Color hitColor = Color.red; //�ǰ� ������ �� ��Ÿ���� �÷�
    private Color spawnColor; //����Ÿ���� �� �÷�

    [Header("�� ���� ����")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private float curHP;
    [SerializeField] private float damage = 2f;
    [SerializeField] private float hitTime = 0f; //�˹� �ð�
    [SerializeField] private int haveCoin = 10;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyScale = transform.localScale;
        curHP = maxHP;
    }

    private void Start()
    {
        trsPlayer = GameObject.Find("Player").transform;
        defColor = spriteRenderer.color;
        spawnColor = new Color(defColor.r, defColor.g, defColor.b, 0.5f);
        spriteRenderer.color = spawnColor;
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (isSpawnTime)
        {
            Invoke("SpawnTimeEnd", 1f);
            return;
        }
        Moving();
        CheckDeath();
    }

    private void SpawnTimeEnd()
    {
        spriteRenderer.color = defColor;
        isSpawnTime = false;
    }

    /// <summary>
    /// ���Ͱ� �÷��̾ �����ϴ� ���
    /// </summary>
    private void Moving()
    {
        Vector3 direction = trsPlayer.position - transform.position;

        if (!isHitting) //�ǰ� ���� �ƴ� ���
        {
            transform.position += new Vector3(direction.x, direction.y, 0).normalized * moveSpeed * Time.deltaTime;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-enemyScale.x, enemyScale.y, 1);
            }

            else if (direction.x > 0)
            {
                transform.localScale = new Vector3(enemyScale.x, enemyScale.y, 1);
            }
        }

        else //�ǰ� ���� ���
        {
            spriteRenderer.color = hitColor;
            transform.position += hitDirection.normalized * moveSpeed * 1.2f * Time.deltaTime; //�˹�

            if (hitDirection.x < 0) //�ǰ� ������ �����̸� ������ �������� �ٶ󺸱�
            {
                transform.localScale = new Vector3(enemyScale.x, enemyScale.y, 1);
            }

            else if (hitDirection.x > 0) //�ǰ� ������ �������̸� ���� �������� �ٶ󺸱�
            {
                transform.localScale = new Vector3(-enemyScale.x, enemyScale.y, 1);
            }

            hitTime -= Time.deltaTime; //�˹� �ð�
            if (hitTime <= 0f)
            {
                spriteRenderer.color = defColor;
                isHitting = false; //�ǰ� ���� ����
            }
        }
    }

    /// <summary>
    /// ���� ���� Ȯ��
    /// </summary>
    private void CheckDeath()
    {
        if (curHP <= 0f)
        {
            Destroy(gameObject);

            if (isDeath)
            {
                return;
            }

            gameManager.PSetCoin(haveCoin);
            isDeath = true;
        }
    }

    /// <summary>
    /// ���� �ǰ�
    /// </summary>
    /// <param �ǰ� ����="_direction">�ǰ��� ���� ������ ��Ÿ��</param>
    public void PHit(Vector3 _direction, float _damage)
    {
        curHP -= _damage;
        hitDirection = _direction; //�ǰ� ���� ����
        hitTime = 0.2f; //�ǰ� �ð�
        isHitting = true; //�ǰ� ���� Ű��
    }

    public bool PIsSpawnTime()
    {
        return isSpawnTime;
    }
}
