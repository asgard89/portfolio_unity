using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using ASGA.DS;
using TMPro;

public enum ITEM_PARTS
{
    NONE = -1,
    HELMET,
    NECKLACE,
    ARMOR,
    BELT,
    RING,
    BOOTS,
}

enum ITEM_FILTER
{
    GRADE,
    TYPE,
}

public class UIInventory : UIBase
{
    [SerializeField]
    PoolingGridLayoutGroup inventoryGridLayout = null;

    [SerializeField]
    List<UIEquipSlot> equipSlotList = null;

    //List<UIInventorySlot> viewList;
    List<UIInventorySlot> userInvenList;

    Dictionary<ITEM_FILTER, string> filterDic;

    [SerializeField]
    TMP_Dropdown gradeDropdown;
    [SerializeField]
    TMP_Dropdown typeDropdown;

    #region test
    List<UserItemInfo> userItems = null;
    int userItemNum = 1000;
    #endregion

    public override void Start()
    {
        base.Start();

        InitializeUI();
    }

    protected override void InitializeUI()
    {
        base.InitializeUI();

        if (inventoryGridLayout != null)
        {
            inventoryGridLayout.OnRefresh += gridRefresh;
        }
            

        gradeDropdown.onValueChanged.AddListener(GradeFilter);
        typeDropdown.onValueChanged.AddListener(TypeFilter);

        loadItemDatas();

        foreach (UIEquipSlot _equipSlot in equipSlotList)
        {
            _equipSlot.ReleaseCB = releaseItemFromEquipSlot;
        }
    }

    void loadItemDatas()
    {
        if (userItems == null)
        {
            userItems = new List<UserItemInfo>();
        }

        foreach (UIEquipSlot equipSlot in equipSlotList)
        {
            equipSlot.SetData<UserItemInfo>(null);
        }
        #region test
        for (int i = 0; i < userItemNum; ++i)
        {
            UserItemInfo __n = new UserItemInfo();

            __n.UserID = 0;
            __n.ItemSeq = i;

            List<string> __icons = new List<string>()
            {
                "Helmet_1",
                "Helmet_2",
                "Helmet_3",
                "Helmet_4",
                "Helmet_5",
                "Helmet_6",
                "Necklace_1",
                "Necklace_2",
                "Necklace_3",
                "Necklace_4",
                "Necklace_5",
                "Necklace_6",
                "BodyArmor_1",
                "BodyArmor_2",
                "BodyArmor_3",
                "BodyArmor_4",
                "BodyArmor_5",
                "BodyArmor_6",
                "Belt_1",
                "Belt_2",
                "Belt_3",
                "Belt_4",
                "Belt_5",
                "Belt_6",
                "Ring_1",
                "Ring_2",
                "Ring_3",
                "Ring_4",
                "Ring_5",
                "Ring_6",
                "Boots_1",
                "Boots_2",
                "Boots_3",
                "Boots_4",
                "Boots_5",
                "Boots_6",
            };

            ResItemInfo __ni = new ResItemInfo();

            __ni.AtlasName = $"SAItemIcons";
            int _random = UnityEngine.Random.Range(0, __icons.Count);
            __ni.ItemType = _random / 6;
            __ni.SpriteName = $"{__icons[_random]}";
            __n.ItemData = __ni;
            __n.Grade = UnityEngine.Random.Range(0, 6);

            userItems.Add(__n);
        }
        #endregion


        filterDic = new Dictionary<ITEM_FILTER, string>
        {
            { ITEM_FILTER.GRADE, "ALL" },
            { ITEM_FILTER.TYPE, "ALL" }
        };

        updateGrid(userItems);
    }

