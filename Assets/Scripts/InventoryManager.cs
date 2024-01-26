using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; //싱글톤으로 설정하여 스테이지 넘어갈 때 아이템을 유지 시킬수 있게 해줌

    [Header("인벤토리 구성")]
    [SerializeField] private GameObject inventory; //인벤토리 오브젝트
    [SerializeField] private GameObject slotsParent; //슬롯 부모 오브젝트
    [SerializeField] private ItemSlot[] slots; //아이템 슬롯 배열, 시리얼라이즈는 확인용으로 쓰기 때문에 빼도 상관 없음

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
        slots = slotsParent.GetComponentsInChildren<ItemSlot>(); //저장되어 있는 슬롯 개수 만큼 생성
        isInven = false;
    }

    /// <summary>
    /// 슬롯에 아이템을 등록하는 함수
    /// </summary>
    /// <param name="_item"></param>
    public void PGetItem(Item _item)
    {
        for (int iNum01 = 0; iNum01 < slots.Length; iNum01++) //모든 슬롯 확인
        {
            if (slots[iNum01].PCheckItem() == null) //만약 아이템이 비어있으면
            {
                slots[iNum01].PAddItem(_item); //아이템 등록
                return; //등록이 됬으면 리턴하여 함수 취소하기
            }
        }
    }
}
