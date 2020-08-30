﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace NewLife.Security
{
    /// <summary>椭圆曲线数字签名算法 (ECDSA) </summary>
    public static class ECDsaHelper
    {
        #region 生成密钥
        /// <summary>产生非对称密钥对</summary>
        /// <param name="keySize">密钥长度，默认521位强密钥</param>
        /// <returns></returns>
        public static String[] GenerateKey(Int32 keySize = 521)
        {
            var dsa = new ECDsaCng(keySize);

            var ss = new String[2];
            ss[0] = dsa.Key.Export(CngKeyBlobFormat.EccPrivateBlob).ToBase64();
            ss[1] = dsa.Key.Export(CngKeyBlobFormat.EccPublicBlob).ToBase64();

            return ss;
        }

        /// <summary>创建ECDsa对象，支持Base64密钥和Pem密钥</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ECDsaCng Create(String key)
        {
            key = key?.Trim();
            if (key.IsNullOrEmpty()) return null;

            if (key.StartsWith("-----") && key.EndsWith("-----"))
            {
                var ec = new ECDsaCng();
                var p = ReadPem(key);
                //ec.ImportParameters(ReadPem(key));
                return ec;
            }
            else
            {
                var buf = key.ToBase64();
                var ckey = CngKey.Import(buf, buf.Length < 100 ? CngKeyBlobFormat.EccPublicBlob : CngKeyBlobFormat.EccPrivateBlob);

                return new ECDsaCng(ckey);
            }
        }
        #endregion

        #region 数字签名
        /// <summary>签名，MD5散列</summary>
        /// <param name="data"></param>
        /// <param name="priKey"></param>
        /// <returns></returns>
        public static Byte[] Sign(Byte[] data, String priKey)
        {
            var key = CngKey.Import(priKey.ToBase64(), CngKeyBlobFormat.EccPrivateBlob);
            var ecc = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.MD5 };

            return ecc.SignData(data);
        }

        /// <summary>验证，MD5散列</summary>
        /// <param name="data"></param>
        /// <param name="pukKey"></param>
        /// <param name="rgbSignature"></param>
        /// <returns></returns>
        public static Boolean Verify(Byte[] data, String pukKey, Byte[] rgbSignature)
        {
            var key = CngKey.Import(pukKey.ToBase64(), CngKeyBlobFormat.EccPublicBlob);
            var ecc = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.MD5 };

            return ecc.VerifyData(data, rgbSignature);
        }

        /// <summary>Sha256</summary>
        /// <param name="data"></param>
        /// <param name="priKey"></param>
        /// <returns></returns>
        public static Byte[] SignSha256(this Byte[] data, String priKey)
        {
            var key = CngKey.Import(priKey.ToBase64(), CngKeyBlobFormat.EccPrivateBlob);
            var ecc = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.Sha256 };

            return ecc.SignData(data);
        }

        /// <summary>Sha256</summary>
        /// <param name="data"></param>
        /// <param name="pukKey"></param>
        /// <param name="rgbSignature"></param>
        /// <returns></returns>
        public static Boolean VerifySha256(this Byte[] data, String pukKey, Byte[] rgbSignature)
        {
            var key = CngKey.Import(pukKey.ToBase64(), CngKeyBlobFormat.EccPublicBlob);
            var ecc = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.Sha256 };

            return ecc.VerifyData(data, rgbSignature);
        }

        /// <summary>Sha384</summary>
        /// <param name="data"></param>
        /// <param name="priKey"></param>
        /// <returns></returns>
        public static Byte[] SignSha384(this Byte[] data, String priKey)
        {
            var key = CngKey.Import(priKey.ToBase64(), CngKeyBlobFormat.EccPrivateBlob);
            var ecc = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.Sha384 };

            return ecc.SignData(data);
        }

        /// <summary>Sha384</summary>
        /// <param name="data"></param>
        /// <param name="pukKey"></param>
        /// <param name="rgbSignature"></param>
        /// <returns></returns>
        public static Boolean VerifySha384(this Byte[] data, String pukKey, Byte[] rgbSignature)
        {
            var key = CngKey.Import(pukKey.ToBase64(), CngKeyBlobFormat.EccPublicBlob);
            var ecc = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.Sha384 };

            return ecc.VerifyData(data, rgbSignature);
        }

        /// <summary>Sha512</summary>
        /// <param name="data"></param>
        /// <param name="priKey"></param>
        /// <returns></returns>
        public static Byte[] SignSha512(this Byte[] data, String priKey)
        {
            var key = CngKey.Import(priKey.ToBase64(), CngKeyBlobFormat.EccPrivateBlob);
            var ecc = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.Sha512 };

            return ecc.SignData(data);
        }

        /// <summary>Sha512</summary>
        /// <param name="data"></param>
        /// <param name="pukKey"></param>
        /// <param name="rgbSignature"></param>
        /// <returns></returns>
        public static Boolean VerifySha512(this Byte[] data, String pukKey, Byte[] rgbSignature)
        {
            var key = CngKey.Import(pukKey.ToBase64(), CngKeyBlobFormat.EccPublicBlob);
            var ecc = new ECDsaCng(key) { HashAlgorithm = CngAlgorithm.Sha512 };

            return ecc.VerifyData(data, rgbSignature);
        }
        #endregion

        #region PEM
        /// <summary>读取PEM文件到RSA参数</summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Byte[] ReadPem(String content)
        {
            if (String.IsNullOrEmpty(content)) throw new ArgumentNullException(nameof(content));

            // 公钥私钥分别处理
            content = content.Trim();
            if (content.StartsWithIgnoreCase("-----BEGIN RSA PRIVATE KEY-----", "-----BEGIN PRIVATE KEY-----"))
            {
                var content2 = content.TrimStart("-----BEGIN RSA PRIVATE KEY-----")
                     .TrimEnd("-----END RSA PRIVATE KEY-----")
                     .TrimStart("-----BEGIN PRIVATE KEY-----")
                     .TrimEnd("-----END PRIVATE KEY-----")
                     .Replace("\n", null).Replace("\r", null);

                var data = Convert.FromBase64String(content2);

                // PrivateKeyInfo: version + Algorithm(algorithm + parameters) + privateKey
                var asn = Asn1.Read(data);
                var keys = asn.Value as Asn1[];

                // Algorithm(algorithm + parameters)
                var oids = asn.GetOids();
                var algorithm = oids[0];
                var parameters = oids[1];

                if (algorithm.FriendlyName != "ECC") throw new InvalidDataException($"Invalid key {algorithm}");

                keys = Asn1.Read(keys[2].Value as Byte[]).Value as Asn1[];
                var k2 = Asn1.Read(keys[2].Value as Byte[]);

                // 参数数据
                //return new ECParameters
                //{
                //    Curve = ECCurve.CreateFromOid(parameters),
                //    D = keys[1].Value as Byte[],
                //};
                var buf = new Byte[32 * 3];
                buf.Write(0, keys[1].Value as Byte[]);
                buf.Write(32, k2.Value as Byte[], 1, -1);

                return buf;
            }
            else
            {
                content = content.Replace("-----BEGIN PUBLIC KEY-----", null)
                    .Replace("-----END PUBLIC KEY-----", null)
                    .Replace("\n", null).Replace("\r", null);

                var data = Convert.FromBase64String(content);

                var asn = Asn1.Read(data);
                var keys = asn.Value as Asn1[];

                // Algorithm(algorithm + parameters)
                var oids = asn.GetOids();
                var algorithm = oids[0];
                var parameters = oids[1];

                if (algorithm.FriendlyName != "ECC") throw new InvalidDataException($"Invalid key {algorithm}");

                // 参数数据
                //return new ECParameters
                //{
                //    D = keys[1].Value as Byte[],
                //};
                var buf = keys[1].Value as Byte[];

                return buf.ReadBytes(1);
            }
        }
        #endregion
    }
}