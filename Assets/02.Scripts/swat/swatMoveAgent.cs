using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class swatMoveAgent : MonoBehaviour
{
    public List<Transform> wayPointList;
    private NavMeshAgent agent;
    private Transform S_Tr;

    private readonly string WayPointStr = "WayPointGroup";

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 3.5f;
    private float damping = 1.0f;

    public int nextIdx = 0;
    private int curIdx;

    private bool Patoling;
    public bool _Patoling
    {
        get { return Patoling; }
        set 
        {
            Patoling = value;
            if (Patoling)
            {
                agent.speed = patrolSpeed;
                damping = 1.0f;
                MoveWaypoint();
            }
        }
    }

    private Vector3 traceTarget;
    public Vector3 _traceTarget
    {
        get { return traceTarget; }
        set
        {
            traceTarget = value;
            damping = 7.0f;
            agent.speed = traceSpeed;
            TraceTarget(traceTarget);
        }
    }
    public float Speed
    {
        get { return agent.velocity.magnitude; }
    }
    void Start()
    {
        var group = GameObject.Find(WayPointStr);
        if (group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPointList);
            wayPointList.RemoveAt(0);
        }
        agent = GetComponent<NavMeshAgent>();
        S_Tr = GetComponent<Transform>();
        agent.autoBraking = false;
        agent.updateRotation = false;

        nextIdx = Random.Range(0, wayPointList.Count);
        MoveWaypoint();
    }
    void Update()
    {
        if(!agent.isStopped && !GameManager.G_instance.isGameOver)
        {
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            S_Tr.rotation = Quaternion.Slerp(S_Tr.rotation, rot, damping * Time.time);
        }
        if (!Patoling) return;
        if (agent.remainingDistance <= 0.5f)
        {
            curIdx = nextIdx;
            nextIdx = Random.Range(0, wayPointList.Count);
            if (nextIdx != curIdx) //다음 위치를 랜덤으로 뽑는 AI구조
                MoveWaypoint();

            //nextIdx = ++nextIdx % wayPointList.Count; //순서대로 위치를 이동하는 구조
            //MoveWaypoint();
        }
    }
    private void MoveWaypoint()
    {
        if(agent.isPathStale) return;

        agent.destination = wayPointList[nextIdx].position;
        agent.isStopped = false;
    }
    private void TraceTarget(Vector3 pos)
    {
        if(agent.isPathStale) return;

        agent.destination = pos;
        agent.isStopped = false;
    }
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        Patoling = false;
    }
}
