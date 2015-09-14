using System;
using System.Threading;
using System.IO;


namespace Tests.Prime
{
    public class Primes 
    {
        static public void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Argument is required");
            }
            else
            {
                string inputFile = args[0];

                // get file zise
                FileInfo f = new FileInfo(inputFile);            
                long fileSize = f.Length;

                using(Stream source = File.OpenRead(inputFile))
                {
                    PrimeManager mgr = new PrimeManager(source);
                    Console.WriteLine("Press any key to stop...");
                    
                    long currentOffset = 0;
                    while (!Console.KeyAvailable)
                    {
                        int blockSize = mgr.RunBlock();
                        if (blockSize == 0)
                            break;
                        
                        currentOffset += blockSize;
                        
                        int w = Console.WindowWidth - "[>]".Length;
                        int progress = (int)(w * currentOffset / fileSize);
                        
                        Console.Write("\r[{0}>{1}]", new String('=', progress), new String(' ', w-progress));
                    }

                    Record? bestRecord = mgr.FindBestRecord();
                    
                    if (bestRecord == null)
                        Console.WriteLine("Queue is not found");
                    else
                    {
                        Record best = (Record)bestRecord;
                        Console.WriteLine("Best queue: first prime={0}, first prime offset={1}, last prime={2}, last prime offset={3}, count of primes={4}", best.firstPrime, best.firstShift, best.lastPrime, best.lastShift, best.count);
                    }
                }
            }
        }
    }
}
