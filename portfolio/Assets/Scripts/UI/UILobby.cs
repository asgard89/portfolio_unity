using UnityEngine;
using System.Collections.Generic;

public class UILobby : UIBase
{
    [SerializeField]
    List<UILobbySlot> uILobbySlots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        InitializeUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void close()
    {
        MainProc.Instance.GetMGR<UIManager>().UnLoadUI<UILobby>(this);
    }

    protected override void InitializeUI()
    {
        foreach (UILobbySlot slot in uILobbySlots)
        {
            slot.OnClickCB += close;
        }
    }
}
