// ================================================================================================
// <summary>
//      MediaWikiのnowikiブロックを解析するパーサークラスソース</summary>
//
// <copyright file="MediaWikiNowikiParser.cs" company="honeplusのメモ帳">
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
    using Honememo.Wptscs.Websites;

    /// <summary>
    /// MediaWikiのnowikiブロックを解析するパーサークラスです。
    /// </summary>
    /// <remarks>
    /// <para>
    /// nowikiブロックでは、MediaWikiの各種構文やコメントも含めたhtmlタグが全て無効化されます。
    /// </para>
    /// <para>
    /// ブロック内の再帰解析を行わないパーサーというだけなので、
    /// 使おうと思えばMediaWiki以外のページに対しても使用可能です。
    /// </para>
    /// </remarks>
    public class MediaWikiNowikiParser : XmlElementParser
    {
        #region 定数宣言

        /// <summary>
        /// nowikiタグ。
        /// </summary>
        private static readonly string NowikiTag = "nowiki";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// MediaWikiのnowikiブロックを解析するためのパーサーを作成する。
        /// </summary>
        /// <param name="parser">このパーサーが参照する<see cref="XmlParser"/>。</param>
        public MediaWikiNowikiParser(XmlParser parser)
        {
            // nowikiブロックではMediaWikiの各種構文やコメントも含むhtmlタグも全て無効なため、
            // 親クラスにそうした処理を含まない空のXMLParserを指定する。
            // ※ HTML/XMLの扱い等に関する設定はMediaWiki全体のものを引き継ぐ
            // ※ XmlParserの仕様に依存するため良くないが、XmlParser.Disposeで行う
            //    Parsersの解放が結果的に不要になっているので、このクラスではIDisposableを実装していない
            this.Parser = new XmlParser();
            this.Parser.Parsers = new IParser[0];
            this.Parser.IgnoreCase = parser.IgnoreCase;
            this.Parser.IsHtml = parser.IsHtml;

            // nowikiタグ専用に設定。変更されると困るので読み取り専用リストに
            List<string> list = new List<string>();
            list.Add(NowikiTag);
            base.Targets = list.AsReadOnly();
        }

        #endregion

        #region 公開プロパティ

        /// <summary>
        /// このパーサーの解析対象のタグ。
        /// </summary>
        /// <exception cref="NotSupportedException">値を更新しようとした場合。</exception>
        /// <remarks>
        /// このパーサーはnowikiタグ専用です。値の変更・追加はできません。
        /// </remarks>
        public override IList<string> Targets
        {
            get
            {
                return base.Targets;
            }

            set
            {
                throw new NotSupportedException(NowikiTag + " only");
            }
        }

        #endregion

        #region インタフェース実装メソッド
        
        /// <summary>
        /// 渡されたテキストをMediaWikiのnowikiブロックとして解析する。
        /// </summary>
        /// <param name="s">解析対象の文字列。</param>
        /// <param name="result">解析したnowikiブロック。</param>
        /// <returns>解析に成功した場合<c>true</c>。</returns>
        /// <remarks>
        /// nowikiブロックと判定するには、1文字目が開始タグである必要がある。
        /// ただし、後ろについては閉じタグが無ければ全て、あればそれ以降は無視する。
        /// </remarks>
        public override bool TryParse(string s, out IElement result)
        {
            result = null;
            IElement element;
            if (base.TryParse(s, out element))
            {
                // nowiki区間は内部要素を全てテキストとして扱う
                XmlElement xmlElement = (XmlElement)element;
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

            return false;
        }
        
        #endregion
    }
}
