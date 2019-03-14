using System;
using System.Collections.Generic;
using System.Globalization;
using ByrneLabs.Commons;

namespace ByrneLabs.TestoRoboto.HttpServices.Mutators.Resources
{
    public static class RandomValues
    {
        public static IEnumerable<object> Values => new object[]
        {
            DateTime.MinValue, DateTime.MaxValue, DateTime.Now, BetterRandom.NextDateTime(), BetterRandom.NextDateTime(DateTime.Now), BetterRandom.NextDateTime(DateTime.Now, DateTime.MaxValue), new DateTime(1700, 1, 1), new DateTime(1970, 1, 1),
            Guid.Empty, Guid.NewGuid(),
            int.MaxValue, int.MinValue, 0, BetterRandom.Next(),
            long.MaxValue, long.MinValue, BetterRandom.NextLong(),
            decimal.MaxValue, decimal.MinValue, 0.0,
            float.MaxValue, float.MinValue,
            int.MaxValue.ToString(), int.MinValue.ToString(), "0", BetterRandom.Next().ToString(),
            long.MaxValue.ToString(), long.MinValue.ToString(), BetterRandom.NextLong().ToString(),
            decimal.MaxValue.ToString(CultureInfo.InvariantCulture), decimal.MinValue.ToString(CultureInfo.InvariantCulture), "0.0",
            double.MaxValue.ToString(CultureInfo.InvariantCulture), double.MinValue.ToString(CultureInfo.InvariantCulture), "00.00",
            float.MaxValue.ToString(CultureInfo.InvariantCulture), float.MinValue.ToString(CultureInfo.InvariantCulture),
            "999999999999999999999999999999999999999999999999999999", "-999999999999999999999999999999999999999999999999999999",
            BetterRandom.NextString(10, 10), BetterRandom.NextString(10, 10, BetterRandom.CharacterGroup.Ascii), BetterRandom.NextString(10, 10, BetterRandom.CharacterGroup.ExtendedAscii), BetterRandom.NextString(10, 10, BetterRandom.CharacterGroup.Unicode),
            BetterRandom.NextString(100, 100), BetterRandom.NextString(100, 100, BetterRandom.CharacterGroup.Ascii), BetterRandom.NextString(100, 100, BetterRandom.CharacterGroup.ExtendedAscii), BetterRandom.NextString(100, 100, BetterRandom.CharacterGroup.Unicode),
            BetterRandom.NextString(10000, 10000), BetterRandom.NextString(10000, 10000, BetterRandom.CharacterGroup.Ascii), BetterRandom.NextString(10000, 10000, BetterRandom.CharacterGroup.ExtendedAscii), BetterRandom.NextString(10000, 10000, BetterRandom.CharacterGroup.Unicode),
            BetterRandom.NextString(10, 10, BetterRandom.CharacterGroup.AsciiWithControlCharacters), BetterRandom.NextString(10, 10, BetterRandom.CharacterGroup.ExtendedAsciiWithControlCharacters), BetterRandom.NextString(10, 10, BetterRandom.CharacterGroup.UnicodeWithControlCharacters),
            BetterRandom.NextString(100, 100, BetterRandom.CharacterGroup.AsciiWithControlCharacters), BetterRandom.NextString(100, 100, BetterRandom.CharacterGroup.ExtendedAsciiWithControlCharacters), BetterRandom.NextString(100, 100, BetterRandom.CharacterGroup.UnicodeWithControlCharacters),
            BetterRandom.NextString(10000, 10000, BetterRandom.CharacterGroup.AsciiWithControlCharacters), BetterRandom.NextString(10000, 10000, BetterRandom.CharacterGroup.ExtendedAsciiWithControlCharacters), BetterRandom.NextString(10000, 10000, BetterRandom.CharacterGroup.UnicodeWithControlCharacters),
            true, false
        };
    }
}
