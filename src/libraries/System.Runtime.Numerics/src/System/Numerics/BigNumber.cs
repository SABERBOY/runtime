// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// The BigNumber class implements methods for formatting and parsing
// big numeric values. To format and parse numeric values, applications should
// use the Format and Parse methods provided by the numeric
// classes (BigInteger). Those
// Format and Parse methods share a common implementation
// provided by this class, and are thus documented in detail here.
//
// Formatting
//
// The Format methods provided by the numeric classes are all of the
// form
//
//  public static String Format(XXX value, String format);
//  public static String Format(XXX value, String format, NumberFormatInfo info);
//
// where XXX is the name of the particular numeric class. The methods convert
// the numeric value to a string using the format string given by the
// format parameter. If the format parameter is null or
// an empty string, the number is formatted as if the string "G" (general
// format) was specified. The info parameter specifies the
// NumberFormatInfo instance to use when formatting the number. If the
// info parameter is null or omitted, the numeric formatting information
// is obtained from the current culture. The NumberFormatInfo supplies
// such information as the characters to use for decimal and thousand
// separators, and the spelling and placement of currency symbols in monetary
// values.
//
// Format strings fall into two categories: Standard format strings and
// user-defined format strings. A format string consisting of a single
// alphabetic character (A-Z or a-z), optionally followed by a sequence of
// digits (0-9), is a standard format string. All other format strings are
// used-defined format strings.
//
// A standard format string takes the form Axx, where A is an
// alphabetic character called the format specifier and xx is a
// sequence of digits called the precision specifier. The format
// specifier controls the type of formatting applied to the number and the
// precision specifier controls the number of significant digits or decimal
// places of the formatting operation. The following table describes the
// supported standard formats.
//
// C c - Currency format. The number is
// converted to a string that represents a currency amount. The conversion is
// controlled by the currency format information of the NumberFormatInfo
// used to format the number. The precision specifier indicates the desired
// number of decimal places. If the precision specifier is omitted, the default
// currency precision given by the NumberFormatInfo is used.
//
// D d - Decimal format. This format is
// supported for integral types only. The number is converted to a string of
// decimal digits, prefixed by a minus sign if the number is negative. The
// precision specifier indicates the minimum number of digits desired in the
// resulting string. If required, the number will be left-padded with zeros to
// produce the number of digits given by the precision specifier.
//
// E e Engineering (scientific) format.
// The number is converted to a string of the form
// "-d.ddd...E+ddd" or "-d.ddd...e+ddd", where each
// 'd' indicates a digit (0-9). The string starts with a minus sign if the
// number is negative, and one digit always precedes the decimal point. The
// precision specifier indicates the desired number of digits after the decimal
// point. If the precision specifier is omitted, a default of 6 digits after
// the decimal point is used. The format specifier indicates whether to prefix
// the exponent with an 'E' or an 'e'. The exponent is always consists of a
// plus or minus sign and three digits.
//
// F f Fixed point format. The number is
// converted to a string of the form "-ddd.ddd....", where each
// 'd' indicates a digit (0-9). The string starts with a minus sign if the
// number is negative. The precision specifier indicates the desired number of
// decimal places. If the precision specifier is omitted, the default numeric
// precision given by the NumberFormatInfo is used.
//
// G g - General format. The number is
// converted to the shortest possible decimal representation using fixed point
// or scientific format. The precision specifier determines the number of
// significant digits in the resulting string. If the precision specifier is
// omitted, the number of significant digits is determined by the type of the
// number being converted (10 for int, 19 for long, 7 for
// float, 15 for double, 19 for Currency, and 29 for
// Decimal). Trailing zeros after the decimal point are removed, and the
// resulting string contains a decimal point only if required. The resulting
// string uses fixed point format if the exponent of the number is less than
// the number of significant digits and greater than or equal to -4. Otherwise,
// the resulting string uses scientific format, and the case of the format
// specifier controls whether the exponent is prefixed with an 'E' or an
// 'e'.
//
// N n Number format. The number is
// converted to a string of the form "-d,ddd,ddd.ddd....", where
// each 'd' indicates a digit (0-9). The string starts with a minus sign if the
// number is negative. Thousand separators are inserted between each group of
// three digits to the left of the decimal point. The precision specifier
// indicates the desired number of decimal places. If the precision specifier
// is omitted, the default numeric precision given by the
// NumberFormatInfo is used.
//
// X x - Hexadecimal format. This format is
// supported for integral types only. The number is converted to a string of
// hexadecimal digits. The format specifier indicates whether to use upper or
// lower case characters for the hexadecimal digits above 9 ('X' for 'ABCDEF',
// and 'x' for 'abcdef'). The precision specifier indicates the minimum number
// of digits desired in the resulting string. If required, the number will be
// left-padded with zeros to produce the number of digits given by the
// precision specifier.
//
// Some examples of standard format strings and their results are shown in the
// table below. (The examples all assume a default NumberFormatInfo.)
//
// Value        Format  Result
// 12345.6789   C       $12,345.68
// -12345.6789  C       ($12,345.68)
// 12345        D       12345
// 12345        D8      00012345
// 12345.6789   E       1.234568E+004
// 12345.6789   E10     1.2345678900E+004
// 12345.6789   e4      1.2346e+004
// 12345.6789   F       12345.68
// 12345.6789   F0      12346
// 12345.6789   F6      12345.678900
// 12345.6789   G       12345.6789
// 12345.6789   G7      12345.68
// 123456789    G7      1.234568E8
// 12345.6789   N       12,345.68
// 123456789    N4      123,456,789.0000
// 0x2c45e      x       2c45e
// 0x2c45e      X       2C45E
// 0x2c45e      X8      0002C45E
//
// Format strings that do not start with an alphabetic character, or that start
// with an alphabetic character followed by a non-digit, are called
// user-defined format strings. The following table describes the formatting
// characters that are supported in user defined format strings.
//
//
// 0 - Digit placeholder. If the value being
// formatted has a digit in the position where the '0' appears in the format
// string, then that digit is copied to the output string. Otherwise, a '0' is
// stored in that position in the output string. The position of the leftmost
// '0' before the decimal point and the rightmost '0' after the decimal point
// determines the range of digits that are always present in the output
// string.
//
// # - Digit placeholder. If the value being
// formatted has a digit in the position where the '#' appears in the format
// string, then that digit is copied to the output string. Otherwise, nothing
// is stored in that position in the output string.
//
// . - Decimal point. The first '.' character
// in the format string determines the location of the decimal separator in the
// formatted value; any additional '.' characters are ignored. The actual
// character used as a the decimal separator in the output string is given by
// the NumberFormatInfo used to format the number.
//
// , - Thousand separator and number scaling.
// The ',' character serves two purposes. First, if the format string contains
// a ',' character between two digit placeholders (0 or #) and to the left of
// the decimal point if one is present, then the output will have thousand
// separators inserted between each group of three digits to the left of the
// decimal separator. The actual character used as a the decimal separator in
// the output string is given by the NumberFormatInfo used to format the
// number. Second, if the format string contains one or more ',' characters
// immediately to the left of the decimal point, or after the last digit
// placeholder if there is no decimal point, then the number will be divided by
// 1000 times the number of ',' characters before it is formatted. For example,
// the format string '0,,' will represent 100 million as just 100. Use of the
// ',' character to indicate scaling does not also cause the formatted number
// to have thousand separators. Thus, to scale a number by 1 million and insert
// thousand separators you would use the format string '#,##0,,'.
//
// % - Percentage placeholder. The presence of
// a '%' character in the format string causes the number to be multiplied by
// 100 before it is formatted. The '%' character itself is inserted in the
// output string where it appears in the format string.
//
// E+ E- e+ e-   - Scientific notation.
// If any of the strings 'E+', 'E-', 'e+', or 'e-' are present in the format
// string and are immediately followed by at least one '0' character, then the
// number is formatted using scientific notation with an 'E' or 'e' inserted
// between the number and the exponent. The number of '0' characters following
// the scientific notation indicator determines the minimum number of digits to
// output for the exponent. The 'E+' and 'e+' formats indicate that a sign
// character (plus or minus) should always precede the exponent. The 'E-' and
// 'e-' formats indicate that a sign character should only precede negative
// exponents.
//
// \ - Literal character. A backslash character
// causes the next character in the format string to be copied to the output
// string as-is. The backslash itself isn't copied, so to place a backslash
// character in the output string, use two backslashes (\\) in the format
// string.
//
// 'ABC' "ABC" - Literal string. Characters
// enclosed in single or double quotation marks are copied to the output string
// as-is and do not affect formatting.
//
// ; - Section separator. The ';' character is
// used to separate sections for positive, negative, and zero numbers in the
// format string.
//
// Other - All other characters are copied to
// the output string in the position they appear.
//
// For fixed point formats (formats not containing an 'E+', 'E-', 'e+', or
// 'e-'), the number is rounded to as many decimal places as there are digit
// placeholders to the right of the decimal point. If the format string does
// not contain a decimal point, the number is rounded to the nearest
// integer. If the number has more digits than there are digit placeholders to
// the left of the decimal point, the extra digits are copied to the output
// string immediately before the first digit placeholder.
//
// For scientific formats, the number is rounded to as many significant digits
// as there are digit placeholders in the format string.
//
// To allow for different formatting of positive, negative, and zero values, a
// user-defined format string may contain up to three sections separated by
// semicolons. The results of having one, two, or three sections in the format
// string are described in the table below.
//
// Sections:
//
// One - The format string applies to all values.
//
// Two - The first section applies to positive values
// and zeros, and the second section applies to negative values. If the number
// to be formatted is negative, but becomes zero after rounding according to
// the format in the second section, then the resulting zero is formatted
// according to the first section.
//
// Three - The first section applies to positive
// values, the second section applies to negative values, and the third section
// applies to zeros. The second section may be left empty (by having no
// characters between the semicolons), in which case the first section applies
// to all non-zero values. If the number to be formatted is non-zero, but
// becomes zero after rounding according to the format in the first or second
// section, then the resulting zero is formatted according to the third
// section.
//
// For both standard and user-defined formatting operations on values of type
// float and double, if the value being formatted is a NaN (Not
// a Number) or a positive or negative infinity, then regardless of the format
// string, the resulting string is given by the NaNSymbol,
// PositiveInfinitySymbol, or NegativeInfinitySymbol property of
// the NumberFormatInfo used to format the number.
//
// Parsing
//
// The Parse methods provided by the numeric classes are all of the form
//
//  public static XXX Parse(String s);
//  public static XXX Parse(String s, int style);
//  public static XXX Parse(String s, int style, NumberFormatInfo info);
//
// where XXX is the name of the particular numeric class. The methods convert a
// string to a numeric value. The optional style parameter specifies the
// permitted style of the numeric string. It must be a combination of bit flags
// from the NumberStyles enumeration. The optional info parameter
// specifies the NumberFormatInfo instance to use when parsing the
// string. If the info parameter is null or omitted, the numeric
// formatting information is obtained from the current culture.
//
// Numeric strings produced by the Format methods using the Currency,
// Decimal, Engineering, Fixed point, General, or Number standard formats
// (the C, D, E, F, G, and N format specifiers) are guaranteed to be parseable
// by the Parse methods if the NumberStyles.Any style is
// specified. Note, however, that the Parse methods do not accept
// NaNs or Infinities.
//

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Numerics
{
    internal static class BigNumber
    {
        private const NumberStyles InvalidNumberStyles = ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
                                                           | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign
                                                           | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint
                                                           | NumberStyles.AllowThousands | NumberStyles.AllowExponent
                                                           | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowHexSpecifier);

        private static readonly uint[] s_uint32PowersOfTen = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };

        private struct BigNumberBuffer
        {
            public StringBuilder digits;
            public int precision;
            public int scale;
            public bool sign;  // negative sign exists

            public static BigNumberBuffer Create()
            {
                BigNumberBuffer number = default;
                number.digits = new StringBuilder();
                return number;
            }
        }


        internal static bool TryValidateParseStyleInteger(NumberStyles style, [NotNullWhen(false)] out ArgumentException? e)
        {
            // Check for undefined flags
            if ((style & InvalidNumberStyles) != 0)
            {
                e = new ArgumentException(SR.Argument_InvalidNumberStyles, nameof(style));
                return false;
            }
            if ((style & NumberStyles.AllowHexSpecifier) != 0)
            { // Check for hex number
                if ((style & ~NumberStyles.HexNumber) != 0)
                {
                    e = new ArgumentException(SR.Argument_InvalidHexStyle, nameof(style));
                    return false;
                }
            }
            e = null;
            return true;
        }

        internal static bool TryParseBigInteger(string? value, NumberStyles style, NumberFormatInfo info, out BigInteger result)
        {
            if (value == null)
            {
                result = default;
                return false;
            }

            return TryParseBigInteger(value.AsSpan(), style, info, out result);
        }

        internal static bool TryParseBigInteger(ReadOnlySpan<char> value, NumberStyles style, NumberFormatInfo info, out BigInteger result)
        {
            if (!TryValidateParseStyleInteger(style, out ArgumentException? e))
            {
                throw e; // TryParse still throws ArgumentException on invalid NumberStyles
            }

            BigNumberBuffer bigNumber = BigNumberBuffer.Create();
            if (!FormatProvider.TryStringToBigInteger(value, style, info, bigNumber.digits, out bigNumber.precision, out bigNumber.scale, out bigNumber.sign))
            {
                result = default;
                return false;
            }

            if ((style & NumberStyles.AllowHexSpecifier) != 0)
            {
                return HexNumberToBigInteger(ref bigNumber, out result);
            }
            else
            {
                return NumberToBigInteger(ref bigNumber, out result);
            }
        }

        internal static BigInteger ParseBigInteger(string value, NumberStyles style, NumberFormatInfo info)
        {
            ArgumentNullException.ThrowIfNull(value);

            return ParseBigInteger(value.AsSpan(), style, info);
        }

        internal static BigInteger ParseBigInteger(ReadOnlySpan<char> value, NumberStyles style, NumberFormatInfo info)
        {
            if (!TryValidateParseStyleInteger(style, out ArgumentException? e))
            {
                throw e;
            }
            if (!TryParseBigInteger(value, style, info, out BigInteger result))
            {
                throw new FormatException(SR.Overflow_ParseBigInteger);
            }
            return result;
        }

        private static bool HexNumberToBigInteger(ref BigNumberBuffer number, out BigInteger result)
        {
            if (number.digits == null || number.digits.Length == 0)
            {
                result = default;
                return false;
            }

            const int DigitsPerBlock = 8;

            int totalDigitCount = number.digits.Length - 1;   // Ignore trailing '\0'
            int blockCount, partialDigitCount;

            blockCount = Math.DivRem(totalDigitCount, DigitsPerBlock, out int remainder);
            if (remainder == 0)
            {
                partialDigitCount = 0;
            }
            else
            {
                blockCount += 1;
                partialDigitCount = DigitsPerBlock - remainder;
            }

            bool isNegative = HexConverter.FromChar(number.digits[0]) >= 8;
            uint partialValue = (isNegative && partialDigitCount > 0) ? 0xFFFFFFFFu : 0;

            uint[]? arrayFromPool = null;

            Span<uint> bitsBuffer = ((uint)blockCount <= BigIntegerCalculator.StackAllocThreshold
                ? stackalloc uint[BigIntegerCalculator.StackAllocThreshold]
                : arrayFromPool = ArrayPool<uint>.Shared.Rent(blockCount)).Slice(0, blockCount);

            int bitsBufferPos = blockCount - 1;

            try
            {
                foreach (ReadOnlyMemory<char> digitsChunkMem in number.digits.GetChunks())
                {
                    ReadOnlySpan<char> chunkDigits = digitsChunkMem.Span;
                    for (int i = 0; i < chunkDigits.Length; i++)
                    {
                        char digitChar = chunkDigits[i];
                        if (digitChar == '\0')
                        {
                            break;
                        }

                        int hexValue = HexConverter.FromChar(digitChar);
                        Debug.Assert(hexValue != 0xFF);

                        partialValue = (partialValue << 4) | (uint)hexValue;
                        partialDigitCount++;

                        if (partialDigitCount == DigitsPerBlock)
                        {
                            bitsBuffer[bitsBufferPos] = partialValue;
                            bitsBufferPos--;
                            partialValue = 0;
                            partialDigitCount = 0;
                        }
                    }
                }

                Debug.Assert(partialDigitCount == 0 && bitsBufferPos == -1);

                // BigInteger requires leading zero blocks to be truncated.
                bitsBuffer = bitsBuffer.TrimEnd(0u);

                int sign;
                uint[]? bits;

                if (bitsBuffer.IsEmpty)
                {
                    sign = 0;
                    bits = null;
                }
                else if (bitsBuffer.Length == 1)
                {
                    sign = (int)bitsBuffer[0];
                    bits = null;

                    if ((!isNegative && sign < 0) || sign == int.MinValue)
                    {
                        bits = new[] { (uint)sign };
                        sign = isNegative ? -1 : 1;
                    }
                }
                else
                {
                    sign = isNegative ? -1 : 1;
                    bits = bitsBuffer.ToArray();

                    if (isNegative)
                    {
                        NumericsHelpers.DangerousMakeTwosComplement(bits);
                    }
                }

                result = new BigInteger(sign, bits);
                return true;
            }
            finally
            {
                if (arrayFromPool != null)
                {
                    ArrayPool<uint>.Shared.Return(arrayFromPool);
                }
            }
        }

        //
        // This threshold is for choosing the algorithm to use based on the number of digits.
        //
        // Let N be the number of digits. If N is less than or equal to the bound, use a naive
        // algorithm with a running time of O(N^2). And if it is greater than the threshold, use
        // a divide-and-conquer algorithm with a running time of O(NlogN).
        //
        private static int s_naiveThreshold = 20000;
        private static bool NumberToBigInteger(ref BigNumberBuffer number, out BigInteger result)
        {
            int currentBufferSize = 0;

            int totalDigitCount = 0;
            int numberScale = number.scale;

            const int MaxPartialDigits = 9;
            const uint TenPowMaxPartial = 1000000000;

            int[]? arrayFromPoolForResultBuffer = null;
            if (numberScale < 0)
            {
                result = default;
                return false;
            }

            try
            {
                if (number.digits.Length <= s_naiveThreshold)
                {
                    return Naive(ref number, out result);
                }
                else
                {
                    return DivideAndConquer(ref number, out result);
                }
            }
            finally
            {
                if (arrayFromPoolForResultBuffer != null)
                {
                    ArrayPool<int>.Shared.Return(arrayFromPoolForResultBuffer);
                }
            }

            bool Naive(ref BigNumberBuffer number, out BigInteger result)
            {
                Span<uint> stackBuffer = stackalloc uint[BigIntegerCalculator.StackAllocThreshold];
                Span<uint> currentBuffer = stackBuffer;
                uint partialValue = 0;
                int partialDigitCount = 0;

                foreach (ReadOnlyMemory<char> digitsChunk in number.digits.GetChunks())
                {
                    if (!ProcessChunk(digitsChunk.Span, ref currentBuffer))
                    {
                        result = default;
                        return false;
                    }
                }

                if (partialDigitCount > 0)
                {
                    MultiplyAdd(ref currentBuffer, s_uint32PowersOfTen[partialDigitCount], partialValue);
                }

                result = NumberBufferToBigInteger(currentBuffer, number.sign);
                return true;

                bool ProcessChunk(ReadOnlySpan<char> chunkDigits, ref Span<uint> currentBuffer)
                {
                    int remainingIntDigitCount = Math.Max(numberScale - totalDigitCount, 0);
                    ReadOnlySpan<char> intDigitsSpan = chunkDigits.Slice(0, Math.Min(remainingIntDigitCount, chunkDigits.Length));

                    bool endReached = false;

                    // Storing these captured variables in locals for faster access in the loop.
                    uint _partialValue = partialValue;
                    int _partialDigitCount = partialDigitCount;
                    int _totalDigitCount = totalDigitCount;

                    for (int i = 0; i < intDigitsSpan.Length; i++)
                    {
                        char digitChar = chunkDigits[i];
                        if (digitChar == '\0')
                        {
                            endReached = true;
                            break;
                        }

                        _partialValue = _partialValue * 10 + (uint)(digitChar - '0');
                        _partialDigitCount++;
                        _totalDigitCount++;

                        // Update the buffer when enough partial digits have been accumulated.
                        if (_partialDigitCount == MaxPartialDigits)
                        {
                            MultiplyAdd(ref currentBuffer, TenPowMaxPartial, _partialValue);
                            _partialValue = 0;
                            _partialDigitCount = 0;
                        }
                    }

                    // Check for nonzero digits after the decimal point.
                    if (!endReached)
                    {
                        ReadOnlySpan<char> fracDigitsSpan = chunkDigits.Slice(intDigitsSpan.Length);
                        for (int i = 0; i < fracDigitsSpan.Length; i++)
                        {
                            char digitChar = fracDigitsSpan[i];
                            if (digitChar == '\0')
                            {
                                break;
                            }
                            if (digitChar != '0')
                            {
                                return false;
                            }
                        }
                    }

                    partialValue = _partialValue;
                    partialDigitCount = _partialDigitCount;
                    totalDigitCount = _totalDigitCount;

                    return true;
                }
            }

            bool DivideAndConquer(ref BigNumberBuffer number, out BigInteger result)
            {
                Span<uint> currentBuffer;
                int[]? arrayFromPoolForMultiplier = null;
                try
                {
                    totalDigitCount = Math.Min(number.digits.Length - 1, numberScale);
                    int bufferSize = (totalDigitCount + MaxPartialDigits - 1) / MaxPartialDigits;

                    Span<uint> buffer = new uint[bufferSize];
                    arrayFromPoolForResultBuffer = ArrayPool<int>.Shared.Rent(bufferSize);
                    Span<uint> newBuffer = MemoryMarshal.Cast<int, uint>(arrayFromPoolForResultBuffer).Slice(0, bufferSize);
                    newBuffer.Clear();

                    // Separate every MaxPartialDigits digits and store them in the buffer.
                    // Buffers are treated as little-endian. That means, the array { 234567890, 1 }
                    // represents the number 1234567890.
                    int bufferIndex = bufferSize - 1;
                    uint currentBlock = 0;
                    int shiftUntil = (totalDigitCount - 1) % MaxPartialDigits;
                    int remainingIntDigitCount = totalDigitCount;
                    foreach (ReadOnlyMemory<char> digitsChunk in number.digits.GetChunks())
                    {
                        ReadOnlySpan<char> digitsChunkSpan = digitsChunk.Span;
                        ReadOnlySpan<char> intDigitsSpan = digitsChunkSpan.Slice(0, Math.Min(remainingIntDigitCount, digitsChunkSpan.Length));

                        for (int i = 0; i < intDigitsSpan.Length; i++)
                        {
                            char digitChar = intDigitsSpan[i];
                            Debug.Assert(char.IsDigit(digitChar));
                            currentBlock *= 10;
                            currentBlock += unchecked((uint)(digitChar - '0'));
                            if (shiftUntil == 0)
                            {
                                buffer[bufferIndex] = currentBlock;
                                currentBlock = 0;
                                bufferIndex--;
                                shiftUntil = MaxPartialDigits;
                            }
                            shiftUntil--;
                        }
                        remainingIntDigitCount -= intDigitsSpan.Length;
                        Debug.Assert(0 <= remainingIntDigitCount);

                        ReadOnlySpan<char> fracDigitsSpan = digitsChunkSpan.Slice(intDigitsSpan.Length);
                        for (int i = 0; i < fracDigitsSpan.Length; i++)
                        {
                            char digitChar = fracDigitsSpan[i];
                            if (digitChar == '\0')
                            {
                                break;
                            }
                            if (digitChar != '0')
                            {
                                result = default;
                                return false;
                            }
                        }
                    }
                    Debug.Assert(currentBlock == 0);
                    Debug.Assert(bufferIndex == -1);

                    int blockSize = 1;
                    arrayFromPoolForMultiplier = ArrayPool<int>.Shared.Rent(blockSize);
                    Span<uint> multiplier = MemoryMarshal.Cast<int, uint>(arrayFromPoolForMultiplier).Slice(0, blockSize);
                    multiplier[0] = TenPowMaxPartial;

                    // This loop is executed ceil(log_2(bufferSize)) times.
                    while (true)
                    {
                        // merge each block pairs.
                        // When buffer represents:
                        // |     A     |     B     |     C     |     D     |
                        // Make newBuffer like:
                        // |  A + B * multiplier   |  C + D * multiplier   |
                        for (int i = 0; i < bufferSize; i += blockSize * 2)
                        {
                            Span<uint> curBuffer = buffer.Slice(i);
                            Span<uint> curNewBuffer = newBuffer.Slice(i);

                            int len = Math.Min(bufferSize - i, blockSize * 2);
                            int lowerLen = Math.Min(len, blockSize);
                            int upperLen = len - lowerLen;
                            if (upperLen != 0)
                            {
                                Debug.Assert(blockSize == lowerLen);
                                Debug.Assert(blockSize == multiplier.Length);
                                Debug.Assert(multiplier.Length == lowerLen);
                                BigIntegerCalculator.Multiply(multiplier, curBuffer.Slice(blockSize, upperLen), curNewBuffer.Slice(0, len));
                            }

                            long carry = 0;
                            int j = 0;
                            for (; j < lowerLen; j++)
                            {
                                long digit = (curBuffer[j] + carry) + curNewBuffer[j];
                                curNewBuffer[j] = unchecked((uint)digit);
                                carry = digit >> 32;
                            }
                            if (carry != 0)
                            {
                                while (true)
                                {
                                    curNewBuffer[j]++;
                                    if (curNewBuffer[j] != 0)
                                    {
                                        break;
                                    }
                                    j++;
                                }
                            }
                        }

                        Span<uint> tmp = buffer;
                        buffer = newBuffer;
                        newBuffer = tmp;
                        blockSize *= 2;

                        if (bufferSize <= blockSize)
                        {
                            break;
                        }
                        newBuffer.Clear();
                        int[]? arrayToReturn = arrayFromPoolForMultiplier;

                        arrayFromPoolForMultiplier = ArrayPool<int>.Shared.Rent(blockSize);
                        Span<uint> newMultiplier = MemoryMarshal.Cast<int, uint>(arrayFromPoolForMultiplier).Slice(0, blockSize);
                        newMultiplier.Clear();
                        BigIntegerCalculator.Square(multiplier, newMultiplier);
                        multiplier = newMultiplier;
                        if (arrayToReturn is not null)
                        {
                            ArrayPool<int>.Shared.Return(arrayToReturn);
                        }
                    }

                    // shrink buffer to the currently used portion.
                    // First, calculate the rough size of the buffer from the ratio that the number
                    // of digits follows. Then, shrink the size until there is no more space left.
                    // The Ratio is calculated as: log_{2^32}(10^9)
                    const double digitRatio = 0.934292276687070661;
                    currentBufferSize = Math.Min((int)(bufferSize * digitRatio) + 1, bufferSize);
                    Debug.Assert(buffer.Length == currentBufferSize || buffer[currentBufferSize] == 0);
                    while (0 < currentBufferSize && buffer[currentBufferSize - 1] == 0)
                    {
                        currentBufferSize--;
                    }
                    currentBuffer = buffer.Slice(0, currentBufferSize);
                    result = NumberBufferToBigInteger(currentBuffer, number.sign);
                }
                finally
                {
                    if (arrayFromPoolForMultiplier != null)
                    {
                        ArrayPool<int>.Shared.Return(arrayFromPoolForMultiplier);
                    }
                }
                return true;
            }

            BigInteger NumberBufferToBigInteger(Span<uint> currentBuffer, bool signa)
            {
                int trailingZeroCount = numberScale - totalDigitCount;

                while (trailingZeroCount >= MaxPartialDigits)
                {
                    MultiplyAdd(ref currentBuffer, TenPowMaxPartial, 0);
                    trailingZeroCount -= MaxPartialDigits;
                }

                if (trailingZeroCount > 0)
                {
                    MultiplyAdd(ref currentBuffer, s_uint32PowersOfTen[trailingZeroCount], 0);
                }

                int sign;
                uint[]? bits;

                if (currentBufferSize == 0)
                {
                    sign = 0;
                    bits = null;
                }
                else if (currentBufferSize == 1 && currentBuffer[0] <= int.MaxValue)
                {
                    sign = (int)(signa ? -currentBuffer[0] : currentBuffer[0]);
                    bits = null;
                }
                else
                {
                    sign = signa ? -1 : 1;
                    bits = currentBuffer.Slice(0, currentBufferSize).ToArray();
                }

                return new BigInteger(sign, bits);
            }

            // This function should only be used for result buffer.
            void MultiplyAdd(ref Span<uint> currentBuffer, uint multiplier, uint addValue)
            {
                Span<uint> curBits = currentBuffer.Slice(0, currentBufferSize);
                uint carry = addValue;

                for (int i = 0; i < curBits.Length; i++)
                {
                    ulong p = (ulong)multiplier * curBits[i] + carry;
                    curBits[i] = (uint)p;
                    carry = (uint)(p >> 32);
                }

                if (carry == 0)
                {
                    return;
                }

                if (currentBufferSize == currentBuffer.Length)
                {
                    int[]? arrayToReturn = arrayFromPoolForResultBuffer;

                    arrayFromPoolForResultBuffer = ArrayPool<int>.Shared.Rent(checked(currentBufferSize * 2));
                    Span<uint> newBuffer = MemoryMarshal.Cast<int, uint>(arrayFromPoolForResultBuffer);
                    currentBuffer.CopyTo(newBuffer);
                    currentBuffer = newBuffer;

                    if (arrayToReturn != null)
                    {
                        ArrayPool<int>.Shared.Return(arrayToReturn);
                    }
                }

                currentBuffer[currentBufferSize] = carry;
                currentBufferSize++;
            }
        }

        internal static char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
        {
            digits = -1;
            if (format.Length == 0)
            {
                return 'R';
            }

            int i = 0;
            char ch = format[i];
            if (char.IsAsciiLetter(ch))
            {
                // The digits value must be >= 0 && <= 999_999_999,
                // but it can begin with any number of 0s, and thus we may need to check more than 9
                // digits.  Further, for compat, we need to stop when we hit a null char.
                i++;
                int n = 0;
                while ((uint)i < (uint)format.Length && char.IsAsciiDigit(format[i]))
                {
                    // Check if we are about to overflow past our limit of 9 digits
                    if (n >= 100_000_000)
                    {
                        throw new FormatException(SR.Argument_BadFormatSpecifier);
                    }
                    n = ((n * 10) + format[i++] - '0');
                }

                // If we're at the end of the digits rather than having stopped because we hit something
                // other than a digit or overflowed, return the standard format info.
                if (i >= format.Length || format[i] == '\0')
                {
                    digits = n;
                    return ch;
                }
            }
            return (char)0; // Custom format
        }

        private static string? FormatBigIntegerToHex(bool targetSpan, BigInteger value, char format, int digits, NumberFormatInfo info, Span<char> destination, out int charsWritten, out bool spanSuccess)
        {
            Debug.Assert(format == 'x' || format == 'X');

            // Get the bytes that make up the BigInteger.
            byte[]? arrayToReturnToPool = null;
            Span<byte> bits = stackalloc byte[64]; // arbitrary threshold
            if (!value.TryWriteOrCountBytes(bits, out int bytesWrittenOrNeeded))
            {
                bits = arrayToReturnToPool = ArrayPool<byte>.Shared.Rent(bytesWrittenOrNeeded);
                bool success = value.TryWriteBytes(bits, out bytesWrittenOrNeeded);
                Debug.Assert(success);
            }
            bits = bits.Slice(0, bytesWrittenOrNeeded);

            var sb = new ValueStringBuilder(stackalloc char[128]); // each byte is typically two chars

            int cur = bits.Length - 1;
            if (cur > -1)
            {
                // [FF..F8] drop the high F as the two's complement negative number remains clear
                // [F7..08] retain the high bits as the two's complement number is wrong without it
                // [07..00] drop the high 0 as the two's complement positive number remains clear
                bool clearHighF = false;
                byte head = bits[cur];

                if (head > 0xF7)
                {
                    head -= 0xF0;
                    clearHighF = true;
                }

                if (head < 0x08 || clearHighF)
                {
                    // {0xF8-0xFF} print as {8-F}
                    // {0x00-0x07} print as {0-7}
                    sb.Append(head < 10 ?
                        (char)(head + '0') :
                        format == 'X' ? (char)((head & 0xF) - 10 + 'A') : (char)((head & 0xF) - 10 + 'a'));
                    cur--;
                }
            }

            if (cur > -1)
            {
                Span<char> chars = sb.AppendSpan((cur + 1) * 2);
                int charsPos = 0;
                string hexValues = format == 'x' ? "0123456789abcdef" : "0123456789ABCDEF";
                while (cur > -1)
                {
                    byte b = bits[cur--];
                    chars[charsPos++] = hexValues[b >> 4];
                    chars[charsPos++] = hexValues[b & 0xF];
                }
            }

            if (digits > sb.Length)
            {
                // Insert leading zeros, e.g. user specified "X5" so we create "0ABCD" instead of "ABCD"
                sb.Insert(
                    0,
                    value._sign >= 0 ? '0' : (format == 'x') ? 'f' : 'F',
                    digits - sb.Length);
            }

            if (arrayToReturnToPool != null)
            {
                ArrayPool<byte>.Shared.Return(arrayToReturnToPool);
            }

            if (targetSpan)
            {
                spanSuccess = sb.TryCopyTo(destination, out charsWritten);
                return null;
            }
            else
            {
                charsWritten = 0;
                spanSuccess = false;
                return sb.ToString();
            }
        }

        internal static string FormatBigInteger(BigInteger value, string? format, NumberFormatInfo info)
        {
            return FormatBigInteger(targetSpan: false, value, format, format, info, default, out _, out _)!;
        }

        internal static bool TryFormatBigInteger(BigInteger value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
        {
            FormatBigInteger(targetSpan: true, value, null, format, info, destination, out charsWritten, out bool spanSuccess);
            return spanSuccess;
        }

        private static string? FormatBigInteger(
            bool targetSpan, BigInteger value,
            string? formatString, ReadOnlySpan<char> formatSpan,
            NumberFormatInfo info, Span<char> destination, out int charsWritten, out bool spanSuccess)
        {
            Debug.Assert(formatString == null || formatString.Length == formatSpan.Length);

            int digits = 0;
            char fmt = ParseFormatSpecifier(formatSpan, out digits);
            if (fmt == 'x' || fmt == 'X')
            {
                return FormatBigIntegerToHex(targetSpan, value, fmt, digits, info, destination, out charsWritten, out spanSuccess);
            }


            if (value._bits == null)
            {
                if (fmt == 'g' || fmt == 'G' || fmt == 'r' || fmt == 'R')
                {
                    formatSpan = formatString = digits > 0 ? $"D{digits}" : "D";
                }

                if (targetSpan)
                {
                    spanSuccess = value._sign.TryFormat(destination, out charsWritten, formatSpan, info);
                    return null;
                }
                else
                {
                    charsWritten = 0;
                    spanSuccess = false;
                    return value._sign.ToString(formatString, info);
                }
            }

            // First convert to base 10^9.
            const uint kuBase = 1000000000; // 10^9
            const int kcchBase = 9;

            int cuSrc = value._bits.Length;
            int cuMax;
            try
            {
                cuMax = checked(cuSrc * 10 / 9 + 2);
            }
            catch (OverflowException e) { throw new FormatException(SR.Format_TooLarge, e); }
            uint[] rguDst = new uint[cuMax];
            int cuDst = 0;

            for (int iuSrc = cuSrc; --iuSrc >= 0;)
            {
                uint uCarry = value._bits[iuSrc];
                for (int iuDst = 0; iuDst < cuDst; iuDst++)
                {
                    Debug.Assert(rguDst[iuDst] < kuBase);
                    ulong uuRes = NumericsHelpers.MakeUInt64(rguDst[iuDst], uCarry);
                    rguDst[iuDst] = (uint)(uuRes % kuBase);
                    uCarry = (uint)(uuRes / kuBase);
                }
                if (uCarry != 0)
                {
                    rguDst[cuDst++] = uCarry % kuBase;
                    uCarry /= kuBase;
                    if (uCarry != 0)
                        rguDst[cuDst++] = uCarry;
                }
            }

            int cchMax;
            try
            {
                // Each uint contributes at most 9 digits to the decimal representation.
                cchMax = checked(cuDst * kcchBase);
            }
            catch (OverflowException e) { throw new FormatException(SR.Format_TooLarge, e); }

            bool decimalFmt = (fmt == 'g' || fmt == 'G' || fmt == 'd' || fmt == 'D' || fmt == 'r' || fmt == 'R');
            if (decimalFmt)
            {
                if (digits > 0 && digits > cchMax)
                    cchMax = digits;
                if (value._sign < 0)
                {
                    try
                    {
                        // Leave an extra slot for a minus sign.
                        cchMax = checked(cchMax + info.NegativeSign.Length);
                    }
                    catch (OverflowException e) { throw new FormatException(SR.Format_TooLarge, e); }
                }
            }

            int rgchBufSize;

            try
            {
                // We'll pass the rgch buffer to native code, which is going to treat it like a string of digits, so it needs
                // to be null terminated.  Let's ensure that we can allocate a buffer of that size.
                rgchBufSize = checked(cchMax + 1);
            }
            catch (OverflowException e) { throw new FormatException(SR.Format_TooLarge, e); }

            char[] rgch = new char[rgchBufSize];

            int ichDst = cchMax;

            for (int iuDst = 0; iuDst < cuDst - 1; iuDst++)
            {
                uint uDig = rguDst[iuDst];
                Debug.Assert(uDig < kuBase);
                for (int cch = kcchBase; --cch >= 0;)
                {
                    rgch[--ichDst] = (char)('0' + uDig % 10);
                    uDig /= 10;
                }
            }
            for (uint uDig = rguDst[cuDst - 1]; uDig != 0;)
            {
                rgch[--ichDst] = (char)('0' + uDig % 10);
                uDig /= 10;
            }

            if (!decimalFmt)
            {
                // sign = true for negative and false for 0 and positive values
                bool sign = (value._sign < 0);
                // The cut-off point to switch (G)eneral from (F)ixed-point to (E)xponential form
                int precision = 29;
                int scale = cchMax - ichDst;

                var sb = new ValueStringBuilder(stackalloc char[128]); // arbitrary stack cut-off
                FormatProvider.FormatBigInteger(ref sb, precision, scale, sign, formatSpan, info, rgch, ichDst);

                if (targetSpan)
                {
                    spanSuccess = sb.TryCopyTo(destination, out charsWritten);
                    return null;
                }
                else
                {
                    charsWritten = 0;
                    spanSuccess = false;
                    return sb.ToString();
                }
            }

            // Format Round-trip decimal
            // This format is supported for integral types only. The number is converted to a string of
            // decimal digits (0-9), prefixed by a minus sign if the number is negative. The precision
            // specifier indicates the minimum number of digits desired in the resulting string. If required,
            // the number is padded with zeros to its left to produce the number of digits given by the
            // precision specifier.
            int numDigitsPrinted = cchMax - ichDst;
            while (digits > 0 && digits > numDigitsPrinted)
            {
                // pad leading zeros
                rgch[--ichDst] = '0';
                digits--;
            }
            if (value._sign < 0)
            {
                string negativeSign = info.NegativeSign;
                for (int i = negativeSign.Length - 1; i > -1; i--)
                    rgch[--ichDst] = negativeSign[i];
            }

            int resultLength = cchMax - ichDst;
            if (!targetSpan)
            {
                charsWritten = 0;
                spanSuccess = false;
                return new string(rgch, ichDst, cchMax - ichDst);
            }
            else if (new ReadOnlySpan<char>(rgch, ichDst, cchMax - ichDst).TryCopyTo(destination))
            {
                charsWritten = resultLength;
                spanSuccess = true;
                return null;
            }
            else
            {
                charsWritten = 0;
                spanSuccess = false;
                return null;
            }
        }
    }
}
