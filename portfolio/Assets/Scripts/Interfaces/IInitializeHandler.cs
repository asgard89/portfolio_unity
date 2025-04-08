using System;
/// <summary>
/// Manager, Controller, Object�� �ʱ�ȭ, ����, ����
/// </summary>
public interface IInitializeHandler
{
    event EventHandler OnInitialize;
    event EventHandler OnRelease;
    event EventHandler OnRefresh;
}