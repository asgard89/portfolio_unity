using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using ASGA.DS;
using System.Linq;
using UnityEngine.LightTransport;
using static UnityEngine.Rendering.DebugUI;

public class PoolingGridLayoutGroup : GridLayoutGroup, IPoolingScroller, IInitializeHandler
{
    public UIBase gridItem;
    public ScrollRect scrollRect;

    int totalCount;                     //��ü ������ ��

    //ĳ��
    RectTransform contentRect;
    RectTransform viewPortRect;

    List<GridData> allItemList;         //��ü ������ list
    List<UIBase> currentItemList;       //�׸��� ���������� ������ list 
    //
    Queue<UIBase> poolBuffer;           //Ǯ�� ����

    Action<int, UIBase> onCreateCB;     //�׸��� �������� �����ǰ� ����.
    Action<int, UIBase> onRefreshData;  //�׸��� �������� ���ۿ��� ���� �� ����. (������ �ʱ�ȭ)
    Action<UIBase> onDeleteCB;          //�׸��� �������� ������ Destroy �Ǿ ������ �� ����.

    int currentStartRow;                //���� ���� Row Index
    int currentEndRow;                  //���� �� Row Index
    int bufferSize = 10;                //Ǯ�� ���� ������

    //�⺻ padding value
    int baseTop;
    int baseBottom;
    int baseLeft;
    int baseRight;

    GameObject gridItembuffer;

    #region Derived Interface - IPoolingScroller
    
    public float ViewportWidth
    {
        get
        {
            if (viewPortRect == null)
            {
                return 0;
            }

            return viewPortRect.rect.width;
        }
    }

    public float ViewportHeight
    {
        get
        {
            if (viewPortRect == null)
            {
                return 0;
            }

            return viewPortRect.rect.height;
        }
    }

    public int RowCount => (totalCount % constraintCount == 0) ? (totalCount / constraintCount) : (totalCount / constraintCount) + 1;

    public int StartRowIDX
    {
        get
        {
            if (contentRect == null)
            {
                return 0;
            }

            int __ret = (int)((contentRect.anchoredPosition.y - baseTop) / (cellSize.y + spacing.y));

            return Mathf.Clamp(__ret, 0, RowCount - 1);
        }
    }

    public int EndRowIDX
    {
        get
        {
            if (contentRect == null)
            {
                return 0;
            }
            
            int __ret = (int)((contentRect.anchoredPosition.y + ViewportHeight - baseTop) / (cellSize.y + spacing.y));

            return Mathf.Clamp(__ret, 0, RowCount - 1);
        }
    }

    public int StartColIDX
    {
        get
        {
            if (contentRect == null)
            {
                return 0;
            }

            int __ret = (int)((-contentRect.anchoredPosition.x - baseLeft) / (cellSize.x + spacing.x));
            return Mathf.Clamp(__ret, 0, constraintCount - 1);
        }
    }

    public int EndColIDX
    {
        get
        {
            if (contentRect == null)
            {
                return 0;
            }

            int __ret = (int)((-contentRect.anchoredPosition.x + ViewportWidth - baseLeft) / (cellSize.x + spacing.x));

            return Mathf.Clamp(__ret, 0, constraintCount - 1);
        }
    }
    
    public void Clear()
    {
        onCreateCB = null;
        onRefreshData = null;
        onDeleteCB = null;

        if (allItemList != null)
        {
            allItemList.Clear();
            allItemList = null;
        }

        if (currentItemList != null)
        {
            foreach (UIBase item in currentItemList)
            {
                Destroy(item.gameObject);
            }

            currentItemList.Clear();
            currentItemList = null;
        }

        padding.top = baseTop;
        padding.bottom = baseBottom;
        padding.left = baseLeft;
        padding.right = baseRight;

        totalCount = 0;

        contentRect.anchoredPosition = Vector2.zero;

        ClearBuffer();
    }

    public void ClearBuffer()
    {
        if (gridItembuffer != null)
        {
            Destroy(gridItembuffer);
            gridItembuffer = null;
        }

        if (poolBuffer != null)
        {
            poolBuffer.Clear();
            poolBuffer = null;
        }
    }

    public void Create(List<GridData> _itemList ,Action<int, UIBase> _onCreate, Action<int, UIBase> _onRefresh, Action<UIBase> _onDelete)
    {
        allItemList = _itemList;

        totalCount = (allItemList == null) ? 0 : allItemList.Count;

        OnInitialize = initalize;
        OnInitialize?.Invoke(this, EventArgs.Empty);

        onCreateCB = _onCreate; //�׸��� ������ ���� �� �� �����ۿ��� ������ ����
        onRefreshData = _onRefresh;
        onDeleteCB = _onDelete;

        gridItembuffer = new GameObject("Buffer");
        gridItembuffer.transform.SetParent(transform);
        gridItembuffer.SetActive(false);

        createGridItems();
    }

