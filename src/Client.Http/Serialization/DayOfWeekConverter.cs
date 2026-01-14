// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class DayOfWeekConverter
    {
        internal static DayOfWeek? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            DayOfWeek obj;

            if (string.Compare(value, "Sunday", StringComparison.Ordinal) == 0)
                obj = DayOfWeek.Sunday;
            else if (string.Compare(value, "Monday", StringComparison.Ordinal) == 0)
                obj = DayOfWeek.Monday;
            else if (string.Compare(value, "Tuesday", StringComparison.Ordinal) == 0)
                obj = DayOfWeek.Tuesday;
            else if (string.Compare(value, "Wednesday", StringComparison.Ordinal) == 0)
                obj = DayOfWeek.Wednesday;
            else if (string.Compare(value, "Thursday", StringComparison.Ordinal) == 0)
                obj = DayOfWeek.Thursday;
            else if (string.Compare(value, "Friday", StringComparison.Ordinal) == 0)
                obj = DayOfWeek.Friday;
            else if (string.Compare(value, "Saturday", StringComparison.Ordinal) == 0)
                obj = DayOfWeek.Saturday;
            else
                throw new ArgumentException($"Could not convert string value {value} to enum type DayOfWeek.");

            return obj;
        }

        internal static void Serialize(JsonWriter writer, DayOfWeek? value)
        {
            switch (value)
            {
                case DayOfWeek.Sunday:
                    writer.WriteStringValue("Sunday");
                    break;
                case DayOfWeek.Monday:
                    writer.WriteStringValue("Monday");
                    break;
                case DayOfWeek.Tuesday:
                    writer.WriteStringValue("Tuesday");
                    break;
                case DayOfWeek.Wednesday:
                    writer.WriteStringValue("Wednesday");
                    break;
                case DayOfWeek.Thursday:
                    writer.WriteStringValue("Thursday");
                    break;
                case DayOfWeek.Friday:
                    writer.WriteStringValue("Friday");
                    break;
                case DayOfWeek.Saturday:
                    writer.WriteStringValue("Saturday");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type DayOfWeek");
            }
        }
    }
}
