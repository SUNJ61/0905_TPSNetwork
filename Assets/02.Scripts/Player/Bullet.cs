using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform tr;
    private Rigidbody rb;
    private TrailRenderer trail;

    private float Speed = 1500.0f;
    public float Damage = 25.0f;
    void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        //Destroy(this.gameObject, 2.0f); //������Ʈ Ǯ�� �ț��� �� ���
    }
    private void BulletDisable()
    {
        this.gameObject.SetActive(false);
    }
    private void OnEnable() //������Ʈ�� ������ �� �ߵ� �Ǵ� �Լ�
    {
        Damage = GameManager.G_instance.gameData.damage;
        GameManager.OnItemChange += UpdateSetUp; //�κ��丮���� �������� �ְ� ���� �� �ߵ��Ѵ�.
        rb.AddForce(tr.forward * Speed);
        Invoke("BulletDisable", 2.0f); //2�ʵ��� ������Ʈ�� ��ȭ�� ���� ��� ������ ������Ʈ�� ����.
    }
    void UpdateSetUp()
    {
        Damage = GameManager.G_instance.gameData.damage;
    }
    private void OnDisable()
    {
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep(); //����ٵ� �۵� ����
    }
}
