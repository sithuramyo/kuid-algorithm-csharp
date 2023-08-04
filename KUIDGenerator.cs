using System;

namespace UniqueIdAlgorithms
{
    public class KUIDGenerator
    {
        private static readonly object LockObject = new object { };
        private long lastTimestamp = -1;
        private long sequence = 0;
        private int uniqueIdCounter = 0; // Simple counter for demonstration

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
                int positiveKuid = (kuid & 0x7FFFFFFF) % 90000 + 10000;
                return positiveKuid;
            }
        }


        public long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
