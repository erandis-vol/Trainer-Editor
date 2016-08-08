#ifndef ROM_H
#define ROM_H

#include <QByteArray>

// inspiration:
// https://github.com/pokedude9/QBoy

class ROM
{
public:
    ROM();
    ~ROM();

    bool load(const QString &filePath);
    bool save();
    void close();

    bool isOpen(); // path != null

    UInt8 readByte() const;
    UInt16 readHWord() const;
    UInt32 readWord() const;
    UInt32 readPointer() const;
    QByteArray readBytes(int count);
    QByteArray readCompressedBytes();

    void writeByte(UInt8 value);
    void writeHWord(UInt16 value);
    void writeWord(UInt32 value);
    void writePointer(UInt32 offset);

    bool findFreeSpace(UInt32 startOffset, UInt32* offset);

private:
    int readDecompressedSize() const;

    QByteArray buffer;
    mutable UInt32 offset;
    QString path;
};

#endif // ROM_H
