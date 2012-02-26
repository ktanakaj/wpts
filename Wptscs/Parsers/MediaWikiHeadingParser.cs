// ================================================================================================
// <summary>
//      MediaWikiの見出しを解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiHeadingParser.cs" company="honeplusのメモ帳">
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
        /// （ただし、--&lt;==見出し== みたいな事もできるので、その場合は=の開始部分から。）
        /// </remarks>
        public override bool TryParse(string s, out IElement result)
        {
            // 入力値確認、空の場合は即終了
            result = null;
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            // 始まりの = の数を数える
            // ※ 構文はWikipediaのプレビューで色々試して確認、足りなかったり間違ってたりするかも・・・
            // TODO: Wikipediaでは <!--test-->=<!--test-->=関連項目<!--test-->==<!--test--> みたいなのでも認識するが、2012年1月現在未対応
            //      （昔は対応していたが、その過程でコメントが失われれるつくりになっており、
            //        Parser周りを整理した際に情報を取りこぼさないことを最優先としたため取り止め。）
            int startCount = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == MediaWikiHeading.DelimiterStart)
                {
                    ++startCount;
                }
                else
                {
                    break;
                }
            }

            // = で始まる行ではない場合、処理対象外
            if (startCount < 1)
            {
                return false;
            }

            // 始まりの = の次の文字から、行の終わりまでを解析
            // （=={{lang\n|ja|見出し}}== みたいに何かの中にある改行はOK。Wikipediaでも認識された）
            IElement element;
            this.parser.TryParseToDelimiter(StringUtils.Substring(s, startCount), out element, "\r", "\n");

            // 終わりの = の数を確認
            // ※ この処理だと中身の無い行（====とか）は弾かれてしまうが、どうせ処理できないので許容する
            string substr = element.ToString().TrimEnd();
            int endCount = 0;
            for (int i = substr.Length - 1; i >= 0; i--)
            {
                if (substr[i] == MediaWikiHeading.DelimiterEnd)
                {
                    ++endCount;
                }
                else
                {
                    break;
                }
            }

            // = で終わる行ではない場合、処理対象外
            if (endCount < 1)
            {
                return false;
            }

            // 始まりと終わり、=の少ないほうにあわせる（==test===とか用の処理）
            int level = startCount;
            if (startCount > endCount)
            {
                level = endCount;
            }

            // 確定した見出しの階層から、見出し内部の文字列を抽出。内部要素を再帰的に探索する
            // ※ 二重処理になってしまうが、後ろの = を取り除くと微妙にややこしいことになりそうだったので
            //    見出しは処理件数も少なく、深い再帰もないはずなので、影響ない・・・はず
            IElement innerElement;
            if (!this.parser.TryParse(substr.Substring(0, substr.Length - level), out innerElement))
            {
                return false;
            }

            // 解析に成功した場合、結果を出力値に設定
            result = this.MakeElement(innerElement, level, s.Substring(0, startCount + element.ToString().Length));
            return true;
        }
        
        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// 見出しタグを解析した結果から、MediaWiki見出し要素を生成する。
        /// </summary>
        /// <param name="innerElement">見出しタグ上の見出し部分の要素。</param>
        /// <param name="level">見出しの階層。</param>
        /// <param name="parsedString">解析した見出しタグの文字列。</param>
        /// <returns>生成した見出し要素。</returns>
        private MediaWikiHeading MakeElement(IElement innerElement, int level, string parsedString)
        {
            MediaWikiHeading heading = new MediaWikiHeading();

            // 解析した見出しの素のテキストを保存
            heading.ParsedString = parsedString;

            // 見出しの階層を保存
            heading.Level = level;

            // 内部要素については、結果がリストの場合マージ、それ以外はそのままElementに代入
            if (innerElement.GetType() == typeof(ListElement))
            {
                heading.AddRange((ListElement)innerElement);
            }
            else
            {
                heading.Add(innerElement);
            }

            return heading;
        }

        #endregion
    }
}
