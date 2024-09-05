using UnityEngine;
/*
플레이가 되었을 때 배럴 색상이 랜덤하게 바뀌는 것을 만든다.
5번 이상 배럴이 총알에 맞으면 폭파 물리현상을 구현하기.
*/
public class BarrelCtrl : MonoBehaviour
{
    [SerializeField] private Texture[] textures;
    private MeshRenderer meshRenderer;
    private Rigidbody rb;
    private GameObject Effect;
    private AudioClip Expclip;
    private MeshFilter filter; //드럼통 찌그러짐 표현하기.
    private Mesh[] meshes; //찌그러진 메쉬를 넣기 위한 배열. 

    private int HitCount = 0;
    private readonly string BarTexture = "BarrelTextures";
    private readonly string bulletStr = "BULLET";
    private readonly string EffStr = "ExpEffect";
    private readonly string ExploreSound = "Sound/grenade_exp2";
    private readonly string e_bullettag = "E_BULLET";
    private readonly string s_bullettag = "S_BULLET";
    private readonly string meshestag = "Meshes";
    void Start()
    {
        textures = Resources.LoadAll<Texture>(BarTexture); //형변환 방법중 하나, as를 쓰지 않아도 됨, 여기선 as쓰면 안잡힘
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        Effect = Resources.Load(EffStr) as GameObject;
        meshRenderer.material.mainTexture = textures[Random.Range(0,textures.Length)];
        Expclip = Resources.Load(ExploreSound) as AudioClip;

        filter = GetComponent<MeshFilter>();
        meshes = Resources.LoadAll<Mesh>(meshestag);
    }
    #region 프로젝타일 방식의 충돌감지(주석 안했음)
    private void OnCollisionEnter(Collision col) //프로젝타일 방식의 충돌감지
    {
        if(col.gameObject.tag == bulletStr || col.gameObject.CompareTag(e_bullettag) || col.gameObject.tag == s_bullettag)
        {
            if(++HitCount == 5)
            {
                ExplosionBarrel();
            }
        }
    }
    #endregion
    void OnHit(object[] _params)
    {
        Vector3 firepos = (Vector3)_params[0]; //발사 위치
        Vector3 hitpos = (Vector3)_params[1]; //맞은 위치
        Vector3 incomeVector = hitpos - firepos; //거리와 방향 값이 저장됨. 즉, 입사벡터 (Ray의 각도)를 구하기위해 사용.
        incomeVector = incomeVector.normalized; //정규화 시켜 방향 정보만 남긴다.
        GetComponent<Rigidbody>().AddForceAtPosition(incomeVector * 1500f, hitpos);//Ray의 hit좌표에 입사벡터의 각도로 힘 생성.
        //어떤지점에 힘을 모아서 물리가 생성되게 하려고 할 때 호출 되는 메소드.(맞은 위치에서 입사벡터 방향으로 1500의 물리 발생.)
        if (++HitCount == 5)
        {
            ExplosionBarrel();
        }
    }
    private void ExplosionBarrel()
    {
        Vector3 hitPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        GameObject eff = Instantiate(Effect, transform.position, Quaternion.identity); //심화 : 이것도 오브젝트 풀링으로 제작
        Destroy(eff, 2.0f); //폭파 파티클 생성후 파괴
        SoundManager.S_instance.PlaySound(hitPos, Expclip);
        CameraCtrl.C_instance.CameraMoveOn();

        Collider[] Cols = Physics.OverlapSphere(transform.position, 20f, 1 << 7 | 1 << 13 | 1 << 15);
        //배럴 자기자신 위치 20반경안에 있는 배럴 레이어(7번 레이어)만 Cols배열에 담는다.
        foreach (Collider col in Cols) //위에서 담은 배럴 콜라이더를 다 꺼낸다.
        {
            Rigidbody rigidbody = col.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = 1.0f;//배럴 무게를 가볍게 바꾼다.
                rigidbody.AddExplosionForce(1000, transform.position, 20.0f, 1200f);//리깃바디에서 지원하는 폭파 함수
                col.gameObject.SendMessage("ExpDie", col, SendMessageOptions.DontRequireReceiver);
            }//AddExplosionForce(폭파력, 폭파위치, 폭파반경, 위로 솟구치는 힘) 으로 사용
            Invoke("BarrelMassChange", 3.0f);
        }//즉, 주변 반경에 있는 배럴들이 다 터진다.
        int Idx = Random.Range(0, meshes.Length);
        filter.sharedMesh = meshes[Idx]; //가져온 메쉬필터중 랜덤으로 1개뽑아서 바꾼다.
        GetComponent<MeshCollider>().sharedMesh = meshes[Idx]; //찌그러진 형태대로 메쉬 콜라이더 변경.

        //Collider[] Mobs = Physics.OverlapSphere(transform.position, 20f, 1 << 13 | 1 << 15);
        ////배럴 자기자신 위치에 20반경안에 있는 swat과 enemy를 전부 Mobs 배열(콜라이더)에 담는다.
        //foreach(Collider Mob in Mobs)
        //{
        //    if (Mob != null)
        //        Mob.gameObject.SendMessage("Die", Mob);
        //}
    }
    private void BarrelMassChange()
    {
        Collider[] Cols = Physics.OverlapSphere(transform.position, 20f, 1 << 7);
        //배럴 자기자신 위치 20반경안에 있는 배럴 레이어(7번 레이어)만 Cols배열에 담는다.
        foreach (Collider col in Cols) //위에서 담은 배럴 콜라이더를 다 꺼낸다.
        {
            Rigidbody rigidbody = col.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = 60.0f;//배럴 무게를 무겁게 바꾼다.
            }
        }
    }
}
