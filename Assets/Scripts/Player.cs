using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Player : MonoBehaviour
{
    //캐릭터 기본 설정
    private float moveX; //x축 움직임
    private float moveY; //y축 움직임
    private Camera mainCam; //카메라
    private Vector3 posTarget; //타겟(마우스 커서)의 포지션
    [SerializeField] private Vector3 direction; //스킬샷 방향
    private Quaternion rotTarget;

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
    [SerializeField] private GameObject fire01;
    [SerializeField] private Transform curSor;
    [SerializeField] private Transform objDynamic;

    [Header("플레이어 스탯")]
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
    /// 플레이어 움직임
    /// </summary>
    private void Moving()
    {
        moveX = Input.GetAxisRaw("Horizontal"); //수평
        moveY = Input.GetAxisRaw("Vertical"); //수직
        transform.position += new Vector3(moveX, moveY, 0f) * Time.deltaTime * moveSpeed;
    }

    /// <summary>
    /// 마우스 확인
    /// </summary>
    private void CheckMouse()
    {
        Vector3 posMouse = Input.mousePosition; //마우스 Position값 저장
                          //Input.mousePosition은 Game화면(1920 * 1080) 크기로 적용
        posMouse.z = -mainCam.transform.position.z;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(posMouse);
        //화면 비율로 저장한 마우스 포지션 값을 카메라 화면에 있는 월드 좌표값으로 적용
        //x, y 좌표값은 월드 좌표로 적용이 되고 z값은 카메라의 position.z값을 적용
        //위 z값에서 카메라의 z 포지션 값을 미리 빼서 마우스 월드 좌표값의 z값을 0으로 맞춤
        posTarget = mouseWorldPos; //타겟좌표 저장
        curSor.position = posTarget; //커서 오브젝트의 포지션을 타겟 좌표로 저장
        direction = posTarget - transform.position; //스킬샷 방향 설정
        
        rotTarget = Quaternion.FromToRotation(Vector3.right, direction);
    }

    /// <summary>
    /// 플레이어 이미지 방향 조정
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
    /// 플레이어를 따라가는 카메라
    /// </summary>
    private void PlayerCamera()
    {
        Vector3 posPlayer = transform.position;
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
                    PlayerSkills(playerSkills[iNum01].skillKey);
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

    /// <summary>
    /// 플레이어 스킬을 등록하는 함수
    /// </summary>
    /// <param 스킬 키코드="_skillKey">스킬 키코드</param>
    private void PlayerSkills(KeyCode _skillKey)
    {
        switch (_skillKey)
        {
            case KeyCode.Q:
                Debug.Log("Q스킬 발동");                
                break;

            case KeyCode.E:
                //플레이어 기준 마우스 방향으로 일정 거리 대쉬
                //대쉬를 사용하는 동안 다른 키를 조작 불가
                break;

            case KeyCode.Mouse0:
                GameObject obj = Instantiate(fire01, transform.position, rotTarget, objDynamic);
                Skill sc = obj.GetComponent<Skill>();
                sc.PSetDirection(direction);
                break;

            case KeyCode.Mouse1:
                Debug.Log("마우스 오른쪽 스킬 발동");
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
    public void PHit(float _damage)
    {
        curHP -= _damage;
    }
}
