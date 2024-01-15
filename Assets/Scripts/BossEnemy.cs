using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    private int randLava = 0; //��� ������ 2������ ������ �������� �ϳ��� ���� ��Ű�� ���� Random.Range�� ����

    [Header("���� ����")]
    [SerializeField] private float pokeSpeed = 20f; //��� ������ �̵� �ӵ�

    //private int bossPhase = 1;
    private Vector3 posTarget; //�÷��̾��� ��ġ�� Ÿ������ ���� ���� ����
    private Vector3 dirTarget; //�� ��ġ���� �÷��̾� ��ġ������ ������ ���� ���� ����
    private Vector3 startPoint; //������ ��� ������ �� ��� �����ϴ� ��ġ�� �����ϴ� ��
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) //�׽�Ʈ ����
        {
            CheckPattern();
        }

        if (!UsingPattern) //���� ��� ���� �ƴϰ� ü���� ������ �ƴҰ��
        {
            NextPattern(); //���� �ٲٱ�
        }

        PatternManager(PatternName.PokePattern, bossPatterns[0].startPattern);
        PatternManager(PatternName.MeteorPattern, bossPatterns[1].startPattern);
        PatternManager(PatternName.LavaPattern, bossPatterns[2].startPattern);
        PatternManager(PatternName.LinePattern, bossPatterns[3].startPattern);
    }

    /// <summary>
    /// ���� �ѱ�� �Լ�
    /// </summary>
    private void NextPattern()
    {
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
        //if (UsingPattern) //������ ������̶�� ����
        //{
        //    return;
        //}

        for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++) //���� ��� Ž��
        {
            if (!bossPatterns[iNum01].usedPattern) //���� ��� ���� ������ ������
            {
                ChangePattern(); //���� ��ȭ ��Ų�� �ߵ�

                return;
            }
        }

        ResetPattern(); //��� ������ ���Ǿ����� �׷α� �� �ʱ�ȭ�� ���� ���
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

        Debug.Log(randNum);
        bossPatterns[randNum].startPattern = true; //���� �ߵ�
        UsingPattern = true; //���� ��� ���
    }

    private void ResetPattern()
    {
        for (int iNum01 = 0; iNum01 < bossPatterns.Count; iNum01++)
        {
            bossPatterns[iNum01].usedPattern = false; //���� �ʱ�ȭ
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
        startPoint = transform.position; //���� ��ġ
        dirTarget = posTarget - startPoint; //���� ����
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
        //transform.position += dirTarget.normalized * pokeSpeed * Time.deltaTime; //�Է��� ���ݹ������� ���
        transform.position = Vector3.MoveTowards(transform.position, posTarget, Time.deltaTime * pokeSpeed);
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

        for (int iNum01 = 0; iNum01 < 3; iNum01++)
        {
            float xVector = Random.Range(minVector.x, maxVector.x); //x��ǥ ���� ����
            float yVector = Random.Range(minVector.y, maxVector.y); //y��ǥ ���� ����
            Vector3 spawnField = new Vector3(xVector, yVector, 0); //��ȯ ��ǥ ����

            GameObject obj = Instantiate(bossPatterns[3].objPattern, spawnField, Quaternion.identity);

            Destroy(obj, 5f);
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// - �Ű������� Enum���� ����Ͽ� �˸��� ������ switch���� ����
    /// </summary>
    private void PatternManager(PatternName _pattern, bool _startPattern = false) //���� �̸��� ������ ���ÿ� false�� �ֱ�
    {
        switch (_pattern)
        {
            case PatternName.PokePattern: //��� ���� -------------------------------------------------------------------------------------------------
                if (Input.GetKeyDown(KeyCode.Alpha1)) //���� �ߵ� Ʈ����
                {
                    bossPatterns[0].startPattern = true;
                    bossPatterns[0].rePattern = true;
                }

                if (bossPatterns[0].rePattern && _startPattern) //���� �����ϰų� ������ ���
                {
                    bossPatterns[0].curRepeatNum += 1; //���� Ƚ�� 1����
                    bossPatterns[0].curDelayTime = bossPatterns[0].setDelayTime;
                    posSearch = true;
                    bossPatterns[0].rePattern = false;
                }

                if (_startPattern)
                {
                    PokePlayer();
                }
                break;

            case PatternName.MeteorPattern: //���׿� ���� --------------------------------------------------------------------------------------------------
                if (Input.GetKeyDown(KeyCode.Alpha2)) //���� �ߵ� Ʈ����
                {
                    bossPatterns[1].startPattern = true;
                    bossPatterns[1].curDelayTime = bossPatterns[1].setDelayTime;
                }

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
                if (Input.GetKeyDown(KeyCode.Alpha3)) //���� �ߵ� Ʈ����
                {
                    bossPatterns[2].startPattern = true;
                    randLava = Random.Range(0, 2); //�� ������ ��� ���� �� �ϳ��� ����
                }

                if (bossPatterns[2].curRepeatNum != bossPatterns[2].setRepeatNum && bossPatterns[2].startPattern) //�������� �� Ƚ���� ������ ���� Ƚ���� ���� �ʰ� ���� ������� ���
                {
                    bossPatterns[2].curDelayTime -= Time.deltaTime; //�ٽ� ���� ������ ������ �ð�

                    if (bossPatterns[2].curDelayTime <= 0f) //������ �ð��� ������ �ٽ� ���� �ݺ�
                    {
                        randLava = Random.Range(0, 2); //�� ������ ��� ���� �� �ϳ��� ����

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
                    UsingPattern = false; //������ �������� �ǹ�
                }
                break;

            case PatternName.LinePattern: //�� ���� ���� --------------------------------------------------------------------------------------------
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    bossPatterns[3].startPattern = true;
                }
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
}
