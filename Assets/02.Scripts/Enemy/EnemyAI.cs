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

    //private readonly string playStr = "Player"; //�̱ۿ�
    private readonly string e_tag = "ENEMY";
    private readonly string u_tag = "Untagged";

    public float attackDist = 5.0f; //�÷��̾�� �Ÿ��� 5 �����̸� ���� ���.
    public float traceDist = 10.0f; //�÷��̾�� �Ÿ��� 10���ϸ� �߰��Ѵ�. �� �̻��̸� ��Ʈ��
    public bool isDie = false; //���ʹ��� ���� ���� �Ǵ�

    //�ִϸ��̼� ��Ʈ�ѷ��� ���� �� �Ķ������ �ؽð��� ������ �����Ѵ�. (�ؽð��� �ش� �Ķ������ �ּҰ��̶�� ������..)
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int hashDie = Animator.StringToHash("DieIdx");
    private readonly int hasDieTrigger = Animator.StringToHash("DieTrigger");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDieTrigger");
    public enum State //���ʹ��� ���¸� ������ ����� ����
    {
        PTROL=0 ,TRACE ,ATTACK ,DIE, EXPDIE
    }
    public State state = State.PTROL; //ó�� �⺻ ���´� 0�� PTROL

    void Awake()
    { //���ʹ̰� ��Ʈ���ϴ� ��ɺ��� �������� ������ ��� ���� Awake���.
        //var player = GameObject.FindGameObjectWithTag(playStr); //��Ʈ��ũ������ �ش��ڵ� ���Ұ�.
        //if(player != null)
        //    playerTr = player.GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();

        ws = new WaitForSeconds(0.3f); //��ٸ��� �ð� 0.3�ʷ� �̸� �ʱ�ȭ

        moveAgent = GetComponent<EnemyMoveAgent>(); //EnemyMoveAgent��ũ��Ʈ ����
        enemyFire = GetComponent<EnemyFire>(); //EnemyFire��ũ��Ʈ ����

        rb = GetComponent<Rigidbody>();
        CapCol = GetComponent<CapsuleCollider>();

        enemyFOV = GetComponent<EnemyFOV>();
    }
    private void OnEnable() //������Ʈ�� Ȱ��ȭ �� ������. ȣ��
    {
        Damage.OnPlayerDie += OnPlayerDie; //Damage��ũ��Ʈ�� OnPlayerDie �븮�� ������ OnPlayerDie�Լ��� �ִ´�.
        //�̺�Ʈ ����
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f)); //��� 
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 2.0f)); //�ȴ� �ӵ��� 1��Ӻ��� 2��ӱ��� ���´�.

        if (PhotonNetwork.IsMasterClient)
        { 
            StartCoroutine(CheckState()); //�Ÿ� �������� state���� ����
            StartCoroutine(Action()); // state���¿� ���� �ִϸ��̼� ���
        }
    }
    private void OnDisable() //������Ʈ�� ��Ȱ��ȭ �Ǹ� ȣ��.
    {
        Damage.OnPlayerDie -= OnPlayerDie; ////Damage��ũ��Ʈ�� OnPlayerDie �븮�� ������ OnPlayerDie�Լ��� ����.
    }
    IEnumerator CheckState() //state�� �����ϴ� �Լ�
    {
        yield return new WaitForSeconds(2.0f); //������Ʈ Ǯ������ ������ �ٸ� ��ũ��Ʈ�� �ʱ�ȭ�� ���� ���.

        while(!isDie)
        {
            if(state == State.DIE || state == State.EXPDIE) yield break; //��� ���¸� StartCoroutine�ٷ� ����
            if (enemyFOV.PlayerTr != null)
            {
                //float dist = (playerTr.position - enemyTr.position).magnitude; //�̱ۿ�
                float dist = (enemyFOV.PlayerTr.transform.position - enemyTr.position).magnitude; //��Ƽ��
                if (dist <= attackDist) //���� �����Ÿ� ���̶��
                {
                    if (enemyFOV.isViewPlayer()) //�÷��̾ ���Ҵٸ�
                        state = State.ATTACK; //state�� ���û���� �ٲ�
                    else //��ֹ��̳� �跲�� ���� �ִٸ�
                        state = State.TRACE; //state�� Ʈ���̽� ����� �ٲ�
                }
                else if (enemyFOV.isTracePlayer()) //Ʈ���̽� �����Ÿ� �ȿ��� �÷��̾ �ٶ󺸰� �ִٸ� (���� �ڵ� dist <= traceDist)
                    state = State.TRACE; //state�� Ʈ���̽� ����� �ٲ�

                else //��� �ƴ϶��
                    state = State.PTROL; //state�� ��Ʈ�� ����ιٲ�
            }

            yield return ws;//���¸� üũ�� �� 0.3�� �����̸� ����
        }
    }
    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws; //0.3�� �� ���� ����ġ�� �ߵ�

            switch (state)
            {
                case State.PTROL:
                    moveAgent.patrolling = true; //��Ʈ�� ������Ƽ�� set�� true�� �ִ´�, ��Ʈ�� �Լ� ������.
                    animator.SetBool(hashMove, true);
                    enemyFire.isFire = false;
                    break;

                case State.ATTACK:
                    moveAgent.Stop(); //�÷��̾ ������ ���� ��� ���缭 ���� �����ϱ� ������ ���� �Լ� �ҷ���.
                    animator.SetBool(hashMove, false);
                    if(enemyFire.isFire == false) //if���� �־ �ǰ� ��� ����� ����.
                        enemyFire.isFire = true;
                    break;

                case State.TRACE:
                    //moveAgent.traceTarget = playerTr.position; //�߰� ������Ƽ�� ȣ���Ͽ� player��ġ�� �Է��Ѵ�. //�̱ۿ�
                    moveAgent.traceTarget = enemyFOV.PlayerTr.transform.position; //�߰� ������Ƽ�� ȣ���Ͽ� player��ġ�� �Է��Ѵ�. //��Ƽ��
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
        if (isDie) return; //�����Լ� ������ ȣ�� ����
        photonView.RPC("RPC_E_Diestate", RpcTarget.Others);
        moveAgent.Stop(); //���ʹ̰� �׾��� ��� ���ڸ����� �������.
        enemyFire.isFire = false;
        isDie = true;
        animator.SetTrigger(hasDieTrigger);
        animator.SetInteger(hashDie, Random.Range(0, 3));
        rb.isKinematic = true; //��������
        CapCol.enabled = false; //�ݶ��̴� ����
        gameObject.tag = u_tag; //�±� �����ϱ�
        StartCoroutine(ObjPoolPush());
    }
    private void E_ExpDiestate()
    {
        if (isDie) return; //�����Լ� ������ ȣ�� ����
        photonView.RPC("RPC_E_ExpDiestate", RpcTarget.Others);
        moveAgent.Stop(); //���ʹ̰� �׾��� ��� ���ڸ����� �������.
        enemyFire.isFire = false;
        isDie = true;
        animator.SetTrigger(hasDieTrigger);
        animator.SetInteger(hashDie, 2);
        rb.isKinematic = true; //��������
        CapCol.enabled = false; //�ݶ��̴� ����
        gameObject.tag = u_tag; //�±� �����ϱ�
        StartCoroutine(ObjPoolPush());
    }
    IEnumerator ObjPoolPush()
    {
        yield return new WaitForSeconds(3.0f);
        isDie = false;
        rb.isKinematic = false; //���� �ٽ� Ű��
        CapCol.enabled = true; //�ݶ��̴� �ٽ� Ű��
        gameObject.tag = e_tag; //�±� �ٽ� �޾��ֱ�
        state = State.PTROL;
        gameObject.SetActive(false);
    }
    void OnPlayerDie()
    {
        StopAllCoroutines();//��� �ڷ�ƾ ����
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
