// ================================================================================================
// <summary>
//      アプリケーション起動用クラスソース</summary>
//
// <copyright file="Program.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;
    using Honememo.Wptscs.Properties;

    /// <summary>
    /// アプリケーション起動時に最初に呼ばれるクラスです。
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// 設定ファイルから表示言語の設定を読み込む。
        /// </summary>
        /// <remarks>特に表示言語が指定されていない場合は何もしない。</remarks>
        public static void LoadSelectedCulture()
        {
            if (!String.IsNullOrWhiteSpace(Settings.Default.LastSelectedLanguage))
            {
                try
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Settings.Default.LastSelectedLanguage);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(Settings.Default.LastSelectedLanguage);
                }
                catch (Exception ex)
                {
                    // 設定ファイルに手で不正な値が設定された場合など、万が一エラーになった場合デバッグログ
                    System.Diagnostics.Debug.WriteLine("Program.LoadSelectedCulture : " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // 初回実行時は古いバージョンの設定があればバージョンアップ
            if (!Settings.Default.IsUpgraded)
            {
                // 現バージョンを上書きしてしまうため一度だけ実施
                Settings.Default.Upgrade();
                Settings.Default.IsUpgraded = true;
            }

            // 表示言語の設定が存在する場合、画面表示前にその設定を読み込み
            Program.LoadSelectedCulture();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}