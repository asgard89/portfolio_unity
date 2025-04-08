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

    int totalCount;                     //전체 데이터 수

    //캐싱
    RectTransform contentRect;
    RectTransform viewPortRect;

    List<GridData> allItemList;         //전체 데이터 list
    List<UIBase> currentItemList;       //그리드 아이템으로 생성된 list 
    //
    Queue<UIBase> poolBuffer;           //풀링 버퍼

    Action<int, UIBase> onCreateCB;     //그리드 아이템이 생성되고 실행.
    Action<int, UIBase> onRefreshData;  //그리드 아이템을 버퍼에서 꺼낸 후 실행. (데이터 초기화)
    Action<UIBase> onDeleteCB;          //그리드 아이템이 실제로 Destroy 되어서 삭제될 때 실행.

    int currentStartRow;                //현재 시작 Row Index
    int currentEndRow;                  //현재 끝 Row Index
    int bufferSize = 10;                //풀링 버퍼 사이즈

    //기본 padding value
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

        onCreateCB = _onCreate; //그리드 아이템 생성 후 각 아이템에서 실행할 로직
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

        //그리드 설정상 생성된 그리드아이템에 맞추서 스크롤 핏이 이루어지기 때문에
        //padding을 전체 아이템을 생성했을 때 사이즈로 설정해놓고
        //그리드아이템을 풀링하여 생성 삭제할 때 padding값을 같이 조절하여 스크롤이되도록 한다.

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
        //실제 인덱스가 들어옴
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

        __item.transform.SetSiblingIndex(__currentIdx); //하이어라키에서의 위치

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

        //캐싱한 아이템이 있고 이 수가 설정한 캐시크기에 꽉 차면
        //맨 앞의 데이터를 삭제하고 추가
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
    /// 현재 Grid에서의 인덱스를 구하는 함수
    /// </summary>
    int GetCurrentItemInedx(int _index)
    {
        //startRow
        //view안에 있는 item 갯수 이내로 나와야 함.
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

    //스크롤을 내릴 때
    void DownScrollFunc()
    {
        ///최상단에 새로운 줄을 추가.
        ///이 때는 안보이는 위치의 줄이 내려와서 생성
        if (currentStartRow > StartRowIDX)
        {
            //.LogError($"1Scroll Chcek : {currentStartRow}, {currentStartRow - 1}, {StartRowIDX}");
            //현재 줄 바로 윗 줄 추가 
            for (int i = currentStartRow - 1; i >= StartRowIDX; --i)
            {
                createItemsInRow(i);
                //확보해놨던 실제로 없는 공간을 제거
                padding.top -= (int)(cellSize.y + spacing.y);
            }
            //현재 줄 갱신
            currentStartRow = StartRowIDX;
        }

        ///최하단에 있던 줄을 삭제
        ///최하단에 있던 줄이 영영밖으로 내려가서 삭제
        if (currentEndRow > EndRowIDX)
        {
            //Debug.LogError($"2Scroll Chcek : {currentEndRow} , {EndRowIDX}");
            for (int i = currentEndRow; i > EndRowIDX; --i)
            {
                deleteItemsInRow(i);
                //실제로 없는 공간을 확보
                padding.bottom += (int)(cellSize.y + spacing.y);
            }
            //현재 줄 갱신
            currentEndRow = EndRowIDX;
        }
    }
    
    //스크롤 올릴 때
    void UpScrollFunc()
    {
        ///최상단에 있던 줄이 영역 위로 올라가면서 삭제
        if (currentStartRow < StartRowIDX)
        {
            //Debug.LogError($"3Scroll Chcek : {currentStartRow} , {StartRowIDX}");
            for (int i = currentStartRow; i < StartRowIDX; ++i)
            {
                deleteItemsInRow(i);
                //실제로 없는 공간을 확보
                padding.top += (int)(cellSize.y + spacing.y);
            }

            //현재 줄 갱신
            currentStartRow = StartRowIDX;
        }

        ///최하단에 새로운 줄을 추가
        ///영역 밖에 있던 줄이 올라와서 생성
        if (currentEndRow < EndRowIDX)
        {
            //Debug.LogError($"4Scroll Chcek : {currentEndRow} , {currentEndRow + 1} ,{EndRowIDX}");
            for (int i = currentEndRow + 1; i <= EndRowIDX; ++i)
            {
                createItemsInRow(i);
                //확보해놨던 실제로 없는 공간을 제거
                padding.bottom -= (int)(cellSize.y + spacing.y);
            }
            //현재 줄 갱신
            currentEndRow = EndRowIDX;
        }
    }
}
