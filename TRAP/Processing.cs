using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globalsettings;

namespace TRAP
{
    class Processing
    {
        /* Mean radius of the Earth */
        private const double EarthRadius = 6371000.0;   // metres
        /* Constant time factors. */
        private const double secPerHour = 3600;
        private const double secPerDay = 86400;
        private const double hoursPerDay = 24;
        private const double minutesPerHour = 60;
        private const double secPerMinute = 60;


        /// <summary>
        /// Convert degrees in to radians
        /// </summary>
        /// <param name="degrees">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        private double degress2radians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// </summary>
        /// <param name="latitude1">Latitude of location 1.</param>
        /// <param name="longitude1">Longitude of location 1.</param>
        /// <param name="latitude2">Latitude of location 2.</param>
        /// <param name="longitude2">Longitude of location 2.</param>
        /// <returns>The Distance between the two points in metres.</returns>
        public double calculateGreatCircleDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {

            double arcsine = Math.Sin(degress2radians((latitude2 - latitude1) / 2)) * Math.Sin(degress2radians((latitude2 - latitude1) / 2)) +
                Math.Cos(degress2radians(latitude1)) * Math.Cos(degress2radians(latitude2)) *
                Math.Sin(degress2radians((longitude2 - longitude1) / 2)) * Math.Sin(degress2radians((longitude2 - longitude1) / 2));
            double arclength = 2 * Math.Atan2(Math.Sqrt(arcsine), Math.Sqrt(1 - arcsine));

            return EarthRadius * arclength;

        }

        public double calculateGreatCircleDistance(GeoLocation point1, GeoLocation point2)
        {

            double arcsine = Math.Sin(degress2radians((point2.latitude - point1.latitude) / 2)) * Math.Sin(degress2radians((point2.latitude - point1.latitude) / 2)) +
                Math.Cos(degress2radians(point1.latitude)) * Math.Cos(degress2radians(point2.latitude)) *
                Math.Sin(degress2radians((point2.longitude - point1.longitude) / 2)) * Math.Sin(degress2radians((point2.longitude - point1.longitude) / 2));
            double arclength = 2 * Math.Atan2(Math.Sqrt(arcsine), Math.Sqrt(1 - arcsine));

            return EarthRadius * arclength;

        }

        /// <summary>
        /// Determine if the single train journey has a single direction. When the journey has 
        /// multilpe directions, the part of the journey that has the largest total length in
        /// a single direction is returned.
        /// </summary>
        /// <param name="journey">The complete train journey.</param>
        /// <param name="trackGeometry">The geometry of the track.</param>
        /// <returns>A list of train details objects describing the longest distance the train has 
        /// travelled in a single direction.</returns>
        public List<TrainJourney> longestDistanceTravelledInOneDirection(List<TrainJourney> journey, List<TrackGeometry> trackGeometry)
        {
            /* Set up intial conditions */
            double movingAverage = 0;
            double previousAverage = 0;
            double distance = 0;
            double increasingDistance = 0;
            double decreasingDistance = 0;

            int start, end;
            int newStart, count;
            double maxValue = 0;

            /* Create lists to add each journey for each change in direction. */
            List<double> distances = new List<double>();
            List<int> startIdx = new List<int>();
            List<int> endIdx = new List<int>();

            /* Set the number of points to average over. */
            int numPoints = 10;

            if (journey.Count <= numPoints)
                return journey;

            /* Set the kmPosts to the closest points on the geometry alignment. */
            TRAP.track.matchTrainLocationToTrackGeometry(journey, trackGeometry);

            start = 0;

            for (int journeyIdx = 0; journeyIdx < journey.Count() - numPoints; journeyIdx++)
            {
                /* Calculate the moving average of the kmposts ahead of current position. */
                distance = journey[journeyIdx + numPoints].kmPost - journey[journeyIdx].kmPost;
                movingAverage = distance / numPoints;

                /* Check the direction has not changed. */
                if (Math.Sign(movingAverage) == Math.Sign(previousAverage) || Math.Sign(movingAverage) == 0 || Math.Sign(previousAverage) == 0)
                {
                    /* Increment the assumed distance travelled in current direction. */
                    if (movingAverage > 0)
                        increasingDistance = increasingDistance + movingAverage;

                    else if (movingAverage < 0)
                        decreasingDistance = decreasingDistance - movingAverage;

                }
                else
                {
                    /* There has been a change in direction. */
                    end = journeyIdx;

                    /* Add the total distance achieved from the previous km posts to the list. */
                    if (previousAverage > 0)
                    {
                        distances.Add(increasingDistance);
                        startIdx.Add(start);
                        endIdx.Add(end);
                        increasingDistance = 0;
                    }
                    else if (previousAverage < 0)
                    {
                        distances.Add(decreasingDistance);
                        startIdx.Add(start);
                        endIdx.Add(end);
                        decreasingDistance = 0;
                    }

                    /* Reset the new start postion. */
                    start = journeyIdx++;
                }

                previousAverage = movingAverage;

            }

            /* Add the last total distance achieved to the list. */
            end = journey.Count() - 1;
            if (previousAverage > 0)
            {
                distances.Add(increasingDistance);
                startIdx.Add(start);
                endIdx.Add(end);
            }
            else if (previousAverage < 0)
            {
                distances.Add(decreasingDistance);
                startIdx.Add(start);
                endIdx.Add(end);
            }
            else
            {
                /* Condition when last average is 0, determine which total to add to the list. */
                if (increasingDistance > decreasingDistance)
                {
                    distances.Add(increasingDistance);
                    startIdx.Add(start);
                    endIdx.Add(end);
                }
                else
                {
                    distances.Add(decreasingDistance);
                    startIdx.Add(start);
                    endIdx.Add(end);
                }
            }

            if (distances.Count() == 1)
                return journey;

            /* Determine the largest distance to return that section of the journey */
            maxValue = distances.Max();
            int index = distances.ToList().IndexOf(maxValue);
            newStart = startIdx[index];
            count = endIdx[index] - newStart + 1;

            /* Return the part of the journey that has the largest total length in a single direction. */
            return journey.GetRange(newStart, count);

        }

