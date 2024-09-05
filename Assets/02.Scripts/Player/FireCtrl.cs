using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
[System.Serializable]
public struct PlayerSound
{
    public AudioClip[] fire; //�ѱ� ��ü�� �߻���� �ٸ����ϱ� ���� ����ü ����
    public AudioClip[] reload; //�ѱ� ��ü�� �������� �ٸ����ϱ� ���� ����ü ���� 
}

public class FireCtrl : MonoBehaviourPun, IPunObservable
{
    public enum WeaponType //������ ������ �����ϱ� ���� ������ ���
    {
        RIFLE = 0, SHOTGUN = 1
    }

    private Transform FirePos;
    private AudioSource Source;
    //private AudioClip fireClip;
    private Player player;
    [SerializeField]private ParticleSystem MuzzleFlash; //���� ����.
    private Image magazineImg; //źâ �̹���
    private Text magazineTxt; //���� �Ѿ˼� text UI
    private Sprite[] weaponIcon; //sprite�̹����� ��� �迭 ����
    private Image weaponImg;
    private Animator animator;

    private int enemyLayer; //�� ���̾� ��ȣ�� ���� ����.
    private int swatLayer;
    private int barrelLayer;
    private int boxesLayer;
    private int layerMask;

    public PlayerSound playerSound; //����ü�� ����ϱ� ���� ����.
    public WeaponType curType = WeaponType.RIFLE; //�⺻������ ������ �����ϰ� �ִٴ� ���� �˷��ֱ� ���� ����.

    private float fireTime;
    //private float reloadTime = 2.0f; //���������� �ɸ��� �ð�. �ش� ���� �Ⱦ��� �ִϸ��̼� ���̺��� 0.3�� �� ��� ���.
    private float nextFire;//�߻�� �ð��� ������ ����.
    public float fireRate = 0.5f; //�Ѿ� �߻� ����.

    private readonly string fireposStr = "P_FirePos";
    //private readonly string fireClipStr = "Sound/p_ak_1";
    private readonly string enemytag = "ENEMY";
    private readonly string swattag = "SWAT";
    private readonly string walltag = "WALL";
    private readonly string barreltag = "BARREL";
    private readonly string UIObj = "Canvas_UI";
    private readonly string WPImgFolder = "WeaponIcons";
    private readonly string boxestag = "BOXES";

    private int maxBullet = 10; //�ִ� �Ѿ� ��
    private int curBullet = 10; //���� �Ѿ� ��

    public bool isReloading = false; //������ ���¸� �˸��� ����
    public bool isFire = false; //�� �߻� ���¸� ��Ÿ���� ����
    
    private const float FIREDIST = 20f; //�Ѿ��� ������ �ִ�Ÿ��� ����� ����
    void Start()
    {
        FirePos = GameObject.Find(fireposStr).transform.GetComponent<Transform>();
        Source = GetComponent<AudioSource>();
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        //fireClip = Resources.Load(fireClipStr) as AudioClip;
        magazineImg = GameObject.Find(UIObj).transform.GetChild(1).GetChild(2).GetComponent<Image>();
        magazineTxt = GameObject.Find(UIObj).transform.GetChild(1).GetChild(0).GetComponent<Text>();
        weaponImg = GameObject.Find(UIObj).transform.GetChild(3).GetChild(0).GetComponent<Image>();
        weaponIcon = Resources.LoadAll<Sprite>(WPImgFolder);

        enemyLayer = LayerMask.NameToLayer(enemytag);//���̾��� �̸����� ���̾� ��ȣ�� ã�� ���� (string -> int)
        swatLayer = LayerMask.NameToLayer(swattag);//LayerMask.LayerToName(����); ���̾��� ��ȣ�� �̸��� ã�� ���� (int -> string)
        barrelLayer = LayerMask.NameToLayer(barreltag);
        boxesLayer = LayerMask.NameToLayer(boxestag);
        layerMask = 1 << enemyLayer | 1 << swatLayer |1 << barrelLayer | 1 << boxesLayer;

        fireTime = Time.time;
        MuzzleFlash.Stop();
    }
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //�հ����̳� ���콺Ŀ���� ���� ������Ʈ ������ Ŭ���Ǹ� ���ϵȴ�. 

