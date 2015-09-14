using System;
using System.Collection.Generic;


public class PrimeManager
{
    int const NUMBER_SIZE = 6;
    int const BLOCK_SIZE = 100000;

    int threadCount;
    string filePath;

    long offset = 0;
    
    public PrimeManager(Stream fileStream)
    {
        threadCount = GetCoreCount() - 1; // minus one for better responsobility of keyboard
    }

    public RunBlock()
    {
        byte[] buffer = new byte[BLOCK_SIZE * NUMBER_SIZE];
        bytesRead = source.Read(buffer, 0, buffer.Length);
        if (bytesRead > 0)
        {
            List<PrimeShiftPair> primeShiftPairs = new List<PrimeShiftPair>();
            foreach (PrimeShiftPair pair in GetPrimeShiftPairs(ParseNumbers(buffer)))
            {
                UpdatePrimeSortedList(pair.prime, pair.shift + offset);
            }
            offset += bytesRead.Length / NUMBER_SIZE;
        }   
    }

    SortedList<long, Record> sortedRecords = new SortedList<long, Record>(); 

    void UpdatePrimeSortedList(long prime, long shift)
    {
        long previousIndex = BinarySearchSameOrPreviousRecordIndex(prime);
        
        Record previous = sortedRecords.GetKey(previousIndex);
        
        if (previous.lastPrime == pair.prime)  // try to replace current lastPrime
        {
            if (previousIndex > 0)
            {
                Record beforePrevious = sortedRecords.GetKey(previousIndex - 1);
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

    ///<summary>
    ///Find key index for the prime or index of previous prime.
    ///</summary>
    long BinarySearchSameOrPreviousRecordIndex(long prime)
    {
        long floor = 0;
        long ceil = sortedRecords.Count-1;

        while (floor <= ceil)
        {
            long current = (floor + ceiling) / 2;

            if (prime == sortedRecords.GetKey(current) || (prime > sortedRecords.GetKey(current) && (current == sortedRecord.Count-1 || record < sortedRecords.GetKey(current+1))))
                return current;
            else if (sortedRecords.GetKey(current) < prime)
                floor = current + 1;
            else 
                ceil = current - 1;
        }

        return -1;
    }

    ///<summary>
    ///Parse bytes array to list of number. Bytes is numbers in big-indian order. Ignore tail bytes.
    ///</summary>
    static long[] ParseNumbers(byte[0] bytes)
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
                l.Add(new PrimeShiftPair(nums[i], i);
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

    struct Record: IComparable
    {
        public long firstPrime, firstShift, count, lastPrime, lastShift;

        public Record(firstPrime, firstShift, count, lastPrime, lastShift)
        {
            this.firstPrime = firstPrime;
            this.firstShift = firstShift;
            this.count = count;
            this.lastPrime = lastPrime;
            this.lastShift = lastShift;
        }
        
        public int CompareTo(object obj) {
            if (obj == null) return 1;

            recordKey = obj as RecordSortingKey;
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
            return 1;
        }

        public static bool operator ==(RatingInformation left, RatingInformation right)
        {
            return left.CompareTo(right) == 0;
        }
        public static bool operator !=(RatingInformation left, RatingInformation right)
        {
            return left.CompareTo(right) != 0;
        }
        public static bool operator <(RatingInformation left, RatingInformation right)
        {
            return left.CompareTo(right) < 0;
        }
        public static bool operator >(RatingInformation left, RatingInformation right)
        {
            return left.CompareTo(right) > 0;
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

