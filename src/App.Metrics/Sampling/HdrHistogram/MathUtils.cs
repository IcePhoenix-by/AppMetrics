﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


// Ported to.NET Standard Library by Allan Hardy

using System;

namespace App.Metrics.Sampling.HdrHistogram
{
    internal static class MathUtils
    {
        public static int NumberOfLeadingZeros(long i)
        {
            if (i == 0)
                return 64;
            var n = 1;
            var x = (int)unchecked((long)((ulong)i >> 32));
            if (x == 0)
            {
                n += 32;
                x = (int)i;
            }
            if ((uint)x >> 16 == 0)
            {
                n += 16;
                x <<= 16;
            }
            if ((uint)x >> 24 == 0)
            {
                n += 8;
                x <<= 8;
            }
            if ((uint)x >> 28 == 0)
            {
                n += 4;
                x <<= 4;
            }
            if ((uint)x >> 30 == 0)
            {
                n += 2;
                x <<= 2;
            }
            n -= (int)((uint)x >> 31);
            return n;
        }

        public static double ULP(double value)
        {
            // TODO: HdrHistogram

            // This is actually a constant in the same static class as this method, but 
            // we put it here for brevity of this example.
            const double MaxULP = 1.9958403095347198116563727130368E+292;

            if (Double.IsNaN(value))
            {
                return Double.NaN;
            }
            else if (Double.IsPositiveInfinity(value) || Double.IsNegativeInfinity(value))
            {
                return Double.PositiveInfinity;
            }
            else if (value == 0.0)
            {
                return Double.Epsilon; // Equivalent of Double.MIN_VALUE in Java; Double.MinValue in C# is the actual minimum value a double can hold.
            }
            else if (Math.Abs(value) == Double.MaxValue)
            {
                return MaxULP;
            }

            // All you need to understand about DoubleInfo is that it's a helper struct 
            // that provides more functionality than is used here, but in this situation, 
            // we only use the `Bits` property, which is just the double converted into a 
            // long.
            var bits = BitConverter.DoubleToInt64Bits(value);
            //DoubleInfo info = new DoubleInfo(value);

            // This is safe because we already checked for value == Double.MaxValue.
            return Math.Abs(BitConverter.Int64BitsToDouble(bits + 1) - value);
        }
    }
}