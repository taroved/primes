using System;


namespace Tests.Prime
{
    public struct Record
    {
        public long firstPrime, firstShift, count, lastPrime, lastShift;

        public Record(long firstPrime, long firstShift, long count, long lastPrime, long lastShift)
        {
            this.firstPrime = firstPrime;
            this.firstShift = firstShift;
            this.count = count;
            this.lastPrime = lastPrime;
            this.lastShift = lastShift;
        }

        public int Compare(Record record)
        {
            if (count > record.count)
                return 1;
            else if (count == record.count)
            {
                if (firstPrime > record.firstPrime)
                    return 1;
                else if (firstPrime == record.firstPrime)
                {
                    if (firstShift < record.firstShift)
                        return 1;
                    else if (firstShift == record.firstShift)
                        return 0;
                }
            }
            return -1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Record))
                return false;
            Record record = (Record)obj;
            return this.Compare(record) == 0;
        }

        public override int GetHashCode()
        {
            return (int)(this.count % ((long)int.MaxValue+1));
        }

        public static bool operator ==(Record left, Record right)
        {
            return left.Compare(right) == 0;
        }
        public static bool operator !=(Record left, Record right)
        {
            return left.Compare(right) != 0;
        }
        public static bool operator <(Record left, Record right)
        {
            return left.Compare(right) < 0;
        }
        public static bool operator >(Record left, Record right)
        {
            return left.Compare(right) > 0;
        }
    }
}
