using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[RequireComponent(typeof(Animator))]

public class EnemyAI : MonoBehaviourPun
{
    //private Transform playerTr;
    private Transform enemyTr;
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider CapCol;

    private EnemyFire enemyFire;
    private EnemyMoveAgent moveAgent;
    private WaitForSeconds ws;

    private EnemyFOV enemyFOV;

    //private readonly string playStr = "Player"; //싱글용
    private readonly string e_tag = "ENEMY";
    private readonly string u_tag = "Untagged";

    public float attackDist = 5.0f; //플레이어와 거리가 5 이하이면 총을 쏜다.
    public float traceDist = 10.0f; //플레이어와 거리가 10이하면 추격한다. 그 이상이면 패트롤
    public bool isDie = false; //에너미의 죽음 여부 판단

    //애니메이션 컨트롤러에 정의 한 파라미터의 해시값을 정수로 추출한다. (해시값은 해당 파라미터의 주소값이라고 이해중..)
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int hashDie = Animator.StringToHash("DieIdx");
    private readonly int hasDieTrigger = Animator.StringToHash("DieTrigger");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDieTrigger");
    public enum State //에너미의 상태를 열거형 상수로 선언
    {
        PTROL=0 ,TRACE ,ATTACK ,DIE, EXPDIE
    }
    public State state = State.PTROL; //처음 기본 상태는 0번 PTROL

