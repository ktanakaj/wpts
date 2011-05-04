// ================================================================================================
// <summary>
//      MediaWikiの変数を解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiVariableParser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
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
    /// MediaWikiの変数を解析するパーサークラスです。
    /// </summary>
    public class MediaWikiVariableParser : AbstractParser
    {
        #region private変数

        /// <summary>
        /// このパーサーが参照する<see cref="MediaWikiParser"/>。
        /// </summary>
        private MediaWikiParser parser;

        #endregion
        
        #region コンストラクタ

        /// <summary>
        /// 指定された<see cref="MediaWikiParser"/>を元に変数を解析するためのパーサーを作成する。
        /// </summary>
        /// <param name="parser">このパーサーが参照する<see cref="MediaWikiParser"/>。</param>
        public MediaWikiVariableParser(MediaWikiParser parser)
        {
            this.parser = parser;
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// 渡されたテキストをMediaWikiの変数として解析する。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析した変数。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        public override bool TryParse(string s, out IElement result)
        {
            // 出力値初期化
            result = null;

            // 入力値確認
            if (!s.StartsWith(MediaWikiVariable.DelimiterStart))
            {
                return false;
            }

            // ブロック終了まで取得
            StringBuilder variable = new StringBuilder();
            StringBuilder value = null;
            bool pipeFlag = false;
            int lastIndex = -1;
            for (int i = 0 + MediaWikiVariable.DelimiterStart.Length; i < s.Length; i++)
            {
                // 終了条件のチェック
                if (StringUtils.StartsWith(s, MediaWikiVariable.DelimiterEnd, i))
                {
                    lastIndex = i + MediaWikiVariable.DelimiterEnd.Length - 1;
                    break;
                }

                IElement comment;
                if (this.TryParseElement(s, i, out comment, this.parser.CommentParser))
                {
                    // コメント（<!--）ブロック
                    i += comment.ToString().Length - 1;
                    continue;
                }

                // | が含まれている場合、以降の文字列は代入された値として扱う
                if (s[i] == '|')
                {
                    pipeFlag = true;
                    value = new StringBuilder();
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

                    variable.Append(s[i]);
                }
                else
                {
                    // | の後のとき
                    IElement element;
                    if (this.TryParseElement(s, i, out element, this.parser.NowikiParser, this, this.parser.LinkParser, this.parser.TemplateParser))
                    {
                        // nowiki、または変数（{{{1|{{{2}}}}}}とか）やリンク [[ {{ （{{{1|[[test]]}}}とか）の再帰チェック
                        i += element.ToString().Length - 1;
                        value.Append(element.ToString());
                        continue;
                    }

                    value.Append(s[i]);
                }
            }

            if (lastIndex < 0)
            {
                return false;
            }

            // 値については再帰的に探索
            IElement innerElement = null;
            if (value != null)
            {
                if (!this.parser.TryParse(value.ToString(), out innerElement))
                {
                    return false;
                }
            }

            // 変数ブロックの文字列を出力値に設定
            result = new MediaWikiVariable(variable.ToString(), innerElement);
            result.ParsedString = s.Substring(0, lastIndex + 1);

            return true;
        }

        /// <summary>
        /// 渡された文字が<see cref="TryParse"/>等の候補となる先頭文字かを判定する。
        /// </summary>
        /// <param name="c">解析文字列の先頭文字。</param>
        /// <returns>候補となる場合<c>true</c>。このクラスでは常に<c>true</c>を返す。</returns>
        /// <remarks>性能対策などで<see cref="TryParse"/>を呼ぶ前に目処を付けたい場合用。</remarks>
        public override bool IsPossibleParse(char c)
        {
            return MediaWikiVariable.DelimiterStart[0] == c;
        }

        #endregion
    }
}
