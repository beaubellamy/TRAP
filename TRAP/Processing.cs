using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globalsettings;

namespace TRAP
{
    public class Processing
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

        /// <summary>
        /// Calculate the shortes distance between two geographical locations using the great circle formula.
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

        /// <summary>
        /// Calculate the shortes distance between two geographical locations using the great circle formula.
        /// </summary>
        /// <param name="point1">The geographic location of the first point.</param>
        /// <param name="point2">The geographic location of the second point.</param>
        /// <returns></returns>
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
            TrainPerformance.track.matchTrainLocationToTrackGeometry(journey, trackGeometry);
            

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
                return direction.IncreasingKm;
            else
                return direction.DecreasingKm;


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
                journey[journeyIdx].kilometreage = TrainPerformance.track.findClosestTrackGeometryPoint(trackGeometry, trainPoint);
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
        /// This function cycles through each train and determines if a TSR had applied to any part of the journey.
        /// </summary>
        /// <param name="trains">A list of trains containing the journey for each.</param>
        /// <param name="TSRs">A list of TSR objects.</param>
        public void populateAllTrainsTemporarySpeedRestrictions(List<Train> trains, List<TSRObject> TSRs)
        {

            foreach (Train train in trains)
            {
                int tsrIndex = 0;

                foreach (TrainJourney journey in train.journey)
                {
                    /* Establish the TSR that applies to the train position. */
                    if (journey.kilometreage > TSRs[tsrIndex].endKm && tsrIndex < TSRs.Count() - 1)
                        tsrIndex++;

                    /* Determine if the TSR is applicable to the train by location and date. */
                    if (journey.kilometreage >= TSRs[tsrIndex].startKm && journey.kilometreage <= TSRs[tsrIndex].endKm &&
                        journey.dateTime >= TSRs[tsrIndex].IssueDate && journey.dateTime <= TSRs[tsrIndex].LiftedDate)
                    {
                        journey.isTSRHere = true;                        
                    }
                }
            }
        }


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
                    index1 = findClosestGreaterKm(currentKm, journey);

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
                Train trainItem = new Train(trains[trainIdx].catagory,trains[trainIdx].trainID, trains[trainIdx].locoID, 
                    trains[trainIdx].trainOperator, trains[trainIdx].commodity, trains[trainIdx].powerToWeight, 
                    interpolatedJourney, trains[trainIdx].trainDirection);
                    
