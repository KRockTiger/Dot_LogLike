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
    }

    [System.Serializable]
    public class BossPattern
    {
        public GameObject objPattern;
        public PatternName patternName; //���� �̸�
        public int patternNum; //���� Ƚ��
        public float delayTime; //���Ͽ� �����̸� �ְ� ���� �� ���
        public bool startPattern = false; //���� ���� ����
        public bool usedPattern = false; //���� ��� ����
    }

    [Header("���� ����")]
    [SerializeField] private List<BossPattern> bossPatterns;

    [Header("���� ����")]
    [SerializeField] private float pokeSpeed = 20f;

    //private int bossPhase = 1;
    private Vector3 posTarget; //�÷��̾��� ��ġ�� Ÿ������ ���� ���� ����
    private Vector3 dirTarget; //�� ��ġ���� �÷��̾� ��ġ������ ������ ���� ���� ����
    private Vector3 startPoint; //������ ��� ������ �� ��� �����ϴ� ��ġ�� �����ϴ� ��
    [SerializeField] private float delayTime; //���� ������ �ð�
    [SerializeField] private int repeatPattern = 1; //������ �� �� �������� ����ߴ��� Ȯ��
    [SerializeField] private bool rePattern; //���� ��� ���ϰ�� �Ϻ� ����� ���� �ϱ� ���� bool�� ����
    [SerializeField] private bool posSearch; //��ġ�� �� ���� ã�� ���� �����
    
    private void Update()
    {
        PatternManager(PatternName.PokePattern, bossPatterns[0].startPattern);
        //PatternManager(PatternName.MeteorPattern, bossPatterns[1].startPattern);
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
    /// ������ ��ȭ ��Ű�� ���� �Լ�
    /// - �ϳ��� ������ ������ ���� ������ ����ؾ��ϸ� ���� ���� ����ϱ��� �����̰� �ʿ�
    /// </summary>
    private void ChangePattern()
    {
        int RandPattern = Random.Range(0, bossPatterns.Count);
        

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

        if (distance < 0.25f) //�÷��̾���� �Ÿ��� 0.25������ ��������
        {
            if (repeatPattern != bossPatterns[0].patternNum && bossPatterns[0].startPattern) //�������� �� Ƚ���� ������ ���� Ƚ���� ���� �ʰ� ���� ������� ���
            {
                delayTime -= Time.deltaTime; //�ٽ� ���� ������ ������ �ð�               

                if (delayTime <= 0f) //������ �ð��� ������ �ٽ� ���� �ݺ�
                {
                    rePattern = true;
                }

            }

            else //���� ���� Ƚ���� �ݺ��� ������
            {
                repeatPattern = 0; //���� Ƚ�� �ʱ�ȭ
                bossPatterns[0].startPattern = false; //���� �߻� ����
                bossPatterns[0].usedPattern = true; //���� ��� ���� ���
            }
            return;
        }
        transform.position += dirTarget.normalized * pokeSpeed * Time.deltaTime; //�Է��� ���ݹ������� ���
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
    /// ���� ���� ����
    /// - �Ű������� Enum���� ����Ͽ� �˸��� ������ switch���� ����
    /// </summary>
    private void PatternManager(PatternName _pattern, bool _startPattern = false) //���� �̸��� ������ ���ÿ� false�� �ֱ�
    {
        switch (_pattern)
        {
            case PatternName.PokePattern:
                if (Input.GetKeyDown(KeyCode.T))
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
                if (Input.GetKeyDown(KeyCode.R))
                {
                    bossPatterns[1].startPattern = true;
                    rePattern = true;
                    delayTime = bossPatterns[1].delayTime;
                }

                //if (rePattern && _startPattern)
                //{
                //}

                if (repeatPattern != bossPatterns[1].patternNum && bossPatterns[1].startPattern) //�������� �� Ƚ���� ������ ���� Ƚ���� ���� �ʰ� ���� ������� ���
                {
                    delayTime -= Time.deltaTime; //�ٽ� ���� ������ ������ �ð�

                    if (delayTime <= 0f) //������ �ð��� ������ �ٽ� ���� �ݺ�
                    {
                        rePattern = true;
                        MeteorInstantiate();
                        delayTime = bossPatterns[1].delayTime;
                        repeatPattern += 1;
                    }

                }
                else if (repeatPattern == bossPatterns[1].patternNum) //������ �� ������� ���
                {
                    repeatPattern = 0; //���� Ƚ�� �ʱ�ȭ
                    bossPatterns[1].startPattern = false; //���� �߻� ����
                    bossPatterns[1].usedPattern = true;
                }
                break;
        }
    }

}
