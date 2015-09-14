using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Threading.Tasks;

namespace Tests.Prime
{
    public class PrimeManager
    {
        const int NUMBER_SIZE = 6;
        const int BLOCK_SIZE = 100000;

        int threadCount = 4;
        Stream fileStream;

        long offset = 0;
        
        public PrimeManager(Stream fileStream)
        {
            this.fileStream = fileStream;
            //threadCount = GetCoreCount(); // minus one for better responsobility of keyboard
        }

        public int RunBlock()
        {
            int bytesReadSum = 0;

            List<long[]> numbers = new List<long[]>();

            //prepare numbers for threads
            for (int i=0; i<threadCount; i++)
            {
                byte[] buffer = new byte[BLOCK_SIZE * NUMBER_SIZE];
                int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                numbers.Add(ParseNumbers(buffer, bytesRead));
                bytesReadSum += bytesRead;
            }
            
            if (bytesReadSum > 0)
            {
                //get pairs in several threads
                List<PrimeShiftPair> pairs = GetPrimeShiftPairsMultiThread(numbers);

                // update sorted list
                foreach (PrimeShiftPair pair in pairs)
                {
                    UpdatePrimeSortedRecords(pair.prime, pair.shift + offset);
                }
                offset += bytesReadSum / NUMBER_SIZE;
            }
            return bytesReadSum;
        }

        List<PrimeShiftPair> GetPrimeShiftPairsMultiThread(List<long[]> numbersList)
        {
            List<Task<List<PrimeShiftPair>>> tasks = new List<Task<List<PrimeShiftPair>>>();

            //start tasks
            foreach (long[] numbers in numbersList)
            {
                Task<List<PrimeShiftPair>> t = new Task<List<PrimeShiftPair>>(nums => GetPrimeShiftPairs((long[])nums), numbers);
                tasks.Add(t);
                t.Start();
            }

            List<PrimeShiftPair> pairs = new List<PrimeShiftPair>();
            //wait tasks
            foreach (Task<List<PrimeShiftPair>> t in tasks)
            {
                t.Wait();
                pairs.AddRange(t.Result);
            }

            return pairs;
        }

        SortedList<long, Record> sortedRecords = new SortedList<long, Record>(); 

        void UpdatePrimeSortedRecords(long prime, long shift)
        {
            int previousIndex = BinarySearchSameOrPreviousRecordIndex(prime);

            if (previousIndex == -1)
            {
                sortedRecords.Add(prime, new Record(prime, shift, 1, prime, shift));
            }
            else
            { 
                Record previous = sortedRecords.Values[previousIndex];
                if (previous.lastPrime == prime)  // try to replace current lastPrime
                {
                    if (previousIndex > 0)
                    {
                        Record beforePrevious = sortedRecords.Values[previousIndex - 1];
                        beforePrevious.count ++;
                        beforePrevious.lastPrime = prime;
                        beforePrevious.lastShift = shift;
                        if (beforePrevious > previous)
                        {
                            sortedRecords[previousIndex] = beforePrevious;
                        }
                    }
                }
                else
                {
                    previous.count ++;
                    previous.lastPrime = prime;
                    previous.lastShift = shift;
                    sortedRecords.Add(prime, previous);
                }
            }
        }

        public Record? FindBestRecord()
        {
            if (sortedRecords.Count == 0)
                return null;

            Record best = sortedRecords.Values[0];
            for (int i=1; i<sortedRecords.Values.Count; i++)
                if (sortedRecords.Values[i] > best)
                    best = sortedRecords.Values[i];

            return best;
        }

        ///<summary>
        ///Find key index for the prime or index of previous prime.
        ///</summary>
        int BinarySearchSameOrPreviousRecordIndex(long prime)
        {
            int floor = 0;
            int ceil = sortedRecords.Count-1;

            while (floor <= ceil)
            {
                int current = (floor + ceil) / 2;

                if (prime == sortedRecords.Keys[current] || (prime > sortedRecords.Keys[current] && (current == sortedRecords.Count-1 || prime < sortedRecords.Keys[current+1])))
                    return current;
                else if (sortedRecords.Keys[current] < prime)
                    floor = current + 1;
                else 
                    ceil = current - 1;
            }

            return -1;
        }

        ///<summary>
        ///Parse bytes array to list of number. Bytes is numbers in big-indian order. Ignore tail bytes.
        ///</summary>
        static long[] ParseNumbers(byte[] bytes, int length)
        {
            int numberCount = length / NUMBER_SIZE;
            long[] numbers = new long[numberCount]; 
            int ni = 0;
            for (int i=0; i<numberCount; i++)
            {
                long num = 0;
                for (int k=0; k<NUMBER_SIZE; k++)
                {
                    num = (num << 8) + bytes[i*NUMBER_SIZE + k];
                }
                numbers[ni++] = num;
            }
            return numbers;
        }

        static List<PrimeShiftPair> GetPrimeShiftPairs(long[] nums)
        {
            List<PrimeShiftPair> l = new List<PrimeShiftPair>();
            for (int i=0; i<nums.Length; i++)
            {
                if (Prime(nums[i]))
                    l.Add(new PrimeShiftPair(nums[i], i));
            }
            return l;
        }

        static bool Prime(long n)
        {
            if (n == 1) // one is neither a prime nor a composite
                return false;

            for (long i=2; i<=Lsqrt(n); i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

        // Finds the integer square root of a positive number  
        public static long Lsqrt(long num) {  
            if (0 == num) { return 0; }  // Avoid zero divide  
            long n = (num / 2) + 1;       // Initial estimate, never low  
            long n1 = (n + (num / n)) / 2;  
            while (n1 < n) {  
                n = n1;  
                n1 = (n + (num / n)) / 2;  
            }
            return n;  
        }

        struct PrimeShiftPair
        {
            public long prime, shift;

            public PrimeShiftPair(long prime, long shift)
            {
                this.prime = prime;
                this.shift = shift;
            }
        }

        struct CountLastPrime
        {
            public long count, lastPrime;

            public CountLastPrime(long count, long lastPrime)
            {
                this.count = count;
                this.lastPrime = lastPrime;
            }
        }
        
        int GetCoreCount()
        {
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                    coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            return coreCount;
        }
    }
}
