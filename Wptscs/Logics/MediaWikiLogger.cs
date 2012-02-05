// ================================================================================================
// <summary>
//      MediaWiki翻訳支援処理用のログテキストの生成を行うためのクラスソース</summary>
//
// <copyright file="MediaWikiLogger.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Text;
    using Honememo.Parsers;
    using Honememo.Wptscs.Parsers;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// MediaWiki翻訳支援処理用のログテキストの生成を行うためのクラスです。
    /// </summary>
    public class MediaWikiLogger : Logger
    {
        #region ログ登録メソッド（翻訳支援処理）

        /// <summary>
        /// 変換元を表すログを追加する。
        /// </summary>
        /// <param name="source">変換元を表す要素。</param>
        /// <remarks>
        /// 変換結果をログ出力する際は
        /// <code>AddSource</code>→(<see cref="AddAlias"/>)→<see cref="AddDestination"/>
        /// の順で一連のメソッドをコールする。
        /// 上記以外のメソッドをコールした場合、該当行の変換結果の出力は終了したものとみなす。
        /// </remarks>
        public override void AddSource(IElement source)
        {
            if (source is MediaWikiHeading)
            {
                // 見出しの場合、変換先が無い場合は単に「見出し」として出力するので矢印を出さない
                this.AddNewLineIfNotEndWithNewLine();
                this.Log += this.FormatElement(source);
            }
            else
            {
                // それ以外は独自に整形だけ行い、後は親クラスの処理を使用
                base.AddSource(this.FormatElement(source));
            }
        }

        /// <summary>
        /// リダイレクトを表すログを追加する。
        /// </summary>
        /// <param name="alias">リダイレクトを表す要素。</param>
        /// <remarks>
        /// 変換結果をログ出力する際は
        /// <see cref="AddSource"/>→(<code>AddAlias</code>)→<see cref="AddDestination"/>
        /// の順で一連のメソッドをコールする。
        /// 上記以外のメソッドをコールした場合、該当行の変換結果の出力は終了したものとみなす。
        /// </remarks>
        public override void AddAlias(IElement alias)
        {
            // 直前のログが見出しの場合矢印を出力、その後リダイレクトを出力
            this.AddRightArrowIfEndWithHeading();
            ListElement list = new ListElement();
            list.Add(new TextElement(Resources.LogMessageRedirect + " "));
            list.Add(this.FormatElement(alias));
            base.AddAlias(list);
        }

        /// <summary>
        /// 変換先を表すログを追加する。
        /// </summary>
        /// <param name="destination">変換先を表す要素。</param>
        /// <param name="cacheUsed">対訳表を使用している場合<code>true</code>。デフォルトは<code>false</code>。</param>
        /// <remarks>
        /// 変換結果をログ出力する際は
        /// <see cref="AddSource"/>→(<see cref="AddAlias"/>)→<code>AddDestination</code>
        /// の順で一連のメソッドをコールする。
        /// 上記以外のメソッドをコールした場合、該当行の変換結果の出力は終了したものとみなす。
        /// </remarks>
        public override void AddDestination(IElement destination, bool cacheUsed = false)
        {
            // 直前のログが見出しの場合矢印を出力、その後独自に整形した変換先を出力
            this.AddRightArrowIfEndWithHeading();
            base.AddDestination(this.FormatElement(destination), cacheUsed);
        }

        #endregion

        #region 内部処理用メソッド

        /// <summary>
        /// 渡された要素の種類に応じた翻訳支援処理ログ出力用の書式に変換した要素を返す。
        /// </summary>
        /// <param name="element">書式化する要素。</param>
        /// <returns>書式化した内容で<code>ToString</code>が可能な要素。</returns>
        private IElement FormatElement(IElement element)
        {
            // 要素のクラスに応じた書式に変換して返す
            if (element is MediaWikiTemplate)
            {
                return this.FormatMediaWikiTemplate((MediaWikiTemplate)element);
            }
            else if (element is MediaWikiLink)
            {
                return this.FormatMediaWikiLink((MediaWikiLink)element);
            }

            // どこにも該当しないものはそのまま
            return element;
        }

        /// <summary>
        /// 翻訳支援処理ログ出力用の書式に変換した要素を返す。
        /// </summary>
        /// <param name="element">書式化する要素。</param>
        /// <returns>書式化した内容で<code>ToString</code>が可能な要素。</returns>
        private IElement FormatMediaWikiTemplate(MediaWikiTemplate element)
        {
            // そのままだとログに出すには情報が多すぎるので、必要な部分だけ抜粋
            // ※ 現状は記事名のみ
            return new MediaWikiTemplate(element.Title);
        }

        /// <summary>
        /// 翻訳支援処理ログ出力用の書式に変換した要素を返す。
        /// </summary>
        /// <param name="element">書式化する要素。</param>
        /// <returns>書式化した内容で<code>ToString</code>が可能な要素。</returns>
        private IElement FormatMediaWikiLink(MediaWikiLink element)
        {
            // そのままだとログに出すには情報が多すぎるので、必要な部分だけ抜粋
            // ※ 現状は記事名のみ
            return new MediaWikiLink(element.Title);
        }

        /// <summary>
        /// 最終ログが見出しの場合、<see cref="AddSource"/>で出していない矢印を出力する。
        /// </summary>
        private void AddRightArrowIfEndWithHeading()
        {
            if (this.Log.EndsWith(MediaWikiHeading.DelimiterEnd.ToString()))
            {
                this.Log += " " + Resources.RightArrow + " ";
            }
        }

        #endregion
    }
}
