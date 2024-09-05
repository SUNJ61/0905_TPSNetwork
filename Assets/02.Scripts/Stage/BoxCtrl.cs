using UnityEngine;

public class BoxCtrl : MonoBehaviour
{
    private BoxCollider boxCol;
    private GameObject Explore;
    //private AudioSource source; //������Ŵ����� ��ü
    private AudioClip ExploreClip;

    private string ExploreSound = "Sound/grenade_exp2";
    private string TinyFlames = "TinyFlames";
    //private string bullet = "BULLET"; //�÷��̾� ���� ĳ��Ʈ�� ���, �Ⱦ��� ��.
    private string e_bullet = "E_BULLET";
    private string s_bullettag = "S_BULLET";
    void Start()
    {
        boxCol = GetComponent<BoxCollider>();
        //source = GetComponent<AudioSource>(); //������Ŵ����� ��ü
        ExploreClip = Resources.Load(ExploreSound) as AudioClip;
        Explore = Resources.Load(TinyFlames) as GameObject;
    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == e_bullet || col.gameObject.tag == s_bullettag)
        {
            Vector3 hitPos = col.transform.position;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitPos.normalized);
            GameObject eff = Instantiate(Explore, hitPos, rot);
            Destroy(eff, 1.0f);
            //Destroy(col.gameObject); //����ĳ��Ʈ�� ��ü
            col.gameObject.SetActive(false); //���� ������ �Ѿ� ��Ȱ��ȭ
            //source.PlayOneShot(ExploreClip, 1.0f); ����� �Ŵ����� ��ü
            SoundManager.S_instance.PlaySound(hitPos, ExploreClip);
        }
    }
    void OnHit(object[] _params)
    {
        Vector3 hitpos = (Vector3)_params[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitpos.normalized);
        GameObject eff = Instantiate(Explore, hitpos, rot);
        Destroy(eff, 1.0f);
        SoundManager.S_instance.PlaySound(hitpos, ExploreClip);
    }
}
