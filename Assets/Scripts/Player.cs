using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Player : MonoBehaviour
{
    //캐릭터 기본 설정
    private float moveX; //x축 움직임
    private float moveY; //y축 움직임
    private Camera mainCam; //카메라
    private Vector3 posTarget; //타겟(마우스 커서)의 포지션
    private Vector3 direction; //스킬샷 방향
    private Quaternion rotTarget;
    private bool isRight;
    [SerializeField] private bool isDash;
    private SpriteRenderer spRenderer; //현재 스프라이트
    private Color defColor; //일반 스프라이트 색
    private Color passColor; //무적용 스프라이트 색

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
    [SerializeField] private List<PlayerSkill> playerSkills; //플레이어 스킬 리스트
    [SerializeField] private List<TMP_Text> coolTimes; //스킬쿨타임 텍스트
    [SerializeField] private List<Image> skillImages; //스킬 이미지
    [SerializeField] private Transform curSor; //커서오브젝트
    [SerializeField] private Transform objDynamic; //소환할 오브젝트를 담을 오브젝트
    private Color coolSkillColor;

    [Header("플레이어 스탯")]
    [SerializeField] private float moveSpeed = 10f; //일반 이동속도
    [SerializeField] private float dashSpeed = 10f; //대쉬 이동속도
    [SerializeField] private GameObject[] hearts; //하트 UI를 담을 오브젝트 배열
    [SerializeField] private int curHP; //현재 체력
    [SerializeField] private int maxHP; //최대 체력
    //private int setMaxHP = 5; //설정 가능한 최대 체력 ==> 최대 체력 증가 아이템을 먹었을 경우 막기 위한 코드

    [Header("플레이어 상태")]
    [SerializeField] private bool isPassDamage = false; //대쉬 중 무적효과를 적용
    [SerializeField] private bool passMode = false; //한번 피격을 당하면 1초동안 무적효과를 적용

    private void Awake()
    {
        mainCam = Camera.main;
        curHP = maxHP; //현재 체력 설정
        spRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        defColor = spRenderer.color; //현재 스프라이트 저장
        passColor = defColor;
        passColor.a = 0.5f; //데미지 무시 상태일 때의 스프라이트 저장
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
        
        if (isDash) //대쉬 상태일 때 리턴
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
    /// 필드를 벗어 날려고 할 경우 방지
    /// </summary>
    private void CheckPosition()
    {
        //현재 위치를 체크
        Vector3 checkPosition = transform.position;

        //만약 정해진 영역을 넘으려고 하면 그 영역으로 이동시켜 막기
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

        //조정된 위치를 다시 오브젝트에 적용
        transform.position = checkPosition;
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
        mainCam.gameObject.transform.position = new Vector3(posPlayer.x, posPlayer.y, posPlayer.z - 12);
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
        isPassDamage = true;
        spRenderer.color = passColor;
        transform.position += direction.normalized * Time.deltaTime * dashSpeed; //일정 방향으로 빠르게 지나가기

        playerSkills[3].curDuration -= Time.deltaTime; //지속시간 확인

        if (playerSkills[3].curDuration <= 0f) //지속시간이 끝나면 대쉬 끄기
        {
            isDash = false;
            spRenderer.color = defColor;
            isPassDamage = false;
        }
    }

    /// <summary>
    /// 스킬 쿨타임일 때 UI설정
    /// </summary>
    private void CoolTimeUI()
    {
        for (int iNum01 = 0; iNum01 < coolTimes.Count; iNum01++) //모든 스킬 탐지
        {
            if (playerSkills[iNum01].skillActive) //스킬 사용이 가능하면
            {
                coolTimes[iNum01].gameObject.SetActive(false); //쿨타임 오브젝트 비활성화
                skillImages[iNum01].color = Color.white; //스킬 이미지를 불투명으로 설정
            }

            else //스킬 사용이 불가능하면
            {
                coolTimes[iNum01].gameObject.SetActive(true); //쿨타임 오브젝트 활성화
                skillImages[iNum01].color = coolSkillColor; //스킬 이미지를 반투명으로 설정
            }
        }
    }

    /// <summary>
    /// 플레이어 피격하는 기능
    /// </summary>
    public void PHit(int _damage)
    {
        if (isPassDamage || passMode)
        {
            Debug.Log($"현재 보호를 받는 상태입니다.");
            return;
        }
        curHP -= _damage;
        Debug.Log($"{_damage}만큼 피해를 입었습니다.");
        passMode = true;
        spRenderer.color = passColor;
        Invoke("PassEnd", 1f);
    }

    /// <summary>
    /// 체력 변환이 될 때 사용
    /// UI적용만 쓰는 함수
    /// </summary>
    private void HeartCheck()
    {
        int heartNum = 0; //하트 개수를 담을 임의의 변수 생성
        
        for (int iNum01 = 0; iNum01 < hearts.Length;  iNum01++)
        {
            if (hearts[iNum01].activeSelf)
            {
                heartNum += 1;
            }
        } //=>활성화 되어있는 하트 갯수에 따라 1씩증가

        if (heartNum != curHP) //만약 하트 개수와 현재 체력이 다를 때
        {
            if (heartNum < curHP) //하트 개수가 현재 체력보다 적으면
            {
                for (int iNum02 = 0; iNum02 < curHP; iNum02++)
                {
                    hearts[iNum02].SetActive(true); //현재 체력만큼 하트 활성화
                }
            }

            else if (heartNum > curHP) //하트 개수가 현재 체력보다 많으면
            {
                for (int iNum03 = hearts.Length - 1; iNum03 >= curHP; iNum03--)
                {
                    hearts[iNum03].SetActive(false); //현재 체력만큼 하트 비활성화
                }
            }
        }
    }

    /// <summary>
    /// 무적 모드 풀기 위해 사용
    /// </summary>
    private void PassEnd()
    {
        passMode = false;
        spRenderer.color = defColor;
    }
}
