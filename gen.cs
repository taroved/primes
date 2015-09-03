using System;
using System.IO;


public class Generator
{
    const int NUM_SIZE = 6;

    static public void Main (string[] args)
    {
        if (args == null)
        {
            Console.Write("Number list required");
        }
        else
        {
            byte[] bytes = new byte[args.Length*NUM_SIZE];
            for (int i=0; i<args.Length; i++)
            {
                long n = Convert.ToInt64(args[i]);
                for (int k=0; k<NUM_SIZE; k++)
                {
                    bytes[(NUM_SIZE * i) + k] = (byte)(n >> (8*(NUM_SIZE - k - 1))); //big-endian
                }
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open("generated.bin", FileMode.Create)))
            {
                writer.Write(bytes);
            }
        }
    }
}
