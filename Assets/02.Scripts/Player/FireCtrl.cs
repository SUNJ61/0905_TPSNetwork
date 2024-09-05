using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
[System.Serializable]
public struct PlayerSound
{
    public AudioClip[] fire; //총기 교체시 발사사운드 다르게하기 위해 구조체 선언
    public AudioClip[] reload; //총기 교체시 장전사운드 다르게하기 위해 구조체 선언 
}

public class FireCtrl : MonoBehaviourPun, IPunObservable
{
    public enum WeaponType //무기의 종류를 선언하기 위한 열거형 상수
    {
        RIFLE = 0, SHOTGUN = 1
    }

    private Transform FirePos;
    private AudioSource Source;
    //private AudioClip fireClip;
    private Player player;
    [SerializeField]private ParticleSystem MuzzleFlash; //직접 넣음.
    private Image magazineImg; //탄창 이미지
    private Text magazineTxt; //남은 총알수 text UI
    private Sprite[] weaponIcon; //sprite이미지를 담는 배열 선언
    private Image weaponImg;
    private Animator animator;

    private int enemyLayer; //적 레이어 번호를 받을 변수.
    private int swatLayer;
    private int barrelLayer;
    private int boxesLayer;
    private int layerMask;

    public PlayerSound playerSound; //구조체를 사용하기 위한 변수.
    public WeaponType curType = WeaponType.RIFLE; //기본적으로 샷건을 장착하고 있다는 것을 알려주기 위한 변수.

    private float fireTime;
    //private float reloadTime = 2.0f; //재장전까지 걸리는 시간. 해당 변수 안쓰고 애니메이션 길이보다 0.3초 더 길게 사용.
    private float nextFire;//발사된 시간을 저장할 변수.
    public float fireRate = 0.5f; //총알 발사 간격.

    private readonly string fireposStr = "P_FirePos";
    //private readonly string fireClipStr = "Sound/p_ak_1";
    private readonly string enemytag = "ENEMY";
    private readonly string swattag = "SWAT";
    private readonly string walltag = "WALL";
    private readonly string barreltag = "BARREL";
    private readonly string UIObj = "Canvas_UI";
    private readonly string WPImgFolder = "WeaponIcons";
    private readonly string boxestag = "BOXES";

    private int maxBullet = 10; //최대 총알 수
    private int curBullet = 10; //현재 총알 수

    public bool isReloading = false; //재장전 상태를 알리는 변수
    public bool isFire = false; //총 발사 상태를 나타내는 변수
    
    private const float FIREDIST = 20f; //총알이 나가는 최대거리를 상수로 선언
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

        enemyLayer = LayerMask.NameToLayer(enemytag);//레이어의 이름으로 레이어 번호를 찾아 저장 (string -> int)
        swatLayer = LayerMask.NameToLayer(swattag);//LayerMask.LayerToName(숫자); 레이어의 번호로 이름을 찾아 저장 (int -> string)
        barrelLayer = LayerMask.NameToLayer(barreltag);
        boxesLayer = LayerMask.NameToLayer(boxestag);
        layerMask = 1 << enemyLayer | 1 << swatLayer |1 << barrelLayer | 1 << boxesLayer;

