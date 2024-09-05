using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDirction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Transform ItemListTr;
    private Transform ItemTr;

    private Vector2 startPosition;

    public bool isDown;

    void Start()
    {
        ItemListTr = GameObject.Find("ItemList").transform;
        ItemTr = GetComponent<Transform>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = Input.mousePosition; //마우스의 위치값 저장
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 endPosition = Input.mousePosition; //마우스의 위치값 저장
        Vector2 direction = endPosition - startPosition; //끝지점 벡터에서 시작지점 벡터를 뺌

        // 아래쪽으로 드래그했는지 판별
        if (direction.y < 0) //뺀 y값이 0보다 작으면 아래쪽으로 드래그 됬음.
        {
            Debug.Log("아래쪽으로 드래그했습니다.");
            isDown = true;
        }
        else //뺀 y값이 0보다 크면 위쪽으로 드래그 됬음.
        {
            Debug.Log("다른 방향으로 드래그했습니다.");
            isDown = false;
        }
    }
}
