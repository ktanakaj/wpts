// ================================================================================================
// <summary>
//      ネットワークを使用する翻訳支援処理を実装するための共通クラスソース</summary>
//
// <copyright file="TranslateNetworkObject.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Logics
{
    using System;
    using System.Net.NetworkInformation;

    using Honememo.Wptscs.Models;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// ネットワークを使用する翻訳支援処理を実装するための共通クラスです。
    /// </summary>
    public abstract class TranslateNetworkObject : Translate
    {
        #region private変数

        /// <summary>
        /// 通信時に使用するUserAgent。
        /// </summary>
        public string UserAgent;

        /// <summary>
        /// 通信時に使用するReferer。
        /// </summary>
        public string Referer;
        
        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="source">翻訳元言語。</param>
        /// <param name="target">翻訳先言語。</param>
        public TranslateNetworkObject(
            LanguageWithServerInformation source, LanguageWithServerInformation target)
            : base(source, target)
        {
        }
        
        #endregion
        
        #region メソッド

        /// <summary>
        /// 翻訳支援処理実行。
        /// </summary>
        /// <param name="name">記事名。</param>
        /// <returns><c>true</c> 処理成功。</returns>
        public override bool Run(string name)
        {
            // 変数を初期化
            RunInitialize();

            // サーバー接続チェック
            if (this.Ping(((LanguageWithServerInformation) source).Server) == false)
            {
                return false;
            }

            // 翻訳支援処理実行部の本体を実行
            // ※以降の処理は、継承クラスにて定義
            return RunBody(name);
        }

        /// <summary>
        /// サーバー接続チェック。
        /// </summary>
        /// <param name="server">サーバー名。</param>
        /// <returns><c>true</c> 接続成功。</returns>
        private bool Ping(string server)
        {
            // サーバー接続チェック
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(server);
                if (reply.Status != IPStatus.Success)
                {
                    LogLine(String.Format(Resources.ErrorMessage_MissNetworkAccess, reply.Status.ToString()));
                    return false;
                }
            }
            catch (Exception e)
            {
                LogLine(String.Format(Resources.ErrorMessage_MissNetworkAccess, e.InnerException.Message));
                return false;
            }

            return true;
        }
        
        #endregion
    }
}
