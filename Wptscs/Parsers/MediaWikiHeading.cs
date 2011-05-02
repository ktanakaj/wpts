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
    public class MediaWikiHeading : IElement
    {
        #region 定数

        /// <summary>
        /// 見出しの開始文字。
        /// </summary>
        private static readonly string startSign = "=";

        /// <summary>
        /// 見出しの閉じ文字。
        /// </summary>
        private static readonly string endSign = "=";

        #endregion
        
        #region インタフェース実装プロパティ

        /// <summary>
        /// この要素の文字数。
        /// </summary>
        public virtual int Length
        {
            get
            {
                return this.Original != null ? this.Original.Length : this.ToString().Length;
            }
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// リンクの記事名。
        /// </summary>
        /// <remarks>リンクに記載されていた記事名であり、名前空間の情報などは含まない可能性があるため注意。</remarks>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Parse等によりインスタンスを生成した場合の元文字列。
        /// </summary>
        protected virtual string Original
        {
            // ※ 本当はParseしてから値が変更されていない場合、
            //    ToStringでこの値を返すとしたいが、
            //    子要素があるのでこのクラスでは実現困難。
            get;
            set;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 渡されたテキストをMediaWikiの内部リンクとして解析する。
        /// </summary>
        /// <param name="text">[[で始まる文字列。</param>
        /// <param name="parser">解析に使用するパーサー。</param>
        /// <param name="link">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public static bool TryParse(string text, IParser parser, out MediaWikiHeading link)
        {
            // 出力値初期化
            link = null;

            // 入力値確認
            if (!text.StartsWith(MediaWikiHeading.startSign))
            {
                return false;
            }

            // TODO: 未実装
            return true;
        }

        /// <summary>
        /// 渡されたテキストをMediaWikiの内部リンクとして解析する。
        /// </summary>
        /// <param name="text">[[で始まる文字列。</param>
        /// <param name="link">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public static bool TryParse(string s, out MediaWikiHeading result)
        {
            // パーサーにMediaWikiParserの標準設定を指定して解析
            return MediaWikiHeading.TryParse(s, new MediaWikiParser(), out result);
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// この要素を書式化した内部リンクテキストを返す。
        /// </summary>
        /// <returns>内部リンクテキスト。</returns>
        public override string ToString()
        {
            // 戻り値初期化
            StringBuilder b = new StringBuilder();
            
            // 開始タグの付加
            b.Append(MediaWikiHeading.startSign);


            // 閉じタグの付加
            b.Append(MediaWikiHeading.endSign);
            return b.ToString();
        }

        #endregion
    }
}
