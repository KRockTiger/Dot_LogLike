using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject slotsParent; //슬롯 부모 오브젝트
    public ItemSlot[] slots; //아이템 슬롯 배열

    private void Start()
    {
        slots = slotsParent.GetComponentsInChildren<ItemSlot>(); //저장되어 있는 슬롯 개수 만큼 생성
    }

    public void PGetItem(Item _item)
    {
        for (int iNum01 = 0;  iNum01 < slots.Length; iNum01++) //모든 슬롯 확인
        {
            if (slots[iNum01].PCheckItem() == null) //만약 아이템이 비어있으면
            {
                slots[iNum01].PAddItem(_item); //아이템 등록
                return; //등록이 됬으면 리턴하여 함수 취소하기
            }
        }
    }
}
