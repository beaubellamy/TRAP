﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAP
{
    class Statistics
    {

        public string catagory;
        public double numberOfTrains;
        public double averageDistanceTravelled;
        public double averageSpeed;
        public double averagePowerToWeightRatio;
        public double standardDeviationP2W;

        /// <summary>
        /// Default Statisitcs Constructor
        /// </summary>
        public Statistics()
        {
        }

        /// <summary>
        /// Calculates the statistics of the list of trains passed in.
        /// </summary>
        /// <param name="trains">A list of train objects.</param>
        public static Statistics generateStats(List<Train> trains)
        {
            Statistics stats = new Statistics();

            stats.catagory = trains[0].catagory.ToString()+ " " + trains[0].trainDirection.ToString();

            /* Extract the number of trains in the list */
            stats.numberOfTrains = trains.Count();

            List<double> distance = new List<double>();
            List<double> speed = new List<double>();
            List<double> power2Weight = new List<double>();

            /* Cycle through all the trains. */
            foreach (Train train in trains)
            {
                /* Calculate the distance travelled for each train */
                double distanceTravelled = 0;
                if (train.journey.Where(t => t.speed > 0).Count() != 0)
                {
                    distanceTravelled = (train.journey.Where(t => t.speed > 0).Max(t => t.kilometreage) - train.journey.Where(t => t.speed > 0).Min(t => t.kilometreage));
                    /* Calculate the average speed of the train journey. */
                    speed.Add(train.journey.Where(t => t.speed > 0).Average(t => t.speed));
                }

                distance.Add(distanceTravelled);

                /* Add the power to weight ratio to the list. */
                power2Weight.Add(train.powerToWeight);

            }

            /* Calculate the averages. */
            if (speed.Count() > 0)
                stats.averageSpeed = speed.Average();
            else
                stats.averageSpeed = 0;

            if (distance.Count() > 0)
                stats.averageDistanceTravelled = distance.Average();
            else
                stats.averageDistanceTravelled = 0;

            if (power2Weight.Count() > 0)
            {
                stats.averagePowerToWeightRatio = power2Weight.Average();
                /* Calculate the standard deviation of the power to weight ratios. */
                stats.standardDeviationP2W = Math.Sqrt(power2Weight.Average(v => Math.Pow(v - stats.averagePowerToWeightRatio, 2)));
            }
            else
            {
                stats.averagePowerToWeightRatio = 0;
                stats.standardDeviationP2W = 0;
            }

            return stats;
        }

        
    }
}