    void updateGrid(List<UserItemInfo> _itemList)
    {
        if (inventoryGridLayout == null)
        {
            return;
        }
        
        int __count = _itemList.Count;

        if (userInvenList == null)
        {
            userInvenList = new List<UIInventorySlot>();
        }

        List<GridData> gridList = new List<GridData>();
        for (int i = 0; i < __count; ++i)
        {
            UserItemInfo __n = _itemList[i];

            gridList.Add(new GridData { GridIndex = i, Data = __n });
        }

        inventoryGridLayout.SetBuffer(20);

        inventoryGridLayout.Create(gridList, 
            //create callback
            (idx, slot) => { 
            UIInventorySlot gridslot = slot as UIInventorySlot;

            gridslot.EquipCB = equipItemFromInventory;
            gridslot.ReleaseCB = releaseItemFromInventory;

            UserItemInfo __d = gridList[idx].Data as UserItemInfo;

            gridslot.SetData(__d);

            userInvenList.Add(gridslot);
            },
            // refresh callback
            (idx, slot) =>
            {
                UIInventorySlot gridslot = slot as UIInventorySlot;

                gridslot.EquipCB = equipItemFromInventory;
                gridslot.ReleaseCB = releaseItemFromInventory;

                UserItemInfo __d = gridList[idx].Data as UserItemInfo;
                gridslot.SetData(__d);
            }, 
            // delete callback
            (slot) =>
            {
                UIInventorySlot gridslot = slot as UIInventorySlot;

                userInvenList.Remove(gridslot);
            });
    }

    UserItemInfo equipItemFromInventory(UserItemInfo _item)
    {
        if (_item == null)
            return null;

        if (equipSlotList == null || equipSlotList.Count == 0)
            return null;

        ITEM_PARTS SelectTYpe = _item.ITEM_PARTS_TYPE;

        UIEquipSlot __equipSlot = equipSlotList.Find(x => x.partsType == SelectTYpe);

        if (__equipSlot == null)
            return null;

        UserItemInfo __beforeEquipItem = __equipSlot.EquipItem(_item);

        UIInventorySlot __slot =  userInvenList.Find(x => x.GetData<UserItemInfo>() == __beforeEquipItem);

        if (__slot != null)
        {
            __slot.SetData(__beforeEquipItem);
        }

        return __beforeEquipItem;
    }

    void releaseItemFromInventory(UserItemInfo _item)
    {
        if (_item == null)
            return;

        if (equipSlotList == null || equipSlotList.Count == 0)
            return;

        ITEM_PARTS SelectTYpe = _item.ITEM_PARTS_TYPE;

        UIEquipSlot __equipSlot = equipSlotList.Find(x => x.partsType == SelectTYpe);

        if (__equipSlot == null)
            return;

        __equipSlot.ReleaseItem();
    }

    void releaseItemFromEquipSlot(UserItemInfo _item)
    {
        if (_item == null)
            return;

        UIInventorySlot __slot = userInvenList.Find(x => x.GetData<UserItemInfo>() == _item);

        if (__slot != null)
        {
            __slot.SetData(_item);
        }
    }

    //필터 기능
    public void GradeFilter(int __optionKey)
    {
        TMP_Dropdown.OptionData __d = gradeDropdown.options[__optionKey];

        filterDic[ITEM_FILTER.GRADE] = __d.text;

        filtering();
    }

    public void TypeFilter(int __optionKey)
    {
        TMP_Dropdown.OptionData __d = typeDropdown.options[__optionKey];

        filterDic[ITEM_FILTER.TYPE] = __d.text;

        filtering();
    }

    void filtering()
    {
        inventoryGridLayout?.GridRefresh();

        List<UserItemInfo> __filterList = userItems;

        if (__filterList == null)
        {
            return;
        }

        foreach (var item in filterDic)
        {
            if (item.Value == "ALL")
                continue;

            switch (item.Key)
            {
                case ITEM_FILTER.GRADE:
                    __filterList = __filterList.FindAll(x => x.GRADE_NAME == item.Value);
                    break;
                case ITEM_FILTER.TYPE:
                    __filterList = __filterList.FindAll(x => x.ITEM_PARTS_TYPE.GetEnumName() == item.Value);
                    break;
            }
        }

        if (__filterList != null && __filterList.Count > 0)
        {
            updateGrid(__filterList);
        }
    }

    void gridRefresh(object _object, EventArgs _args)
    {
        if (inventoryGridLayout == null)
            return;

        inventoryGridLayout.Clear();
        if (userInvenList != null)
        {
            userInvenList.Clear();
        }
    }
}
