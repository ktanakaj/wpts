// ================================================================================================
// <summary>
//      末尾がピリオドのページが取得できない既知の不具合に該当することを表す例外クラスソース</summary>
//
// <copyright file="EndPeriodException.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Utilities
{
    using System;

    /// <summary>
    /// 末尾がピリオドのページが取得できない既知の不具合に該当することを表す例外クラスです。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 末尾がピリオドのページが取得できない既知の不具合への暫定対応。
    /// もともとただの<see cref="NotSupportedException"/>を投げていたが、
    /// スキーム名が不正な場合等にもこの例外が飛ぶことが判明したため、
    /// 区別できるように作成。
    /// </para>
    /// <para>
    /// この問題は<see cref="IWebProxy"/>実装クラスでおきているため、
    /// このパッケージに定義する。
    /// </para>
    /// </remarks>
    public class EndPeriodException : NotSupportedException
    {
        #region コンストラクタ

        /// <summary>
        /// 指定したエラーメッセージを使用して、
        /// 新しい例外インスタンスを作成します。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        public EndPeriodException(string message)
            : base(message)
        {
        }

        #endregion
    }
}
