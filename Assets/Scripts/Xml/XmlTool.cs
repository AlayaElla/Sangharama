using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;
using System;

public class XmlTool
{


    //判断是否存在文件
    public bool hasFile(String fileName)
    {
        return File.Exists(fileName);
    }

    //新建item配置表
    public void createItemXml()
    {
        //保存路径
        string filepath = GetDataPath() + "/Materiral/Item.xml";

        if (!File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("ItemList");
            XmlElement item = xmlDoc.CreateElement("Item");
            item.SetAttribute("ID", "1");
            item.SetAttribute("Name", "这是什么！");
            item.SetAttribute("Type", "1");
            item.SetAttribute("des", "这一个中文描述");

            root.AppendChild(item);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            Debug.Log("Xml is OK!");
        }
    }

    //新建mind配置表
    public void createMindXml()
    {
        //保存路径
        string filepath = GetDataPath() + "/Materiral/Mind.xml";

        if (!File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("MindList");
            XmlElement item = xmlDoc.CreateElement("Mind");
            item.SetAttribute("ID", "1");
            item.SetAttribute("Name", "这是什么！");
            item.SetAttribute("Type", "1");
            item.SetAttribute("des", "这一个中文描述");

            root.AppendChild(item);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            Debug.Log("Xml is OK!");
        }
    }

    //新建采集配置表
    public void createCollectionXml()
    {
        //保存路径
        string filepath = GetDataPath() + "/Materiral/Collection.xml";

        if (!File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("CollectionList");
            XmlElement item = xmlDoc.CreateElement("Collection");
            item.SetAttribute("Map", "1");
            item.SetAttribute("MateriralType", "1");
            item.SetAttribute("ID", "1");
            item.SetAttribute("Weight", "1");
            item.SetAttribute("RandomQuality", "1,100");
            item.SetAttribute("RandomProperty", "1,2,3");
            item.SetAttribute("PropertyWeight", "1,1,1");
            root.AppendChild(item);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            Debug.Log("Xml is OK!");
        }
    }

    //////////////////////////////////////////
    //读取XML表
    public ArrayList loadCollectionXmlToArray()
    {
        //保存路径
        string filepath = "Materiral/Collection";

        string _result = Resources.Load(filepath).ToString();

        ArrayList collection = new ArrayList();
                    
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList=xmlDoc.SelectSingleNode("CollectionList").ChildNodes;

        foreach(XmlElement map in nodeList)
        {
            CollectAction.CollectionMap collectionMap = new CollectAction.CollectionMap();
            collectionMap.RandomQuality = new int[2];
            collectionMap.RandomProperty = new int[4, 2];

            //读取node内属性，把string转化为对应的属性
            if (map.GetAttribute("Map") != "")
                collectionMap.Map = int.Parse(map.GetAttribute("Map"));
            if (map.GetAttribute("MateriralType") != "")
                collectionMap.MateriralType = int.Parse(map.GetAttribute("MateriralType"));
            if (map.GetAttribute("ID") != "")
                collectionMap.ID = int.Parse(map.GetAttribute("ID"));
            if (map.GetAttribute("Weight") != "")
                collectionMap.Weight = int.Parse(map.GetAttribute("Weight"));
            if(map.GetAttribute("RandomQuality")!="")
            {
                string qualityString = map.GetAttribute("RandomQuality");
                string[] quaList = qualityString.Split(',');

                for (int i = 0; i < quaList.Length; i++)
                {
                    collectionMap.RandomQuality[i] = int.Parse(quaList[i]);
                }
            }
            if (map.GetAttribute("PropertyProbability") != "")
                collectionMap.PropertyProbability = int.Parse(map.GetAttribute("PropertyProbability"));
            if ((map.GetAttribute("RandomProperty") != "") && (map.GetAttribute("PropertyWeight") != ""))
            {
                string proString = map.GetAttribute("RandomProperty");
                string[] proList = proString.Split(',');

                string weiString = map.GetAttribute("PropertyWeight");
                string[] weiList = weiString.Split(',');

                for (int i = 0; i < proList.Length; i++)
                {
                    collectionMap.RandomProperty[i, 0] = int.Parse(proList[i]);
                    collectionMap.RandomProperty[i, 1] = int.Parse(weiList[i]);
                }
            }
            collection.Add(collectionMap);
        }
        return collection;
    }

