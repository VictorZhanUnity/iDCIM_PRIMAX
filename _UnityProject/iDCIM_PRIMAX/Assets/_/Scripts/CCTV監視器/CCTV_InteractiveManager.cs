using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VictorDev.Managers;

/// <summary>
/// 互動物件管理器 - CCTV監視器
/// </summary>
public class CCTV_InteractiveManager : InteractiveManager
{
    [Header(">>> CCTV_InteractiveManager")]
    [SerializeField] private ToggleGroup toggleGroup;

    [Header(">>> 當點擊到Indicator時")]
    public UnityEvent<CCTV_DataHandler> onToggleChanged;

    private Dictionary<Transform, CCTV_DataHandler> dataHandlerDict { get; set; } = new Dictionary<Transform, CCTV_DataHandler>();

    /// <summary>
    /// 目前所選取的CCTV_DataHandler物件
    /// </summary>
    private CCTV_DataHandler selectedDataHandler = null;

    protected override void AddMoreComponentToObject(Collider target)
    {
        CCTV_LineIndicator indicator = target.transform.GetChild(0).GetComponent<CCTV_LineIndicator>();
        indicator.toggleGroup = toggleGroup;
        indicator.onToggleChanged.AddListener(OnToggleChangedHandler);

        CCTV_DataHandler dataHandler = target.GetComponent<CCTV_DataHandler>();
        dataHandler.ShaderMaterial = target.GetComponent<MeshRenderer>().material;

        dataHandlerDict[target.transform] = dataHandler;
    }
    private void Awake()
    {
        // 當點選模型物件時
        onClickInteractiveItem.AddListener((targetTrans) =>
        {
            dataHandlerDict[targetTrans].IsSelected = !dataHandlerDict[targetTrans].IsSelected;
        });
    }

    /// <summary>
    /// 當點選圖標Toggle或直接改變Toggle狀態時
    /// </summary>
    private void OnToggleChangedHandler(Transform targetTrans, bool isOn)
    {
        //取消目標選取狀態
        if (isOn == false) dataHandlerDict[targetTrans].IsSelected = false;
        else
        {
            if (selectedDataHandler != null)
            {
                //判斷點擊的對像是否為同一個
                if (dataHandlerDict[targetTrans] != selectedDataHandler)
                {
                    selectedDataHandler.IsSelected = false;
                    selectedDataHandler = dataHandlerDict[targetTrans];
                }
            }
            dataHandlerDict[targetTrans].IsSelected = true;
        }
        onToggleChanged.Invoke(dataHandlerDict[targetTrans]);
    }
    /// <summary>
    /// 設置Indicator顯示/隱藏 (供主選單Toggle使用)
    /// </summary>
    public void SetIndicatorVisible(bool isVisible)
    {
        SetOutlineVisible(isVisible);
        foreach (Transform target in dataHandlerDict.Keys)
        {
            dataHandlerDict[target].IsActivated = isVisible;
        }
    }

    private void OnValidate() => toggleGroup ??= GetComponent<ToggleGroup>();
}