        if (photonView.IsMine) //���� ������ ������ �� �����ؾ���.
        {
            //Debug.DrawRay(FirePos.position, FirePos.forward * 25f, Color.green); ����ĳ��Ʈ ������ �� �󿡼� ���̱�.
            RaycastHit hit; //������ ���� ������Ʈ�� ��ġ�� �Ÿ������� ���� ����.
            if (Physics.Raycast(FirePos.position, FirePos.forward, out hit, 25f, layerMask))
                isFire = (hit.collider.CompareTag(enemytag)) | (hit.collider.CompareTag(swattag));
            else //����ĳ��Ʈ�� swat�̳� enemy���̾ �����Ǵ� ����
            {
                isFire = false;
                //MuzzleFlash.Stop();
                photonView.RPC("OnMuzzleFlash", RpcTarget.All, isFire);
            }

            if (!player.isRun && !isReloading && isFire) //�޸��� �ʰ�, �������� �ʰ�, isFire�� ���� ���
            {
                if (Time.time > nextFire)
                {
                    --curBullet;
                    //Fire(); //��Ƽ������ ������Ŭ���̾�Ʈ�� �߻�ó�� -> �ش� �����͵� �����ؾ���.
                    photonView.RPC("Fire", RpcTarget.MasterClient);
                    //MuzzleFlash.Play();
                    photonView.RPC("OnMuzzleFlash", RpcTarget.All, isFire);
                    if (curBullet == 0)
                    {
                        //StartCoroutine(ReLoading());
                        photonView.RPC("RPCReloading", RpcTarget.All);
                    }
                    nextFire = Time.time + fireRate; //�߻� �� 0.2�� �ڿ� �߻��.
                }
            }
            #region ���콺 ���� ��ư���� �߻�
            if (Input.GetMouseButtonDown(0) && !isReloading)
            {
                //if (!player.isRun)
                //{
                isFire = true;
                --curBullet;
                //Fire(); //��Ƽ������ ������Ŭ���̾�Ʈ�� �߻�ó��
                photonView.RPC("Fire", RpcTarget.MasterClient);
                //MuzzleFlash.Play();
                photonView.RPC("OnMuzzleFlash", RpcTarget.All, isFire);
                if (curBullet == 0)
                {
                    //StartCoroutine(ReLoading());
                    photonView.RPC("RPCReloading", RpcTarget.All);
                }
                //}
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isFire = false;
                //MuzzleFlash.Stop();
                photonView.RPC("OnMuzzleFlash", RpcTarget.All, isFire);
            }
            #endregion
        }
    }
    IEnumerator ReLoading()
    {
        isReloading = true;
        Source.PlayOneShot(playerSound.reload[(int)curType], playerSound.reload[(int)curType].length);
        animator.SetTrigger("Reload");
        yield return new WaitForSeconds(playerSound.reload[(int)curType].length + 0.3f); //�ش� �ѿ� ���ε� ���庸�� 0.3�� ��� ����.
        isReloading = false;
        curBullet = maxBullet;
        UpdateBulletUI();
    }

    [PunRPC]
    private void Fire()
    {
            #region projectile movement���
            //Instantiate(Bullet, FirePos.position, FirePos.rotation); //������Ʈ Ǯ���� �ƴ� ��
            //var _bullet = ObjectPoolingManager.poolingManager.GetBulletPool();
            //if (_bullet != null)
            //{
            //    _bullet.transform.position = FirePos.position;
            //    _bullet.transform.rotation = FirePos.rotation;
            //    _bullet.SetActive(true);
            //}
            #endregion
            #region Raycast���
            RaycastHit hit; //������ ������Ʈ�� ������ �浹�����̳� �Ÿ��� �˷��ִ� ����ü
            if (Physics.Raycast(FirePos.position, FirePos.forward, out hit, FIREDIST)) //out�� hit�� ��� �浹 ���θ� ��ȯ?
            {//������ FirePos.position���� �������� �߻��ϰ�, 20���� �ȿ��� �¾Ҵ��� ���θ� bool�� ��ȯ.
                if (hit.collider.gameObject.tag == enemytag || hit.collider.gameObject.tag == swattag)//�������� ������ enemy�� swat�� ���
                {
                    object[] _params = new object[2]; //��� �ڷ����� ������ �ִ� ������Ʈ �ڷ����� ���� hit�������� ��´�.
                    _params[0] = hit.point; //ù��° �迭�� ���� ��ġ�� ��´�.
                    _params[1] = 25f; //�ι�° �迭�� ������ ���� ��´�.
                    hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
                    //sendmessage�� �ٸ� ���� �ִ� �Լ��� �ҷ� �� �� ����. private�� �ҷ��� �� ����.
                    //������ ���� ������Ʈ�� �Լ��� ȣ���ϸ鼭 �Ű�����(_params) ���� �����Ѵ�.
                    //������ ������ OnDamage��� �Լ��� ���ų� �� �Լ��� ȣ�� �� �� ������ ������ �߻����� �ʵ��� �ϴ� ��ɾ�.
                }
                if (hit.collider.gameObject.tag == walltag)
                {
                    object[] _params = new object[2];
                    _params[0] = hit.point;
                    hit.collider.gameObject.SendMessage("OnHit", _params, SendMessageOptions.DontRequireReceiver);
                }
                if (hit.collider.gameObject.tag == barreltag)
                {
                    object[] _params = new object[2];
                    _params[0] = FirePos.position;//�߻� ��ġ
                    _params[1] = hit.point;//���� ��ġ
                    hit.collider.gameObject.SendMessage("OnHit", _params, SendMessageOptions.DontRequireReceiver);
                }
            #endregion
            Source.PlayOneShot(playerSound.fire[(int)curType], 0.2f);
            fireTime = Time.time;
            UpdateBulletUI();
        }
    }

    [PunRPC]
    private void RPCReloading()
    {
        StartCoroutine(ReLoading());
    }

    [PunRPC]
    private void OnMuzzleFlash(bool state)
    {
        if (state)
            MuzzleFlash.Play();
        else
            MuzzleFlash.Stop();
    }

    private void UpdateBulletUI()
    {
        magazineImg.fillAmount = (float)curBullet / (float)maxBullet;
        magazineTxt.text = string.Format($"<color=#ff0000>{curBullet}</color>/{maxBullet}");
    }

    public void OnChangeWeapon()
    {
        curType = (WeaponType)((int)++curType % 2); //ü���� ��ư�� ������ curType�� ������ 1�� �ø���.
        //������ ������ ������ 2���̰� ���� �ε����� 0�̱⶧���� 2�� ���� ������ ������ curType ���� ����
        weaponImg.sprite = weaponIcon[(int)curType]; //������ ������ curType�� �ε����� int�� ����ȯ�Ͽ� �ش� sprite�� ���
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
