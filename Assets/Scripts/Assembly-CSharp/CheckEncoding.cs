using System.Security.Cryptography;
using System.Text;

public static class CheckEncoding
{
	public static byte[] ComputeSHA1(byte[] data)
	{
		using (SHA1 sHA = SHA1.Create())
		{
			return sHA.ComputeHash(data);
		}
	}

	public static byte[] ComputeSHA1(string data, Encoding encoding)
	{
		if (encoding == null)
		{
			encoding = Encoding.UTF8;
		}
		return ComputeSHA1(encoding.GetBytes(data));
	}
}