    public void SetBuffer(int _size)
    {
        bufferSize = _size;
    }
    #endregion

    #region Derived Interface - IInitializeHandler

    public event EventHandler OnInitialize;
    public event EventHandler OnRelease;
    public event EventHandler OnRefresh;

    #endregion

    protected override void OnDisable()
    {
        base.OnDisable();

        OnRelease?.Invoke(this, EventArgs.Empty);

        OnInitialize = null;
        OnRefresh = null;
        OnRelease = null;
    }


    void initalize(object _o, EventArgs _Args)
    {
        contentRect = GetComponent<RectTransform>();
        viewPortRect = scrollRect?.viewport;

        baseTop = padding.top;
        baseBottom = padding.bottom;
        baseLeft = padding.left;
        baseRight = padding.right;

        float __width = cellSize.x * constraintCount + spacing.x * (constraintCount - 1) + baseLeft + baseRight;
        float __height = cellSize.y * RowCount + spacing.y * (RowCount - 1) + baseTop + baseBottom;

        contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, __width);
        contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, __height);

        currentStartRow = StartRowIDX;
        currentEndRow = EndRowIDX;

        //�׸��� ������ ������ �׸�������ۿ� ���߼� ��ũ�� ���� �̷������ ������
        //padding�� ��ü �������� �������� �� ������� �����س���
        //�׸���������� Ǯ���Ͽ� ���� ������ �� padding���� ���� �����Ͽ� ��ũ���̵ǵ��� �Ѵ�.

        int initPaddingBottom = baseBottom + (int)((RowCount - EndRowIDX - 1) * (cellSize.y + spacing.y));

        padding.top = baseTop; //0
        padding.bottom = baseBottom + initPaddingBottom;

        scrollRect?.onValueChanged.AddListener(onScroll);
    }

    public void GridRefresh()
    {
        OnRefresh?.Invoke(this, EventArgs.Empty);
    }

    void createGridItems()
    {
        //Debug.LogError($"# createGridItems :  {StartRowIDX}, {EndRowIDX} , {EndColIDX}");
        if (totalCount == 0)
        {
            return;
        }

        for (int i = 0; i <= EndRowIDX; ++i)
        {
            for (int j = 0; j <= EndColIDX; ++j)
            {
                int _index = GetItemIndex(i, j);
                createItem(_index);
            }
        }
    }

    void createItem(int _index) 
    {
        //���� �ε����� ����
        //Debug.LogError($"## createItem {_index}");
        if (totalCount <= _index)
        {
            return;
        }

        UIBase __item;
        if (poolBuffer == null)
        {
            poolBuffer = new Queue<UIBase>();
        }

        if (poolBuffer.Count > 0)
        {
            __item = poolBuffer.Dequeue();

            __item.transform.SetParent(contentRect);

            onRefreshData?.Invoke(_index, __item);
        }
        else
        {
            __item = Instantiate(gridItem, contentRect) as UIBase;

            onCreateCB?.Invoke(_index, __item);
        }

        if (currentItemList == null)
        {
            currentItemList = new List<UIBase>();
        }

        int __currentIdx = GetCurrentItemInedx(_index);

        __item.gameObject.name = $"{_index} - {__currentIdx}";

        //Enable
        CanvasGroup __cg = __item.GetOrAddComponent<CanvasGroup>();
        __cg.alpha = 1f;

        __item.gameObject.SetActive(true);

        __item.transform.SetSiblingIndex(__currentIdx); //���̾��Ű������ ��ġ

        //Debug.LogError($"## createItem {_index} - {__currentIdx}");

        currentItemList.Add(__item);
    }

    void deleteItem(int _index)
    {
        GridData __d = allItemList.Find(x => x.GridIndex == _index);

        UIBase _deleteItem = currentItemList.Find(x => x.GetData<BaseInfoData>() == __d.Data);

        if (_deleteItem == null)
        {
            //error
            return;
        }
        currentItemList.Remove(_deleteItem);
        CanvasGroup __cg = _deleteItem.gameObject.GetComponent<CanvasGroup>();
        __cg.alpha = 0f;

        _deleteItem.gameObject.SetActive(false);
        _deleteItem.transform.SetParent(gridItembuffer.transform);

        if (poolBuffer == null)
        {
            poolBuffer = new Queue<UIBase>();
        }

        //ĳ���� �������� �ְ� �� ���� ������ ĳ��ũ�⿡ �� ����
        //�� ���� �����͸� �����ϰ� �߰�
        if (bufferSize > 0)
        {
            if (poolBuffer.Count == bufferSize)
            {
                UIBase __deleteCacheItem = poolBuffer.Dequeue();
                onDeleteCB?.Invoke(__deleteCacheItem);
                Destroy(__deleteCacheItem.gameObject);
            }
            poolBuffer.Enqueue(_deleteItem);
        }
        else
        {
            onDeleteCB?.Invoke(_deleteItem);
            Destroy(_deleteItem.gameObject);
        }
    }

    int GetItemIndex(int _rowIndex, int _colIndex)
    {
        return constraintCount * _rowIndex + _colIndex;
    }

    /// <summary>
    /// ���� Grid������ �ε����� ���ϴ� �Լ�
    /// </summary>
    int GetCurrentItemInedx(int _index)
    {
        //startRow
        //view�ȿ� �ִ� item ���� �̳��� ���;� ��.
        int __idxRow = (_index == 0) ? 0 : _index / constraintCount;
        int __idxCol = (_index == 0) ? 0 : _index % constraintCount;

        int __curIdx = __idxRow * constraintCount + __idxCol;

        if (StartRowIDX <= __idxRow)
        {
            __curIdx = (__idxRow - StartRowIDX) * constraintCount + __idxCol;
        }

        return __curIdx;
    }

    void createItemsInRow(int _rowIndex)
    {
        int thisRowEnd = GetItemIndex(_rowIndex, EndColIDX);

        if (thisRowEnd >= totalCount)
        {
            thisRowEnd = totalCount - 1;
        }

        int thisRowStart = GetItemIndex(_rowIndex, StartColIDX);

        for (int i = thisRowStart; i <= thisRowEnd; ++i)
        {
            createItem(i);
        }
    }

    void deleteItemsInRow(int _rowIndex)
    {
        int thisRowEnd = GetItemIndex(_rowIndex, EndColIDX);

        if (thisRowEnd >= totalCount)
        {
            thisRowEnd = totalCount - 1;
        }

        int thisRowStart = GetItemIndex(_rowIndex, StartColIDX);

        for (int i = thisRowStart; i <= thisRowEnd; ++i)
        {
            deleteItem(i);
        }
    }

    void onScroll(Vector2 _position)
    {
        if (true == isImpossibleScroll())
        {
            return;
        }

        //Debug.LogError($"{StartRowIDX},{EndRowIDX}");

        DownScrollFunc();
        UpScrollFunc();
    }

    bool isImpossibleScroll()
    {
        return ((currentStartRow > EndRowIDX) || (currentEndRow < StartRowIDX));
    }

    //��ũ���� ���� ��
    void DownScrollFunc()
    {
        ///�ֻ�ܿ� ���ο� ���� �߰�.
        ///�� ���� �Ⱥ��̴� ��ġ�� ���� �����ͼ� ����
        if (currentStartRow > StartRowIDX)
        {
            //.LogError($"1Scroll Chcek : {currentStartRow}, {currentStartRow - 1}, {StartRowIDX}");
            //���� �� �ٷ� �� �� �߰� 
            for (int i = currentStartRow - 1; i >= StartRowIDX; --i)
            {
                createItemsInRow(i);
                //Ȯ���س��� ������ ���� ������ ����
                padding.top -= (int)(cellSize.y + spacing.y);
            }
            //���� �� ����
            currentStartRow = StartRowIDX;
        }

        ///���ϴܿ� �ִ� ���� ����
        ///���ϴܿ� �ִ� ���� ���������� �������� ����
        if (currentEndRow > EndRowIDX)
        {
            //Debug.LogError($"2Scroll Chcek : {currentEndRow} , {EndRowIDX}");
            for (int i = currentEndRow; i > EndRowIDX; --i)
            {
                deleteItemsInRow(i);
                //������ ���� ������ Ȯ��
                padding.bottom += (int)(cellSize.y + spacing.y);
            }
            //���� �� ����
            currentEndRow = EndRowIDX;
        }
    }
    
    //��ũ�� �ø� ��
    void UpScrollFunc()
    {
        ///�ֻ�ܿ� �ִ� ���� ���� ���� �ö󰡸鼭 ����
        if (currentStartRow < StartRowIDX)
        {
            //Debug.LogError($"3Scroll Chcek : {currentStartRow} , {StartRowIDX}");
            for (int i = currentStartRow; i < StartRowIDX; ++i)
            {
                deleteItemsInRow(i);
                //������ ���� ������ Ȯ��
                padding.top += (int)(cellSize.y + spacing.y);
            }

            //���� �� ����
            currentStartRow = StartRowIDX;
        }

        ///���ϴܿ� ���ο� ���� �߰�
        ///���� �ۿ� �ִ� ���� �ö�ͼ� ����
        if (currentEndRow < EndRowIDX)
        {
            //Debug.LogError($"4Scroll Chcek : {currentEndRow} , {currentEndRow + 1} ,{EndRowIDX}");
            for (int i = currentEndRow + 1; i <= EndRowIDX; ++i)
            {
                createItemsInRow(i);
                //Ȯ���س��� ������ ���� ������ ����
                padding.bottom -= (int)(cellSize.y + spacing.y);
            }
            //���� �� ����
            currentEndRow = EndRowIDX;
        }
    }
}
