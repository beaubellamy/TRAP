using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globalsettings;

namespace TRAP
{
   


        /// <summary>
        /// Enumerated direction of the train km's.
        /// </summary>
        public enum direction { IncreasingKm, DecreasingKm, Invalid, Unknown };
        public enum trainOperator
        {
            ARTC, Aurizon, CityRail, CountryLink, Freightliner, GreatSouthernRail, Interail, LauchlanValleyRailSociety,
            PacificNational, QUBE, RailCorp, SCT, SouthernShorthaulRail, SydneyRailService, TheRailMotorService, VLinePassenger,
            Combined, Simulated, Unknown
        };
        public enum catagory
        {
            ARTC, Aurizon, CityRail, CountryLink, Freightliner, GreatSouthernRail, Interail, LauchlanValleyRailSociety,
            PacificNational, QUBE, RailCorp, SCT, SouthernShorthaulRail, SydneyRailService, TheRailMotorService, VLinePassenger,
            Combined, Actual, Simulated, Unknown
        };
        public enum trainCommodity { Freight, Coal, Grain, Mineral, Steel, Clinker, Intermodal, Passenger, Work, Unknown };
       

        public class Train
        {
            public catagory catagory;
            public string trainID;
            public string locoID;
            public trainOperator trainOperator;
            public trainCommodity commodity;
            public double powerToWeight;
            public List<TrainJourney> journey;
            public direction trainDirection;
            public bool include;

            public Train()
            {
                this.catagory = catagory.Unknown;
                this.trainID = "none";
                this.locoID = "none";
                this.trainOperator = trainOperator.Unknown;
                this.commodity = trainCommodity.Unknown;
                this.powerToWeight = 0;
                this.journey = new List<TrainJourney>();
                this.trainDirection = direction.Unknown;
                this.include = false;
            }

            public Train(string trainId, string locoID, trainOperator trainOperator, trainCommodity commodity, double power, List<TrainJourney> journey, direction direction, bool include)
            {
                /* Designed for standard train */
                this.catagory = catagory;
                this.trainID = trainId;
                this.locoID = locoID;
                this.trainOperator = trainOperator;
                this.commodity = commodity;
                this.powerToWeight = power;
                this.journey = journey;
                this.trainDirection = direction;
                this.include = include;
            }

            public Train(catagory catagory,string trainId, string locoID, trainOperator trainOperator, trainCommodity commodity, double power, List<TrainJourney> journey, direction direction)
            {
                /* Designed for interpolated train */
                this.catagory = catagory;
                this.trainID = trainId;
                this.locoID = locoID;
                this.trainOperator = trainOperator;
                this.commodity = commodity;
                this.powerToWeight = power;
                this.journey = journey;
                this.trainDirection = direction;
                this.include = true;
            }

            public Train(List<TrainJourney> journey, catagory catagory, direction direction)
            {
                /* Designed for simulated train */
                this.catagory = catagory;
                this.trainID = "Simulated";
                this.locoID = "Simulated";
                this.trainOperator = trainOperator.Simulated;
                this.commodity = trainCommodity.Unknown;
                this.powerToWeight = 0;
                this.journey = journey;
                this.trainDirection = direction;
                this.include = true;
            }

            /// <summary>
            /// Determine the index of the geomerty data for the supplied kilometreage.
            /// </summary>
            /// <param name="TrainJourney">List of train details objects containt the journey details of the train.</param>
            /// <param name="targetKm">The target location to find in the geomerty data.</param>
            /// <returns>The index of the target kilometreage in the geomerty data, -1 if the target is not found.</returns>
            public int indexOfgeometryKm(List<TrainJourney> TrainJourney, double targetKm)
            {
                /* Loop through the train journey. */
                for (int journeyIdx = 0; journeyIdx < TrainJourney.Count(); journeyIdx++)
                {
                    /* Match the current location with the geometry information. */
                    if (Math.Abs(TrainJourney[journeyIdx].kilometreage - targetKm) * 1e12 < 1)
                        return journeyIdx;
                }

                return -1;
            }

        }

        public class TrainJourney
        {
            public GeoLocation location;
            public DateTime dateTime;
            public double speed;
            public double kmPost;
            public double kilometreage;
            public double elevation;
            public bool isLoopHere;
            public bool isTSRHere;

            public TrainJourney()
            {
                this.location = new GeoLocation();
                this.dateTime = DateTime.MinValue;
                this.speed = 0;
                this.kmPost = 0;
                this.kilometreage = 0;
                this.elevation = 0;
                this.isLoopHere = false;
                this.isTSRHere = false;
            }

            public TrainJourney(TrainRecord record)
            {
                this.location = record.location;
                this.dateTime = record.dateTime;
                this.speed = record.speed;
                this.kmPost = record.kmPost;
                this.kilometreage = record.kmPost;
                this.elevation = 0;
                this.isLoopHere = false;
                this.isTSRHere = false;
            }
            
            public TrainJourney(GeoLocation location, DateTime date, double speed, double kmPost, double kilometreage, double elevation, bool loop, bool TSR)
            {
                /* For the standard train */
                this.location = location;
                this.dateTime = date;
                this.speed = speed;
                this.kmPost = kmPost;
                this.kilometreage = kilometreage;
                this.elevation = elevation;
                this.isLoopHere = loop;
                this.isTSRHere = TSR;
            }

            public TrainJourney(DateTime date, double speed, double kmPost, double virtualKm, double elevation, bool loop, bool TSR)
            {
                /* For interpolated Trains */
                this.location = null;
                this.dateTime = date;
                this.speed = speed;
                this.kmPost = kmPost;
                this.kilometreage = virtualKm;
                this.elevation = elevation;
                this.isLoopHere = loop;
                this.isTSRHere = TSR;
            }

