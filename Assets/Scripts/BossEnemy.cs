using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossEnemy : MonoBehaviour
{
    public enum PatternName //패턴 이름
    {
        PokePattern,
        MeteorPattern,
        LavaPattern,
        LinePattern,
    }

    [System.Serializable]
    public class BossPattern //보스 패턴 클래스
    {
        public GameObject objPattern; //사용할 스킬 오브젝트
        public PatternName patternName; //패턴 이름
        public int setRepeatNum; //설정힐 패턴 횟수
        public int curRepeatNum; //현재 반복한 패턴 횟수
        public float setDelayTime; //패턴에 딜레이를 넣고 싶을 때 사용
        public float curDelayTime; //현재 딜레이 시간
        public bool startPattern = false; //패턴 시작 여부
        public bool rePattern = false; //패턴 재사용 여부
        public bool usedPattern = false; //다음 패턴 실행할 때 이미 실행한 패턴 사용하지 않기 위해 사용
    }

    [Header("보스 패턴")]
    [SerializeField] private List<BossPattern> bossPatterns; //패턴 리스트 만들기
    [SerializeField] private float lavaRange; //라바 패턴 범위 설정
    [SerializeField] private Transform[] fireLineSpawnPoints; //불 장판 생성할 필드 넓이를 설정할 Transform
    private int randLava = 0; //라바 패턴은 2종류가 있으며 랜덤으로 하나를 적용 시키기 위해 Random.Range를 설정

    [Header("보스 스텟")]
    [SerializeField] private float pokeSpeed = 20f; //찌르기 패턴의 이동 속도

    //private int bossPhase = 1;
    private Vector3 posTarget; //플레이어의 위치를 타겟으로 넣을 벡터 변수
    private Vector3 dirTarget; //현 위치에서 플레이어 위치까지의 방향을 정할 벡터 변수
    private Vector3 startPoint; //보스가 찌르기 공격할 때 찌르기 시작하는 위치를 저장하는 곳
    private Vector3 minVector; //불 장판 최소점
    private Vector3 maxVector; //불 장판 최대점
    [SerializeField] private bool posSearch; //위치를 한 번만 찾기 위한 제어기
    private bool _usingPattern; //패턴 사용 중일 때 true
    private bool UsingPattern
    {
        set
        {
            _usingPattern = value;
        }
        get => _usingPattern;
    }
    [SerializeField] private bool testStart = false; //테스트시작

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) //테스트 설정
        {
            testStart = !testStart;
        }

        if (testStart) //테스트 중이 아닐경우 리턴
        {
            //이제 1~2페이즈는 순서 랜덤으로 패턴은 중복되지 않게 사용
            ChangePattern(); //패턴 변경
        }

        PatternManager(PatternName.PokePattern, bossPatterns[0].startPattern);
        PatternManager(PatternName.MeteorPattern, bossPatterns[1].startPattern);
        PatternManager(PatternName.LavaPattern, bossPatterns[2].startPattern);
        PatternManager(PatternName.LinePattern, bossPatterns[3].startPattern);
    }

    /// <summary>
    /// 패턴을 변화 시키기 위한 함수
    /// - 하나의 패턴이 끝나면 다음 패턴을 등록해야하며 다음 패턴 사용하기의 딜레이가 필요
    /// </summary>
    private void ChangePattern()
    {
        if (UsingPattern) //패턴이 사용중이라면 리턴
        {
            return;
        }

        for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++) //패턴 목록 탐지
        {
            if (!bossPatterns[iNum01].usedPattern) //만약 이미 사용했던 패턴이면
            {
                bossPatterns[iNum01].startPattern = true; //패턴 사용 시작
                bossPatterns[iNum01].rePattern = true;
                UsingPattern = true; //패턴 사용 등록

                return;
            }
        }

        ResetPattern(); //모든 패턴이 사용되었으면 그로기 후 초기화를 위해 사용
    }

    private void ResetPattern()
    {
        for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++)
        {
            bossPatterns[iNum01].usedPattern = false; //패턴 초기화
        }
    }

    /// <summary>
    /// 플레이어의 위치를 찾는 함수
    /// - 플레이어의 위치를 한번만 찾는 것으로 주로 타겟 위치를 설정할 때 사용
    /// </summary>
    private void SearchPlayer()
    {
        if (!posSearch) //패턴 사용 중 일경우 리턴
        { return; }
        posTarget = GameObject.Find("Player").transform.position; //플레이어의 위치값 저장
        startPoint = transform.position; //시작 위치
        dirTarget = posTarget - startPoint; //방향 벡터
        posSearch = false; //한번 사용하면 바로 잠금
    }

    /// <summary>
    /// 플레이어의 위치를 찾고 찌르는 공격
    /// </summary>
    private void PokePlayer()
    {
        if (posSearch) //플레이어의 위치 찾기 시작
        {
            SearchPlayer();
        }
        float distance = Vector3.Distance(posTarget, transform.position); //타겟 좌표와 현 보스 오브젝트 위치 좌표 차이로 거리를 계산

        if (distance < 0.3f) //플레이어와의 거리가 0.3정도로 좁아지면 ==> distance는 거리의 길이를 계산하므로 적절한 범위를 설정해야 한다.
        {
            if (bossPatterns[0].curRepeatNum != bossPatterns[0].setRepeatNum && bossPatterns[0].startPattern) //연속으로 쓴 횟수와 설정한 연속 횟수가 맞지 않고 패턴 사용중일 경우
            {
                bossPatterns[0].curDelayTime -= Time.deltaTime; //다시 쓰기 까지의 딜레이 시간        

                if (bossPatterns[0].curDelayTime <= 0f) //딜레이 시간이 끝나면 다시 패턴 반복
                {
                    bossPatterns[0].rePattern = true; //패턴 재사용 명령
                }

            }

            else //만약 일정 횟수를 반복이 끝나면
            {
                bossPatterns[0].curRepeatNum = 0; //패턴 횟수 초기화
                bossPatterns[0].startPattern = false; //패턴 발생 끄기
                bossPatterns[0].usedPattern = true; //패턴 사용 여부 등록
                UsingPattern = false; //패턴이 끝났음을 의미
            }
            return;
        }
        //transform.position += dirTarget.normalized * pokeSpeed * Time.deltaTime; //입력한 공격방향으로 찌르기
        transform.position = Vector3.MoveTowards(transform.position, posTarget, Time.deltaTime * pokeSpeed);
    }

    /// <summary>
    /// 플레이어의 위치를 추적하여 메테오를 생성하는 함수
    /// </summary>
    private void MeteorInstantiate()
    {
        Transform trsPlayer = GameObject.Find("Player").transform;
        Instantiate(bossPatterns[1].objPattern, new Vector3(trsPlayer.position.x, trsPlayer.position.y + 10f, 0f), Quaternion.identity);
    }

    /// <summary>
    /// 수직형 라바 패턴
    /// </summary>
    private void LavaInstantiate01()
    {
        lavaRange += 5f;//5//10//15//20
        Vector3 rangePlusX = new Vector3(transform.position.x + lavaRange, transform.position.y, 0f);
        Vector3 rangeMinusX = new Vector3(transform.position.x - lavaRange, transform.position.y, 0f);
        Vector3 rangePlusY = new Vector3(transform.position.x, transform.position.y + lavaRange, 0f);
        Vector3 rangeMinusY = new Vector3(transform.position.x, transform.position.y - lavaRange, 0f);
        Instantiate(bossPatterns[2].objPattern, rangePlusX, Quaternion.identity); //오른쪽
        Instantiate(bossPatterns[2].objPattern, rangeMinusX, Quaternion.identity); //왼쪽
        Instantiate(bossPatterns[2].objPattern, rangePlusY, Quaternion.identity); //위
        Instantiate(bossPatterns[2].objPattern, rangeMinusY, Quaternion.identity); //아래
    }

    /// <summary>
    /// 대각형 라바 패턴
    /// </summary>
    private void LavaInstantiate02()
    {
        lavaRange += 5f;
        Vector3 rangePXPY = new Vector3(transform.position.x + lavaRange, transform.position.y + lavaRange, 0f);
        Vector3 rangePXMY = new Vector3(transform.position.x + lavaRange, transform.position.y - lavaRange, 0f);
        Vector3 rangeMXPY = new Vector3(transform.position.x - lavaRange, transform.position.y + lavaRange, 0f);
        Vector3 rangeMXMY = new Vector3(transform.position.x - lavaRange, transform.position.y - lavaRange, 0f);
        Instantiate(bossPatterns[2].objPattern, rangePXPY, Quaternion.identity); //오른쪽 위
        Instantiate(bossPatterns[2].objPattern, rangePXMY, Quaternion.identity); //오른쪽 아래
        Instantiate(bossPatterns[2].objPattern, rangeMXPY, Quaternion.identity); //왼쪽 위
        Instantiate(bossPatterns[2].objPattern, rangeMXMY, Quaternion.identity); //왼쪽 아래
    }


    /// <summary>
    /// 불 장판 소환 필드 생성
    /// </summary>
    /// <returns></returns>
    private (Vector3 _minVector, Vector3 _maxVector) FireLineField()
    {
        minVector = fireLineSpawnPoints[0].position; //좌표 최소점
        maxVector = fireLineSpawnPoints[1].position; //좌표 최대점
        return (minVector, maxVector);
    }

    /// <summary>
    /// 불 장판 생성
    /// </summary>
    private void SpawnFireLine()
    {
        FireLineField(); //필드 생성

        for (int iNum01 = 0; iNum01 < 3; iNum01++)
        {
            float xVector = Random.Range(minVector.x, maxVector.x); //x좌표 랜덤 적용
            float yVector = Random.Range(minVector.y, maxVector.y); //y좌표 랜덤 적용
            Vector3 spawnField = new Vector3(xVector, yVector, 0); //소환 좌표 적용

            GameObject obj = Instantiate(bossPatterns[3].objPattern, spawnField, Quaternion.identity);

            Destroy(obj, 5f);
        }
    }

    /// <summary>
    /// 보스 패턴 관리
    /// - 매개변수를 Enum으로 사용하여 알맞은 패턴을 switch문에 적용
    /// </summary>
    private void PatternManager(PatternName _pattern, bool _startPattern = false) //패턴 이름을 넣으며 평상시에 false로 넣기
    {
        switch (_pattern)
        {
            case PatternName.PokePattern:
                if (Input.GetKeyDown(KeyCode.Alpha1)) //패턴 발동 트리거
                {
                    bossPatterns[0].startPattern = true;
                    bossPatterns[0].rePattern = true;
                }

                if (bossPatterns[0].rePattern && _startPattern) //패턴 시작하거나 재사용할 경우
                {
                    bossPatterns[0].curRepeatNum += 1; //패턴 횟수 1증가
                    bossPatterns[0].curDelayTime = bossPatterns[0].setDelayTime;
                    posSearch = true;
                    bossPatterns[0].rePattern = false;
                }
                PokePlayer();
                break;

            case PatternName.MeteorPattern:
                if (Input.GetKeyDown(KeyCode.Alpha2)) //패턴 발동 트리거
                {
                    bossPatterns[1].startPattern = true;
                    bossPatterns[1].curDelayTime = bossPatterns[1].setDelayTime;
                }

                if (bossPatterns[1].curRepeatNum != bossPatterns[1].setRepeatNum && bossPatterns[1].startPattern) //연속으로 쓴 횟수와 설정한 연속 횟수가 맞지 않고 패턴 사용중일 경우
                {
                    bossPatterns[1].curDelayTime -= Time.deltaTime; //다시 쓰기 까지의 딜레이 시간

                    if (bossPatterns[1].curDelayTime <= 0f) //딜레이 시간이 끝나면 다시 패턴 반복
                    {
                        MeteorInstantiate();
                        bossPatterns[1].curDelayTime = bossPatterns[1].setDelayTime; //딜레이 타임 설정
                        bossPatterns[1].curRepeatNum += 1; //패턴 연속 횟수 증가
                    }

                }

                else if (bossPatterns[1].curRepeatNum == bossPatterns[1].setRepeatNum) //패턴을 다 사용했을 경우
                {
                    bossPatterns[1].curRepeatNum = 0; //패턴 횟수 초기화
                    bossPatterns[1].startPattern = false; //패턴 발생 끄기
                    bossPatterns[1].usedPattern = true;
                    UsingPattern = false; //패턴이 끝났음을 의미
                }
                break;

            case PatternName.LavaPattern:
                if (Input.GetKeyDown(KeyCode.Alpha3)) //패턴 발동 트리거
                {
                    bossPatterns[2].startPattern = true;
                    randLava = Random.Range(0, 2); //두 가지의 라바 패턴 중 하나를 결정
                }

                if (bossPatterns[2].curRepeatNum != bossPatterns[2].setRepeatNum && bossPatterns[2].startPattern) //연속으로 쓴 횟수와 설정한 연속 횟수가 맞지 않고 패턴 사용중일 경우
                {
                    bossPatterns[2].curDelayTime -= Time.deltaTime; //다시 쓰기 까지의 딜레이 시간

                    if (bossPatterns[2].curDelayTime <= 0f) //딜레이 시간이 끝나면 다시 패턴 반복
                    {
                        if (randLava == 0)
                        {
                            LavaInstantiate01(); //수직형 라바 패턴
                        }

                        else if (randLava == 1)
                        {
                            LavaInstantiate02(); //대각형 라바 패턴
                        }
                        bossPatterns[2].curDelayTime = bossPatterns[2].setDelayTime; //딜레이 타임 설정
                        bossPatterns[2].curRepeatNum += 1; //패턴 연속 횟수 증가
                    }

                }

                else if (bossPatterns[2].curRepeatNum == bossPatterns[2].setRepeatNum) //패턴을 다 사용했을 경우
                {
                    bossPatterns[2].curRepeatNum = 0; //패턴 횟수 초기화
                    bossPatterns[2].startPattern = false; //패턴 발생 끄기
                    bossPatterns[2].usedPattern = true;
                    lavaRange = 0f; //사거리 초기화
                    UsingPattern = false; //패턴이 끝났음을 의미
                }
                break;

            case PatternName.LinePattern:
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    bossPatterns[3].startPattern = true;
                    SpawnFireLine();
                }
                if (bossPatterns[3].startPattern == true)
                { 
                    bossPatterns[3].startPattern = false;
                    bossPatterns[3].usedPattern = true;
                    UsingPattern = false; //패턴이 끝났음을 의미
                }
                break;
        }
    }
}
