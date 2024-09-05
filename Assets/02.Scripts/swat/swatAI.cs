using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class swatAI : MonoBehaviourPun, IPunObservable
{
    private Transform S_tr;
    //private Transform P_tr; //싱글용
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider CapCol;

    private WaitForSeconds Ws;
    private swatMoveAgent moveAgent;
    private swatFire swatfire;
    private swatFOV swat_FOV;

    //private readonly string P_tag = "Player"; //싱글용
    private readonly string s_tag = "SWAT";
    private readonly string u_tag = "Untagged";
    private readonly string hashIsMove = "IsMove";
    private readonly string hashMoveSpeed = "MoveSpeed";
    private readonly int hashDie = Animator.StringToHash("DieIdx");
    private readonly int hasDieTrigger = Animator.StringToHash("DieTrigger");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDieTrigger");

    private float ATK_dist = 5.0f;
    private float TRA_dist = 10.0f;
    public bool isDie = false;

    public enum S_State
    {
        PATOL = 0, TRACE, ATTACK, DIE, EXPDIE
    }
    public S_State state = S_State.PATOL;
    public S_State remote_State = S_State.PATOL;

    void Awake()
    {
        //var PlayerObj = GameObject.FindWithTag(P_tag); //네트워크에서는 해당 코드 사용불가
        //if (PlayerObj != null)
        //    P_tr = PlayerObj.transform;

        S_tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        CapCol = GetComponent<CapsuleCollider>();

        Ws = new WaitForSeconds(0.3f);

        moveAgent = GetComponent<swatMoveAgent>();
        swatfire =  GetComponent<swatFire>();
        swat_FOV = GetComponent<swatFOV>();
    }
    private void OnEnable()
    {
        Damage.OnPlayerDie += OnPlayerDie;
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 2.0f));

        if (PhotonNetwork.IsMasterClient) //마스터 클라이언트에서만 계산, 리모트클라이어트에 데이터 전달 필요.
        {
            StartCoroutine(StateCheck());
        }
        StartCoroutine(PlayAction());
    }
    private void OnDisable()
    {
        Damage.OnPlayerDie -= OnPlayerDie;
    }
    IEnumerator StateCheck()
    {
        yield return new WaitForSeconds(2.0f);

        while (!isDie)
        {
            if(state == S_State.DIE || state == S_State.EXPDIE) yield break;
            //float dist = (P_tr.position - S_tr.position).magnitude; //싱글
            if (swat_FOV.PlayerTr != null)
            {
                float dist = (swat_FOV.PlayerTr.transform.position - S_tr.position).magnitude; //멀티
                if (dist <= ATK_dist)
                    state = S_State.ATTACK;
                else if (dist <= TRA_dist)
                    state = S_State.TRACE;
                else
                    state = S_State.PATOL;
            }

            yield return Ws;
        }
    }
    IEnumerator PlayAction()
    {
        while(!isDie)
        {
            yield return Ws;

            if (PhotonNetwork.IsMasterClient)
            {
                switch (state)
                {
                    case S_State.PATOL:
                        animator.SetBool(hashIsMove, true);
                        moveAgent._Patoling = true;
                        swatfire.isFire = false;
                        break;

                    case S_State.ATTACK:
                        animator.SetBool(hashIsMove, false);
                        swatfire.isFire = true;
                        moveAgent.Stop();
                        break;

                    case S_State.TRACE:
                        animator.SetBool(hashIsMove, true);
                        //moveAgent._traceTarget = P_tr.position; //싱글
                        moveAgent._traceTarget = swat_FOV.PlayerTr.transform.position; //멀티
                        swatfire.isFire = false;
                        break;

                    case S_State.DIE:
                        S_Diestate();
                        break;

                    case S_State.EXPDIE:
                        S_ExpDiestate();
                        break;
                }
            }

            else
            {
                switch (remote_State)
                {
                    case S_State.PATOL:
                        animator.SetBool(hashIsMove, true);
                        moveAgent._Patoling = true;
                        swatfire.isFire = false;
                        break;

                    case S_State.ATTACK:
                        animator.SetBool(hashIsMove, false);
                        swatfire.isFire = true;
                        moveAgent.Stop();
                        break;

                    case S_State.TRACE:
                        animator.SetBool(hashIsMove, true);
                        //moveAgent._traceTarget = P_tr.position; //싱글
                        moveAgent._traceTarget = swat_FOV.PlayerTr.transform.position; //멀티
                        swatfire.isFire = false;
                        break;

                    case S_State.DIE:
                        S_Diestate();
                        break;

                    case S_State.EXPDIE:
                        S_ExpDiestate();
                        break;
                }
            }
        }

    }

    private void S_Diestate()
    {
        if (isDie) return;
        isDie = true;
        moveAgent.Stop();
        swatfire.isFire = false;
        animator.SetTrigger(hasDieTrigger);
        animator.SetInteger(hashDie, Random.Range(0, 3));
        rb.isKinematic = true;
        CapCol.enabled = false;
        gameObject.tag = u_tag;
        StartCoroutine(ObjPoolPush());
    }
    private void S_ExpDiestate()
    {
        if (isDie) return;
        isDie = true;
        moveAgent.Stop();
        swatfire.isFire = false;
        animator.SetTrigger(hasDieTrigger);
        animator.SetInteger(hashDie, 2);
        rb.isKinematic = true;
        CapCol.enabled = false;
        gameObject.tag = u_tag;
        StartCoroutine(ObjPoolPush());
    }
    IEnumerator ObjPoolPush()
    {
        yield return new WaitForSeconds(3.0f);
        isDie = false;
        rb.isKinematic = false;
        CapCol.enabled = true;
        gameObject.tag = s_tag;
        state = S_State.PATOL;
        gameObject.SetActive(false);
    }
    void OnPlayerDie()
    {
        StopAllCoroutines();//모든 코루틴 중지
        animator.SetTrigger(hashPlayerDie);
        GameManager.G_instance.isGameOver = true;
    }
    void Update()
    {
        animator.SetFloat(hashMoveSpeed, moveAgent.Speed);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(state);
        }
        else
        {
            remote_State = (S_State)stream.ReceiveNext();
        }
    }
}
