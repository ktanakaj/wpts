// ================================================================================================
// <summary>
//      MediaWikiの変数を解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiVariableParser.cs" company="honeplusのメモ帳">
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

            // 開始条件 {{{ のチェック
            if (s == null || !s.StartsWith(MediaWikiVariable.DelimiterStart))
            {
                return false;
            }

            // ブロック終了まで取得
            StringBuilder variable = new StringBuilder();
            IElement value = null;
            int lastIndex = -1;
            for (int i = MediaWikiVariable.DelimiterStart.Length; i < s.Length; i++)
            {
                // 終了条件 }}} のチェック
                if (StringUtils.StartsWith(s, MediaWikiVariable.DelimiterEnd, i))
                {
                    lastIndex = i + MediaWikiVariable.DelimiterEnd.Length - 1;
                    break;
                }

                // {{{変数名|デフォルト値}}} といったフォーマットのため、| の前後で処理を変更
                if (s[i] == '|')
                {
                    // | の後（変数のデフォルト値など）は何でもありえるので親のパーサーで再帰的に解析
                    if (!this.parser.TryParseToDelimiter(StringUtils.Substring(s, i + 1), out value, MediaWikiVariable.DelimiterEnd))
                    {
                        // 平文でも解析するメソッドのため、基本的に失敗することは無い
                        // 万が一の場合は解析失敗とする
                        break;
                    }

                    i += value.ToString().Length;
                }
                else
                {
                    // | の前（変数名の部分）のとき、変数・コメントの再帰チェック
                    IElement element;
                    if (this.TryParseAt(s, i, out element, this.parser.CommentParser, this.parser.VariableParser))
                    {
                        // 変数・コメントなら、解析したブロック単位で変数名に追加
                        i += element.ToString().Length - 1;
                        variable.Append(element.ToString());
                        continue;
                    }

                    // それ以外の普通の文字なら1文字ずつ変数名に追加
                    variable.Append(s[i]);
                }
            }

            // 終了条件でループを抜けていない場合、解析失敗
            if (lastIndex < 0)
            {
                return false;
            }

            // 変数名・値と、解析した素の文字列を結果に格納して終了
            result = new MediaWikiVariable(variable.ToString(), value);
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
