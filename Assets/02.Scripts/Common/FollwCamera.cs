using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollwCamera : MonoBehaviour
{
    [SerializeField]private Transform Target; //따라갈 타겟 위치정보가 필요하다, 카메라가 붙을 타겟이 고정이 아니기에 직접 넣음.
    private Transform CamTr; //카메라 위치

    private float height = 5.0f; //카메라가 위치할 높이
    private float distance = 7.0f; //타겟과 카메라의 거리
    private float movedamping = 10.0f; //카메라의 이동을 부드럽게 완화하는 값.
    private float rotdamping = 15.0f; //카메라의 회전을 부드럽게 완화하는 값.
    private float targetOffset = 2.0f; //타겟에서의 카메라 높이값

    private float maxHeight = 12f; //카메라가 장애물에 가려지면 올라갈 높이.
    private float castOffset = 2.0f; //플레이어 머리 위치 값을 가져오기 위해 높이 배수를 곱한다.
    private float orignHeight;
    void Start()
    {
        CamTr = GetComponent<Transform>();
        orignHeight = height; //원래 카메라가 위치한 높이. (높이 변경이후 다시 되돌아올 값.)
    }
    void Update() //LateUpdate보다 먼저 실행되어 이곳에서 장애물을 감지한다.
    {
        if (Target == null) return;
        Vector3 castTarget = Target.position + (Target.up * castOffset); //타겟(플레이어)위치에서 높이를 1만큼 올린다. 
        Vector3 castDir = (castTarget - CamTr.position).normalized; // 카메라가 바라보고 있는 방향
        //float castDis = (castTarget - CamTr.position).magnitude; // 카메라와 플레이어 사이의 거리

        RaycastHit hit;
        if(Physics.Raycast(CamTr.position, castDir, out hit, Mathf.Infinity))
        {//캠 위치에서, 플레이어 방향으로 감지, 측정 거리 무한대의 레이캐스트.
            if (!hit.collider.CompareTag("Player")) //플레이어가 맞지 않았다면
            {
                height = Mathf.Lerp(height, maxHeight, Time.deltaTime * 5f); //height부터 maxHeight까지 선형보간한다.
            }
            else //플레이어가 맞았다면
            {
                height = Mathf.Lerp(height,orignHeight,Time.deltaTime * 5f); //height부터 orignHeight까지 선형보간한다.
            }
        }
    }
    void LateUpdate() // update함수가 먼저 된다음 lateupdate로 따라간다.
    {//CamPos계산에서 Vector3.forward와 up은 절대 좌표라 캐릭터가 움직이면 카메라가 같이 회전을 안함. 
        var CamPos = Target.position - (Target.forward * distance) + (Target.up * height); //캠은 타겟의 후방 위쪽에 위치.
        CamTr.position = Vector3.Slerp(CamTr.position, CamPos, movedamping * Time.deltaTime);
        //곡면 보간(원래 캠위치에서 Campos까지 movedamping * 델타타임의 속도로 부드럽게 이동.) 카메라가 바라보는 위치 조절
        CamTr.rotation = Quaternion.Slerp(CamTr.rotation, Target.rotation, rotdamping * Time.deltaTime);
        //곡면 보간(원래 본인 로테이션에서 타겟의 로테이션까지 rotdamping * 델타타임의 속도로 부드럽게 이동.) 바라보는 각도 조절
        CamTr.LookAt(Target.position + (Target.up * targetOffset));
        //기존 타겟의 targetOffset의 높이만큼 위쪽을 카메라가 바라본다. 타겟의 다리가 아닌 머리를 보기위한 작업.


    }

    private void OnDrawGizmos() //씬 화면에서 색상이나 선을 그려주는 함수
    {
        Gizmos.color = new Color(0, 255, 0, 255);
        Gizmos.DrawSphere(Target.position + (Target.up * targetOffset), 0.1f); //카메라가 바라보는 위치를 알려줌
        //Gizmos.DrawLine(Target.position + (Target.up * targetOffset), CamTr.position); //카메라가 바라보는 위치와 카메라를 잇는 선
        //해당 코드에서 오류 발생, LateUpdate에서 카메라위치가 늦게 변화하여 발생 하는 오류
    }
}
