using UnityEngine;
using System.Collections;

public class ChatManager : MonoBehaviour {

    ArrayList chatconifg = new ArrayList();
    ArrayList chatText = new ArrayList();
    ArrayList ActionList = new ArrayList();
    ArrayList CharacterList = new ArrayList();

    PlayerInfo.Info info;

    public struct ChatConfig
    {
        public string Languege;
        public int speed;
    }

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        //获取角色配置
        PlayerInfo.SetLanguege("zh");
        info = PlayerInfo.GetPlayerInfo();

        //获取配置表
        XmlTool xt = new XmlTool();
        chatconifg = xt.loadChatConfigXmlToArray();
        LoadStory("shop1_1");
    }

	// Update is called once per frame
	void Update () {
	
	}

    void LoadStory(string path)
    {
        TxtTool tt = new TxtTool();
        string[] txt = tt.ReadFile(info.Languege, path);

        PushToActionList(txt);
    }


    void PushToActionList(string[] story)
    {
        //读取故事的类型，如果为0则读取类型为角色模式，如果为1则读取类型为故事模式
        int loadType = 0;

        foreach(string str in story)
        {
            //如果碰见注释符号或为空行，则忽略本行
            if (str.Contains("//") || str=="")
                continue;
            
            //设置读取类型
            if (str == "[Character]")
            {
                loadType = 0;
                continue;
            }
            else if (str == "[ChatList]")
            {
                loadType = 1;
                continue;
            }

            //读取角色模式的方法
            if (loadType == 0)
            {
                ChatAction.StoryCharacter character = new ChatAction.StoryCharacter();
                character.CharacterID = str.Substring(0, str.IndexOf("<"));

                string tempstr = str.Substring(str.IndexOf("<") + 1, str.IndexOf(">") - character.CharacterID.Length - 1);
                string[] parameter = tempstr.Split(';');

                for(int i=0;i<parameter.Length;i++)
                {
                    //读取name
                    if(i==0)
                        character.Name = parameter[i].Substring(5, parameter[i].Length - 5);
                    else if (i == 1)
                        character.Image = parameter[i].Substring(6, parameter[i].Length - 6);
                    else if (i == 2)
                        character.Windows = parameter[i].Substring(8, parameter[i].Length - 8);
                }

                CharacterList.Add(character);
            }
            //读取故事模式的方法
            else if (loadType == 1)
            {
                //{rourou:idle}我是<t 200>大笨蛋<t 100>！！<s>
                //{rourou:idle}我是大笨蛋！！<s>

                //方法字段
                if (str[0] == '<')
                {
                    ChatAction.StoryAction action = new ChatAction.StoryAction();
                    action.Command = str.Substring(1, str.IndexOf(' ') - 1);
                    string tempstr = str.Substring(action.Command.Length + 2, str.Length - action.Command.Length - 3);
                    string[] parameters = tempstr.Split(',');

                    //设定参数大小
                    action.Parameter = new string[parameters.Length - 3];
                    //区分参数
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        //设置方法参数
                        if (i < parameters.Length - 3)
                        {
                            action.Parameter[i] = parameters[i]; 
                        }
                        //设置角色id参数
                        else if (i == parameters.Length - 3)
                        {
                            action.CharacterID = parameters[i];
                        }
                        //设置动作参数ActionType
                        else if (i == parameters.Length - 2)
                        {
                            if (parameters[i] == "sametime")
                                action.ActionType = ChatAction.ACTIONTYPE.SAMETIME;
                            else if (parameters[i] == "step")
                                action.ActionType = ChatAction.ACTIONTYPE.STEP;
                        }
                        //设置进入下一步的方式SKIPTYPE
                        else if (i == parameters.Length - 1)
                        {
                            if (parameters[i] == "auto")
                                action.SkipType = ChatAction.SKIPTYPE.AUTO;
                            else if (parameters[i] == "click")
                                action.SkipType = ChatAction.SKIPTYPE.CLICK;
                        }
                    }
                    ActionList.Add(action);
                }
                //对话字段
                else if (str[0] == '{')
                {

                }
            }
        }
        Debug.Log("ok!");
    }
}
