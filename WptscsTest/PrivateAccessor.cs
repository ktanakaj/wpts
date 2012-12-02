// ================================================================================================
// <summary>
//      ユニットテスト支援用クラスソース。</summary>
//
// <copyright file="PrivateAccessor.cs" company="honeplusのメモ帳">
//      Copyright (C) 2012 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// ユニットテスト用に private, protected といった外部からアクセスできないメソッドを実行するためのクラス。
    /// クラス・オブジェクトに被せるように使用する。
    /// </summary>
    /// <typeparam name="T">ラップするオブジェクトのクラス。</typeparam>
    /// <remarks>使い方については、概ねJUnitのPrivateAccessorと同じようなイメージ。</remarks>
    public class PrivateAccessor<T>
    {
        #region private変数

        /// <summary>
        /// オブジェクトを生成する際に使用するコンストラクタ。
        /// </summary>
        private ConstructorInfo constructor;

        /// <summary>
        /// テストするメソッド。
        /// </summary>
        private MethodInfo method;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 指定されたオブジェクトをテストするためのインスタンスを生成。
        /// </summary>
        /// <param name="obj">オブジェクト。</param>
        public PrivateAccessor(T obj)
        {
            this.Instance = obj;
        }

        /// <summary>
        /// 新しいオブジェクトをテストするためのインスタンスを生成。
        /// </summary>
        public PrivateAccessor()
        {
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// テストする／したクラスのオブジェクト。
        /// </summary>
        public T Instance
        {
            get;
            set;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// テストするクラスのコンストラクタを指定する。
        /// </summary>
        /// <param name="parameterTypes">パラメータ配列。</param>
        /// <remarks>コンストラクタの確保に失敗した場合は、<see cref="Assert.Fail()"/>にてテストを失敗させる。</remarks>
        public void SetConstructor(params Type[] parameterTypes)
        {
            try
            {
                // 指定されたコンストラクタを取得
                this.constructor = typeof(T).GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    parameterTypes,
                    null);
                if (this.constructor == null)
                {
                    // メソッド取得失敗のためテスト失敗
                    Assert.Fail("Constructor is not found");
                }
            }
            catch (Exception e)
            {
                // メソッド指定誤り、アクセス不可も含め全てテスト失敗
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// テストするクラスのオブジェクトを指定されたパラメータで生成する。
        /// </summary>
        /// <param name="initargs">コンストラクタ呼び出しに引数として渡すオブジェクトの配列。</param>
        /// <returns>このオブジェクトが表すコンストラクタを呼び出すことで作成される新規オブジェクト。</returns>
        /// <remarks>
        /// コンストラクタには、<see cref="SetConstructor"/>で指定されたメソッドを用いる。
        /// 指定されていない場合は、デフォルトコンストラクタを使用する。
        /// オブジェクトの生成に失敗した場合は、<see cref="Assert.Fail()"/>にてテストを失敗させる。
        /// </remarks>
        public T NewInstance(params object[] initargs)
        {
            try
            {
                // コンストラクタが指定されておらず、パラメータも0件の場合
                if (this.constructor == null
                        && (initargs == null || initargs.Length == 0))
                {
                    // デフォルトコンストラクタを使用する
                    this.SetConstructor();
                }

                // 指定されているコンストラクタでオブジェクトを生成
                this.Instance = (T)this.constructor.Invoke(initargs);

                // 生成したオブジェクトを返す
                return this.Instance;
            }
            catch (TargetInvocationException e)
            {
                // コンストラクタが例外を投げる場合は、そのまま返す
                throw e.InnerException;
            }
            catch (Exception e)
            {
                // オブジェクトが生成できない場合、テスト失敗
                Assert.Fail(e.Message);

                // failの時点で処理は終了する、コンパイルエラー抑止
                throw new Exception("Accessor error", e);
            }
        }

        /// <summary>
        /// テストするメソッドを指定する。
        /// </summary>
        /// <param name="name">メソッドの名前。</param>
        /// <param name="parameterTypes">パラメータ配列。</param>
        /// <remarks>メソッドの確保に失敗した場合は、<see cref="Assert.Fail()"/>にてテストを失敗させる。</remarks>
        public void SetMethod(string name, params Type[] parameterTypes)
        {
            try
            {
                // 指定されたメソッドを取得
                this.method = typeof(T).GetMethod(
                    name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                    null,
                    parameterTypes,
                    null);
                if (this.method == null)
                {
                    // メソッド取得失敗のためテスト失敗
                    Assert.Fail(name + " is not found");
                }
            }
            catch (Exception e)
            {
                // メソッド指定誤り、アクセス不可も含め全てテスト失敗
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// 指定されているメソッドを実行する。
        /// </summary>
        /// <param name="args">メソッド呼び出しに使用される引数。</param>
        /// <returns>このオブジェクトが表すメソッドを、パラメータ<paramref name="args"/>を使用して実行した結果。</returns>
        /// <remarks>
        /// staticメソッド以外の場合必要な呼び出し元オブジェクトには、
        /// <see cref="NewInstance"/>または<see cref="Instance"/>
        /// で指定されたオブジェクトを用いる。
        /// 指定されていない場合は、可能であればデフォルトコンストラクタでオブジェクトを生成する。
        /// メソッドの実行に失敗した場合は、<see cref="Assert.Fail()"/>にてテストを失敗させる。
        /// </remarks>
        public object Invoke(params object[] args)
        {
            try
            {
                // オブジェクトが指定されておらず、非staticメソッドの場合
                if (this.Instance == null && !this.method.IsStatic)
                {
                    // デフォルトコンストラクタでオブジェクトを作成する
                    // ※ コンストラクタが指定されている場合、例外
                    this.NewInstance();
                }

                // 指定されているオブジェクトでメソッドを実行
                return this.method.Invoke(this.Instance, args);
            }
            catch (TargetInvocationException e)
            {
                // メソッドが例外を投げる場合は、そのまま返す
                throw e.InnerException;
            }
            catch (Exception e)
            {
                // メソッド未設定、型違いも含めて全てテスト失敗
                Assert.Fail(e.Message);

                // failの時点で処理は終了する、コンパイルエラー抑止
                throw new Exception("Accessor error", e);
            }
        }

        /// <summary>
        /// テストする／したクラスのオブジェクトより、指定されたフィールドの値を返す。
        /// </summary>
        /// <param name="name">フィールド名。</param>
        /// <returns>フィールドの値。</returns>
        /// <remarks>値の取得に失敗した場合は、<see cref="Assert.Fail()"/>にてテストを失敗させる。</remarks>
        public object GetField(string name)
        {
            try
            {
                // 指定されたフィールドを取得
                FieldInfo field = typeof(T).GetField(
                    name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (field == null)
                {
                    // メソッド取得失敗のためテスト失敗
                    Assert.Fail(name + " is not found");
                }

                // 指定された値をフィールドから取得する
                return field.GetValue(this.Instance);
            }
            catch (Exception e)
            {
                // フィールド指定誤り、アクセス不可、オブジェクト無しも含め全てテスト失敗
                Assert.Fail(e.Message);

                // failの時点で処理は終了する、コンパイルエラー抑止
                throw new Exception("Accessor error", e);
            }
        }

        /// <summary>
        /// テストするクラスのオブジェクトで、指定されたフィールドに値を設定する。
        /// </summary>
        /// <param name="name">フィールド名。</param>
        /// <param name="value">新しいフィールド値。</param>
        /// <remarks>
        /// staticフィールド以外の場合に必要な設定先オブジェクトには、
        /// <see cref="NewInstance"/>または<see cref="Instance"/>
        /// で指定されたオブジェクトを用いる。
        /// 指定されていない場合は、可能であればデフォルトコンストラクタでオブジェクトを生成する。
        /// 値の設定に失敗した場合は、<see cref="Assert.Fail()"/>にてテストを失敗させる。
        /// </remarks>
        public void SetField(string name, object value)
        {
            try
            {
                // 指定されたフィールドを取得
                FieldInfo field = typeof(T).GetField(
                    name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (field == null)
                {
                    // メソッド取得失敗のためテスト失敗
                    Assert.Fail(name + " is not found");
                }

                // オブジェクトが指定されておらず、非staticフィールドの場合
                if (this.Instance == null && !field.IsStatic)
                {
                    // デフォルトコンストラクタでオブジェクトを作成する
                    // ※ コンストラクタが指定されている場合、例外
                    this.NewInstance();
                }

                // 指定された値をフィールドに設定する
                field.SetValue(this.Instance, value);
            }
            catch (Exception e)
            {
                // フィールド指定誤り、アクセス不可も含め全てテスト失敗
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// テストする／したクラスのオブジェクトより、指定されたプロパティの値を返す。
        /// </summary>
        /// <param name="name">プロパティ名。</param>
        /// <param name="index">インデックスプロパティの場合のインデックス。</param>
        /// <returns>プロパティの値。</returns>
        /// <remarks>値の取得に失敗した場合は、<see cref="Assert.Fail()"/>にてテストを失敗させる。</remarks>
        public object GetProperty(string name, params object[] index)
        {
            try
            {
                // 指定されたプロパティを取得
                PropertyInfo property = typeof(T).GetProperty(
                    name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property == null)
                {
                    // メソッド取得失敗のためテスト失敗
                    Assert.Fail(name + " is not found");
                }

                // 指定された値をプロパティから取得する
                return property.GetValue(this.Instance, index);
            }
            catch (Exception e)
            {
                // プロパティ指定誤り、アクセス不可、オブジェクト無しも含め全てテスト失敗
                Assert.Fail(e.Message);

                // failの時点で処理は終了する、コンパイルエラー抑止
                throw new Exception("Accessor error", e);
            }
        }

        /// <summary>
        /// テストするクラスのオブジェクトで、指定されたプロパティに値を設定する。
        /// </summary>
        /// <param name="name">プロパティ名。</param>
        /// <param name="value">新しいプロパティ値。</param>
        /// <param name="index">インデックスプロパティの場合のインデックス。</param>
        /// <remarks>
        /// 設定先オブジェクトには<see cref="NewInstance"/>または<see cref="Instance"/>
        /// で指定されたオブジェクトを用いる。
        /// 指定されていない場合は、可能であればデフォルトコンストラクタでオブジェクトを生成する。
        /// 値の設定に失敗した場合は、<see cref="Assert.Fail()"/>にてテストを失敗させる。
        /// </remarks>
        public void SetProperty(string name, object value, params object[] index)
        {
            try
            {
                // 指定されたプロパティを取得
                PropertyInfo property = typeof(T).GetProperty(
                    name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property == null)
                {
                    // メソッド取得失敗のためテスト失敗
                    Assert.Fail(name + " is not found");
                }

                // オブジェクトが指定されていない場合
                if (this.Instance == null)
                {
                    // デフォルトコンストラクタでオブジェクトを作成する
                    // ※ コンストラクタが指定されている場合、例外
                    this.NewInstance();
                }

                // 指定された値をプロパティに設定する
                property.SetValue(this.Instance, value, index);
            }
            catch (TargetInvocationException e)
            {
                // メソッドが例外を投げる場合は、そのまま返す
                throw e.InnerException;
            }
            catch (Exception e)
            {
                // プロパティ指定誤り、アクセス不可も含め全てテスト失敗
                Assert.Fail(e.Message);
            }
        }

        #endregion
    }
}
