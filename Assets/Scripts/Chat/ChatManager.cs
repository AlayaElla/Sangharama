using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour {

    public struct ChatConfig
    {
        public string Languege;
        public float speed;
    }
    public struct ChatActionBox
    {
        public ArrayList ActionList;
        public Dictionary<string, ChatAction.StoryCharacter> CharacterList;
        public string[] BG;
        public int NowIndex;
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
    Dictionary<string, RectTransform> CharacterRects;


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
        DoingAction(NowStroyActionBox.NowIndex);
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
        LeanTween.alpha(BGLayer, 1, 0.5f); 
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

        TextBoardLayer.WordsBacklayer.localScale = new Vector2(1, 0);
        TextBoardLayer.NameText.text = "";
        TextBoardLayer.WordsText.text = "";

    }

    void DoingAction(int index)
    {
        //如果完成所有动作
        if (index >= NowStroyActionBox.ActionList.Count)
        {
            return;
        }

        ChatAction.StoryAction action = (ChatAction.StoryAction)NowStroyActionBox.ActionList[index];
        switch (action.Command)
        {
            case "show":
                SetActionState(ChatAction.NOWSTATE.DOING, index);
                RectTransform character = GetCharacterRectTransform(action.CharacterID);
                character.localPosition = new Vector2(CharacterLayer.rect.width * float.Parse(action.Parameter[0]), CharacterLayer.rect.height * float.Parse(action.Parameter[1]));
                character.transform.SetSiblingIndex(int.Parse(action.Parameter[3]));
                SetCharacterSprite(action.CharacterID, action.Parameter[5]);

                if (action.Parameter[4] == "left")
                {
                    character.localScale = new Vector2(character.localScale.x * -1, character.localScale.y);
                }

                if (action.SkipType == ChatAction.SKIPTYPE.AUTO)
                {
                    LeanTween.alpha(character, 1, float.Parse(action.Parameter[2])).setOnComplete(() =>
                    { 
                        SetActionState(ChatAction.NOWSTATE.DONE, index);
                        SetActionIndex(index + 1);
                        DoingAction(NowStroyActionBox.NowIndex);
                        return;
                    });

                }
                else if (action.SkipType == ChatAction.SKIPTYPE.SAMETIME)
                {
                    LeanTween.alpha(character, 1, float.Parse(action.Parameter[2])).setOnComplete(() =>
                    {
                        SetActionState(ChatAction.NOWSTATE.DONE, index);
                    });
                    SetActionIndex(index + 1);
                    DoingAction(NowStroyActionBox.NowIndex);
                    return;
                }
                else
                {
                    LeanTween.alpha(character, 1, float.Parse(action.Parameter[2])).setOnComplete(() =>
                    {
                        SetActionState(ChatAction.NOWSTATE.DONE, index);
                    }); ;
                    SetActionIndex(index + 1);
                }
                break;
            case "hide":
                break;
            case "move":
                break;
            case "scale":
                break;
            case "rotate":
                break;
            case "setwindows":
                break;
            case "windowsmove":
                break;
            case "windowsscale":
                break;
            case "bgmove":
                break;
            case "bgscale":
                break;
            case "screenshake":
                break;
            case "talk":
                if (TextBoardLayer.NameText.text != NowStroyActionBox.CharacterList[action.CharacterID].Name)
                {
                    //改变窗口
                    SetChatWindow(action);
                    return;
                }
                action.NowState = ChatAction.NOWSTATE.DOING;
                int wordslengh = TextBoardLayer.WordsText.text.Length;
                string origText = TextBoardLayer.WordsText.text + action.Parameter[0];
                float speed = float.Parse(action.Parameter[1]);
                string face = action.Parameter[2];

                //设置角色表情
                SetCharacterSprite(action.CharacterID, face);
                //区分不同的动作方式
                if (action.SkipType == ChatAction.SKIPTYPE.AUTO)
                {
                    LeanTween.value(TextBoardLayer.WordsText.gameObject, wordslengh, (float)origText.Length, speed * origText.Length).setOnUpdate((float val) =>
                    {
                        TextBoardLayer.WordsText.text = origText.Substring(0, Mathf.RoundToInt(val));
                    }).setOnComplete(() =>
                    {
                        SetActionState(ChatAction.NOWSTATE.DONE, index);
                        SetActionIndex(index + 1);
                        DoingAction(NowStroyActionBox.NowIndex);
                        return;
                    });
                }
                else if (action.SkipType == ChatAction.SKIPTYPE.SAMETIME)
                {
                    LeanTween.value(TextBoardLayer.WordsText.gameObject, wordslengh, (float)origText.Length, speed * origText.Length).setOnUpdate((float val) =>
                    {
                        TextBoardLayer.WordsText.text = origText.Substring(0, Mathf.RoundToInt(val));
                    }).setOnComplete(() =>
                    {
                        SetActionState(ChatAction.NOWSTATE.DONE, index);
                    });
                    SetActionIndex(index + 1);
                    DoingAction(NowStroyActionBox.NowIndex);
                    return;
                }
                else
                {
                    LeanTween.value(TextBoardLayer.WordsText.gameObject, wordslengh, (float)origText.Length, speed * origText.Length).setOnUpdate((float val) =>
                    {
                        TextBoardLayer.WordsText.text = origText.Substring(0, Mathf.RoundToInt(val));
                    }).setOnComplete(() =>
                    {
                        SetActionState(ChatAction.NOWSTATE.DONE, index);
                    });
                    SetActionIndex(index + 1);
                }
                break;
            default:
                break;
        }
    }

    void SetActionIndex(int index)
    {
        NowStroyActionBox.NowIndex = index;
    }

    void SetActionState(ChatAction.NOWSTATE state, int index)
    {
        ChatAction.StoryAction action = (ChatAction.StoryAction)NowStroyActionBox.ActionList[index];
        action.NowState = state;
        NowStroyActionBox.ActionList[index] = action;
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

    //获取角色RectTransform
    RectTransform GetCharacterRectTransform(string id)
    {
        if (id == null)
            return null;

        RectTransform c;
        if (CharacterRects == null)
        {
            CharacterRects = new Dictionary<string, RectTransform>();
        }
        if (CharacterRects.TryGetValue(id, out c))
        {
            return c;
        }
        else
        {
            GameObject obj = new GameObject();
            obj.name = id;
            obj.transform.SetParent(CharacterLayer);
            c = obj.AddComponent<RectTransform>();
            c.pivot = new Vector2(0.5f, 0);

            Image img = obj.AddComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);

            CharacterRects.Add(id, c);

            return c;
        }
    }

    //更改图片
    void SetCharacterSprite(string id,string name)
    {
        RectTransform rt = new RectTransform();
        if (CharacterRects.TryGetValue(id, out rt))
        {
            Image objimg = rt.GetComponent<Image>();
            objimg.sprite = NowResourcesBox.characterSprites[id][name];
            objimg.SetNativeSize();
        }
        else
        {
            Debug.Log("can't find " + id + ":" + name);
        }
    }

    //更改窗口
    void SetChatWindow(ChatAction.StoryAction action)
    {
        LeanTween.scaleY(TextBoardLayer.WordsBacklayer.gameObject, 0, 0.5f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            Sprite[] s = GetWindowsSprit(action.CharacterID);
            TextBoardLayer.WordsBacklayer.GetComponent<Image>().sprite = s[0];
            TextBoardLayer.WordsOutLayer.GetComponent<Image>().sprite = s[1];

            TextBoardLayer.NameBackLayer.GetComponent<Image>().sprite = s[0];
            TextBoardLayer.NameOutLayer.GetComponent<Image>().sprite = s[1];

            TextBoardLayer.WordsText.text = "";
            TextBoardLayer.NameText.text = NowStroyActionBox.CharacterList[action.CharacterID].Name;

            LeanTween.scaleY(TextBoardLayer.WordsBacklayer.gameObject, 1, 0.25f).setOnComplete(() =>
                {
                    DoingAction(NowStroyActionBox.NowIndex);
                });
        });
    }
}
