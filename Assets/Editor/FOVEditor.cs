using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(EnemyFOV))] //EnemyFOV스크립트를 참조한다
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyFOV fov = (EnemyFOV)target; // EnemyFOV클래스의 형태를 가진 fov변수는 프로퍼티에 의해 변경된다.
        //원주 위의 시작점의 좌표를 계산(주어진 각도의 1/2이 된다)
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f); //EnemyFOV의 viewAngle을 EnemyFOV의 CirclePoint함수에 대입.

        Handles.color = Color.white; //원의 색상은 흰색으로 지정
        Handles.DrawWireDisc(fov.transform.position, //도형이 시작하는 원점 좌표이다. fov 스크립트가 속한 에너미의 위치를 받음.
            Vector3.up, //노멀 벡터
            fov.viewRange); //반지름을 의미한다. 외곽선만 표현하는 원반을 그린다.

        Handles.color = new Color(1,1,1,0.2f); //흰색, 투명하게 만듬.
        Handles.DrawSolidArc(fov.transform.position,//도형이 시작하는 원점좌표이다. fov스크립트가 속한 에너미의 위치를 받음.
            Vector3.up, //노멀 벡터 (y축 방향으로 움직임.)
            fromAnglePos, //fov.transform.position는 원의 중심이고 fromAnglePos는 에너미가 보는 방향을 포함한 원의 중심좌표이다.
            fov.viewAngle, //부채꼴의 각도
            fov.viewRange); //부채꼴의 반지름

        Handles.Label(fov.transform.position + (fov.transform.forward * 2.0f), fov.viewAngle.ToString());
        //시야각의 텍스트를 표시 (에너미 위치에서 2.0f만큼 앞쪽에 생성, 부채꼴의 각도를 텍스트로 표시)
    }
}
