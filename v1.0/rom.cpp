#include "rom.h"
#include <QFile>

ROM::ROM()
    : path(QString::null),
      offset(0u),
{

}

ROM::~ROM()
{
    if (!buffer.isEmpty())
        buffer.clear();

    offset = 0u;
    path.clear();
}

bool ROM::load(const QString &filePath)
{
    QFile file(filePath);

    // read file contents into buffer
    buffer = file.readAll();
    if (buffer.isEmpty() || buffer.isNull()) {
        file.close();
        return false;
    }

    // we no longer need this open
    file.close();

    // TODO: we can load other ROM info now?

    // success, remember filePath
    path = filePath;
    return true;
}

bool ROM::save()
{
    // only save once a file is loaded
    if (path.isNull())
        return false;

    // try to open ROM file
    QFile file(path);
    if (!file.open(QIODevice::WriteOnly))
        return false;

    // write buffer to file
    file.seek(0LL);
    file.write(buffer);
    file.close();
    return true;
}

void ROM::close()
{
    if (!buffer.isEmpty())
        buffer.clear();
}

bool ROM::isOpen()
{
    return !path.isNull() && !buffer.isEmpty();
}

UInt8 ROM::readByte() const
{
    // todo: check against length
    return buffer.at(offest++);
}

UInt16 ROM::readHWord() const
{
    return (UInt16)(buffer.at(offset++) | (buffer.at(offset++) << 8));
}

UInt32 ROM::readWord() const
{
    return (UInt32)(buffer.at(offset++) | (buffer.at(offset++) << 8) | (buffer.at(offset++) << 16) | (buffer.at(offset++) << 24));
}

UInt32 ROM::readPointer() const
{
    // todo: ensure good pointer
    return readWord() & 0x1FFFFFF;
}

QByteArray ROM::readBytes(int count)
{
    // copy to a new bytearray
    // there are better ways to do this
    QByteArray b(count, 0);
    for (int i = 0; i < count; i++) {
        b[i] = readByte();
    }
    return b;
}

int ROM::readDecompressedSize()
{
    // decompressed size is stored as a 24-bit int
    return (int)(buffer.at(offset++) | (buffer.at(offset++) << 8) | (buffer.at(offset++) << 16));
}

QByteArray ROM::readCompressedBytes()
{
    QByteArray output();

    // check for signature byte
    if (readByte() != 0x10)
        return output;

    // note: could just read signature and size together, maniplate bits
    int length = readDecompressedSize();
    output.fill('\0', length);

    // perform decompression
    int size = 0;
    int pos = 0;
    UInt8 flags = 0;
    while (size < length)
    {
        if (pos == 0)
            flags = readByte();

        if ((flags & (0x80 >> pos)))
        {
            int block = (readByte() << 8) | readByte();
            int bytes = (block >> 12) + 3;
            int disp = size - (block & 0xFFF) - 1;

            while (bytes-- && size < length)
                buffer[size++] = buffer[disp++];
        }
        else
        {
            output[size++] = readByte();
        }

        pos = ++pos % 8;
    }

    return output;
}

void ROM::writeByte(UInt8 value)
{
    buffer[offset++] = value;
}

void ROM::writeHWord(UInt16 value)
{
    buffer[offset++] = (UInt8)value;
    buffer[offset++] = (UInt8)(value >> 8);
}

void ROM::writeWord(UInt32 value)
{
    buffer[offset++] = (UInt8)value;
    buffer[offset++] = (UInt8)(value >> 8);
    buffer[offset++] = (UInt8)(value >> 16);
    buffer[offset++] = (UInt8)(value >> 24);
}

void ROM::writePointer(UInt32 offset)
{
    if (offset == 0)
        writeWord(offset);
    else
        writeWord(offset | 0x8000000);
}

bool ROM::findFreeSpace(UInt32 startOffset, UInt32* offset)
{
    // todo
}

