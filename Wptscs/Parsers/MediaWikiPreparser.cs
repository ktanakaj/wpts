// ================================================================================================
// <summary>
//      MediaWikiページ解析の前処理用の解析を行うパーサークラスソース</summary>
//
// <copyright file="MediaWikiPreparser.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Parsers
{
    using System;
    using System.Collections.Generic;
    using Honememo.Parsers;
    using Honememo.Utilities;

    /// <summary>
    /// MediaWikiページ解析の前処理用の解析を行うパーサークラスです。
    /// </summary>
    /// <remarks>
    /// <para>
    /// MediaWikiのページに対して、コメント, includeonly, noincludeの解析を行います。
    /// </para>
    /// <para>
    /// <see cref="IParser.Parse"/>等では上記要素を解析した結果を返します。
    /// <see cref="MediaWikiParser"/>の前処理として使用する場合は、
    /// 解析結果に対して別途
    /// <see cref="FilterByNoinclude"/>, <see cref="FilterByInclude"/>
    /// の処理を行うか、
    /// <see cref="PreprocessByNoinclude"/>, <see cref="PreprocessByInclude"/>
    /// を用いてください。
    /// </para>
    /// </remarks>
    public class MediaWikiPreparser : XmlParser
    {
        #region 定数宣言

        /// <summary>
        /// includeonlyタグ。
        /// </summary>
        private static readonly string IncludeonlyTag = "includeonly";

        /// <summary>
        /// noincludeタグ。
        /// </summary>
        private static readonly string NoincludeTag = "noinclude";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// MediaWikiのページの前処理用の解析を行うパーサーを作成する。
        /// </summary>
        public MediaWikiPreparser()
        {
            // コメント, nowiki, includeonly, noincludeのみを処理対象とする
            this.Parsers = new IParser[]
            {
                new XmlCommentElementParser(),
                new MediaWikiNowikiParser(this),
                new XmlElementParser(this) { Targets = new string[] { IncludeonlyTag, NoincludeTag } }
            };
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 通常ページ解析用の前処理を行う。
        /// </summary>
        /// <param name="text">ページテキスト。</param>
        /// <returns>前処理を行ったページテキスト。</returns>
        /// <remarks>通常のページ解析を行う際はこの処理を用いる。</remarks>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public static string PreprocessByNoinclude(string text)
        {
            return PreprocessTag(text, false);
        }

        /// <summary>
        /// インクルードページ解析用の前処理を行う。
        /// </summary>
        /// <param name="text">ページテキスト。</param>
        /// <returns>前処理を行ったページテキスト。</returns>
        /// <remarks>テンプレート呼び出しによるページ解析を行う際はこの処理を用いる。</remarks>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        public static string PreprocessByInclude(string text)
        {
            return PreprocessTag(text, true);
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// 渡された解析結果内のnoinclude要素の展開と、includeonly, コメント要素の除去を行う。
        /// </summary>
        /// <param name="element">
        /// 要素を展開／除去する解析結果。
        /// <see cref="ListElement"/>の場合その内部の上記要素が更新／除去される。
        /// noincludeの<see cref="XmlElement"/>の場合、内部要素の展開を、
        /// includeonlyまたは<see cref="XmlCommentElement"/>要素の場合、<c>null</c>への置き換えを行う。
        /// </param>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        /// <remarks>通常のページ解析を行う際はこの処理を用いる。</remarks>
        public void FilterByNoinclude(ref IElement element)
        {
            this.FilterTag(ref element, false);
        }

        /// <summary>
        /// 渡された解析結果内のincludeonly要素の展開と、noinclude, コメント要素の除去を行う。
        /// </summary>
        /// <param name="element">
        /// 要素を展開／除去する解析結果。
        /// <see cref="ListElement"/>の場合その内部の上記要素が更新／除去される。
        /// includeonlyの<see cref="XmlElement"/>の場合、内部要素の展開を、
        /// noincludeまたは<see cref="XmlCommentElement"/>要素の場合、<c>null</c>への置き換えを行う。
        /// </param>
        /// <exception cref="ArgumentNullException"><c>null</c>が指定された場合。</exception>
        /// <remarks>テンプレート呼び出しによるページ解析を行う際はこの処理を用いる。</remarks>
        public void FilterByInclude(ref IElement element)
        {
            this.FilterTag(ref element, true);
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// 指定されたパラメータに応じた、ページ解析用の前処理を行う。
        /// </summary>
        /// <param name="text">ページテキスト。</param>
        /// <param name="include">インクルードとして処理する場合<c>true</c>。</param>
        /// <returns>前処理を行ったページテキスト。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/>が<c>null</c>の場合。</exception>
        private static string PreprocessTag(string text, bool include)
        {
            IElement element;
            using (MediaWikiPreparser parser = new MediaWikiPreparser())
            {
                element = parser.Parse(text);
                parser.FilterTag(ref element, include);
            }

            return ObjectUtils.ToString(element);
        }

        /// <summary>
        /// 渡された解析結果から、指定に応じてincludeonly, noinclude、またコメント要素を展開／除去する。
        /// </summary>
        /// <param name="element">
        /// 要素を展開／除去する解析結果。
        /// <see cref="ListElement"/>の場合その内部の上記要素が更新／除去される。
        /// includeonly, noincludeの<see cref="XmlElement"/>の場合、更新または<c>null</c>への置き換えを、
        /// <see cref="XmlCommentElement"/>要素の場合、<c>null</c>への置き換えを行う。
        /// </param>
        /// <param name="include">インクルードとして処理する場合<c>true</c>。</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/>が<c>null</c>の場合。</exception>
        private void FilterTag(ref IElement element, bool include)
        {
            if (Validate.NotNull(element, "element") is XmlCommentElement)
            {
                // XMLコメントは除去
                element = null;
                return;
            }
            else if (element is XmlElement)
            {
                // XML/HTML要素の場合、タグの種類を見て対応
                XmlElement xml = (XmlElement)element;
                string name = xml.Name.ToLower();
                if ((include && name == IncludeonlyTag)
                    || (!include && name == NoincludeTag))
                {
                    // インクルードでincludeonly、非インクルードでnoincludeは中身を展開
                    // ※ このリストは、この後↓でもう一度処理される
                    ListElement list = new ListElement();
                    list.AddRange(xml);
                    element = list;
                }
                else if ((include && name == NoincludeTag)
                    || (!include && name == IncludeonlyTag))
                {
                    // インクルードでnoinclude、非インクルードでincludeonlyは中身を展開
                    element = null;
                    return;
                }
                else
                {
                    // それ以外のXML/HTML要素はそのまま
                    return;
                }
            }

            if (element is ListElement)
            {
                // リスト要素の場合、中身を再帰的に処理
                // ※ XML/HTMLの処理で中身を展開したものもここで処理
                ListElement list = (ListElement)element;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    IElement e = list[i];
                    this.FilterTag(ref e, include);
                    if (e == null)
                    {
                        list.RemoveAt(i);
                    }
                    else
                    {
                        list[i] = e;
                    }
                }
            }
        }

        #endregion
    }
}
