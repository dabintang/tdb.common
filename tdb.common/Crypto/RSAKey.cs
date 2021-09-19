using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.common.Crypto
{
	/// <summary>
	/// RAS key
	/// </summary>
	public class RSAKey
	{
		/// <summary>
		/// 公钥
		/// </summary>
		public string PublicKey { get; set; }

		/// <summary>
		/// 私钥
		/// </summary>
		public string PrivateKey { get; set; }
	}
}
