// ================================================================================================
// <summary>
//      ページの各要素をあらわすモデルインタフェースソース</summary>
//
// <copyright file="IPageElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    /// <summary>
    /// ページの各要素をあらわすモデルのインタフェースです。
    /// </summary>
    public interface IPageElement
    {
        /// <summary>
        /// 各要素を書式化したテキスト形式で返す。
        /// </summary>
        /// <returns>書式化したテキスト。<c>null</c>は返さないこと。</returns>
        string ToString();
    }
}
