// ================================================================================================
// <summary>
//      XML/HTMLテキストを解析するためのクラスソース</summary>
//
// <copyright file="XmlParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using Honememo.Utilities;

    /// <summary>
    /// XML/HTMLテキストを解析するためのクラスです。
    /// </summary>
    /// <remarks>HTMLについては、解析はできるもののほぼXml用のElementで結果が返されます。</remarks>
    public class XmlParser : AbstractTextParser, IDisposable
    {
        #region private変数

        /// <summary>
        /// パーサー内で使用する各要素のパーサー。
        /// </summary>
        private IParser[] parsers;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// XML/HTMLテキストを解析するためのパーサーを作成する。
        /// </summary>
        public XmlParser()
        {
            this.IgnoreCase = true;
            this.parsers = new IParser[]
            {
                new XmlCommentElementParser(),
                new XmlElementParser(this)
            };
        }

        #endregion
        
        #region デストラクタ

        /// <summary>
        /// オブジェクトのリソースを破棄する。
        /// </summary>
        /// <remarks><see cref="Dispose"/>の呼び出しのみ。</remarks>
        ~XmlParser()
        {
            this.Dispose();
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// パーサー内で使用する各要素のパーサー。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public IParser[] Parsers
        {
            get
            {
                return this.parsers;
            }

            set
            {
                this.parsers = Validate.NotNull(value);
            }
        }

        /// <summary>
        /// 大文字小文字を無視するか？
        /// </summary>
        public bool IgnoreCase
        {
            get;
            set;
        }

        /// <summary>
        /// タグはHTMLの書式か？
        /// </summary>
        public bool IsHtml
        {
            get;
            set;
        }

        #endregion

        #region IDisposableインタフェース実装メソッド

        /// <summary>
        /// このパーサーで使用する子パーサーを解放する。
        /// </summary>
        public virtual void Dispose()
        {
            // 子パーサーを解放
            // ※ 循環参照のため、明示的に解放しないとGCされない可能性がある
            if (this.parsers != null)
            {
                this.parsers = null;
            }

            // ファイナライザ（このクラスではDisposeを呼ぶだけ）が不要であることを通知
            GC.SuppressFinalize(this);
        }

        #endregion

        #region XmlParser, XmlElementPaser共通メソッド

        /// <summary>
        /// 文字列をXML/HTML読み込み用にデコードする。
        /// </summary>
        /// <param name="s">デコードする文字列。</param>
        /// <returns>デコードされた文字列。<c>null</c>の場合、空文字列を返す。</returns>
        internal string Decode(string s)
        {
            if (s == null)
            {
                return String.Empty;
            }
            else if (this.IsHtml)
            {
                return WebUtility.HtmlDecode(s);
            }
            else
            {
                return XmlUtils.XmlDecode(s);
            }
        }

        #endregion

        #region 実装支援用メソッド拡張

        /// <summary>
        /// 渡されたテキストを各種解析処理で解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="index">処理インデックス。</param>
        /// <param name="result">解析した結果要素。</param>
        /// <returns>解析できた場合<c>true</c>。</returns>
        protected override bool TryParseElementAt(string s, int index, out IElement result)
        {
            return this.TryParseAt(s, index, out result, this.parsers);
        }

        /// <summary>
        /// 文字列が空でない場合、リストにTextエレメントを追加して、文字列をリセットする。
        /// </summary>
        /// <param name="list">追加されるリスト。</param>
        /// <param name="b">追加する文字列。</param>
        protected override void FlashText(ref ListElement list, ref StringBuilder b)
        {
            if (b.Length > 0)
            {
                string s = b.ToString();
                XmlTextElement e = new XmlTextElement(this.Decode(s));
                e.ParsedString = s;
                list.Add(e);
                b.Clear();
            }
        }

        #endregion
    }
}
