// ================================================================================================
// <summary>
//      曖昧なXML/HTMLタグを解析するためのクラスソース</summary>
//
// <copyright file="LazyXmlParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 曖昧なXML/HTMLタグを解析するためのクラスです。
    /// </summary>
    /// <remarks>
    /// クラス名通り、多少不正な構文であっても解析する。
    /// </remarks>
    public class LazyXmlParser
    {
        #region 定数宣言

        /// <summary>
        /// コメントの開始。
        /// </summary>
        protected static readonly string CommentStart = "<!--";

        /// <summary>
        /// コメントの終了。
        /// </summary>
        protected static readonly string CommentEnd = "-->";

        #endregion

        #region private変数

        /// <summary>
        /// 大文字小文字を無視するか？
        /// </summary>
        private bool ignoreCase = true;

        #endregion

        #region プロパティ

        /// <summary>
        /// 大文字小文字を無視するか？
        /// </summary>
        public bool IgnoreCase
        {
            get
            {
                return this.ignoreCase;
            }

            set
            {
                this.ignoreCase = value;
            }
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

        #region 静的メソッド

        /// <summary>
        /// 渡されたHTMLテキストがコメントかを解析する。
        /// </summary>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="comment">解析したコメント。</param>
        /// <returns>コメントの場合<c>true</c>。</returns>
        /// <remarks>
        /// コメントと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public static bool TryParseComment(string text, out string comment)
        {
            // 入力値確認
            comment = null;
            if (String.IsNullOrEmpty(text) || !text.StartsWith(LazyXmlParser.CommentStart))
            {
                return false;
            }

            // コメント終了まで取得
            int index = text.IndexOf(LazyXmlParser.CommentEnd);
            if (index < 0)
            {
                // 閉じタグが存在しない場合、最後までコメントと判定
                comment = text;
                return true;
            }

            // 閉じタグがあった場合、閉じタグの終わりまでを返す
            comment = text.Substring(0, index + CommentEnd.Length);
            return true;
        }

        #endregion
        
        #region 公開メソッド

        /// <summary>
        /// 渡されたテキストをXML/HTMLタグとして解析する。
        /// </summary>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="element">解析したタグ。</param>
        /// <returns>タグの場合<c>true</c>。</returns>
        /// <remarks>
        /// XML/HTMLタグと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// また、本メソッドはあくまで簡易的な構文用であり、入れ子は考慮しない。
        /// </remarks>
        public bool TryParse(string text, out SimpleElement element)
        {
            // 入力値確認。最低3文字あるか、先頭2文字目までが使用可能な文字かをチェック
            element = null;
            if (StringUtils.DefaultString(text).Length < 3 || text[0] != '<'
                || (!Char.IsLetter(text[1]) && text[1] != '_' && text[1] != ':'))
            {
                return false;
            }

            // タグ名取得、タグ名の次に出現しうる文字を探索
            int index = text.IndexOfAny(new char[] { ' ', '>', '/' }, 1);
            if (index < 0)
            {
                return false;
            }

            string name = text.Substring(1, index - 1);

            // 開始タグの終端に到達するまで属性を探索
            //// TODO: ここから先とりあえず動けばいいや的に書いて凄く汚いので直す
            IDictionary<string, string> attribute = new Dictionary<string, string>();
            StringBuilder attrKey = new StringBuilder();
            StringBuilder attrValue = new StringBuilder();
            bool existedEqual = false;
            bool existedSpace = false;
            char[] separators = null;
            for (; index < text.Length; index++)
            {
                char c = text[index];
                if (c == ' ' && attrKey.Length == 0)
                {
                    // キーに入る前のスペースは無視
                    continue;
                }
                else if (!existedEqual)
                {
                    // イコールの前（キー）
                    if (c == '=')
                    {
                        // イコール後の処理に切り替え
                        existedEqual = true;
                        existedSpace = false;
                        continue;
                    }
                    else if (c == '>' || c == '/')
                    {
                        // ループ終了
                        if (attrKey.Length > 0)
                        {
                            // キーだけで値が無いパターン
                            attribute[this.Decode(attrKey.ToString())] = String.Empty;
                            attrKey.Clear();
                        }

                        break;
                    }
                    else if (c == ' ')
                    {
                        // キーの後イコールの前のスペースは記録して無視
                        existedSpace = true;
                        continue;
                    }

                    if (existedSpace)
                    {
                        // 既にスペースが出現した状態で新たに普通の文字が出現した場合、
                        // キーだけで値が無いパターンだったと判定
                        attribute[this.Decode(attrKey.ToString())] = String.Empty;
                        attrKey.Clear();
                        existedSpace = false;
                    }

                    // キーに1文字追加
                    attrKey.Append(c);
                }
                else if (c == ' ' && separators == null)
                {
                    // イコールの後の、値に入る前のスペースは無視
                    continue;
                }
                else
                {
                    // イコールの後（値）
                    if (separators == null)
                    {
                        // イコール後の最初の文字の場合、区切り文字かを確認
                        if (c == '\'' || c == '"')
                        {
                            separators = new char[] { c };
                            continue;
                        }

                        // いきなり値が始まっている場合は、次の文字までを値と判定
                        separators = new char[] { ' ', '>', '/' };
                    }

                    if (separators.Contains(c))
                    {
                        // 区切り文字に到達
                        attribute[this.Decode(attrKey.ToString())] = this.Decode(attrValue.ToString());
                        attrKey.Clear();
                        attrValue.Clear();
                        separators = null;
                        if (c == '>' || c == '/')
                        {
                            // ループ終了
                            break;
                        }

                        existedEqual = false;
                        continue;
                    }

                    // 値
                    attrValue.Append(c);
                }
            }

            // 最後までループしてしまった場合、閉じていないのでNG
            if (index == text.Length)
            {
                return false;
            }

            // '/'で終わった場合は、ちゃんと閉じることを確認して判定終了
            if (text[index] == '/')
            {
                if (index + 1 == text.Length)
                {
                    return false;
                }

                element = new SimpleElement(name, attribute, String.Empty, text.Substring(0, index + 2));
                return true;
            }

            // 閉じタグまでを取得
            // 終わりが見つからない場合は、全てタグブロックと判断
            string outerXml = text;
            string innerXml = text.Substring(index + 1);
            //// TODO: これだと後ろにスペースがあった場合検知されないので要習性
            string endTag = "</" + name + ">";

            // 大文字小文字を区別しない場合、全て小文字で比較
            string diff = text;
            if (this.IgnoreCase)
            {
                diff = text.ToLower();
            }

            bool ended = false;
            for (int i = index + 1; i < diff.Length; i++)
            {
                // 終了条件のチェック
                if (StringUtils.StartsWith(diff, endTag, i))
                {
                    outerXml = text.Substring(0, i + endTag.Length);
                    innerXml = text.Substring(index + 1, i - index - 1);
                    ended = true;
                    break;
                }

                // コメント（<!--）のチェック
                string comment;
                if (LazyXmlParser.TryParseComment(text.Substring(i), out comment))
                {
                    i += comment.Length - 1;
                    continue;
                }
            }

            // HTMLの場合、閉じタグが無いのは開始タグだけのパターンと判定
            if (!ended && this.IsHtml)
            {
                element = new SimpleElement(name, attribute, String.Empty, text.Substring(0, index + 1));
            }
            else
            {
                element = new SimpleElement(name, attribute, innerXml, outerXml);
            }

            return true;
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// 文字列をXML/HTML読み込み用にデコードする。
        /// </summary>
        /// <param name="str">デコードする文字列。</param>
        /// <returns>デコードされた文字列。<c>null</c>の場合、空文字列を返す。</returns>
        protected string Decode(string str)
        {
            if (str == null)
            {
                return String.Empty;
            }
            else if (this.IsHtml)
            {
                return System.Web.HttpUtility.HtmlDecode(str);
            }
            else
            {
                return str.Replace("&lt;", "<").Replace("&gt;", ">")
                    .Replace("&quot;", "\"").Replace("&apos;", "\'").Replace("&amp;", "&");
            }
        }

        #endregion

        #region 内部クラス

        /// <summary>
        /// XML/HTMLタグをあらわすモデルクラスです。
        /// </summary>
        /// <remarks>
        /// パラメータ等は<c>XmlElement</c>に似せている。
        /// 本当はそちらを返したいが設定すべき項目が多く簡単に使用できないため。
        /// </remarks>
        public class SimpleElement
        {
            #region コンストラクタ

            /// <summary>
            /// 指定された情報を持つタグを作成する。
            /// </summary>
            /// <param name="name">タグ名。</param>
            /// <param name="attributes">属性情報。</param>
            /// <param name="innerXml">このタグに含まれる値。</param>
            /// <param name="outerXml">このタグの開始／終了タグを含む文字列。</param>
            public SimpleElement(string name, IDictionary<string, string> attributes, string innerXml, string outerXml)
            {
                this.Name = name;
                this.Attributes = attributes;
                this.InnerXml = innerXml;
                this.OuterXml = outerXml;
            }

            #endregion

            #region プロパティ

            /// <summary>
            /// タグ名。
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// このタグに含まれる文字列。
            /// </summary>
            public string InnerXml
            {
                get;
                private set;
            }

            /// <summary>
            /// このタグの開始／終了タグを含む文字列。
            /// </summary>
            public string OuterXml
            {
                get;
                private set;
            }

            /// <summary>
            /// タグに含まれる属性情報。
            /// </summary>
            public IDictionary<string, string> Attributes
            {
                get;
                private set;
            }

            #endregion

            #region メソッド

            /// <summary>
            /// タグに指定した名前の属性があるかどうかを確認する。
            /// </summary>
            /// <param name="name">検索する属性の名前。</param>
            /// <returns>現在のノードに指定した属性がある場合は <c>true</c>。それ以外の場合は <c>false</c>。</returns>
            public bool HasAttribute(string name)
            {
                return this.Attributes.ContainsKey(name);
            }

            /// <summary>
            /// 指定した名前の属性の値を返す。
            /// </summary>
            /// <param name="name">取得する属性の名前。</param>
            /// <returns>指定した属性の値。一致する属性が見つからない場合、属性に指定した値がない場合は、空の文字列を返す。</returns>
            public string GetAttribute(string name)
            {
                string value;
                if (this.Attributes.TryGetValue(name, out value))
                {
                    return StringUtils.DefaultString(value);
                }

                return String.Empty;
            }

            /// <summary>
            /// <c>OuterXml</c> を返す。
            /// </summary>
            /// <returns><c>OuterXml</c>。</returns>
            public override string ToString()
            {
                return this.OuterXml;
            }

            #endregion
        }

        #endregion
    }
}
