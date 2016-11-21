using System;

namespace MSCorp.FirstResponse.AzureSearch.Commands.Helper
{
    public static class IntUtil
    {
        private static Random random;

        private static void Init()
        {
            if (random == null) random = new Random();
        }

        public static int Random(int min, int max)
        {
            Init();
            return random.Next(min, max);
        }
    }
}