        fireTime = Time.time;
        MuzzleFlash.Stop();
    }
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        //손가락이나 마우스커서가 게임 오브젝트 위에서 클릭되면 리턴된다. 

        if (photonView.IsMine) //맞은 정보를 서버가 모름 전달해야함.
        {
            //Debug.DrawRay(FirePos.position, FirePos.forward * 25f, Color.green); 레이캐스트 레이저 씬 상에서 보이기.
            RaycastHit hit; //광선에 맞은 오브젝트의 위치와 거리정보가 담기는 변수.
            if (Physics.Raycast(FirePos.position, FirePos.forward, out hit, 25f, layerMask))
                isFire = (hit.collider.CompareTag(enemytag)) | (hit.collider.CompareTag(swattag));
            else //레이캐스트에 swat이나 enemy레이어가 감지되는 조건
            {
                isFire = false;
                //MuzzleFlash.Stop();
                photonView.RPC("OnMuzzleFlash", RpcTarget.All, isFire);
            }

            if (!player.isRun && !isReloading && isFire) //달리지 않고, 장전하지 않고, isFire가 참일 경우
            {
                if (Time.time > nextFire)
                {
                    --curBullet;
                    //Fire(); //멀티에서는 마스터클라이언트가 발사처리 -> 해당 데이터들 전송해야함.
                    photonView.RPC("Fire", RpcTarget.MasterClient);
                    //MuzzleFlash.Play();
                    photonView.RPC("OnMuzzleFlash", RpcTarget.All, isFire);
                    if (curBullet == 0)
                    {
                        //StartCoroutine(ReLoading());
                        photonView.RPC("RPCReloading", RpcTarget.All);
                    }
                    nextFire = Time.time + fireRate; //발사 후 0.2초 뒤에 발사됨.
                }
            }
            #region 마우스 왼쪽 버튼으로 발사
            if (Input.GetMouseButtonDown(0) && !isReloading)
            {
                //if (!player.isRun)
                //{
                isFire = true;
                --curBullet;
                //Fire(); //멀티에서는 마스터클라이언트가 발사처리
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
        yield return new WaitForSeconds(playerSound.reload[(int)curType].length + 0.3f); //해당 총에 리로드 사운드보다 0.3초 길게 쉰다.
        isReloading = false;
        curBullet = maxBullet;
        UpdateBulletUI();
    }

    [PunRPC]
    private void Fire()
    {
            #region projectile movement방식
            //Instantiate(Bullet, FirePos.position, FirePos.rotation); //오브젝트 풀링이 아닐 때
            //var _bullet = ObjectPoolingManager.poolingManager.GetBulletPool();
            //if (_bullet != null)
            //{
            //    _bullet.transform.position = FirePos.position;
            //    _bullet.transform.rotation = FirePos.rotation;
            //    _bullet.SetActive(true);
            //}
            #endregion
            #region Raycast방식
            RaycastHit hit; //광선이 오브젝트에 맞으면 충돌지점이나 거리를 알려주는 구조체
            if (Physics.Raycast(FirePos.position, FirePos.forward, out hit, FIREDIST)) //out은 hit에 담긴 충돌 여부를 반환?
            {//광선을 FirePos.position에서 전방으로 발사하고, 20범위 안에서 맞았는지 여부를 bool로 반환.
                if (hit.collider.gameObject.tag == enemytag || hit.collider.gameObject.tag == swattag)//레이저에 맞은게 enemy나 swat일 경우
                {
                    object[] _params = new object[2]; //모든 자료형을 담을수 있는 오브젝트 자료형을 통해 hit정보들을 담는다.
                    _params[0] = hit.point; //첫번째 배열은 맞은 위치를 담는다.
                    _params[1] = 25f; //두번째 배열은 데미지 값을 담는다.
                    hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
                    //sendmessage는 다른 곳에 있는 함수를 불러 올 수 있음. private도 불러올 수 있음.
                    //광선에 맞은 오브젝트의 함수를 호출하면서 매개변수(_params) 값을 전달한다.
                    //마지막 조건은 OnDamage라는 함수가 없거나 그 함수를 호출 할 수 없더라도 오류가 발생하지 않도록 하는 명령어.
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
                    _params[0] = FirePos.position;//발사 위치
                    _params[1] = hit.point;//맞은 위치
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
        curType = (WeaponType)((int)++curType % 2); //체인지 버튼이 눌리면 curType의 변수를 1개 늘린다.
        //지금은 웨폰의 개수가 2개이고 시작 인덱스가 0이기때문에 2로 나눠 나머지 값으로 curType 변수 결정
        weaponImg.sprite = weaponIcon[(int)curType]; //위에서 결정된 curType의 인덱스를 int로 형변환하여 해당 sprite를 출력
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
