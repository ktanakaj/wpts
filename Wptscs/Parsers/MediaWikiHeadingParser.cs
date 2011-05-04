// ================================================================================================
// <summary>
//      MediaWikiの見出しを解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiHeadingParser.cs" company="honeplusのメモ帳">
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

    /// <summary>
    /// MediaWikiの見出しを解析するパーサークラスです。
    /// </summary>
    public class MediaWikiHeadingParser : AbstractParser
    {
        #region private変数

        /// <summary>
        /// このパーサーが参照する<see cref="MediaWikiParser"/>。
        /// </summary>
        private MediaWikiParser parser;

        #endregion
        
        #region コンストラクタ

        /// <summary>
        /// 指定された<see cref="MediaWikiParser"/>を元に見出しを解析するためのパーサーを作成する。
        /// </summary>
        /// <param name="parser">このパーサーが参照する<see cref="MediaWikiParser"/>。</param>
        public MediaWikiHeadingParser(MediaWikiParser parser)
        {
            this.parser = parser;
        }

        #endregion

        #region インタフェース実装メソッド

        /// <summary>
        /// 渡されたテキストをMediaWikiの見出し（==関連項目==みたいなの）として解析する。
        /// </summary>
        /// <param name="s">行頭からの文字列。</param>
        /// <param name="result">解析した見出し。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>
        /// 見出しは行単位で有効になるため、行頭からの文字列を渡す必要がある。
        /// 見出し行であればコメントが含まれていても解析する。
        /// </remarks>
        public override bool TryParse(string s, out IElement result)
        {
            // 出力値初期化
            result = null;

            // 構文を解析して、1行の文字列と、=の個数を取得
            // ※ 構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            // ※ Wikipediaでは <!--test-->=<!--test-->=関連項目<!--test-->==<!--test--> みたいなのでも
            //    正常に認識するので、できるだけ対応する
            // ※ 上記の見出し前後のコメントは、元文字列には含むが設定する方法はない。
            bool startFlag = true;
            int startSignCounter = 0;
            string nonCommentLine = String.Empty;
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                // 改行まで
                if (c == '\n' || c == '\r')
                {
                    break;
                }

                // コメントは無視する
                XmlCommentElement comment;
                if (XmlCommentElement.TryParseLazy(s.Substring(i), out comment))
                {
                    b.Append(comment.ToString());
                    i += comment.ToString().Length - 1;
                    continue;
                }
                else if (startFlag)
                {
                    // 先頭部の場合、=の数を数える
                    if (c == MediaWikiHeading.DelimiterStart)
                    {
                        ++startSignCounter;
                    }
                    else
                    {
                        startFlag = false;
                    }
                }

                nonCommentLine += c;
                b.Append(c);
            }

            string line = b.ToString();

            // = で始まる行ではない場合、処理対象外
            if (startSignCounter < 1)
            {
                return false;
            }

            // 終わりの = の数を確認
            // ※↓の処理だと中身の無い行（====とか）は弾かれてしまうが、どうせ処理できないので許容する
            nonCommentLine = nonCommentLine.TrimEnd();
            int endSignCounter = 0;
            for (int i = nonCommentLine.Length - 1; i >= startSignCounter; i--)
            {
                if (nonCommentLine[i] == MediaWikiHeading.DelimiterEnd)
                {
                    ++endSignCounter;
                }
                else
                {
                    break;
                }
            }

            // = で終わる行ではない場合、処理対象外
            if (endSignCounter < 1)
            {
                return false;
            }

            // 始まりと終わり、=の少ないほうにあわせる（==test===とか用の処理）
            int signCounter = startSignCounter;
            if (startSignCounter > endSignCounter)
            {
                signCounter = endSignCounter;
            }

            // 内部要素を再帰的に探索
            IElement innerElement;
            if (!this.parser.TryParse(nonCommentLine.Trim('='), out innerElement))
            {
                return false;
            }

            // 結果がリストの場合マージ、それ以外はそのままElementに代入
            MediaWikiHeading heading = new MediaWikiHeading();
            heading.Level = signCounter;
            heading.ParsedString = line;
            if (innerElement.GetType() == typeof(ListElement))
            {
                heading.AddRange((ListElement)innerElement);
            }
            else
            {
                heading.Add(innerElement);
            }

            result = heading;
            return true;
        }
        
        #endregion
    }
}