            public TrainJourney(GeoLocation location, DateTime date, double speed, double kmPost, double singleLineKm, double elevation)
            {
                /* For the simulated trains */
                this.location = location;
                this.dateTime = date;
                this.speed = speed;
                this.kmPost = kmPost;
                this.kilometreage = singleLineKm;
                this.elevation = elevation;
                this.isLoopHere = false;
                this.isTSRHere = false;
            }
        
        }

        public class TrainRecord
        {
            public string trainID;
            public string locoID;
            public DateTime dateTime;
            public GeoLocation location;
            public trainOperator trainOperator;
            public trainCommodity commodity;
            public double kmPost;
            public double speed;
            public double powerToWeight;


            public TrainRecord()
            { 
            }

            public TrainRecord(string trainID, string locoID, DateTime time, GeoLocation location, trainOperator Operator, trainCommodity commodity, double kmPost, double speed, double power)
            {
                this.trainID = trainID;
                this.locoID = locoID;
                this.dateTime = time;
                this.location = location;
                this.trainOperator = Operator;
                this.commodity = commodity;
                this.kmPost = kmPost;
                this.speed = speed;
                this.powerToWeight = power;
            }


        }

        public class AverageTrain
        {
            public catagory trainCatagory;
            public direction direction;
            public int trainCount;
            public List<double> kilometreage;
            public List<double> elevation;
            public List<double> averageSpeed;
            public List<bool> isInLoopBoundary;
            public List<bool> isInTSRboundary;

            public AverageTrain()
            {
                this.trainCatagory = catagory.Unknown;
                this.direction = direction.Unknown;
                this.trainCount = 0;
                this.kilometreage = new List<double>();
                this.elevation = new List<double>();
                this.averageSpeed = new List<double>();
                this.isInLoopBoundary = new List<bool>();
                this.isInTSRboundary = new List<bool>();
            }

            public AverageTrain(catagory catagory, direction direction, int count, List<double> kilometreage,List<double> elevation,List<double> averageSpeed,List<bool> loop,List<bool> TSR)
            {
                this.trainCatagory = catagory;
                this.direction = direction;
                this.trainCount = count;
                this.kilometreage = kilometreage;
                this.elevation = elevation;
                this.averageSpeed = averageSpeed;
                this.isInLoopBoundary = loop;
                this.isInTSRboundary = TSR;            
            }

        }

        //public class Simulations
        //{
        //    public catagory simulationCatagory;
        //    public direction simulatedDirection;
        //    public List<double> kilometreage;
        //    public List<DateTime> time;
        //    public List<double> elevation;
        //    public List<double> speed;

        //    public Simulations()
        //    {
        //        this.simulationCatagory = catagory.Simulated;
        //        this.simulatedDirection = direction.NotSpecified;
        //        this.kilometreage = new List<double>();
        //        this.time = new List<DateTime>();
        //        this.elevation = new List<double>();
        //        this.speed = new List<double>();
        //    }

        //    //public Simulations(catagory catagory)
        //    //{
        //    //    this.simulationCatagory = catagory;
        //    //    this.kilometreage = new List<double>();
        //    //    this.time = new List<DateTime>();
        //    //    this.elevation = new List<double>();
        //    //    this.speed = new List<double>();
        //    //}

        //    public Simulations(catagory catagory, direction direction, List<double> kilometreage, List<DateTime> time, List<double> elevation, List<double> speed)
        //    {
        //        this.simulationCatagory = catagory;
        //        this.simulatedDirection = direction;
        //        this.kilometreage = kilometreage;
        //        this.time = time;
        //        this.elevation = elevation;
        //        this.speed = speed;
        //    }

        //}

        /// <summary>
        /// A class describing a geographic location with latitude and longitude.
        /// </summary>
        public class GeoLocation
        {
            /* Latitude and longitude of the location */
            public double latitude;
            public double longitude;

            /// <summary>
            /// Default constructor.
            /// </summary>
            public GeoLocation()
            {
                // Default: Sydney Harbour Bridge
                this.latitude = -33.8519;
                this.longitude = 151.2108;
            }

            /// <summary>
            /// Geolocation constructor
            /// </summary>
            /// <param name="lat">latitude of the location.</param>
            /// <param name="lon">longitude of the location.</param>
            public GeoLocation(double lat, double lon)
            {
                this.latitude = lat;
                this.longitude = lon;
            }

            public GeoLocation(TrainRecord record)
            {
                this.latitude = record.location.latitude;
                this.longitude = record.location.longitude;
            }

            public GeoLocation(TrainJourney journey)
            {
                this.latitude = journey.location.latitude;
                this.longitude = journey.location.longitude;
            }

        }

        /// <summary>
        /// A class describing the parameters associated with a TSR.
        /// </summary>
        public class TSRObject
        {
            public string Region;
            public DateTime IssueDate;
            public DateTime LiftedDate;
            public double startKm;
            public double endKm;
            public double TSRSpeed;

            /// <summary>
            /// Default TSRObject constructor.
            /// </summary>
            public TSRObject()
            {
                this.Region = "Unknown";
                this.IssueDate = DateTime.MinValue;
                this.LiftedDate = DateTime.MinValue;
                this.startKm = 0;
                this.endKm = 0;
                this.TSRSpeed = 0;
            }

            /// <summary>
            /// TSRObject constructor
            /// </summary>
            /// <param name="region">Region the TSR is in.</param>
            /// <param name="issued">The date the TSR was applied.</param>
            /// <param name="lifted">The Date the TSR was lifted, if applicable.</param>
            /// <param name="start">The start km of the TSR.</param>
            /// <param name="finish">The end Km of the TSR.</param>
            /// <param name="speed">The speed restriction applied to the TSR.</param>
            public TSRObject(string region, DateTime issued, DateTime lifted, double start, double finish, double speed)
            {
                this.Region = region;
                this.IssueDate = issued;
                this.LiftedDate = lifted;
                this.startKm = start;
                this.endKm = finish;
                this.TSRSpeed = speed;
            }

        }



