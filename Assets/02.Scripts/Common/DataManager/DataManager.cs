using System.IO; // ���̳ʸ� ���� ������� ���� ���ӽ����̽�, �����͸� ��ȯ �ϱ����� ����� stream�� ����� ���� ���.
using System.Runtime.Serialization.Formatters.Binary; //���̳ʸ� ���� ������ �ǽð����� ����ȭ�� ����� �����ϱ� ���� ���.
using UnityEngine;//����ȭ�� �ڵ忡�� ����� �ڷ�����(int, string, float)�� byte���� �������� �ٲٴ� ���� ���Ѵ�.
using DataInfo;

public class DataManager : MonoBehaviour
{
    [SerializeField] string dataPath; //���� ���
    public void Initialized() //�����θ� �ʱ�ȭ �ϴ� �Լ�
    {
        dataPath = Application.persistentDataPath + "/gameData.dat"; //���� �����ο� ���ϸ�,Ȯ���� ����
        //Application.persistentDataPath ���������� gameData.dat�� �̸��� ���� datȮ���� ���Ϸ� ����
    }
    public void Save(GameData gamedata)
    {
        BinaryFormatter bf = new BinaryFormatter();//���̳ʸ� ���� ������ ���� BinaryFormatter ����
        FileStream file = File.Create(dataPath);//������ ������ ���� ���� ����. file�� ������� ������ ������ �����Ѵ�.

        GameData data = new GameData(); //���Ͽ� ������ �����͸� Ŭ������ �Ҵ��Ѵ�.
        data.KillCount = gamedata.KillCount; //�ܺο��� ���� ������ ���� Ŭ���� ������ �ٽ� �����Ѵ�.
        data.hp = gamedata.hp;
        data.speed = gamedata.speed;
        data.equipItem = gamedata.equipItem;
        bf.Serialize(file, data); //file�� data������ ����ȭ ������ ���� �����Ѵ�.
        file.Close(); //���� ��Ʈ���� �ݴ´�. ���� �ʰ� ���� ������ �����ϸ� �޸𸮰� ����� ���� ���� ��ä�� ������ ����.
    }

    public GameData Load()
    {
        if(File.Exists(dataPath))//�ش� ��ο� ������ ���� �Ѵٸ�
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open); //������ ��ο� ������ ����. file�� �ش� ���� ����
            GameData data = (GameData)bf.Deserialize(file); //������ȭ�� ���� ����� �����͸� Byte���¿��� C#�ڷ������� �ٲ۴�.

            file.Close();
            return data; //����� �����͸� ��ȯ�Ѵ�.
        }
        else //��ο� ������ �������� �ʴ´ٸ� �⺻ ���� ��ȯ
        {
            GameData data = new GameData();
            return data;
        }
    }
}
