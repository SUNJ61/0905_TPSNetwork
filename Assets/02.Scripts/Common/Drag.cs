using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Transform ItemTr;
    private Transform inventoryTr;
    private Transform ItemList;
    private CanvasGroup canvasGroup;

    private string invenStr = "Inventory";
    private string itemListStr = "ItemList";

    public static GameObject draggingItem = null;
    void Start()
    {
        inventoryTr = GameObject.Find(invenStr).transform; //부모,자식오브젝트가 이벤트가 발생하면 달라지기때문에
        ItemList = GameObject.Find(itemListStr).transform; //Find로 찾는다.
        canvasGroup = GetComponent<CanvasGroup>();
        ItemTr = GetComponent<RectTransform>();
    }
    public void OnDrag(PointerEventData eventData) //드래그 이벤트 (드래그하는 이벤트가 발생하면 실행)
    {
        ItemTr.position = Input.mousePosition; //드래그 이벤트가 발생하면 아이템의 위치를 마우스 커서의 위치로 변경한다.
    }
    
    public void OnBeginDrag(PointerEventData eventData)//드래그를 시작할 때 한번 호출 되는 이벤트
    {
        this.transform.SetParent(inventoryTr); //드래그가 시작되면 드래그 받는 오브젝트의 부모 오브젝트는 인벤토리가 된다.
        draggingItem = this.gameObject; //드래그된 오브젝트를 draggingItem변수에 넣는다.
        canvasGroup.blocksRaycasts = false; //드래그가 시작되었을 때 다른 UI이벤트를 받지 않기 위해
    }

    public void OnEndDrag(PointerEventData eventData)//드래그가 끝났을 때 한번 호출 되는 이벤트
    {
        draggingItem = null; //드래그가 끝났으므로 저장된 오브젝트를 삭제
        canvasGroup.blocksRaycasts = true;
        if(ItemTr.parent == inventoryTr) //드래그한 아이템의 부모가 인벤토리일때, 즉 드래그가 끝났을 때 슬롯에 넣지 못하였을 경우
        {
            ItemTr.SetParent(ItemList);//드래그한 아이템의 부모를 아이템 리스트로 설정한다. 다시 아이템 리스트로 돌아간다.
            GameManager.G_instance.RemoveItem(GetComponent<ItemInfo>().itemData);
            //슬롯에 있는 아이템을 뺏을 경우 게임매니저에있는 RemoveItem함수를 불러온다.
            //RemoveItem함수에는 현재 드래그 중인 아이템의 정보를 itemData에 담아서 전달한다.
            //itemData는 GameData의 Item클래스의 형태의 변수이다.
        }
    }
}
