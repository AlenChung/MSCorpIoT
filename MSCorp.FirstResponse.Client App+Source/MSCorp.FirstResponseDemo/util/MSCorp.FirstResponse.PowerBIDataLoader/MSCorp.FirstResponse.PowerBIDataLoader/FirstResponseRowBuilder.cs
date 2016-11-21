using System;
using MSCorp.FirstResponse.PowerBIDataLoader.model;

namespace MSCorp.FirstResponse.PowerBIDataLoader
{
    public class FirstResponseRowBuilder
    {
        private readonly Random _random;

        public FirstResponseRowBuilder(Random random)
        {
            _random = random;
            PriorityIncidents = Int32.MinValue;
            AverageResponseTime = Int32.MinValue;
            UnassignedTickets = Int32.MinValue;
        }

        public int UnassignedTickets { get; set; }

        public int AverageResponseTime { get; set; }

        public int PriorityIncidents { get; set; }

        public FirstResponseRow Build()
        {
            //initial setup
            if (PriorityIncidents == Int32.MinValue)
            {
                PriorityIncidents = _random.Next(5, 10);
                AverageResponseTime = _random.Next(9, 14);
                UnassignedTickets = _random.Next(7, 13);

                return new FirstResponseRow
                {
                    AverageResponseTime = AverageResponseTime,
                    UnassignedTickets = UnassignedTickets,
                    PriorityIncidents = PriorityIncidents, 
                    IsInitialSeed = true
                };

            }
            var nextPriorityIncidents = _random.Next(5, 10);
            var nextAverageResponseTime = _random.Next(9, 14);
            var nextUnassignedTickets = _random.Next(7, 13);

            var delta = new FirstResponseRow
            {
                AverageResponseTime = nextAverageResponseTime - AverageResponseTime,
                UnassignedTickets = nextUnassignedTickets - UnassignedTickets,
                PriorityIncidents = nextPriorityIncidents - PriorityIncidents,
                IsInitialSeed = false
            };

            //Update the current known count.
            AverageResponseTime = nextAverageResponseTime;
            UnassignedTickets = nextUnassignedTickets;
            PriorityIncidents = nextPriorityIncidents;

            return delta;
        }
    }
}