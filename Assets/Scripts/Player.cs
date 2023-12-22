using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Player : MonoBehaviour
{
    //ĳ���� �⺻ ����
    private float horizontal;
    private float vertical;
    private Camera mainCam;
    private Vector3 posPlayer;
    private Transform trsPlayer;

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

    [Header("�÷��̾� ����")]
    [SerializeField] private float moveSpeed = 10f;

    private void Awake()
    {
        mainCam = Camera.main;
        trsPlayer = gameObject.transform;
    }

    private void Update()
    {
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
        horizontal = Input.GetAxisRaw("Horizontal"); //����
        vertical = Input.GetAxisRaw("Vertical"); //����
        transform.position += new Vector3(horizontal, vertical, 0f) * Time.deltaTime * moveSpeed;
    }

    /// <summary>
    /// �÷��̾� �̹��� ���� ����
    /// </summary>
    private void Turning()
    {
        Vector3 scale = transform.localScale;

        if (horizontal < 0f)
        {
            scale = new Vector3(1, 1, 1);
            transform.localScale = scale;
        }

        else if (horizontal > 0f)
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
        posPlayer = transform.position;
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
                    Debug.Log($"��ų {playerSkills[iNum01].skillName}��(��) ����߽��ϴ�.");
                    //PlayerSkills(playerSkills[iNum01].skillKey));
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

    private void PlayerSkills(KeyCode _skillKey)
    {
        switch ( _skillKey )
        {
            case KeyCode.Q:
                break;

            case KeyCode.E:
                //�÷��̾� ���� ���콺 �������� ���� �Ÿ� �뽬
                //�뽬�� ����ϴ� ���� �ٸ� Ű�� ���� �Ұ�
                break;

            case KeyCode.Mouse0:
                break;

            case KeyCode.Mouse1:
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
    public void PHit()
    {
        Debug.Log("������ ���߽��ϴ�.");
    }
}
