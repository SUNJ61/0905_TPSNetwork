using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
[RequireComponent(typeof(NavMeshAgent))]//�� ������Ʈ���� navmeshagent�� ���� ���ϰ���.
public class EnemyMoveAgent : MonoBehaviourPun
{
    public List<Transform> wayPointList; //�������� ����Ʈ�� ������� �ε���
    private NavMeshAgent agent;
    private Transform enemyTr;

    private readonly string WayPointStr = "WayPointGroup";

    public int nexIdx = 0; //���� �������� ����Ʈ�� �ε��� �� ����
    private int curIdx;

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 3.5f;
    private float damping = 1.0f; //ȸ�� �� ���� �ӵ��� �����Ѵ�.

    private bool _patrolling; // �� ������ �����ϱ� ���� ������Ƽ�� ���. ��, ����ʵ� ��ȣ�� ����.
    public bool patrolling // _patrolling�� �����ϱ� ���� ������Ƽ
    {
        get { return _patrolling; }
        set { 
            _patrolling = value;
                if(_patrolling) //��Ʈ�� ���̶��
                {
                    damping = 1.0f;
                    agent.speed = patrolSpeed; //��Ʈ�� ���ǵ� ����
                    MoveWayPoint(); //���� ����Ʈ�� ��Ʈ��
                }
            }
    }

    private Vector3 _traceTarget; // �� ������ �����ϱ� ���� ������Ƽ�� ���.
    public Vector3 traceTarget //������Ƽ�� �̿��� �Լ� ȣ���� �����ϴ�.
    {
        get { return _traceTarget; }
        set {
            damping = 7.0f;
            _traceTarget = value; //pos���� �� (vector3)
            agent.speed = traceSpeed;
            TraceTarget(_traceTarget);
            }
    }

    public float speed
    {
        get { return agent.velocity.magnitude; } //speed������ agent�ӵ��� ��ȯ�Ѵ�.
    }

    void Start()
    {//Find�迭 �Լ��� ������ �������� ����
        var group = GameObject.Find(WayPointStr); //���̶�Ű�� �ִ� WayPointGroup�� ã�Ƽ� ����
        if (group != null)//��ȿ�� �˻�, group�� �о�Դ��� �˻�
        {
            group.GetComponentsInChildren<Transform>(wayPointList); //WayPointGroup�ȿ� �ڽĵ��� ����Ʈ�� ����
            wayPointList.RemoveAt(0);// WayPointGroup�̶�� ������Ʈ�� 0���� ����ǹǷ� ����
        }

        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false; //��������� ���ӱ�� off
        agent.updateRotation = false; //������Ʈ�� �̿��ؼ� ȸ���ϴ� ����� ��Ȱ��ȭ. (�� ����� �ε巴���� ����.)

        nexIdx = Random.Range(0, wayPointList.Count);
        MoveWayPoint();
    }
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (agent.isStopped == false && !GameManager.G_instance.isGameOver) //�����̴� �� �̶��
            {
                Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity); //AI�� ������ �ϴ� ������ rot����������
                enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
                //���ʹ̰� ���¹����� damping�� �ӵ���ŭ rot�� �������� ȸ����Ų��.
            }
            if (_patrolling == false) return; //��Ʈ�Ѹ��� false�� ������Ʈ �Լ� Ż��.
                                              //float dist = Vector3.Distance(transform.position, wayPointList[nexIdx].position); //�� ������Ʈ ������ �Ÿ�
                                              //float dist = (wayPointList[nexIdx].position - transform.position).magnitude;//�� ������Ʈ ���� �Ÿ�, �Ѵ� if���ǰ� ����
            if (agent.remainingDistance <= 0.5f) //�߰����� �������� �Ÿ��� 0.5���� �۰ų� ���ٸ�, agent�Ÿ� �Ի��� ���� ����
            {
                curIdx = nexIdx;
                nexIdx = Random.Range(0, wayPointList.Count);
                if (nexIdx != curIdx) //���� ��ġ�� �������� �̴� AI����
                    MoveWayPoint();

                //nexIdx = ++nexIdx % wayPointList.Count; //�ε����� ��� �����ϴ��� �������� 0~10���̷� �����Ǿ� 0~10���� �� ����.
                //MoveWayPoint();
            }
        }
    }
    private void MoveWayPoint()
    {
        if (agent.isPathStale) return; //�ִ� ��� ����� ������ �ʰ� ���� �Ҿ���� ��� ������ ����.

        agent.destination = wayPointList[nexIdx].position;//���� ������������ �߰��Ѵ�.
        agent.isStopped = false;
    }
    private void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return; //�ִ� ��� ����� ������ �ʰ� ���� �Ҿ���� ��� ������ ����. 
        agent.destination = pos; // �߰���ġ�� �Է¹��� pos��ġ�� ����
        agent.isStopped = false; //���� ����
    }

    public void Stop() //�߰� ���� �Լ�
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
}
