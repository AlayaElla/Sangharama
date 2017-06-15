using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ResourcesLoader
{
    public static Sprite[] LoadTextures(string path)
    {
        List<string> filePaths = new List<string>();
        string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG";
        string[] ImageType = imgtype.Split('|');
        for (int i = 0; i < ImageType.Length; i++)
        {
            string[] dirs = Directory.GetFiles(GetDataPath(path), ImageType[i]);
            for (int j = 0; j < dirs.Length; j++)
            {
                filePaths.Add(dirs[j]);
            }
        }

        Sprite[] sprites = new Sprite[filePaths.Count];
        for (int i = 0; i < filePaths.Count; i++)
        {
            Texture2D tx = new Texture2D(100, 100);
            tx.LoadImage(getImageByte(filePaths[i]));
            sprites[i] = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
            sprites[i].name = Path.GetFileNameWithoutExtension(filePaths[i]);
        }
        return sprites;
    }

    public static Sprite LoadSingleTexture(string path)
    {
        string temppath = GetDataPath(path) + ".png";
        Sprite sprites = new Sprite();
        Texture2D tx = new Texture2D(1, 1);
        tx.LoadImage(getImageByte(temppath));
        sprites = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
        sprites.name = Path.GetFileNameWithoutExtension(temppath);
        return sprites;
    }


    private static byte[] getImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }

    public static string[] LoadStoryConfig(string path)
    {
        string filepath = GetDataPath(path);

        string tempstr = File.ReadAllText(@filepath);

        string str1 = System.Text.RegularExpressions.Regex.Unescape(tempstr);
        string[] textflie = System.Text.RegularExpressions.Regex.Split(str1, "\r\n");


        return textflie;
    }


    //获取路径//
    private static string GetDataPath(string path)
    {
        return Application.dataPath + "/StoryResources/" + path;
    }
}
