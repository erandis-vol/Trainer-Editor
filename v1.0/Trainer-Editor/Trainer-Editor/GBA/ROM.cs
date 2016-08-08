using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace HTE.GBA
{
    public class ROM
    {
        private byte[] buffer;
        private int pos = 0;

        public ROM(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Could not find " + filePath + "!");
            }

            // Read contents of file into buffer
            // We have to do it this way so that we can interface with A-Map
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }
        }

        ~ROM()
        {
            buffer = null;
        }

        public void Save(string filePath)
        {
            // Again, write using a filestream
            // Using File.WriteAllBytes does not work with greedy programs like A-Map
            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        public void Seek(int offset)
        {
            if (offset < 0 || offset > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }

            pos = offset;
        }

        public void Skip(int bytes)
        {
            pos += bytes;

            if (pos < 0 || pos > buffer.Length)
            {
                throw new IndexOutOfRangeException();
            }
        }

        #region Read

        public byte ReadByte()
        {
            return buffer[pos++];
        }

        public byte PeekByte()
        {
            return buffer[pos];
        }

        public ushort ReadUInt16()
        {
            return (ushort)(buffer[pos++] | (buffer[pos++] << 8));
        }

        public int ReadInt32()
        {
            return buffer[pos++] | (buffer[pos++] << 8) | (buffer[pos++] << 16) | (buffer[pos++] << 24);
        }

        public uint ReadUInt32()
        {
            return (uint)(buffer[pos++] | (buffer[pos++] << 8) | (buffer[pos++] << 16) | (buffer[pos++] << 24));
        }

        public ulong ReadUInt64()
        {
            return (ulong)(buffer[pos++] | (buffer[pos++] << 8) | (buffer[pos++] << 16) | (buffer[pos++] << 24) |
                (buffer[pos++] << 32) | (buffer[pos++] << 40) | (buffer[pos++] << 48) | (buffer[pos++] << 56));
        }

        public byte[] ReadBytes(int count)
        {
            var b = new byte[count];
            for (int i = 0; i < count; i++)
            {
                b[i] = buffer[pos++];
            }
            return b;
        }

        // Read a UTF8 encoded string
        public string ReadString(int length)
        {
            return Encoding.UTF8.GetString(ReadBytes(length));
        }

        public int ReadPointer()
        {
            // Read pointer data
            var ptr = ReadInt32();

            // A pointer must be between 0x0 and 0x1FFFFFF to be valid on the GBA
            // ROM pointer format is OFFSET | 0x8000000, so 0x8000000 <= POINTER <= 0x9FFFFFF
            if (ptr < 0x8000000 || ptr > 0x9FFFFFF) throw new Exception(string.Format("Bad pointer at 0x{0:X6}", pos - 4));

            // Easy way to extract
            return ptr & 0x1FFFFFF;
        }

        public byte[] ReadCompressedBytes()
        {
            if (ReadByte() != 0x10) return new byte[0];

            // read decompressed size
            var length = ReadInt24();
            var buffer = new byte[length];

            // decompress the data
            int size = 0, pos = 0, flags = 0;
            while (size < length)
            {
                if (pos == 0) flags = ReadByte();

                if ((flags & (0x80 >> pos)) == 0)
                {
                    // read value to buffer
                    buffer[size++] = ReadByte();
                }
                else
                {
                    // copy block from buffer
                    int block = (ReadByte() << 8) | ReadByte();

                    int bytes = (block >> 12) + 3;
                    int disp = size - (block & 0xFFF) - 1;

                    while (bytes-- > 0 && size < length)
                    {
                        buffer[size++] = buffer[disp++];
                    }
                }

                pos = ++pos % 8;
            }

            return buffer;
        }

        public int ReadCompressedSize()
        {
            if (ReadByte() != 0x10) return -1;

            // read decompressed size
            var length = ReadInt24();
            int count = 0;

            // decompress the data
            int size = 0, pos = 0, flags = 0;
            while (size < length)
            {
                if (pos == 0)
                {
                    flags = ReadByte();
                    count++;
                }

                if ((flags & (0x80 >> pos)) == 0)
                {
                    // read value to buffer
                    Skip(1); size++; count++;
                }
                else
                {
                    // copy block from buffer
                    int block = (ReadByte() << 8) | ReadByte();
                    count += 2;

                    int bytes = (block >> 12) + 3;
                    while (bytes-- > 0 && size < length)
                    {
                        size++;
                    }
                }

                pos = ++pos % 8;
            }

            return count;
        }

        public bool PeekCompressed()
        {
            return PeekByte() == 0x10;
        }

        int ReadInt24()
        {
            return buffer[pos++] | (buffer[pos++] << 8) | (buffer[pos++] << 16);
        }

        public Color ReadColor()
        {
            var c = ReadUInt16();
            return Color.FromArgb((c & 0x1F) << 3, (c >> 5 & 0x1F) << 3, (c >> 10 & 0x1F) << 3);
        }

        public Color[] ReadPalette(int colors = 16)
        {
            var pal = new Color[colors];
            for (int i = 0; i < colors; i++) pal[i] = ReadColor();
            return pal;
        }

        public Color[] ReadCompressedPalette()
        {
            var buffer = ReadCompressedBytes();
            var pal = new Color[buffer.Length / 2];
            for (int i = 0; i < pal.Length; i++)
            {
                var color = (buffer[i * 2 + 1] << 8) | buffer[i * 2];
                pal[i] = Color.FromArgb((color & 0x1F) << 3, (color >> 5 & 0x1F) << 3, (color >> 10 & 0x1F) << 3);
            }
            return pal;
        }

        #endregion

        #region Write

        public void WriteByte(byte value)
        {
            buffer[pos++] = value;
        }

        public void WriteUInt16(ushort value)
        {
            buffer[pos++] = (byte)value;
            buffer[pos++] = (byte)(value >> 8);
        }

        public void WriteInt32(int value)
        {
            for (int i = 0; i < 4; i++)
            {
                buffer[pos++] = (byte)(value >> (i * 8));
            }
        }

        public void WriteUInt32(uint value)
        {
            for (int i = 0; i < 4; i++)
            {
                buffer[pos++] = (byte)(value >> (i * 8));
            }
        }

        public void WriteUInt64(ulong value)
        {
            for (int i = 0; i < 8; i++)
            {
                buffer[pos++] = (byte)(value >> (i * 8));
            }
        }

        public void WriteBytes(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++) buffer[pos++] = bytes[i];
        }

        // Write a byte value count number of times
        public void WriteBytes(byte value, int count)
        {
            for (int i = 0; i < count; i++) buffer[pos++] = value;
        }

        public void WritePointer(int offset)
        {
            if (offset > 0x1FFFFFF)
            {
                throw new Exception(string.Format("Offset 0x{0:X6} too large for a ROM pointer (0 <= offset <= 0x1FFFFFF)!"));
            }

            WriteInt32(offset | 0x8000000);
        }

        // Write a UTF8 encoded string
        public void WriteString(string str)
        {
            WriteBytes(Encoding.UTF8.GetBytes(str));
        }

        /*public void WriteCompressedBytes(byte[] buffer)
        {
            // Adapted from NSE 2.X by link12552

            // ensure proper data size
            if (buffer.Length > 0xFFFFFF)
                throw new Exception("Cannot compressed buffer longer than 0xFFFFFF bytes!");

            var output = new List<byte>();
            var preOutput = new List<byte>();

            // compressed data header
            output.Add(0x10);                       // signature byte
            output.Add((byte)buffer.Length);        // i24, decompressed length
            output.Add((byte)(buffer.Length >> 8));
            output.Add((byte)(buffer.Length >> 16));

            // provide starting data for compression
            preOutput.Add(buffer[0]);
            preOutput.Add(buffer[1]);

            int actualPos = 2; byte shortPos = 2;
            byte flags = 0;

            while (actualPos < buffer.Length)
            {
                if (shortPos >= 8)
                {
                    output.Add(flags);
                    output.AddRange(preOutput);

                    flags = 0;
                    preOutput.Clear();
                    shortPos = 0;
                }
                else
                {
                    
                }
            }
        }*/

        #endregion

        #region Search

        public int FindFreeSpace(int length, byte freespace = 0xFF, int startOffset = 0, int alignment = 1)
        {
            // The simpest freespace finder I could think of
            if (alignment > 1 && startOffset % alignment != 0)
            {
                startOffset += alignment - (startOffset % alignment);
            }

            for (int i = startOffset; i < buffer.Length - length; i += alignment)
            {
                bool match = true;
                for (int j = 0; j < length; j++)
                {
                    if (buffer[i + j] != freespace)
                    {
                        match = false;
                        break;
                    }
                }

                if (match) return i;
            }
            return -1;
        }

        #endregion

        #region Properties

        public int Length
        {
            get { return buffer.Length; }
        }

        public int Position
        {
            get { return pos; }

            // User should use Seek!
            set
            {
                pos = value;

                if (pos < 0 || pos > buffer.Length)
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public bool EndOfStream
        {
            get
            {
                return pos >= buffer.Length;
            }
        }

        // ROM specific properties

        public string Name
        {
            get
            {
                return Encoding.UTF8.GetString(buffer, 0xA0, 12);
            }
        }

        public string Code
        {
            get
            {
                return Encoding.UTF8.GetString(buffer, 0xAC, 4);
            }
        }

        public string Maker
        {
            get
            {
                return Encoding.UTF8.GetString(buffer, 0xB0, 2);
            }
        }

        #endregion

    }
}
