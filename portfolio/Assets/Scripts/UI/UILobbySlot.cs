using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class UILobbySlot : UIBase, IPointerClickHandler
{
    [SerializeField]
    string openUIKey;
    [SerializeField]
    string slotName;

    [SerializeField]
    TMP_Text tmproSlotName;

    public Action OnClickCB = null;

    public override void Start()
    {
        base.Start();

        InitializeUI();
    }

    public void OnDisable()
    {
        OnClickCB = null;
    }

    protected override void InitializeUI()
    {
        base.InitializeUI();

        if (tmproSlotName != null)
        {
            tmproSlotName.SetText(slotName);
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"## Open UI : [{openUIKey}]");
        MainProc.Instance.GetMGR<UIManager>().LoadUI<UIBase>(openUIKey, (uicontents) =>
        {
            OnClickCB?.Invoke();
        });

    }
}
