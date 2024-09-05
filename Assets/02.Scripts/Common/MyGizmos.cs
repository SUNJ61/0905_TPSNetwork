using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public enum Type { NORMAL, WAYPOINT} //WAYPOINT���°��Ǹ� �������� �������� ����
    private const string WAYPOINTFILE = "Enemy"; //Gizmos���Ͽ� �ִ� ���� �ҷ��� string����
    public Type type = Type.NORMAL;

    public Color _color;
    public float _radius;
    void Start()
    {

    }
    private void OnDrawGizmos()
    {
        if (type == Type.NORMAL)
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _radius); //�� ȭ�鿡�� �����̳� ���� �׷��ִ� �Լ�
        }
        else if (type == Type.WAYPOINT)
        {
            Gizmos.color = _color;
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.0f, WAYPOINTFILE, true);
            //��������Ʈ Ÿ���� ������Ʈ �����Ǻ��� y���� 1����, Gizmos���Ͽ� Enemy������ ������ ���� ������ ��� �ҷ��´�.
            //������ ���� ������(ũ��)��� �ҷ����� �־����� �۰Ժ��̰� ������ ũ�Ժ��δ�.
            Gizmos.DrawWireSphere(transform.position,_radius);
        }
    } 
}
