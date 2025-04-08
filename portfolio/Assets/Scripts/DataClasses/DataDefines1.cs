using ASGA.DS;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace ASGA.DS
{
    public partial class ResCharacterInfo : BaseInfoData
    {
        public ResCharacterInfo() { }

        public override void Dispose()
        {

        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int OpenLevel { get; set; }
        public int OpenPrice { get; set; }
        public int SpawnCost { get; set; }
        public int AttackPower { get; set; }
        public int AttackSpeed { get; set; }
        public int AttackRange { get; set; }
        public int HP { get; set; }
    }

    public partial class ResItemInfo : BaseInfoData
    {
        public ResItemInfo() { }
        public override void Dispose()
        {

        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string AtlasName { get; set; }
        public string SpriteName { get; set; }

        public int ItemType { get; set; }

        public ITEM_PARTS GetPartType()
        {
            return (ITEM_PARTS)ItemType;
        }

    }

    public partial class UserItemInfo :BaseInfoData
    { 
        public UserItemInfo() { }

        public ResItemInfo ItemData;

        public List<ResStatInfo> ItemStats;

        public int UserID { get; set; }
        public int ItemSeq { get; set; }

        public int Grade { get; set; }
        public bool IsEquip { get; set; }

        public string SPRITE_ATLAS
        {
            get
            {
                if (ItemData == null)
                {
                    return "";
                }
                return $"{ItemData.AtlasName},{ItemData.SpriteName}";
            }
        }

        public ITEM_PARTS ITEM_PARTS_TYPE
        {
            get
            {
                if (ItemData == null)
                {
                    return ITEM_PARTS.NONE;
                }

                return ItemData.GetPartType();
            }
        }

        public string GRADE_NAME
        {
            get
            {
                return Utils.GetItemGradeName(Grade);
            }
        }

        public string STR_EQUIP
        {
            get
            {
                return (IsEquip) ? "E" : ""; 
            }
        }
    }

    public partial class ResStatInfo : BaseInfoData
    {
        public ResStatInfo() { }
        public override void Dispose()
        {

        }

        public int ParentID { get; set; }
        public int ParentType { get; set; }

        public int StatType { get; set; }
        public int StatValue { get; set; }
    }

    public partial struct GridData
    {
        public int GridIndex;
        public BaseInfoData Data;
    }
}

