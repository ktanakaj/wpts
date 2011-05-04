// ================================================================================================
// <summary>
//      MediaWikiのページを解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Honememo.Parsers;
    using Honememo.Utilities;
    using Honememo.Wptscs.Properties;
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiのページを解析するパーサークラスです。
    /// </summary>
    public class MediaWikiParser : XmlParser
    {
        #region 定数宣言

        /// <summary>
        /// nowikiタグ。
        /// </summary>
        private static readonly string nowikiTag = "nowiki";

        #endregion

        #region private変数

        /// <summary>
        /// このパーサーが対応するMediaWiki。
        /// </summary>
        private MediaWiki website;

        #endregion
        
        #region コンストラクタ

        /// <summary>
        /// 指定されたMediaWikiサーバーのページを解析するためのパーサーを作成する。
        /// </summary>
        /// <param name="site">このパーサーが対応するMediaWiki</param>
        public MediaWikiParser(MediaWiki site)
        {
            this.Website = site;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// このパーサーが対応するMediaWiki。
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public MediaWiki Website
        {
            get
            {
                return this.website;
            }

            set
            {
                this.website = Validate.NotNull(value);
            }
        }

        #endregion
        
        #region 公開メソッド
        
        /// <summary>
        /// 渡されたテキストがnowikiブロックかを解析する。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析したnowikiブロック。</param>
        /// <returns>nowikiブロックの場合<c>true</c>。</returns>
        /// <remarks>
        /// nowikiブロックと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// また、入れ子は考慮しない。
        /// </remarks>
        public bool TryParseNowiki(string s, out XmlElement result)
        {
            result = null;
            IElement element;
            if (new XmlElementParser(this).TryParse(s, out element))
            {
                XmlElement xmlElement = (XmlElement)element;
                if (xmlElement.Name.ToLower() == MediaWikiParser.nowikiTag)
                {
                    // nowiki区間は内部要素を全てテキストとして扱う
                    XmlTextElement innerElement = new XmlTextElement();
                    StringBuilder b = new StringBuilder();
                    foreach (IElement e in xmlElement)
                    {
                        b.Append(e.ToString());
                    }

                    innerElement.Raw = b.ToString();
                    innerElement.ParsedString = b.ToString();
                    xmlElement.Clear();
                    xmlElement.Add(innerElement);
                    result = xmlElement;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 渡された文字が<see cref="TryParseNowiki"/>の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        /// <remarks>性能対策などで処理自体を呼ばせたく無い場合用。</remarks>
        public bool IsNowikiPossible(char c)
        {
            return '<' == c;
        }

        /// <summary>
        /// 指定されたページをリダイレクトとして解析する。
        /// </summary>
        /// <param name="s">解析対象のページ。</param>
        /// <param name="result">解析したリダイレクト要素。</param>
        /// <returns>リダイレクトの場合<c>true</c>。</returns>
        /// <remarks>MediaWikiのページ全体を渡す必要がある。</remarks>
        public bool TryParseRedirect(string s, out MediaWikiLink result)
        {
            // 日本語版みたいに、#REDIRECTと言語固有の#転送みたいなのがあると思われるので、
            // 翻訳元言語とデフォルトの設定でチェック
            result = null;
            for (int i = 0; i < 2; i++)
            {
                string format = this.Website.Redirect;
                if (i == 1)
                {
                    format = Settings.Default.MediaWikiRedirect;
                }

                if (!String.IsNullOrEmpty(format)
                    && s.ToLower().StartsWith(format.ToLower()))
                {
                    IElement link;
                    if (new MediaWikiLinkParser(this)
                        .TryParse(s.Substring(format.Length).TrimStart(), out link))
                    {
                        result = link as MediaWikiLink;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <param name="result">解析したリンク。</param>
        /// <returns>正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1。</returns>
        public int TryParseLinkOrTemplate(string s, int index, out IElement result)
        {
            // 出力値初期化
            result = null;

            IParser linkParser = new MediaWikiLinkParser(this);
            IParser templateParser = new MediaWikiTemplateParser(this);
            if (!linkParser.IsPossibleParse(s[index])
                && !templateParser.IsPossibleParse(s[index]))
            {
                // どちらにも該当しそうにない場合速やかに終了
                return -1;
            }

            string sub = s.Substring(index);
            if (linkParser.TryParse(sub, out result))
            {
                // 内部リンク
                return index + result.ToString().Length - 1;
            }
            else if (templateParser.TryParse(sub, out result))
            {
                // テンプレート
                return index + result.ToString().Length - 1;
            }

            return -1;
        }

        /// <summary>
        /// 渡されたテキストの指定された位置に存在する変数を解析。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <param name="variable">解析した変数。</param>
        /// <param name="value">変数のパラメータ値。</param>
        /// <returns>正常時の戻り値には、変数の終了位置のインデックスを返す。異常時は-1。</returns>
        public int TryParseVariable(string s, int index, out string variable, out string value)
        {
            // 出力値初期化
            int lastIndex = -1;
            variable = String.Empty;
            value = String.Empty;

            // 入力値確認
            if (!StringUtils.StartsWith(s, "{{{", index))
            {
                return lastIndex;
            }

            // ブロック終了まで取得
            bool pipeFlag = false;
            for (int i = index + 3; i < s.Length; i++)
            {
                // 終了条件のチェック
                if (StringUtils.StartsWith(s, "}}}", i))
                {
                    lastIndex = i + 2;
                    break;
                }

                if (XmlCommentElement.IsPossibleParse(s[i]))
                {
                    XmlCommentElement comment;
                    if (XmlCommentElement.TryParseLazy(s.Substring(i), out comment))
                    {
                        // コメント（<!--）ブロック
                        i += comment.ToString().Length - 1;
                        continue;
                    }
                }

                // | が含まれている場合、以降の文字列は代入された値として扱う
                if (s[i] == '|')
                {
                    pipeFlag = true;
                }
                else if (!pipeFlag)
                {
                    // | の前のとき
                    // ※Wikipediaの仕様上は、{{{1{|表示}}} のように変数名の欄に { を
                    //   含めることができるようだが、判別しきれないので、エラーとする
                    //   （どうせ意図してそんなことする人は居ないだろうし・・・）
                    if (s[i] == '{')
                    {
                        break;
                    }
                }
                else
                {
                    // | の後のとき
                    if (this.IsNowikiPossible(s[i]))
                    {
                        XmlElement nowiki;
                        if (this.TryParseNowiki(s.Substring(i), out nowiki))
                        {
                            // nowikiブロック
                            i += nowiki.ToString().Length - 1;
                            value += nowiki;
                            continue;
                        }
                    }

                    // 変数（{{{1|{{{2}}}}}}とか）の再帰チェック
                    string var;
                    string dummy;
                    int subindex = this.TryParseVariable(s, i, out var, out dummy);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += var;
                        continue;
                    }

                    // リンク [[ {{ （{{{1|[[test]]}}}とか）の再帰チェック
                    IElement link;
                    subindex = this.TryParseLinkOrTemplate(s, i, out link);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += link.ToString();
                        continue;
                    }

                    value += s[i];
                }
            }

            // 変数ブロックの文字列を出力値に設定
            if (lastIndex != -1)
            {
                variable = s.Substring(index, lastIndex - index + 1);
            }
            else
            {
                // 正常な構文ではなかった場合、出力値をクリア
                variable = String.Empty;
                value = String.Empty;
            }

            return lastIndex;
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
            char c = s[index];
            return XmlCommentElement.IsPossibleParse(c)
                || new MediaWikiLinkParser(this).IsPossibleParse(c)
                || new MediaWikiTemplateParser(this).IsPossibleParse(c);
        }

        /// <summary>
        /// 渡されたテキストを各種解析処理で解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
        /// <param name="result">解析した結果要素。</param>
        /// <returns>解析できた場合<c>true</c>。</returns>
        protected override bool TryParseElements(string s, out IElement result)
        {
            // ※ このアプリではMediaWikiのXMLタグは基本的に無視するため、
            //    あえて解析の対象とはしない。
            //    （出力時に書式を再現するのが難しいので。）
            //    <nowiki>だけ考慮する。
            XmlCommentElement commentElement;
            XmlElement xmlElement;
            if (XmlCommentElement.TryParseLazy(s, out commentElement))
            {
                result = commentElement;
                return true;
            }
            else if (this.TryParseNowiki(s, out xmlElement))
            {
                result = xmlElement;
                return true;
            }
            else if (new MediaWikiLinkParser(this).TryParse(s, out result))
            {
                return true;
            }
            else if (new MediaWikiTemplateParser(this).TryParse(s, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        #endregion
    }
}
