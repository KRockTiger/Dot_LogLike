using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossEnemy : MonoBehaviour
{
    public enum PatternName
    {
        PokePattern,
        MeteorPattern,
        LavaPattern,
        LinePattern,
    }

    [System.Serializable]
    public class BossPattern
    {
        public GameObject objPattern;
        public PatternName patternName; //패턴 이름
        public int patternNum; //패턴 횟수
        public float delayTime; //패턴에 딜레이를 넣고 싶을 때 사용
        public bool startPattern = false; //패턴 시작 여부
        public bool usedPattern = false; //패턴 사용 여부
    }

    [Header("보스 패턴")]
    [SerializeField] private List<BossPattern> bossPatterns;
    [SerializeField] private float lavaRange;
    [SerializeField] private Transform[] fireLineSpawnPoints;
    private int randLava = 0;

    [Header("보스 스텟")]
    [SerializeField] private float pokeSpeed = 20f;

    //private int bossPhase = 1;
    private Vector3 posTarget; //플레이어의 위치를 타겟으로 넣을 벡터 변수
    private Vector3 dirTarget; //현 위치에서 플레이어 위치까지의 방향을 정할 벡터 변수
    private Vector3 startPoint; //보스가 찌르기 공격할 때 찌르기 시작하는 위치를 저장하는 곳
    private Vector3 minVector;
    private Vector3 maxVector;
    [SerializeField] private float delayTime; //패턴 딜레이 시간
    [SerializeField] private int repeatPattern = 1; //패턴을 몇 번 연속으로 사용했는지 확인
    [SerializeField] private bool rePattern; //패턴 사용 중일경우 일부 기능을 제한 하기 위한 bool형 변수
    [SerializeField] private bool posSearch; //위치를 한 번만 찾기 위한 제어기

    private void Update()
    {
        //PatternManager(PatternName.PokePattern, bossPatterns[0].startPattern);
        //PatternManager(PatternName.MeteorPattern, bossPatterns[1].startPattern);
        //PatternManager(PatternName.LavaPattern, bossPatterns[2].startPattern);
        PatternManager(PatternName.LinePattern, bossPatterns[3].startPattern);
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
    /// 패턴을 변화 시키기 위한 함수
    /// - 하나의 패턴이 끝나면 다음 패턴을 등록해야하며 다음 패턴 사용하기의 딜레이가 필요
    /// </summary>
    private void ChangePattern()
    {
        int RandPattern = Random.Range(0, bossPatterns.Count);
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

        if (distance < 0.25f) //플레이어와의 거리가 0.25정도로 좁아지면
        {
            if (repeatPattern != bossPatterns[0].patternNum && bossPatterns[0].startPattern) //연속으로 쓴 횟수와 설정한 연속 횟수가 맞지 않고 패턴 사용중일 경우
            {
                delayTime -= Time.deltaTime; //다시 쓰기 까지의 딜레이 시간        

                if (delayTime <= 0f) //딜레이 시간이 끝나면 다시 패턴 반복
                {
                    rePattern = true;
                }

            }

            else //만약 일정 횟수를 반복이 끝나면
            {
                repeatPattern = 0; //패턴 횟수 초기화
                bossPatterns[0].startPattern = false; //패턴 발생 끄기
                bossPatterns[0].usedPattern = true; //패턴 사용 여부 등록
            }
            return;
        }
        transform.position += dirTarget.normalized * pokeSpeed * Time.deltaTime; //입력한 공격방향으로 찌르기
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
    /// 보스 패턴 관리
    /// - 매개변수를 Enum으로 사용하여 알맞은 패턴을 switch문에 적용
    /// </summary>
    private void PatternManager(PatternName _pattern, bool _startPattern = false) //패턴 이름을 넣으며 평상시에 false로 넣기
    {
        switch (_pattern)
        {
            case PatternName.PokePattern:
                if (Input.GetKeyDown(KeyCode.T)) //패턴 발동 트리거
                {
                    bossPatterns[0].startPattern = true;
                    rePattern = true;
                }
                if (rePattern && _startPattern)
                {
                    repeatPattern += 1;
                    delayTime = bossPatterns[0].delayTime;
                    posSearch = true;
                    rePattern = false;
                }
                PokePlayer();
                break;

            case PatternName.MeteorPattern:
                if (Input.GetKeyDown(KeyCode.R)) //패턴 발동 트리거
                {
                    bossPatterns[1].startPattern = true;
                    rePattern = true;
                    delayTime = bossPatterns[1].delayTime;
                }

                if (repeatPattern != bossPatterns[1].patternNum && bossPatterns[1].startPattern) //연속으로 쓴 횟수와 설정한 연속 횟수가 맞지 않고 패턴 사용중일 경우
                {
                    delayTime -= Time.deltaTime; //다시 쓰기 까지의 딜레이 시간

                    if (delayTime <= 0f) //딜레이 시간이 끝나면 다시 패턴 반복
                    {
                        rePattern = true;
                        MeteorInstantiate();
                        delayTime = bossPatterns[1].delayTime; //딜레이 타임 설정
                        repeatPattern += 1; //패턴 연속 횟수 증가
                    }

                }

                else if (repeatPattern == bossPatterns[1].patternNum) //패턴을 다 사용했을 경우
                {
                    repeatPattern = 0; //패턴 횟수 초기화
                    bossPatterns[1].startPattern = false; //패턴 발생 끄기
                    bossPatterns[1].usedPattern = true;
                }
                break;

            case PatternName.LavaPattern:
                if (Input.GetKeyDown(KeyCode.G)) //패턴 발동 트리거
                {
                    bossPatterns[2].startPattern = true;
                    rePattern = true;
                    randLava = Random.Range(0, 2); //두 가지의 라바 패턴 중 하나를 결정
                }

                if (repeatPattern != bossPatterns[2].patternNum && bossPatterns[2].startPattern) //연속으로 쓴 횟수와 설정한 연속 횟수가 맞지 않고 패턴 사용중일 경우
                {
                    delayTime -= Time.deltaTime; //다시 쓰기 까지의 딜레이 시간

                    if (delayTime <= 0f) //딜레이 시간이 끝나면 다시 패턴 반복
                    {
                        rePattern = true;
                        if (randLava == 0)
                        {
                            LavaInstantiate01(); //수직형 라바 패턴
                        }

                        else if (randLava == 1)
                        {
                            LavaInstantiate02(); //대각형 라바 패턴
                        }
                        delayTime = bossPatterns[2].delayTime; //딜레이 타임 설정
                        repeatPattern += 1; //패턴 연속 횟수 증가
                    }

                }

                else if (repeatPattern == bossPatterns[2].patternNum) //패턴을 다 사용했을 경우
                {
                    repeatPattern = 0; //패턴 횟수 초기화
                    bossPatterns[2].startPattern = false; //패턴 발생 끄기
                    bossPatterns[2].usedPattern = true;
                    lavaRange = 0f; //사거리 초기화
                }
                break;

            case PatternName.LinePattern:
                if (Input.GetKeyDown(KeyCode.V))
                {
                    bossPatterns[3].startPattern = true;
                    rePattern = true;
                }
                break;
        }
    }

    //private (Vector3 _minVector, Vector3 _maxVector) FireLineInstantiate()
    //{
    //    minVector = spawnPoints[0].position;
    //    maxVector = spawnPoints[1].position;

    //    return (minVector, maxVector);
    //}
}
