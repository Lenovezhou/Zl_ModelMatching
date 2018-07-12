using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanelItem {

    /// <summary>
    /// 进入该页面的刷新
    /// </summary>
    void OnEnterThisPage();

    /// <summary>
    /// 离开该页面的刷新
    /// </summary>
    void OnLeveThisPage();


}
