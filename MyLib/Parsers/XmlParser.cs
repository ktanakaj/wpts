// ================================================================================================
// <summary>
//      XML/HTMLテキストを解析するためのクラスソース</summary>
//
// <copyright file="XmlParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Honememo.Utilities;

    /// <summary>
    /// XML/HTMLテキストを解析するためのクラスです。
    /// </summary>
    public class XmlParser : AbstractParser
    {
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
        
        #region 公開メソッド

        /// <summary>
        /// 渡されたテキストをXML/HTMLタグとして解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析したタグ。</param>
        /// <returns>タグの場合<c>true</c>。</returns>
        /// <remarks>
        /// XML/HTMLタグと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// また、本メソッドはあくまで簡易的な構文用であり、入れ子は考慮しない。
        /// </remarks>
        public virtual bool TryParseXmlElement(string s, out XmlElement result)
        {
            // 入力値確認。タグでない場合は即終了
            result = null;
            if (String.IsNullOrEmpty(s) || s[0] != '<')
            {
                return false;
            }

            // タグ名取得、タグ名の次に出現しうる文字を探索
            int index = s.IndexOfAny(new char[] { ' ', '>', '/' }, 1);
            if (index < 0)
            {
                return false;
            }

            // タグ名確認、コメント（<!--）やちゃんと始まっていないもの（< tag>とか）もここで除外
            string name = s.Substring(1, index - 1);
            if (!this.ValidateName(name))
            {
                return false;
            }

            // 開始タグの終端に到達するまで属性を探索
            IDictionary<string, string> attribute;
            int endIndex;
            if (!this.TryParseAttribute(s.Substring(index), out attribute, out endIndex))
            {
                return false;
            }

            index += endIndex;

            // "/>" で終わった場合は、ここでタグ終了
            if (s[index - 1] == '/')
            {
                result = new XmlElement(name, attribute, null, s.Substring(0, index + 1));
                return true;
            }

            // 閉じタグまでを解析
            string innerXml;
            if (!this.TryParseContent(s.Substring(index + 1), name, out innerXml, out endIndex))
            {
                return false;
            }

            if (endIndex < 0)
            {
                // 閉じタグが無い場合
                if (this.IsHtml)
                {
                    // HTMLの場合、閉じタグが無いのは開始タグだけのパターンと判定
                    // ※ indexは開始タグの末尾になっているのでそのまま
                    innerXml = String.Empty;
                }
                else
                {
                    // それ以外は不正な構文だが値の最終文字までを設定
                    index += innerXml.Length;
                }
            }
            else
            {
                index += endIndex;
            }

            // 内部要素を再帰的に探索
            IElement innerElement;
            if (!this.TryParse(innerXml, out innerElement))
            {
                return false;
            }

            // 結果がリストの場合そのまま、それ以外はリストに入れて親のXMLElementに代入
            ICollection<IElement> collection;
            if (innerElement.GetType() == typeof(ListElement))
            {
                collection = (ListElement)innerElement;
            }
            else
            {
                collection = new List<IElement>();
                collection.Add(innerElement);
            }

            result = new XmlElement(name, attribute, collection, s.Substring(0, index + 1));
            return true;
        }

        #endregion

        #region 実装支援用抽象メソッド実装

        /// <summary>
        /// 渡された位置の文字列が<see cref="TryParseElements"/>の候補となるかを判定する。
        /// </summary>
        /// <param name="s">全体文字列。</param>
        /// <param name="index">処理インデックス。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        protected override bool IsElementPossible(string s, int index)
        {
            return s[index] == '<';
        }

        /// <summary>
        /// 渡されたテキストを各種解析処理で解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析した結果要素。</param>
        /// <returns>解析できた場合<c>true</c>。</returns>
        protected override bool TryParseElements(string s, out IElement result)
        {
            CommentElement commentElement;
            XmlElement xmlElement;
            if (CommentElement.TryParseLazy(s, out commentElement))
            {
                result = commentElement;
                return true;
            }
            else if (XmlElement.TryParse(s, this, out xmlElement))
            {
                result = xmlElement;
                return true;
            }

            result = null;
            return false;
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// 文字列をXML/HTML読み込み用にデコードする。
        /// </summary>
        /// <param name="str">デコードする文字列。</param>
        /// <returns>デコードされた文字列。<c>null</c>の場合、空文字列を返す。</returns>
        private string Decode(string str)
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

        /// <summary>
        /// 渡されたタグ名／属性名がXML的に正しいかを判定。
        /// </summary>
        /// <param name="name">XMLタグ名／属性名。</param>
        /// <returns>正しい場合 <c>true</c>。</returns>
        /// <remarks>
        /// デコード前の値を渡すこと。
        /// HTMLについては、HTML用のチェックではないが、基本的に呼んで問題ないはず。
        /// </remarks>
        private bool ValidateName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                // 空は無条件でNG
                return false;
            }

            if (!this.IsNameFirstCharacter(name[0]))
            {
                // 先頭1文字が許可されていない文字の場合NG
                return false;
            }

            // 2文字目以降の調査
            for (int i = 1; i < name.Length; i++)
            {
                if (!this.IsNameCharacter(name[i]))
                {
                    // 2文字目以降として使えない文字が出現したらNG
                    return false;
                }
            }

            // 全てOK
            return true;
        }

        /// <summary>
        /// XMLタグ名／属性名の先頭文字として使用可能な文字かを判定する。
        /// </summary>
        /// <param name="c">文字。</param>
        /// <returns>使用可能な場合 <c>true</c>。</returns>
        private bool IsNameFirstCharacter(char c)
        {
            // 先頭一文字目はLetterクラスの文字と一部記号のみ許可
            return Char.IsLetter(c) || c == '_' || c == ':';
        }

        /// <summary>
        /// XMLタグ名／属性名の2文字目以降として使用可能な文字かを判定する。
        /// </summary>
        /// <param name="c">文字。</param>
        /// <returns>使用可能な場合 <c>true</c>。</returns>
        private bool IsNameCharacter(char c)
        {
            // 2文字目以降は、一文字目の文字に加えて
            // Digitクラス、Combining Characterクラス、Extenderクラスの文字とハイフン・ピリオドが可能
            // ※ 厳密な判定が大変で、Lazyクラスでそこまでやる必要も感じないので、
            //    処理的に困る制御文字や記号類を除きみんな許可する。
            return this.IsNameFirstCharacter(c) || c == '-' || c == '.'
                || (!Char.IsControl(c) && !Char.IsWhiteSpace(c) && !Char.IsSymbol(c) && !Char.IsPunctuation(c));
        }

        /// <summary>
        /// タグの属性情報部分のテキストを受け取り、属性情報を解析する。
        /// </summary>
        /// <param name="text">解析する属性情報文字列（" key1="value1" key2="value2"&gt;～" のような文字列）。</param>
        /// <param name="attribute">解析した属性。</param>
        /// <param name="endIndex">タグ終了箇所（'&gt;'）のインデックス。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        private bool TryParseAttribute(string text, out IDictionary<string, string> attribute, out int endIndex)
        {
            // ※ 不正な構文も通すために、強引な汚いソースになってしまっている。
            //    このメソッドにその辺りの処理を隔離している。

            // 出力値初期化
            attribute = null;
            endIndex = -1;

            // 開始タグの終端に到達するまで属性を探索
            IDictionary<string, string> a = new Dictionary<string, string>();
            string key = null;
            bool existedEqual = false;
            bool existedSpace = false;
            char[] separators = null;
            int i;
            for (i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (key == null)
                {
                    // 属性名の解析
                    if (c == ' ')
                    {
                        // 属性名の前にあるスペースは無視
                        continue;
                    }

                    // 属性名の次に出現しうる文字を探索
                    int index = text.IndexOfAny(new char[] { '=', ' ', '>', '/' }, i);
                    if (index < 0)
                    {
                        // どれも出現しない場合、構文エラー
                        return false;
                    }

                    // 属性名を確認
                    // ※ 属性が無い場合0文字となる
                    key = text.Substring(i, index - i);
                    if (!String.IsNullOrEmpty(key) && !this.ValidateName(key))
                    {
                        // 属性名の位置に出現し得ない記号が含まれているなど構文エラーも弾く
                        return false;
                    }

                    // ループを次回「次に出現しうる文字」になるよう更新
                    i = index - 1;
                }
                else if (!existedEqual)
                {
                    // 属性名は解析済みでも値が始まっていない場合
                    if (c == '=')
                    {
                        // イコール後の処理に切り替え
                        existedEqual = true;
                        existedSpace = false;
                    }
                    else if (c == '>' || c == '/')
                    {
                        // ループ終了
                        if (!String.IsNullOrEmpty(key))
                        {
                            // 属性名だけで値が無いパターン（<div disable>とか）
                            a[this.Decode(key)] = String.Empty;
                            key = null;
                        }

                        break;
                    }
                    else if (c == ' ')
                    {
                        // 属性名の後にあるスペースは記録して無視
                        existedSpace = true;
                    }
                    else if (existedSpace)
                    {
                        // 既にスペースが出現した状態で新たに普通の文字が出現した場合、
                        // キーだけで値が無いパターンだったと判定
                        a[this.Decode(key)] = String.Empty;
                        key = null;
                        existedSpace = false;
                    }
                }
                else
                {
                    // 属性値の解析
                    if (separators == null)
                    {
                        if (c == ' ')
                        {
                            // イコールの後の、値に入る前のスペースは無視
                            continue;
                        }

                        // イコール後の最初の文字の場合、区切り文字かを確認
                        if (c == '\'' || c == '"')
                        {
                            separators = new char[] { c };
                            continue;
                        }

                        // いきなり値が始まっている場合は、次の文字までを値と判定
                        separators = new char[] { ' ', '>', '/' };
                    }

                    // 属性値の終了文字を探索
                    int index = text.IndexOfAny(separators, i);
                    if (index < 0)
                    {
                        // どれも出現しない場合、閉じてないということで構文エラー
                        return false;
                    }

                    // 区切り文字に到達
                    a[this.Decode(key)] = this.Decode(text.Substring(i, index - i));
                    key = null;
                    existedEqual = false;
                    separators = null;

                    if (text[index] == '>' || text[index] == '/')
                    {
                        // 区切り文字がループ終了の文字だったらここで終了
                        break;
                    }

                    // ループを次回「区切り文字の次の文字」になるよう更新
                    i = index;
                }
            }

            // '/' で抜けた場合は、閉じ括弧があるはず位置にインデックスを移動
            if (text.ElementAtOrDefault(i) == '/')
            {
                ++i;
            }

            // 最後が閉じ括弧で無い場合、閉じていないのでNG
            // ※ ループが閉じ括弧で終わらなかった場合もここに引っかかる
            if (text.ElementAtOrDefault(i) != '>')
            {
                return false;
            }

            // 出力値の設定
            attribute = a;
            endIndex = i;

            return true;
        }

        /// <summary>
        /// タグの開始タグ以降の部分のテキストを受け取り、値と閉じタグを解析する。
        /// </summary>
        /// <param name="text">解析する部分文字列（"～&lt;/tag&gt;" のような文字列）。</param>
        /// <param name="tag">解析するタグ名。</param>
        /// <param name="innerXml">解析したコンテンツ部分。</param>
        /// <param name="endIndex">閉じタグ終了箇所（'&gt;'）のインデックス。閉じていない場合は-1。</param>
        /// <returns>
        /// 解析に成功した場合<c>true</c>。
        /// ※現状では常に<c>true</c>。単に他とパラメータをあわせただけ。
        /// </returns>
        private bool TryParseContent(string text, string tag, out string innerXml, out int endIndex)
        {
            // 閉じタグまでを取得。終わりが見つからない場合は、全てタグブロックと判断
            innerXml = text;
            endIndex = -1;

            // 検索条件作成
            RegexOptions options = RegexOptions.Singleline;
            if (this.IgnoreCase)
            {
                // 大文字小文字を区別しない
                options = options | RegexOptions.IgnoreCase;
            }

            Regex endRegex = new Regex("^</" + Regex.Escape(tag) + "\\s*>", options);

            for (int i = 0; i < text.Length; i++)
            {
                // ※ 本当は全て正規表現一発で処理したいが、コメントが含まれている可能性があるのでループ
                if (text[i] != '<')
                {
                    continue;
                }

                // 終了条件のチェック
                string s = text.Substring(i);
                Match match = endRegex.Match(s);
                if (match.Success)
                {
                    innerXml = text.Substring(0, i);
                    endIndex = i + match.Length;
                    break;
                }

                // コメント（<!--）のチェック
                CommentElement comment;
                if (CommentElement.TryParseLazy(s, out comment))
                {
                    i += comment.ToString().Length - 1;
                    continue;
                }
            }

            return true;
        }

        #endregion
    }
}
