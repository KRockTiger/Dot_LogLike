using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    //캐릭터 기본 설정
    private float moveX; //x축 움직임
    private float moveY; //y축 움직임
    private Camera mainCam; //카메라
    private Vector3 posTarget; //타겟(마우스 커서)의 포지션
    [SerializeField] private Vector3 direction; //스킬샷 방향
    private Quaternion rotTarget;
    private bool isRight;
    [SerializeField] private bool isDash;
    private SpriteRenderer spRenderer;
    private Color defColor;
    private Color dashColor;

    [Serializable] //플레이어 스킬 등록
    public class PlayerSkill
    {
        public GameObject skillObject;
        public string skillName; //스킬 이름
        public KeyCode skillKey; //스킬 키 (※여기서 인스펙터로 설정할 때 마우스 왼쪽은 Mouse4, 오른쪽은 Mouse5로 설정
                                 //        ==> 유니티 인스펙터 오류로 인한 조정)
        public float coolTime = 1f; //정해진 스킬 쿨타임
        public float coolDown = 0f; //현재 쿨타임
        public float skillDuration; //설정할 스킬 지속 시간
        public float curDuration; //현재 지속 시간
        public bool skillActive = true; //스킬 활성화 확인
    }
    
    [Header("플레이어 스킬 셋팅")]
    [SerializeField] private List<PlayerSkill> playerSkills;
    [SerializeField] private List<TMP_Text> coolTimes;
    [SerializeField] private Transform curSor;
    [SerializeField] private Transform objDynamic;

    [Header("플레이어 스탯")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private float curHP;

    [Header("플레이어 상태")]
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
        
        if (isDash)
        { return; }
        direction = posTarget - transform.position; //스킬샷 방향 설정
        
        rotTarget = Quaternion.FromToRotation(Vector3.right, direction);
    }

    /// <summary>
    /// 플레이어 움직임
    /// </summary>
    private void Moving()
    {
        if (isDash) //대쉬가 켜질경우 if문 안에 있는 Dash함수 사용하며 아래 코드에 접근 못하게 막기
        {
            Dash();
            return;
        }
        moveX = Input.GetAxisRaw("Horizontal"); //수평
        moveY = Input.GetAxisRaw("Vertical"); //수직
        Vector3 moveDir = new Vector3(moveX, moveY, 0f);
        transform.position += moveDir * Time.deltaTime * moveSpeed;
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

        //캐릭터가 바라보는 방향 저장
        isRight = (transform.localScale.x <= 0f) ? true : false;
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
                    playerSkills[iNum01].curDuration = playerSkills[iNum01].skillDuration; //지속 시간 설정
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
                GameObject objMeteor = playerSkills[0].skillObject; //오브젝트 저장
                Vector3 localMeteor = objMeteor.transform.localScale; //메테오의 방향값 저장
                if (isRight && localMeteor.x < 0) //캐릭터가 오른쪽을 향하고 x로컬 스케일이 왼쪽을 향하면
                {
                    localMeteor.x *= -1;
                    objMeteor.transform.localScale = localMeteor;
                }

                else if (!isRight && localMeteor.x > 0) //위 조건과 반대일 경우
                {
                    localMeteor.x *= -1;
                    objMeteor.transform.localScale = localMeteor;
                }
                Vector3 tarMeteor = posTarget;
                tarMeteor.x += (isRight) ? -10f : 10f;
                tarMeteor.y += 15f; //아래로 떨어지다가 적중을 해야 하므로 위로 설정
                Instantiate(objMeteor, tarMeteor, Quaternion.identity, objDynamic); //메테오 소환
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
    /// 대쉬 함수
    /// </summary>
    private void Dash()
    {
        transform.position += direction.normalized * Time.deltaTime * dashSpeed; //일정 방향으로 빠르게 지나가기

        playerSkills[3].curDuration -= Time.deltaTime; //지속시간 확인

        if (playerSkills[3].curDuration <= 0f) //지속시간이 끝나면 대쉬 끄기
        {
            isDash = false;
        }
    }

    /// <summary>
    /// 플레이어 피격하는 기능
    /// </summary>
    public void PHit(float _damage)
    {
        if (isPassDamage || passMode)
        {
            Debug.Log($"현재 보호를 받는 상태입니다.");
            return;
        }
        curHP -= _damage;
        Debug.Log($"{_damage}만큼 피해를 입었습니다.");
        isPassDamage = true;
        Invoke("PassEnd", 1f);
    }

    private void PassEnd()
    {
        isPassDamage = false;
    }
}
