using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour {

    public struct ChatConfig
    {
        public string Languege;
        public int speed;
    }
    public struct ChatActionBox
    {
        public ArrayList ActionList;
        public Dictionary<string, ChatAction.StoryCharacter> CharacterList;
        public string[] BG;
    }
    struct ResourcesBox
    {
        public Dictionary<string, Sprite[]> windowsSprites;
        public Dictionary<string, Dictionary<string, Sprite>> characterSprites;
        public Dictionary<string, Sprite> bgSprites;
    }
    struct ChatBoard
    {
        public RectTransform WordsBacklayer;
        public RectTransform WordsOutLayer;

        public RectTransform NameBackLayer;
        public RectTransform NameOutLayer;

        public Text WordsText;
        public Text NameText;
    }

    ChatConfig NowConfig;
    ChatActionBox NowStroyActionBox;
    ResourcesBox NowResourcesBox;

    RectTransform StoryBoardLayer;

    RectTransform BGLayer;
    RectTransform CharacterLayer;
    RectTransform MaskLayer;
    ChatBoard TextBoardLayer;

    void Awake()
    {
        
    }

    // Use this for initialization
    void Start()
    {

    }

	// Update is called once per frame
	void Update () {
        //Debug.Log("chatmanager update!!!!");
	}

    //读取故事配置
    public void LoadChatStory(string storyname)
    {
        ChatLoader loader = new ChatLoader();
        NowConfig = loader.LoadNowConfig();

        NowStroyActionBox = loader.LoadStory(storyname, NowConfig);

        LoadStoryResources();
        CreateChatLayer();
    }

    //读取故事资源
    void LoadStoryResources()
    {
        NowResourcesBox.characterSprites = new Dictionary<string, Dictionary<string, Sprite>>();
        NowResourcesBox.windowsSprites = new Dictionary<string, Sprite[]>();
        NowResourcesBox.bgSprites = new Dictionary<string, Sprite>();
        Dictionary<string, Sprite> tempResource;

        //查找角色资源
        foreach (KeyValuePair<string, ChatAction.StoryCharacter> character in NowStroyActionBox.CharacterList)
        {
            Sprite[] tempSprite;
            tempResource = new Dictionary<string, Sprite>();

            //读取角色立绘
            tempSprite = Resources.LoadAll<Sprite>("Texture/story/character/" + character.Key);
            foreach (Sprite s in tempSprite)
            {
                tempResource.Add(s.name, s);
            }
            NowResourcesBox.characterSprites.Add(character.Key, tempResource);

            //读取窗口文件
            tempSprite = Resources.LoadAll<Sprite>("Texture/story/board/" + character.Value.Windows);
            NowResourcesBox.windowsSprites.Add(character.Key, tempSprite);
        }

        //读取背景
        if (NowStroyActionBox.BG != null)
        {
            foreach (string name in NowStroyActionBox.BG)
            {
                Sprite tempSprite;

                //读取角色立绘
                tempSprite = Resources.Load<Sprite>("Texture/story/bg/" + name);
                NowResourcesBox.bgSprites.Add(name, tempSprite);
            }
        }

        Debug.Log("Load Story Resources Complete!");

    }

    //创建故事面板
    void CreateChatLayer()
    {
        //创建面板
        GameObject StoryLayerObj = Resources.Load<GameObject>("Prefab/Chat/StoryLayer");
        StoryLayerObj = (GameObject)Instantiate(StoryLayerObj, transform.Find("/Canvas").transform);

        //获取组件
        GetLayerComponent(StoryLayerObj);

        //初始化故事面板
        InstChatLayer();

        //初始动作
        LeanTween.scaleY(TextBoardLayer.WordsBacklayer.gameObject, 1, 0.5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.alpha(BGLayer, 1, 3f).setEase(LeanTweenType.easeOutQuad); 
    }

    //获取组件方法
    void GetLayerComponent(GameObject StoryLayerObj)
    {
        StoryBoardLayer = StoryLayerObj.GetComponent<RectTransform>();
        MaskLayer = StoryBoardLayer.FindChild("Mask").GetComponent<RectTransform>();
        BGLayer = StoryBoardLayer.FindChild("BG").GetComponent<RectTransform>();
        CharacterLayer = StoryBoardLayer.FindChild("Character").GetComponent<RectTransform>();
        TextBoardLayer.WordsBacklayer = StoryBoardLayer.FindChild("TextBoard").GetComponent<RectTransform>();
        TextBoardLayer.WordsOutLayer = StoryBoardLayer.FindChild("TextBoard/OutBoard").GetComponent<RectTransform>();
        TextBoardLayer.WordsText = StoryBoardLayer.FindChild("TextBoard/Text").GetComponent<Text>();

        TextBoardLayer.NameBackLayer = StoryBoardLayer.FindChild("TextBoard/NameBoard").GetComponent<RectTransform>();
        TextBoardLayer.NameOutLayer = StoryBoardLayer.FindChild("TextBoard/NameBoard/OutBoard").GetComponent<RectTransform>();
        TextBoardLayer.NameText = StoryBoardLayer.FindChild("TextBoard/NameBoard/Text").GetComponent<Text>();
    }

    //初始化故事面板
    void InstChatLayer()
    {
        //修改图片
        Sprite[] s = GetWindowsSprit("rourou");
        TextBoardLayer.WordsBacklayer.GetComponent<Image>().sprite = s[0];
        TextBoardLayer.WordsOutLayer.GetComponent<Image>().sprite = s[1];

        TextBoardLayer.NameBackLayer.GetComponent<Image>().sprite = s[0];
        TextBoardLayer.NameOutLayer.GetComponent<Image>().sprite = s[1];

        //调整大小/位置
        StoryBoardLayer.sizeDelta = new Vector2(0, 0);
        StoryBoardLayer.localPosition = new Vector3(0, 0, 0);
        TextBoardLayer.WordsBacklayer.localScale = new Vector3(1, 0, 1);

        BGLayer.GetComponent<Image>().sprite = GetBGSprit("1");
        BGLayer.GetComponent<Image>().SetNativeSize();

        TextBoardLayer.NameText.text = NowStroyActionBox.CharacterList["rourou"].Name;
        TextBoardLayer.WordsText.text = "啦啦啦啦啦啦啦啦啦\n嘎嘎嘎嘎嘎！";

    }

    //获取window的图片
    Sprite[] GetWindowsSprit(string name)
    {
        if (name == null)
            return null;

        Sprite[] s;
        if (NowResourcesBox.windowsSprites.TryGetValue(name,out s))
            return s;
        return null;  //如果找不到则返回-1
    }

    //获取背景的图片
    Sprite GetBGSprit(string name)
    {
        if (name == null)
            return null;

        Sprite s;
        if (NowResourcesBox.bgSprites.TryGetValue(name, out s))
            return s;
        return null;  //如果找不到则返回-1
    }
}