        /// <summary>
        /// Calcualte the single train journey length.
        /// </summary>
        /// <param name="journey">The journey points for the train.</param>
        /// <returns>The total distance travelled in metres.</returns>
        public double calculateTrainJourneyDistance(List<TrainJourney> journey)
        {
            double distance = 0;

            for (int pointIdx = 1; pointIdx < journey.Count; pointIdx++)
            {
                /* Create the conequtive points */
                GeoLocation point1 = journey[pointIdx - 1].location;
                GeoLocation point2 = journey[pointIdx].location;

                /* Calcualte the great circle distance. */
                distance = distance + calculateGreatCircleDistance(point1, point2);
            }

            return distance;

        }

        /// <summary>
        /// Function determines the direction of the train using the first and last km posts.
        /// </summary>
        /// <param name="train">A train object containing kmPost information</param>
        /// <returns>Enumerated direction of the train km's.</returns>
        public direction getTrainDirection(Train train)
        {
            /* NOTE: This function does not take into account any train journey data that have 
             * multiple changes of direction. This should not be seen when the 'Cleaned Data' 
             * is deleviered by Enetrprise services.
             * This is currently corrected for in longestDistanceTravelledInOneDirection()
             */

            /* Determine the distance and sign from the first point to the last point */
            double journeyDistance = train.journey[train.journey.Count() - 1].kmPost - train.journey[0].kmPost;

            if (journeyDistance > 0)
                return direction.increasing;
            else
                return direction.decreasing;


        }

        /// <summary>
        /// Populate the geometry km information based on the calculated distance from the first km post.
        /// </summary>
        /// <param name="train">A train object.</param>
        public void populateGeometryKm(List<TrainJourney> journey, List<TrackGeometry> trackGeometry)
        {
            /* Determine the direction of the km's the train is travelling. */
            GeoLocation trainPoint = new GeoLocation();

            /* The first km point is populated by the parent function ICEData.CleanData(). */
            for (int journeyIdx = 0; journeyIdx < journey.Count(); journeyIdx++)
            {
                /* Find the kilometerage of the closest point on the track and associate it with the current train location.*/
                trainPoint = new GeoLocation(journey[journeyIdx]);
                journey[journeyIdx].kilometreage = TRAP.track.findClosestTrackGeometryPoint(trackGeometry, trainPoint);
                /* This method reduces any error in the actual klometreage when
                 * calculating the straight line distance over a few kilometres.
                 */
            }


        }

