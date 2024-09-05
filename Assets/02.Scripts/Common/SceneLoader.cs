using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private CanvasGroup fadeCG;
    [Range(0.5f, 2.0f)]public float fadeDuration = 1.0f; //���ϴ� �ð�
    private Dictionary<string, LoadSceneMode> loadScenes = new Dictionary<string, LoadSceneMode>();
    //Array������ hashtag, List������ ��ųʸ�
    void InitSceneInfo()
    {
        loadScenes.Add("Level_1", LoadSceneMode.Additive); //���ε�� �Ѱ��� ��Ƶξ�� �Ѵ�.
        loadScenes.Add("BattleFieldScene", LoadSceneMode.Additive); //���� ��ġ�°� ��Ը� ������Ʈ������ �ſ� �������� ����.
    }
    IEnumerator Start()
    {
        fadeCG = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<CanvasGroup>();
        fadeCG.alpha = 1.0f; //ó�� ���İ� 1.0. ��, ��������.
        InitSceneInfo(); //�ε��� ��� ���� ��ųʸ��� ��Ƶδ� �Լ�
        foreach (var scene in loadScenes) //��ųʸ��� ��Ƶ� ���� ��� StartCoroutine �Լ��� �ε��Ѵ�.
        {
            yield return StartCoroutine(LoadScene(scene.Key, scene.Value)); //yield return���� StartCoroutine�Լ� ��ȯ
        }

        StartCoroutine(Fade(0.0f));
    }
    IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {//���� : ���۾��� ���ʷ� ���� , �񵿱� : �ٸ� �۾��� �������̿��� �ԷµǴ� �۾��� ���� ����(���� ����)
        yield return SceneManager.LoadSceneAsync(sceneName, mode); //�񵿱� ������� ���� �ε��ϰ� �ε尡 �Ϸ� �� �� ���� ���.

        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1); //ȣ��� ���� 
        SceneManager.SetActiveScene(loadedScene); //ȣ��� ���� Ȱ��ȭ �Ѵ�.
    }//��, ��ųʸ��� ����� ���� ���������� ��� �ε� �ǰ� �ִ�.
    IEnumerator Fade(float finalAlpah)
    {
        //����Ʈ ���� ������ ���� �����ϱ� ���� �������� ���� Ȱ��ȭ �Ѵ�.
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));
        fadeCG.blocksRaycasts = true; //�� �̹����� �̺�Ʈ �߻��� ���´�.
        float fadeSpeed = Mathf.Abs(fadeCG.alpha - finalAlpah) / fadeDuration; //���밪 �Լ��� ����� ���.

        while (!Mathf.Approximately(fadeCG.alpha, finalAlpah))
        {// fadeCG.alpha�� finalAlpah�� ���� �������� �ʾҴٸ�
            fadeCG.alpha = Mathf.MoveTowards(fadeCG.alpha, finalAlpah, fadeSpeed * Time.deltaTime);
            //fadeCG.alpha�� ���� finalAlpah�� ������ fadeSpeed�� �ӵ��� ��ȭ ��Ų��. (�����ϰ� ��ȭ��Ŵ)
            yield return null; //�������� ����.
        }
        fadeCG.blocksRaycasts = false; //�� �̹����� �̺�Ʈ �߻� ���.
        SceneManager.UnloadSceneAsync("SceneLoader");//SceneLoader�� �����Ѵ�. (������ �񵿱������� FadeIn�� �Ϸ��� ����.)
    }
}
