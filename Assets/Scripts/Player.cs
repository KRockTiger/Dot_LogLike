using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //캐릭터 기본 설정
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
    
    [Header("플레이어 스킬 셋팅")]
    [SerializeField] private PlayerSkill[] playerSkills;

    [Header("플레이어 스탯")]
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
    /// 플레이어 움직임
    /// </summary>
    private void Moving()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); //수평
        vertical = Input.GetAxisRaw("Vertical"); //수직
        transform.position += new Vector3(horizontal, vertical, 0f) * Time.deltaTime * moveSpeed;
    }

    /// <summary>
    /// 플레이어를 따라가는 카메라
    /// </summary>
    private void PlayerCamera()
    {
        posPlayer = transform.position;
        mainCam.gameObject.transform.position = new Vector3(posPlayer.x, posPlayer.y, posPlayer.z - 10);
    }

    /// <summary>
    /// 플레이어 스킬 관리
    /// </summary>
    private void PlayerSkills()
    {
       
    }

    /// <summary>
    /// 스킬 쿨타임 관리
    /// </summary>
    private void CoolDown()
    {
        //playerSkills[0].coolTime -= Time.deltaTime;
        //playerSkills[1].coolTime -= Time.deltaTime;
        //playerSkills[2].coolTime -= Time.deltaTime;
        //playerSkills[3].coolTime -= Time.deltaTime;
    }
}
