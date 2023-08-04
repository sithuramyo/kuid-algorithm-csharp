using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Xml.Linq;
namespace UniqueIdAlgorithms
{
    public class KUIDGenerator
    {
        //LockObject line is defining a synchronization object (a lock) named LockObject.
        //This object is used for thread synchronization to ensure that the critical section of code within the GenerateKuid() method is executed by only one thread at a time.
        //This is important to maintain the consistency and correctness of the generated IDs when multiple threads are involved.
        private static readonly object LockObject = new object { };

        //lastTimestamp variable is used to track the most recent timestamp that was used for generating an ID.
        //Its purpose is to ensure that IDs generated in the same millisecond are assigned unique values
        //by utilizing the sequence component of the ID.
        private long lastTimestamp = -1;

        //sequence variable is used to prevent conflicts between multiple IDs generated within the same millisecond.
        //It's a common technique in ID generation systems to include
        //a sequence number to ensure that even if multiple IDs are generated in the same millisecond.
        private long sequence = 0;

        // Simple counter for demonstration
        private int uniqueIdCounter = 0;
        private const long Twepoch = 99999L;

        public int GenerateKuid()
        {
            lock (LockObject)
            {
                long timestamp = GetCurrentTimestamp();
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("Clock moved backwards! Wait a moment.");
                }
                if (timestamp == lastTimestamp)
                {
                    sequence = (sequence + 1) & 4095;
                }
                if (sequence == 0)
                {
                    timestamp = WaitNextmillis(lastTimestamp);
                }
                else
                {
                    sequence = 0;
                }
                lastTimestamp = timestamp;

                //timestamp is the current timestamp in milliseconds.
                //Twepoch is a constant value representing a starting timestamp.
                //This subtraction calculates the difference between the current timestamp and the Twepoch timestamp.
                //((timestamp - Twepoch) << 22):

                //The result of the previous subtraction is shifted left by 22 bits.
                //Shifting left by 22 bits effectively multiplies the result by 2 ^ 22, effectively moving the timestamp difference to the leftmost bits.
                //(sequence << 10):

                //sequence is a value that increments within the same millisecond to ensure unique IDs.
                //Shifting the sequence value left by 10 bits effectively multiplies it by 2 ^ 10, moving it to the middle bits.
                //GetUniqueId():

                //This method generates a unique identifier, usually an incrementing counter.
                //(((timestamp - Twepoch) << 22) | (sequence << 10) | GetUniqueId()):

                //The results from steps 2, 3, and 4 are combined using bitwise OR (|) operations.
                //This creates a single value that combines the timestamp difference, sequence, and the unique identifier.
                //(int)(((timestamp - Twepoch) << 22) | (sequence << 10) | GetUniqueId()):

                //The final combined value is cast to an int data type.
                //This is likely done to ensure the value fits within the 32 - bit integer range.
                int kuid = (int)(((timestamp - Twepoch) << 22) | (sequence << 10) | UniqueIdCounter());



                //Format for 5 digit.U can remove or increase 0 to 90000 + 10000 (In this case u need to plus 100000 because


                //it's only 900000 u can get 5 digit key or 5 digit key).
                int Kuid = (kuid & 0x7FFFFFFF) % 90000 + 10000;

                //it's only 900000 u can get 5 digit key or 6 digit key).
                //int Kuid = (kuid & 0x7FFFFFFF) % 900000 + 100000;
                return Kuid;
            }
        }


        public long GetCurrentTimestamp()
        {
            //Default timestamp got 13 digit so if we can output random sorting uid in 0.1 sec for 5 loop. 
            //But timestamp is below than 13 output can be ascending uid in 0.1 sec for 5 loop. 
            //loop can be ur option.

            //For 13 digit timestamp.
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            //For 9 digit timestamp.
            //return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 10000;
        }

        public long WaitNextmillis(long lastTimestamp)
        {
            long timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }

        //Replace with your logic.
        private int UniqueIdCounter()
        {
            return uniqueIdCounter++;
        }
    }
}