    public ArrayList loadItemXmlToArray()
    {
        //保存路径
        string filepath = "Materiral/Item";

        string _result = Resources.Load(filepath).ToString();

        ArrayList itemList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("ItemList").ChildNodes;

        foreach (XmlElement item in nodeList)
        {
            Materiral.Items _items = new Materiral.Items();

            //读取node内属性，把string转化为对应的属性
            if (item.GetAttribute("ID") != "")
                _items.ID = int.Parse(item.GetAttribute("ID"));
            if (item.GetAttribute("Name") != "")
                _items.Name = item.GetAttribute("Name");
            if (item.GetAttribute("Image") != "")
                _items.IMG = item.GetAttribute("Image");
            if (item.GetAttribute("Type") != "")
                _items.Type = int.Parse(item.GetAttribute("Type"));
            if (item.GetAttribute("Price") != "")
                _items.Price = int.Parse(item.GetAttribute("Price"));
            if (item.GetAttribute("des") != "")
                _items.Des = item.GetAttribute("des");

            //添加进itemList中
            itemList.Add(_items);
        }
        return itemList;
    }

    public ArrayList loadCharacterXmlToArray()
    {
        //保存路径
        string filepath = "Character/CharacterList";

        string _result = Resources.Load(filepath).ToString();

        ArrayList characterList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("CharacterList").ChildNodes;

        foreach (XmlElement character in nodeList)
        {
            CharacterModle.CharacterInfo _character = new CharacterModle.CharacterInfo();

            //读取node内属性，把string转化为对应的属性
            if (character.GetAttribute("ID") != "")
                _character.ID = int.Parse(character.GetAttribute("ID"));
            if (character.GetAttribute("Name") != "")
                _character.Name = character.GetAttribute("Name");
            if (character.GetAttribute("Skin") != "")
                _character.Skin = character.GetAttribute("Skin");
            if (character.GetAttribute("Type") != "")
                _character.Sex = (CharacterModle.SexType)int.Parse(character.GetAttribute("Sex"));
            if (character.GetAttribute("OutTime") != "")
                _character.OutTime = (CharacterModle.TimeType)int.Parse(character.GetAttribute("OutTime"));

            if (character.GetAttribute("FavoriteItems") != "")
            {
                string[] _items = character.GetAttribute("FavoriteItems").Split(',');
                _character.FavoriteItems = new int[_items.Length];
                for(int i=0;i<_items.Length;i++)
                {
                    _character.FavoriteItems[i] = int.Parse(_items[i]);
                }
            }
            if (character.GetAttribute("FavoriteMinds") != "")
            {
                string[] _minds = character.GetAttribute("FavoriteMinds").Split(',');
                _character.FavoriteMinds = new int[_minds.Length];
                for (int i = 0; i < _minds.Length; i++)
                {
                    _character.FavoriteMinds[i] = int.Parse(_minds[i]);
                }
            }
            if (character.GetAttribute("Weight") != "")
                _character.Weight = int.Parse(character.GetAttribute("Weight"));
            if (character.GetAttribute("Des") != "")
                _character.Des = character.GetAttribute("Des");


            //添加进itemList中
            characterList.Add(_character);
        }
        return characterList;
    }

    public ArrayList loadSkinXmlToArray()
    {
        //保存路径
        string filepath = "Character/skinConfig";

        string _result = Resources.Load(filepath).ToString();

        ArrayList skinList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("SkinList").ChildNodes;

        foreach (XmlElement skin in nodeList)
        {
            CharacterModle.Skin _skin = new CharacterModle.Skin();

            //读取node内属性，把string转化为对应的属性
            if (skin.GetAttribute("SkinName") != "")
                _skin.SkinName = skin.GetAttribute("SkinName");
            for (int i = 1; i <= skin.Attributes.Count - 1; i++)
            {
                string _att = "Action" + i.ToString();
                if (skin.HasAttribute(_att))
                {
                    string _temp = skin.GetAttribute(_att);
                    int index = _temp.IndexOf(":");
                    string _name = _temp.Substring(0, index);
                    string[] _value = _temp.Substring(index + 1).Split(',');
                    _skin.ActionList[_name] = new int[2];
                    _skin.ActionList[_name][0] = int.Parse(_value[0]);
                    _skin.ActionList[_name][1] = int.Parse(_value[1]);
                }
            }

            //添加进itemList中
            skinList.Add(_skin);
        }
        return skinList;
    }

