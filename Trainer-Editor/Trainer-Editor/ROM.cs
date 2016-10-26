using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Lost
{
    using Encoding = TextTable.Encoding;

    /// <summary>
    /// Reads and writes primitive data types to/from a ROM.
    /// </summary>
    public class ROM : IDisposable
    {
        private const int BUFFER_SIZE = 8; // tune as needed, 8 is all we need for 64-bit integers
        private byte[] buffer = new byte[BUFFER_SIZE];

        private Stream stream;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ROM"/> class based on the specified file,
        /// with the default access and sharing options.
        /// </summary>
        /// <param name="filePath">The file.</param>
        /// <exception cref="FileNotFoundException">unable to open specified file.</exception>
        /// <exception cref="ArgumentException">file is larger than 0x1FFFFFF bytes.</exception>
        public ROM(string filePath) : this(filePath, FileAccess.ReadWrite, FileShare.ReadWrite)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ROM"/> class based on the specified file,
        /// with the specified read/write access and the the specified sharing option.
        /// </summary>
        /// <param name="filePath">The file.</param>
        /// <param name="access">A <see cref="FileAccess"/> value that specifies the actions that can be performed on the ROM.</param>
        /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the ROM.</param>
        /// <exception cref="FileNotFoundException">unable to open specified file.</exception>
        /// <exception cref="ArgumentException">file is larger than 0x1FFFFFF bytes.</exception>
        public ROM(string filePath, FileAccess access, FileShare share)
        {
            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch //(Exception e)
            {
                throw new FileNotFoundException($"Unable to open {filePath}!");
            }

            if (stream.Length > 0x1FFFFFF)
            {
                stream.Dispose();
                throw new ArgumentException("filePath", "File is too large for a ROM!");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ROM"/> class based on the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentException">stream is longer than 0x1FFFFFF bytes.</exception>
        public ROM(Stream stream)
        {
            if (stream.Length > 0x1FFFFFF)
                throw new ArgumentException("stream", "Stream is too large for a ROM!");

            //if (!stream.CanRead || !stream.CanWrite)
            //    throw new ArgumentException("stream", "Stream is not read-write!");

            this.stream = stream;
        }

        ~ROM()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="ROM"/> class.
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            stream.Dispose();
            buffer = null;
        }

        /// <summary>
        /// Forces data to be written to the underlying disk.
        /// </summary>
        /// <exception cref="IOException">idk</exception>
        public void Flush()
        {
            stream.Flush();
        }

        /// <summary>
        /// Sets the position of the stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to 0.</param>
        public void Seek(int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// Moves the position of the stream by the specified number of bytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to move; sign determines direction.</param>
        public void Skip(int bytes)
        {
            stream.Seek(bytes, SeekOrigin.Current);
        }

        // safely fill the buffer from the stream
        private void FillBuffer(int bytes)
        {
            if (bytes <= 0 || bytes > stream.Length)
                throw new Exception();

            if (bytes == 1)
            {
                int n = stream.ReadByte();
                if (n == -1)
                    throw new EndOfStreamException();

                buffer[0] = (byte)n;
            }
            else
            {
                int bytesRead = 0;

                do
                {
                    int n = stream.Read(buffer, bytesRead, bytes - bytesRead);
                    if (n == 0)
                        throw new EndOfStreamException();
                    bytesRead += n;
                } while (bytesRead < bytes);
            }
        }

        // safely read the entire stream into a buffer
        private byte[] ReadAllBytes()
        {
            // preserve original position
            long originalPosition = stream.Position;

            // seek start and read entire file
            byte[] buffer = new byte[stream.Length];
            int bytesRead = 0;

            stream.Position = 0;
            do
            {
                int n = stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
                if (n == 0)
                    throw new EndOfStreamException("Unable to read entire ROM!");
                bytesRead += n;
            } while (bytesRead < buffer.Length);

            // reset current position
            stream.Position = originalPosition;

            // done
            return buffer;
        }

        // safe write the entire stream from a buffer
        private void WriteAllBytes(byte[] buffer)
        {
            // preserve original position
            long originalPosition = stream.Position;

            // overwrite entire stream
            stream.Write(buffer, 0, buffer.Length);

            // reset position
            stream.Position = originalPosition;
        }

        #region Read

        /// <summary>
        /// Reads the next byte from the stream and advances the position by one byte.
        /// </summary>
        /// <exception cref="EndOfStreamException">attempted to read beyond the stream.</exception>
        /// <returns>The next byte read from the stream.</returns>
        public byte ReadByte()
        {
            int n = stream.ReadByte();
            if (n == -1)
                throw new EndOfStreamException();

            return (byte)n;
        }

        /// <summary>
        /// Reads the next byte from the stream.
        /// </summary>
        /// <exception cref="EndOfStreamException">attempted to read beyond the stream.</exception>
        /// <returns>The next byte from the stream.</returns>
        public byte PeekByte()
        {
            int n = stream.ReadByte();
            if (n == -1)
                throw new EndOfStreamException();

            stream.Position -= 1;
            return (byte)n;
        }

        /// <summary>
        /// Reads a signed byte from the stream and advances the position by one byte.
        /// </summary>
        /// <returns>The next signed byte read from the stream.</returns>
        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the stream and advances the position by two bytes.
        /// </summary>
        /// <returns>A 2-byte unsigned integer read from the stream.</returns>
        public ushort ReadUInt16()
        {
            FillBuffer(2);
            return (ushort)(buffer[0] | (buffer[1] << 8));
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the stream.
        /// </summary>
        /// <returns></returns>
        /*public ushort PeekUInt16()
        {
            FillBuffer(2);
            stream.Position -= 2;
            return (ushort)(buffer[0] | (buffer[1] << 8));
        }*/

        /// <summary>
        /// Reads a 4-byte signed integer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            FillBuffer(4);
            return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            FillBuffer(4);
            return (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the stream and advances the position by eight bytes.
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            FillBuffer(8);
            return (ulong)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24) |
                (buffer[4] << 32) | (buffer[5] << 40) | (buffer[6] << 48) | (buffer[7] << 56));
        }

        /// <summary>
        /// Reads the specified number of bytes from the stream into a byte array and advances the position by that number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="EndOfStreamException"></exception> 
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Argument cannot be negative!");

            if (count == 0)
                return new byte[0];

            byte[] result = new byte[count];
            int bytesRead = 0;
            do
            {
                int n = stream.Read(result, bytesRead, count - bytesRead);
                if (n == 0)
                    throw new EndOfStreamException();
                bytesRead += n;
            } while (bytesRead < count);

            return result;
        }

        /// <summary>
        /// Reads a UTF-8 encoded string of the specified length from the stream and advances the position by that number of bytes.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ReadString(int length)
        {
            //return Encoding.UTF8.GetString(ReadBytes(length));
            var buffer = ReadBytes(length);
            var sb = new StringBuilder();

            foreach (var b in buffer)
            {
                if (b == 0)
                    break;

                sb.Append((char)b);
            }

            return sb.ToString();
        }

        // Read a UTF16 encoded string
        /*public string ReadUnicodeString(int length)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                var b = ReadUInt16();
                if (b == 0)
                    break;

                sb.Append((char)b);
            }

            return sb.ToString();
        }*/

        /// <summary>
        /// Reads and validates a 4-byte ROM pointer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns>The offset pointed to if valid; -1 otherwise.</returns>
        public int ReadPointer()
        {
            // read value
            var ptr = ReadInt32();

            // return on blank pointer
            if (ptr == 0)
                return 0;

            // a pointer must be between 0x0 and 0x1FFFFFF to be valid on the GBA
            // ROM pointer format is OFFSET | 0x8000000, so 0x8000000 <= POINTER <= 0x9FFFFFF
            if (ptr < 0x8000000 || ptr > 0x9FFFFFF)
                return -1;

            // easy way to extract
            return ptr & 0x1FFFFFF;
        }

        /// <summary>
        /// Reads and validates a 4-byte ROM pointer from the stream.
        /// </summary>
        /// <returns>The offset pointed to if valid; -1 otherwise.</returns>
        /*public int PeekPointer()
        {
            var ptr = ReadPointer();
            pos -= 4;
            return ptr;
        }*/

        /// <summary>
        /// Reads an FF-terminated string using the given <see cref="Encoding"/> and advances the position.
        /// </summary>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns></returns>
        public string ReadText(Encoding encoding)
        {
            // read string until FF
            List<byte> buffer = new List<byte>();
            byte temp = 0;
            do
            {
                buffer.Add((temp = ReadByte()));
            } while (temp != 0xFF);

            // convert to string
            return TextTable.GetString(buffer.ToArray(), encoding);
        }

        /// <summary>
        /// Reads a string of the given length using the given <see cref="Encoding"/> and advances the position by that many bytes.
        /// </summary>
        /// <param name="length">The length of the string.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns></returns>
        public string ReadText(int length, Encoding encoding)
        {
            return TextTable.GetString(ReadBytes(length), encoding);
        }

        public string[] ReadTextTable(int stringLength, int tableSize, Encoding encoding)
        {
            var table = new string[tableSize];
            for (int i = 0; i < tableSize; i++)
                table[i] = ReadText(stringLength, encoding);

            return table;
        }

        /// <summary>
        /// If possible, reads LZ77 compressed bytes from the stream into a byte array and advances the position.
        /// </summary>
        /// <returns>The decompressed bytes.</returns>
        public byte[] ReadLZ77CompressedBytes()
        {
            // check if actually compressed
            if (!PeekLZ77Compressed())
                return new byte[0];

            // skip L7ZZ identifier
            Skip(1);

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

        /// <summary>
        /// If possible, reads LZ77 compressed data and returns the number of bytes it occupied in the ROM.
        /// </summary>
        /// <returns></returns>
        public int ReadLZ77CompressedSize()
        {
            if (!PeekLZ77Compressed())
                return -1;

            // read decompressed size
            var length = ReadInt24();
            int count = 4;

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

        /// <summary>
        /// Returns whether the following data could be L7ZZ compressed.
        /// </summary>
        /// <returns><c>true</c> if it could be; <c>false</c> otherwise.</returns>
        public bool PeekLZ77Compressed()
        {
            return PeekByte() == 0x10;
        }

        /// <summary>
        /// Reads a 3-byte unsigned integer from the stream and advances the position by three bytes.
        /// </summary>
        /// <returns></returns>
        int ReadInt24()
        {
            FillBuffer(3);
            return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16);
        }

        /// <summary>
        /// Reads a 2-byte BGR555 color from the stream and advances the position by two bytes.
        /// </summary>
        /// <returns></returns>
        public Color ReadColor()
        {
            var c = ReadUInt16();
            return Color.FromArgb((c & 0x1F) << 3, (c >> 5 & 0x1F) << 3, (c >> 10 & 0x1F) << 3);
        }

        /// <summary>
        /// Reads the specified number of BGR555 colors from the stream and advances the position by twice that many bytes.
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        public Color[] ReadPalette(int colors = 16)
        {
            var pal = new Color[colors];
            for (int i = 0; i < colors; i++)
                pal[i] = ReadColor();
            return pal;
        }

        /// <summary>
        /// lol reads a compressed palette or something.
        /// </summary>
        /// <returns></returns>
        public Color[] ReadLZ77CompressedPalette()
        {
            var buffer = ReadLZ77CompressedBytes();
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

        /// <summary>
        /// Writes a byte to the stream and advances the position by one byte.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value to write.</param>
        public void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }

        /// <summary>
        /// Writes a signed byte to the stream and advances the position by one byte.
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> value to write.</param>
        public void WriteSByte(sbyte value)
        {
            stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes a 2-byte unsigned integer to the stream and advances the position by two bytes.
        /// </summary>
        /// <param name="value">The <see cref="ushort"/> value to write.</param>
        public void WriteUInt16(ushort value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            stream.Write(buffer, 0, 2);
        }

        /// <summary>
        /// Writes a 4-byte signed integer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value to write.</param>
        public void WriteInt32(int value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            stream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Writes a 4-byte unsigned integer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value to write.</param>
        public void WriteUInt32(uint value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            stream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Writes an 8-byte unsigned integer to the stream and advances the position by eight bytes.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> value to write.</param>
        public void WriteUInt64(ulong value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[3] = (byte)(value >> 32);
            buffer[1] = (byte)(value >> 40);
            buffer[2] = (byte)(value >> 48);
            buffer[3] = (byte)(value >> 56);
            stream.Write(buffer, 0, 8);
        }

        /// <summary>
        /// Writes a <see cref="byte"/> array to the stream and advances the position by the length of the array.
        /// </summary>
        /// <param name="bytes">The <see cref="byte"/> array to write.</param>
        /// <exception cref="ArgumentNullException">bytes is null.</exception>
        public void WriteBytes(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a <see cref="byte"/> to the stream the specified number of times and advances the position by that many bytes.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value to write.</param>
        /// <param name="count">The number of times to write the value.</param>
        public void WriteBytes(byte value, int count)
        {
            for (int i = 0; i < count; i++)
                stream.WriteByte(value);
        }

        /// <summary>
        /// Writes a 4-byte pointer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="offset">The offset of the pointer to write.</param>
        /// <exception cref="ArgumentOutOfRangeException">offset is less than <c>0</c> or greater than <c>0x1FFFFFF</c>.</exception>
        public void WritePointer(int offset)
        {
            if (offset < 0 || offset > 0x1FFFFFF)
                throw new ArgumentOutOfRangeException(
                    $"Offset 0x{offset:X7} too large or too small for a ROM pointer (0 <= offset <= 0x1FFFFFF)!");

            if (offset > 0)
                WriteInt32(offset | 0x8000000);
            else
                WriteInt32(0);
        }

        /// <summary>
        /// Writes a UTF-8 encoded string to the stream and advances the position.
        /// </summary>
        /// <param name="str">The <see cref="string"/> value to write.</param>
        public void WriteString(string str)
        {
            // utf8 encoded string
            WriteBytes(System.Text.Encoding.UTF8.GetBytes(str));
        }

        public void WriteText(string str, Encoding encoding)
        {
            WriteBytes(TextTable.GetBytes(str, encoding));
        }

        public void WriteText(string str, int length, Encoding encoding)
        {
            // convert string
            byte[] buffer = TextTable.GetBytes(str, encoding);

            // ensure proper length
            if (buffer.Length != length)
                Array.Resize(ref buffer, length);
            buffer[length - 1] = 0xFF;

            WriteBytes(buffer);
        }

        public void WriteTextTable(string[] table, int entryLength, Encoding encoding)
        {
            foreach (var str in table)
                WriteText(str, entryLength, encoding);
        }

        #endregion

        #region Search

        /// <summary>
        /// Finds free space of the given length in the <see cref="ROM"/>.
        /// </summary>
        /// <param name="length">The number of bytes needed.</param>
        /// <param name="freespace">The freespace byte to search for. Default is <c>0xFF</c>.</param>
        /// <param name="startOffset">The offset to start searching from. Default is <c>0</c>.</param>
        /// <param name="alignment">The alignment of the offset to search for. Default is <c>1</c>, recommended is <c>4</c>.</param>
        /// <exception cref="EndOfStreamException">if the entire ROM was not able to be read.</exception>
        /// <returns>The offset of the freespace if found; -1 otherwise.</returns>
        public int FindFreeSpace(int length, byte freespace = 0xFF, int startOffset = 0, int alignment = 1)
        {
            // load search buffer
            byte[] buffer = ReadAllBytes();

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

        /// <summary>
        /// Finds and changes all pointers from one offset to another.
        /// </summary>
        /// <param name="originalOffset">The offset to search for.</param>
        /// <param name="newOffset">The offset to replace.</param>
        public void ReplaceAllPointers(int originalOffset, int newOffset)
        {
            // load search/replace buffer buffer
            byte[] buffer = ReadAllBytes();

            // replace any pointers from original to new
            var originalPtr = BitConverter.GetBytes(originalOffset | 0x8000000);
            var newPtr = BitConverter.GetBytes(newOffset | 0x8000000);

            for (int i = 0; i < buffer.Length - 4; i++)
            {
                var match = true;
                for (int j = 0; j < 4; j++)
                {
                    if (buffer[i + j] != originalPtr[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    //Console.WriteLine("pointer at 0x{0:X7}", i);

                    for (int j = 0; j < 4; j++)
                        buffer[i++] = newPtr[j];
                }
            }

            // write changes
            WriteAllBytes(buffer);
        }

        #endregion

        /// <summary>
        /// Returns whether the given integer is a valid ROM bank offset. NOTE: Pokémon ROMs only use the first bank.
        /// </summary>
        /// <param name="offset">The integer to validate.</param>
        /// <returns><c>true</c> if the offset is valid; <c>false</c> otherwise.</returns>
        public static bool IsValidOffset(int offset)
        {
            return offset >= 0 && offset <= 0x1FFFFFF;// && offset <= buffer.Length;
        }

        #region Properties

        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        public int Length
        {
            get { return (int)stream.Length; }
        }

        /// <summary>
        /// Gets or sets the position of the stream.
        /// </summary>
        public int Position
        {
            get { return (int)stream.Position; }
            set { stream.Position = value; }
        }

        /// <summary>
        /// Gets whether the stream has read to the end.
        /// </summary>
        public bool EndOfStream
        {
            get { return stream.Position >= stream.Length; }
        }

        // so we aren't continuously re-reading these properties
        private string name, code, maker;

        /// <summary>
        /// Gets the 12 character name of the ROM specified by the header at offset 0xA0.
        /// </summary>
        public string Name
        {
            get
            {
                if (name != null)
                    return name;

                var i = stream.Position;
                stream.Position = 0xA0;
                name = ReadString(12);
                stream.Position = i;

                return name;
            }
        }

        /// <summary>
        /// Gets the 4 character code of the ROM specified by the header at offset 0xAC.
        /// </summary>
        public string Code
        {
            get
            {
                if (code != null)
                    return code;

                var i = stream.Position;
                stream.Position = 0xAC;
                code = ReadString(4);
                stream.Position = i;

                return code;
            }
        }

        /// <summary>
        /// Gets the 2 character maker code of the ROM specified by the header at offset 0xB0.
        /// </summary>
        public string Maker
        {
            get
            {
                if (maker != null)
                    return maker;

                var i = stream.Position;
                stream.Position = 0xB0;
                maker = ReadString(2);
                stream.Position = i;

                return maker;
            }
        }

        #endregion
    }
}

