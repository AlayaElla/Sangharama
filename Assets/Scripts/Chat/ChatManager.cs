using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public RectTransform ClickHintLayer;

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


    string NowScene = "";
    string lastWords = "";  //用于保存上一个动作时说话的台词，在点击时在lastword中增加当前语句，来达到点击快速完成当前对话的功能。啊，这个方法我知道有点坑!

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

        //检测鼠标点击事件
        if (Input.GetMouseButtonDown(0))
        {
            ClickToDoing();
        }

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

        //创建背景
        if (NowResourcesBox.bgSprites.Count != 0)
        {
            BGLayer.GetComponent<Image>().sprite = GetBGSprit(NowStroyActionBox.BG[0]);
            BGLayer.GetComponent<Image>().SetNativeSize();

            LeanTween.alpha(BGLayer, 1, 0.5f); 
        }

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

        TextBoardLayer.ClickHintLayer = StoryBoardLayer.FindChild("ClickHint").GetComponent<RectTransform>();
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

        TextBoardLayer.WordsBacklayer.localScale = new Vector2(1, 0);
        TextBoardLayer.NameText.text = "";
        TextBoardLayer.WordsText.text = "";

    }

    public void SetNowScene(string scene)
    {
        NowScene = scene;
    }

    //关闭故事面板
    void EndChatLayer()
    {
        LeanTween.scaleY(TextBoardLayer.WordsBacklayer.gameObject, 0, 0.25f).setOnComplete(() =>
            {
                LeanTween.alpha(CharacterLayer, 0, 1f);
                LeanTween.alpha(BGLayer, 0, 1f).setOnComplete(() =>
                {
                    Destroy(StoryBoardLayer.gameObject);
                    Destroy(this.gameObject);
                });
            });

        if (NowScene == "Shop")
        {
            ShopUI.ChangeStoryState();
            Character.ChangeStoryState();
        }
    }

    void DoingAction(int index)
    {
        //如果完成所有动作
        if (index >= NowStroyActionBox.ActionList.Count)
        {
            return;
        }

        ChatAction.StoryAction action = (ChatAction.StoryAction)NowStroyActionBox.ActionList[index];
        ChatAction.StoryAction preaction = new ChatAction.StoryAction();
        if(index>0)
            preaction = (ChatAction.StoryAction)NowStroyActionBox.ActionList[index - 1];

        switch (action.Command)
        {
            case "setbg":
                break;
            case "show":
                SetActionState(ChatAction.NOWSTATE.DOING, index);
                RectTransform character = GetCharacterRectTransform(action.CharacterID);
                character.localPosition = new Vector2(CharacterLayer.rect.width * float.Parse(action.Parameter[0]), CharacterLayer.rect.height * float.Parse(action.Parameter[1]));
                character.transform.SetSiblingIndex(int.Parse(action.Parameter[3]));
                SetCharacterSprite(action.CharacterID, action.Parameter[5]);

                if (action.Parameter[4] == "left")
                {
                    if (character.localScale.x < 0)
                        character.localScale = new Vector2(character.localScale.x * -1, character.localScale.y);
                }

                if (action.SkipType == ChatAction.SKIPTYPE.AUTO)
                {
                    LeanTween.alpha(character, 1, float.Parse(action.Parameter[2])).setOnComplete(() =>
                    {
                        //如果是最后一个动作，则停止自动
                        if (index >= NowStroyActionBox.ActionList.Count)
                        {
                            SetActionState(ChatAction.NOWSTATE.DONE, index);
                            //WaitingForClick();
                            return;
                        }

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

                        //WaitingForClick();
                    });
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
                SetActionState(ChatAction.NOWSTATE.DOING, index);

                //对话最开始
                if (preaction.Command == "talk" && preaction.Parameter[3] == "endpage")
                {
                    TextBoardLayer.WordsText.text = "";
                }

                //获取上一条对话的语句
                lastWords = TextBoardLayer.WordsText.text;

                Regex reg = new Regex("(<.*?>)(.*?)(<.*?>)", RegexOptions.IgnoreCase);
                string replacestr = reg.Replace(action.Parameter[0], @"$2");

                int wordslengh = replacestr.Length;
                string origText = TextBoardLayer.WordsText.text + action.Parameter[0];
                float speed = float.Parse(action.Parameter[1]);
                string face = action.Parameter[2];

                int strlength = origText.Length;

                //设置角色表情
                SetCharacterSprite(action.CharacterID, face);

                //区分不同的动作方式
                if (action.SkipType == ChatAction.SKIPTYPE.AUTO || action.SkipType == ChatAction.SKIPTYPE.TimeAUTO)
                {
                    LeanTween.value(TextBoardLayer.WordsText.gameObject, wordslengh, (float)origText.Length, speed * strlength).setOnUpdate((float val) =>
                    {
                        SetTextBoardWords(origText, Mathf.RoundToInt(val), action.Richparamater);
                    }).setOnComplete(() =>
                    {
                        //如果是最后一个动作，则停止自动
                        if (index >= NowStroyActionBox.ActionList.Count - 1)
                        {
                            SetActionState(ChatAction.NOWSTATE.DONE, index);
                            WaitingForClick(action.CharacterID);
                            return;
                        }

                        if (action.Parameter[3] == "endpage")
                        {
                            LeanTween.delayedCall(origText.Length * NowConfig.speed, () =>
                            {
                                SetActionState(ChatAction.NOWSTATE.DONE, index);
                                SetActionIndex(index + 1);
                                DoingAction(NowStroyActionBox.NowIndex);
                            });
                        }
                        else 
                        {
                            SetActionState(ChatAction.NOWSTATE.DONE, index);
                            SetActionIndex(index + 1);
                            DoingAction(NowStroyActionBox.NowIndex);
                        }
                        return;
                    });
                }
                else if (action.SkipType == ChatAction.SKIPTYPE.SAMETIME)
                {
                    LeanTween.value(TextBoardLayer.WordsText.gameObject, wordslengh, (float)origText.Length, speed * strlength).setOnUpdate((float val) =>
                    {
                        SetTextBoardWords(origText, Mathf.RoundToInt(val), action.Richparamater);
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
                    LeanTween.value(TextBoardLayer.WordsText.gameObject, wordslengh, (float)origText.Length, speed * strlength).setOnUpdate((float val) =>
                    {
                        SetTextBoardWords(origText, Mathf.RoundToInt(val), action.Richparamater);
                    }).setOnComplete(() =>
                    {
                        SetActionState(ChatAction.NOWSTATE.DONE, index);
                        WaitingForClick(action.CharacterID);
                    });
                }
                break;
            default:
                break;
        }
    }

    void ClickToDoing()
    {
        //如果当前动作为Doing且下一步为Click，则遍历所有正在Doing的且状态不为Loop的动作，设置为完成状态。
        ChatAction.StoryAction action = (ChatAction.StoryAction)NowStroyActionBox.ActionList[NowStroyActionBox.NowIndex];
        if (action.NowState == ChatAction.NOWSTATE.DOING && (action.SkipType == ChatAction.SKIPTYPE.CLICK || action.SkipType == ChatAction.SKIPTYPE.TimeAUTO || action.SkipType == ChatAction.SKIPTYPE.SAMETIME))
        {
            for (int i = 0; i < NowStroyActionBox.ActionList.Count; i++)
            {
                ChatAction.StoryAction _action = (ChatAction.StoryAction)NowStroyActionBox.ActionList[i];
                if (_action.NowState == ChatAction.NOWSTATE.DOING && _action.LoopType != ChatAction.LOOPTYPE.LOOP)
                {
                    RectTransform rt;
                    switch (_action.Command)
                    {
                        case "setbg":
                            break;
                        case "show":
                            rt = GetCharacterRectTransform(_action.CharacterID);
                            LeanTween.cancel(rt.gameObject);
                            Color rtcshow = rt.GetComponent<Image>().color;
                            rt.GetComponent<Image>().color = new Color(rtcshow.r, rtcshow.g, rtcshow.b, 1);
                            SetActionState(ChatAction.NOWSTATE.DONE, i);

                            break;
                        case "hide":
                            rt = GetCharacterRectTransform(_action.CharacterID);
                            LeanTween.cancel(rt.gameObject);
                            Color rtchide = rt.GetComponent<Image>().color;
                            rtchide = new Color(rtchide.r, rtchide.g, rtchide.b, 0);
                            SetActionState(ChatAction.NOWSTATE.DONE, i);

                            //WaitingForClick();
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
                            LeanTween.cancel(TextBoardLayer.WordsText.gameObject);
                            //如果是设置文字速度的自动模式，则可以点击加速
                            if (action.SkipType == ChatAction.SKIPTYPE.TimeAUTO)
                            {
                                for (int wordsindex = NowStroyActionBox.NowIndex; wordsindex < NowStroyActionBox.ActionList.Count; wordsindex++)
                                {
                                    ChatAction.StoryAction talkaction = (ChatAction.StoryAction)NowStroyActionBox.ActionList[wordsindex];
                                    if (talkaction.Command == "talk")
                                    {
                                        SetActionState(ChatAction.NOWSTATE.DONE, wordsindex);
                                        SetActionIndex(wordsindex);
                                        lastWords += talkaction.Parameter[0];
                                        if (talkaction.Parameter[3] == "endpage")
                                            break;
                                    }
                                    else
                                    {
                                        SetActionState(ChatAction.NOWSTATE.DONE, wordsindex);
                                        SetActionIndex(wordsindex);
                                        Debug.Log("嗯。。一定是你的读取方式或者配置错了，总之先让他过去呗~");
                                    }
                                }
                                TextBoardLayer.WordsText.text = lastWords;
                            }
                            //如果是点击模式，则直接点击加速
                            else
                            {
                                lastWords += _action.Parameter[0];
                                TextBoardLayer.WordsText.text = lastWords;
                                SetActionState(ChatAction.NOWSTATE.DONE, i);
                            }
                            WaitingForClick(action.CharacterID);
                            break;
                    }
                }
            }
        }//end if
        //如果为Done，则直接进入下一步
        else if (action.NowState == ChatAction.NOWSTATE.DONE && action.SkipType == ChatAction.SKIPTYPE.CLICK)
        {
            HideClickHint();

            if (NowStroyActionBox.NowIndex >= NowStroyActionBox.ActionList.Count - 1)
            {
                EndChatLayer();
            }
            else
            {
                SetActionIndex(NowStroyActionBox.NowIndex + 1);
                DoingAction(NowStroyActionBox.NowIndex);
            }
        }
        else if (action.NowState == ChatAction.NOWSTATE.DONE && NowStroyActionBox.NowIndex >= NowStroyActionBox.ActionList.Count - 1)
        {
            EndChatLayer();
        }
    }

    void WaitingForClick(string id)
    {
        //如果已经显示，则跳过
        if (!AniController.Get(TextBoardLayer.ClickHintLayer).IsPlaying())
        {
            Sprite[] hintsprite;
            if (NowResourcesBox.windowsSprites.TryGetValue(id, out hintsprite))
            {
                AniController.Get(TextBoardLayer.ClickHintLayer).AddSprite(hintsprite);
                AniController.Get(TextBoardLayer.ClickHintLayer).PlayAni(4, 7, AniController.AniType.Loop, 5);

                Color c = TextBoardLayer.ClickHintLayer.GetComponent<Image>().color;
                TextBoardLayer.ClickHintLayer.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1);
            }
            else
                Debug.Log("can't find " + id);
        }
    }

    void HideClickHint()
    {
        LeanTween.alpha(TextBoardLayer.ClickHintLayer, 0, 0.15f);
        AniController.Get(TextBoardLayer.ClickHintLayer).Stop();
    }

    int clipindex = 0;
    bool isfindText = false;
    void SetTextBoardWords(string origText,int val,MatchCollection mac)
    {
        //if (val < TextBoardLayer.WordsText.text.Length || origText.Length <= TextBoardLayer.WordsText.text.Length)
        //    return;

        if (mac.Count > 0)
        {
            int offset = 0;
            isfindText = false;

            for (int i = 0; i < mac.Count; i++)
            {
                if (val > mac[i].Index - offset && val <= mac[i].Groups[3].Index - mac[i].Groups[1].Length - offset)
                {
                    int nowlengh = mac[i].Index - offset;
                    TextBoardLayer.WordsText.text = origText.Substring(0, nowlengh) + mac[i].Groups[1] + origText.Substring(nowlengh, val - nowlengh) + mac[i].Groups[3];
                    
                    //TextBoardLayer.WordsText.text = origText.Substring(0, val);

                    Debug.Log("okokokok" + val + "   " + nowlengh);

                    isfindText = true;
                    break;
                }

                offset += mac[i].Groups[1].Length + mac[i].Groups[3].Length;
            }

            if(!isfindText)
                TextBoardLayer.WordsText.text = origText.Substring(0, val);

            //string words = "";
            //if (val == mac[clipindex].Length)
            //{
            //    words = origText.Substring(0, val + mac[clipindex].Length);
            //    TextBoardLayer.WordsText.text = words;
            //    clipindex++;
            //}
            //else
            //    TextBoardLayer.WordsText.text = origText.Substring(0, val);
        }
        else
        {
            TextBoardLayer.WordsText.text = origText.Substring(0, val);
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

        clipindex = 0;
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
        
        LeanTween.scaleY(TextBoardLayer.WordsBacklayer.gameObject, 0, 0.25f).setOnComplete(() =>
        {
            Sprite[] s = GetWindowsSprit(action.CharacterID);
            TextBoardLayer.WordsBacklayer.GetComponent<Image>().sprite = s[0];
            TextBoardLayer.WordsOutLayer.GetComponent<Image>().sprite = s[1];

            TextBoardLayer.NameBackLayer.GetComponent<Image>().sprite = s[0];
            TextBoardLayer.NameOutLayer.GetComponent<Image>().sprite = s[1];

            TextBoardLayer.WordsText.text = "";
            TextBoardLayer.NameText.text = NowStroyActionBox.CharacterList[action.CharacterID].Name;

            RectTransform rt = GetCharacterRectTransform(action.CharacterID);
            TextBoardLayer.NameBackLayer.localPosition = new Vector2(rt.localPosition.x - TextBoardLayer.WordsBacklayer.rect.width / 2, TextBoardLayer.NameBackLayer.localPosition.y);

            LeanTween.scaleY(TextBoardLayer.WordsBacklayer.gameObject, 1, 0.25f).setOnComplete(() =>
                {
                    DoingAction(NowStroyActionBox.NowIndex);
                });
        });
    }
}
