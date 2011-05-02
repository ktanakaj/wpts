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
            XmlParser parser = new XmlParser();
            XmlElement element;
            if (parser.TryParseXmlElement(text, out element))
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
        /// 渡されたテキストの指定された位置に存在するWikipediaの内部リンク・テンプレートをチェック。
        /// </summary>
        /// <param name="element">解析したリンク。</param>
        /// <param name="text">解析するテキスト。</param>
        /// <param name="index">解析開始インデックス。</param>
        /// <returns>正常時の戻り値には、]]の後ろの]の位置のインデックスを返す。異常時は-1。</returns>
        public int ChkLinkText(out IElement element, string text, int index)
        {
            // 入力値に応じて、処理を振り分け
            if (StringUtils.StartsWith(text, "[[", index))
            {
                // 内部リンク
                MediaWikiLink linkElement;
                if (MediaWikiLink.TryParse(text.Substring(index), this, out linkElement))
                {
                    element = linkElement;
                    return index + element.Length - 1;
                }
            }
            else if (StringUtils.StartsWith(text, "{{", index))
            {
                // テンプレート
                MediaWikiTemplate templateElement;
                if (MediaWikiTemplate.TryParse(text.Substring(index), this, out templateElement))
                {
                    element = templateElement;
                    return index + element.Length - 1;
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
                    IElement link;
                    subindex = this.ChkLinkText(out link, text, i);
                    if (subindex != -1)
                    {
                        i = subindex;
                        //value += link.OriginalText;
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
            CommentElement commentElement;
            XmlElement xmlElement;
            MediaWikiLink linkElement;
            MediaWikiTemplate templateElement;
            MediaWikiHeading headingElement;
            if (CommentElement.TryParseLazy(s, out commentElement))
            {
                result = commentElement;
                return true;
            }
            else if (XmlElement.TryParse(s, this, out xmlElement))
            {
                // TODO: XmlElementは微妙な構文が保持されないのでそのままでは使えない
                result = xmlElement;
                return true;
            }
            else if (MediaWikiLink.TryParse(s, this, out linkElement))
            {
                result = linkElement;
                return true;
            }
            else if (MediaWikiTemplate.TryParse(s, this, out templateElement))
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