        /// <summary>
        /// Populate the loop location information for each point in the train journey.
        /// </summary>
        /// <param name="train">A train object containing the journey details.</param>
        /// <param name="trackGeometry">The track geometry object indicating the location of the loops.</param>
        public void populateLoopLocations(List<TrainJourney> trainJourney, List<TrackGeometry> trackGeometry)
        {
            /* Create a track geometry object. */
            TrackGeometry track = new TrackGeometry();
            int index = 0;
            double trainPoint = 0;

            /* Cycle through the train journey. */
            foreach (TrainJourney journey in trainJourney)
            {
                trainPoint = journey.kilometreage;
                /* Find the index of the closest point on the track to the train. */
                index = track.findClosestTrackGeometryPoint(trackGeometry, trainPoint);
                /* Populate the loop */
                journey.isLoopHere = trackGeometry[index].isLoopHere;

            }

        }

        /// <summary>
        /// populate the temporary speed restriction information for each train journey.
        /// </summary>
        /// <param name="train">A train object containing the journey details.</param>
        /// <param name="trackGeometry">the track Geometry object indicating the TSR information at each location.</param>
        //public void populateTemporarySpeedRestrictions(Train train, List<TrackGeometry> trackGeometry, List<TSRObject> TSRs)
        //{
        //    /* Create a track geometry object. */
        //    TrackGeometry track = new TrackGeometry();
        //    //int index = 0;
        //    double trainPoint = 0;

        //    /* Cycle through the train journey. */
        //    foreach (TrainDetails journey in train.TrainJourney)
        //    {
        //        /* Extract the current point in the journey */
        //        trainPoint = journey.geometryKm;

        //        /* Cycle through each TSR. */
        //        foreach (TSRObject TSR in TSRs)
        //        {
        //            if (trainPoint >= TSR.startKm && trainPoint <= TSR.endKm)
        //            {
        //                if (journey.NotificationDateTime >= TSR.IssueDate && journey.NotificationDateTime <= TSR.LiftedDate)
        //                {
        //                    /* When the train is within the applicable TSR, add it to the journey. */
        //                    journey.isTSRHere = true;
        //                    journey.TSRspeed = TSR.TSRSpeed;
        //                }
        //            }

        //            /* When a TSR is applicable, break out of the current loop and continue with the rest of the journey. */
        //            if (journey.isTSRHere)
        //                break;
        //        }
        //    }
        //}

        /// <summary>
        /// Linear interpolation to a target point.
        /// </summary>
        /// <param name="targetX">Target invariant location to be interpolated to.</param>
        /// <param name="X0">Lower invariant position to interpolate between.</param>
        /// <param name="X1">Upper invariant position to interpolate between.</param>
        /// <param name="Y0">Lower variant to interpolate between.</param>
        /// <param name="Y1">Upper variant to interpolate between.</param>
        /// <returns>The interpolate variant value at the target invariant location.</returns>
        private double linear(double targetX, double X0, double X1, double Y0, double Y1)
        {
            /* Take the average when the invariant location does not change. */
            if ((X1 - X0) == 0)
                return (Y0 + Y1) / 2;

            return Y0 + (targetX - X0) * (Y1 - Y0) / (X1 - X0);

        }


