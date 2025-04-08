using System;
using UnityEngine;
using ASGA.DS;
using UnityEngine.EventSystems;

public class UIInventorySlot : UIBase, IPointerClickHandler
{
    DataLinker dataLinker;
    //DataLinker�� ���
    //DataLinker�� PropertyLinker�� ���
    //PropertyLinker�� PropertyOption�� ���

    public Func<UserItemInfo, UserItemInfo> EquipCB = null;
    public Action<UserItemInfo> ReleaseCB = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        UserItemInfo __current = GetData<UserItemInfo>();
        if (__current != null)
        {
            if (__current.IsEquip == false)
            {
                EquipItem();
            }
            else
            {
                ReleaseItem();
            }
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

        if (_data is UserItemInfo __data)
        {
            dataLinker.setData(__data);
        }
    }

    //Popup ����
    public void EquipItem()
    {
        UserItemInfo __itemInfo = GetData<UserItemInfo>();

        if (__itemInfo == null)
        {
            return;
        }

        if (__itemInfo.IsEquip == true)
        {
            return;
        }

        EquipCB?.Invoke(__itemInfo);

        SetData(__itemInfo);
    }

    public void ReleaseItem()
    {
        UserItemInfo __itemInfo = GetData<UserItemInfo>();

        if (__itemInfo == null)
        {
            return;
        }

        if (__itemInfo.IsEquip == false)
        {
            return;
        }

        __itemInfo.IsEquip = false;
        ReleaseCB?.Invoke(__itemInfo);

        SetData(__itemInfo);
    }



    public override void Refresh()
    {

    }
}
