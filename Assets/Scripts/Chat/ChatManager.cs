using UnityEngine;
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
    }
    struct ResourcesBox
    {
        public Dictionary<string, Dictionary<string, Sprite>> windowsSprites;
        public Dictionary<string, Dictionary<string, Sprite>> characterSprites;
        public Dictionary<string, Sprite> bgSprites;
    }

    ChatConfig NowConfig;

    GameObject ChatLayer;
    ChatActionBox NowStroyActionBox;
    ResourcesBox NowResourcesBox;

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
        NowResourcesBox.windowsSprites = new Dictionary<string, Dictionary<string, Sprite>>();
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
            tempResource = new Dictionary<string, Sprite>();
            tempSprite = Resources.LoadAll<Sprite>("Texture/story/board/" + character.Value.Windows);
            foreach (Sprite s in tempSprite)
            {
                tempResource.Add(s.name, s);
            }
            NowResourcesBox.windowsSprites.Add(character.Key, tempResource);
        }

        Debug.Log("Load Story Resources Complete!");

    }

    //创建故事面板
    void CreateChatLayer()
    {
        ChatLayer = Resources.Load<GameObject>("Prefab/Chat/StoryLayer");
        ChatLayer = (GameObject)Instantiate(ChatLayer, transform.Find("/Canvas").transform);

        RectTransform chatRect = ChatLayer.GetComponent<RectTransform>();

        chatRect.sizeDelta = new Vector2(0, 0);
        chatRect.localPosition = new Vector3(0, -Screen.height / 3, 0);

        LeanTween.moveLocalY(chatRect.gameObject, 0, 0.7f).setEase(LeanTweenType.easeOutQuad); 
    }
}
