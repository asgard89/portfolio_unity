using ASGA.DS;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEquipSlot : UIBase, IPointerClickHandler
{
    public ITEM_PARTS partsType;

    DataLinker dataLinker;

    public Action<UserItemInfo> ReleaseCB = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        UserItemInfo __current = GetData<UserItemInfo>();
        if (__current != null)
        {
            ReleaseItem();
        }
    }

    public override T GetData<T>()
    {
        if (dataLinker == null)
        {
            return null;
        }

        return dataLinker.getData<T>();

    }

    public override void SetData<T>(T _data)
    {
        if (dataLinker == null)
        {
            dataLinker = GetComponent<DataLinker>();
        }

        if (dataLinker == null)
        {
            return;
        }

        if (_data == null)
        {
            dataLinker.setData<UserItemInfo>(null);
        }

        if (_data is UserItemInfo __data)
        {
            dataLinker.setData(__data);
        }
    }

    public UserItemInfo EquipItem(UserItemInfo _item)
    {
        if (_item == null)
        {
            return null;
        }

        UserItemInfo __currentItem = GetData<UserItemInfo>();

        if (__currentItem != null) 
        {
            __currentItem.IsEquip = false;
        }

        _item.IsEquip = true;

        SetData(_item);

        return __currentItem;
    }

    public void ReleaseItem()
    {
        UserItemInfo __currentItem = GetData<UserItemInfo>();

        if (__currentItem == null)
            return;
        __currentItem.IsEquip = false;

        ReleaseCB?.Invoke(__currentItem);

        SetData<UserItemInfo>(null);
    }
}
