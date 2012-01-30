// ================================================================================================
// <summary>
//      ページや文字列のテキスト要素をあらわすモデルクラスソース</summary>
//
// <copyright file="TextElement.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;

    /// <summary>
    /// ページや文字列のテキスト要素をあらわすモデルクラスです。
    /// </summary>
    /// <remarks>テキストを扱うだけの単純な要素。</remarks>
    public class TextElement : AbstractElement
    {
        #region コンストラクタ

        /// <summary>
        /// 指定されたテキストを用いて要素を作成する。
        /// </summary>
        /// <param name="text">テキスト文字列、未指定時は<c>null</c>。</param>
        public TextElement(string text = null)
        {
            this.Text = text;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// このテキスト要素のテキスト。
        /// </summary>
        public virtual string Text
        {
            get;
            set;
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// このテキスト要素のテキストを返す。
        /// </summary>
        /// <returns>このテキスト要素のテキスト。</returns>
        protected override string ToStringImpl()
        {
            return this.Text;
        }

        #endregion
    }
}
