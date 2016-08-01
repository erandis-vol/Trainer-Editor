#include "rom.h"

ROM::ROM()
    : ptr(NULL),
      offset(0u),
      length(0u)
{
}

ROM::~ROM()
{
    ptr = NULL;
    offset = 0u;
    length = 0u;
}

UInt8 ROM::readByte() const
{
    // todo: check against length
    return ptr[offest++];
}

UInt16 ROM::readHWord() const
{
    return (UInt16)(ptr[offset++] | (ptr[offset++] << 8));
}

UInt32 ROM::readWord() const
{
    return (UInt32)(ptr[offset++] | (ptr[offset++] << 8) | (ptr[offset++] << 16) | (ptr[offset++] << 24));
}

UInt32 ROM::readPointer() const
{
    // todo: ensure good pointer
    return readWord() & 0x1FFFFFF;
}

void ROM::writeByte(UInt8 value)
{
    ptr[offset++] = value;
}

void ROM::writeHWord(UInt16 value)
{
    ptr[offset++] = (UInt8)value;
    ptr[offset++] = (UInt8)(value >> 8);
}

void ROM::writeWord(UInt32 value)
{
    ptr[offset++] = (UInt8)value;
    ptr[offset++] = (UInt8)(value >> 8);
    ptr[offset++] = (UInt8)(value >> 16);
    ptr[offset++] = (UInt8)(value >> 24);
}

void ROM::writePointer(UInt32 offset)
{
    if (offset == 0)
        writeWord(offset);
    else
        writeWord(offset | 0x08000000);
}
