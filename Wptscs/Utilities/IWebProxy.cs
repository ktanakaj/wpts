// ================================================================================================
// <summary>
//      ウェブアクセス処理を隠蔽するプロキシインタフェースソース</summary>
//
// <copyright file="IWebProxy.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Utilities
{
    using System;
    using System.IO;

    /// <summary>
    /// ウェブアクセス処理を隠蔽するプロキシのインタフェースです。
    /// </summary>
    public interface IWebProxy
    {
        #region プロパティ

        /// <summary>
        /// このプロキシで使用するUserAgent。
        /// </summary>
        string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// このプロキシで使用するReferer。
        /// </summary>
        string Referer
        {
            get;
            set;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 指定されたURIの情報をストリームで取得。
        /// </summary>
        /// <param name="uri">取得対象のURI。</param>
        /// <returns>取得したストリーム。使用後は必ずクローズすること。</returns>
        /// <remarks>取得できない場合（通信エラーなど）は例外を投げる。</remarks>
        Stream GetStream(Uri uri);

        #endregion
    }
}
