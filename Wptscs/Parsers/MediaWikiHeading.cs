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
        private static readonly string delimiterStart = "=";

        /// <summary>
        /// 見出しの閉じ文字。
        /// </summary>
        private static readonly string delimiterEnd = "=";

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

        #region 静的メソッド

        /// <summary>
        /// 渡されたテキストをMediaWikiの見出しとして解析する。
        /// </summary>
        /// <param name="text">=で始まる文字列。</param>
        /// <param name="parser">解析に使用するパーサー。</param>
        /// <param name="link">解析した見出し。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public static bool TryParse(string text, IParser parser, out MediaWikiHeading link)
        {
            // 出力値初期化
            link = null;

            // 入力値確認
            if (!text.StartsWith(MediaWikiHeading.delimiterStart))
            {
                return false;
            }

            // TODO: 未実装
            return false;
        }

        /// <summary>
        /// 渡された文字が<c>TryParse</c>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        /// <remarks>性能対策などで処理自体を呼ばせたく無い場合用。</remarks>
        public static bool IsElementPossible(char c)
        {
            return MediaWikiHeading.delimiterStart[0] == c;
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
                b.Append(MediaWikiHeading.delimiterStart);
            }

            // 見出し文字列の設定
            b.Append(base.ToStringImpl());

            // 閉じ文字の付加
            for (int i = 0; i < this.Level; i++)
            {
                b.Append(MediaWikiHeading.delimiterEnd);
            }

            return b.ToString();
        }

        #endregion
    }
}