                newTrainList.Add(trainItem);

            }

            /* Return the completed interpolated train data. */
            return newTrainList;
        }

        /// <summary>
        /// Calculate the aggregated average speed of all trains.
        /// </summary>
        /// <param name="trains">A list of trains to be aggregated in a single group.</param>
        /// <param name="catagorySim">The simulted train for the specified analysis catagory.</param>
        /// <param name="trackGeometry">The track alignment information for the train journey.</param>
        /// <returns>An average train containing information about the average speed at each location.</returns>
        public AverageTrain averageTrain(List<Train> trains, List<TrainJourney> catagorySim, List<TrackGeometry> trackGeometry)
        {

            bool loopBoundary = false;
            bool TSRBoundary = false;
            List<bool> TSRList = new List<bool>();

            /* Set up the average train journey lists. */
            List<double> kilometreage = new List<double>();
            List<double> elevation = new List<double>();
            List<double> averageSpeed = new List<double>();
            List<bool> isInLoopBoundary = new List<bool>();
            List<bool> isInTSRboundary = new List<bool>();


            double kmPost = 0;
            double altitude = 0;
            List<double> speed = new List<double>();
            double sum = 0;
            double aveSpeed = 0;

            /* Determine the number of points in the average train journey. */
            int size = (int)((Settings.endKm - Settings.startKm) / (Settings.interval / 1000));

            TrainJourney journey = new TrainJourney();

            /* Cycle through each location to average the valid values. */
            for (int journeyIdx = 0; journeyIdx < size; journeyIdx++)
            {
                /* Determine the current location and elevation of the alignemnt at this point. */
                kmPost = Settings.startKm + Settings.interval / 1000 * journeyIdx;
                altitude = trackGeometry[TrainPerformance.track.findClosestTrackGeometryPoint(trackGeometry, kmPost)].elevation;

                speed.Clear();
                sum = 0;

                /* Cycle through each train in the list. */
                foreach (Train train in trains)
                {
                    
                    journey = train.journey[journeyIdx];

                    /* Does a TSR apply */
                    if (!withinTemporarySpeedRestrictionBoundaries(train, journey.kilometreage))
                    {
                        TSRList.Add(false);
                        /* Is the train within a loop boundary */
                        if (!isTrainInLoopBoundary(train, journey.kilometreage))
                        {// train is NOT in loop
                            loopBoundary = false;

                            speed.Add(journey.speed);
                            sum = sum + journey.speed;
                        }
                        else
                        {// train is IN loop
                            loopBoundary = true;

                            if (journey.speed > (Settings.loopSpeedThreshold * catagorySim[journeyIdx].speed))
                            {
                                speed.Add(journey.speed);
                                sum = sum + journey.speed;
                            }

                        }

                    }
                    else
                    {
                        TSRList.Add(true);

                        /* We dont want to include the speed in the aggregation if the train is within the
                         * bundaries of a TSR and is forced to slow down.  
                         */

                    }

                }

                /* Identify where the TSR's were applied for the average train. */
                int TSRtrue = TSRList.Where(t => t == true).Count();
                if (TSRtrue > 0)
                    TSRBoundary = true;
                else
                    TSRBoundary = false;    

                /* If the TSR applied for the whole analysis period, the simulation speed is used. */
                if (TSRtrue == TSRList.Count())
                    aveSpeed = catagorySim[journeyIdx].speed;
                else
                {
                    /* Calculate the average speed at each location. */
                    if (speed.Count() == 0 || sum == 0)
                        aveSpeed = 0;
                    else
                        aveSpeed = speed.Where(x => x > 0.0).Average();
                }

                /* Add to each list for this location. */
                kilometreage.Add(kmPost);
                elevation.Add(altitude);
                averageSpeed.Add(aveSpeed);
                isInLoopBoundary.Add(loopBoundary);
                isInTSRboundary.Add(TSRBoundary);


            }
            
            /* Create the new average train object. */
            AverageTrain averageTrain = new AverageTrain(trains[0].catagory, trains[0].trainDirection, trains.Count() ,kilometreage, elevation, averageSpeed, isInLoopBoundary, isInTSRboundary);

            return averageTrain;

        }

        /// <summary>
        /// Calculate the weighted average of all simulations. This simulation is then used for 
        /// comparison when calculating the combined catagories (weighted average train).
        /// </summary>
        /// <param name="simulations">List of simulations for each catagory analysed.</param>
        /// <param name="averageTrains">A List of the average train data, only used for the weighting.</param>
        /// <returns>A list of average train data describing the combined catagories.</returns>
        public static List<Train> getWeightedAverageSimulation(List<Train> simulations, List<AverageTrain> averageTrains)
        {
            /* The list of trains (2) that will be returned */
            List<Train> weightedAvergeTrain = new List<Train>();

            /* The denominator for the weighting calulations */
            int increasingTrainCount = averageTrains.Where(t => t.direction == direction.IncreasingKm).Select(t => t.trainCount).Sum(); 
            int decreasingTrainCount = averageTrains.Where(t => t.direction == direction.DecreasingKm).Select(t => t.trainCount).Sum();

            List<TrainJourney> increasingJourney = new List<TrainJourney>();    
            List<TrainJourney> decreasingJourney = new List<TrainJourney>();
            
            double speedIncreasing = 0;
            double speedDecreasing = 0;

            /* If there are only two simulations, there is no need to calculate weighting */
            if (simulations.Count() == 2)
                return simulations;

            for (int journeyIdx = 0; journeyIdx < simulations[0].journey.Count();  journeyIdx++ )
            {
                /* The journey for each simulation should be the same length, So loop 
                 * through the journey 
                 */
                if (simulations.Count() == 4)
                {
                    /* Assumes 2 individual catagories for increasing and decreasing directions. */
                    speedIncreasing = (simulations[0].journey[journeyIdx].speed * averageTrains[0].trainCount +
                        simulations[2].journey[journeyIdx].speed * averageTrains[2].trainCount) / increasingTrainCount;

                    speedDecreasing = (simulations[1].journey[journeyIdx].speed * averageTrains[1].trainCount +
                        simulations[3].journey[journeyIdx].speed * averageTrains[3].trainCount) / decreasingTrainCount;
                }
                else if (simulations.Count() == 6)
                {
                    /* Assumes 3 individual catagories for increasing and decreasing directions. */
                    speedIncreasing = (simulations[0].journey[journeyIdx].speed * averageTrains[0].trainCount +
                           simulations[2].journey[journeyIdx].speed * averageTrains[2].trainCount +
                           simulations[4].journey[journeyIdx].speed * averageTrains[4].trainCount) / increasingTrainCount;

                    speedDecreasing = (simulations[1].journey[journeyIdx].speed * averageTrains[1].trainCount +
                        simulations[3].journey[journeyIdx].speed * averageTrains[3].trainCount +
                        simulations[5].journey[journeyIdx].speed * averageTrains[5].trainCount) / decreasingTrainCount;
                }
                else
                {
                    /* Assumes 4 individual catagories for increasing and decreasing directions. */
                    speedIncreasing = (simulations[0].journey[journeyIdx].speed * averageTrains[0].trainCount +
                           simulations[2].journey[journeyIdx].speed * averageTrains[2].trainCount +
                           simulations[4].journey[journeyIdx].speed * averageTrains[4].trainCount +
                           simulations[6].journey[journeyIdx].speed * averageTrains[6].trainCount) / increasingTrainCount;

                    speedDecreasing = (simulations[1].journey[journeyIdx].speed * averageTrains[1].trainCount +
                        simulations[3].journey[journeyIdx].speed * averageTrains[3].trainCount +
                        simulations[5].journey[journeyIdx].speed * averageTrains[5].trainCount +
                        simulations[7].journey[journeyIdx].speed * averageTrains[7].trainCount) / decreasingTrainCount;
                }
                
                /* Assumed the same properties as the existing simulations */
                GeoLocation location = simulations[0].journey[journeyIdx].location;
                DateTime increasingTime = simulations[0].journey[journeyIdx].dateTime;
                DateTime decreasingTime = simulations[1].journey[journeyIdx].dateTime;
                double kilometreage = simulations[0].journey[journeyIdx].kilometreage;
                double elevation = simulations[0].journey[journeyIdx].elevation;

                /* Add to the journey for the increasing weighted average */
                TrainJourney itemIncreasing = new TrainJourney(location,increasingTime,speedIncreasing,kilometreage,kilometreage, elevation);
                increasingJourney.Add(itemIncreasing);

                /* Add to the journey for the decreasing weighted average */
                TrainJourney itemDecreasing = new TrainJourney(location, decreasingTime, speedDecreasing, kilometreage, kilometreage, elevation);
                decreasingJourney.Add(itemDecreasing);
            }

            /* Add the weighted average trains to the list */
            Train itemInc = new Train(increasingJourney, catagory.Simulated, direction.IncreasingKm);
            weightedAvergeTrain.Add(itemInc);
            Train itemDec = new Train(decreasingJourney, catagory.Simulated, direction.DecreasingKm);
            weightedAvergeTrain.Add(itemDec);
            
            return weightedAvergeTrain;
        }

        /// <summary>
        /// Determine if the train is approaching, leaving or within a loop.
        /// </summary>
        /// <param name="train">The train object containing the journey details.</param>
        /// <param name="targetLocation">The specific location being considered.</param>
        /// <returns>True, if the train is within the boundaries of the loop window.</returns>
        public bool isTrainInLoopBoundary(Train train, double targetLocation)
        {

            /* Find the indecies of the boundaries of the loop. */
            double lookBack = targetLocation - Settings.loopBoundaryThreshold;
            double lookForward = targetLocation + Settings.loopBoundaryThreshold;
            int lookBackIdx = train.indexOfgeometryKm(train.journey, lookBack);
            int lookForwardIdx = train.indexOfgeometryKm(train.journey, lookForward);

            /* Check the indecies are valid */
            if (lookBack < Settings.startKm && lookBackIdx == -1)
                lookBackIdx = 0;
            if (lookForward > Settings.endKm && lookForwardIdx == -1)
            {
                if (train.trainDirection == direction.IncreasingKm)
                    lookForwardIdx = train.journey.Count() - 1;
                else
                    lookForwardIdx = 0;
            }

            /* Determine if a loop is within the loop window of the current position. */
            if (lookBackIdx >= 0 && lookForwardIdx >= 0)
            {
                for (int journeyIdx = lookBackIdx; journeyIdx < lookForwardIdx; journeyIdx++)
                {
                    TrainJourney journey = train.journey[journeyIdx];

                    if (journey.isLoopHere)
                        return true;

                }
            }
            return false;
        }

        /// <summary>
        /// Determine the properties of the TSR if one applies.
        /// </summary>
        /// <param name="train">The train object containing the journey details.</param>
        /// <param name="targetLocation">The specific location being considered.</param>
        /// <returns>TSR object containting the TSR flag and the associated speed. </returns>
        public bool withinTemporarySpeedRestrictionBoundaries(Train train, double targetLocation)
        {

            bool isTSRHere = false;

            /* Find the indecies of the boundaries of the loop. */
            double lookBack = targetLocation - Settings.TSRwindowBoundary;
            double lookForward = targetLocation + Settings.TSRwindowBoundary;
            int lookBackIdx = train.indexOfgeometryKm(train.journey, lookBack);
            int lookForwardIdx = train.indexOfgeometryKm(train.journey, lookForward);

            /* Check the indecies are valid */
            if (lookBack < Settings.startKm && lookBackIdx == -1)
                lookBackIdx = 0;
            if (lookForward > Settings.endKm && lookForwardIdx == -1)
                lookForwardIdx = train.journey.Count() - 1;

            /* Determine if a loop is within the loop window of the current position. */
            if (lookBackIdx >= 0 && lookForwardIdx >= 0)
            {
                for (int journeyIdx = lookBackIdx; journeyIdx < lookForwardIdx; journeyIdx++)
                {
                    TrainJourney journey = train.journey[journeyIdx];

                    if (journey.isTSRHere)
                        isTSRHere = true;
                }
            }
            return isTSRHere;
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
        /// Find the index of the closest kilometerage that is larger than the target point.
        /// </summary>
        /// <param name="target">The target kilometerage.</param>
        /// <param name="journey">The list of train details containig the journey parameters.</param>
        /// <returns>The index of the closest point that is larger than the target point. 
        /// Returns -1 if a point does not exist.</returns>
        private int findClosestGreaterKm(double target, List<TrainJourney> journey)
        {
            /* Set the initial values. */
            double minimum = double.MaxValue;
            double difference = double.MaxValue;
            int index = 0;

            /* Cycle through the journey parameters. */
            for (int journeyIdx = 0; journeyIdx < journey.Count(); journeyIdx++)
            {
                /* Find the difference if the value is lower. */
                if (journey[journeyIdx].kilometreage > target)
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

        /// <summary>
        /// Populate the Setting parameters from the form provided.
        /// </summary>
        /// <param name="form">The Form object containg the form parameters.</param>
        public void populateFormParameters(TrainPerformance form)
        {

            /* Extract the form parameters. */
            Settings.dateRange = form.getDateRange();
            Settings.topLeftLocation = form.getTopLeftLocation();
            Settings.bottomRightLocation = form.getBottomRightLocation();
            Settings.includeAListOfTrainsToExclude = form.getTrainListExcludeFlag();
            Settings.startKm = form.getStartKm();
            Settings.endKm = form.getEndKm();
            Settings.interval = form.getInterval();
            Settings.minimumJourneyDistance = form.getJourneydistance();
            Settings.loopSpeedThreshold = form.getLoopFactor();
            Settings.loopBoundaryThreshold = form.getLoopBoundary();
            Settings.TSRwindowBoundary = form.getTSRWindow();
            Settings.timeThreshold = form.getTimeSeparation();
            Settings.distanceThreshold = form.getDataSeparation();
            Settings.catagory1LowerBound = form.getCatagory1LowerBound();
            Settings.catagory1UpperBound = form.getCatagory1UpperBound();
            Settings.catagory2LowerBound = form.getCatagory2LowerBound();
            Settings.catagory2UpperBound = form.getCatagory2UpperBound();
            //Settings.combinedLowerBound = form.getUnderpoweredLowerBound();
            //Settings.combinedUpperBound = form.getOvderpoweredUpperBound();
            Settings.HunterValleyRegion = form.getHunterValleyRegion();
            Settings.analysisCatagory = form.getAnalysisCatagory();
            Settings.catagory1Commodity = form.getCommodity1Catagory();
            Settings.catagory1Operator = form.getOperator1Catagory();
            Settings.catagory2Commodity = form.getCommodity2Catagory();
            Settings.catagory2Operator = form.getOperator2Catagory();
            Settings.catagory3Commodity = form.getCommodity3Catagory();
            Settings.catagory3Operator = form.getOperator3Catagory();

        }

        /// <summary>
        /// Validate the form parameters are within logical boundaries.
        /// </summary>
        /// <returns></returns>
        public bool areFormParametersValid()
        {

            if (Settings.dateRange == null ||
                Settings.dateRange[0] > DateTime.Today || Settings.dateRange[1] > DateTime.Today ||
                Settings.dateRange[0] > Settings.dateRange[1])
                return false;

            if (Settings.topLeftLocation == null ||
                Settings.topLeftLocation.latitude > -10 ||      /* Australian top left boundary */
                Settings.topLeftLocation.longitude < 110 ||
                Settings.topLeftLocation.latitude > -10 ||      /* Australian top right boundary */
                Settings.topLeftLocation.longitude > 155)
                return false;

            if (Settings.bottomRightLocation == null ||
                Settings.bottomRightLocation.latitude < -40 ||      /* Australian bottom left boundary */
                Settings.bottomRightLocation.longitude < 110 ||
                Settings.bottomRightLocation.latitude < -40 ||      /* Australian bottom right boundary */
                Settings.bottomRightLocation.longitude > 155)
                return false;

            if (Settings.startKm < 0)
                return false;

            if (Settings.endKm < 0)
                return false;

            if (Settings.interval < 0)
                return false;

            if (Settings.minimumJourneyDistance < 0)
                return false;

            if (Settings.loopSpeedThreshold < 0 || Settings.loopSpeedThreshold > 100)
                return false;

            if (Settings.loopBoundaryThreshold < 0)
                return false;

            if (Settings.TSRwindowBoundary < 0)
                return false;

            if (Settings.timeThreshold < 0)
                return false;

            if (Settings.distanceThreshold < 0)
                return false;

            if (Settings.catagory1LowerBound < 0)
                return false;

            if (Settings.catagory1UpperBound < 0)
                return false;

            if (Settings.catagory2LowerBound < 0)
                return false;

            if (Settings.catagory2UpperBound < 0)
                return false;

            return true;


        }




    } // Class Processing
}
