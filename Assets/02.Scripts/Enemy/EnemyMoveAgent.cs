using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
[RequireComponent(typeof(NavMeshAgent))]//이 오브젝트에서 navmeshagent를 삭제 못하게함.
public class EnemyMoveAgent : MonoBehaviourPun
{
    public List<Transform> wayPointList; //정찰지점 포인트를 담기위한 인덱스
    private NavMeshAgent agent;
    private Transform enemyTr;

    private readonly string WayPointStr = "WayPointGroup";

    public int nexIdx = 0; //다음 정찰지점 포인트의 인덱스 값 저장
    private int curIdx;

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 3.5f;
    private float damping = 1.0f; //회전 할 때의 속도를 조절한다.

    private bool _patrolling; // 이 변수를 은닉하기 위해 프로퍼티를 사용. 즉, 멤버필드 보호의 역할.
    public bool patrolling // _patrolling을 관리하기 위한 프로퍼티
    {
        get { return _patrolling; }
        set { 
            _patrolling = value;
                if(_patrolling) //패트롤 중이라면
                {
                    damping = 1.0f;
                    agent.speed = patrolSpeed; //패트롤 스피드 변경
                    MoveWayPoint(); //다음 포인트로 패트롤
                }
            }
    }

    private Vector3 _traceTarget; // 이 변수를 은닉하기 위해 프로퍼티를 사용.
    public Vector3 traceTarget //프로퍼티를 이용해 함수 호출이 가능하다.
    {
        get { return _traceTarget; }
        set {
            damping = 7.0f;
            _traceTarget = value; //pos값이 들어감 (vector3)
            agent.speed = traceSpeed;
            TraceTarget(_traceTarget);
            }
    }

    public float speed
    {
        get { return agent.velocity.magnitude; } //speed변수에 agent속도를 반환한다.
    }

    void Start()
    {//Find계열 함수는 느려서 시작전에 실행
        var group = GameObject.Find(WayPointStr); //하이라키에 있는 WayPointGroup을 찾아서 대입
        if (group != null)//유효성 검사, group을 읽어왔는지 검사
        {
            group.GetComponentsInChildren<Transform>(wayPointList); //WayPointGroup안에 자식들을 리스트에 저장
            wayPointList.RemoveAt(0);// WayPointGroup이라는 오브젝트가 0번에 저장되므로 삭제
        }

        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false; //가까워지면 감속기능 off
        agent.updateRotation = false; //에이전트를 이용해서 회전하는 기능을 비활성화. (이 기능은 부드럽지가 않음.)

        nexIdx = Random.Range(0, wayPointList.Count);
        MoveWayPoint();
    }
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (agent.isStopped == false && !GameManager.G_instance.isGameOver) //움직이는 중 이라면
            {
                Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity); //AI가 가려고 하는 방향을 rot변수에저장
                enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
                //에너미가 보는방향을 damping의 속도만큼 rot의 방향으로 회전시킨다.
            }
            if (_patrolling == false) return; //패트롤링이 false면 업데이트 함수 탈출.
                                              //float dist = Vector3.Distance(transform.position, wayPointList[nexIdx].position); //두 오브젝트 사이의 거리
                                              //float dist = (wayPointList[nexIdx].position - transform.position).magnitude;//두 오브젝트 사이 거리, 둘다 if조건과 동일
            if (agent.remainingDistance <= 0.5f) //추격중인 지점과의 거리가 0.5보다 작거나 같다면, agent거리 게산이 가장 빠름
            {
                curIdx = nexIdx;
                nexIdx = Random.Range(0, wayPointList.Count);
                if (nexIdx != curIdx) //다음 위치를 랜덤으로 뽑는 AI구조
                    MoveWayPoint();

                //nexIdx = ++nexIdx % wayPointList.Count; //인덱스가 계속 증가하더라도 나머지가 0~10사이로 결정되어 0~10사이 값 대입.
                //MoveWayPoint();
            }
        }
    }
    private void MoveWayPoint()
    {
        if (agent.isPathStale) return; //최단 경로 계산이 끝나지 않고 길을 잃어버린 경우 강제로 종료.

        agent.destination = wayPointList[nexIdx].position;//다음 정찰지점까지 추격한다.
        agent.isStopped = false;
    }
    private void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return; //최단 경로 계산이 끝나지 않고 길을 잃어버린 경우 강제로 종료. 
        agent.destination = pos; // 추격위치를 입력받은 pos위치로 설정
        agent.isStopped = false; //추적 시작
    }

    public void Stop() //추격 중지 함수
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
}
