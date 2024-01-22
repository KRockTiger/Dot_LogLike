using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        attackPostion, //공격력관련 포션
        healthPostion, //체력관련 포션
        artifact, //장비
    }

    [Header("아이템 정보")]
    [SerializeField] private SpriteRenderer itemSprite; //아이템 스프라이트
    [SerializeField] private ItemType itemType; //아이템 종류
    [SerializeField] private string itemName; //아이템 이름
    [SerializeField] private string itemDescription; //아이템 설명
    [SerializeField] private int intValue; //int형 밸류값 ==> 주로 체력관련 사용
    [SerializeField] private float floatValue; //float형 밸류값 ==> 주로 공격력관련 사용

    /// <summary>
    /// 아이템 종류 적용
    /// </summary>
    public ItemType PGetItemType()
    {
       return itemType;
    }

    /// <summary>
    /// int값 가져오는 public 함수
    /// </summary>
    /// <returns></returns>
    public int PGetIntValue()
    {
        return intValue;
    }

    /// <summary>
    /// float값 가져오는 public 함수
    /// </summary>
    /// <returns></returns>
    public float PGetFloatValue()
    {
        return floatValue;
    }
}
