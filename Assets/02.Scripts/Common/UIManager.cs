using UnityEngine;
//using UnityEngine.SceneManagement; //�̱۰��ӿ����� �ʿ�
using UnityEditor;

public class UIManager : MonoBehaviour
{
    #region �̱۰��� ����
    //public void OnClickPlayBtn() //��ư�� �������� �� ���� ����
    //{ //��Ʈ��ũ �������� �ٲٸ鼭 ��� x
    //    #region �ε����� (�ش��� �ε������ ����)
    //    //SceneManager.LoadScene("Level_1"); //Level_1���� �ҷ��´�.
    //    //SceneManager.LoadScene("BattleFieldScene",LoadSceneMode.Additive);//LoadSceneMode.Additive��
    //    //������(Level_1��)�� �������� �ʰ� BattleFieldScene���� �߰��ؼ� ���ο� ���� �ε��Ѵ�. (��, �� ����)
    //    //LoadSceneMode.Single�� ���� ���� �����ϰ� ()�ȿ� ����� ���ο� ���� �ε��Ѵ�.
    //    #endregion
    //    SceneManager.LoadScene("SceneLoader");
    //}
    #endregion
    public void OnClickQuitBtn()
    {
#if UNITY_EDITOR
        //�������� ���ø����̼� ����
        EditorApplication.isPlaying = false; //����Ƽ���� ����
#else
    Application.Quit(); // ����Ϸ��� ���� �� ȭ�鿡�� ����, �������� ����
#endif
    }
}
