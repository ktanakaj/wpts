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
        
        #region 公開インスタンスメソッド

        #region MediaWikiLink関連

        /// <summary>
        /// 渡されたテキストをMediaWikiの内部リンクとして解析する。
        /// </summary>
        /// <param name="s">[[で始まる文字列。</param>
        /// <param name="result">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public bool TryParseMediaWikiLink(string s, out MediaWikiLink result)
        {
            // 出力値初期化
            result = null;

            // 入力値確認
            if (!s.StartsWith(MediaWikiLink.DelimiterStart))
            {
                return false;
            }

            // 構文を解析して、[[]]内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            string section = null;
            IList<StringBuilder> pipeTexts = new List<StringBuilder>();
            int lastIndex = -1;
            int pipeCounter = 0;
            bool sharpFlag = false;
            for (int i = 2; i < s.Length; i++)
            {
                char c = s[i];

                // ]]が見つかったら、処理正常終了
                if (StringUtils.StartsWith(s, MediaWikiLink.DelimiterEnd, i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は表示名などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(new StringBuilder());
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy;
                string variable;
                int index = this.ChkVariable(out variable, out dummy, s, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1].Append(variable);
                    }
                    else if (sharpFlag)
                    {
                        section += variable;
                    }
                    else
                    {
                        article += variable;
                    }

                    continue;
                }

                // | の前のとき
                if (pipeCounter <= 0)
                {
                    // 変数以外で { } または < > [ ] \n が含まれている場合、リンクは無効
                    if ((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}') || (c == '\n'))
                    {
                        break;
                    }

                    // # の前のとき
                    if (!sharpFlag)
                    {
                        // #が含まれている場合、以降の文字列は見出しへのリンクとして扱う（1つめの#のみ有効）
                        if (c == '#')
                        {
                            sharpFlag = true;
                            section = String.Empty;
                        }
                        else
                        {
                            article += c;
                        }
                    }
                    else
                    {
                        // # の後のとき
                        section += c;
                    }
                }
                else
                {
                    // | の後のとき
                    if (c == '<')
                    {
                        string subtext = s.Substring(i);
                        XmlCommentElement comment;
                        XmlElement nowiki;
                        if (XmlCommentElement.TryParseLazy(subtext, out comment))
                        {
                            // コメント（<!--）が含まれている場合、リンクは無効
                            break;
                        }
                        else if (this.TryParseNowiki(subtext, out nowiki))
                        {
                            // nowikiブロック
                            i += nowiki.ToString().Length - 1;
                            pipeTexts[pipeCounter - 1].Append(nowiki.ToString());
                            continue;
                        }
                    }

                    // リンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰チェック
                    IElement l;
                    index = this.ChkLinkText(out l, s, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1].Append(l.ToString());
                        continue;
                    }

                    pipeTexts[pipeCounter - 1].Append(c);
                }
            }

            // 解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            result = new MediaWikiLink();

            // 変数ブロックの文字列をリンクのテキストに設定
            result.ParsedString = s.Substring(0, lastIndex + 1);

            // 前後のスペースは削除（見出しは後ろのみ）
            result.Title = article.Trim();
            result.Section = section != null ? section.TrimEnd() : section;

            // | 以降は再帰的に解析して設定
            result.PipeTexts = new List<IElement>();
            foreach (StringBuilder b in pipeTexts)
            {
                result.PipeTexts.Add(this.Parse(b.ToString()));
            }

            // 記事名から情報を抽出
            // サブページ
            if (result.Title.StartsWith("/"))
            {
                result.IsSubpage = true;
            }
            else if (result.Title.StartsWith(":"))
            {
                // 先頭が :
                result.IsColon = true;
                result.Title = result.Title.TrimStart(':').TrimStart();
            }

            // 標準名前空間以外で[[xxx:yyy]]のようになっている場合、言語コード
            if (result.Title.Contains(":") && new MediaWikiPage(this.Website, result.Title).IsMain())
            {
                // ※本当は、言語コード等の一覧を作り、其処と一致するものを・・・とすべきだろうが、
                //   メンテしきれないので : を含む名前空間以外を全て言語コード等と判定
                result.Code = result.Title.Substring(0, result.Title.IndexOf(':')).TrimEnd();
                result.Title = result.Title.Substring(result.Title.IndexOf(':') + 1).TrimStart();
            }

            return true;
        }

        /// <summary>
        /// 渡された文字が<c>TryParse</c>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        /// <remarks>性能対策などで処理自体を呼ばせたく無い場合用。</remarks>
        public bool IsMediaWikiLinkPossible(char c)
        {
            return MediaWikiLink.DelimiterStart[0] == c;
        }

        #endregion

        #region MediaWikiTemplate関連

        /// <summary>
        /// 渡されたテキストをMediaWikiのテンプレートとして解析する。
        /// </summary>
        /// <param name="s">{{で始まる文字列。</param>
        /// <param name="result">解析したテンプレート。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public bool TryParseMediaWikiTemplate(string s, out MediaWikiTemplate result)
        {
            // 出力値初期化
            result = null;

            // 入力値確認
            if (!s.StartsWith(MediaWikiTemplate.DelimiterStart))
            {
                return false;
            }

            // 構文を解析して、{{}}内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            IList<StringBuilder> pipeTexts = new List<StringBuilder>();
            int lastIndex = -1;
            int pipeCounter = 0;
            for (int i = 2; i < s.Length; i++)
            {
                char c = s[i];

                // }}が見つかったら、処理正常終了
                if (StringUtils.StartsWith(s, MediaWikiTemplate.DelimiterEnd, i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は引数などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(new StringBuilder());
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy;
                string variable;
                int index = this.ChkVariable(out variable, out dummy, s, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1].Append(variable);
                    }
                    else
                    {
                        article += variable;
                    }

                    continue;
                }

                // | の前のとき
                if (pipeCounter <= 0)
                {
                    // 変数以外で < > [ ] { } が含まれている場合、リンクは無効
                    if ((c == '<') || (c == '>') || (c == '[') || (c == ']') || (c == '{') || (c == '}'))
                    {
                        break;
                    }

                    article += c;
                }
                else
                {
                    // | の後のとき
                    if (c == '<')
                    {
                        string subtext = s.Substring(i);
                        XmlCommentElement comment;
                        XmlElement nowiki;
                        if (XmlCommentElement.TryParseLazy(subtext, out comment))
                        {
                            // コメント（<!--）が含まれている場合、リンクは無効
                            break;
                        }
                        else if (this.TryParseNowiki(subtext, out nowiki))
                        {
                            // nowikiブロック
                            i += nowiki.ToString().Length - 1;
                            pipeTexts[pipeCounter - 1].Append(nowiki.ToString());
                            continue;
                        }
                    }

                    // リンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
                    IElement l;
                    index = this.ChkLinkText(out l, s, i);
                    if (index != -1)
                    {
                        i = index;
                        pipeTexts[pipeCounter - 1].Append(l.ToString());
                        continue;
                    }

                    pipeTexts[pipeCounter - 1].Append(c);
                }
            }

            // 解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            // 前後のスペース・改行は削除（見出しは後ろのみ）
            result = new MediaWikiTemplate(article.Trim());

            // 変数ブロックの文字列をリンクのテキストに設定
            result.ParsedString = s.Substring(0, lastIndex + 1);

            // | 以降は再帰的に解析して設定
            result.PipeTexts = new List<IElement>();
            foreach (StringBuilder b in pipeTexts)
            {
                result.PipeTexts.Add(this.Parse(b.ToString()));
            }

            // 記事名から情報を抽出
            // サブページ
            if (result.Title.StartsWith("/") == true)
            {
                result.IsSubpage = true;
            }
            else if (result.Title.StartsWith(":"))
            {
                // 先頭が :
                result.IsColon = true;
                result.Title = result.Title.TrimStart(':').TrimStart();
            }

            // 先頭が msgnw:
            result.IsMsgnw = result.Title.ToLower().StartsWith(MediaWikiTemplate.Msgnw.ToLower());
            if (result.IsMsgnw)
            {
                result.Title = result.Title.Substring(MediaWikiTemplate.Msgnw.Length);
            }

            // 記事名直後の改行の有無
            if (article.TrimEnd(' ').EndsWith("\n"))
            {
                result.NewLine = true;
            }

            return true;
        }

        /// <summary>
        /// 渡された文字が<c>TryParse</c>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。</returns>
        /// <remarks>性能対策などで処理自体を呼ばせたく無い場合用。</remarks>
        public bool IsMediaWikiTemplatePossible(char c)
        {
            return MediaWikiTemplate.DelimiterStart[0] == c;
        }

        #endregion

        #region Nowiki関連

        /// <summary>
        /// 渡されたテキストがnowikiブロックかを解析する。
        /// </summary>
        /// <param name="s">解析するテキスト。</param>
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
            XmlElement element;
            if (this.TryParseXmlElement(s, out element))
            {
                if (element.Name.ToLower() == MediaWikiParser.nowikiTag)
                {
                    // nowiki区間は内部要素を全てテキストとして扱う
                    XmlTextElement innerElement = new XmlTextElement();
                    StringBuilder b = new StringBuilder();
                    foreach (IElement e in element)
                    {
                        b.Append(e.ToString());
                    }

                    innerElement.Raw = b.ToString();
                    innerElement.ParsedString = b.ToString();
                    element.Clear();
                    element.Add(innerElement);
                    result = element;
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

        #endregion

        /// <summary>
        /// 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック。
        /// </summary>
        /// <param name="element">解析したリンク。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1。</returns>
        public int ChkLinkText(out IElement element, string text, int index)
        {
            // 入力値に応じて、処理を振り分け
            if (this.IsMediaWikiLinkPossible(text[index]))
            {
                // 内部リンク
                MediaWikiLink linkElement;
                if (this.TryParseMediaWikiLink(text.Substring(index), out linkElement))
                {
                    element = linkElement;
                    return index + element.ToString().Length - 1;
                }
            }
            else if (this.IsMediaWikiTemplatePossible(text[index]))
            {
                // テンプレート
                MediaWikiTemplate templateElement;
                if (this.TryParseMediaWikiTemplate(text.Substring(index), out templateElement))
                {
                    element = templateElement;
                    return index + element.ToString().Length - 1;
                }
            }

            // 出力値初期化。リンク以外の場合、nullを返す
            element = null;
            return -1;
        }

        /// <summary>
        /// 渡されたテキストの指定された位置に存在する変数を解析。
        /// </summary>
        /// <param name="variable">解析した変数。</param>
        /// <param name="value">変数のパラメータ値。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>正常時の戻り値には、変数の終了位置のインデックスを返す。異常時は-1。</returns>
        public int ChkVariable(out string variable, out string value, string text, int index)
        {
            // 出力値初期化
            int lastIndex = -1;
            variable = String.Empty;
            value = String.Empty;

            // 入力値確認
            if (!StringUtils.StartsWith(text, "{{{", index))
            {
                return lastIndex;
            }

            // ブロック終了まで取得
            bool pipeFlag = false;
            for (int i = index + 3; i < text.Length; i++)
            {
                // 終了条件のチェック
                if (StringUtils.StartsWith(text, "}}}", i))
                {
                    lastIndex = i + 2;
                    break;
                }

                if (text[i] == '<')
                {
                    XmlCommentElement comment;
                    if (XmlCommentElement.TryParseLazy(text.Substring(i), out comment))
                    {
                        // コメント（<!--）ブロック
                        i += comment.ToString().Length - 1;
                        continue;
                    }
                }

                // | が含まれている場合、以降の文字列は代入された値として扱う
                if (text[i] == '|')
                {
                    pipeFlag = true;
                }
                else if (!pipeFlag)
                {
                    // | の前のとき
                    // ※Wikipediaの仕様上は、{{{1{|表示}}} のように変数名の欄に { を
                    //   含めることができるようだが、判別しきれないので、エラーとする
                    //   （どうせ意図してそんなことする人は居ないだろうし・・・）
                    if (text[i] == '{')
                    {
                        break;
                    }
                }
                else
                {
                    // | の後のとき
                    if (text[i] == '<')
                    {
                        XmlElement nowiki;
                        if (this.TryParseNowiki(text.Substring(i), out nowiki))
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
                    int subindex = this.ChkVariable(out var, out dummy, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += var;
                        continue;
                    }

                    // リンク [[ {{ （{{{1|[[test]]}}}とか）の再帰チェック
                    IElement link;
                    subindex = this.ChkLinkText(out link, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
                        value += link.ToString();
                        continue;
                    }

                    value += text[i];
                }
            }

            // 変数ブロックの文字列を出力値に設定
            if (lastIndex != -1)
            {
                variable = text.Substring(index, lastIndex - index + 1);
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
            return c == '<' || c == '[' || c == '{' || c == '=';
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
            //    （入れ子とかがややこしい話になるので。）
            //    <nowiki>だけ考慮する。
            XmlCommentElement commentElement;
            XmlElement xmlElement;
            MediaWikiLink linkElement;
            MediaWikiTemplate templateElement;
            MediaWikiHeading headingElement;
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
            else if (this.TryParseMediaWikiLink(s, out linkElement))
            {
                result = linkElement;
                return true;
            }
            else if (this.TryParseMediaWikiTemplate(s, out templateElement))
            {
                result = templateElement;
                return true;
            }
            else if (MediaWikiHeading.TryParse(s, this, out headingElement))
            {
                result = headingElement;
                return true;
            }

            result = null;
            return false;
        }

        #endregion
    }
}
