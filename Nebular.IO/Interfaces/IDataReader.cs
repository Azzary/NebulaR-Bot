using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.IO.Interfaces
{
    public interface IDataReader : IDisposable
    {
        long Position { get; }

        long BytesAvailable { get; }

        short ReadShort();

        byte[] Data { get; }

        int ReadInt();

        long ReadLong();

        ushort ReadUShort();

        uint ReadUInt();

        ulong ReadULong();

        byte ReadByte();

        sbyte ReadSByte();

        byte[] ReadBytes(int n);

        bool ReadBoolean();

        char ReadChar();

        double ReadDouble();

        float ReadFloat();

        string ReadUTF();

        string ReadUTFBytes(ushort len);

        ushort ReadVarUhShort();

        long ReadVarLong();

        uint ReadVarUhInt();

        int ReadVarInt();

        short ReadVarShort();

        ulong ReadVarUhLong();

        void Seek(int offset, SeekOrigin seekOrigin);

        void SkipBytes(int n);
    }
}
