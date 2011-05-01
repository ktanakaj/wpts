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
    /// MediaWikiのページをあらわすモデルクラスです。
    /// </summary>
    public class MediaWikiParser : IParser
    {
        #region 公開静的メソッド

        /// <summary>
        /// 渡されたテキストがnowikiブロックかを解析する。
        /// </summary>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="nowiki">解析したnowikiブロック。</param>
        /// <returns>nowikiブロックの場合<c>true</c>。</returns>
        /// <remarks>
        /// nowikiブロックと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// また、入れ子は考慮しない。
        /// </remarks>
        public static bool TryParseNowiki(string text, out string nowiki)
        {
            nowiki = null;
            LazyXmlParser parser = new LazyXmlParser();
            XmlElement element;
            if (parser.TryParseTag(text, out element))
            {
                if (element.Name.ToLower() == MediaWikiPage.NowikiTag)
                {
                    nowiki = element.ToString();
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 公開インスタンスメソッド

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析結果。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public bool TryParse(string s, out IElement result)
        {
            result = null;
            return false;
        }

        /// <summary>
        /// 渡された文字列の解析を行う。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <returns>解析結果。</returns>
        /// <exception cref="FormatException">文字列が解析できないフォーマットの場合。</exception>
        /// <remarks>解析に失敗した場合は、各種例外を投げる。</remarks>
        public IElement Parse(string s)
        {
            IElement result;
            if (this.TryParse(s, out result))
            {
                return result;
            }

            throw new FormatException("Invalid String : " + s);
        }

        #region Linkクラスに移動したいメソッド

        // TODO: 以下の各メソッドのうち、リンクに関するものはLinkクラスに移したい。
        //       また、余計な依存関係を持っているものを整理したい。

        /// <summary>
        /// 渡されたWikipediaの内部リンクを解析。
        /// </summary>
        /// <param name="text">[[で始まる文字列。</param>
        /// <param name="link">解析したリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public bool TryParseLink(string text, out MediaWikiLink link)
        {
            // 出力値初期化
            link = null;

            // 入力値確認
            if (!text.StartsWith("[["))
            {
                return false;
            }

            // 構文を解析して、[[]]内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            string section = String.Empty;
            IList<string> pipeTexts = new List<string>();
            int lastIndex = -1;
            int pipeCounter = 0;
            bool sharpFlag = false;
            for (int i = 2; i < text.Length; i++)
            {
                char c = text[i];

                // ]]が見つかったら、処理正常終了
                if (StringUtils.StartsWith(text, "]]", i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は表示名などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(String.Empty);
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy;
                string variable;
                int index = this.ChkVariable(out variable, out dummy, text, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1] += variable;
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
                        string subtext = text.Substring(i);
                        CommentElement comment;
                        string value;
                        if (CommentElement.TryParseLazy(subtext, out comment))
                        {
                            // コメント（<!--）が含まれている場合、リンクは無効
                            break;
                        }
                        else if (MediaWikiPage.TryParseNowiki(subtext, out value))
                        {
                            // nowikiブロック
                            i += value.Length - 1;
                            pipeTexts[pipeCounter - 1] += value;
                            continue;
                        }
                    }

                    // リンク [[ {{ （[[image:xx|[[test]]の画像]]とか）の再帰チェック
                    MediaWikiLink l;
                    index = this.ChkLinkText(out l, text, i);
                    if (index != -1)
                    {
                        i = index;
 //                       pipeTexts[pipeCounter - 1] += l.OriginalText;
                        continue;
                    }

                    pipeTexts[pipeCounter - 1] += c;
                }
            }

            // 解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            link = new MediaWikiLink();

            // 変数ブロックの文字列をリンクのテキストに設定
 //           link.OriginalText = text.Substring(0, lastIndex + 1);

            // 前後のスペースは削除（見出しは後ろのみ）
            link.Title = article.Trim();
            link.Section = section.TrimEnd();

            // | 以降はそのまま設定
            link.PipeTexts = pipeTexts;

            // 記事名から情報を抽出
            // サブページ
            if (link.Title.StartsWith("/"))
            {
                link.IsSubpage = true;
            }
            else if (link.Title.StartsWith(":"))
            {
                // 先頭が :
                link.IsColon = true;
                link.Title = link.Title.TrimStart(':').TrimStart();
            }

            // 標準名前空間以外で[[xxx:yyy]]のようになっている場合、言語コード
            //if (link.Title.Contains(":") && new MediaWikiPage(this.Website, link.Title).IsMain())
            //{
            //    // ※本当は、言語コード等の一覧を作り、其処と一致するものを・・・とすべきだろうが、
            //    //   メンテしきれないので : を含む名前空間以外を全て言語コード等と判定
            //    link.Code = link.Title.Substring(0, link.Title.IndexOf(':')).TrimEnd();
            //    link.Title = link.Title.Substring(link.Title.IndexOf(':') + 1).TrimStart();
            //}

            return true;
        }

        /// <summary>
        /// 渡されたWikipediaのテンプレートを解析。
        /// </summary>
        /// <param name="text">{{で始まる文字列。</param>
        /// <param name="link">解析したテンプレートのリンク。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public bool TryParseTemplate(string text, out MediaWikiLink link)
        {
            // 出力値初期化
            link = null;

            // 入力値確認
            if (!text.StartsWith("{{"))
            {
                return false;
            }

            // 構文を解析して、{{}}内部の文字列を取得
            // ※構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            string article = String.Empty;
            IList<string> pipeTexts = new List<string>();
            int lastIndex = -1;
            int pipeCounter = 0;
            for (int i = 2; i < text.Length; i++)
            {
                char c = text[i];

                // }}が見つかったら、処理正常終了
                if (StringUtils.StartsWith(text, "}}", i))
                {
                    lastIndex = ++i;
                    break;
                }

                // | が含まれている場合、以降の文字列は引数などとして扱う
                if (c == '|')
                {
                    ++pipeCounter;
                    pipeTexts.Add(String.Empty);
                    continue;
                }

                // 変数（[[{{{1}}}]]とか）の再帰チェック
                string dummy;
                string variable;
                int index = this.ChkVariable(out variable, out dummy, text, i);
                if (index != -1)
                {
                    i = index;
                    if (pipeCounter > 0)
                    {
                        pipeTexts[pipeCounter - 1] += variable;
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
                        string subtext = text.Substring(i);
                        CommentElement comment;
                        string value;
                        if (CommentElement.TryParseLazy(subtext, out comment))
                        {
                            // コメント（<!--）が含まれている場合、リンクは無効
                            break;
                        }
                        else if (MediaWikiPage.TryParseNowiki(subtext, out value))
                        {
                            // nowikiブロック
                            i += value.Length - 1;
                            pipeTexts[pipeCounter - 1] += value;
                            continue;
                        }
                    }

                    // リンク [[ {{ （{{test|[[例]]}}とか）の再帰チェック
                    MediaWikiLink l;
                    index = this.ChkLinkText(out l, text, i);
                    if (index != -1)
                    {
                        i = index;
 //                       pipeTexts[pipeCounter - 1] += l.OriginalText;
                        continue;
                    }

                    pipeTexts[pipeCounter - 1] += c;
                }
            }

            // 解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            link = new MediaWikiLink();

            // 前後のスペース・改行は削除（見出しは後ろのみ）
            link.Title = article.Trim();

            // | 以降はそのまま設定
            link.PipeTexts = pipeTexts;

            // 記事名から情報を抽出
            // サブページ
            if (link.Title.StartsWith("/") == true)
            {
                link.IsSubpage = true;
            }
            else if (link.Title.StartsWith(":"))
            {
                // 先頭が :
                link.IsColon = true;
                link.Title = link.Title.TrimStart(':').TrimStart();
            }

            // 先頭が msgnw:
            //link.IsMsgnw = link.Title.ToLower().StartsWith(Msgnw.ToLower());
            //if (link.IsMsgnw)
            //{
            //    link.Title = link.Title.Substring(Msgnw.Length);
            //}

            //// 記事名直後の改行の有無
            //if (article.TrimEnd(' ').EndsWith("\n"))
            //{
            //    link.Enter = true;
            //}

            return true;
        }

        #endregion

        /// <summary>
        /// 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック。
        /// </summary>
        /// <param name="link">解析したリンク。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1。</returns>
        public int ChkLinkText(out MediaWikiLink link, string text, int index)
        {
            // 入力値に応じて、処理を振り分け
            if (StringUtils.StartsWith(text, "[[", index))
            {
                // 内部リンク
                if (this.TryParseLink(text.Substring(index), out link))
                {
  //                  return index + link.OriginalText.Length - 1;
                }
            }
            else if (StringUtils.StartsWith(text, "{{", index))
            {
                // テンプレート
                if (this.TryParseTemplate(text.Substring(index), out link))
                {
//                    return index + link.OriginalText.Length - 1;
                }
            }

            // 出力値初期化。リンク以外の場合、nullを返す
            link = null;
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
                    CommentElement comment;
                    if (CommentElement.TryParseLazy(text.Substring(i), out comment))
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
                        string nowiki;
                        if (MediaWikiPage.TryParseNowiki(text.Substring(i), out nowiki))
                        {
                            // nowikiブロック
                            i += nowiki.Length - 1;
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
                    MediaWikiLink link;
                    subindex = this.ChkLinkText(out link, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
 //                       value += link.OriginalText;
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
    }
}
