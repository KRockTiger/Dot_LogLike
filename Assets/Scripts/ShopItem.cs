using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager gameManager;
    InventoryManager inventoryManager;

    [SerializeField] private Item item;
    [SerializeField] private int price;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemText;
    [SerializeField] private GameObject framImageObj;
    [SerializeField] private Player player;
    private bool isSell = false; //팔림 여부
    private Color sellColor = new Color(1, 1, 1, 0.5f);
    private Color onColor = new Color(1, 1, 0, 1);
    private Color offColor = new Color(1, 1, 1, 1);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSell)
        {
            if (gameManager.PCoin() >= price)
            {
                Image frameImg = GetComponentInChildren<Image>();
                frameImg.color = offColor;
                gameManager.PUseCoin(price); //코인 사용
                isSell = true; //팔림 여부 등록
                itemImage.color = sellColor;
                itemText.color = sellColor;

                if (item.itemType == Item.ItemType.artifact)
                {
                    inventoryManager.PGetItem(item);
                }

                else if (item.itemType != Item.ItemType.artifact)
                {
                    player.PGetHP(item.PGetIntValue());
                }
            }

            else if (gameManager.PCoin() < price)
            {
                Debug.Log("가지고 있는 금액이 부족합니다.");
            }
        }

        else
        {
            Debug.Log("이미 구매한 아이템입니다.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSell)
        {
            Image frameImg = framImageObj.GetComponentInChildren<Image>();
            frameImg.color = onColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSell)
        {
            Image frameImg = framImageObj.GetComponentInChildren<Image>();
            frameImg.color = offColor;
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        inventoryManager = InventoryManager.Instance;
    }

    
}
