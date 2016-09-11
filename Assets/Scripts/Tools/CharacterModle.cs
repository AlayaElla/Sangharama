using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterModle : MonoBehaviour {

    public ArrayList characterList;
    ArrayList skinList;

    Dictionary<string, Sprite[]> CharacterSprites = new Dictionary<string, Sprite[]>();
    float timer = 0;
    public int characterID = 0; //添加测试模式characterID为-1时随机角色

    public enum TimeType { Day, Noon, Night };
    public enum SexType { Male, Female };

    public float cSpeed = 0.01f;

    public class Skin
    {
        public string SkinName;
        public Dictionary<string, int[]> ActionList = new Dictionary<string, int[]>();
    }

    public class CharacterInfo
    {
        public int ID;
        public string Name;
        public string Skin;

        public SexType Sex;
        public TimeType OutTime;

        public int[] FavoriteItems;
        public int[] FavoriteMinds;

        public int Weight;

        public string Des;

        public int startPiont;
    }


    // Use this for initialization
    void Awake()
    {
        XmlTool xl = new XmlTool();
        characterList = xl.loadCharacterXmlToArray();
        skinList = xl.loadSkinXmlToArray();

        //Sprite[] tempSprite = Resources.LoadAll<Sprite>("Texture/character");
        foreach (Skin skin in skinList)
        {
            Sprite[] tempSprite = Resources.LoadAll<Sprite>("Texture/character/" + skin.SkinName);
            CharacterSprites.Add(skin.SkinName, tempSprite);
        }
    }

    public Sprite[] GetSkinSprite(string skin)
    {
        return CharacterSprites[skin];
    }

    public Skin GetSkin(string skin)
    {
        foreach (Skin _skin in skinList)
        {
            if (_skin.SkinName == skin)
                return _skin;
        }
        Skin wrong = new Skin();
        wrong.SkinName = "don't have this skin:" + skin;
        Debug.Log("don't have this skin:" + skin);
        return wrong;
    }
}
