using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;

    private Transform trsPlayer; //�÷��̾� ��ġ
    private bool isHitting = false; //�˹� Ȯ��
    private bool isDeath = false; //��� Ȯ��
    [SerializeField] private bool isSpawnTime = true; //���� Ȯ��
    private Vector3 enemyScale;
    private Vector3 hitDirection; //�˹� ����
    private SpriteRenderer spriteRenderer;
    private Color defColor; //�� ������ �÷�
    private Color hitColor = Color.red; //�ǰ� ������ �� ��Ÿ���� �÷�
    private Color spawnColor; //����Ÿ���� �� �÷�

    [Header("�� ���� ����")]
    [SerializeField] private float moveSpeed = 5f; //�̵��ӵ�
    [SerializeField] private float maxHP = 10f; //�����ų ���� ü��
    [SerializeField] private float curHP; //���� ü��
    [SerializeField] private float hitTime = 0f; //�˹� �ð�
    [SerializeField] private int damage = 1; //������ ���ݷ�
    [SerializeField] private int haveCoin = 10; //���� ���Ͱ� ������ �ִ� ���� ����
    [SerializeField, Tooltip("���� ������ ��� true")] private bool isBoss = false; //���� ����
    [SerializeField, Tooltip("���� ���͸� ���")] private Image bossHPImage; //���� ü�� �̹���

    [Header("�׽�Ʈ��")]
    [SerializeField] private bool skillTest = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player sc = collision.GetComponent<Player>();
            sc.PHit(damage);
        } //=> �÷��̾� �±׸� �����ϰ� ��ũ��Ʈ�� �����Ͽ� ������ �ֱ�
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyScale = transform.localScale;
        curHP = maxHP;

        if (isBoss)
        {
            return;
        }

        //������ �� �������� �ٶ� ������ ����
        int randDirenction = Random.Range(0, 2);
        if (randDirenction == 0)
        {
            transform.localScale = new Vector3(-enemyScale.x, enemyScale.y, 0);
        }

        else
        {
            transform.localScale = new Vector3(enemyScale.x, enemyScale.y, 0);
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        trsPlayer = GameObject.Find("Player").transform;
        defColor = spriteRenderer.color;

        if (!isSpawnTime || isBoss)
        {
            gameManager.PSetBossBattle(true); //������ Ű��
            return;
        }
        spawnColor = new Color(defColor.r, defColor.g, defColor.b, 0.5f);
        spriteRenderer.color = spawnColor;
        bossHPImage = GetComponent<Image>();
    }

    public virtual void Update()
    {
        CheckDeath(); //��� ���� ���Ϳ� ���

        if (skillTest || isBoss)
        {
            BossHPUI();
            return;
        }

        if (isSpawnTime)
        {
            Invoke("SpawnTime", 1f);
            return;
        }
        Moving();
        CheckPosition();
    }

    /// <summary>
    /// �����ϴ� �ð�
    /// </summary>
    private void SpawnTime()
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

            if (direction.x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-enemyScale.x, enemyScale.y, 1);
            }

            else if (direction.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(enemyScale.x, enemyScale.y, 1);
            }
        }

        else //�ǰ� ���� ���
        {
            spriteRenderer.color = hitColor; //���� ��������Ʈ�� �ǰ� �÷��� ����
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
                spriteRenderer.color = defColor; //��������Ʈ ����
                isHitting = false; //�ǰ� ���� ����
            }
        }
    }

    /// <summary>
    /// �ʵ带 ���� ������ �� ��� ����
    /// </summary>
    private void CheckPosition()
    {
        //���� ��ġ�� üũ
        Vector3 checkPosition = transform.position;

        //���� ������ ������ �������� �ϸ� �� �������� �̵����� ����
        if (checkPosition.x <= -21.45f)
        {
            checkPosition.x = -21.45f;
        }

        if (checkPosition.x >= 21.45f)
        {
            checkPosition.x = 21.45f;
        }

        if (checkPosition.y <= -15.3f)
        {
            checkPosition.y = -15.3f;
        }

        if (checkPosition.y >= 15.3f)
        {
            checkPosition.y = 15.3f;
        }

        //������ ��ġ�� �ٽ� ������Ʈ�� ����
        transform.position = checkPosition;
    }

    /// <summary>
    /// ���� ���� Ȯ��
    /// </summary>
    private void CheckDeath()
    {
        if (curHP <= 0f)
        {
            switch (isBoss)
            {
                case false: //���� ���Ͱ� �ƴ� ���
                    VDie(); //�Ϲ� ������ ���̱�
                    break;

                case true: //���� ������ ���
                    VPhaseCheck(); //������ Ȯ�� �� �Ǵ��ϱ� (������ �ѱ�� Ȥ�� ���� �����ϱ�)
                    break;
            }
        }
    }

    /// <summary>
    /// ���� ������ ��� ü�¹� �̹����� �ǽð����� ����
    /// </summary>
    private void BossHPUI()
    {
        bossHPImage.fillAmount = curHP / maxHP;
    }

    /// <summary>
    /// ���� �ǰ�
    /// </summary>
    /// <param �ǰ� ����="_direction">�ǰ��� ���� ������ ��Ÿ��</param>
    public void PHit(Vector3 _direction, float _damage)
    {
        if (skillTest)
        {
            Debug.Log("���Ͱ� ��ų ������ �޾ҽ��ϴ�.");
            return;
        }

        curHP -= _damage;
        hitDirection = _direction; //�ǰ� ���� ����
        hitTime = 0.2f; //�ǰ� �ð�
        isHitting = true; //�ǰ� ���� Ű��
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public virtual void VDie()
    {
        Destroy(gameObject); //������Ʈ �ı�

        if (isDeath)
        {
            return; //�ߺ� ���� ���� ����
        }

        if (isBoss)
        {
            gameManager.PSetBossBattle(false); //������ ����
        }
        gameManager.PSetCoin(haveCoin); //���� �Ŵ����� ���� ���
        isDeath = true; //���� ����
    }

    /// <summary>
    /// BossEnemy ��ũ��Ʈ�� �����ϱ� ���� ����� �Լ�
    /// -���� ���͸� ����ϹǷ� �Ϲ� ���� �ƹ��͵� ����.
    /// </summary>
    public virtual void VPhaseCheck()
    {

    }

    /// <summary>
    /// ����� �ѱ� �� ü�� �ʱ�ȭ �ϱ� ���� ����� �Լ�
    /// </summary>
    public virtual void VSetHP()
    {
        curHP = maxHP;
    }

    public bool PIsSpawnTime()
    {
        return isSpawnTime;
    }
}
