using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager S_instance;
    public List<GameObject> SoundList;

    public float SoundVolumn = 10.0f; //소리 크기 조절 변수
    public bool isSoundeMute = false; //소리 음소거 조절 변수

    //private int S_ListMax = 20;//사운드 오브젝트를 다른 오브젝트 안에 생성하게 만들기. 아직안함

    void Awake() //스크립트 컴포넌트가 비활성화 되더라도 실행된다.
    {
        if (S_instance == null)
            S_instance = this;
        else if (S_instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySound(Vector3 pos, AudioClip clip)
    {
        if (isSoundeMute) return;  //음소거 일시 즉시 함수 탈출

        GameObject soundObj = new GameObject("Sound!"); //soundObj 게임오브젝트를 하나 동적 생성
        soundObj.transform.position = pos; //soundObj 게임오브젝트의 생성 위치는 전달 받은 pos값.
        AudioSource audioSource = soundObj.AddComponent<AudioSource>(); //soundObj 게임오브젝트에 오디오소스 컴퍼넌트를 생성한다.
        audioSource.clip = clip; //soundObj 게임오브젝트의 기본 오디오 클립을 받아온 오디오 클립으로 설정.
        audioSource.minDistance = 20f; // soundObj 게임오브젝트의 최소 소리 범위를 20으로 설정
        audioSource.maxDistance = 50f; // soundObj 게임오브젝트의 최대 소리 범위를 50으로 설정
        audioSource.volume = SoundVolumn; //soundObj 게임오브젝트의 소리 출력 크키를 설정.ㅠ
        audioSource.Play(); //소리 재생
        Destroy(soundObj, clip.length); //소리 클립의 길이 만큼 출력후 삭제.
    }
}
