using UnityEngine;
using System.Collections;

public class ChatAction {

    public struct StoryAction
    {
        public string CharacterID;
        public string Command;
        public string[] Parameter;

        public LOOPTYPE LoopType;
        public SKIPTYPE SkipType;

        public NOWSTATE NowState;
    }

    public struct StoryCharacter
    {
        public string CharacterID;
        public int Orientation;
        public string Name;
        public string Image;
        public string Windows;
    }

    public enum LOOPTYPE
    {
        NOTLOOP,
        LOOP

    }
    public enum SKIPTYPE
    {
        CLICK,
        AUTO,
        TimeAUTO,
        SAMETIME
        
    }
    public enum NOWSTATE
    {
        WAITING,
        DOING,
        DONE
    }
}
