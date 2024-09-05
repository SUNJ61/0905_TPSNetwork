using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public enum Type { NORMAL, WAYPOINT} //WAYPOINT상태가되면 아이콘이 나오도록 설계
    private const string WAYPOINTFILE = "Enemy"; //Gizmos파일에 있는 사진 불러올 string변수
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
            Gizmos.DrawSphere(transform.position, _radius); //씬 화면에서 색상이나 선을 그려주는 함수
        }
        else if (type == Type.WAYPOINT)
        {
            Gizmos.color = _color;
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.0f, WAYPOINTFILE, true);
            //웨이포인트 타입의 오브젝트 포지션보다 y축이 1위에, Gizmos파일에 Enemy파일을 파일이 가진 스케일 대로 불러온다.
            //파일이 가진 스케일(크기)대로 불러오면 멀어지면 작게보이고 가까우면 크게보인다.
            Gizmos.DrawWireSphere(transform.position,_radius);
        }
    } 
}
