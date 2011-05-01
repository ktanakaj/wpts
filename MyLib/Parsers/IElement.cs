// ================================================================================================
// <summary>
//      ページや文字列の各要素をあらわすモデルインタフェースソース</summary>
//
// <copyright file="IElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    /// <summary>
    /// ページや文字列の各要素をあらわすモデルのインタフェースです。
    /// </summary>
    public interface IElement
    {
        #region メソッド

        /// <summary>
        /// 各要素を書式化したテキスト形式で返す。
        /// </summary>
        /// <returns>書式化したテキスト。<c>null</c>は返さないこと。</returns>
        string ToString();

        #endregion
    }
}
