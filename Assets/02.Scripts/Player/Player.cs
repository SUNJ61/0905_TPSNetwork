using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
//[System.Serializable] // public���� ����� ��� �ʵ带 �ν����� â�� �����ش�.
//public class PlayerAniMation //�ִϸ��̼� �̸��� �����ϰ� �� �� �ְ�, ���羲�� �ִϸ��̼��� �������� �� �� �ִ�.
//{
//    public AnimationClip idle;
//    public AnimationClip runForward;
//    public AnimationClip runBackward;
//    public AnimationClip runLeft;
//    public AnimationClip runRight;
//    public AnimationClip Sprint;
//}
public class Player : MonoBehaviourPun, IPunObservable
{
    //public PlayerAniMation playerAnimation;

    private Rigidbody rb;
    private CapsuleCollider capCol;
    private Transform tr;
    //private Animation _animation;
    private Animator animator;

    private Vector3 MoveDir = Vector3.zero;

    private Vector3 RemotePos = Vector3.zero;
    private Quaternion RemoteRot = Quaternion.identity;

    [SerializeField]private float moveSpeed;
    private float MoveRot;
    //private float rotSpeed = 90f;
    //private float h, v, r;

    public bool isRun = false;
    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetUp; //�̺�Ʈ�� ����Ѵ�. �ش� �̺�Ʈ�� �κ��丮�� �������� �߰��ǰų� ������ �ߵ�.
    }
    void UpdateSetUp()
    {
        moveSpeed = GameManager.G_instance.gameData.speed;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();//�̿� ���� ������� ���۳�Ʈ ĳ��ó����� �Ѵ�.
        capCol = rb.GetComponent<CapsuleCollider>();
        tr = rb.GetComponent<Transform>();
        animator = GetComponent<Animator>();
        //_animation = GetComponent<Animation>();
        //_animation.Play(playerAnimation.idle.name); // �ִϸ��̼� idle�ൿ�� string ���� ��������

        moveSpeed = GameManager.G_instance.gameData.speed;
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            #region ���Ž� ��ǲ
            //h = Input.GetAxis("Horizontal");
            //v = Input.GetAxis("Vertical");
            //r = Input.GetAxis("Mouse X");

            //Vector3 moveDir = (h * Vector3.right) + (v * Vector3.forward);
            //tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
            ////Space.Self�� ���� ��ǥ�� �������� �ǹ��Ѵ�.(�ٶ󺸴� ������) world�� ���� ���� ��ǥ -> �̵��� ������ ���� �������� �̵�
            #endregion
            tr.Translate(MoveDir * moveSpeed * Time.deltaTime);
            {
                MoveAni();
                //RunAni();
            }
            #region ���Ž� ��ǲ
            //tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * r);
            #endregion
            tr.Rotate(Vector3.up * MoveRot * Time.deltaTime * 5.0f);
        }
        else
        {
            tr.position = Vector3.Lerp(tr.position, RemotePos, Time.deltaTime * 10.0f);
            tr.rotation = Quaternion.Lerp(tr.rotation, RemoteRot, Time.deltaTime * 10.0f);
        }
    }
    #region �ִϸ��̼� ����� ��Ƽ �Ұ�
    //private void RunAni()
    //{
        //if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    moveSpeed = GameManager.G_instance.gameData.speed + 6f;
        //    _animation.CrossFade(playerAnimation.Sprint.name, 0.3f);
        //    isRun = true;
        //}
        //else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.LeftShift))
        //{
        //    moveSpeed = GameManager.G_instance.gameData.speed;
        //    _animation.CrossFade(playerAnimation.runForward.name, 0.3f);
        //    isRun = false;
        //}
    //}
    #endregion
    private void MoveAni()
    {
        #region ���Ž� ��ǲ
        //if (h > 0.1f)
        //    _animation.CrossFade(playerAnimation.runRight.name, 0.3f); //���� �ִϿ� ���� ������ �ִϸ� 0.3�ʰ� ȥ���Ͽ� �ε巴�� ���
        //else if (h < -0.1f)
        //    _animation.CrossFade(playerAnimation.runLeft.name, 0.3f);
        //else if (v > 0.1f)
        //    _animation.CrossFade(playerAnimation.runForward.name, 0.3f);
        //else if (v < -0.1f)
        //    _animation.CrossFade(playerAnimation.runBackward.name, 0.3f);
        //else
        //    _animation.CrossFade(playerAnimation.idle.name, 0.3f);
        #endregion

        #region �ִϸ��̼� ����� ��Ƽ �Ұ�
        //if (MoveDir.z > 0.1f)
        //    _animation.CrossFade(playerAnimation.runRight.name, 0.3f); //���� �ִϿ� ���� ������ �ִϸ� 0.3�ʰ� ȥ���Ͽ� �ε巴�� ���
        //else if (MoveDir.z< -0.1f)
        //    _animation.CrossFade(playerAnimation.runLeft.name, 0.3f);
        //else if (MoveDir.x > 0.1f)
        //    _animation.CrossFade(playerAnimation.runForward.name, 0.3f);
        //else if (MoveDir.x < -0.1f)
        //    _animation.CrossFade(playerAnimation.runBackward.name, 0.3f);
        //else
        //    _animation.CrossFade(playerAnimation.idle.name, 0.3f);
        #endregion
        float x = MoveDir.z;
        float y = MoveDir.x;
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = GameManager.G_instance.gameData.speed + 6f;
            isRun = true;
            x = MoveDir.z * 2.0f;
        }
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = GameManager.G_instance.gameData.speed;
            isRun = false;
            x = MoveDir.z;
        }

        animator.SetFloat("Xpos", x);
        animator.SetFloat("Ypos", y);
    }

    private void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        MoveDir = new Vector3(dir.x, 0f, dir.y).normalized;
    }

    private void OnLook(InputValue value)
    {
        Vector2 rot = value.Get<Vector2>();
        float xPos = rot.x;

        MoveRot = xPos;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            RemotePos = (Vector3)stream.ReceiveNext();
            RemoteRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