    public ArrayList loadMindXmlToArray()
    {
        //保存路径
        string filepath = "Materiral/Mind";

        string _result = Resources.Load(filepath).ToString();

        ArrayList mindList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("MindList").ChildNodes;

        foreach (XmlElement mind in nodeList)
        {
            Materiral.Minds _mind = new Materiral.Minds();

            //读取node内属性，把string转化为对应的属性
            if (mind.GetAttribute("ID") != "")
                _mind.ID = int.Parse(mind.GetAttribute("ID"));
            if (mind.GetAttribute("Name") != "")
                _mind.Name = mind.GetAttribute("Name");
            if (mind.GetAttribute("Image") != "")
                _mind.IMG = mind.GetAttribute("Image");
            if (mind.GetAttribute("Type") != "")
                _mind.Type = int.Parse(mind.GetAttribute("Type"));
            if (mind.GetAttribute("Price") != "")
                _mind.Price = int.Parse(mind.GetAttribute("Price"));
            if (mind.GetAttribute("des") != "")
                _mind.Des = mind.GetAttribute("des");

            //添加进itemList中
            mindList.Add(_mind);
        }
        return mindList;
    }

    public ArrayList loadTypeXmlToArray()
    {
        ArrayList _typeList = new ArrayList();

        string filepath = "Materiral/MaterialType";
        string _result = Resources.Load(filepath).ToString();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_result);
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("TypeList").ChildNodes;

        foreach (XmlElement type in nodeList)
        {
            Materiral.MaterialType _type = new Materiral.MaterialType();

            if (type.GetAttribute("ID") != "")
                _type.ID = int.Parse(type.GetAttribute("ID"));

            if (type.GetAttribute("Name") != "")
                _type.Name = type.GetAttribute("Name");

            if (type.GetAttribute("Image") != "")
                _type.IMG = type.GetAttribute("Image");

            _typeList.Add(_type);
        }

