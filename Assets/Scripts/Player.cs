using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    //ĳ���� �⺻ ����
    private float moveX; //x�� ������
    private float moveY; //y�� ������
    private Camera mainCam; //ī�޶�
    private Vector3 posTarget; //Ÿ��(���콺 Ŀ��)�� ������
    [SerializeField] private Vector3 direction; //��ų�� ����
    private Quaternion rotTarget;
    private bool isRight;
    [SerializeField] private bool isDash;
    private SpriteRenderer spRenderer;
    private Color defColor;
    private Color dashColor;

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
    [SerializeField] private List<PlayerSkill> playerSkills;
    [SerializeField] private List<TMP_Text> coolTimes;
    [SerializeField] private Transform curSor;
    [SerializeField] private Transform objDynamic;

    [Header("�÷��̾� ����")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private float curHP;

    [Header("�÷��̾� ����")]
    [SerializeField] private bool isPassDamage = false;
    [SerializeField] private bool passMode = false;

    private void Awake()
    {
        mainCam = Camera.main;
        curHP = maxHP;
        spRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        defColor = spRenderer.color;
        dashColor = defColor;
        dashColor.a = 0.5f;
    }

    private void Update()
    {
        CheckMouse();
        Moving();
        Turning();
        PlayerCamera();
        SkillManager();
        CoolDown();
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
        
        if (isDash)
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
        mainCam.gameObject.transform.position = new Vector3(posPlayer.x, posPlayer.y, posPlayer.z - 10);
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
        transform.position += direction.normalized * Time.deltaTime * dashSpeed; //���� �������� ������ ��������

        playerSkills[3].curDuration -= Time.deltaTime; //���ӽð� Ȯ��

        if (playerSkills[3].curDuration <= 0f) //���ӽð��� ������ �뽬 ����
        {
            isDash = false;
        }
    }

    /// <summary>
    /// �÷��̾� �ǰ��ϴ� ���
    /// </summary>
    public void PHit(float _damage)
    {
        if (isPassDamage || passMode)
        {
            Debug.Log($"���� ��ȣ�� �޴� �����Դϴ�.");
            return;
        }
        curHP -= _damage;
        Debug.Log($"{_damage}��ŭ ���ظ� �Ծ����ϴ�.");
        isPassDamage = true;
        Invoke("PassEnd", 1f);
    }

    private void PassEnd()
    {
        isPassDamage = false;
    }
}
