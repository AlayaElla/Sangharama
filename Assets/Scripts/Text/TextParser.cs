using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TextParser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    static public string SpecialString(string str)
    {
        string pattern = @"\[(.*?)\]";
        foreach (Match match in Regex.Matches(str, pattern))
        {
            string spstr = "";
            int index = match.Value.IndexOf(" ");
            if (index > 0)
                spstr = match.Value.Substring(1, index - 1);
            else
                spstr = match.Value.Substring(1, match.Value.Length - 2);

            Debug.Log(spstr);

            string parstr = "";
            switch (spstr)
            {
                case "localtime":
                    str = str.Replace(match.Value, System.DateTime.Now.ToString("HH:mm"));
                    break;
                case "gametime":
                    break;
                case "item":
                    index = match.Value.LastIndexOf(" ");
                    parstr = match.Value.Substring(index, match.Value.Length - index - 1);
                    //查找材料名称
                    string[] str_temp = parstr.Split(':');
                    string itemname = "<color=red>" + Materiral.GetMaterialName(int.Parse(str_temp[0]), int.Parse(str_temp[1]))+ "</color>";
                    str = str.Replace(match.Value, itemname);
                    break;
                case "point":
                    index = match.Value.LastIndexOf(" ");
                    parstr = match.Value.Substring(index, match.Value.Length - index - 1);
                    //查找路点名称
                    string pathname = "<color=blue>" + MapPathManager.GetPathName(int.Parse(parstr)) + "</color>"; ;
                    str = str.Replace(match.Value, pathname);
                    break;
                default:
                    break;
            }
        }
        return str;
    }
}
