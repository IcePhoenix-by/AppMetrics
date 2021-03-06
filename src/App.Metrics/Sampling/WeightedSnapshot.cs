// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET and will retain the same license
// Ported/Refactored to .NET Standard Library by Allan Hardy

using System;
using System.Collections.Generic;
using System.Linq;
using App.Metrics.Sampling.Interfaces;

namespace App.Metrics.Sampling
{
    public sealed class WeightedSnapshot : ISnapshot
    {
        private readonly double[] _normWeights;
        private readonly double[] _quantiles;
        private readonly long[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedSnapshot"/> class.
        /// </summary>
        /// <param name="count">The count of the sample set.</param>
        /// <param name="values">The values within the sample set.</param>
        public WeightedSnapshot(long count, IEnumerable<WeightedSample> values)
        {
            Count = count;
            var sample = values.ToArray();
            Array.Sort(sample, WeightedSampleComparer.Instance);

            var sumWeight = sample.Sum(s => s.Weight);

            _values = new long[sample.Length];
            _normWeights = new double[sample.Length];
            _quantiles = new double[sample.Length];

            for (var i = 0; i < sample.Length; i++)
            {
                _values[i] = sample[i].Value;
                _normWeights[i] = sample[i].Weight / sumWeight;
                if (i > 0)
                {
                    _quantiles[i] = _quantiles[i - 1] + _normWeights[i - 1];
                }
            }

            MinUserValue = sample.Select(s => s.UserValue).FirstOrDefault();
            MaxUserValue = sample.Select(s => s.UserValue).LastOrDefault();
        }

        /// <inheritdoc cref="ISnapshot"/>
        public long Count { get; }

        /// <inheritdoc cref="ISnapshot"/>
        public long Max => _values.LastOrDefault();

        /// <inheritdoc cref="ISnapshot"/>
        public string MaxUserValue { get; }

        /// <inheritdoc cref="ISnapshot"/>
        public double Mean
        {
            get
            {
                if (_values.Length == 0)
                {
                    return 0.0;
                }

                double sum = 0;
                for (var i = 0; i < _values.Length; i++)
                {
                    sum += _values[i] * _normWeights[i];
                }
                return sum;
            }
        }

        /// <inheritdoc cref="ISnapshot"/>
        public double Median => GetValue(0.5d);

        /// <inheritdoc cref="ISnapshot"/>
        public long Min => _values.FirstOrDefault();

        /// <inheritdoc cref="ISnapshot"/>
        public string MinUserValue { get; }

        /// <inheritdoc cref="ISnapshot"/>
        public double Percentile75 => GetValue(0.75d);

        /// <inheritdoc cref="ISnapshot"/>
        public double Percentile95 => GetValue(0.95d);

        /// <inheritdoc cref="ISnapshot"/>
        public double Percentile98 => GetValue(0.98d);

        /// <inheritdoc cref="ISnapshot"/>
        public double Percentile99 => GetValue(0.99d);

        /// <inheritdoc cref="ISnapshot"/>
        public double Percentile999 => GetValue(0.999d);

        /// <inheritdoc cref="ISnapshot"/>
        public int Size => _values.Length;

        /// <inheritdoc cref="ISnapshot"/>
        public double StdDev
        {
            get
            {
                if (Size <= 1)
                {
                    return 0;
                }

                var mean = Mean;
                double variance = 0;

                for (var i = 0; i < _values.Length; i++)
                {
                    var diff = _values[i] - mean;
                    variance += _normWeights[i] * diff * diff;
                }

                return Math.Sqrt(variance);
            }
        }

        /// <inheritdoc cref="ISnapshot"/>
        public IEnumerable<long> Values => _values;

        /// <inheritdoc cref="ISnapshot"/>
        public double GetValue(double quantile)
        {
            if (quantile < 0.0 || quantile > 1.0 || double.IsNaN(quantile))
            {
                throw new ArgumentException($"{quantile} is not in [0..1]");
            }

            if (Size == 0)
            {
                return 0;
            }

            var posx = Array.BinarySearch(_quantiles, quantile);
            if (posx < 0)
            {
                posx = ~posx - 1;
            }

            if (posx < 1)
            {
                return _values[0];
            }

            return posx >= _values.Length ? _values[_values.Length - 1] : _values[posx];
        }

        private class WeightedSampleComparer : IComparer<WeightedSample>
        {
            public static readonly IComparer<WeightedSample> Instance = new WeightedSampleComparer();

            public int Compare(WeightedSample x, WeightedSample y)
            {
                return Comparer<long>.Default.Compare(x.Value, y.Value);
            }
        }
    }
}