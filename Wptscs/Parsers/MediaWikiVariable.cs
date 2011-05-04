// ================================================================================================
// <summary>
//      MediaWikiページの変数要素をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiVariable.cs" company="honeplusのメモ帳">
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
    /// MediaWikiページの変数要素をあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiVariable : AbstractElement
    {
        #region 定数

        /// <summary>
        /// 内部リンクの開始タグ。
        /// </summary>
        public static readonly string DelimiterStart = "{{{";

        /// <summary>
        /// 内部リンクの閉じタグ。
        /// </summary>
        public static readonly string DelimiterEnd = "}}}";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 変数要素をあらわす空のインスタンスを生成する。
        /// </summary>
        /// <param name="variable">変数。</param>
        /// <param name="value">値。</param>
        public MediaWikiVariable(string variable, IElement value = null)
        {
            this.Variable = variable;
            this.Value = value;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 変数。
        /// </summary>
        public virtual string Variable
        {
            get;
            set;
        }

        /// <summary>
        /// 値。
        /// </summary>
        public virtual IElement Value
        {
            get;
            set;
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// この要素を書式化した変数テキストを返す。
        /// </summary>
        /// <returns>変数テキスト。</returns>
        protected override string ToStringImpl()
        {
            // 戻り値初期化
            StringBuilder b = new StringBuilder();
            
            // 開始タグの付加
            b.Append(MediaWikiVariable.DelimiterStart);

            // 変数
            if (!String.IsNullOrEmpty(this.Variable))
            {
                b.Append(this.Variable);
            }

            // 値
            if (this.Value != null)
            {
                b.Append('|');
                b.Append(this.Value.ToString());
            }

            // 閉じタグの付加
            b.Append(MediaWikiVariable.DelimiterEnd);
            return b.ToString();
        }

        #endregion
    }
}
