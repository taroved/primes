using System;
using System.Collection.Generic;


public class PrimeManager
{
    int const NUMBER_SIZE = 6;
    int const BLOCK_SIZE = 100000;

    int threadCount;
    string filePath;
    
    public PrimeManager(string filePath)
    {
        threadCount = GetCoreCount() - 1; // minus one for better responsobility of keyboard
    }

    public RunBlock()
    {
        
    }

    SortedList<RecordSortingKey, Record> sortedRecords = new SortedList<RecordSortingKey, Record>(); 
    HashSet<long> primes = new HashSet<long>();

    void UpdatePrimeSortedList(List<PrimeShiftPair> primeShiftPairs)
    {
        if (!primes.Contains(primeShiftPairs.prime))
            sortedRecords.Add();
    }

    ///<summary>
    ///Find record which contain max lower lastPrime with max count and max firstPrime and min firstShift
    ///</summary>
    Record FindTraitRecord()

    ///<summary>
    ///Parse bytes array to list of number. Bytes is numbers in big-indian order.
    ///</summary>
    static long[] ParseBytes(byte[0] bytes)
    {
        long[] numbers = new long[bytes.Length / NUMBER_SIZE];
        int ni = 0;
        for (int i=0; i<bytes.Length; i+=NUMBER_SIZE)
        {
            long num = 0;
            for (int k=NUMBER_SIZE-1; k>=0; k--)
            {
                num = num << 8 + bytes[i*NUMBER_SIZE];
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

        public PrimeShiftPair(prime, shift)
        {
            this.prime = prime;
            this.shift = shift;
        }
    }

    struct CountLastPrime
    {
        public long count, lastPrime;

        public CountLastPrime(count, lastPrime)
        {
            this.count = count;
            this.lastPrime = lastPrime;
        }
    }

    struct Record
    {
        public long firstPrime, firstShift, count, lastPrime;

        public Record(firstPrime, firstShift, count, lastPrime)
        {
            this.firstPrime = firstPrime;
            this.firstShift = firstShift;
            this.count = count;
            this.lastPrime = lastPrime;
        }
    }

    struct RecordSortingKey: IComparable
    {
        public long firstPrime, firstShift, count, lastPrime;

        public int CompareTo(object obj) {
            if (obj == null) return 1;

            recordKey = obj as RecordSortingKey;
            if (lastPrime > recordKey.lastPrime)
                return -1;
            else if (lastPrime == recordKey.lastPrime)
            {
                if (count > recordKey.count)
                    return -1;
                else if (count == recordKey.count)
                {
                    if (firstPrime > recordKey.firstPrime)
                        return -1;
                    else if (firstPrime == recordKey.firstPrime)
                    {
                        if (firstShift < recordKey.firstShift)
                            return -1;
                        else if (firstShift == recordKey.firstShift)
                            return 0;
                    }
                }
            }
            return 1;
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

