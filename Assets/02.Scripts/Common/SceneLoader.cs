using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private CanvasGroup fadeCG;
    [Range(0.5f, 2.0f)]public float fadeDuration = 1.0f; //변하는 시간
    private Dictionary<string, LoadSceneMode> loadScenes = new Dictionary<string, LoadSceneMode>();
    //Array에서는 hashtag, List에서는 딕셔너리
    void InitSceneInfo()
    {
        loadScenes.Add("Level_1", LoadSceneMode.Additive); //씬로드는 한곳에 모아두어야 한다.
        loadScenes.Add("BattleFieldScene", LoadSceneMode.Additive); //씬을 합치는게 대규모 프로젝트에서는 매우 많아지기 때문.
    }
    IEnumerator Start()
    {
        fadeCG = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<CanvasGroup>();
        fadeCG.alpha = 1.0f; //처음 알파값 1.0. 즉, 불투명함.
        InitSceneInfo(); //로드할 모든 씬을 딕셔너리에 모아두는 함수
        foreach (var scene in loadScenes) //딕셔너리에 모아둔 씬을 모두 StartCoroutine 함수로 로드한다.
        {
            yield return StartCoroutine(LoadScene(scene.Key, scene.Value)); //yield return으로 StartCoroutine함수 반환
        }

        StartCoroutine(Fade(0.0f));
    }
    IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {//동기 : 한작업씩 차례로 진행 , 비동기 : 다른 작업이 진행중이여도 입력되는 작업들 동시 진행(독립 시행)
        yield return SceneManager.LoadSceneAsync(sceneName, mode); //비동기 방식으로 씬을 로드하고 로드가 완료 될 때 까지 대기.

        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1); //호출된 씬을 
        SceneManager.SetActiveScene(loadedScene); //호출된 씬을 활성화 한다.
    }//즉, 딕셔너리에 저장된 씬이 독립적으로 계속 로드 되고 있다.
    IEnumerator Fade(float finalAlpah)
    {
        //라이트 맵이 깨지는 것을 방지하기 위해 스테이지 씬을 활성화 한다.
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));
        fadeCG.blocksRaycasts = true; //씬 이미지에 이벤트 발생을 막는다.
        float fadeSpeed = Mathf.Abs(fadeCG.alpha - finalAlpah) / fadeDuration; //절대값 함수로 백분율 계산.

        while (!Mathf.Approximately(fadeCG.alpha, finalAlpah))
        {// fadeCG.alpha가 finalAlpah의 값과 같아지지 않았다면
            fadeCG.alpha = Mathf.MoveTowards(fadeCG.alpha, finalAlpah, fadeSpeed * Time.deltaTime);
            //fadeCG.alpha의 값을 finalAlpah의 값까지 fadeSpeed의 속도로 변화 시킨다. (투명하게 변화시킴)
            yield return null; //한프레임 쉰다.
        }
        fadeCG.blocksRaycasts = false; //씬 이미지의 이벤트 발생 허용.
        SceneManager.UnloadSceneAsync("SceneLoader");//SceneLoader를 삭제한다. (위에서 비동기적으로 FadeIn이 완료됬기 때문.)
    }
}
