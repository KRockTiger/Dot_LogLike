using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossEnemy : MonoBehaviour
{
    public enum PatternName
    {
        PokePattern,
    }

    [System.Serializable]
    public class BossPattern
    {
        public PatternName patternName; //���� �̸�
        public int patternNum; //���� Ƚ��
        public float delayTime; //���Ͽ� �����̸� �ְ� ���� �� ���
        public bool startPattern = false; //���� ���� ����
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

        if (distance < 0.25f) //�÷��̾���� �Ÿ��� 0.25������ ��������
        {
            if (repeatPattern != bossPatterns[0].patternNum && bossPatterns[0].startPattern) //�������� �� Ƚ���� ������ ���� Ƚ���� ���� �ʰ� ���� ������� ���
            {
                delayTime -= Time.deltaTime; //�ٽ� ���� ������ ������ �ð�
                if (Input.GetKeyDown(KeyCode.O))
                {
                    Debug.Log(delayTime);
                }

                if (delayTime <= 0f) //������ �ð��� ������ �ٽ� ���� �ݺ�
                {
                    rePattern = true;
                }

            }

            else //���� ���� Ƚ���� �ݺ��� ������
            {
                repeatPattern = 0; //���� Ƚ�� �ʱ�ȭ
                bossPatterns[0].startPattern = false; //���� �߻� ����
            }
            return;
        }
        transform.position += dirTarget.normalized * pokeSpeed * Time.deltaTime; //�Է��� ���ݹ������� ���
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
        }
    }
}
