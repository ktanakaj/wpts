// ================================================================================================
// <summary>
//      MediaWikiページの内部リンク要素をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiLink.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Websites
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Honememo.Utilities;

    /// <summary>
    /// MediaWikiページの内部リンク要素をあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiLink
    {
        #region 定数

        /// <summary>
        /// 内部リンクの開始タグ。
        /// </summary>
        private static readonly string startSign = "[[";

        /// <summary>
        /// 内部リンクの閉じタグ。
        /// </summary>
        private static readonly string endSign = "]]";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 内部リンク要素をあらわす空のインスタンスを生成する。
        /// </summary>
        public MediaWikiLink()
        {
            this.PipeTexts = new List<string>();
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
        /// リンクのセクション名（#）。
        /// </summary>
        public string Section
        {
            get;
            set;
        }

        /// <summary>
        /// リンクのパイプ後の文字列（|）。
        /// </summary>
        public IList<string> PipeTexts
        {
            get;
            set;
        }

        /// <summary>
        /// 言語間または他プロジェクトへのリンクの場合、コード。
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// リンクの先頭が : で始まるかを示すフラグ。
        /// </summary>
        public bool IsColon
        {
            get;
            set;
        }

        /// <summary>
        /// 記事名の先頭がサブページを示す / で始まるか？
        /// </summary>
        /// <remarks>※ 2011年5月現在、この処理には不足あり。</remarks>
        public bool IsSubpage
        {
            // TODO: サブページには相対パスで[[../～]]や[[../../～]]というような書き方もある模様。
            //       この辺りの処理は[[Help:サブページ]]を元に全面的に見直す必要あり
            get;
            set;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 渡されたMediaWikiの内部リンクを解析。
        /// </summary>
        /// <param name="text">[[で始まる文字列。</param>
        /// <param name="link">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public static bool TryParse(string text, out MediaWikiLink link)
        {
            // 出力値初期化
            link = null;

            // 入力値確認
            if (!text.StartsWith(MediaWikiLink.startSign))
            {
                return false;
            }

            // TODO: 未実装
            return true;
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
            b.Append(MediaWikiLink.startSign);

            // 先頭の : の付加
            if (this.IsColon)
            {
                b.Append(':');
            }

            // 言語コード・他プロジェクトコードの付加
            if (!String.IsNullOrEmpty(this.Code))
            {
                b.Append(this.Code);
            }

            // リンクの付加
            if (!String.IsNullOrEmpty(this.Title))
            {
                b.Append(this.Title);
            }

            // セクション名の付加
            if (!String.IsNullOrEmpty(this.Section))
            {
                b.Append('#');
                b.Append(this.Section);
            }

            // パイプ後の文字列の付加
            if (this.PipeTexts != null)
            {
                foreach (string s in this.PipeTexts)
                {
                    b.Append('|');
                    b.Append(s);
                }
            }

            // 閉じタグの付加
            b.Append(MediaWikiLink.endSign);
            return b.ToString();
        }

        #endregion
        
        #region 内部処理用メソッド

        #endregion
    }
}
