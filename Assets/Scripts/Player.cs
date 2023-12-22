using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Player : MonoBehaviour
{
    //캐릭터 기본 설정
    private float horizontal;
    private float vertical;
    private Camera mainCam;
    private Vector3 posPlayer;
    private Transform trsPlayer;

    [Serializable] //플레이어 스킬 등록
    public class PlayerSkill
    {
        public string skillName; //스킬 이름
        public KeyCode skillKey; //스킬 키 (※여기서 인스펙터로 설정할 때 마우스 왼쪽은 Mouse4, 오른쪽은 Mouse5로 설정
                                 //        ==> 유니티 인스펙터 오류로 인한 조정)
        public float coolTime = 1f; //정해진 스킬 쿨타임
        public float coolDown = 0f; //현재 쿨타임
        public bool skillActive = true; //스킬 활성화 확인
    }
    
    [Header("플레이어 스킬 셋팅")]
    [SerializeField] private List<PlayerSkill> playerSkills;
    [SerializeField] private List<TMP_Text> coolTimes;

    [Header("플레이어 스탯")]
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
    /// 플레이어 움직임
    /// </summary>
    private void Moving()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); //수평
        vertical = Input.GetAxisRaw("Vertical"); //수직
        transform.position += new Vector3(horizontal, vertical, 0f) * Time.deltaTime * moveSpeed;
    }

    /// <summary>
    /// 플레이어 이미지 방향 조정
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
    private void SkillManager()
    {
        int count = playerSkills.Count;
        for (int iNum01 = 0; iNum01 < count; iNum01++)
        {
            if (playerSkills[iNum01].skillActive) //스킬이 활성화 상태일 때
            {
                if (Input.GetKeyDown(playerSkills[iNum01].skillKey))
                {
                    Debug.Log($"스킬 {playerSkills[iNum01].skillName}을(를) 사용했습니다.");
                    //PlayerSkills(playerSkills[iNum01].skillKey));
                    playerSkills[iNum01].skillActive = false;
                }
            }

            else //스킬이 비활성화 상태일때
            {
                if (Input.GetKeyDown(playerSkills[iNum01].skillKey))
                {
                    Debug.Log($"스킬 {playerSkills[iNum01].skillName} 쿨타임이 남았습니다.");
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
                //플레이어 기준 마우스 방향으로 일정 거리 대쉬
                //대쉬를 사용하는 동안 다른 키를 조작 불가
                break;

            case KeyCode.Mouse0:
                break;

            case KeyCode.Mouse1:
                break;
        }
    }

    /// <summary>
    /// 스킬 쿨타임 관리
    /// </summary>
    private void CoolDown()
    {
        int count = playerSkills.Count;

        for (int iNum01 = 0; iNum01 < count; iNum01++) //스킬 쿨타임 텍스트
        {
            //스킬 쿨이 1초 이하일 때 소수점 한 자리까지 표현
            if (playerSkills[iNum01].coolDown < 1f)
            {
                coolTimes[iNum01].text = playerSkills[iNum01].coolDown.ToString("F1");
            }

            //그 외에는 정수로 표현
            else
            {
                coolTimes[iNum01].text = playerSkills[iNum01].coolDown.ToString("F0");
            }
        }

        for (int iNum02 = 0; iNum02 < count; iNum02++) //스킬 쿨타임 적용 및 활성화 확인
        {
            if (!playerSkills[iNum02].skillActive)
            {
                playerSkills[iNum02].coolDown -= Time.deltaTime;    
            }

            if (playerSkills[iNum02].coolDown <= 0f)
            {
                playerSkills[iNum02].coolDown = playerSkills[iNum02].coolTime;
                playerSkills[iNum02].skillActive = true; //스킬 활성화
            }
        }
    }

    /// <summary>
    /// 플레이어 피격하는 기능
    /// </summary>
    public void PHit()
    {
        Debug.Log("공격을 당했습니다.");
    }
}
