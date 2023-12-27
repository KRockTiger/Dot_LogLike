using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Player : MonoBehaviour
{
    //ĳ���� �⺻ ����
    private float moveX; //x�� ������
    private float moveY; //y�� ������
    private Camera mainCam; //ī�޶�
    private Vector3 posTarget; //Ÿ��(���콺 Ŀ��)�� ������
    [SerializeField] private Vector3 direction; //��ų�� ����
    private Quaternion rotTarget;

    [Serializable] //�÷��̾� ��ų ���
    public class PlayerSkill
    {
        public string skillName; //��ų �̸�
        public KeyCode skillKey; //��ų Ű (�ؿ��⼭ �ν����ͷ� ������ �� ���콺 ������ Mouse4, �������� Mouse5�� ����
                                 //        ==> ����Ƽ �ν����� ������ ���� ����)
        public float coolTime = 1f; //������ ��ų ��Ÿ��
        public float coolDown = 0f; //���� ��Ÿ��
        public bool skillActive = true; //��ų Ȱ��ȭ Ȯ��
    }
    
    [Header("�÷��̾� ��ų ����")]
    [SerializeField] private List<PlayerSkill> playerSkills;
    [SerializeField] private List<TMP_Text> coolTimes;
    [SerializeField] private GameObject fire01;
    [SerializeField] private Transform curSor;
    [SerializeField] private Transform objDynamic;

    [Header("�÷��̾� ����")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private float curHP;

    private void Awake()
    {
        mainCam = Camera.main;
        curHP = maxHP;
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
    /// �÷��̾� ������
    /// </summary>
    private void Moving()
    {
        moveX = Input.GetAxisRaw("Horizontal"); //����
        moveY = Input.GetAxisRaw("Vertical"); //����
        transform.position += new Vector3(moveX, moveY, 0f) * Time.deltaTime * moveSpeed;
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
        direction = posTarget - transform.position; //��ų�� ���� ����
        
        rotTarget = Quaternion.FromToRotation(Vector3.right, direction);
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
                Debug.Log("Q��ų �ߵ�");                
                break;

            case KeyCode.E:
                //�÷��̾� ���� ���콺 �������� ���� �Ÿ� �뽬
                //�뽬�� ����ϴ� ���� �ٸ� Ű�� ���� �Ұ�
                break;

            case KeyCode.Mouse0:
                GameObject obj = Instantiate(fire01, transform.position, rotTarget, objDynamic);
                Skill sc = obj.GetComponent<Skill>();
                sc.PSetDirection(direction);
                break;

            case KeyCode.Mouse1:
                Debug.Log("���콺 ������ ��ų �ߵ�");
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
    /// �÷��̾� �ǰ��ϴ� ���
    /// </summary>
    public void PHit(float _damage)
    {
        curHP -= _damage;
    }
}
