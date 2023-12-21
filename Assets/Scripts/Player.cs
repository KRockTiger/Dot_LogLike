using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private PlayerSkill[] playerSkills;

    [Header("�÷��̾� ����")]
    [SerializeField] private float moveSpeed = 10f;

    private void Awake()
    {
        mainCam = Camera.main;
        trsPlayer = gameObject.transform;
        
        for (int i = 0; i < playerSkills.Length; i++)
        {
            playerSkills[i] = gameObject.GetComponent<PlayerSkill>();
        }
    }

    private void Update()
    {
        Moving();
        PlayerCamera();
        PlayerSkills();
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
        //playerSkills[0].coolTime -= Time.deltaTime;
        //playerSkills[1].coolTime -= Time.deltaTime;
        //playerSkills[2].coolTime -= Time.deltaTime;
        //playerSkills[3].coolTime -= Time.deltaTime;
    }
}
