using System;
using System.Data;

namespace StandPoint.XProtocol
{
    public class XProtocolEncryption
    {
        private static string Key { get; } = "2e985f930";

        public static byte[] Encrypt(byte[] data)
        {
            throw new NotImplementedException();
            //return RijndaelHandler.Encrypt(data, Key);
        }

        public static byte[] Decrypt(byte[] data)
        {
            throw new NotImplementedException();
            //return RijndaelHandler.Decrypt(data, Key);
        }
    }
}
