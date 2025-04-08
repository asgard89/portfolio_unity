using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITitle : UIBase, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogError("## Click Title");

        MainProc.Instance.GetMGR<UIManager>().LoadUI<UILobby>("UI_Lobby", (uilobby) =>
        {
            MainProc.Instance.GetMGR<UIManager>().UnLoadUI<UITitle>(this);
        });

        
    }
}
