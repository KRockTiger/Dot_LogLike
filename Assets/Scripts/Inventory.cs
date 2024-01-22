using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject slotsParent; //���� �θ� ������Ʈ
    [SerializeField] private ItemSlot[] slots; //������ ���� �迭

    private void Start()
    {
        slots = slotsParent.GetComponentsInChildren<ItemSlot>(); //����Ǿ� �ִ� ���� ���� ��ŭ ����
    }
}
