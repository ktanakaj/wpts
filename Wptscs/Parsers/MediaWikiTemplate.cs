// ================================================================================================
// <summary>
//      MediaWikiページのテンプレート要素をあらわすモデルクラスソース</summary>
//
// <copyright file="MediaWikiTemplate.cs" company="honeplusのメモ帳">
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
    using Honememo.Utilities;

    /// <summary>
    /// MediaWikiページのテンプレート要素をあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiTemplate : MediaWikiLink
    {
        #region 定数

        /// <summary>
        /// テンプレートの開始タグ。
        /// </summary>
        public static readonly new string DelimiterStart = "{{";

        /// <summary>
        /// テンプレートの閉じタグ。
        /// </summary>
        public static readonly new string DelimiterEnd = "}}";

        /// <summary>
        /// msgnwの書式。
        /// </summary>
        public static readonly string Msgnw = "msgnw:";

        #endregion

        #region private変数

        /// <summary>
        /// テンプレートの記事名。
        /// </summary>
        private string title;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたタイトルのテンプレート要素をあらわすインスタンスを生成する。
        /// </summary>
        /// <param name="title">テンプレート名。</param>
        public MediaWikiTemplate(string title)
        {
            this.Title = title;
            this.PipeTexts = new List<IElement>();
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// テンプレートの記事名。
        /// </summary>
        /// <exception cref="ArgumentNullException">記事名がnullの場合。</exception>
        /// <exception cref="ArgumentException">記事名が空の場合。</exception>
        /// <remarks>テンプレートに記載されていた記事名であり、名前空間の情報などは含まない可能性があるため注意。</remarks>
        public override string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = Validate.NotBlank(value);
            }
        }

        /// <summary>
        /// テンプレートのソースをそのまま出力することを示す msgnw: が付加されているか？
        /// </summary>
        public virtual bool IsMsgnw
        {
            get;
            set;
        }

        /// <summary>
        /// 記事名の後で改行が入るか？
        /// </summary>
        public virtual bool NewLine
        {
            get;
            set;
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// この要素を書式化したテンプレートテキストを返す。
        /// </summary>
        /// <returns>テンプレートテキスト。</returns>
        protected override string ToStringImpl()
        {
            // 戻り値初期化
            StringBuilder b = new StringBuilder();
            
            // 開始タグの付加
            b.Append(MediaWikiTemplate.DelimiterStart);

            // 先頭の : の付加
            if (this.IsColon)
            {
                b.Append(':');
            }

            // msgnw: （テンプレートを<nowiki>タグで挟む）の付加
            if (this.IsMsgnw)
            {
                b.Append(MediaWikiTemplate.Msgnw);
            }

            // 言語コード・他プロジェクトコードの付加
            if (!String.IsNullOrEmpty(this.Code))
            {
                b.Append(this.Code);
                b.Append(':');
            }

            // テンプレート名の付加
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

            // 改行の付加
            if (this.NewLine)
            {
                b.Append('\n');
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
            b.Append(MediaWikiTemplate.DelimiterEnd);
            return b.ToString();
        }

        #endregion
    }
}
