// ================================================================================================
// <summary>
//      IWebProxyのリトライイベントクラスソース</summary>
//
// <copyright file="RetryEventArgs.cs" company="honeplusのメモ帳">
//      Copyright (C) 2026 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

using System;

namespace Honememo.Wptscs.Utilities;

/// <summary>
/// IWebProxyのリトライイベントクラスです。
/// </summary>
public class RetryEventArgs
{
    #region コンストラクタ

    /// <summary>
    /// 指定された情報から、リトライイベントを生成する。
    /// </summary>
    /// <param name="retryCount">リトライ回数。</param>
    /// <param name="waitTime">ウェイト時間。</param>
    /// <param name="message">リトライメッセージ。</param>
    public RetryEventArgs(int retryCount, TimeSpan waitTime, string message)
    {
        this.RetryCount = retryCount;
        this.WaitTime = waitTime;
        this.Message = message;
    }

    #endregion

    #region プロパティ

    /// <summary>
    /// リトライ回数。
    /// </summary>
    public int RetryCount { get; private set; }

    /// <summary>
    /// ウェイト時間。
    /// </summary>
    public TimeSpan WaitTime { get; private set; }

    /// <summary>
    /// リトライメッセージ。
    /// </summary>
    public string Message { get; private set; }

    #endregion
}
