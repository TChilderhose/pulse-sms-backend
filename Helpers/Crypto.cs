using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Pulse.Helpers
{
	public static class Crypto
	{
		public static string GenerateSalt()
		{
			byte[] salt = new byte[128 / 8];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(salt);
			}
			return Convert.ToBase64String(salt);
		}

		public static string GetPasswordHash(string password, string salt)
		{
			return Convert.ToBase64String(KeyDerivation.Pbkdf2(
											   password: password,
											   salt: Encoding.UTF8.GetBytes(salt),
											   prf: KeyDerivationPrf.HMACSHA1,
											   iterationCount: 10000,
											   numBytesRequested: 256 / 8));
		}
	}
}
