﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using App.Metrics.Configuration;

namespace App.Metrics
{
    /// <summary>
    ///     Sampling avoids unbound memory usage, sampling is used to generate <see cref="Histogram" />'s from a reservoir of
    ///     values
    /// </summary>
    /// <remarks>
    ///     https://en.wikipedia.org/wiki/Reservoir_sampling
    /// </remarks>
    public enum SamplingType
    {
        /// <summary>
        ///     If set the default sampling type configured via the <see cref="AppMetricsOptions" /> will be used.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Sampling will be done with a A High Dynamic Range (HDR) Histogram. Note: The HDR Histogram implementation is in
        ///     beta stage, some issues might still be present.
        /// </summary>
        /// <remarks>
        ///     The HDR Histogram is an extremely efficient implementation of a histogram.
        ///     More information about <a href="http://hdrhistogram.github.io/HdrHistogram/">HDR Histogram</a>
        /// </remarks>
        HighDynamicRange,

        /// <summary>
        ///     Sampling will be done with a Exponentially Decaying Reservoir.
        /// </summary>
        /// <remarks>
        ///     A histogram with an exponentially decaying reservoir produces quantiles which are representative of (roughly) the
        ///     last five minutes of data.
        ///     It does so by using a forward-decaying priority reservoir with an exponential weighting towards newer data.
        ///     Unlike the uniform reservoir, an exponentially decaying reservoir represents recent data, allowing you to know very
        ///     quickly if the distribution
        ///     of the data has changed.
        ///     More information about
        ///     <a href="http://metrics.codahale.com/manual/core/#man-core-histograms">Exponentially Decaying Reservoir</a>
        /// </remarks>
        ExponentiallyDecaying,

        /// <summary>
        ///     Sampling will done with a Uniform Reservoir.
        /// </summary>
        /// <remarks>
        ///     A histogram with a uniform reservoir produces quantiles which are valid for the entirely of the histogram’s
        ///     lifetime.
        ///     It will return a median value, for example, which is the median of all the values the histogram has ever been
        ///     updated with.
        ///     Use a uniform histogram when you’re interested in long-term measurements.
        ///     Don’t use one where you’d want to know if the distribution of the underlying data stream has changed recently.
        ///     More information about
        ///     <a href="http://metrics.codahale.com/manual/core/#man-core-histograms">Exponentially Decaying Reservoir</a>
        /// </remarks>
        LongTerm,

        /// <summary>
        ///     Sampling will done with a Sliding Window Reservoir.
        ///     A histogram with a sliding window reservoir produces quantiles which are representative of the past N measurements.
        ///     More information about
        ///     <a href="http://metrics.codahale.com/manual/core/#man-core-histograms">Exponentially Decaying Reservoir</a>
        /// </summary>
        SlidingWindow
    }
}