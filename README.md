# SECCS
![Build badge](https://github.com/pipe01/SECCS/workflows/.NET%20Core/badge.svg)

Speedy & Easy C# Class Serialization

## Purpose

SECCS is a tool for performant binary serialization of C# classes containing arbitrary data. It is quick and easy to use, and it doesn't require any attributes on the classes you want to serialize.

## Performance

The performance and output size of SECCS is heavily dependent on the buffer you use, which in this case is `System.IO.BinaryWriter` and `System.IO.BinaryReader` for serialization and deserialization respectively.

Serialization:

| Method         |       Mean |     Error |    StdDev |
| -------------- | ---------: | --------: | --------: |
| SECCS          |   746.3 ns |  4.735 ns |  4.429 ns |
| NewtonsoftJson | 6,577.1 ns | 14.282 ns | 11.926 ns |
| MessagePack    |   519.1 ns |  5.469 ns |  5.115 ns |

Deserialization:

| Method         |     Mean |     Error |   StdDev |
| -------------- | -------: | --------: | -------: |
| SECCS          | 11.91 us | 1.1663 us | 3.384 us |
| NewtonsoftJson | 56.95 us | 2.4383 us | 6.957 us |
| MessagePack    | 13.25 us | 0.9638 us | 2.781 us |

| Library         | Output size |
| --------------- | ----------- |
| SECCS           | 102 bytes   |
| Newtonsoft.Json | 149 bytes   |
| MessagePack     | 40 bytes    |
