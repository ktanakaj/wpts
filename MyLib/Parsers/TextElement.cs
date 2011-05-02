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
    public class TextElement : IElement
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
        
        #region インタフェース実装プロパティ

        /// <summary>
        /// この要素の文字数。
        /// </summary>
        public virtual int Length
        {
            get
            {
                return this.ToString().Length;
            }
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
