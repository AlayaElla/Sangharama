using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TxtTool{

    //判断是否存在文件
    public bool hasFile(String fileName)
    {
        return File.Exists(fileName);
    }

    public void WriteFile(string path,string name,string info)
    {
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        sw = t.CreateText();
        //以行写入信息
        sw.WriteLine(info);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
    }

    public ArrayList ReadFile(string path, string name)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + name);
        }
        catch (Exception e)
        {
            return null;
        }
        string line;
        ArrayList text = new ArrayList();
        //逐行读取
        while ((line = sr.ReadLine()) != null)
        {
            text.Add(line);
        }
        sr.Close();
        sr.Dispose();
        return text;
    }
	
}
