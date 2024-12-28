using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Type packet in hexadecimal without spaces...");
        string? packet_str = Console.ReadLine();
#if DEBUG
        Console.WriteLine("You type:\t" + packet_str);
#endif
        Console.WriteLine("Try parse...");

        if (packet_str?.Length % 2 != 0)
        {
            Console.WriteLine(">>\tSize of packet can't be odd!");
            Console.ReadLine();
            return;
        }

        List<byte> bytes = new();
        try
        {
            for (int i = 0; i < packet_str?.Length; i += 2)
            {
                bytes.Add(byte.Parse(packet_str.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }
        }
        catch (Exception e)
        {

        }

        foreach (var b in bytes)
            Console.Write(b + " ");
        Console.WriteLine();

        int size_32 = bytes.Count / 2 + bytes.Count % 2 != 0 ? 1 : 0;
        var upd_bytes = new List<byte>();

        for (int i = 0; i < bytes.Count; i += 4)
        {
            var diff = bytes.Count - i;
            if (diff >= 4)
            {
                for (int j = 3; j >= 0; --j)
                    upd_bytes.Add(bytes[i + j]);
            }
            else
            {
                var diff2 = 4 - diff;
                var _diff = diff;
                while (diff2-- > 0)
                    upd_bytes.Add(0x00);

                while (_diff-- > 0)
                    upd_bytes.Add(bytes[i + _diff]);

            }
        }

        Console.Write("Packet:\t\t");
        foreach (var ub in upd_bytes)
            Console.Write(ub + " ");
        Console.WriteLine();

        byte crc = crc8x_simple(0xff, upd_bytes);
        Console.WriteLine("CRC:\t\t" + crc);

        Console.Write("Command:\t");
        foreach (var b in bytes)
            Console.Write("{0:X2} ", b);
        Console.WriteLine("{0:X2}", crc);

        Console.WriteLine("Press any key...");
        Console.ReadLine();
    }

    static byte crc8x_simple(uint crc, List<byte> packet)
    {
        if (packet.Count == 0)
            return 0xff;
        for (int i = 0; i < packet.Count; ++i)
        {
            crc ^= packet[i];
            for (byte k = 0; k < 8; k++)
                crc = (crc & 0x80) > 0 ? (crc << 1) ^ 0xb7 : crc << 1;
            crc &= 0xff;
        }
        return (byte)crc;
    }
}