using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Item item; //넣을 아이템
    [SerializeField] private Image itemImage; //아이템 이미지
    [SerializeField] private GameObject objHighlight; //하이라이트 오브젝트
    [SerializeField] private GameObject objItemImage; //아이템 이미지 오브젝트

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!objHighlight.activeSelf)
        {
            objHighlight.SetActive(true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (objHighlight.activeSelf)
        {
            objHighlight.SetActive(false);
        }
    }

    private void Update()
    {
        if (itemImage == null)
        {
            objItemImage.SetActive(false); //아이템이 들어있지 않은 경우 이미지 끄기
        }
    }
}
