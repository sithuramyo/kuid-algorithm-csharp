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

                int kuid = (int)(((timestamp - Twepoch) << 22) | (sequence << 10) | GetUniqueId());

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

        private int GetUniqueId()
        {
            return uniqueIdCounter++;
        }
    }
}
