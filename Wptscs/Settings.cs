// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツールの設定アクセス用ソース</summary>
//
// <copyright file="Settings.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs.Properties
{    
    // このクラスでは設定クラスでの特定のイベントを処理することができます:
    //  SettingChanging イベントは、設定値が変更される前に発生します。
    //  PropertyChanged イベントは、設定値が変更された後に発生します。
    //  SettingsLoaded イベントは、設定値が読み込まれた後に発生します。
    //  SettingsSaving イベントは、設定値が保存される前に発生します。

    /// <summary>
    /// Wikipedia翻訳支援ツールの設定アクセス用クラスです。
    /// </summary>
    internal sealed partial class Settings
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public Settings()
        {
            // // 設定の保存と変更のイベント ハンドラを追加するには、以下の行のコメントを解除します:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
        }

        /// <summary>
        /// 設定値が変更される前の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // SettingChangingEvent イベントを処理するコードをここに追加してください。
        }
        
        /// <summary>
        /// 設定値が保存される前の処理。
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト。</param>
        /// <param name="e">発生したイベント。</param>
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // SettingsSaving イベントを処理するコードをここに追加してください。
        }
    }
}
