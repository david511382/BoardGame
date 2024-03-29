﻿using System.IO;
using System.Text;

namespace Util
{
    public static class DataConvert
    {
        #region byte
        public static string BytesToHex(this byte[] bs)
        {
            StringBuilder hex = new StringBuilder(bs.Length * 2);
            foreach (byte b in bs)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] HexToBytes(this string hexString)
        {
            //運算後的位元組長度:16進位數字字串長/2
            byte[] byteOUT = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                //每2位16進位數字轉換為一個10進位整數
                byteOUT[i / 2] = System.Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return byteOUT;
        }
        #endregion

        #region stream
        public static string StreamToString(this Stream stream)
        {
            string result = null;
            if (stream.CanRead)
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            return result;
        }
        #endregion
    }
}
