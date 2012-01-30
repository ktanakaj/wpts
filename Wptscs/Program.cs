// ================================================================================================
// <summary>
//      アプリケーション起動用クラスソース</summary>
//
// <copyright file="Program.cs" company="honeplusのメモ帳">
//      Copyright (C) 2011 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// アプリケーション起動時に最初に呼ばれるクラスです。
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // 初回実行時は古いバージョンの設定があればバージョンアップ
            if (!Properties.Settings.Default.IsUpgraded)
            {
                // 現バージョンを上書きしてしまうため一度だけ実施
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.IsUpgraded = true;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}