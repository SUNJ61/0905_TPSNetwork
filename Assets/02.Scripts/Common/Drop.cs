using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;

public class Drop : MonoBehaviour, IDropHandler
{
    void Start()
    {

    }
    public void OnDrop(PointerEventData eventData) //오브젝트를 드랍했다면
    {
        if(transform.childCount == 0) //슬롯 밑에 자식 오브젝트가 없으면
            Drag.draggingItem.transform.SetParent(transform, false); //드래그된 아이템 오브젝트의 부모를 슬롯으로 바꾼다.
                                                                     //이때 아이템 오브젝트의 절대 좌표는 끈다.

        Item item = Drag.draggingItem.GetComponent<ItemInfo>().itemData;
        //드래그 되고 있는 오브젝트의 ItemInfo의 itemData를 대입 itemData는 GameData의 Item클래스를 담고있다.
        //Item클래스 형태의 변수 item에 위에서 얻은 정보를 대입.
        GameManager.G_instance.AddItem(item);
    }
}
