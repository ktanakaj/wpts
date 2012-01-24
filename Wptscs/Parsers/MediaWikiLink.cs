// ================================================================================================
// <summary>
//      MediaWikiページの内部リンク要素をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiLink.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Honememo.Parsers;

    /// <summary>
    /// MediaWikiページの内部リンク要素をあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiLink : AbstractElement
    {
        #region 定数

        /// <summary>
        /// 内部リンクの開始タグ。
        /// </summary>
        public static readonly string DelimiterStart = "[[";

        /// <summary>
        /// 内部リンクの閉じタグ。
        /// </summary>
        public static readonly string DelimiterEnd = "]]";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたタイトルの内部リンク要素をあらわすインスタンスを生成する。
        /// </summary>
        /// <param name="title">記事名。</param>
        public MediaWikiLink(string title) : this()
        {
            this.Title = title;
        }

        /// <summary>
        /// 内部リンク要素をあらわす空のインスタンスを生成する。
        /// </summary>
        public MediaWikiLink()
        {
            this.PipeTexts = new List<IElement>();
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// リンクの記事名。
        /// </summary>
        /// <remarks>リンクに記載されていた記事名であり、名前空間の情報などは含まない可能性があるため注意。</remarks>
        public virtual string Title
        {
            get;
            set;
        }

        /// <summary>
        /// リンクのセクション名（#）。
        /// </summary>
        public virtual string Section
        {
            get;
            set;
        }

        /// <summary>
        /// リンクのパイプ後の文字列（|）。
        /// </summary>
        public virtual IList<IElement> PipeTexts
        {
            get;
            set;
        }

        /// <summary>
        /// 言語間または他プロジェクトへのリンクの場合、コード。
        /// </summary>
        public virtual string Code
        {
            get;
            set;
        }

        /// <summary>
        /// リンクの先頭が : で始まるかを示すフラグ。
        /// </summary>
        public virtual bool IsColon
        {
            get;
            set;
        }

        /// <summary>
        /// 記事名の先頭がサブページを示す / で始まるか？
        /// </summary>
        /// <remarks>※ 2011年5月現在、この処理には不足あり。</remarks>
        public virtual bool IsSubpage
        {
            // TODO: サブページには相対パスで[[../～]]や[[../../～]]というような書き方もある模様。
            //       この辺りの処理は[[Help:サブページ]]を元に全面的に見直す必要あり
            get;
            set;
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// この要素を書式化した内部リンクテキストを返す。
        /// </summary>
        /// <returns>内部リンクテキスト。</returns>
        protected override string ToStringImpl()
        {
            // 戻り値初期化
            StringBuilder b = new StringBuilder();
            
            // 開始タグの付加
            b.Append(MediaWikiLink.DelimiterStart);

            // 先頭の : の付加
            if (this.IsColon)
            {
                b.Append(':');
            }

            // 言語コード・他プロジェクトコードの付加
            if (!String.IsNullOrEmpty(this.Code))
            {
                b.Append(this.Code);
                b.Append(':');
            }

            // 記事名の付加
            if (!String.IsNullOrEmpty(this.Title))
            {
                b.Append(this.Title);
            }

            // セクション名の付加
            if (this.Section != null)
            {
                b.Append('#');
                b.Append(this.Section);
            }

            // パイプ後の文字列の付加
            if (this.PipeTexts != null)
            {
                foreach (IElement p in this.PipeTexts)
                {
                    b.Append('|');
                    b.Append(p.ToString());
                }
            }

            // 閉じタグの付加
            b.Append(MediaWikiLink.DelimiterEnd);
            return b.ToString();
        }

        #endregion
    }
}
