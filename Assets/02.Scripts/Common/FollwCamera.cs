using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollwCamera : MonoBehaviour
{
    [SerializeField]private Transform Target; //���� Ÿ�� ��ġ������ �ʿ��ϴ�, ī�޶� ���� Ÿ���� ������ �ƴϱ⿡ ���� ����.
    private Transform CamTr; //ī�޶� ��ġ

    private float height = 5.0f; //ī�޶� ��ġ�� ����
    private float distance = 7.0f; //Ÿ�ٰ� ī�޶��� �Ÿ�
    private float movedamping = 10.0f; //ī�޶��� �̵��� �ε巴�� ��ȭ�ϴ� ��.
    private float rotdamping = 15.0f; //ī�޶��� ȸ���� �ε巴�� ��ȭ�ϴ� ��.
    private float targetOffset = 2.0f; //Ÿ�ٿ����� ī�޶� ���̰�

    private float maxHeight = 12f; //ī�޶� ��ֹ��� �������� �ö� ����.
    private float castOffset = 2.0f; //�÷��̾� �Ӹ� ��ġ ���� �������� ���� ���� ����� ���Ѵ�.
    private float orignHeight;
    void Start()
    {
        CamTr = GetComponent<Transform>();
        orignHeight = height; //���� ī�޶� ��ġ�� ����. (���� �������� �ٽ� �ǵ��ƿ� ��.)
    }
    void Update() //LateUpdate���� ���� ����Ǿ� �̰����� ��ֹ��� �����Ѵ�.
    {
        if (Target == null) return;
        Vector3 castTarget = Target.position + (Target.up * castOffset); //Ÿ��(�÷��̾�)��ġ���� ���̸� 1��ŭ �ø���. 
        Vector3 castDir = (castTarget - CamTr.position).normalized; // ī�޶� �ٶ󺸰� �ִ� ����
        //float castDis = (castTarget - CamTr.position).magnitude; // ī�޶�� �÷��̾� ������ �Ÿ�

        RaycastHit hit;
        if(Physics.Raycast(CamTr.position, castDir, out hit, Mathf.Infinity))
        {//ķ ��ġ����, �÷��̾� �������� ����, ���� �Ÿ� ���Ѵ��� ����ĳ��Ʈ.
            if (!hit.collider.CompareTag("Player")) //�÷��̾ ���� �ʾҴٸ�
            {
                height = Mathf.Lerp(height, maxHeight, Time.deltaTime * 5f); //height���� maxHeight���� ���������Ѵ�.
            }
            else //�÷��̾ �¾Ҵٸ�
            {
                height = Mathf.Lerp(height,orignHeight,Time.deltaTime * 5f); //height���� orignHeight���� ���������Ѵ�.
            }
        }
    }
    void LateUpdate() // update�Լ��� ���� �ȴ��� lateupdate�� ���󰣴�.
    {//CamPos��꿡�� Vector3.forward�� up�� ���� ��ǥ�� ĳ���Ͱ� �����̸� ī�޶� ���� ȸ���� ����. 
        var CamPos = Target.position - (Target.forward * distance) + (Target.up * height); //ķ�� Ÿ���� �Ĺ� ���ʿ� ��ġ.
        CamTr.position = Vector3.Slerp(CamTr.position, CamPos, movedamping * Time.deltaTime);
        //��� ����(���� ķ��ġ���� Campos���� movedamping * ��ŸŸ���� �ӵ��� �ε巴�� �̵�.) ī�޶� �ٶ󺸴� ��ġ ����
        CamTr.rotation = Quaternion.Slerp(CamTr.rotation, Target.rotation, rotdamping * Time.deltaTime);
        //��� ����(���� ���� �����̼ǿ��� Ÿ���� �����̼Ǳ��� rotdamping * ��ŸŸ���� �ӵ��� �ε巴�� �̵�.) �ٶ󺸴� ���� ����
        CamTr.LookAt(Target.position + (Target.up * targetOffset));
        //���� Ÿ���� targetOffset�� ���̸�ŭ ������ ī�޶� �ٶ󺻴�. Ÿ���� �ٸ��� �ƴ� �Ӹ��� �������� �۾�.


    }

    private void OnDrawGizmos() //�� ȭ�鿡�� �����̳� ���� �׷��ִ� �Լ�
    {
        Gizmos.color = new Color(0, 255, 0, 255);
        Gizmos.DrawSphere(Target.position + (Target.up * targetOffset), 0.1f); //ī�޶� �ٶ󺸴� ��ġ�� �˷���
        //Gizmos.DrawLine(Target.position + (Target.up * targetOffset), CamTr.position); //ī�޶� �ٶ󺸴� ��ġ�� ī�޶� �մ� ��
        //�ش� �ڵ忡�� ���� �߻�, LateUpdate���� ī�޶���ġ�� �ʰ� ��ȭ�Ͽ� �߻� �ϴ� ����
    }
}
