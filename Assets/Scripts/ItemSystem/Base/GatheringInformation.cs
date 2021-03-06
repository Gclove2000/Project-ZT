﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gathering info", menuName = "ZetanStudio/采集物信息")]
public class GatheringInformation : ScriptableObject
{
    [SerializeField]
    protected string _ID;
    public string ID
    {
        get
        {
            return _ID;
        }
    }

    [SerializeField]
    protected string _name;
    public new string name
    {
        get
        {
            return _name;
        }
    }

    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("手动", "斧子", "镐子", "铲子", "锄头")]
#endif
    protected GatherType gatherType;
    public GatherType GatherType
    {
        get
        {
            return gatherType;
        }
    }

    [SerializeField]
    protected float gatherTime;
    public float GatherTime
    {
        get
        {
            return gatherTime;
        }
    }

    [SerializeField]
    protected float refreshTime;
    public float RefreshTime
    {
        get
        {
            return refreshTime;
        }
    }

    [SerializeField]
    protected List<DropItemInfo> productItems = new List<DropItemInfo>();
    public List<DropItemInfo> ProductItems
    {
        get
        {
            return productItems;
        }
    }

    [SerializeField]
    private GameObject lootPrefab;
    public GameObject LootPrefab
    {
        get
        {
            return lootPrefab;
        }
    }
}
public enum GatherType
{
    /// <summary>
    /// 手采
    /// </summary>
    Hands,
    /// <summary>
    /// 斧子
    /// </summary>
    Axe,
    /// <summary>
    /// 镐子
    /// </summary>
    Shovel,
    /// <summary>
    /// 铲子
    /// </summary>
    Spade,
    /// <summary>
    /// 锄头
    /// </summary>
    Hoe
}