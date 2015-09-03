using System;
using System.Threading;

 
public class Primes 
{

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

    static public bool Prime(long n)
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

    static public void Main(string[] args)
    {
        if (args == null)
        {
            Console.WriteLine("Argument is required");
        }
        else
        {
            long inputFile = Convert.ToInt64(args[0]);
            Thread.Sleep(2000);
            while (Console.KeyAvailable)
                Console.ReadKey(true);
            Console.Write("Press any key to stop...");
            while (!Console.KeyAvailable)
                Thread.Sleep(2000);
            Console.ReadKey(true);
        }
    }

}

