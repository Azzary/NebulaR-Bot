using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.IO
{
    public class BigEndianReader : Interfaces.IDataReader, IDisposable
    {
        public const int INT_SIZE = 32;
        public const int SHORT_SIZE = 16;
        public const int SHORT_MIN_VALUE = -0x8000;
        public const int SHORT_MAX_VALUE = 0x7FFF;
        public const int USHORT_MAX_VALUE = 0x10000;
        public const int CHUNCK_BIT_SIZE = 7;
        public static readonly int MAX_ENCODING_LENGHT = (int)Math.Ceiling((double)INT_SIZE / CHUNCK_BIT_SIZE);
        public const int MASK_10000000 = 0x80;
        public const int MASK_01111111 = 0x7F;

        #region Properties

        private BinaryReader m_reader;

        /// <summary>
        ///   Gets availiable bytes number in the buffer
        /// </summary>
        public long BytesAvailable
        {
            get { return m_reader.BaseStream.Length - m_reader.BaseStream.Position; }
        }

        public long Position
        {
            get
            {
                return m_reader.BaseStream.Position;
            }
        }


        public Stream BaseStream
        {
            get
            {
                return m_reader.BaseStream;
            }
        }

        public byte[] Data
        {
            get
            {
                long position = this.BaseStream.Position;
                byte[] buffer = new byte[this.BaseStream.Length];
                this.BaseStream.Position = 0L;
                this.BaseStream.Read(buffer, 0, (int)this.BaseStream.Length);
                this.BaseStream.Position = position;
                return buffer;
            }
        }

        #endregion

        #region Initialisation

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BigEndianReader" /> class.
        /// </summary>
        public BigEndianReader()
        {
            m_reader = new BinaryReader(new MemoryStream(), Encoding.UTF8);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BigEndianReader" /> class.
        /// </summary>
        /// <param name = "stream">The stream.</param>
        public BigEndianReader(Stream stream)
        {
            m_reader = new BinaryReader(stream, Encoding.UTF8);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BigEndianReader" /> class.
        /// </summary>
        /// <param name = "tab">Memory buffer.</param>
        public BigEndianReader(byte[] tab)
        {
            m_reader = new BinaryReader(new MemoryStream(tab), Encoding.UTF8);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Read bytes in big endian format
        /// </summary>
        /// <param name = "count"></param>
        /// <returns></returns>
        private byte[] ReadBigEndianBytes(int count)
        {
            var bytes = new byte[count];
            int i;
            for (i = count - 1; i >= 0; i--)
                bytes[i] = (byte)BaseStream.ReadByte();
            return bytes;
        }

        #endregion

        #region Public Method

        public int ReadVarInt()
        {
            int value = 0;
            int size = 0;
            while (size < INT_SIZE)
            {
                var b = ReadByte();
                bool bit = (b & MASK_10000000) == MASK_10000000;
                if (size > 0)
                    value |= ((b & MASK_01111111) << size);
                else
                    value |= (b & MASK_01111111);
                size += CHUNCK_BIT_SIZE;
                if (!bit)
                    return value;
            }

            throw new Exception("Overflow varint : too much data");
        }

        public uint ReadVarUhInt()
        {
            return unchecked((uint)ReadVarInt());
        }

        public short ReadVarShort()
        {
            int value = 0;
            int size = 0;
            while (size < SHORT_SIZE)
            {
                var b = ReadByte();
                bool bit = (b & MASK_10000000) == MASK_10000000;
                if (size > 0)
                    value |= ((b & MASK_01111111) << size);
                else
                    value |= (b & MASK_01111111);
                size += CHUNCK_BIT_SIZE;
                if (!bit)
                {
                    if (value > SHORT_MAX_VALUE)
                        value = value - USHORT_MAX_VALUE;

                    return (short)value;
                }
            }

            throw new Exception("Overflow varint : too much data");
        }

        public ushort ReadVarUhShort()
        {
            return unchecked((ushort)ReadVarShort());
        }

        public long ReadVarLong()
        {
            int low = 0;
            int high = 0;
            int size = 0;
            byte lastByte = 0;
            while (size < 28)
            {
                lastByte = m_reader.ReadByte();
                if ((lastByte & MASK_10000000) == MASK_10000000)
                {
                    low |= ((lastByte & MASK_01111111) << size);
                    size += 7;
                }
                else
                {
                    low |= lastByte << size;
                    return low;
                }
            }
            lastByte = m_reader.ReadByte();
            if ((lastByte & MASK_10000000) == MASK_10000000)
            {
                low |= (lastByte & MASK_01111111) << size;
                high = (lastByte & MASK_01111111) >> 4;
                size = 3;
                while (size < 32)
                {
                    lastByte = m_reader.ReadByte();
                    if ((lastByte & MASK_10000000) == MASK_10000000)
                        high |= (lastByte & MASK_01111111) << size;
                    else break;
                    size += 7;
                }
                high |= lastByte << size;
                return (low & 0xFFFFFFFF) | ((long)high << 32);
            }
            low |= lastByte << size;
            high = lastByte >> 4;
            return (low & 0xFFFFFFFF) | (long)high << 32;
        }

        public ulong ReadVarUhLong()
        {
            return unchecked((ulong)ReadVarLong());
        }

        /// <summary>
        ///   Read a Short from the Buffer
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadBigEndianBytes(2), 0);
        }

        /// <summary>
        ///   Read a int from the Buffer
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadBigEndianBytes(4), 0);
        }

        /// <summary>
        ///   Read a long from the Buffer
        /// </summary>
        /// <returns></returns>
        public Int64 ReadLong()
        {
            return BitConverter.ToInt64(ReadBigEndianBytes(8), 0);
        }

        /// <summary>
        ///   Read a Float from the Buffer
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBigEndianBytes(4), 0);
        }

        /// <summary>
        ///   Read a UShort from the Buffer
        /// </summary>
        /// <returns></returns>
        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(ReadBigEndianBytes(2), 0);
        }


        /// <summary>
        ///   Read a int from the Buffer
        /// </summary>
        /// <returns></returns>
        public UInt32 ReadUInt()
        {
            return BitConverter.ToUInt32(ReadBigEndianBytes(4), 0);
        }

        /// <summary>
        ///   Read a long from the Buffer
        /// </summary>
        /// <returns></returns>
        public UInt64 ReadULong()
        {
            return BitConverter.ToUInt64(ReadBigEndianBytes(8), 0);
        }

        /// <summary>
        ///   Read a byte from the Buffer
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            return m_reader.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return m_reader.ReadSByte();
        }

        /// <summary>
        ///   Returns N bytes from the buffer
        /// </summary>
        /// <param name = "n">Number of read bytes.</param>
        /// <returns></returns>
        public byte[] ReadBytes(int n)
        {
            return m_reader.ReadBytes(n);
        }

        /// <summary>
        /// Returns N bytes from the buffer
        /// </summary>
        /// <param name = "n">Number of read bytes.</param>
        /// <returns></returns>
        public BigEndianReader ReadBytesInNewBigEndianReader(int n)
        {
            return new BigEndianReader(m_reader.ReadBytes(n));
        }

        /// <summary>
        ///   Read a Boolean from the Buffer
        /// </summary>
        /// <returns></returns>
        public Boolean ReadBoolean()
        {
            return m_reader.ReadByte() == 1;
        }

        /// <summary>
        ///   Read a Char from the Buffer
        /// </summary>
        /// <returns></returns>
        public Char ReadChar()
        {
            return (char)ReadUShort();
        }

        /// <summary>
        ///   Read a Double from the Buffer
        /// </summary>
        /// <returns></returns>
        public Double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBigEndianBytes(8), 0);
        }

        /// <summary>
        ///   Read a Single from the Buffer
        /// </summary>
        /// <returns></returns>
        public Single ReadSingle()
        {
            return BitConverter.ToSingle(ReadBigEndianBytes(4), 0);
        }

        /// <summary>
        ///   Read a string from the Buffer
        /// </summary>
        /// <returns></returns>
        public string ReadUTF()
        {
            ushort length = ReadUShort();

            byte[] bytes = ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///   Read a string from the Buffer
        /// </summary>
        /// <returns></returns>
        public string ReadUTF7BitLength()
        {
            int length = ReadInt();

            byte[] bytes = ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///   Read a string from the Buffer
        /// </summary>
        /// <returns></returns>
        public string ReadUTFBytes(ushort len)
        {
            byte[] bytes = ReadBytes(len);

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///   Skip bytes
        /// </summary>
        /// <param name = "n"></param>
        public void SkipBytes(int n)
        {
            int i;
            for (i = 0; i < n; i++)
            {
                m_reader.ReadByte();
            }
        }


        public void Seek(int offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            m_reader.BaseStream.Seek(offset, seekOrigin);
        }

        /// <summary>
        ///   Add a bytes array to the end of the buffer
        /// </summary>
        public void Add(byte[] data, int offset, int count)
        {
            long pos = m_reader.BaseStream.Position;

            m_reader.BaseStream.Position = m_reader.BaseStream.Length;
            m_reader.BaseStream.Write(data, offset, count);
            m_reader.BaseStream.Position = pos;
        }

        #endregion

        #region Dispose

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_reader == null)
                return;

            m_reader.Dispose();
            m_reader = null;
        }

        #endregion
    }
}
