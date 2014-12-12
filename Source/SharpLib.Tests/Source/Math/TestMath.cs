using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace SharpLib.Tests
{
    [TestFixture]
    public class TestMath
    {
        /// <summary>
        /// Провверка MD5
        /// </summary>
        [TestCase("", Result = "d41d8cd98f00b204e9800998ecf8427e")]
        [TestCase("0", Result = "cfcd208495d565ef66e7dff9f98764da")]
        [TestCase("0123456789", Result = "781e5e245d69b566979b86e28d23f2c7")]
        public string TestMd5(string text)
        {
            return Md5.Hash(text);
        }

        /// <summary>
        /// Провверка MD5
        /// </summary>
        [TestCase("123", Sha.Algorithm.SHA1,  Result = "40bd001563085fc35165329ea1ff5c5ecbdbbeef")]
        [TestCase("123", Sha.Algorithm.SHA256, Result = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3")]
        [TestCase("123", Sha.Algorithm.SHA384, Result = "9a0a82f0c0cf31470d7affede3406cc9aa8410671520b727044eda15b4c25532a9b5cd8aaf9cec4919d76255b6bfb00f")]
        [TestCase("123", Sha.Algorithm.SHA512, Result = "3c9909afec25354d551dae21590bb26e38d53f2173b8d3dc3eee4c047e7ab1c1eb8b85103e3be7ba613b31bb5c9c36214dc9f14a42fd7a2fdb84856bca5c44c2")]
        public string TestSha(string text, Sha.Algorithm alg)
        {
            return Sha.Hash(text, alg);
        }
    }
}