    void Awake()
    { //에너미가 패트롤하는 기능보다 더빠르게 정보를 얻기 위해 Awake사용.
        //var player = GameObject.FindGameObjectWithTag(playStr); //네트워크에서는 해당코드 사용불가.
        //if(player != null)
        //    playerTr = player.GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();

        ws = new WaitForSeconds(0.3f); //기다리는 시간 0.3초로 미리 초기화

        moveAgent = GetComponent<EnemyMoveAgent>(); //EnemyMoveAgent스크립트 연결
        enemyFire = GetComponent<EnemyFire>(); //EnemyFire스크립트 연결

        rb = GetComponent<Rigidbody>();
        CapCol = GetComponent<CapsuleCollider>();

        enemyFOV = GetComponent<EnemyFOV>();
    }
    private void OnEnable() //오브젝트가 활성화 될 때마다. 호출
    {
        Damage.OnPlayerDie += OnPlayerDie; //Damage스크립트에 OnPlayerDie 대리자 변수에 OnPlayerDie함수를 넣는다.
        //이벤트 연결
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f)); //재생 
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 2.0f)); //걷는 속도가 1배속부터 2배속까지 나온다.

        if (PhotonNetwork.IsMasterClient)
        { 
            StartCoroutine(CheckState()); //거리 측정으로 state상태 설정
            StartCoroutine(Action()); // state상태에 따라 애니메이션 출력
        }
    }
    private void OnDisable() //오브젝트가 비활성화 되면 호출.
    {
        Damage.OnPlayerDie -= OnPlayerDie; ////Damage스크립트에 OnPlayerDie 대리자 변수에 OnPlayerDie함수를 뺀다.
    }
    IEnumerator CheckState() //state만 관리하는 함수
    {
        yield return new WaitForSeconds(2.0f); //오브젝트 풀링으로 생성시 다른 스크립트의 초기화를 위해 대기.

        while(!isDie)
        {
            if(state == State.DIE || state == State.EXPDIE) yield break; //사망 상태면 StartCoroutine바로 종료
            if (enemyFOV.PlayerTr != null)
            {
                //float dist = (playerTr.position - enemyTr.position).magnitude; //싱글용
                float dist = (enemyFOV.PlayerTr.transform.position - enemyTr.position).magnitude; //멀티용
                if (dist <= attackDist) //어택 사정거리 안이라면
                {
                    if (enemyFOV.isViewPlayer()) //플레이어를 보았다면
                        state = State.ATTACK; //state를 어택상수로 바꿈
                    else //장애물이나 배럴에 막혀 있다면
                        state = State.TRACE; //state를 트레이스 상수로 바꿈
                }
                else if (enemyFOV.isTracePlayer()) //트레이스 사정거리 안에서 플레이어를 바라보고 있다면 (기존 코드 dist <= traceDist)
                    state = State.TRACE; //state를 트레이스 상수로 바꿈

                else //모두 아니라면
                    state = State.PTROL; //state를 패트롤 상수로바꿈
            }

            yield return ws;//상태를 체크한 후 0.3초 딜레이를 가짐
        }
    }
    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws; //0.3초 후 부터 스위치문 발동

            switch (state)
            {
                case State.PTROL:
                    moveAgent.patrolling = true; //패트롤 프로퍼티에 set에 true를 넣는다, 패트롤 함수 가져옴.
                    animator.SetBool(hashMove, true);
                    enemyFire.isFire = false;
                    break;

                case State.ATTACK:
                    moveAgent.Stop(); //플레이어가 가까이 있을 경우 멈춰서 총을 쏴야하기 때문에 멈춤 함수 불러옴.
                    animator.SetBool(hashMove, false);
                    if(enemyFire.isFire == false) //if문이 있어도 되고 없어도 결과는 같다.
                        enemyFire.isFire = true;
                    break;

                case State.TRACE:
                    //moveAgent.traceTarget = playerTr.position; //추격 프로퍼티를 호출하여 player위치를 입력한다. //싱글용
                    moveAgent.traceTarget = enemyFOV.PlayerTr.transform.position; //추격 프로퍼티를 호출하여 player위치를 입력한다. //멀티용
                    animator.SetBool(hashMove, true);
                    enemyFire.isFire = false;
                    break;

                case State.DIE:
                    E_Diestate();
                    break;
                case State.EXPDIE:
                    E_ExpDiestate();
                    break;
            }
        }
    }

    private void E_Diestate()
    {
        if (isDie) return; //죽음함수 여러번 호출 방지
        photonView.RPC("RPC_E_Diestate", RpcTarget.Others);
        moveAgent.Stop(); //에너미가 죽었을 경우 제자리에서 멈춰야함.
        enemyFire.isFire = false;
        isDie = true;
        animator.SetTrigger(hasDieTrigger);
        animator.SetInteger(hashDie, Random.Range(0, 3));
        rb.isKinematic = true; //물리제거
        CapCol.enabled = false; //콜라이더 제거
        gameObject.tag = u_tag; //태그 제거하기
        StartCoroutine(ObjPoolPush());
    }
    private void E_ExpDiestate()
    {
        if (isDie) return; //죽음함수 여러번 호출 방지
        photonView.RPC("RPC_E_ExpDiestate", RpcTarget.Others);
        moveAgent.Stop(); //에너미가 죽었을 경우 제자리에서 멈춰야함.
        enemyFire.isFire = false;
        isDie = true;
        animator.SetTrigger(hasDieTrigger);
        animator.SetInteger(hashDie, 2);
        rb.isKinematic = true; //물리제거
        CapCol.enabled = false; //콜라이더 제거
        gameObject.tag = u_tag; //태그 제거하기
        StartCoroutine(ObjPoolPush());
    }
    IEnumerator ObjPoolPush()
    {
        yield return new WaitForSeconds(3.0f);
        isDie = false;
        rb.isKinematic = false; //물리 다시 키기
        CapCol.enabled = true; //콜라이더 다시 키기
        gameObject.tag = e_tag; //태그 다시 달아주기
        state = State.PTROL;
        gameObject.SetActive(false);
    }
    void OnPlayerDie()
    {
        StopAllCoroutines();//모든 코루틴 중지
        animator.SetTrigger(hashPlayerDie);
        GameManager.G_instance.isGameOver = true;
    }

    [PunRPC]
    private void RPC_E_Diestate()
    {
        if (isDie) return;
        enemyFire.isFire = false;
        isDie = true;
        rb.isKinematic = true;
        CapCol.enabled = false; 
        gameObject.tag = u_tag; 
        StartCoroutine(RPC_ObjPoolPush());
    }

    [PunRPC]
    private void RPC_E_ExpDiestate()
    {
        if (isDie) return;
        enemyFire.isFire = false;
        isDie = true;
        rb.isKinematic = true;
        CapCol.enabled = false;
        gameObject.tag = u_tag;
        StartCoroutine(RPC_ObjPoolPush());
    }

    IEnumerator RPC_ObjPoolPush()
    {
        yield return new WaitForSeconds(3.0f);
        isDie = false;
        rb.isKinematic = false;
        CapCol.enabled = true;
        gameObject.tag = e_tag;
        gameObject.SetActive(false);
    }


    void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }
}
