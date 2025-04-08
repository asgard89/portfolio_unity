using ASGA.DS;
using System;
using System.Collections.Generic;

public interface IPoolingScroller 
{
    float ViewportWidth { get; }
    float ViewportHeight { get; }

    int RowCount { get; }

    int StartRowIDX { get; } //��ü �� �� ���� ����
    int EndRowIDX { get; } //��ü �� �� ���� ��

    int StartColIDX { get; }
    int EndColIDX { get; }

    void Create(List<GridData> _itemList, Action<int, UIBase> _onCreate, Action<int, UIBase> _onRefresh, Action<UIBase> _onDelete);

    void SetBuffer(int _size);

    void Clear();

    void ClearBuffer();
    
}
