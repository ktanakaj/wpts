// ================================================================================================
// <summary>
//      ページのテキスト要素をあらわすモデルクラスソース</summary>
//
// <copyright file="TextElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;

    /// <summary>
    /// ページのテキスト要素をあらわすモデルクラスです。
    /// </summary>
    /// <remarks>テキストを扱うだけの単純なページ要素。</remarks>
    public class TextElement : IPageElement
    {
        #region プロパティ

        /// <summary>
        /// このテキスト要素のテキスト。
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// このテキスト要素のテキストを返す。
        /// </summary>
        /// <returns>このテキスト要素のテキスト。</returns>
        public override string ToString()
        {
            return this.Text != null ? this.Text : String.Empty;
        }

        #endregion
    }
}
