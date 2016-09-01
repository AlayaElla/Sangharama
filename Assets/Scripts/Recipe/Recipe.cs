using UnityEngine;
using System.Collections;

public class Recipe : MonoBehaviour {

    public struct RecipeMap
    {
        public int ID;  //合成配方id，对应种类
        public string Name;  //合成配方id，对应种类

        public string Target;
        public MaterialEft EffetBox;

        public Slot[] Slots;   //表示合成配方的栏位
    }

    public struct Slot
    {
        public SlotTypeList SlotType;
        public int MatType;     //表示如果SlotType是特定的材料则表示材料的type(0:item;1:mind)，如果是种类则表示材料种类的id
        public int MatId;     //表示如果Filter1是材料type则表示材料的id，如果不是则定成-1，什么都不表示
        public int Num;     //表示需要消耗的数量
    }

    //材料效果结构体
    public struct MaterialEft
    {
        public struct Eft
        {
            public int NeedQuality;
            public int EftID;
        }
        public Eft Eft1;
        public Eft Eft2;
        public Eft Eft3;
    }

    public enum SlotTypeList    //表示材料的类型（0：材料；1：材料种类）
    {
        Material = 0,
        MaterialType
    }

    //材料效果配方
    public struct PropertyRecipe
    {
        public int ID;  //合成配方id，对应种类
        public int Target;

        public int[] Slots;   //表示合成属性的ID
    }

    public ArrayList ReicipeList = new ArrayList();
    public ArrayList PropertyRecipeList = new ArrayList();
    public static Recipe Intance;

    void Awake()
    {
        Intance = this;
    }

	// Use this for initialization
	void Start () {
        XmlTool xt = new XmlTool();
        ReicipeList = xt.loadRecipeXmlToArray();
        PropertyRecipeList = xt.loadPropertyRecipeXmlToArray();
	}

    public RecipeMap GetRecipeByID(int ID)
    {
        RecipeMap recipe = new RecipeMap();

        foreach (RecipeMap _recipe in ReicipeList)
        {
            if (_recipe.ID == ID)
            {
                recipe = _recipe;
                return recipe;
            }
        }
        return recipe;
    }

    //合成属性的方法
    public ArrayList ComposeProperty(ArrayList PropertyBox, out Result Compose)
    {
        //ArrayList SortPropertyBox = SelectionSortAscendingByProperty(PropertyBox);
        ArrayList AllComposeResult = FindAllComposeResult(PropertyBox);

        Compose = new Result();
        bool canCompose = false;

        foreach (Result r in AllComposeResult)
        {
            //如何有可以合成的属性则跳出循环
            if (canCompose)
                break;

            foreach (PropertyRecipe pr in PropertyRecipeList)
            {
                int _sum = pr.Slots[0] + pr.Slots[1];
                if (_sum == r.sum)
                {
                    if (pr.Slots[0] == r.Base1.Property.ID && pr.Slots[1] == r.Base2.Property.ID)
                    {
                        //表示可以合成
                        canCompose = true;
                        Compose.sum = pr.Target;
                        Compose.Base1 = r.Base1;
                        Compose.Base2 = r.Base2;
                        break;
                    }
                    if (pr.Slots[1] == r.Base1.Property.ID && pr.Slots[0] == r.Base2.Property.ID)
                    {
                        //表示可以合成
                        canCompose = true;
                        Compose.sum = pr.Target;
                        Compose.Base1 = r.Base1;
                        Compose.Base2 = r.Base2;
                        break;
                    }
                }
            }
        }

        if (canCompose)
        {
            Compose.Base1.Property = Materiral.GetProNameByProID(Compose.sum);
            PropertyBox[Compose.Base1.ID - 1] = Compose.Base1;
            PropertyBox.Remove(Compose.Base2);

            for (int i = 0; i < PropertyBox.Count; i++)
            {
                RecipeUI.PropertyElementBase _p = (RecipeUI.PropertyElementBase)PropertyBox[i];
                _p.ID = i + 1;
                PropertyBox[i] = _p;
            }
        }
        return PropertyBox;
    }

    public struct Result
    {
        public int sum;
        public RecipeUI.PropertyElementBase Base1;
        public RecipeUI.PropertyElementBase Base2;
    }

    ArrayList FindAllComposeResult(ArrayList PropertyBox)
    {
        ArrayList result = new ArrayList();

        for (int i = 0; i < PropertyBox.Count; i++)
        {
            RecipeUI.PropertyElementBase _proBase1 = (RecipeUI.PropertyElementBase)PropertyBox[i];
            Materiral.Property p1 = _proBase1.Property;

            for (int j = i + 1; j < PropertyBox.Count; j++)
            {
                RecipeUI.PropertyElementBase _proBase2 = (RecipeUI.PropertyElementBase)PropertyBox[j];
                Materiral.Property p2 = _proBase2.Property;

                //用于储存合成的结果数字（求和）和合成属性的ID，第一个为属性1，第二个为属性2
                Result _r = new Result();
                _r.sum = p1.ID + p2.ID;
                _r.Base1 = _proBase1;
                _r.Base2 = _proBase2;

                result.Add(_r);
            }
        }
        return result;
    }

    ArrayList SelectionSortAscendingByProperty(ArrayList PropertyBox)
    {
        for (int i = 0; i < PropertyBox.Count - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < PropertyBox.Count; j++)
            {
                RecipeUI.PropertyElementBase _proi = (RecipeUI.PropertyElementBase)PropertyBox[i];
                RecipeUI.PropertyElementBase _proj = (RecipeUI.PropertyElementBase)PropertyBox[j];
                int _i = _proi.Property.ID;
                int _j = _proj.Property.ID;

                if (_i < _j)
                {
                    min = j;
                }
            }
            if (min != i)
            {
                RecipeUI.PropertyElementBase temp = (RecipeUI.PropertyElementBase)PropertyBox[i];
                PropertyBox[i] = PropertyBox[min];
                PropertyBox[min] = temp;
            }
        }

        return PropertyBox;
    }


}
