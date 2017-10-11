using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

    public GameObject charPrefab;
    public GameObject[] startPiont;

    public ArrayList characterList;
    ArrayList skinList;

    Dictionary<string, Sprite[]> CharacterSprites = new Dictionary<string, Sprite[]>();
    float timer = 0;
    public float spawnWait = 5;
    public int characterID = 0; //添加测试模式characterID为-1时随机角色

    public float cSpeed = 0.01f;

    static bool isInStory = false;

	// Use this for initialization
	void Start () {
        XmlTool xl = new XmlTool();
        characterList = xl.loadCharacterXmlToArray();
        skinList = xl.loadSkinXmlToArray();

        //Sprite[] tempSprite = Resources.LoadAll<Sprite>("Texture/character");
        foreach (CharacterModle.Skin skin in skinList)
        {
            Sprite[] tempSprite = Resources.LoadAll<Sprite>("Texture/character/" + skin.SkinName);
            CharacterSprites.Add(skin.SkinName, tempSprite);
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (isInStory)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnAllCharacter();
            timer = spawnWait;
        }
	}

    public static void ChangeStoryState()
    {
        isInStory = !isInStory;
    }

    void SpawnAllCharacter()
    {
        ArrayList cList = characterList;
        Spawn(cList);
    }

    void Spawn(ArrayList List)
    {
        int max = 0;

        foreach (CharacterModle.CharacterInfo c in List)
        {
            max += c.Weight;
        }

        int point = Random.Range(1, max);
        int check = 0;

        //-1为测试模式
        if (characterID != -1)
        {
            CreateCharacter(List[characterID] as CharacterModle.CharacterInfo);
        }
        else
        {
            foreach (CharacterModle.CharacterInfo c in List)
            {
                check += c.Weight;

                if (check >= point)
                {
                    CreateCharacter(c);
                    return;
                }
            }
        }

    }

    void CreateCharacter(CharacterModle.CharacterInfo c)
    {
        int range = Random.Range(0, 2);
        Vector3 _start = startPiont[range].transform.position;
        //设定角色出生点
        c.startPiont = range;

        GameObject _char = Instantiate(charPrefab);
        _char.transform.position = _start;
        Rigidbody2D rigidbody2d = _char.AddComponent<Rigidbody2D>();
        rigidbody2d.isKinematic = true;
        _char.AddComponent<BoxCollider2D>().isTrigger = true;

        //Dictionary<string, int[]> _actionList = new Dictionary<string, int[]>();
        CharacterModle.Skin skin = new CharacterModle.Skin();
        foreach (CharacterModle.Skin s in skinList)
        {
            if (s.SkinName == c.Skin)
            {
                skin = s;
            }
        }
        AniController.Get(_char).AddSprite(CharacterSprites[c.Skin]);
  
        MyCharacterController.Get(_char).Init(c, skin);
        MyCharacterController.Get(_char).speed = cSpeed;
        
        if (c.startPiont == 0)
            MyCharacterController.Get(_char).SetTarget("finish2");
        else
            MyCharacterController.Get(_char).SetTarget("finish1");
    }
}
