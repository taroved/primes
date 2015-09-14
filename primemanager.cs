using System;
using System.Collections.Generic;
using System.IO;
using System.Management;


public class PrimeManager
{
    const int NUMBER_SIZE = 6;
    const int BLOCK_SIZE = 100000;

    int threadCount;
    Stream fileStream;

    long offset = 0;
    
    public PrimeManager(Stream fileStream)
    {
        this.fileStream = fileStream;
        //threadCount = GetCoreCount() - 1; // minus one for better responsobility of keyboard
    }

    public int RunBlock()
    {
        byte[] buffer = new byte[BLOCK_SIZE * NUMBER_SIZE];
        int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
        if (bytesRead > 0)
        {
            foreach (PrimeShiftPair pair in GetPrimeShiftPairs(ParseNumbers(buffer, bytesRead)))
            {
                UpdatePrimeSortedRecords(pair.prime, pair.shift + offset);
            }
            offset += bytesRead / NUMBER_SIZE;
        }
        return bytesRead;
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

	    for (long i=2; i<=Primes.Lsqrt(n); i++)
	    {
	        if (n % i == 0)
	        {
                return false;
            }
	    }
        return true;
    }

    // Finds the integer square root of a positive number  
    static public long Lsqrt(long num) {  
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

