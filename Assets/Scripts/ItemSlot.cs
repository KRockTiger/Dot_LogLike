using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Player player;
    [SerializeField] private Item item; //넣을 아이템
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private GameObject objHighlight; //하이라이트 오브젝트
    [SerializeField] private GameObject objItemImage; //아이템 이미지 오브젝트

    public void OnPointerEnter(PointerEventData eventData)
    {
        objHighlight.SetActive(true);

        if (item != null)
        {
            itemDescription.gameObject.SetActive(true);
            itemDescription.text = $"{item.itemName}\n{item.itemDescription}";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        objHighlight.SetActive(false);

        if (item != null)
        {
            itemDescription.gameObject.SetActive(false);
            itemDescription.text = string.Empty;
        }
    }

    private void Start()
    {
        itemDescription.gameObject.SetActive(false);
        itemDescription.text = string.Empty;
    }

    private void Update()
    {
        if (item == null)
        {
            objItemImage.SetActive(false); //아이템이 들어있지 않은 경우 이미지 끄기
        }

        else if (item != null && player != null)
        {
            UsingItem();
        }
    }

    /// <summary>
    /// 아이템을 소지하고 있을 경우 적용
    /// </summary>
    private void UsingItem()
    {
        objItemImage.SetActive(true);

        switch (item.valueType)
        {
            case Item.ValueType.damage:
                player.PGetDamage(item.PGetFloatValue());
                break;

            case Item.ValueType.move:
                player.PGetMove(item.PGetFloatValue());
                break;

            case Item.ValueType.maxHP:
                player.PGetMaxHP(item.PGetIntValue());
                break;
        }
    }

    /// <summary>
    /// 아이템 정보 확인하는 함수
    /// </summary>
    /// <returns></returns>
    public Item PCheckItem()
    {
        return item;
    }

    /// <summary>
    /// 아이템 정보 가져오기
    /// </summary>
    /// <param name="_item"></param>
    public void PAddItem(Item _item)
    {
        Image itemImg = objItemImage.GetComponentInChildren<Image>(); //자식 오브젝트의 이미지 컴퍼넌트 가져오기
        item = _item; //아이템 등록
        itemImg.sprite = item.itemSprite; //아이템 이미지 등록
    }
}
