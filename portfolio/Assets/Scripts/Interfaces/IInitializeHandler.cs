using System;
/// <summary>
/// Manager, Controller, Object의 초기화, 해제, 갱신
/// </summary>
public interface IInitializeHandler
{
    event EventHandler OnInitialize;
    event EventHandler OnRelease;
    event EventHandler OnRefresh;
}