        class Algorithm
        {
            /* Create a tools object. */
            //public static Tools tool = new Tools();
            ///* Create a processing object. */
            public static Processing processing = new Processing();
            ///* Create a trackGeometry object. */
            public static TrackGeometry track = new TrackGeometry();
            ///* Create a statistics object. */
            //public static Statistics stats = new Statistics();

            /// <summary>
            /// Determine the average train performance in both directions based on the supplied 
            /// actual train data. The included data is restricted to values of the power to weight 
            /// ratios for the underpowered and overpowered catagories on the form. The loop 
            /// locations and TSR information is also used to extract the data that corresponds 
            /// to a train that enteres a loop and is bound by a TSR. If the train is within the 
            /// 'loop bounday threshold' and is deemed to be stopping in the loop, the data at this 
            /// location is not included. The train is deemed to be stopping in a loop if the train 
            /// speed drops below the simulated speed multiplied by the 'loop speed factor'. If the 
            /// train is within the 'TSR window', the data at this location is ignored as the train 
            /// is bound by the TSR at the location. The average train is then determined from the 
            /// included train data.
            /// 
            /// This function produces a number of ouput files containing the data through the 
            /// progresive steps.
            /// </summary>        
            [STAThread]
            public static List<Train> trainPerformance()
            {

                //Settings.includeAListOfTrainsToExclude = false;
                //FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin Geometry.csv"; ;
                //FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin TSR.csv";
                //FileSettings.dataFile= @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin Data 2016-2017.txt";
                //FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Increasing.csv");
                //FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Decreasing.csv");
                //FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Increasing-60.csv");
                //FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Decreasing.csv");
                //FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin";
                //Settings.distanceThreshold = 4*1000;
                //Settings.minimumJourneyDistance = 250*1000;
                //Settings.startKm = 264;
                //Settings.endKm = 541;
                //Settings.interval = 50;
                //Settings.timeThreshold = 10*60;
                //Settings.dateRange = new DateTime[]{new DateTime(2016,1,1), new DateTime(2016,1,8)};
                //Settings.topLeftLocation = new GeoLocation(-10,110);
                //Settings.bottomRightLocation = new GeoLocation(-40,152);
                //Settings.loopBoundaryThreshold = 1;
                //Settings.loopSpeedThreshold = 0.5;
                //Settings.TSRwindowBoundary = 1;
                //Settings.HunterValleyRegion = true;



                /* Ensure there is a empty list of trains to exclude to start. */
                List<string> excludeTrainList = new List<string> { };

                /* Populate the exluded train list. */
                if (Settings.includeAListOfTrainsToExclude)
                    excludeTrainList = FileOperations.readTrainList(FileSettings.trainListFile);

                /* Read in the track geometry data. */
                List<TrackGeometry> trackGeometry = new List<TrackGeometry>();
                trackGeometry = FileOperations.readGeometryfile(FileSettings.geometryFile);

                /* Read in the TSR information */
                List<TSRObject> TSRs = new List<TSRObject>();
                TSRs = FileOperations.readTSRFile(FileSettings.temporarySpeedRestrictionFile);

                

                /* Read the data. */
                List<TrainRecord> TrainRecords = new List<TrainRecord>();
                TrainRecords = FileOperations.readICEData(FileSettings.dataFile, excludeTrainList);

                if (TrainRecords.Count() == 0)
                {
                    //tool.messageBox("There are no records in the list to analyse.", "No trains available.");
                    return new List<Train>();
                }

                List<trainOperator> operators = TrainRecords.Select(t => t.trainOperator).Distinct().ToList();
                operators.Remove(trainOperator.Unknown);
                int numberOfOperators = operators.Count();


                /* Read in the simulation data and interpolate to the desired interval. */
                /* Maybe prduce a weighted simulation to replace the values for those points that are affected by TSRs */
                
                List<catagory> simCatagories = new List<catagory>();
                //catagory simCatagory1 = catagory.Unknown;
                //catagory simCatagory2 = catagory.Unknown;
                //catagory simCatagory3 = catagory.Unknown;
                
                /* Check conditions to change the catagories */
                if (Settings.HunterValleyRegion)
                {
                    /* Analysing train in the Hunter Valley region:
                     * Newcastle/Muswellbrook to Narrabri - only Pacific National and Aurizon
                     * Newcastle/Muswellbrook to Ulan - Pacific National, Aurizon and Freightliner 
                     */
                    if (numberOfOperators == 2)
                    {
                        /* Analysing Newcastle/Muswellbrook to Narrabri. */
                        //simCatagory1 = catagory.PacificNational;
                        //simCatagory2 = catagory.Aurizon;
                        
                        /* OR */
                        simCatagories.Add(catagory.PacificNational);
                        simCatagories.Add(catagory.Aurizon);
                        
                    }
                    else if (numberOfOperators == 3)
                    {
                        /* Analysing Newcastle/Muswellbrook to Ulan. */
                        //simCatagory1 = catagory.PacificNational;
                        //simCatagory2 = catagory.Aurizon;
                        //simCatagory3 = catagory.Freightliner;

                        /* OR */
                        simCatagories.Add(catagory.PacificNational);
                        simCatagories.Add(catagory.Aurizon);
                        simCatagories.Add(catagory.Freightliner);
                        
                    }
                        
                    else
                    {
                        Console.WriteLine("The number of operators in the train list is: " + numberOfOperators);
                        throw new ArgumentOutOfRangeException("The number of operators is " + numberOfOperators + ", this many operatos are not supported.");
                    }

                }
                else 
                {
                    //simCatagory1 = catagory.Underpowered;
                    //simCatagory2 = catagory.Overpowered;

                    /* OR */
                    simCatagories.Add(catagory.Underpowered);
                    simCatagories.Add(catagory.Overpowered);

                }

                List<Train> simulatedTrains = new List<Train>();
                
                /* Check the size of teh catagories and the size of teh simulation file list match. */
               

                for (int index = 0; index < simCatagories.Count(); index++ )
                {
                    simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.simulationFiles[index*2], simCatagories[index], direction.IncreasingKm));
                    simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.simulationFiles[index*2+1], simCatagories[index], direction.DecreasingKm));
                }
                //simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.underpoweredDecreasingSimulationFile, simCatagory1, direction.decreasing));
                //simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.overpoweredIncreasingSimulationFile, simCatagory2, direction.increasing));
                //simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.overpoweredDecreasingSimulationFile, simCatagory2, direction.decreasing));

                //if (numberOfOperators == 3)
                //{
                //    simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.alternativeIncreasingSimulationFile, simCatagory3, direction.increasing));
                //    simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.alternativeDecreasingSimulationFile, simCatagory3, direction.decreasing));
                //}

                /* Interpolate the simulations to the same granularity as the ICE data will be. */
                List<Train> interpolatedSimulations = new List<Train>();
                interpolatedSimulations = processing.interpolateTrainData(simulatedTrains, trackGeometry);



                //List<simulatedTrain> underpoweredIncreasingSimulation = new List<simulatedTrain>();
                //underpoweredIncreasingSimulation = FileOperations.readSimulationData(FileSettings.underpoweredIncreasingSimulationFile);
                //List<InterpolatedTrain> simulationUnderpoweredIncreasing = new List<InterpolatedTrain>();
                //simulationUnderpoweredIncreasing = processing.interpolateSimulationData(underpoweredIncreasingSimulation, trackGeometry);

                //List<simulatedTrain> underpoweredDecreasingSimulation = new List<simulatedTrain>();
                //underpoweredDecreasingSimulation = FileOperations.readSimulationData(FileSettings.underpoweredDecreasingSimulationFile);
                //underpoweredDecreasingSimulation = underpoweredDecreasingSimulation.OrderBy(t => t.singleLineKm).ToList();
                //List<InterpolatedTrain> simulationUnderpoweredDecreasing = new List<InterpolatedTrain>();
                //simulationUnderpoweredDecreasing = processing.interpolateSimulationData(underpoweredDecreasingSimulation, trackGeometry);

                ///* Ovderpowered Simualtions. */
                //List<simulatedTrain> overpoweredIncreasingSimulation = new List<simulatedTrain>();
                //overpoweredIncreasingSimulation = FileOperations.readSimulationData(FileSettings.overpoweredIncreasingSimulationFile);
                //List<InterpolatedTrain> simulationOverpoweredIncreasing = new List<InterpolatedTrain>();
                //simulationOverpoweredIncreasing = processing.interpolateSimulationData(overpoweredIncreasingSimulation, trackGeometry);

                //List<simulatedTrain> overpoweredDecreasingSimulation = new List<simulatedTrain>();
                //overpoweredDecreasingSimulation = FileOperations.readSimulationData(FileSettings.overpoweredDecreasingSimulationFile);
                //overpoweredDecreasingSimulation = overpoweredDecreasingSimulation.OrderBy(t => t.singleLineKm).ToList();
                //List<InterpolatedTrain> simulationOverpoweredDecreasing = new List<InterpolatedTrain>();
                //simulationOverpoweredDecreasing = processing.interpolateSimulationData(overpoweredDecreasingSimulation, trackGeometry);


                /* If the data doesn't contain P/W ratios, replace the P/W ratio boundaries */
                //if (TrainRecords.Where(t => t.powerToWeight == 0).Count() == TrainRecords.Count())
                //    Settings.resetPowerToWeightBoundariesToZero();

                /* Sort the data by [trainID, locoID, Date & Time, kmPost]. */
                

                List<TrainRecord> OrderdTrainRecords = new List<TrainRecord>();
                OrderdTrainRecords = TrainRecords.OrderBy(t => t.trainID).ThenBy(t => t.locoID).ThenBy(t => t.dateTime).ThenBy(t => t.kmPost).ToList();
                

                /**************************************************************************************************/
                /* Clean data - remove trains with insufficient data. */
                /******** Should only be required while we are waiting for the data in the prefered format ********/

                /* Isolate the individual trains and process the journey */
                // need to try to limit the number of list objects that are created.
                /* MakeTrains */
                /* CleanTrains */

                //List<Train> testTrainRecords = new List<Train>();
                //testTrainRecords = MakeTrains(trackGeometry, OrderdTrainRecords, TSRs);

                List<Train> CleanTrainRecords = new List<Train>();
                CleanTrainRecords = CleanData(OrderdTrainRecords, trackGeometry, TSRs);

                
                /* interpolate data */
                /******** Should only be required while we are waiting for the data in the prefered format ********/
                List<Train> interpolatedTrains = new List<Train>();
                interpolatedTrains = processing.interpolateTrainData(CleanTrainRecords, trackGeometry);
                //interpolatedRecords = processing.interpolateTrainData(testTrainRecords, trackGeometry);

                /**************************************************************************************************/




                /* Populate the trains TSR values after interpolation to gain more granularity with TSR boundary. */
                processing.populateAllTrainsTemporarySpeedRestrictions(interpolatedTrains, TSRs);

                //List<InterpolatedTrain> unpackedInterpolation = new List<InterpolatedTrain>();
                //unpackedInterpolation = unpackInterpolatedData(interpolatedRecords);
                //FileOperations.writeTrainData(unpackedInterpolation);
                FileOperations.writeTrainData(interpolatedTrains);

                /******************************************************/
                /* Can we have a generic average function for operators, power catagories and commodity?
                 */
                /* Generate sats for each */

                ///* Genearate the statistics lists. */
                //List<Statistics> stats = new List<Statistics>();
                //List<Train> increasing = interpolatedRecords.Where(t => t.TrainJourney[0].trainDirection == direction.increasing).ToList();
                //List<Train> decreasing = interpolatedRecords.Where(t => t.TrainJourney[0].trainDirection == direction.decreasing).ToList();

                /* Average the train data for each direction with regard for TSR's and loop locations. */
                /* generate a list of trains that comply with specific requiremnts - (direction, operator/powerToWeight)
                 * supply interpolated simulation of same catagory
                 * supply interpolated weighted simulation
                 */

                //List<averagedTrainData> averageData = new List<averagedTrainData>();

                //if (Settings.HunterValleyRegion)
                //{
                //    averageData = processing.operatorAverageSpeed(interpolatedRecords, trackGeometry, simulationUnderpoweredIncreasing, simulationUnderpoweredDecreasing,
                //        simulationOverpoweredIncreasing, simulationOverpoweredDecreasing);

                //    /* Generate some statistical information for the aggregated data. */
                //    List<Train> PacificNationalIncreasing = interpolatedRecords.Where(t => t.TrainJourney[0].Operator == trainOperator.PacificNational).Where(t => t.TrainJourney[0].trainDirection == direction.increasing).ToList();
                //    List<Train> PacificNationalDecreasing = interpolatedRecords.Where(t => t.TrainJourney[0].Operator == trainOperator.PacificNational).Where(t => t.TrainJourney[0].trainDirection == direction.decreasing).ToList();
                //    List<Train> AurizonIncreasing = interpolatedRecords.Where(t => t.TrainJourney[0].Operator == trainOperator.Aurizon).Where(t => t.TrainJourney[0].trainDirection == direction.increasing).ToList();
                //    List<Train> AurizonDecreasing = interpolatedRecords.Where(t => t.TrainJourney[0].Operator == trainOperator.Aurizon).Where(t => t.TrainJourney[0].trainDirection == direction.decreasing).ToList();

                //    /* Calcualte the statistics on each group. */
                //    stats.Add(Statistics.generateStats(PacificNationalIncreasing, "Pacific National Increasing"));
                //    stats.Add(Statistics.generateStats(PacificNationalDecreasing, "Pacific National Decreasing"));
                //    stats.Add(Statistics.generateStats(AurizonIncreasing, "Aurizon Increasing"));
                //    stats.Add(Statistics.generateStats(AurizonDecreasing, "Aurizon Decreasing"));


                //}
                //else
                //{
                //    averageData = processing.powerToWeightAverageSpeed(interpolatedRecords, trackGeometry, simulationUnderpoweredIncreasing, simulationUnderpoweredDecreasing,
                //        simulationOverpoweredIncreasing, simulationOverpoweredDecreasing);

                //    /* Generate some statistical information for the aggregated data. */
                //    List<Train> underpoweredTrains = interpolatedRecords.Where(t => t.TrainJourney[0].powerToWeight > Settings.underpoweredLowerBound &&
                //        t.TrainJourney[0].powerToWeight > Settings.underpoweredLowerBound).ToList();
                //    List<Train> overpoweredTrains = interpolatedRecords.Where(t => t.TrainJourney[0].powerToWeight > Settings.overpoweredLowerBound &&
                //        t.TrainJourney[0].powerToWeight > Settings.overpoweredLowerBound).ToList();

                //    /* Calcualte the statistics on each group. */
                //    stats.Add(Statistics.generateStats(underpoweredTrains, "Underpowered Trains"));
                //    stats.Add(Statistics.generateStats(overpoweredTrains, "Overpowered Trains"));
                //    stats.Add(Statistics.generateStats(interpolatedRecords, "Combined"));

                //}

                //stats.Add(Statistics.generateStats(increasing, "Combined Increasing"));
                //stats.Add(Statistics.generateStats(decreasing, "Combined Decreasing"));

                /* Extract the train operator from the catagory */
                //simCatagory1
                List<AverageTrain> averageTrains = new List<AverageTrain>();

                List<Train> increasingTrainCatagory = new List<Train>();
                List<Train> decreasingTrainCatagory = new List<Train>();

                List<Statistics> stats = new List<Statistics>();

                for (int index = 0; index < simCatagories.Count(); index++)
                {
                    
                    if (Settings.HunterValleyRegion)
                    {
                        trainOperator operatorCatagory = matchOperatorToCatagory(simCatagories[index]);
                        /* will be an operator; can be 2 or 3 different operators */
                        increasingTrainCatagory = interpolatedTrains.Where(t => t.trainOperator == operatorCatagory).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        decreasingTrainCatagory = interpolatedTrains.Where(t => t.trainOperator == operatorCatagory).Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                        stats.Add(Statistics.generateStats(increasingTrainCatagory));
                        stats.Add(Statistics.generateStats(decreasingTrainCatagory));
                    }
                    else
                    { 
                        /* Can only be 2 catagories; underpowered and overpowered. */
                        increasingTrainCatagory = interpolatedTrains.Where(t => t.catagory == simCatagories[index]).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        decreasingTrainCatagory = interpolatedTrains.Where(t => t.catagory == simCatagories[index]).Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                        stats.Add(Statistics.generateStats(increasingTrainCatagory));
                        stats.Add(Statistics.generateStats(decreasingTrainCatagory));
                    }


                    averageTrains.Add(processing.averageTrain(increasingTrainCatagory, interpolatedSimulations[index*2].journey, trackGeometry));
                    averageTrains.Add(processing.averageTrain(decreasingTrainCatagory, interpolatedSimulations[index*2+1].journey, trackGeometry));

                }

                /* Add the weighted average trains to the list. */
                List<Train> increasingCombined = new List<Train>();
                List<Train> decreasingCombined = new List<Train>();

                if (Settings.HunterValleyRegion)
                {
                    /* Will be an operator; can be 2 or 3 different operators */
                    increasingCombined = interpolatedTrains.Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                    decreasingCombined = interpolatedTrains.Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                    setOperatorToCombined(increasingCombined);
                    setOperatorToCombined(decreasingCombined);

                    stats.Add(Statistics.generateStats(increasingCombined));
                    stats.Add(Statistics.generateStats(decreasingCombined));                    
                }
                else
                {
                    List<Train> increasingSubList = new List<Train>();
                    List<Train> decreasingSubList = new List<Train>();
                    /* Can only be 2 catagories; underpowered and overpowered;
                     * Add each catagory to the final lists.
                     */
                    foreach(catagory simCatagory in simCatagories)
                    {
                        increasingSubList = interpolatedTrains.Where(t => t.catagory == simCatagory).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        increasingCombined.AddRange(increasingSubList);
                        decreasingSubList = interpolatedTrains.Where(t => t.catagory == simCatagory).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                        decreasingCombined.AddRange(decreasingSubList);
                    }

                    setOperatorToCombined(increasingCombined);
                    setOperatorToCombined(decreasingCombined);
                    stats.Add(Statistics.generateStats(increasingCombined));
                    stats.Add(Statistics.generateStats(decreasingCombined));
                    
                }

                
                /* Create a weighted average simulation */
                List<Train> weightedSimualtion = new List<Train>();
                weightedSimualtion = Processing.getWeightedAverageSimulation(interpolatedSimulations, averageTrains);

                /* Calculate the weighted average train in each direction. */
                if (weightedSimualtion.Count() >= 2)
                {
                    averageTrains.Add(processing.averageTrain(increasingCombined, weightedSimualtion[0].journey, trackGeometry));
                    averageTrains.Add(processing.averageTrain(decreasingCombined, weightedSimualtion[1].journey, trackGeometry));
                }
                else
                {
                    string error = "There aren't enough weighted simualtion files to proceed.";
                    Console.WriteLine(error);

                    throw new ArgumentException(error);
                }
                /******************************************************/
                
                





                /* Seperate averages for P/W ratio groups, commodity, Operator */
                /* AverageByPower2Weight    -> powerToWeightAverageSpeed
                 * AverageByCommodity       -> not written
                 * AverageByOperator        -> not written
                 * 
                 * Maybe use a generic function - pass in only the list of trains that conform to the desired boundaries.
                 */

                /* Write the averaged Data to file for inspection. */
                FileOperations.wrtieAverageData(averageTrains, stats);
                ///* Unpack the records into a single trainDetails object list. */
                //List<TrainDetails> unpackedData = new List<TrainDetails>();
                //unpackedData = unpackCleanData(CleanTrainRecords);

                ///* Write data to an excel file. */
                //FileOperations.writeTrainData(unpackedData);

                return interpolatedTrains;
            }

            /// <summary>
            /// This function cleans the data from large gaps in the data and ensures the trains 
            /// are all travelling in a single direction with a minimum total distance.
            /// </summary>
            /// <param name="trackGeometry">A list of track Geometry objects</param>
            /// <param name="record">List of TrainDetail objects</param>
            /// <returns>List of Train objects containign the journey details of each train.</returns>
            public static List<Train> CleanData(List<TrainRecord> record, List<TrackGeometry> trackGeometry, List<TSRObject> TSRs)
            {
                /* Note: this function will not be needed when Enterprise Services delivers the interpolated 
                 * date directly to the database. We can access this data directly, then analyse.
                 */

                bool removeTrain = false;
                double distance = 0;
                double journeyDistance = 0;

                List<Train> cleanTrainList = new List<Train>();
                //trainOperator trainOperator = trainOperator.unknown;
                List<TrainJourney> journey = new List<TrainJourney>();

                GeoLocation point1 = null;
                GeoLocation point2 = null;

                /* Add the first point to the train journey. */
                journey.Add(new TrainJourney(record[0]));

                for (int trainIndex = 1; trainIndex < record.Count(); trainIndex++)
                {
                    /* Compare next train details with current train details to establish if its a new train. */
                    if (record[trainIndex].trainID.Equals(record[trainIndex - 1].trainID) &&
                        record[trainIndex].locoID.Equals(record[trainIndex - 1].locoID) &&
                        (record[trainIndex].dateTime - record[trainIndex - 1].dateTime).TotalMinutes < Settings.timeThreshold)
                    {

                        /* If the current and previous record represent the same train journey, add it to the list. */
                        journey.Add(new TrainJourney(record[trainIndex]));

                        point1 = new GeoLocation(record[trainIndex - 1]);
                        point2 = new GeoLocation(record[trainIndex]);

                        distance = processing.calculateGreatCircleDistance(point1, point2);

                        if (distance > Settings.distanceThreshold)
                        {
                            /* If the distance between successive km points is greater than the
                             * threshold then we want to remove this train from the data. 
                             */
                            removeTrain = true;
                        }

                    }
                    else
                    {
                        /* Check uni directionality of the train */
                        journey = processing.longestDistanceTravelledInOneDirection(journey, trackGeometry);
                        /* Calculate the total length of the journey */
                        journeyDistance = processing.calculateTrainJourneyDistance(journey);

                        /* Validate the direction of train */
                        Train item = new Train();
                        //bool HunterValley = true;

                        //if (HunterValley)
                        //    trainOperator = whoIsOperator(tranJourney[0].LocoID);

                        item.journey = journey;
                        item.trainDirection = processing.getTrainDirection(item);
                        

                        //processing.populateOperator(item, trainOperator);
                        //processing.populateDirection(item, trackGeometry);

                        /* remove the train if the direction is not valid. */
                        if (item.trainDirection == direction.Invalid)
                            removeTrain = true;

                        /* The end of the train journey has been reached. */
                        if (!removeTrain && journeyDistance > Settings.minimumJourneyDistance)
                        {
                            /* If all points are acceptable and the train travels the minimum distance, 
                             * add the train journey to the cleaned list. 
                             */
                            item.trainID = record[trainIndex - 1].trainID ;
                            item.locoID = record[trainIndex - 1].locoID;
                            item.trainOperator = record[trainIndex - 1].trainOperator;
                            item.commodity = record[trainIndex - 1].commodity;
                            item.powerToWeight = record[trainIndex - 1].powerToWeight;

                            if (Settings.HunterValleyRegion)
                            {
                                item.catagory = matchCatagoryToOperator(item.trainOperator);
                            }
                            else
                            {
                                if (item.powerToWeight > Settings.catagory1LowerBound && item.powerToWeight <= Settings.catagory1UpperBound)
                                    item.catagory = catagory.Underpowered;
                                else if (item.powerToWeight > Settings.catagory2LowerBound && item.powerToWeight <= Settings.catagory2UpperBound)
                                    item.catagory = catagory.Overpowered;
                                else
                                    item.catagory = catagory.Actual;
                            
                            }


                            /* Determine the actual km, and populate the loops and TSR information. */
                            processing.populateGeometryKm(item.journey, trackGeometry);
                            processing.populateLoopLocations(item.journey, trackGeometry);

                            /* Sort the journey in ascending order. */
                            item.journey = item.journey.OrderBy(t => t.kilometreage).ToList();

                            cleanTrainList.Add(item);
                            
                        }

                        /* Reset the parameters for the next train. */
                        removeTrain = false;
                        journeyDistance = 0;
                        journey.Clear();

                        /* Add the first record of the new train journey. */
                        journey.Add(new TrainJourney(record[trainIndex]));
                        //trainPoint = new GeoLocation(record[trainIndex]);

                    }

                    /* The end of the records have been reached. */
                    if (trainIndex == record.Count() - 1 && !removeTrain)
                    {
                        /* Check uni directionality of the last train */
                        journey = processing.longestDistanceTravelledInOneDirection(journey, trackGeometry);
                        /* Calculate the total length of the journey */
                        journeyDistance = processing.calculateTrainJourneyDistance(journey);
                        

                        /* Validate the direction of train */
                        Train lastItem = new Train();
                        //bool HunterValley = true;
                        //if (HunterValley)
                        //    trainOperator = whoIsOperator(tranJourney[0].LocoID);

                        lastItem.journey = journey;
                        //lastItem.trainOperator = record[trainIndex - 1].trainOperator;
                        lastItem.trainDirection = processing.getTrainDirection(lastItem);
                        
                        /* remove the train if the direction is not valid. */
                        if (lastItem.trainDirection == direction.Invalid)
                            removeTrain = true;

                        if (!removeTrain && journeyDistance > Settings.minimumJourneyDistance)
                        {
                            lastItem.trainID = record[trainIndex - 1].trainID;
                            lastItem.locoID = record[trainIndex - 1].locoID;
                            lastItem.trainOperator = record[trainIndex - 1].trainOperator;
                            lastItem.commodity = record[trainIndex - 1].commodity;
                            lastItem.powerToWeight = record[trainIndex - 1].powerToWeight;

                            //lastItem.journey = journey;

                            /* If all points are aceptable, add the train journey to the cleaned list. */
                            processing.populateGeometryKm(lastItem.journey, trackGeometry);
                            processing.populateLoopLocations(lastItem.journey, trackGeometry);

                            /* Sort the journey in ascending order. */
                            lastItem.journey = lastItem.journey.OrderBy(t => t.kilometreage).ToList();

                            cleanTrainList.Add(lastItem);
                        }

                    }

                }

                return cleanTrainList;

            }

            ///// <summary>
            ///// Construct a list of Trains with individual train journey and details
            ///// </summary>
            ///// <param name="trackGeometry">A list of track Geometry objects</param>
            ///// <param name="trainRecords">List of TrainDetail objects</param>
            ///// <returns>List of Train objects containign the journey details of each train.</returns>
            //public static List<Train> MakeTrains(List<TrackGeometry> trackGeometry, List<TrainDetails> trainRecords, List<TSRObject> TSRs)
            //{

            //    /* Place holder for the train records that are acceptable. */
            //    List<TrainDetails> newTrainList = new List<TrainDetails>();
            //    /* List of each Train with its journey details that is acceptable. */
            //    List<Train> cleanTrainList = new List<Train>();

            //    trainOperator trainOperator = trainOperator.unknown;

            //    /* Add the first record to the list. */
            //    newTrainList.Add(trainRecords[0]);
            //    GeoLocation trainPoint = new GeoLocation(trainRecords[0]);
            //    /* Populate the first actual kilometreage point. */
            //    newTrainList[0].geometryKm = track.findClosestTrackGeometryPoint(trackGeometry, trainPoint);

            //    for (int trainIndex = 1; trainIndex < trainRecords.Count(); trainIndex++)
            //    {
            //        /* Compare next train details with current train details to establish if its a new train. */
            //        if (trainRecords[trainIndex].TrainID.Equals(trainRecords[trainIndex - 1].TrainID) &&
            //            trainRecords[trainIndex].LocoID.Equals(trainRecords[trainIndex - 1].LocoID) &&
            //            (trainRecords[trainIndex].NotificationDateTime - trainRecords[trainIndex - 1].NotificationDateTime).TotalMinutes < Settings.timeThreshold)
            //        {
            //            /* If the current and previous record represent the same train journey, add it to the list. */
            //            newTrainList.Add(trainRecords[trainIndex]);

            //        }
            //        else
            //        {
            //            /* Check uni directionality of the train */
            //            newTrainList = processing.longestDistanceTravelledInOneDirection(newTrainList, trackGeometry);

            //            /* Validate the direction of train */
            //            Train item = new Train();
            //            bool HunterValley = true;
            //            if (HunterValley)
            //                trainOperator = whoIsOperator(newTrainList[0].LocoID);

            //            item.TrainJourney = newTrainList.ToList();
            //            processing.populateOperator(item, trainOperator);
            //            processing.populateDirection(item, trackGeometry);

            //            /* Determine the actual km, and populate the loops and TSR information. */
            //            processing.populateGeometryKm(item, trackGeometry);
            //            processing.populateLoopLocations(item, trackGeometry);

            //            /* Sort the journey in ascending order. */
            //            item.TrainJourney = item.TrainJourney.OrderBy(t => t.geometryKm).ToList();

            //            cleanTrainList.Add(item);



            //            /* Reset the parameters for the next train. */
            //            newTrainList.Clear();

            //            /* Add the first record of the new train journey. */
            //            newTrainList.Add(trainRecords[trainIndex]);

            //        }

            //        /* The end of the records have been reached. */
            //        if (trainIndex == trainRecords.Count() - 1)
            //        {
            //            /* Check uni directionality of the last train */
            //            newTrainList = processing.longestDistanceTravelledInOneDirection(newTrainList, trackGeometry);

            //            /* Validate the direction of train */
            //            Train lastItem = new Train();
            //            bool HunterValley = true;
            //            if (HunterValley)
            //                trainOperator = whoIsOperator(newTrainList[0].LocoID);

            //            lastItem.TrainJourney = newTrainList.ToList();
            //            processing.populateOperator(lastItem, trainOperator);
            //            processing.populateDirection(lastItem, trackGeometry);

            //            /* If all points are aceptable, add the train journey to the cleaned list. */
            //            processing.populateGeometryKm(lastItem, trackGeometry);
            //            processing.populateLoopLocations(lastItem, trackGeometry);

            //            /* Sort the journey in ascending order. */
            //            lastItem.TrainJourney = lastItem.TrainJourney.OrderBy(t => t.geometryKm).ToList();

            //            cleanTrainList.Add(lastItem);


            //        }

            //    }

            //    return cleanTrainList;

            //}

            ///// <summary>
            ///// Unpack the Train data structure into a single list of TrainDetails objects.
            ///// </summary>
            ///// <param name="OrderdTrainRecords">The Train object containing a list of trains with their journey details.</param>
            ///// <returns>A single list of TrainDetail objects.</returns>
            //public static List<TrainDetails> unpackCleanData(List<Train> OrderdTrainRecords)
            //{
            //    /* Place holder to store all train records in one list. */
            //    List<TrainDetails> unpackedData = new List<TrainDetails>();

            //    /* Cycle through each train. */
            //    foreach (Train train in OrderdTrainRecords)
            //    {
            //        /* Cycle through each record in the train journey. */
            //        for (int journeyIdx = 0; journeyIdx < train.TrainJourney.Count(); journeyIdx++)
            //        {
            //            /* Add it to the list. */
            //            unpackedData.Add(train.TrainJourney[journeyIdx]);
            //        }
            //    }
            //    return unpackedData;
            //}

            ///// <summary>
            ///// Unpack the Train data structure into a single list of interpolatedTrain objects.
            ///// </summary>
            ///// <param name="OrderdTrainRecords">The Train object containing a list of trains with their journey details.</param>
            ///// <returns>A single list of interpolatedTrain objects.</returns>
            //public static List<InterpolatedTrain> unpackInterpolatedData(List<Train> OrderdTrainRecords)
            //{
            //    /* Place holder to store all train records in one list. */
            //    List<InterpolatedTrain> unpackedData = new List<InterpolatedTrain>();

            //    /* Cycle through each train. */
            //    foreach (Train train in OrderdTrainRecords)
            //    {
            //        /* Cycle through each record in the train journey. */
            //        for (int journeyIdx = 0; journeyIdx < train.TrainJourney.Count(); journeyIdx++)
            //        {
            //            /* Add it to the list. */
            //            unpackedData.Add(new InterpolatedTrain(train.TrainJourney[journeyIdx]));
            //        }
            //    }
            //    return unpackedData;
            //}

            ///// <summary>
            ///// This function determines the train operator based on the loco ID, This is generally only 
            ///// applicable for the Hunter Valley region.
            ///// </summary>
            ///// <param name="locoID">The Loco ID of the train</param>
            ///// <returns>The name of the train operator.</returns>
            //private static trainOperator whoIsOperator(string locoID)
            //{
            //    double aurizon = 0;
            //    double.TryParse(locoID, out aurizon);

            //    if (locoID.Substring(0, 2).Equals("TT", StringComparison.OrdinalIgnoreCase))
            //        return trainOperator.PacificNational;  //"Pacific National";
            //    else if (aurizon >= 5000)
            //        return trainOperator.Aurizon; // "Aurizon";
            //    else
            //        return trainOperator.unknown; // "Unknown";
            //}

            private static trainOperator matchOperatorToCatagory(catagory catagory)
            {
                trainOperator trainOperator = trainOperator.Unknown;

                List<trainOperator> operatorList = Enum.GetValues(typeof(trainOperator)).Cast<trainOperator>().ToList();

                foreach (trainOperator Operator in operatorList)
                {
                    if (Operator.ToString().Equals(catagory.ToString()))
                        trainOperator = Operator;
                }

                return trainOperator;
            }

            private static catagory matchCatagoryToOperator(trainOperator trainOperator)
            {
                catagory trainCatagory = catagory.Unknown;

                List<catagory> catagoryList = Enum.GetValues(typeof(catagory)).Cast<catagory>().ToList();

                foreach (catagory cat in catagoryList)
                {
                    if (cat.ToString().Equals(trainOperator.ToString()))
                        trainCatagory = cat;
                }

                return trainCatagory;
            }

            private static void setOperatorToCombined(List<Train> combined)
            {
                foreach(Train train in combined)
                {
                    train.catagory = catagory.Combined;
                }
            }

            
    } // Class Algorithm
}
