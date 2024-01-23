using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        attackPostion, //���ݷ°��� ����
        healthPostion, //ü�°��� ����
        artifact, //���
    }

    public enum ValueType
    {
        damage,
        move,
        hp,
        maxHP,
    }

    [Header("������ ����")]
    public ItemType itemType; //������ ����
    public ValueType valueType;
    public Sprite itemSprite;
    public string itemName; //������ �̸�
    public string itemDescription; //������ ����
    public int intValue; //int�� ����� ==> �ַ� ü�°��� ���
    public float floatValue; //float�� ����� ==> �ַ� ���ݷ°��� ���
    public bool isHave = false; //������ �������� ��� ���� ���� �Ǵ�

    /// <summary>
    /// ������ ���� ����
    /// </summary>
    public ItemType PGetItemType()
    {
       return itemType;
    }

    /// <summary>
    /// int�� �������� public �Լ�
    /// </summary>
    /// <returns></returns>
    public int PGetIntValue()
    {
        return intValue;
    }

    /// <summary>
    /// float�� �������� public �Լ�
    /// </summary>
    /// <returns></returns>
    public float PGetFloatValue()
    {
        return floatValue;
    }
}
