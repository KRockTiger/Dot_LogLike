using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    //ĳ���� �⺻ ����
    private float horizontal;
    private float vertical;
    private Camera mainCam;
    private Vector3 posPlayer;
    private Transform trsPlayer;

    [Serializable]
    public class PlayerSkill
    {
        public string skillName;
        public KeyCode skillKey;
        public float coolTime = 1f;
        public float coolDown = 0f;
        public bool skillActive = true;
    }
    
    [Header("�÷��̾� ��ų ����")]
    [SerializeField] private List<PlayerSkill> playerSkills;
    [SerializeField] private TMP_Text coolTimeE;
    [SerializeField] private TMP_Text coolTimeQ;
    [SerializeField] private TMP_Text coolTimeM0;
    [SerializeField] private TMP_Text coolTimeM1;

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
        PlayerSkills();
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

    private void Turning()
    {
        Vector3 scale = transform.localScale;

        if (horizontal < 0f)
        {
            scale = new Vector3(1, 1, 1);
        }

        else if (horizontal > 0f)
        {
            scale = new Vector3(-1, 1, 1);
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
    private void PlayerSkills()
    {
       
    }

    /// <summary>
    /// ��ų ��Ÿ�� ����
    /// </summary>
    private void CoolDown()
    {
        int count = playerSkills.Count;
        for (int i = 0; i < count; i++)
        {
            playerSkills[i].coolDown -= Time.deltaTime;
            
            if (playerSkills[i].coolDown <= 0f)
            {
                playerSkills[i].coolDown = playerSkills[i].coolTime;
            }
        }

        coolTimeE.text = playerSkills[0].coolDown.ToString();
        coolTimeQ.text = playerSkills[1].coolDown.ToString();
        coolTimeM0.text = playerSkills[2].coolDown.ToString();
        coolTimeM1.text = playerSkills[3].coolDown.ToString();
    }
}
