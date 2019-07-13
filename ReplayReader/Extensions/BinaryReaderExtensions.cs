using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FNReplayReader
{
    public static class BinaryReaderExtensions
    {
        public static string ReadFString(this BinaryReader reader)
        {
            var length = reader.ReadInt32();

            if (length < 0) //isUnicode
            {
                length = -length;
                var data = reader.ReadBytes(length * 2);
                var value = Encoding.Unicode.GetString(data);
                return value.Trim(new[] { ' ', '\0' });
            }
            else
            {
                var data = reader.ReadBytes(length);
                var value = Encoding.Default.GetString(data);
                return value.Trim(new[] { ' ', '\0' });
            }            
        }

        public static Guid ReadGuid(this BinaryReader reader)
        {
            var guid = reader.ReadBytes(16);
            return new Guid(guid);
        }

        public static IEnumerable<string> ReadFStringArray(this BinaryReader reader)
        {
            var length = reader.ReadUInt32();
            for (int i = 0; i < length; i++)
            {
                yield return reader.ReadFString();
            }
        }

        public static IEnumerable<Tuple<string, uint>> ReadSpecialArray(this BinaryReader reader)
        {
            var length = reader.ReadUInt32();
            for (int i = 0; i < length; i++)
            {
                yield return Tuple.Create<string, uint>(reader.ReadFString(), reader.ReadUInt32());
            }
        }

        public static void SkipBytes(this BinaryReader reader, uint byteCount)
        {
            reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
        }

        private static int SizeOf<T>()
        {
            return Marshal.SizeOf(default(T));
        }
    }
}
