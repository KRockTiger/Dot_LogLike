using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject slotsParent; //���� �θ� ������Ʈ
    public ItemSlot[] slots; //������ ���� �迭

    private void Start()
    {
        slots = slotsParent.GetComponentsInChildren<ItemSlot>(); //����Ǿ� �ִ� ���� ���� ��ŭ ����
    }

    public void PGetItem(Item _item)
    {
        for (int iNum01 = 0;  iNum01 < slots.Length; iNum01++) //��� ���� Ȯ��
        {
            if (slots[iNum01].PCheckItem() == null) //���� �������� ���������
            {
                slots[iNum01].PAddItem(_item); //������ ���
                return; //����� ������ �����Ͽ� �Լ� ����ϱ�
            }
        }
    }
}
