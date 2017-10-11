using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Materiral : MonoBehaviour {

    //材料：物品类
    public struct Items
    {
        public int ID;     //ID
        public string Name;     //名称
        public string IMG;     //图片名称

        public int Type;        //类型ID
        public int Price;        //价格
        public int[] Property;    //属性
        public string Des;        //类型ID
    }
    //材料：念头类
    public struct Minds
    {
        public int ID;     //ID
        public string Name;     //名称
        public string IMG;     //图片名称

        public int Type;        //类型ID
        public int Price;        //价格
        public int[] Property;    //属性
        public string Des;        //类型ID
    }

    //材料：特殊物品，用来触发剧情
    public struct SpecialItem
    {
        public int ID;     //ID
        public string Name;     //名称
        public string IMG;     //图片名称

        public int Type;        //类型ID：0为不可见；1为可见
        public int Price;        //价格
        public int[] Property;    //属性
        public string Des;        //类型ID
    }


    //材料类型
    public struct MaterialType
    {
        public int ID;     //ID
        public string Name;     //名称
        public string IMG;     //图片名称
    }

    //材料属性
    public struct Property
    {
        public int ID;     //ID
        public string Name;     //名称
        public string IMG;     //图片名称

        public string Des;        //类型ID
        public int Effet;        //效果ID
    }

    //材料效果
    public struct Effect
    {
        public int ID;     //ID
        public string Name;     //名称
        public string IMG;     //图片名称

        public string Des;        //类型ID
        public int Effet;        //效果ID
    }

   static ArrayList itemList = new ArrayList();
   static ArrayList mindList = new ArrayList();
   static ArrayList specialItemList = new ArrayList();
   static ArrayList typeList = new ArrayList();
   static ArrayList propertyEffetList = new ArrayList();
   static ArrayList MaterialEffetList = new ArrayList();
   static Dictionary<string, Sprite> IconSprites = new Dictionary<string, Sprite>();

    void Awake()
    {
        XmlTool xl = new XmlTool();
        itemList = xl.loadItemXmlToArray();
        mindList = xl.loadMindXmlToArray();
        specialItemList = xl.loadSpecialItemXmlToArray();
        typeList = xl.loadTypeXmlToArray();
        propertyEffetList = xl.loadPropertyEffetXmlToArray();
        MaterialEffetList = xl.loadMaterialEffetXmlToArray();

        Sprite[] tempSprite = Resources.LoadAll<Sprite>("Texture/materials/actionIcon");
        foreach (Sprite s in tempSprite)
        {
            IconSprites.Add(s.name, s);
        }
    }

    static public ArrayList GetItemList()
    {
        return itemList;
    }

    static public ArrayList GetMindList()
    {
        return mindList;
    }

    static public ArrayList GetSpecialItemList()
    {
        return specialItemList;
    }

    static public ArrayList GetPropertyList()
    {
        return propertyEffetList;
    }

    //通过ID查找物品
    static public Items FindItemByID(int ID)
    {
        //如果返回的ID为-1，则表示没有找到物品
        Items Wrong = new Items();
        Wrong.ID = -1;
        Wrong.Name = "Wrong: Not Find!";

        foreach(Items item in itemList)
        {
            if (item.ID == ID)
            {
                return item;
            }
        }

        return Wrong;
    }

    //通过ID查找物品
    static public SpecialItem FindSpecialItemByID(int ID)
    {
        //如果返回的ID为-1，则表示没有找到物品
        SpecialItem Wrong = new SpecialItem();
        Wrong.ID = -1;
        Wrong.Name = "Wrong: Not Find!";

        foreach (SpecialItem item in specialItemList)
        {
            if (item.ID == ID)
            {
                return item;
            }
        }

        return Wrong;
    }

    static public Minds FindMindByID(int ID)
    {
        //如果返回的ID为-1，则表示没有找到物品
        Minds Wrong = new Minds();
        Wrong.ID = -1;
        Wrong.Name = "Wrong: Not Find!";

        foreach (Minds mind in mindList)
        {
            if (mind.ID == ID)
            {
                return mind;
            }
        }

        return Wrong;
    }

    //通过typeID返回Type的名字
    static public MaterialType FindTypeNameByID(int ID)
    {
        foreach (MaterialType _type in typeList)
        {
            if (_type.ID == ID)
            {
                return _type;
            }
        }
        return new MaterialType();
    }

    //通过物品ID返回道具的TypeID
    static public int GetTypeByMaterialID(int m_type,int ID)
    {
        if(m_type == 0)
        {
            foreach (Items _item in itemList)
            {
                if (_item.ID == ID)
                    return _item.Type;
            }
            return -1;  //如果找不到则返回-1
        }
        else if(m_type == 1)
        {
            foreach (Minds _mind in mindList)
            {
                if (_mind.ID == ID)
                    return _mind.Type;
            }
            return -1;  //如果找不到则返回-1
        }
        else if (m_type == 2)
        {
            foreach (SpecialItem _sp in specialItemList)
            {
                if (_sp.ID == ID)
                    return _sp.Type;
            }
            return -1;  //如果找不到则返回-1
        }
        else
            return -1;
    }

    static public Property GetProNameByProID(int ID)
    {
        //如果返回的ID为-1，则表示没有找到属性
        Property Wrong = new Property();
        Wrong.ID = -1;
        Wrong.Name = "Wrong: Not Find!";

        foreach (Property _p in propertyEffetList)
        {
            if (_p.ID == ID)
            {
                return _p;
            }
        }

        return Wrong;
    }

    //MaterialEffetList
    static public Effect FindMaterialEffectByID(int ID)
    {
        //如果返回的ID为-1，则表示没有找到属性
        Effect Wrong = new Effect();
        Wrong.ID = -1;
        Wrong.Name = "Wrong: Not Find!";

        foreach (Effect _e in MaterialEffetList)
        {
            if (_e.ID == ID)
            {
                return _e;
            }
        }

        return Wrong;
    }

    //通过物品ID返回道具的描述
    static public string GetDesByMaterialID(int m_type, int ID)
    {
        string Wrong = "Can't find!";
        
        if (m_type == 0)
        {
            foreach (Items _item in itemList)
            {
                if (_item.ID == ID)
                    return _item.Des;
            }
            return Wrong;  //如果找不到则返回-1
        }
        else if (m_type == 1)
        {
            foreach (Minds _mind in mindList)
            {
                if (_mind.ID == ID)
                    return _mind.Des;
            }
            return Wrong;  //如果找不到则返回-1
        }
        else if (m_type == 2)
        {
            foreach (SpecialItem _sp in specialItemList)
            {
                if (_sp.ID == ID)
                    return _sp.Des;
            }
            return Wrong;  //如果找不到则返回-1
        }
        else
            return Wrong;
    }

    static public Sprite GetMaterialIcon(int m_type, int ID)
    {
        Sprite s;
        if (m_type == 0)
        {
            foreach (Items _item in itemList)
            {
                if (_item.ID == ID)
                {
                    if (IconSprites.TryGetValue(_item.IMG, out s))
                        return s;
                }
            }
            return null;
        }
        else if (m_type == 1)
        {
            foreach (Minds _mind in mindList)
            {
                if (_mind.ID == ID)
                    if (IconSprites.TryGetValue(_mind.IMG, out s))
                        return s;
            }
            return null;
        }
        else if (m_type == 2)
        {
            foreach (SpecialItem _sp in specialItemList)
            {
                if (_sp.ID == ID)
                    if (IconSprites.TryGetValue(_sp.IMG, out s))
                        return s;
            }
            return null;
        }
        else
            return null;
    }

    static public Sprite GetPropertyIcon(int ID)
    {
        Sprite s;
        foreach (Property _p in propertyEffetList)
        {
            if (_p.ID == ID)
            {
                if (IconSprites.TryGetValue(_p.IMG, out s))
                    return s;
            }
        }
        return null;  //如果找不到则返回-1
    }

    static public Sprite GetIconByName(string name)
    {
        if (name == null)
            return null;

        Sprite s;
        if (IconSprites.TryGetValue(name, out s))
            return s;
        return null;  //如果找不到则返回-1
    }

    static public int GetMaterialPrice(int m_type, int ID)
    {
        if (m_type == 0)
        {
            foreach (Items _item in itemList)
            {
                if (_item.ID == ID)
                {
                    return _item.Price;
                }
            }
            return 0;  //如果找不到则返回0
        }
        else if (m_type == 1)
        {
            foreach (Minds _mind in mindList)
            {
                if (_mind.ID == ID)
                    return _mind.Price;
            }
            return 0;  //如果找不到则返回0
        }
        else if (m_type == 2)
        {
            foreach (SpecialItem _sp in specialItemList)
            {
                if (_sp.ID == ID)
                    return _sp.Price;
            }
            return 0;  //如果找不到则返回0
        }
        else
            return 0;
    }

    //获取特效
    static public int[] GetMaterialProperty(int m_type, int ID)
    {
        if (m_type == 0)
        {
            foreach (Items _item in itemList)
            {
                if (_item.ID == ID)
                {
                    return _item.Property;
                }
            }
            return null;  //如果找不到则返回0
        }
        else if (m_type == 1)
        {
            foreach (Minds _mind in mindList)
            {
                if (_mind.ID == ID)
                    return _mind.Property;
            }
            return null;  //如果找不到则返回0
        }
        else if (m_type == 2)
        {
            foreach (SpecialItem _sp in specialItemList)
            {
                if (_sp.ID == ID)
                    return _sp.Property;
            }
            return null;  //如果找不到则返回0
        }
        else
            return null;
    }

    //获取名称
    static public string GetMaterialName(int m_type, int ID)
    {
        if (m_type == 0)
        {
            foreach (Items _item in itemList)
            {
                if (_item.ID == ID)
                {
                    return _item.Name;
                }
            }
            return "Not find!";  //如果找不到则返回0
        }
        else if (m_type == 1)
        {
            foreach (Minds _mind in mindList)
            {
                if (_mind.ID == ID)
                    return _mind.Name;
            }
            return "Not find!";  //如果找不到则返回0
        }
        else if (m_type == 2)
        {
            foreach (SpecialItem _sp in specialItemList)
            {
                if (_sp.ID == ID)
                    return _sp.Name;
            }
            return "Not find!";  //如果找不到则返回0
        }
        else
            return "Not find!";
    }
}