        return _typeList;
    }

    public ArrayList loadRecipeXmlToArray()
    {
        //保存路径
        string filepath = "Materiral/Recipe";

        string _result = Resources.Load(filepath).ToString();

        ArrayList recipeList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("RecipeList").ChildNodes;

        foreach (XmlElement recipe in nodeList)
        {
            Recipe.RecipeMap _recipe = new Recipe.RecipeMap();

            //读取node内属性，把string转化为对应的属性
            if (recipe.GetAttribute("ID") != "")
                _recipe.ID = int.Parse(recipe.GetAttribute("ID"));
            if (recipe.GetAttribute("Name") != "")
                _recipe.Name = recipe.GetAttribute("Name");
            if (recipe.GetAttribute("Target") != "")
                _recipe.Target = recipe.GetAttribute("Target");
            if (recipe.GetAttribute("Mar1Qua") != "")
                _recipe.EffetBox.Eft1.NeedQuality = int.Parse(recipe.GetAttribute("Mar1Qua"));
            if (recipe.GetAttribute("Mar1Eft") != "")
                _recipe.EffetBox.Eft1.EftID = int.Parse(recipe.GetAttribute("Mar1Eft"));
            if (recipe.GetAttribute("Mar2Qua") != "")
                _recipe.EffetBox.Eft2.NeedQuality = int.Parse(recipe.GetAttribute("Mar2Qua"));
            if (recipe.GetAttribute("Mar2Eft") != "")
                _recipe.EffetBox.Eft2.EftID = int.Parse(recipe.GetAttribute("Mar2Eft"));
            if (recipe.GetAttribute("Mar3Qua") != "")
                _recipe.EffetBox.Eft3.NeedQuality = int.Parse(recipe.GetAttribute("Mar3Qua"));
            if (recipe.GetAttribute("Mar3Eft") != "")
                _recipe.EffetBox.Eft3.EftID = int.Parse(recipe.GetAttribute("Mar3Eft"));

            _recipe.Slots = new Recipe.Slot[recipe.ChildNodes.Count];

            for (int i = 0; i < recipe.ChildNodes.Count; i++)
            {
                XmlElement element = (XmlElement)recipe.ChildNodes[i];
                string slotName = "Slot" + (i + 1).ToString();

                if (element.Name == slotName)
                {
                    if (element.GetAttribute("NeedNum") != "")
                        _recipe.Slots[i].Num = int.Parse(element.GetAttribute("NeedNum"));

                    string mat = element.InnerText;

                    //解析材料内容
                    if (mat[0] == char.Parse("T"))
                    {
                        _recipe.Slots[i].SlotType = (Recipe.SlotTypeList)1;
                        string mat_str = mat.Substring(mat.IndexOf(":") + 1);
                        _recipe.Slots[i].MatType = int.Parse(mat_str);
                        _recipe.Slots[i].MatId = -1;
                    }
                    else if (mat[0] == char.Parse("M"))
                    {
                        _recipe.Slots[i].SlotType = (Recipe.SlotTypeList)0;

                        string mat_type = mat.Substring(mat.IndexOf(":") + 1, mat.IndexOf(",") - 2);
                        string mat_str = mat.Substring(mat.IndexOf(",") + 1);
                        _recipe.Slots[i].MatType = int.Parse(mat_type);
                        _recipe.Slots[i].MatId = int.Parse(mat_str);
                    }
                }
            }

            //添加进itemList中
            recipeList.Add(_recipe);
        }
        return recipeList;
    }

    //读取材料属性列表
    public ArrayList loadPropertyEffetXmlToArray()
    {
        //保存路径
        string filepath = "Materiral/PropertyEffet";

        string _result = Resources.Load(filepath).ToString();

        ArrayList propertyList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("PropertyList").ChildNodes;

        foreach (XmlElement property in nodeList)
        {
            Materiral.Property _property = new Materiral.Property();

            //读取node内属性，把string转化为对应的属性
            if (property.GetAttribute("ID") != "")
                _property.ID = int.Parse(property.GetAttribute("ID"));
            if (property.GetAttribute("Name") != "")
                _property.Name = property.GetAttribute("Name");
            if (property.GetAttribute("Image") != "")
                _property.IMG = property.GetAttribute("Image");
            if (property.GetAttribute("des") != "")
                _property.Des = property.GetAttribute("des");
            if (property.GetAttribute("Effet") != "")
                _property.Effet = int.Parse(property.GetAttribute("Effet"));

            //添加进itemList中
            propertyList.Add(_property);
        }
        return propertyList;
    }

    //读取合成属性列表
    public ArrayList loadMaterialEffetXmlToArray()
    {
        //保存路径
        string filepath = "Materiral/MaterialEffet";

        string _result = Resources.Load(filepath).ToString();

        ArrayList materialEffetList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("MaterialEffetList").ChildNodes;

        foreach (XmlElement property in nodeList)
        {
            Materiral.Effect _effect = new Materiral.Effect();

            //读取node内属性，把string转化为对应的属性
            if (property.GetAttribute("ID") != "")
                _effect.ID = int.Parse(property.GetAttribute("ID"));
            if (property.GetAttribute("Name") != "")
                _effect.Name = property.GetAttribute("Name");
            if (property.GetAttribute("Image") != "")
                _effect.IMG = property.GetAttribute("Image");
            if (property.GetAttribute("des") != "")
                _effect.Des = property.GetAttribute("des");
            if (property.GetAttribute("Effet") != "")
                _effect.Effet = int.Parse(property.GetAttribute("Effet"));

            //添加进itemList中
            materialEffetList.Add(_effect);
        }
        return materialEffetList;
    }

    //读取属性配方表
    public ArrayList loadPropertyRecipeXmlToArray()
    {
        //保存路径
        string filepath = "Materiral/PropertyRecipe";

        string _result = Resources.Load(filepath).ToString();

        ArrayList recipeList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("PropertyRecipeList").ChildNodes;

        foreach (XmlElement recipe in nodeList)
        {
            Recipe.PropertyRecipe _recipe = new Recipe.PropertyRecipe();

            //读取node内属性，把string转化为对应的属性
            if (recipe.GetAttribute("ID") != "")
                _recipe.ID = int.Parse(recipe.GetAttribute("ID"));
            if (recipe.GetAttribute("Target") != "")
                _recipe.Target = int.Parse(recipe.GetAttribute("Target"));

            _recipe.Slots = new int[recipe.ChildNodes.Count];

            for (int i = 0; i < recipe.ChildNodes.Count; i++)
            {
                XmlElement element = (XmlElement)recipe.ChildNodes[i];
                string slotName = "Property" + (i + 1).ToString();

                if (element.Name == slotName)
                {
                    int mat = int.Parse(element.InnerText);
                    _recipe.Slots[i] = mat;
                }
            }

            //添加进itemList中
            recipeList.Add(_recipe);
        }
        return recipeList;
    }

    //读取路径配置表
    public ArrayList loadPathXmlToArray()
    {
        //保存路径
        string filepath = "Map/MapPath";

        string _result = Resources.Load(filepath).ToString();

        ArrayList PathList = new ArrayList();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(_result);

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("PathList").ChildNodes;

        foreach (XmlElement recipe in nodeList)
        {

            MapPathManager.Path _path = new MapPathManager.Path();

            //读取node内属性，把string转化为对应的属性
            if (recipe.GetAttribute("Map") != "")
                _path.Map = int.Parse(recipe.GetAttribute("Map"));
            if (recipe.GetAttribute("Name") != "")
                _path.Name = recipe.GetAttribute("Name");
            if (recipe.GetAttribute("Next") != "")
            {
                string str_next = recipe.GetAttribute("Next");
                if (str_next != "")
                {
                    string[] str_temp = str_next.Split(',');
                    _path.Next = new int[str_temp.Length];
                    for (int i = 0; i < _path.Next.Length; i++)
                    {
                        _path.Next[i] = int.Parse(str_temp[i]);
                    }
                }
            }
            if (recipe.GetAttribute("Pre") != "")
            {
                string str_pre = recipe.GetAttribute("Pre");
                if (str_pre != "")
                {
                    string[] str_temp = str_pre.Split(',');
                    _path.Pre = new int[str_temp.Length];
                    for (int i = 0; i < _path.Pre.Length; i++)
                    {
                        _path.Pre[i] = int.Parse(str_temp[i]);
                    }
                }
            }
            if (recipe.GetAttribute("Price") != "")
                _path.Price = int.Parse(recipe.GetAttribute("Price"));
            else
                _path.Price = 0;
            //添加进itemList中
            PathList.Add(_path);
        }
        return PathList;
    }


    //获取路径//
    private static string GetDataPath()
    {
        return Application.dataPath + "/Resources";
    }

    void Start()
    {
    }

    //新建采集配置表
    public void AddXml()
    {
        //保存路径
        string filepath = GetDataPath() + "/Materiral/my.xml";

        if (!File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("transforms");
            XmlElement elmNew = xmlDoc.CreateElement("rotation");
            elmNew.SetAttribute("id", "1");
            elmNew.SetAttribute("name", "yooo");

            XmlElement rotation_X = xmlDoc.CreateElement("x");
            rotation_X.InnerText = "0";
            rotation_X.SetAttribute("id", "1");
            XmlElement rotation_Y = xmlDoc.CreateElement("y");
            rotation_Y.InnerText = "1";
            XmlElement rotation_Z = xmlDoc.CreateElement("z");
            rotation_Z.InnerText = "2";

            elmNew.AppendChild(rotation_X);
            elmNew.AppendChild(rotation_Y);
            elmNew.AppendChild(rotation_Z);
            root.AppendChild(elmNew);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            Debug.Log("AddXml OK!");
        }
    }
}