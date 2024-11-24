
namespace codecrafters_redis.src.Models;

public class BulkString
{
    public string Value { get; set; }


    public BulkString(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            Value = "$-1\r\n";
            return;
        }

        Value = $"${value.Length}\r\n{value}\r\n";
    }

    public BulkString(string[] values)
    {
        Value = $"*{values.Length}\r\n{string.Join("", values.Select(x => $"${x.Length}\r\n{x}\r\n"))}";
    }

    public static string[] ExtractBulkString(string bulk)
    {
        bulk = bulk.Replace("\\r", "\r").Replace("\\n", "\n");

        if (bulk[0] == '*')
        {
            return bulk
                .Substring(2).TrimStart().Split("$").Skip(1)
                .Select(x =>
                {
                    var length = "";
                    for (int i = 0; i <= x.Length - 1; i++)
                    {
                        if (int.TryParse(x[i].ToString(), System.Globalization.CultureInfo.InvariantCulture, out int number))
                            length += x[i];
                        else
                            break;
                    }

                    return x.Substring(length.Length).Trim();
                }).ToArray();
        }

        return [bulk.Substring(1).Trim()];
    }
}
