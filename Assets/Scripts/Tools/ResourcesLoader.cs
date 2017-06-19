using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class ResourcesLoader : MonoBehaviour
{
    //public Sprite[] LoadTextures(string path)
    //{
    //    List<string> filePaths = new List<string>();
    //    string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG";
    //    string[] ImageType = imgtype.Split('|');
    //    for (int i = 0; i < ImageType.Length; i++)
    //    {
    //        string[] dirs = Directory.GetFiles(GetDataPath(path), ImageType[i]);
    //        for (int j = 0; j < dirs.Length; j++)
    //        {
    //            filePaths.Add(dirs[j]);
    //        }
    //    }

    //    Sprite[] sprites = new Sprite[filePaths.Count];
    //    for (int i = 0; i < filePaths.Count; i++)
    //    {
    //        Texture2D tx = new Texture2D(1, 1);
    //        //tx.filterMode = FilterMode.Point;
    //        tx.LoadImage(getByte(filePaths[i]));
    //        sprites[i] = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
    //        sprites[i].name = Path.GetFileNameWithoutExtension(filePaths[i]);
    //    }
    //    return sprites;
    //}

    //public Sprite LoadSingleTexture(string path)
    //{
    //    string temppath = GetDataPath(path) + ".png";
    //    Sprite sprites = new Sprite();
    //    Texture2D tx = new Texture2D(1, 1);
    //    //tx.filterMode = FilterMode.Point;
    //    tx.LoadImage(getByte(temppath));
    //    sprites = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
    //    sprites.name = Path.GetFileNameWithoutExtension(temppath);
    //    return sprites;
    //}

    //public AudioClip LoadWav(string path)
    //{
    //    string temppath = GetDataPath(path + ".wav");
    //    AudioClip clip = FromWavData(getByte(temppath));
    //    return clip;
    //}

    //public AudioClip LoadMp3(string path)
    //{
    //    string temppath = GetDataPath(path + ".mp3");
    //    AudioClip clip = FromMp3Data(getByte(temppath));
    //    return clip;
    //}

    //private byte[] getByte(string imagePath)
    //{
    //    FileStream files = new FileStream(imagePath, FileMode.Open);
    //    byte[] imgByte = new byte[files.Length];
    //    files.Read(imgByte, 0, imgByte.Length);
    //    files.Close();
    //    return imgByte;
    //}

    //AudioClip FromWavData(byte[] data)
    //{
    //    WAV wav = new WAV(data);
    //    AudioClip audioClip = AudioClip.Create("wavclip", wav.SampleCount, 1, wav.Frequency, false);
    //    audioClip.SetData(wav.LeftChannel, 0);
    //    return audioClip;
    //}

    //AudioClip FromMp3Data(byte[] data)
    //{
    //    // Load the data into a stream  
    //    MemoryStream mp3stream = new MemoryStream(data);
    //    // Convert the data in the stream to WAV format  
    //    Mp3FileReader mp3audio = new Mp3FileReader(mp3stream);

    //    WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(mp3audio);
    //    // Convert to WAV data  
    //    WAV wav = new WAV(AudioMemStream(waveStream).ToArray());
    //    //Debug.Log(wav);  
    //    AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, 1, wav.Frequency, false);
    //    audioClip.SetData(wav.LeftChannel, 0);
    //    // Return the clip  
    //    return audioClip;
    //}

    //MemoryStream AudioMemStream(WaveStream waveStream)
    //{
    //    MemoryStream outputStream = new MemoryStream();
    //    using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat))
    //    {
    //        byte[] bytes = new byte[waveStream.Length];
    //        waveStream.Position = 0;
    //        waveStream.Read(bytes, 0, System.Convert.ToInt32(waveStream.Length));
    //        waveFileWriter.Write(bytes, 0, bytes.Length);
    //        waveFileWriter.Flush();
    //    }
    //    return outputStream;
    //}

    public string[] LoadStoryConfig(string path)
    {
        string filepath = GetDataPath(path);

        string tempstr = File.ReadAllText(@filepath);

        string str1 = System.Text.RegularExpressions.Regex.Unescape(tempstr);
        string[] textflie = System.Text.RegularExpressions.Regex.Split(str1, "\r\n");

        return textflie;
    }

    //获取路径//
    public static string GetDataPath(string path)
    {
        return PathKit.GetResourcesPath() + "StoryResources/" + path;
    }
}

/* From http://answers.unity3d.com/questions/737002/wav-byte-to-audioclip.html */
public class WAV
{

    // convert two bytes to one float in the range -1 to 1  
    static float bytesToFloat(byte firstByte, byte secondByte)
    {
        // convert two bytes to one short (little endian)  
        short s = (short)((secondByte << 8) | firstByte);
        // convert to range from -1 to (just below) 1  
        return s / 32768.0F;
    }

    static int bytesToInt(byte[] bytes, int offset = 0)
    {
        int value = 0;
        for (int i = 0; i < 4; i++)
        {
            value |= ((int)bytes[offset + i]) << (i * 8);
        }
        return value;
    }
    // properties  
    public float[] LeftChannel { get; internal set; }
    public float[] RightChannel { get; internal set; }
    public int ChannelCount { get; internal set; }
    public int SampleCount { get; internal set; }
    public int Frequency { get; internal set; }

    public WAV(byte[] wav)
    {

        // Determine if mono or stereo  
        ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels  

        // Get the frequency  
        Frequency = bytesToInt(wav, 24);

        // Get past all the other sub chunks to get to the data subchunk:  
        int pos = 12;   // First Subchunk ID from 12 to 16  

        // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))  
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        {
            pos += 4;
            int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
            pos += 4 + chunkSize;
        }
        pos += 8;

        // Pos is now positioned to start of actual sound data.  
        SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)  
        if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)  

        // Allocate memory (right will be null if only mono sound)  
        LeftChannel = new float[SampleCount];
        if (ChannelCount == 2) RightChannel = new float[SampleCount];
        else RightChannel = null;

        // Write to double array/s:  
        int i = 0;
        int maxInput = wav.Length - (RightChannel == null ? 1 : 3);
        // while (pos < wav.Length)  
        while ((i < SampleCount) && (pos < maxInput))
        {
            LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
            pos += 2;
            if (ChannelCount == 2)
            {
                RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
            }
            i++;
        }
    }

    public override string ToString()
    {
        return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", LeftChannel, RightChannel, ChannelCount, SampleCount, Frequency);
    }
}
