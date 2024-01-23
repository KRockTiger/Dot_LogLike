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

    public enum ValueType
    {
        damage,
        move,
        hp,
        maxHP,
    }

    [Header("아이템 정보")]
    public ItemType itemType; //아이템 종류
    public ValueType valueType;
    public Sprite itemSprite;
    public string itemName; //아이템 이름
    public string itemDescription; //아이템 설명
    public int intValue; //int형 밸류값 ==> 주로 체력관련 사용
    public float floatValue; //float형 밸류값 ==> 주로 공격력관련 사용
    public bool isHave = false; //유물형 아이템일 경우 소지 유무 판단

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
