using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Player : MonoBehaviour
{
    //ĳ���� �⺻ ����
    private float moveX; //x�� ������
    private float moveY; //y�� ������
    private Camera mainCam; //ī�޶�
    private Vector3 posTarget; //Ÿ��(���콺 Ŀ��)�� ������
    private Vector3 direction; //��ų�� ����
    private Quaternion rotTarget;
    private bool isRight;
    [SerializeField] private bool isDash;
    private SpriteRenderer spRenderer; //���� ��������Ʈ
    private Color defColor; //�Ϲ� ��������Ʈ ��
    private Color passColor; //������ ��������Ʈ ��

    [Serializable] //�÷��̾� ��ų ���
    public class PlayerSkill
    {
        public GameObject skillObject;
        public string skillName; //��ų �̸�
        public KeyCode skillKey; //��ų Ű (�ؿ��⼭ �ν����ͷ� ������ �� ���콺 ������ Mouse4, �������� Mouse5�� ����
                                 //        ==> ����Ƽ �ν����� ������ ���� ����)
        public float coolTime = 1f; //������ ��ų ��Ÿ��
        public float coolDown = 0f; //���� ��Ÿ��
        public float skillDuration; //������ ��ų ���� �ð�
        public float curDuration; //���� ���� �ð�
        public bool skillActive = true; //��ų Ȱ��ȭ Ȯ��
    }
    
    [Header("�÷��̾� ��ų ����")]
    [SerializeField] private List<PlayerSkill> playerSkills; //�÷��̾� ��ų ����Ʈ
    [SerializeField] private List<TMP_Text> coolTimes; //��ų��Ÿ�� �ؽ�Ʈ
    [SerializeField] private List<Image> skillImages; //��ų �̹���
    [SerializeField] private Transform curSor; //Ŀ��������Ʈ
    [SerializeField] private Transform objDynamic; //��ȯ�� ������Ʈ�� ���� ������Ʈ
    private Color coolSkillColor;

    [Header("�÷��̾� ����")]
    [SerializeField] private float moveSpeed = 10f; //�Ϲ� �̵��ӵ�
    [SerializeField] private float dashSpeed = 10f; //�뽬 �̵��ӵ�
    [SerializeField] private GameObject[] hearts; //��Ʈ UI�� ���� ������Ʈ �迭
    [SerializeField] private int curHP; //���� ü��
    [SerializeField] private int maxHP; //�ִ� ü��
    //private int setMaxHP = 5; //���� ������ �ִ� ü�� ==> �ִ� ü�� ���� �������� �Ծ��� ��� ���� ���� �ڵ�

    [Header("�÷��̾� ����")]
    [SerializeField] private bool isPassDamage = false; //�뽬 �� ����ȿ���� ����
    [SerializeField] private bool passMode = false; //�ѹ� �ǰ��� ���ϸ� 1�ʵ��� ����ȿ���� ����

    private void Awake()
    {
        mainCam = Camera.main;
        curHP = maxHP; //���� ü�� ����
        spRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        defColor = spRenderer.color; //���� ��������Ʈ ����
        passColor = defColor;
        passColor.a = 0.5f; //������ ���� ������ ���� ��������Ʈ ����
        coolSkillColor = Color.white;
        coolSkillColor.a = 0.5f;
    }

    private void Update()
    {
        CheckMouse();
        Moving();
        CheckPosition();
        Turning();
        PlayerCamera();
        SkillManager();
        CoolDown();
        CoolTimeUI();
        HeartCheck();
    }


    /// <summary>
    /// ���콺 Ȯ��
    /// </summary>
    private void CheckMouse()
    {
        Vector3 posMouse = Input.mousePosition; //���콺 Position�� ����
                          //Input.mousePosition�� Gameȭ��(1920 * 1080) ũ��� ����
        posMouse.z = -mainCam.transform.position.z;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(posMouse);
        //ȭ�� ������ ������ ���콺 ������ ���� ī�޶� ȭ�鿡 �ִ� ���� ��ǥ������ ����
        //x, y ��ǥ���� ���� ��ǥ�� ������ �ǰ� z���� ī�޶��� position.z���� ����
        //�� z������ ī�޶��� z ������ ���� �̸� ���� ���콺 ���� ��ǥ���� z���� 0���� ����
        posTarget = mouseWorldPos; //Ÿ����ǥ ����
        curSor.position = posTarget; //Ŀ�� ������Ʈ�� �������� Ÿ�� ��ǥ�� ����
        
        if (isDash) //�뽬 ������ �� ����
        { return; }
        direction = posTarget - transform.position; //��ų�� ���� ����
        
        rotTarget = Quaternion.FromToRotation(Vector3.right, direction);
    }

    /// <summary>
    /// �÷��̾� ������
    /// </summary>
    private void Moving()
    {
        if (isDash) //�뽬�� ������� if�� �ȿ� �ִ� Dash�Լ� ����ϸ� �Ʒ� �ڵ忡 ���� ���ϰ� ����
        {
            Dash();
            return;
        }
        moveX = Input.GetAxisRaw("Horizontal"); //����
        moveY = Input.GetAxisRaw("Vertical"); //����
        Vector3 moveDir = new Vector3(moveX, moveY, 0f);
        transform.position += moveDir * Time.deltaTime * moveSpeed;
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
    /// �÷��̾� �̹��� ���� ����
    /// </summary>
    private void Turning()
    {
        Vector3 scale = transform.localScale;

        if (moveX < 0f)
        {
            scale = new Vector3(1, 1, 1);
            transform.localScale = scale;
        }

        else if (moveX > 0f)
        {
            scale = new Vector3(-1, 1, 1);
            transform.localScale = scale;
        }

        //ĳ���Ͱ� �ٶ󺸴� ���� ����
        isRight = (transform.localScale.x <= 0f) ? true : false;
    }

    /// <summary>
    /// �÷��̾ ���󰡴� ī�޶�
    /// </summary>
    private void PlayerCamera()
    {
        Vector3 posPlayer = transform.position;
        mainCam.gameObject.transform.position = new Vector3(posPlayer.x, posPlayer.y, posPlayer.z - 12);
    }

    /// <summary>
    /// �÷��̾� ��ų ����
    /// </summary>
    private void SkillManager()
    {
        int count = playerSkills.Count;
        for (int iNum01 = 0; iNum01 < count; iNum01++)
        {
            if (playerSkills[iNum01].skillActive) //��ų�� Ȱ��ȭ ������ ��
            {
                if (Input.GetKeyDown(playerSkills[iNum01].skillKey))
                {
                    playerSkills[iNum01].curDuration = playerSkills[iNum01].skillDuration; //���� �ð� ����
                    PlayerSkills(playerSkills[iNum01].skillKey);
                    playerSkills[iNum01].skillActive = false;
                }
            }

            else //��ų�� ��Ȱ��ȭ �����϶�
            {
                if (Input.GetKeyDown(playerSkills[iNum01].skillKey))
                {
                    Debug.Log($"��ų {playerSkills[iNum01].skillName} ��Ÿ���� ���ҽ��ϴ�.");
                }
            }
        }
    }

    /// <summary>
    /// �÷��̾� ��ų�� ����ϴ� �Լ�
    /// </summary>
    /// <param ��ų Ű�ڵ�="_skillKey">��ų Ű�ڵ�</param>
    private void PlayerSkills(KeyCode _skillKey)
    {
        switch (_skillKey)
        {
            case KeyCode.Q:
                GameObject objMeteor = playerSkills[0].skillObject; //������Ʈ ����
                Vector3 localMeteor = objMeteor.transform.localScale; //���׿��� ���Ⱚ ����
                if (isRight && localMeteor.x < 0) //ĳ���Ͱ� �������� ���ϰ� x���� �������� ������ ���ϸ�
                {
                    localMeteor.x *= -1;
                    objMeteor.transform.localScale = localMeteor;
                }

                else if (!isRight && localMeteor.x > 0) //�� ���ǰ� �ݴ��� ���
                {
                    localMeteor.x *= -1;
                    objMeteor.transform.localScale = localMeteor;
                }
                Vector3 tarMeteor = posTarget;
                tarMeteor.x += (isRight) ? -10f : 10f;
                tarMeteor.y += 15f; //�Ʒ��� �������ٰ� ������ �ؾ� �ϹǷ� ���� ����
                Instantiate(objMeteor, tarMeteor, Quaternion.identity, objDynamic); //���׿� ��ȯ
                break;

            case KeyCode.E:
                GameObject objLaser = playerSkills[1].skillObject;
                Vector3 localLaser = objLaser.transform.localScale;
                if (isRight && localLaser.x < 0)
                {
                    localLaser.x *= -1;
                    objLaser.transform.localScale = localLaser;
                }

                else if (!isRight && localLaser.x > 0)
                {
                    localLaser.x *= -1;
                    objLaser.transform.localScale = localLaser;
                }
                Instantiate(objLaser, transform.position, Quaternion.identity, objDynamic);
                break;

            case KeyCode.Mouse0:
                GameObject obj = Instantiate(playerSkills[2].skillObject, transform.position, rotTarget, objDynamic);
                SkillManager sc = obj.GetComponent<SkillManager>();
                sc.PSetDirection(direction);
                break;

            case KeyCode.Mouse1:
                isDash = true;                
                break;
        }
    }

    /// <summary>
    /// ��ų ��Ÿ�� ����
    /// </summary>
    private void CoolDown()
    {
        int count = playerSkills.Count;

        for (int iNum01 = 0; iNum01 < count; iNum01++) //��ų ��Ÿ�� �ؽ�Ʈ
        {
            //��ų ���� 1�� ������ �� �Ҽ��� �� �ڸ����� ǥ��
            if (playerSkills[iNum01].coolDown < 1f)
            {
                coolTimes[iNum01].text = playerSkills[iNum01].coolDown.ToString("F1");
            }

            //�� �ܿ��� ������ ǥ��
            else
            {
                coolTimes[iNum01].text = playerSkills[iNum01].coolDown.ToString("F0");
            }
        }

        for (int iNum02 = 0; iNum02 < count; iNum02++) //��ų ��Ÿ�� ���� �� Ȱ��ȭ Ȯ��
        {
            if (!playerSkills[iNum02].skillActive)
            {
                playerSkills[iNum02].coolDown -= Time.deltaTime;    
            }

            if (playerSkills[iNum02].coolDown <= 0f)
            {
                playerSkills[iNum02].coolDown = playerSkills[iNum02].coolTime;
                playerSkills[iNum02].skillActive = true; //��ų Ȱ��ȭ
            }
        }
    }

    /// <summary>
    /// �뽬 �Լ�
    /// </summary>
    private void Dash()
    {
        isPassDamage = true;
        spRenderer.color = passColor;
        transform.position += direction.normalized * Time.deltaTime * dashSpeed; //���� �������� ������ ��������

        playerSkills[3].curDuration -= Time.deltaTime; //���ӽð� Ȯ��

        if (playerSkills[3].curDuration <= 0f) //���ӽð��� ������ �뽬 ����
        {
            isDash = false;
            spRenderer.color = defColor;
            isPassDamage = false;
        }
    }

    /// <summary>
    /// ��ų ��Ÿ���� �� UI����
    /// </summary>
    private void CoolTimeUI()
    {
        for (int iNum01 = 0; iNum01 < coolTimes.Count; iNum01++) //��� ��ų Ž��
        {
            if (playerSkills[iNum01].skillActive) //��ų ����� �����ϸ�
            {
                coolTimes[iNum01].gameObject.SetActive(false); //��Ÿ�� ������Ʈ ��Ȱ��ȭ
                skillImages[iNum01].color = Color.white; //��ų �̹����� ���������� ����
            }

            else //��ų ����� �Ұ����ϸ�
            {
                coolTimes[iNum01].gameObject.SetActive(true); //��Ÿ�� ������Ʈ Ȱ��ȭ
                skillImages[iNum01].color = coolSkillColor; //��ų �̹����� ���������� ����
            }
        }
    }

    /// <summary>
    /// �÷��̾� �ǰ��ϴ� ���
    /// </summary>
    public void PHit(int _damage)
    {
        if (isPassDamage || passMode)
        {
            Debug.Log($"���� ��ȣ�� �޴� �����Դϴ�.");
            return;
        }
        curHP -= _damage;
        Debug.Log($"{_damage}��ŭ ���ظ� �Ծ����ϴ�.");
        passMode = true;
        spRenderer.color = passColor;
        Invoke("PassEnd", 1f);
    }

    /// <summary>
    /// ü�� ��ȯ�� �� �� ���
    /// UI���븸 ���� �Լ�
    /// </summary>
    private void HeartCheck()
    {
        int heartNum = 0; //��Ʈ ������ ���� ������ ���� ����
        
        for (int iNum01 = 0; iNum01 < hearts.Length;  iNum01++)
        {
            if (hearts[iNum01].activeSelf)
            {
                heartNum += 1;
            }
        } //=>Ȱ��ȭ �Ǿ��ִ� ��Ʈ ������ ���� 1������

        if (heartNum != curHP) //���� ��Ʈ ������ ���� ü���� �ٸ� ��
        {
            if (heartNum < curHP) //��Ʈ ������ ���� ü�º��� ������
            {
                for (int iNum02 = 0; iNum02 < curHP; iNum02++)
                {
                    hearts[iNum02].SetActive(true); //���� ü�¸�ŭ ��Ʈ Ȱ��ȭ
                }
            }

            else if (heartNum > curHP) //��Ʈ ������ ���� ü�º��� ������
            {
                for (int iNum03 = hearts.Length - 1; iNum03 >= curHP; iNum03--)
                {
                    hearts[iNum03].SetActive(false); //���� ü�¸�ŭ ��Ʈ ��Ȱ��ȭ
                }
            }
        }
    }

    /// <summary>
    /// ���� ��� Ǯ�� ���� ���
    /// </summary>
    private void PassEnd()
    {
        passMode = false;
        spRenderer.color = defColor;
    }
}
