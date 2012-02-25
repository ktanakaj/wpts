// ================================================================================================
// <summary>
//      状態変化を管理するためのクラスソース</summary>
//
// <copyright file="StatusManager.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 状態変化を管理するためのクラスです。
    /// </summary>
    /// <typeparam name="T">管理する状態の型。</typeparam>
    /// <remarks>
    /// このクラスは<see cref="IDisposable"/>を実装しますが、
    /// これはusingブロックでのステータス変更を目的としたもので、
    /// 特に解放が必要なリソースを持っているわけではありません。
    /// インスタンス解放時の<see cref="Dispose"/>は不要です。
    /// </remarks>
    public class StatusManager<T> : IDisposable
    {
        #region private変数

        /// <summary>
        /// 現在のステータス。
        /// </summary>
        private T status;

        /// <summary>
        /// <see cref="Switch"/>で使用する変更前のステータス。
        /// </summary>
        private Stack<T> oldStatus = new Stack<T>();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたステータスで初期化されたインスタンスを作成。
        /// </summary>
        /// <param name="status">ステータス。</param>
        public StatusManager(T status)
        {
            this.status = status;
        }

        /// <summary>
        /// ステータスが<typeparamref name="T"/>型のデフォルト値で初期化されたインスタンスを作成。
        /// </summary>
        public StatusManager()
        {
        }

        #endregion

        #region イベント

        /// <summary>
        /// ステータス変化後に呼ばれるイベント。
        /// </summary>
        public event EventHandler Changed;

        #endregion

        #region プロパティ

        /// <summary>
        /// 現在のステータス。
        /// </summary>
        /// <remarks>
        /// このプロパティからステータスを更新した時点で、
        /// <see cref="Switch"/>
        /// で処理中の変更前のステータスは全て消去されます。
        /// </remarks>
        public T Status
        {
            get
            {
                return this.status;
            }

            set
            {
                lock (this.oldStatus)
                {
                    this.oldStatus.Clear();
                    this.status = value;
                }

                this.CallChangedEvent();
            }
        }

        #endregion

        #region 公開メソッド

        /// <summary>
        /// ステータスを一時的に切り替える。
        /// </summary>
        /// <param name="status">新しいステータス。</param>
        /// <returns>このオブジェクト。</returns>
        /// <remarks>
        /// このメソッドで変更したステータスは、<see cref="Dispose"/>のタイミングで元の値に戻ります。
        /// 入れ子で再帰的に呼び出すことも可能です。
        /// </remarks>
        /// <example>
        /// ステータスを一時的に変更する場合、このメソッドを下記のように使用する。
        /// <code>
        /// using (var sm = this.statusManager.Switch("実行中"))
        /// {
        ///     // ステータス変更中に行う処理
        /// }
        /// </code>
        /// </example>
        public virtual StatusManager<T> Switch(T status)
        {
            lock (this.oldStatus)
            {
                this.oldStatus.Push(this.status);
                this.status = status;
            }

            this.CallChangedEvent();
            return this;
        }

        /// <summary>
        /// <see cref="Switch"/>で変更されたステータスを元に戻す。
        /// </summary>
        public virtual void Dispose()
        {
            lock (this.oldStatus)
            {
                if (this.oldStatus.Count > 0)
                {
                    this.status = this.oldStatus.Pop();
                }

                this.CallChangedEvent();
            }
        }

        /// <summary>
        /// ステータスを初期状態に戻す。
        /// </summary>
        public virtual void Clear()
        {
            this.Status = default(T);
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// <see cref="Changed"/>イベントに指定された処理を呼び出す。
        /// </summary>
        private void CallChangedEvent()
        {
            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
