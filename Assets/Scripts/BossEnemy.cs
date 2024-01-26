using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossEnemy : Enemy
{
    public enum PatternName //���� �̸�
    {
        PokePattern,
        MeteorPattern,
        LavaPattern,
        LinePattern,
    }

    [System.Serializable]
    public class BossPattern //���� ���� Ŭ����
    {
        public GameObject objPattern; //����� ��ų ������Ʈ
        public PatternName patternName; //���� �̸�
        public int setRepeatNum; //������ ���� Ƚ��
        public int curRepeatNum; //���� �ݺ��� ���� Ƚ��
        public float setDelayTime; //���Ͽ� �����̸� �ְ� ���� �� ���
        public float curDelayTime; //���� ������ �ð�
        public bool startPattern = false; //���� ���� ����
        public bool rePattern = false; //���� ���� ����
        public bool usedPattern = false; //���� ���� ������ �� �̹� ������ ���� ������� �ʱ� ���� ���
    }

    [Header("���� ����")]
    [SerializeField] private List<BossPattern> bossPatterns; //���� ����Ʈ �����
    [SerializeField] private float lavaRange; //��� ���� ���� ����
    [SerializeField] private Transform[] fireLineSpawnPoints; //�� ���� ������ �ʵ� ���̸� ������ Transform
    [SerializeField] private int bossPhase = 1; //���� ������ Ȯ��
    [SerializeField] private bool isGrog = false; //�׷α� ����
    private int randLava = 0; //��� ������ 2������ ������ �������� �ϳ��� ���� ��Ű�� ���� Random.Range�� ����
    private int randUpgrade = 0; //���׷��̵� ������ �����Ű�� ���� Random.Range�� ����
    private bool isRand = true; //��� ������ �� ���� �����ϱ� ���� ���
    private bool isUpPattern = false; //���׷��̵� ���� �ߵ��ϱ� ���� Ʈ����

    [Header("���� ����")]
    [SerializeField] private float pokeSpeed = 20f; //��� ������ �̵� �ӵ�
    [SerializeField] private float setGrogTime; //������ �׷α� �ð�
    [SerializeField] private float curGrogTime; //���� �׷α� �ð�

    private Vector3 posTarget; //�÷��̾��� ��ġ�� Ÿ������ ���� ���� ����
    private Vector3 minVector; //�� ���� �ּ���
    private Vector3 maxVector; //�� ���� �ִ���
    private bool posSearch; //��ġ�� �� ���� ã�� ���� �����
    private bool _usingPattern; //���� ��� ���� �� true
    private bool UsingPattern
    {
        set
        {
            _usingPattern = value;
        }
        get => _usingPattern;
    }

    [SerializeField] private float setDelayTime; //���� ���� �ߵ��ϱ��� ������ �ð� ����
    [SerializeField] private float curDelayTime; //���� ���� �ߵ��ϱ��� ���� ������ �ð�    
    [SerializeField] private bool testStart = false; //�׽�Ʈ����

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.PSetBossBattle(true); //������ Ű��
        curGrogTime = 1f;
        isGrog = true;
    }

    public override void Update()
    {
        base.Update(); //�Ϲݸ� �ڵ嵵 ���
        gameManager.PBossPhaseUI(bossPhase);

        if (testStart) { return; }

        if (!UsingPattern) //���� ��� ���� �ƴϰ� ü���� ������ �ƴҰ��
        {
            NextPattern(); //���� �ٲٱ�
        }

        if (isGrog)
        {
            GrogTime();
            return;
        }

        if (UsingPattern && !isUpPattern) //���� ����� ��ϵǰ� 3����� ���׷��̵� ���� �ߵ��� �ƴ� ���
        {
            PatternManager(PatternName.PokePattern, bossPatterns[0].startPattern);
            PatternManager(PatternName.MeteorPattern, bossPatterns[1].startPattern);
            PatternManager(PatternName.LavaPattern, bossPatterns[2].startPattern);
            PatternManager(PatternName.LinePattern, bossPatterns[3].startPattern);
        }

        if (isUpPattern) //���׷��̵� ������ �ߵ��Ǹ�
        {
            UpgradePattern(randUpgrade);
        }
    }

    /// <summary>
    /// ���� �ѱ�� �Լ�
    /// </summary>
    private void NextPattern()
    {
        if (isGrog)
        { return; }

        curDelayTime -= Time.deltaTime; //���� �ѱ�� ������ �ð� ����

        if (curDelayTime <= 0f) //������ Ÿ���� ������
        {
            curDelayTime = setDelayTime;
            CheckPattern(); //���� ��ü
        }
    }

    /// <summary>
    /// ������ ��ȭ ��Ű�� ���� �Լ�
    /// </summary>
    private void CheckPattern()
    {
        switch (bossPhase) //���� ������ Ȯ��
        {
            case 1: //1�������� ��� ==> 1������� ������ ������ 4���� 3���� �������� ����� ��ȹ

                int phase01Count = 0; //��� Ƚ�� ������ ���� ����

                for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++) //���� ��� Ž��
                {
                    if (bossPatterns[iNum01].usedPattern) //������ ��� �Ǿ�����
                    {
                        phase01Count += 1; //���Ƚ�� 1 �߰�
                    }
                } //==> �� ���� ��뿩�θ� Ȯ���Ͽ� ���Ƚ���� ���� ��Ű�� Ȯ��

                if (phase01Count != 3) //���� Ƚ���� 3�� �ƴ� ���
                {
                    ChangePattern(); //���� ��ȭ ��Ų�� �ߵ�
                }

                else //���� Ƚ���� 3�� ���
                {
                    curGrogTime = setGrogTime; //�׷α� �ð� ����
                    isGrog = true; //�׷α� ����
                    ResetPattern(); //�׷α� �� �ʱ�ȭ�� ���� ���
                }
                break;

            case 2: //2�������� ���

                for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++) //���� ��� Ž��
                {
                    if (!bossPatterns[iNum01].usedPattern) //���� ��� ���� ������ ������
                    {
                        ChangePattern(); //���� ��ȭ ��Ų�� �ߵ�

                        return;
                    }
                }

                curGrogTime = setGrogTime; //�׷α� �ð� ����
                isGrog = true; //�׷α� ����
                ResetPattern(); //��� ������ ���Ǿ����� �׷α� �� �ʱ�ȭ�� ���� ���
                break;

            case 3: //3�������� ���

                for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++) //���� ��� Ž��
                {
                    if (!bossPatterns[iNum01].usedPattern) //���� ��� ���� ������ ������
                    {
                        ChangePattern(); //���� ��ȭ ��Ų�� �ߵ�

                        return;
                    }
                }
                randUpgrade = Random.Range(0, 2);
                isUpPattern = true; //���׷��̵� ���� �ߵ�
                break;
        }
    }

    /// <summary>
    /// ������ ��ȭ ��Ű�� ���� �Լ�
    /// - �ϳ��� ������ ������ ���� ������ ����ؾ��ϸ� ���� ���� ����ϱ��� �����̰� �ʿ�
    /// </summary>
    private void ChangePattern()
    {
        int randNum = Random.Range(0, bossPatterns.Count); //������ ���� ��ȣ�� ����

        if (bossPatterns[randNum].usedPattern) //���� �������� ������ ��ȣ�� ������ �̹� �ߵ��� �Ŷ�� �ٽ� ����
        {
            while (bossPatterns[randNum].usedPattern) //�ߵ��� ������ �� �ɸ� �� �����Ƿ� 
            {
                randNum = Random.Range(0, bossPatterns.Count); //�ٽ� �缳��
            }
        }

        bossPatterns[randNum].startPattern = true; //���� �ߵ�
        UsingPattern = true; //���� ��� ���
    }

    /// <summary>
    /// ����� ���� �ٽ� �ʱ�ȭ
    /// </summary>
    private void ResetPattern()
    {
        for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++)
        {
            bossPatterns[iNum01].usedPattern = false; //���� �ʱ�ȭ
        }
    }

    /// <summary>
    /// �������� �� ���� ������ ���� ���
    /// </summary>
    private void PhaseChange()
    {
        switch (bossPhase) //���� ������ Ȯ��
        {
            case 1: //1������
                for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++)
                {
                    bossPatterns[iNum01].setRepeatNum = 3;
                }
                break;

            case 2: //2������
                for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++)
                {
                    bossPatterns[iNum01].setRepeatNum = 4;
                }
                break;

            case 3: //3������
                for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++)
                {
                    bossPatterns[iNum01].setRepeatNum = 5;
                }
                break;
        }
    }

    /// <summary>
    /// �÷��̾��� ��ġ�� ã�� �Լ�
    /// - �÷��̾��� ��ġ�� �ѹ��� ã�� ������ �ַ� Ÿ�� ��ġ�� ������ �� ���
    /// </summary>
    private void SearchPlayer()
    {
        if (!posSearch) //���� ��� �� �ϰ�� ����
        { return; }
        posTarget = GameObject.Find("Player").transform.position; //�÷��̾��� ��ġ�� ����        
        posSearch = false; //�ѹ� ����ϸ� �ٷ� ���
    }

    /// <summary>
    /// �÷��̾��� ��ġ�� ã�� ��� ����
    /// </summary>
    private void PokePlayer()
    {
        if (posSearch) //�÷��̾��� ��ġ ã�� ����
        {
            SearchPlayer();
        }
        float distance = Vector3.Distance(posTarget, transform.position); //Ÿ�� ��ǥ�� �� ���� ������Ʈ ��ġ ��ǥ ���̷� �Ÿ��� ���

        if (distance < 0.3f) //�÷��̾���� �Ÿ��� 0.3������ �������� ==> distance�� �Ÿ��� ���̸� ����ϹǷ� ������ ������ �����ؾ� �Ѵ�.
        {
            if (bossPatterns[0].curRepeatNum != bossPatterns[0].setRepeatNum && bossPatterns[0].startPattern) //�������� �� Ƚ���� ������ ���� Ƚ���� ���� �ʰ� ���� ������� ���
            {
                bossPatterns[0].curDelayTime -= Time.deltaTime; //�ٽ� ���� ������ ������ �ð�        

                if (bossPatterns[0].curDelayTime <= 0f) //������ �ð��� ������ �ٽ� ���� �ݺ�
                {
                    bossPatterns[0].rePattern = true; //���� ���� ���
                }

            }

            else //���� ���� Ƚ���� �ݺ��� ������
            {
                bossPatterns[0].curRepeatNum = 0; //���� Ƚ�� �ʱ�ȭ
                bossPatterns[0].startPattern = false; //���� �߻� ����
                bossPatterns[0].usedPattern = true; //���� ��� ���� ���
                UsingPattern = false; //������ �������� �ǹ�
            }
            return;
        }
        //transform.position += dirTarget.normalized * pokeSpeed * Time.deltaTime; => �������� ���� �̵��� �� ���� �ڵ�
        transform.position = Vector3.MoveTowards(transform.position, posTarget, Time.deltaTime * pokeSpeed); //�Է��� ���ݹ������� ���
        //�̵��� �������� ���� ��� Vector3.MoveTowards(�̵����� ��ǥ, ��ǥ��ǥ, �̵� �ð�)�� �̿�
    }

    /// <summary>
    /// �÷��̾��� ��ġ�� �����Ͽ� ���׿��� �����ϴ� �Լ�
    /// </summary>
    private void MeteorInstantiate()
    {
        Transform trsPlayer = GameObject.Find("Player").transform;
        Instantiate(bossPatterns[1].objPattern, new Vector3(trsPlayer.position.x, trsPlayer.position.y + 10f, 0f), Quaternion.identity);
    }

    /// <summary>
    /// ������ ��� ����
    /// </summary>
    private void LavaInstantiate01()
    {
        lavaRange += 5f;//5//10//15//20
        Vector3 rangePlusX = new Vector3(transform.position.x + lavaRange, transform.position.y, 0f);
        Vector3 rangeMinusX = new Vector3(transform.position.x - lavaRange, transform.position.y, 0f);
        Vector3 rangePlusY = new Vector3(transform.position.x, transform.position.y + lavaRange, 0f);
        Vector3 rangeMinusY = new Vector3(transform.position.x, transform.position.y - lavaRange, 0f);
        Instantiate(bossPatterns[2].objPattern, rangePlusX, Quaternion.identity); //������
        Instantiate(bossPatterns[2].objPattern, rangeMinusX, Quaternion.identity); //����
        Instantiate(bossPatterns[2].objPattern, rangePlusY, Quaternion.identity); //��
        Instantiate(bossPatterns[2].objPattern, rangeMinusY, Quaternion.identity); //�Ʒ�
    }

    /// <summary>
    /// �밢�� ��� ����
    /// </summary>
    private void LavaInstantiate02()
    {
        lavaRange += 5f;
        Vector3 rangePXPY = new Vector3(transform.position.x + lavaRange, transform.position.y + lavaRange, 0f);
        Vector3 rangePXMY = new Vector3(transform.position.x + lavaRange, transform.position.y - lavaRange, 0f);
        Vector3 rangeMXPY = new Vector3(transform.position.x - lavaRange, transform.position.y + lavaRange, 0f);
        Vector3 rangeMXMY = new Vector3(transform.position.x - lavaRange, transform.position.y - lavaRange, 0f);
        Instantiate(bossPatterns[2].objPattern, rangePXPY, Quaternion.identity); //������ ��
        Instantiate(bossPatterns[2].objPattern, rangePXMY, Quaternion.identity); //������ �Ʒ�
        Instantiate(bossPatterns[2].objPattern, rangeMXPY, Quaternion.identity); //���� ��
        Instantiate(bossPatterns[2].objPattern, rangeMXMY, Quaternion.identity); //���� �Ʒ�
    }

    /// <summary>
    /// �� ���� ��ȯ �ʵ� ����
    /// </summary>
    /// <returns></returns>
    private (Vector3 _minVector, Vector3 _maxVector) FireLineField()
    {
        minVector = fireLineSpawnPoints[0].position; //��ǥ �ּ���
        maxVector = fireLineSpawnPoints[1].position; //��ǥ �ִ���
        return (minVector, maxVector);
    }

    /// <summary>
    /// �� ���� ����
    /// </summary>
    private void SpawnFireLine()
    {
        FireLineField(); //�ʵ� ����

        for (int iNum01 = 0; iNum01 < bossPatterns[3].setRepeatNum; iNum01++)
        {
            float xVector = Random.Range(minVector.x, maxVector.x); //x��ǥ ���� ����
            float yVector = Random.Range(minVector.y, maxVector.y); //y��ǥ ���� ����
            Vector3 spawnField = new Vector3(xVector, yVector, 0); //��ȯ ��ǥ ����

            GameObject obj = Instantiate(bossPatterns[3].objPattern, spawnField, Quaternion.identity);

            Destroy(obj, 5f);
        }
    }

    /// <summary>
    /// �׷α� ���¿� ������ ���
    /// </summary>
    private void GrogTime()
    {
        curGrogTime -= Time.deltaTime; //�׷α� Ÿ��

        if (curGrogTime <= 0f)
        {
            curDelayTime = 0f; //������ �ð� �ʱ�ȭ
            isGrog = false;
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// - �Ű������� Enum���� ����Ͽ� �˸��� ������ switch���� ����
    /// - ��� ����� PatternManager(����� ���� �̸�, true�� ����� �ߵ�)
    /// </summary>
    private void PatternManager(PatternName _pattern, bool _startPattern = false) //���� �̸��� ������ ���ÿ� false�� �ֱ�
    {
        switch (_pattern)
        {
            case PatternName.PokePattern: //��� ���� -------------------------------------------------------------------------------------------------
                if (bossPatterns[0].rePattern && _startPattern) //���� �����ϰų� ������ ���
                {
                    bossPatterns[0].curRepeatNum += 1; //���� Ƚ�� 1����
                    bossPatterns[0].curDelayTime = bossPatterns[0].setDelayTime; //������ Ÿ�� ����
                    posSearch = true; //�÷��̾� ��ġ ã��
                    bossPatterns[0].rePattern = false; //���� ��ư ����
                }

                if (_startPattern)
                {
                    if (bossPatterns[0].curRepeatNum == 0) //ù ��⸸ ����
                    {
                        bossPatterns[0].curRepeatNum += 1; //���� Ƚ�� 1����
                        bossPatterns[0].curDelayTime = bossPatterns[0].setDelayTime; //������ Ÿ�� ����
                        posSearch = true; //�÷��̾� ��ġ ã��
                    }
                    PokePlayer(); //��� ����
                }
                break;

            case PatternName.MeteorPattern: //���׿� ���� --------------------------------------------------------------------------------------------------
                if (bossPatterns[1].curRepeatNum != bossPatterns[1].setRepeatNum && bossPatterns[1].startPattern) //�������� �� Ƚ���� ������ ���� Ƚ���� ���� �ʰ� ���� ������� ���
                {
                    bossPatterns[1].curDelayTime -= Time.deltaTime; //�ٽ� ���� ������ ������ �ð�

                    if (bossPatterns[1].curDelayTime <= 0f) //������ �ð��� ������ �ٽ� ���� �ݺ�
                    {
                        MeteorInstantiate();
                        bossPatterns[1].curDelayTime = bossPatterns[1].setDelayTime; //������ Ÿ�� ����
                        bossPatterns[1].curRepeatNum += 1; //���� ���� Ƚ�� ����
                    }

                }

                else if (bossPatterns[1].curRepeatNum == bossPatterns[1].setRepeatNum) //������ �� ������� ���
                {
                    bossPatterns[1].curRepeatNum = 0; //���� Ƚ�� �ʱ�ȭ
                    bossPatterns[1].startPattern = false; //���� �߻� ����
                    bossPatterns[1].usedPattern = true;
                    UsingPattern = false; //������ �������� �ǹ�
                }
                break;

            case PatternName.LavaPattern: //��� ���� -----------------------------------------------------------------------------------------------------                
                if (isRand && bossPatterns[2].startPattern)
                {
                    lavaRange = 0f;
                    randLava = Random.Range(0, 2); //�� ������ ��� ���� �� �ϳ��� ����
                    Debug.Log("���� ����");
                    isRand = false;
                }

                if (bossPatterns[2].curRepeatNum != bossPatterns[2].setRepeatNum && bossPatterns[2].startPattern) //�������� �� Ƚ���� ������ ���� Ƚ���� ���� �ʰ� ���� ������� ���
                {
                    bossPatterns[2].curDelayTime -= Time.deltaTime; //�ٽ� ���� ������ ������ �ð�

                    if (bossPatterns[2].curDelayTime <= 0f) //������ �ð��� ������ �ٽ� ���� �ݺ�
                    {
                        if (randLava == 0)
                        {
                            LavaInstantiate01(); //������ ��� ����
                        }

                        else if (randLava == 1)
                        {
                            LavaInstantiate02(); //�밢�� ��� ����
                        }
                        bossPatterns[2].curDelayTime = bossPatterns[2].setDelayTime; //������ Ÿ�� ����
                        bossPatterns[2].curRepeatNum += 1; //���� ���� Ƚ�� ����
                    }

                }

                else if (bossPatterns[2].curRepeatNum == bossPatterns[2].setRepeatNum) //������ �� ������� ���
                {
                    bossPatterns[2].curRepeatNum = 0; //���� Ƚ�� �ʱ�ȭ
                    bossPatterns[2].startPattern = false; //���� �߻� ����
                    bossPatterns[2].usedPattern = true;
                    lavaRange = 0f; //��Ÿ� �ʱ�ȭ
                    isRand = true; //���� Ȱ��ȭ
                    UsingPattern = false; //������ �������� �ǹ�
                }
                break;

            case PatternName.LinePattern: //�� ���� ���� --------------------------------------------------------------------------------------------
                if (bossPatterns[3].startPattern == true)
                {
                    SpawnFireLine(); //������Ʈ ����
                    bossPatterns[3].startPattern = false; //startƮ���� ����
                    bossPatterns[3].usedPattern = true; //���� ��������� Ȯ��
                    UsingPattern = false; //������ �������� �ǹ�
                }
                break;
        }
    }

    /// <summary>
    /// 3������ ���¿��� 4�� ���� �� ����� �� ��ȭ ������ ���
    /// ��ȭ ������ �� 3���̰� �� �� �ϳ��� �������� �ߵ�
    /// false�� �ᰡ���ٰ� true�� �ع�
    /// </summary>
    private void UpgradePattern(int _randPattern)
    {
        switch (_randPattern)
        {
            case 0:
                    UpGradeLavaBoom01();
                    isUpPattern = false;
                    curGrogTime = setGrogTime; //�׷α� �ð� ����
                    isGrog = true; //�׷α� ����
                    ResetPattern(); //��� ������ ���Ǿ����� �׷α� �� �ʱ�ȭ�� ���� ���                
                break;

            case 1:
                UpGradeLavaBoom02();
                isUpPattern = false;
                curGrogTime = setGrogTime; //�׷α� �ð� ����
                isGrog = true; //�׷α� ����
                ResetPattern(); //��� ������ ���Ǿ����� �׷α� �� �ʱ�ȭ�� ���� ���   
                break;
        }
    }

    /// <summary>
    /// ���׷��̵� ��� ���� 01
    /// </summary>
    private void UpGradeLavaBoom01()
    {
        lavaRange = 0f;
        lavaRange += 5f;
        Vector3 rangePX01 = new Vector3(transform.position.x + lavaRange, transform.position.y, 0f);
        Vector3 rangeMX01 = new Vector3(transform.position.x - lavaRange, transform.position.y, 0f);
        Vector3 rangePY01 = new Vector3(transform.position.x, transform.position.y + lavaRange, 0f);
        Vector3 rangeMY01 = new Vector3(transform.position.x, transform.position.y - lavaRange, 0f);
        Instantiate(bossPatterns[2].objPattern, rangePX01, Quaternion.identity); //������
        Instantiate(bossPatterns[2].objPattern, rangeMX01, Quaternion.identity); //����
        Instantiate(bossPatterns[2].objPattern, rangePY01, Quaternion.identity); //��
        Instantiate(bossPatterns[2].objPattern, rangeMY01, Quaternion.identity); //�Ʒ�

        for (int iNum01 = 0; iNum01 < 8; iNum01++)
        {
            lavaRange += 5f;
            Vector3 rangePX02 = new Vector3(transform.position.x + lavaRange, transform.position.y, 0f);
            Vector3 rangeMX02 = new Vector3(transform.position.x - lavaRange, transform.position.y, 0f);
            Vector3 rangePY02 = new Vector3(transform.position.x, transform.position.y + lavaRange, 0f);
            Vector3 rangeMY02 = new Vector3(transform.position.x, transform.position.y - lavaRange, 0f);
            Instantiate(bossPatterns[2].objPattern, rangePX02, Quaternion.identity); //������
            Instantiate(bossPatterns[2].objPattern, rangeMX02, Quaternion.identity); //����
            Instantiate(bossPatterns[2].objPattern, rangePY02, Quaternion.identity); //��
            Instantiate(bossPatterns[2].objPattern, rangeMY02, Quaternion.identity); //�Ʒ�
        }
    }

    /// <summary>
    /// ���׷��̵� ��� ���� 02
    /// </summary>
    private void UpGradeLavaBoom02()
    {
        lavaRange = 0f;
        lavaRange += 5f;
        Vector3 rangePXPY01 = new Vector3(transform.position.x + lavaRange, transform.position.y + lavaRange, 0f);
        Vector3 rangePXMY01 = new Vector3(transform.position.x + lavaRange, transform.position.y - lavaRange, 0f);
        Vector3 rangeMXPY01 = new Vector3(transform.position.x - lavaRange, transform.position.y + lavaRange, 0f);
        Vector3 rangeMXMY01 = new Vector3(transform.position.x - lavaRange, transform.position.y - lavaRange, 0f);
        Instantiate(bossPatterns[2].objPattern, rangePXPY01, Quaternion.identity); //������ ��
        Instantiate(bossPatterns[2].objPattern, rangePXMY01, Quaternion.identity); //������ �Ʒ�
        Instantiate(bossPatterns[2].objPattern, rangeMXPY01, Quaternion.identity); //���� ��
        Instantiate(bossPatterns[2].objPattern, rangeMXMY01, Quaternion.identity); //���� �Ʒ�

        for (int iNum01 = 0; iNum01 < 8; iNum01++)
        {
            lavaRange += 5f;
            Vector3 rangePXPY02 = new Vector3(transform.position.x + lavaRange, transform.position.y + lavaRange, 0f);
            Vector3 rangePXMY02 = new Vector3(transform.position.x + lavaRange, transform.position.y - lavaRange, 0f);
            Vector3 rangeMXPY02 = new Vector3(transform.position.x - lavaRange, transform.position.y + lavaRange, 0f);
            Vector3 rangeMXMY02 = new Vector3(transform.position.x - lavaRange, transform.position.y - lavaRange, 0f);
            Instantiate(bossPatterns[2].objPattern, rangePXPY02, Quaternion.identity); //������ ��
            Instantiate(bossPatterns[2].objPattern, rangePXMY02, Quaternion.identity); //������ �Ʒ�
            Instantiate(bossPatterns[2].objPattern, rangeMXPY02, Quaternion.identity); //���� ��
            Instantiate(bossPatterns[2].objPattern, rangeMXMY02, Quaternion.identity); //���� �Ʒ�
        }
    }

    /// <summary>
    /// ���� �ǰ� 0���� �� �� ����� üũ
    /// - ����� �Լ��� ���
    /// </summary>
    public override void VPhaseCheck()
    {
        if (bossPhase != 3)
        {
            bossPhase += 1;
            UsingPattern = false;
            for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++) //������̴� ������ �ʱ�ȭ
            {
                bossPatterns[iNum01].startPattern = false; //������̴� ���� ����
                bossPatterns[iNum01].usedPattern = false; //���� ���� �ʱ�ȭ
                bossPatterns[iNum01].curDelayTime = 0f; //������ �ð� �ʱ�ȭ
                bossPatterns[iNum01].curRepeatNum = 0; //�ݺ� Ƚ�� �ʱ�ȭ
            }
            VSetHP(); //ü�� ����
            PhaseChange(); //������ ��ü
            curGrogTime = setGrogTime;
            isGrog = true;
        }

        else
        {
            VDie(); //�θ� ��ũ��Ʈ�� �Լ� ���
        }
    }

    /// <summary>
    /// ����� �Լ� ���
    /// </summary>
    protected override void VDie()
    {
        base.VDie();
    }

    /// <summary>
    /// ����� �Լ� ���
    /// </summary>
    public override void VSetHP()
    {
        base.VSetHP();
    }
}
