using System;

namespace UniqueIdAlgorithms
{
    public class Program
    {
        public static void Main(string[] args)
        {

            KUIDGenerator kuidGenerator = new KUIDGenerator();
            for (int i = 0; i < 5; i++)
            {
                int uniqueId = kuidGenerator.GenerateKuid();
                Console.WriteLine($"Generated Unique ID: STRM{uniqueId}");
                Thread.Sleep(100);
            }
        }
    }
}
