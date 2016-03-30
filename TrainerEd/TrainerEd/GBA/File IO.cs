using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace HTE.GBA
{
    public class BadPointerException : Exception
    {
        public BadPointerException(uint data, long offset)
            : base("Bad pointer encountered at 0x" + offset.ToString("X") + "!\n0x" + data.ToString("X8") + " is not a pointer!")
        { }

        public BadPointerException(uint data)
            : base("0x" + data.ToString("X8") + " cannot be converted to a pointer!")
        { }
    }

    public class GBABinaryReader : BinaryReader
    {
        // constructor
        public GBABinaryReader(Stream input)
            : base(input)
        { }

        public GBABinaryReader(ROM input)
            : base(File.Open(input.File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        { }

        public uint ReadPointer()
        {
            // read
            uint data = base.ReadUInt32();
            // safety
            if (data < 0x08000000) throw new BadPointerException(data, base.BaseStream.Position - 4);

            // return
            return data - 0x08000000;
        }

        #region LZ77

        public byte[] ReadLZ77Bytes()
        {
            if (base.ReadByte() != 0x10)
                throw new Exception("The data at 0x" + base.BaseStream.Position.ToString("X") + " is not LZ77 compressed!");

            int length = ReadLZ77DecompressedLength();
            byte[] outData = new byte[length];

            int currSize = 0;
            int n, cdest;

            while (currSize < length)
            {
                int flags = base.ReadByte();
                for (int i = 0; i < 8; i++)
                {
                    bool flag = ((flags & (0x80 >> i)) > 0);
                    if (flag)
                    {
                        int disp = 0;
                        byte b = base.ReadByte();
                        n = b >> 4;
                        disp = (b & 0xF) << 8;

                        disp |= base.ReadByte();
                        n += 3;

                        cdest = currSize;
                        if (disp > currSize)
                            throw new Exception("Unable to reverse more than already written!");

                        for (int j = 0; j < n; j++)
                            outData[currSize++] = outData[cdest - disp - 1 + j];

                        if (currSize > length) break;
                    }
                    else
                    {
                        byte b = base.ReadByte();

                        try
                        {
                            outData[currSize++] = b;
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            if (b == 0) break;
                        }

                        if (currSize > length) break;
                    }
                }
            }

            return outData;
        }

        public int ReadLZ77CompressedLength()
        {
            int outlength = 0;

            if (base.ReadByte() != 0x10)
                throw new Exception("The data at 0x" + base.BaseStream.Position.ToString("X") + " is not LZ77 compressed!");

            int length = ReadLZ77DecompressedLength();

            int currSize = 0;
            int n, cdest;

            while (currSize < length)
            {
                int flags = base.ReadByte();
                for (int i = 0; i < 8; i++)
                {
                    bool flag = ((flags & (0x80 >> i)) > 0);
                    if (flag)
                    {
                        int disp = 0;
                        byte b = base.ReadByte();
                        outlength++;
                        n = b >> 4;
                        disp = (b & 0xF) << 8;

                        disp |= base.ReadByte();
                        outlength++;
                        n += 3;

                        cdest = currSize;
                        if (disp > currSize)
                            throw new Exception("Unable to reverse more than already written!");

                        if (currSize > length) break;
                    }
                    else
                    {
                        byte b = base.ReadByte();
                        outlength++;

                        if (currSize > length) break;
                    }
                }
            }

            return outlength;
        }

        public int ReadLZ77DecompressedLength()
        {
            int length = 0;
            for (int i = 0; i < 3; i++)
                length = length | (ReadByte() << (i * 8));

            if (length == 0) length = base.ReadInt32();

            return length;
        }

        #endregion

        #region Palettes

        public Color[] ReadPalette()
        {
            Color[] colors = new Color[16];
            for (int i = 0; i < 16; i++)
            {
                ushort color = base.ReadUInt16();
                colors[i] = Color.FromArgb((color & 0x1F) * 8, (color >> 5 & 0x1F) * 8, (color >> 10 & 0x1F) * 8);
            }
            return colors;            
        }

        public Color[] ReadLZ77Palette()
        {
            byte[] data = ReadLZ77Bytes();
            if (data.Length < 32) throw new Exception("Compressed palette was not 16 colors!");

            Color[] colors = new Color[16];
            for (int i = 0; i < 16; i++)
            {
                ushort color = BitConverter.ToUInt16(data, i * 2); // (data[i * 2 + 1] << 8) | data[i * 2];
                colors[i] = Color.FromArgb((color & 0x1F) * 8, (color >> 5 & 0x1F) * 8, (color >> 10 & 0x1F) * 8);
            }
            return colors;
        }

        #endregion
    }

    public class GBABinaryWriter : BinaryWriter
    {
        // constructor
        public GBABinaryWriter(Stream output)
            : base(output)
        { }

        public GBABinaryWriter(ROM output)
            : base(File.Open(output.File, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        { }

        public void WritePointer(uint offset)
        {
            // safety
            if (offset > 0x1FFFFF) throw new BadPointerException(offset);

            // write
            base.Write(offset + 0x8000000);
        }
    }
}
