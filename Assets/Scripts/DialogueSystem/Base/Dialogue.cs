﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "dialogue", menuName = "ZetanStudio/剧情/对话")]
public class Dialogue : ScriptableObject
{
    [SerializeField]
    private string _ID;
    public string ID
    {
        get
        {
            return _ID;
        }
    }

    [SerializeField]
    private List<DialogueWords> words = new List<DialogueWords>();
    public List<DialogueWords> Words
    {
        get
        {
            return words;
        }
    }

}
[Serializable]
public class DialogueWords
{
    public string TalkerName
    {
        get
        {
            if (TalkerType == TalkerType.NPC)
                if (TalkerInfo)
                    return TalkerInfo.Name;
                else return string.Empty;
            else return "玩家角色";
        }
    }

    [SerializeField]
    private TalkerType talkerType;
    public TalkerType TalkerType
    {
        get
        {
            return talkerType;
        }
    }

    [SerializeField]
    private TalkerInfomation talkerInfo;
    public TalkerInfomation TalkerInfo
    {
        get
        {
            return talkerInfo;
        }
    }

    [SerializeField, TextArea(3, 10)]
    private string words;
    public string Words
    {
        get
        {
            return words;
        }
    }

    [SerializeField]
    private int indexOfRrightBranch = -1;
    public int IndexOfRightBranch
    {
        get
        {
            return indexOfRrightBranch;
        }
    }

    public bool NeedToChusRightBranch
    {
        get
        {
            return indexOfRrightBranch > -1 && branches.Count > 1;
        }
    }

    [SerializeField]
    private string wordsWhenChusWB;//ChuseWB = Choose Wrong Branch
    public string WordsWhenChusWB
    {
        get
        {
            return wordsWhenChusWB;
        }
    }

    [SerializeField]
    private List<BranchDialogue> branches = new List<BranchDialogue>();
    public List<BranchDialogue> Branches
    {
        get
        {
            return branches;
        }
    }

    public DialogueWords()
    {

    }

    public DialogueWords(TalkerInfomation talkerInfo, string words, TalkerType talkerType = 0)
    {
        this.talkerInfo = talkerInfo;
        this.words = words;
        this.talkerType = talkerType;
    }

    public bool IsRightBranch(BranchDialogue branch)
    {
        return NeedToChusRightBranch && Branches.Contains(branch) && Branches.IndexOf(branch) == IndexOfRightBranch;
    }

    public override string ToString()
    {
        if (TalkerType == TalkerType.NPC && talkerInfo)
            return "[" + talkerInfo.Name + "]说：" + words;
        else if (TalkerType == TalkerType.Player)
            return "[玩家]说：" + words;
        else return "[Unnamed]说：" + words;
    }

    public int IndexOf(BranchDialogue branch)
    {
        return Branches.IndexOf(branch);
    }
}

[System.Serializable]
public class BranchDialogue : ICloneable
{
    [SerializeField]
    private string title;
    public string Title
    {
        get
        {
            if (string.IsNullOrEmpty(title)) return "……";
            return title;
        }
    }

    [SerializeField]
    private int specifyIndex = -1;//-1和0是等价的
    public int SpecifyIndex
    {
        get
        {
            return specifyIndex;
        }
    }

    [SerializeField]
    private Dialogue dialogue;
    public Dialogue Dialogue
    {
        get
        {
            return dialogue;
        }
    }

    [SerializeField]
    private bool goBack;
    public bool GoBack
    {
        get
        {
            return goBack;
        }
    }

    [SerializeField]
    private int indexToGo = -1;//-1表示不自定义
    public int IndexToGo
    {
        get
        {
            return indexToGo;
        }
    }

    [SerializeField]
    private bool deleteWhenCmplt = true;
    public bool DeleteWhenCmplt
    {
        get
        {
            return deleteWhenCmplt;
        }
    }

    [HideInInspector]
    public Dialogue runtimeParent;

    [HideInInspector]
    public int runtimeIndexToGoBack;
    public object Clone()
    {
        return MemberwiseClone();
    }
}

public enum TalkerType
{
    NPC,
    Player
}

[System.Serializable]
public class AffectiveDialogue
{
    [SerializeField]
    private Dialogue level_1;
    public Dialogue Level_1
    {
        get
        {
            return level_1;
        }
    }

    [SerializeField]
    private Dialogue level_2;
    public Dialogue Level_2
    {
        get
        {
            return level_2;
        }
    }

    [SerializeField]
    private Dialogue level_3;
    public Dialogue Level_3
    {
        get
        {
            return level_3;
        }
    }

    [SerializeField]
    private Dialogue level_4;
    public Dialogue Level_4
    {
        get
        {
            return level_1;
        }
    }
}