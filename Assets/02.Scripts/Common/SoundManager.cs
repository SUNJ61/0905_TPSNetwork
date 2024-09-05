using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager S_instance;
    public List<GameObject> SoundList;

    public float SoundVolumn = 10.0f; //�Ҹ� ũ�� ���� ����
    public bool isSoundeMute = false; //�Ҹ� ���Ұ� ���� ����

    //private int S_ListMax = 20;//���� ������Ʈ�� �ٸ� ������Ʈ �ȿ� �����ϰ� �����. ��������

    void Awake() //��ũ��Ʈ ������Ʈ�� ��Ȱ��ȭ �Ǵ��� ����ȴ�.
    {
        if (S_instance == null)
            S_instance = this;
        else if (S_instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySound(Vector3 pos, AudioClip clip)
    {
        if (isSoundeMute) return;  //���Ұ� �Ͻ� ��� �Լ� Ż��

        GameObject soundObj = new GameObject("Sound!"); //soundObj ���ӿ�����Ʈ�� �ϳ� ���� ����
        soundObj.transform.position = pos; //soundObj ���ӿ�����Ʈ�� ���� ��ġ�� ���� ���� pos��.
        AudioSource audioSource = soundObj.AddComponent<AudioSource>(); //soundObj ���ӿ�����Ʈ�� ������ҽ� ���۳�Ʈ�� �����Ѵ�.
        audioSource.clip = clip; //soundObj ���ӿ�����Ʈ�� �⺻ ����� Ŭ���� �޾ƿ� ����� Ŭ������ ����.
        audioSource.minDistance = 20f; // soundObj ���ӿ�����Ʈ�� �ּ� �Ҹ� ������ 20���� ����
        audioSource.maxDistance = 50f; // soundObj ���ӿ�����Ʈ�� �ִ� �Ҹ� ������ 50���� ����
        audioSource.volume = SoundVolumn; //soundObj ���ӿ�����Ʈ�� �Ҹ� ��� ũŰ�� ����.��
        audioSource.Play(); //�Ҹ� ���
        Destroy(soundObj, clip.length); //�Ҹ� Ŭ���� ���� ��ŭ ����� ����.
    }
}
