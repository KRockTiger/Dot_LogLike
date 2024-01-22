using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject slotsParent; //슬롯 부모 오브젝트
    [SerializeField] private ItemSlot[] slots; //아이템 슬롯 배열

    private void Start()
    {
        slots = slotsParent.GetComponentsInChildren<ItemSlot>(); //저장되어 있는 슬롯 개수 만큼 생성
    }
}
