// ================================================================================================
// <summary>
//      MediaWikiページの見出し要素をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiHeading.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Honememo.Parsers;
    using Honememo.Utilities;

    /// <summary>
    /// MediaWikiページの見出し要素をあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiHeading : ListElement
    {
        #region 定数

        /// <summary>
        /// 見出しの開始文字。
        /// </summary>
        public static readonly char DelimiterStart = '=';

        /// <summary>
        /// 見出しの閉じ文字。
        /// </summary>
        public static readonly char DelimiterEnd = '=';

        #endregion

        #region プロパティ

        /// <summary>
        /// 見出し階層。
        /// </summary>
        public int Level
        {
            get;
            set;
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// この要素を書式化した見出し文字列を返す。
        /// </summary>
        /// <returns>見出し文字列。</returns>
        protected override string ToStringImpl()
        {
            // 戻り値初期化
            StringBuilder b = new StringBuilder();

            // 開始文字の付加
            for (int i = 0; i < this.Level; i++)
            {
                b.Append(MediaWikiHeading.DelimiterStart);
            }

            // 見出し文字列の設定
            b.Append(base.ToStringImpl());

            // 閉じ文字の付加
            for (int i = 0; i < this.Level; i++)
            {
                b.Append(MediaWikiHeading.DelimiterEnd);
            }

            return b.ToString();
        }

        #endregion
    }
}
