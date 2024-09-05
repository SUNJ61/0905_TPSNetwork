using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(EnemyFOV))] //EnemyFOV��ũ��Ʈ�� �����Ѵ�
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyFOV fov = (EnemyFOV)target; // EnemyFOVŬ������ ���¸� ���� fov������ ������Ƽ�� ���� ����ȴ�.
        //���� ���� �������� ��ǥ�� ���(�־��� ������ 1/2�� �ȴ�)
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f); //EnemyFOV�� viewAngle�� EnemyFOV�� CirclePoint�Լ��� ����.

        Handles.color = Color.white; //���� ������ ������� ����
        Handles.DrawWireDisc(fov.transform.position, //������ �����ϴ� ���� ��ǥ�̴�. fov ��ũ��Ʈ�� ���� ���ʹ��� ��ġ�� ����.
            Vector3.up, //��� ����
            fov.viewRange); //�������� �ǹ��Ѵ�. �ܰ����� ǥ���ϴ� ������ �׸���.

        Handles.color = new Color(1,1,1,0.2f); //���, �����ϰ� ����.
        Handles.DrawSolidArc(fov.transform.position,//������ �����ϴ� ������ǥ�̴�. fov��ũ��Ʈ�� ���� ���ʹ��� ��ġ�� ����.
            Vector3.up, //��� ���� (y�� �������� ������.)
            fromAnglePos, //fov.transform.position�� ���� �߽��̰� fromAnglePos�� ���ʹ̰� ���� ������ ������ ���� �߽���ǥ�̴�.
            fov.viewAngle, //��ä���� ����
            fov.viewRange); //��ä���� ������

        Handles.Label(fov.transform.position + (fov.transform.forward * 2.0f), fov.viewAngle.ToString());
        //�þ߰��� �ؽ�Ʈ�� ǥ�� (���ʹ� ��ġ���� 2.0f��ŭ ���ʿ� ����, ��ä���� ������ �ؽ�Ʈ�� ǥ��)
    }
}
