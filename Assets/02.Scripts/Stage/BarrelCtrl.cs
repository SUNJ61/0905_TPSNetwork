using UnityEngine;
/*
�÷��̰� �Ǿ��� �� �跲 ������ �����ϰ� �ٲ�� ���� �����.
5�� �̻� �跲�� �Ѿ˿� ������ ���� ���������� �����ϱ�.
*/
public class BarrelCtrl : MonoBehaviour
{
    [SerializeField] private Texture[] textures;
    private MeshRenderer meshRenderer;
    private Rigidbody rb;
    private GameObject Effect;
    private AudioClip Expclip;
    private MeshFilter filter; //�巳�� ��׷��� ǥ���ϱ�.
    private Mesh[] meshes; //��׷��� �޽��� �ֱ� ���� �迭. 

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
        textures = Resources.LoadAll<Texture>(BarTexture); //����ȯ ����� �ϳ�, as�� ���� �ʾƵ� ��, ���⼱ as���� ������
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        Effect = Resources.Load(EffStr) as GameObject;
        meshRenderer.material.mainTexture = textures[Random.Range(0,textures.Length)];
        Expclip = Resources.Load(ExploreSound) as AudioClip;

        filter = GetComponent<MeshFilter>();
        meshes = Resources.LoadAll<Mesh>(meshestag);
    }
    #region ������Ÿ�� ����� �浹����(�ּ� ������)
    private void OnCollisionEnter(Collision col) //������Ÿ�� ����� �浹����
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
        Vector3 firepos = (Vector3)_params[0]; //�߻� ��ġ
        Vector3 hitpos = (Vector3)_params[1]; //���� ��ġ
        Vector3 incomeVector = hitpos - firepos; //�Ÿ��� ���� ���� �����. ��, �Ի纤�� (Ray�� ����)�� ���ϱ����� ���.
        incomeVector = incomeVector.normalized; //����ȭ ���� ���� ������ �����.
        GetComponent<Rigidbody>().AddForceAtPosition(incomeVector * 1500f, hitpos);//Ray�� hit��ǥ�� �Ի纤���� ������ �� ����.
        //������� ���� ��Ƽ� ������ �����ǰ� �Ϸ��� �� �� ȣ�� �Ǵ� �޼ҵ�.(���� ��ġ���� �Ի纤�� �������� 1500�� ���� �߻�.)
        if (++HitCount == 5)
        {
            ExplosionBarrel();
        }
    }
    private void ExplosionBarrel()
    {
        Vector3 hitPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        GameObject eff = Instantiate(Effect, transform.position, Quaternion.identity); //��ȭ : �̰͵� ������Ʈ Ǯ������ ����
        Destroy(eff, 2.0f); //���� ��ƼŬ ������ �ı�
        SoundManager.S_instance.PlaySound(hitPos, Expclip);
        CameraCtrl.C_instance.CameraMoveOn();

        Collider[] Cols = Physics.OverlapSphere(transform.position, 20f, 1 << 7 | 1 << 13 | 1 << 15);
        //�跲 �ڱ��ڽ� ��ġ 20�ݰ�ȿ� �ִ� �跲 ���̾�(7�� ���̾�)�� Cols�迭�� ��´�.
        foreach (Collider col in Cols) //������ ���� �跲 �ݶ��̴��� �� ������.
        {
            Rigidbody rigidbody = col.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = 1.0f;//�跲 ���Ը� ������ �ٲ۴�.
                rigidbody.AddExplosionForce(1000, transform.position, 20.0f, 1200f);//����ٵ𿡼� �����ϴ� ���� �Լ�
                col.gameObject.SendMessage("ExpDie", col, SendMessageOptions.DontRequireReceiver);
            }//AddExplosionForce(���ķ�, ������ġ, ���Ĺݰ�, ���� �ڱ�ġ�� ��) ���� ���
            Invoke("BarrelMassChange", 3.0f);
        }//��, �ֺ� �ݰ濡 �ִ� �跲���� �� ������.
        int Idx = Random.Range(0, meshes.Length);
        filter.sharedMesh = meshes[Idx]; //������ �޽������� �������� 1���̾Ƽ� �ٲ۴�.
        GetComponent<MeshCollider>().sharedMesh = meshes[Idx]; //��׷��� ���´�� �޽� �ݶ��̴� ����.

        //Collider[] Mobs = Physics.OverlapSphere(transform.position, 20f, 1 << 13 | 1 << 15);
        ////�跲 �ڱ��ڽ� ��ġ�� 20�ݰ�ȿ� �ִ� swat�� enemy�� ���� Mobs �迭(�ݶ��̴�)�� ��´�.
        //foreach(Collider Mob in Mobs)
        //{
        //    if (Mob != null)
        //        Mob.gameObject.SendMessage("Die", Mob);
        //}
    }
    private void BarrelMassChange()
    {
        Collider[] Cols = Physics.OverlapSphere(transform.position, 20f, 1 << 7);
        //�跲 �ڱ��ڽ� ��ġ 20�ݰ�ȿ� �ִ� �跲 ���̾�(7�� ���̾�)�� Cols�迭�� ��´�.
        foreach (Collider col in Cols) //������ ���� �跲 �ݶ��̴��� �� ������.
        {
            Rigidbody rigidbody = col.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = 60.0f;//�跲 ���Ը� ���̰� �ٲ۴�.
            }
        }
    }
}
