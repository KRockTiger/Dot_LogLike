using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; //�̱������� �����Ͽ� �������� �Ѿ �� �������� ���� ��ų�� �ְ� ����

    [Header("�κ��丮 ����")]
    [SerializeField] private GameObject inventory; //�κ��丮 ������Ʈ
    [SerializeField] private GameObject slotsParent; //���� �θ� ������Ʈ
    [SerializeField] private ItemSlot[] slots; //������ ���� �迭, �ø��������� Ȯ�ο����� ���� ������ ���� ��� ����

    private bool isInven;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        slots = slotsParent.GetComponentsInChildren<ItemSlot>(); //����Ǿ� �ִ� ���� ���� ��ŭ ����
        isInven = false;
    }

    /// <summary>
    /// ���Կ� �������� ����ϴ� �Լ�
    /// </summary>
    /// <param name="_item"></param>
    public void PGetItem(Item _item)
    {
        for (int iNum01 = 0; iNum01 < slots.Length; iNum01++) //��� ���� Ȯ��
        {
            if (slots[iNum01].PCheckItem() == null) //���� �������� ���������
            {
                slots[iNum01].PAddItem(_item); //������ ���
                return; //����� ������ �����Ͽ� �Լ� ����ϱ�
            }
        }
    }
}
