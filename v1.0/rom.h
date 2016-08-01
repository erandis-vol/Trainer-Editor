#ifndef ROM_H
#define ROM_H

#include <QByteArray>

class ROM
{
public:
    ROM();
    ~ROM();

    UInt8 readByte() const;
    UInt16 readHWord() const;
    UInt32 readWord() const;
    UInt32 readPointer() const;

    void writeByte(UInt8 value);
    void writeHWord(UInt16 value);
    void writeWord(UInt32 value);
    void writePointer(UInt32 offset);

private:
    QByteArray buffer;
    UInt8* ptr;
    UInt32 offset;
    UInt32 length;
};

#endif // ROM_H