        /// <summary>
        ///  Interpolate the train speed to a specified interval using a linear interpolation.
        /// </summary>
        /// <param name="trains">List of train objects containing the parameters for each train journey.</param>
        /// <param name="trackGeometry">The list of Track geometry data to align the train location.</param>
        /// <returns>List of train objects with interpolated values at the specified interval.</returns>
        public List<Train> interpolateTrainData(List<Train> trains, List<TrackGeometry> trackGeometry)
        {
            /* Placeholders for the interpolated distance markers. */
            double previousKm = 0;
            double currentKm = 0;
            /* Place holder to calculate the time for each interpolated value. */
            DateTime time = new DateTime();
            /* Flag to indicate when to collect the next time value. */
            bool timeChange = true;

            /* Additional loop and TSR details. */
            int geometryIdx = 0;
            bool loop = false;
            bool TSR = false;
            double TSRspeed = 0;

            /* Index values for the interpolation parameters */
            int index0 = -1;
            int index1 = -1;

            /* Interplation parameters. */
            double interpolatedSpeed = 0;
            double X0, X1, Y0, Y1;

            /* Create a new list of trains for the journies interpolated values. */
            List<Train> newTrainList = new List<Train>();
            /* Create a journey list to store the existing journey details. */
            //List<TrainJourney> journey = new List<TrainJourney>();

            /* Cycle through each train to interpolate between points. */
            for (int trainIdx = 0; trainIdx < trains.Count(); trainIdx++)
            {

                /* Create a new journey list of interpolated values. */
                List<TrainJourney> interpolatedJourney = new List<TrainJourney>();

                List<TrainJourney> journey = trains[trainIdx].journey;

                /* Set the start of the interpolation. */
                currentKm = Settings.startKm;
                previousKm = currentKm;

                while (currentKm < Settings.endKm)
                {

                    /* Find the closest kilometerage markers either side of the current interpolation point. */
                    index0 = findClosestLowerKm(currentKm, journey);
                    index1 = index0 + 1; // findClosestGreaterKm(currentKm, interpolatedJourney);

                    /* If a valid index is found, extract the existing journey parameters and interpolate. */
                    if (index0 >= 0 && index1 >= 0)
                    {
                        X0 = journey[index0].kilometreage;
                        X1 = journey[index1].kilometreage;
                        Y0 = journey[index0].speed;
                        Y1 = journey[index1].speed;
                        if (timeChange)
                        {
                            time = journey[index0].dateTime;
                            timeChange = false;
                        }

                        /* Perform linear interpolation. */
                        interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);
                        /* Interpolate the time. */
                        time = time.AddHours(calculateTimeInterval(previousKm, currentKm, interpolatedSpeed));

                    }
                    else
                    {
                        /* Boundary conditions for interpolating the data prior to and beyond the existing journey points. */
                        time = journey.Where(t => t.dateTime > DateTime.MinValue).Min(t => t.dateTime); // DateTime.MinValue;
                        interpolatedSpeed = 0;
                    }

                    geometryIdx = trackGeometry[0].findClosestTrackGeometryPoint(trackGeometry, currentKm);

                    if (geometryIdx >= 0)
                    {
                        /* Check if there is a loop at this location. */
                        loop = trackGeometry[geometryIdx].isLoopHere;

                        /* Check if there is a TSR at this location. */
                        TSR = trackGeometry[geometryIdx].isTSRHere;
                        TSRspeed = trackGeometry[geometryIdx].temporarySpeedRestriction;
                    }
                   
                    /* Create the interpolated data object and add it to the list. */
                    TrainJourney item = new TrainJourney(time, interpolatedSpeed, currentKm, currentKm, trackGeometry[geometryIdx].elevation, loop, TSR);
                    interpolatedJourney.Add(item);

                    /* Create a copy of the current km marker and increment. */
                    previousKm = currentKm;
                    currentKm = currentKm + Settings.interval / 1000;

                    /* Determine if we need to extract the time from the data or interpolate it. */
                    if (index1 >= 0)
                        if (currentKm >= journey[index1].kilometreage)
                            timeChange = true;

                }

                /* Add the interpolated list to the list of new train objects. */
                Train trainItem = new Train(trains[trainIdx].trainID, trains[trainIdx].locoID, 
                    trains[trainIdx].trainOperator, trains[trainIdx].commodity, trains[trainIdx].powerToWeight, 
                    interpolatedJourney, trains[trainIdx].trainDirection);
                    
                newTrainList.Add(trainItem);

            }

            /* Return the completed interpolated train data. */
            return newTrainList;
        }

        /// <summary>
        /// Find the index of the closest kilometerage that is less than the target point.
        /// </summary>
        /// <param name="target">The target kilometerage.</param>
        /// <param name="journey">The list of train details containig the journey parameters.</param>
        /// <returns>The index of the closest point that is less than the target point. 
        /// Returns -1 if a point does not exist.</returns>
        private int findClosestLowerKm(double target, List<TrainJourney> journey)
        {
            /* Set the initial values. */
            double minimum = double.MaxValue;
            double difference = double.MaxValue;
            int index = 0;

            /* Cycle through the journey parameters. */
            for (int journeyIdx = 0; journeyIdx < journey.Count(); journeyIdx++)
            {
                /* Find the difference if the value is lower. */
                if (journey[journeyIdx].kilometreage < target)
                    difference = Math.Abs(journey[journeyIdx].kilometreage - target);

                /* Find the minimum difference. */
                if (difference < minimum)
                {
                    minimum = difference;
                    index = journeyIdx;
                }

            }

            if (difference == double.MaxValue)
                return -1;

            return index;
        }

        /// <summary>
        /// Calculate the time interval between two locations based on the speed.
        /// </summary>
        /// <param name="startPositon">Starting kilometreage.</param>
        /// <param name="endPosition">Final kilometreage.</param>
        /// <param name="speed">Average speed between locations.</param>
        /// <returns>The time taken to traverse the distance in hours.</returns>
        private double calculateTimeInterval(double startPositon, double endPosition, double speed)
        {

            if (speed > 0)
                return Math.Abs(endPosition - startPositon) / speed;    // hours.
            else
                return 0;
        }



    } // Class Processing
}
