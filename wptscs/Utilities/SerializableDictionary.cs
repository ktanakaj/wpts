// ================================================================================================
// <summary>
//      �V���A���C�Y�\��Dictionary�N���X�\�[�X</summary>
//
// <copyright file="SerializableDictionary.cs" company="honeplus�̃�����">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// <remarks>http://d.hatena.ne.jp/lord_hollow/20090206 ���Q�l�ɍ쐬�����Ă����������\�[�X�B</remarks>
// 
// ================================================================================================

namespace Honememo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// �V���A���C�Y�\��Dictionary�N���X�ł��B
    /// </summary>
    /// <typeparam name="Tkey">�f�B�N�V���i�����̃L�[�̌^�B</typeparam>
    /// <typeparam name="Tvalue">�f�B�N�V���i�����̒l�̌^�B</typeparam>
    public class SerializableDictionary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>, IXmlSerializable
    {
        #region ���\�b�h

        /// <summary>
        /// �V���A���C�Y����XML�̃X�L�[�}��`��Ԃ��B
        /// </summary>
        /// <returns>XML�\�����L�q����XmlSchema�B</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// XML����I�u�W�F�N�g���f�V���A���C�Y����B
        /// </summary>
        /// <param name="reader">�f�V���A���C�Y����XmlReader</param>
        public void ReadXml(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));
            reader.Read();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                KeyValue kv = serializer.Deserialize(reader) as KeyValue;
                if (kv != null)
                {
                    Add(kv.Key, kv.Value);
                }
            }

            reader.Read();
        }

        /// <summary>
        /// �I�u�W�F�N�g��XML�ɃV���A���C�Y����B
        /// </summary>
        /// <param name="writer">�V���A���C�Y���XmlWriter</param>
        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));
            foreach (Tkey key in Keys)
            {
                serializer.Serialize(writer, new KeyValue(key, this[key]));
            }
        }
        
        #endregion

        #region �����N���X

        /// <summary>
        /// Dictionary�̃L�[�E�l���i�[���邽�߂̃N���X�ł��B
        /// </summary>
        public class KeyValue
        {
            #region �����N���Xprivate�ϐ�

            /// <summary>
            /// Dictionary�̃L�[�B
            /// </summary>
            private Tkey key;

            /// <summary>
            /// Dictionary�̒l�B
            /// </summary>
            private Tvalue value;

            #endregion

            #region �����N���X�R���X�g���N�^

            /// <summary>
            /// �R���X�g���N�^�B
            /// </summary>
            public KeyValue()
            {
            }

            /// <summary>
            /// �R���X�g���N�^�i�L�[�E�l���w��j�B
            /// </summary>
            /// <param name="key">�L�[</param>
            /// <param name="value">�l</param>
            public KeyValue(Tkey key, Tvalue value)
            {
                this.Key = key;
                this.Value = value;
            }

            #endregion

            #region �����N���X�v���p�e�B

            /// <summary>
            /// Dictionary�̃L�[�B
            /// </summary>
            public Tkey Key
            {
                get
                {
                    return this.key;
                }

                set
                {
                    this.key = value;
                }
            }

            /// <summary>
            /// Dictionary�̒l�B
            /// </summary>
            public Tvalue Value
            {
                get
                {
                    return this.value;
                }

                set
                {
                    this.value = value;
                }
            }

            #endregion
        }

        #endregion
    }
}
