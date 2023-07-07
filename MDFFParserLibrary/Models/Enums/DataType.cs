﻿namespace MDFFParserLibrary.Models.Enums;

/// <summary>
///     See
///     https://www.aemo.com.au/Electricity/National-Electricity-Market-NEM/Retail-and-metering/-/media/EBA9363B984841079712B3AAD374A859.ashx
///     Section 15 for deatils on Energy flow direction.
/// </summary>
public enum DataType
{
    /// <summary>
    ///     Energy Imported into the Pool.
    ///     I.e. Energy generated by a Customer (via Solar) is considered Imported.
    /// </summary>
    Import,

    /// <summary>
    ///     Energy Exported into the Pool.
    ///     I.e. Energy consumed by a Customer is considered Exported.
    /// </summary>
    Export,
    Net,
    KVAh,
    PowerFactor,
    QMetering,
    ParMetering,
    VoltsOrAmps
}