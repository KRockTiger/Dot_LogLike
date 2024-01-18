using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemOption
    {
        attackPostion, //공격력관련 포션
        healthPostion, //체력관련 포션
        equip, //장비
    }

    [Header("아이템 정보")]
    [SerializeField] private Image itemImage; //아이템 이미지
    [SerializeField] private ItemOption itemOption; //아이템 종류
    [SerializeField] private string itemName; //아이템 이름
    [SerializeField] private string itemDescription; //아이템 설명
    [SerializeField] private int intValue; //int형 밸류값 ==> 주로 체력관련 사용
    [SerializeField] private float floatValue; //float형 밸류값 ==> 주로 공격력관련 사용

    /// <summary>
    /// 아이템을 획득할 시 적용
    /// </summary>
    public void PGetItem()
    {
        switch (itemOption) //아이템 종류에 맞게 사용
        {
            case ItemOption.attackPostion:
                break;

            case ItemOption.healthPostion:
                break;

            case ItemOption.equip:
                break;
        }
    }
}
