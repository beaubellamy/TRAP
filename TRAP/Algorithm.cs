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

    /// <summary>
    /// A List of valid train operators.
    /// </summary>
    public enum trainOperator
    {
        ARTC, Aurizon, AustralianRailwaysHistoricalSociety, CityRail, Countrylink, Freightliner, GenesseeWyoming, GreatSouthernRail, Interail, 
        JohnHollandRail, LauchlanValleyRailSociety, PacificNational, QUBE, RailTransportMuseum, RailCorp, SCT, SouthernShorthaulRail, 
        SydneyRailService, TheRailMotorService, VLinePassenger, GroupRemaining, Combined, Simulated, Unknown
    };

    /// <summary>
    /// A list of available commodities.
    /// </summary>
    public enum trainCommodity
    {
        GeneralFreight, Coal, Grain, Mineral, Steel, Clinker, Intermodal, Passenger, Work, GroupRemaining, Unknown
    };

    /// <summary>
    /// A list of analysis Categories, comprising of train operators, power to weight ratios.
    /// {TrainOperator List, TrainCommodity, power to weight Categories}
    /// </summary>
    public enum Category
    {
        /* Train Operators. */
        ARTC, Aurizon, AustralianRailwaysHistoricalSociety, CityRail, Countrylink, Freightliner, GenesseeWyoming, GreatSouthernRail, Interail,
        JohnHollandRail, LauchlanValleyRailSociety, PacificNational, QUBE, RailTransportMuseum, RailCorp, SCT, SouthernShorthaulRail,
        SydneyRailService, TheRailMotorSociety, VLinePassenger, 
        /* Commodities. */
        GeneralFreight, Coal, Grain, Mineral, Steel, Clinker, Intermodal, Passenger, Work, GroupRemaining,
        /* Power to weight catagories. */
        Underpowered, Overpowered, Alternative, 
        /* Other */
        Combined, Actual, Simulated, Unknown
    };        

    /// <summary>
    /// A Train class to describe each individual train.
    /// </summary>
    public class Train
    {
        public Category Category;
        public string trainID;
        public string locoID;
        public trainOperator trainOperator;
        public trainCommodity commodity;
        public double powerToWeight;
        public List<TrainJourney> journey;
        public direction trainDirection;
        public bool include;

        /// <summary>
        /// Default train constructor
        /// </summary>
        public Train()
        {
            this.Category = Category.Unknown;
            this.trainID = "none";
            this.locoID = "none";
            this.trainOperator = trainOperator.Unknown;
            this.commodity = trainCommodity.Unknown;
            this.powerToWeight = 0;
            this.journey = new List<TrainJourney>();
            this.trainDirection = direction.Unknown;
            this.include = false;
        }

        /// <summary>
        /// Train constructor for a standard train read from data.
        /// </summary>
        /// <param name="Category">Analysis Category, described by operator or power to weight ratio,</param>
        /// <param name="trainId">The Train ID.</param>
        /// <param name="locoID">The locomotive ID.</param>
        /// <param name="trainOperator">Identification of the train operator.</param>
        /// <param name="commodity">identification of the commidity the train is carrying.</param>
        /// <param name="power">The power to weight ratio of the train.</param>
        /// <param name="journey">The list of journey details describing the points along the trains journey.</param>
        /// <param name="direction">The direction of travel indicated by the direction the kilometreage is progressing.</param>
        /// <param name="include">A flag indicating if the train is to be include in the analysis.</param>
        public Train(Category Category, string trainId, string locoID, trainOperator trainOperator, trainCommodity commodity, double power, List<TrainJourney> journey, direction direction, bool include)
        {
            this.Category = Category;
            this.trainID = trainId;
            this.locoID = locoID;
            this.trainOperator = trainOperator;
            this.commodity = commodity;
            this.powerToWeight = power;
            this.journey = journey;
            this.trainDirection = direction;
            this.include = include;
        }

        /// <summary>
        /// Train constructor for the interpolated train data.
        /// </summary>
        /// <param name="Category">Analysis Category, described by operator or power to weight ratio,</param>
        /// <param name="trainId">The Train ID.</param>
        /// <param name="locoID">The locomotive ID.</param>
        /// <param name="trainOperator">Identification of the train operator.</param>
        /// <param name="commodity">identification of the commidity the train is carrying.</param>
        /// <param name="power">The power to weight ratio of the train.</param>
        /// <param name="journey">The list of journey details describing the points along the trains journey.</param>
        /// <param name="direction">The direction of travel indicated by the direction the kilometreage is progressing.</param>
        public Train(Category Category, string trainId, string locoID, trainOperator trainOperator, trainCommodity commodity, double power, List<TrainJourney> journey, direction direction)
        {
            /* Designed for interpolated train */
            this.Category = Category;
            this.trainID = trainId;
            this.locoID = locoID;
            this.trainOperator = trainOperator;
            this.commodity = commodity;
            this.powerToWeight = power;
            this.journey = journey;
            this.trainDirection = direction;
            this.include = true;
        }

        /// <summary>
        /// Train constructor for the simiulated train data.
        /// </summary>
        /// <param name="journey">The list of journey details describing the points along the trains journey.</param>
        /// <param name="Category">Analysis Category, described by operator or power to weight ratio,</param>
        /// <param name="direction">The direction of travel indicated by the direction the kilometreage is progressing.</param>
        public Train(List<TrainJourney> journey, Category Category, direction direction)
        {
            this.Category = Category;
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
                if (Math.Abs(TrainJourney[journeyIdx].kilometreage - targetKm) * 1e10 < 1)
                    return journeyIdx;
            }

            return -1;
        }

    }

    /// <summary>
    /// A Train Journey class to describe data for each point in trains journey.
    /// </summary>
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

        /// <summary>
        /// Default Train journey constructor
        /// </summary>
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

        /// <summary>
        /// Train journey constructor for train record items after processing.
        /// </summary>
        /// <param name="record">A train record item containing all the information from the data.</param>
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

        /// <summary>
        /// Train journey constructor for a standard train, built from the field in the data.
        /// </summary>
        /// <param name="location">Geolocation object describing the latitude and longitude of a data point</param>
        /// <param name="date">Date and time the data point was registered.</param>
        /// <param name="speed">The instantaneous speed of the train at the time of data recording.</param>
        /// <param name="kmPost">The closest kilometreage marker to the current position.</param>
        /// <param name="kilometreage">The calaculated kilometreage of the current train position.</param>
        /// <param name="elevation">The elevation of the train at the current location, this is taken from the geometry information.</param>
        /// <param name="loop">Identification of the presence of a loop at the current position.</param>
        /// <param name="TSR">Identification of the presence of a TSR at the current position.</param>
        public TrainJourney(GeoLocation location, DateTime date, double speed, double kmPost, double kilometreage, double elevation, bool loop, bool TSR)
        {
            this.location = location;
            this.dateTime = date;
            this.speed = speed;
            this.kmPost = kmPost;
            this.kilometreage = kilometreage;
            this.elevation = elevation;
            this.isLoopHere = loop;
            this.isTSRHere = TSR;
        }

        /// <summary>
        /// Train journey constructor for a train after interpolating the data.
        /// </summary>
        /// <param name="date">Date and time the data point was registered.</param>
        /// <param name="speed">The instantaneous speed of the train at the time of data recording.</param>
        /// <param name="kmPost">The closest kilometreage marker to the current position.</param>
        /// <param name="kilometreage">The calaculated kilometreage of the current train position.</param>
        /// <param name="elevation">The elevation of the train at the current location, this is taken from the geometry information.</param>
        /// <param name="loop">Identification of the presence of a loop at the current position.</param>
        /// <param name="TSR">Identification of the presence of a TSR at the current position.</param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location">Geolocation object describing the latitude and longitude of a data point</param>
        /// <param name="date">Date and time the data point was registered.</param>
        /// <param name="speed">The instantaneous speed of the train at the time of data recording.</param>
        /// <param name="kmPost">The closest kilometreage marker to the current position.</param>
        /// <param name="kilometreage">The calculated kilometreage of the current train position.</param>
        /// <param name="singleLineKm">The calculated consecutive kilometreage of the current train position.</param>
        /// <param name="elevation">The elevation of the train at the current location, this is taken from the geometry information.</param>
        public TrainJourney(GeoLocation location, DateTime date, double speed, double kmPost, double singleLineKm, double elevation)
        {
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

    /// <summary>
    /// Train Record class to record each item from the data.
    /// </summary>
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

        /// <summary>
        /// Default train record constructor.
        /// </summary>
        public TrainRecord()
        {
            this.trainID = null;
            this.locoID = null;
            this.dateTime = DateTime.MinValue;
            this.location = null;
            this.trainOperator = trainOperator.Unknown;
            this.commodity = trainCommodity.Unknown;
            this.kmPost = 0;
            this.speed = 0;
            this.powerToWeight = 0;
        }

        /// <summary>
        /// Train record constructor, built from the fields in the data file.
        /// </summary>
        /// <param name="trainID">The train identification.</param>
        /// <param name="locoID">The lead locomotive identification</param>
        /// <param name="time">The data and time the data point was recorded.</param>
        /// <param name="location">the geographic location of the train.</param>
        /// <param name="Operator">Identification of the train operator.</param>
        /// <param name="commodity">Identification of the commodity the tran is carrying.</param>
        /// <param name="kmPost">The closest kilometreage marker of the current position.</param>
        /// <param name="speed">The instantaneous speed of the train at the time of recording the data.</param>
        /// <param name="power">The power to weight ratio of the train.</param>
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

    /// <summary>
    /// A class to describe the aggregated train data.
    /// </summary>
    public class AverageTrain
    {
        public Category trainCategory;
        public direction direction;
        public int trainCount;
        public List<double> kilometreage;
        public List<double> elevation;
        public List<double> averageSpeed;
        public List<bool> isInLoopBoundary;
        public List<bool> isInTSRboundary;

        /// <summary>
        /// Default average train constructor.
        /// </summary>
        public AverageTrain()
        {
            this.trainCategory = Category.Unknown;
            this.direction = direction.Unknown;
            this.trainCount = 0;
            this.kilometreage = new List<double>();
            this.elevation = new List<double>();
            this.averageSpeed = new List<double>();
            this.isInLoopBoundary = new List<bool>();
            this.isInTSRboundary = new List<bool>();
        }

        /// <summary>
        /// Average train constructor built from the aggregated data.
        /// </summary>
        /// <param name="Category">The aggregation Category of the average data</param>
        /// <param name="direction">The direction of travel of the average train.</param>
        /// <param name="count">The number of train included in the aggregation.</param>
        /// <param name="kilometreage">A list of interpolated kilometreage of the trains journey.</param>
        /// <param name="elevation">A list of elevations at the kilometreage points for the trains journey.</param>
        /// <param name="averageSpeed">The calculted average speed of the train at each kilometreage.</param>
        /// <param name="loop">Identification if the a loop is withing the boundary threshold of the current position.</param>
        /// <param name="TSR">Identification if the a TSR is withing the boundary threshold of the current position.</param>
        public AverageTrain(Category Category, direction direction, int count, List<double> kilometreage, List<double> elevation, List<double> averageSpeed, List<bool> loop, List<bool> TSR)
        {
            this.trainCategory = Category;
            this.direction = direction;
            this.trainCount = count;
            this.kilometreage = kilometreage;
            this.elevation = elevation;
            this.averageSpeed = averageSpeed;
            this.isInLoopBoundary = loop;
            this.isInTSRboundary = TSR;
        }

    }

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

        /// <summary>
        /// Geolocation constructor
        /// </summary>
        /// <param name="record">Train record item containing the latitude and longitude.</param>
        public GeoLocation(TrainRecord record)
        {
            this.latitude = record.location.latitude;
            this.longitude = record.location.longitude;
        }

        /// <summary>
        /// Geolocation constructor
        /// </summary>
        /// <param name="journey">Train journey item containing the latitude and longitude.</param>
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
        ///* Create a processing object. */
        public static Processing processing = new Processing();
        ///* Create a trackGeometry object. */
        public static TrackGeometry track = new TrackGeometry();
        
        /// <summary>
        /// Determine the average train performance in both directions based on the supplied 
        /// actual train data. The form allows the user to specify which parameters will be 
        /// used to analyse the data. These can be the train operator, the power to weight 
        /// ratios, and the commodity the train carries.
        /// 
        /// The loop locations and TSR information is also used to extract the data that 
        /// corresponds to a train that enteres a loop and is bound by a TSR. If the train 
        /// is within the 'loop bounday threshold' and is deemed to be stopping in the loop, 
        /// the data at this location is not included. The train is deemed to be stopping in 
        /// a loop if the train speed drops below the simulated speed multiplied by the 
        /// 'loop speed factor'. If the train is within the 'TSR window', the data at this 
        /// location is ignored as the train is bound by the TSR at the location. The average 
        /// train is then determined from the included train data.
        /// 
        /// This function produces a file containing the interpolated data for each train 
        /// and a file containing the aggregated information for each analysis Category.
        /// </summary>        
        [STAThread]
        public static List<Train> trainPerformance()
        {

            /* Ensure there is a empty list of trains to exclude to start. */
            List<string> excludeTrainList = new List<string> { };

            /* Populate the exluded train list. */
            if (FileSettings.trainListFile != null)
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

            /* Identify the number of operators. */
            List<trainOperator> operators = TrainRecords.Select(t => t.trainOperator).Distinct().ToList();
            operators.Remove(trainOperator.Unknown);
            int numberOfOperators = operators.Count();


            /* Create a list of analysis Categories */
            List<Category> simCategories = new List<Category>();

            /* Set the analysis paramteres. */
            if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
            {
                simCategories.Add(Category.Underpowered);
                simCategories.Add(Category.Overpowered);
            }
            else if (Settings.analysisCategory == analysisCategory.TrainOperator)
            {
                if (Settings.Category1Operator != trainOperator.Unknown)
                    simCategories.Add(convertTrainOperatorToCategory(Settings.Category1Operator));

                if (Settings.Category2Operator != trainOperator.Unknown)
                    simCategories.Add(convertTrainOperatorToCategory(Settings.Category2Operator));

                if (Settings.Category3Operator != trainOperator.Unknown)
                    simCategories.Add(convertTrainOperatorToCategory(Settings.Category3Operator));

            }
            else
            {
                /* analysisCategory is commodities. */
                if (Settings.Category1Commodity != trainCommodity.Unknown)
                    simCategories.Add(convertCommodityToCategory(Settings.Category1Commodity));

                if (Settings.Category2Commodity != trainCommodity.Unknown)
                    simCategories.Add(convertCommodityToCategory(Settings.Category2Commodity));

                if (Settings.Category3Commodity != trainCommodity.Unknown)
                    simCategories.Add(convertCommodityToCategory(Settings.Category3Commodity));

            }

            /* Create the list of simulated trains. */
            List<Train> simulatedTrains = new List<Train>();

            /* Read in the simulation data and interpolate to the desired granularity. */
            for (int index = 0; index < simCategories.Count(); index++)
            {
                simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.simulationFiles[index * 2], simCategories[index], direction.IncreasingKm));
                simulatedTrains.Add(FileOperations.readSimulationData(FileSettings.simulationFiles[index * 2 + 1], simCategories[index], direction.DecreasingKm));
            }
            
            /* Interpolate the simulations to the same granularity as the ICE data will be. */
            List<Train> interpolatedSimulations = new List<Train>();
            interpolatedSimulations = processing.interpolateTrainData(simulatedTrains, trackGeometry);
            
            /* Sort the data by [trainID, locoID, Date & Time, kmPost]. */
            List<TrainRecord> OrderdTrainRecords = new List<TrainRecord>();
            OrderdTrainRecords = TrainRecords.OrderBy(t => t.trainID).ThenBy(t => t.locoID).ThenBy(t => t.dateTime).ThenBy(t => t.kmPost).ToList();


            /**************************************************************************************************/
            /* Clean data - remove trains with insufficient data. */
            /******** Should only be required while we are waiting for the data in the prefered format ********/

            //List<Train> testTrainRecords = new List<Train>();
            //testTrainRecords = MakeTrains(trackGeometry, OrderdTrainRecords, TSRs);

            List<Train> CleanTrainRecords = new List<Train>();
            CleanTrainRecords = CleanData(OrderdTrainRecords, trackGeometry);


            /* Interpolate data */
            /******** Should only be required while we are waiting for the data in the prefered format ********/
            List<Train> interpolatedTrains = new List<Train>();
            interpolatedTrains = processing.interpolateTrainData(CleanTrainRecords, trackGeometry);
            /**************************************************************************************************/
            
            /* Populate the trains TSR values after interpolation to gain more granularity with TSR boundary. */
            processing.populateAllTrainsTemporarySpeedRestrictions(interpolatedTrains, TSRs);

            /* Write the interpolated data to file. */
            FileOperations.writeTrainData(interpolatedTrains);

            /* Create the list of averaged trains */
            List<AverageTrain> averageTrains = new List<AverageTrain>();
            /* Create a sublist of trains for each direction. */
            List<Train> increasingTrainCategory = new List<Train>();
            List<Train> decreasingTrainCategory = new List<Train>();

            List<Statistics> stats = new List<Statistics>();

            /* Cycle through each train category. */
            for (int index = 0; index < simCategories.Count(); index++)
            {
                if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
                {
                    /* Create a list for each category. */
                    increasingTrainCategory = interpolatedTrains.Where(t => t.Category == simCategories[index]).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                    decreasingTrainCategory = interpolatedTrains.Where(t => t.Category == simCategories[index]).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                    
                }
                else if (Settings.analysisCategory == analysisCategory.TrainOperator)
                {
                    /* Convert the train category to the train operator. */
                    trainOperator operatorCategory = convertCategoryToTrainOperator(simCategories[index]);
                    
                    /* Create a list for each operator. */
                    if (operatorCategory !=  trainOperator.GroupRemaining)
                    {
                        increasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == operatorCategory).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        decreasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == operatorCategory).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                    }
                    else
                    {
                        /* Create a list for all operators. */
                        increasingTrainCategory = interpolatedTrains.Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        decreasingTrainCategory = interpolatedTrains.Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                        for (int groupIdx = 0; groupIdx < simCategories.Count(); groupIdx++)
                        {
                            if (groupIdx != index)
                            {
                                /* Remove the specified operators from the list so they aren't counted twice. */
                                operatorCategory = convertCategoryToTrainOperator(simCategories[groupIdx]);
                                increasingTrainCategory = increasingTrainCategory.Where(t => t.trainOperator != operatorCategory).ToList();
                                decreasingTrainCategory = decreasingTrainCategory.Where(t => t.trainOperator != operatorCategory).ToList();
                            }
                        }
                        /* Reset the operator to grouped for the analysis */
                        setOperatorToGrouped(increasingTrainCategory);
                        setOperatorToGrouped(decreasingTrainCategory);
                    }

                }
                else
                {
                    /* Convert the train category to the commodity. */
                    trainCommodity commodity = convertCategoryToCommodity(simCategories[index]);

                    /* Create a list for each commodity. */
                    if (commodity != trainCommodity.GroupRemaining)
                    {
                        increasingTrainCategory = interpolatedTrains.Where(t => t.commodity == commodity).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        decreasingTrainCategory = interpolatedTrains.Where(t => t.commodity == commodity).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                    }
                    else
                    {
                        /* Create a list for all commodities. */
                        increasingTrainCategory = interpolatedTrains.Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        decreasingTrainCategory = interpolatedTrains.Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                        for (int groupIdx = 0; groupIdx < simCategories.Count(); groupIdx++)
                        {
                            if (groupIdx != index)
                            {
                                /* Remove the specified commodities from the list so they arent counted twice. */
                                commodity = convertCategoryToCommodity(simCategories[groupIdx]);
                                increasingTrainCategory = increasingTrainCategory.Where(t => t.commodity != commodity).ToList();
                                decreasingTrainCategory = decreasingTrainCategory.Where(t => t.commodity != commodity).ToList();
                            }
                        }
                        /* Reset the operator to grouped for the analysis */
                        setOperatorToGrouped(increasingTrainCategory);
                        setOperatorToGrouped(decreasingTrainCategory);
                    }

                }

                /* Generate statistics for the lists. */
                stats.Add(Statistics.generateStats(increasingTrainCategory));
                stats.Add(Statistics.generateStats(decreasingTrainCategory));

                if (increasingTrainCategory.Count() == 0 || decreasingTrainCategory.Count() == 0)
                {
                    stats[stats.Count() - 1].Category = simCategories[index].ToString() + " " + direction.DecreasingKm.ToString();
                    stats[stats.Count() - 2].Category = simCategories[index].ToString() + " " + direction.IncreasingKm.ToString();
                }

                /* Aggregate the train lists into an average train consistent with the specified Category. */
                if (increasingTrainCategory.Count() > 0)
                    averageTrains.Add(processing.averageTrain(increasingTrainCategory, interpolatedSimulations[index * 2].journey, trackGeometry));
                else
                    averageTrains.Add(createZeroedAverageTrain(simCategories[index], direction.IncreasingKm));
                
                if (decreasingTrainCategory.Count() > 0)
                    averageTrains.Add(processing.averageTrain(decreasingTrainCategory, interpolatedSimulations[index * 2 + 1].journey, trackGeometry));
                else
                    averageTrains.Add(createZeroedAverageTrain(simCategories[index], direction.DecreasingKm));
                
            }

            /* Add the weighted average trains to the list. */
            List<Train> increasingCombined = new List<Train>();
            List<Train> decreasingCombined = new List<Train>();

            /* Combine the analysis categories for a combined weighted average train. */
            if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
            {
                /* Create a list for each direction. */
                List<Train> increasingSubList = new List<Train>();
                List<Train> decreasingSubList = new List<Train>();

                /* Cycle through each category to add to the list. */
                foreach (Category simCategory in simCategories)
                {
                    increasingSubList = interpolatedTrains.Where(t => t.Category == simCategory).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                    increasingCombined.AddRange(increasingSubList);
                    decreasingSubList = interpolatedTrains.Where(t => t.Category == simCategory).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                    decreasingCombined.AddRange(decreasingSubList);
                }

                setOperatorToCombined(increasingCombined);
                setOperatorToCombined(decreasingCombined);
                
            }
            else if (Settings.analysisCategory == analysisCategory.TrainOperator)
            {               
                /* Create a list for each direction. */
                List<Train> increasingSubList = new List<Train>();
                List<Train> decreasingSubList = new List<Train>();

                /* If all commodities are used, group them in each direction. */
                if (simCategories.Contains(Category.GroupRemaining))
                {
                    increasingCombined = interpolatedTrains.Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                    decreasingCombined = interpolatedTrains.Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                }
                else
                {
                    /* Cycle through each commodity to add to the list. */
                    foreach (Category simCategory in simCategories)
                    {
                        increasingSubList = interpolatedTrains.Where(t => t.trainOperator == convertCategoryToTrainOperator(simCategory)).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        increasingCombined.AddRange(increasingSubList);
                        decreasingSubList = interpolatedTrains.Where(t => t.trainOperator == convertCategoryToTrainOperator(simCategory)).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                        decreasingCombined.AddRange(decreasingSubList);
                    }
                }
                setOperatorToCombined(increasingCombined);
                setOperatorToCombined(decreasingCombined);

            }
            else
            {
                /* Create a list for each direction. */
                List<Train> increasingSubList = new List<Train>();
                List<Train> decreasingSubList = new List<Train>();
                
                /* If all commodities are used, group them in each direction. */
                if (simCategories.Contains(Category.GroupRemaining))
                {
                    increasingCombined = interpolatedTrains.Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                    decreasingCombined = interpolatedTrains.Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                }
                else
                {
                    /* Cycle through each commodity to add to the list. */
                    foreach (Category simCategory in simCategories)
                    {
                        increasingSubList = interpolatedTrains.Where(t => t.commodity == convertCategoryToCommodity(simCategory)).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        increasingCombined.AddRange(increasingSubList);
                        decreasingSubList = interpolatedTrains.Where(t => t.commodity == convertCategoryToCommodity(simCategory)).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                        decreasingCombined.AddRange(decreasingSubList);
                    }
                }
                setOperatorToCombined(increasingCombined);
                setOperatorToCombined(decreasingCombined);

            }

            /* Generate statistics for the weighted average trains. */
            stats.Add(Statistics.generateStats(increasingCombined));
            stats.Add(Statistics.generateStats(decreasingCombined));

            if (increasingCombined.Count() == 0 || decreasingCombined.Count() == 0)
            {
                stats[stats.Count() - 1].Category = "Combined " + direction.DecreasingKm.ToString();
                stats[stats.Count() - 2].Category = "Combined " + direction.IncreasingKm.ToString();
            }

            /* Create a weighted average simulation */
            List<Train> weightedSimulation = new List<Train>();
            weightedSimulation = Processing.getWeightedAverageSimulation(interpolatedSimulations, averageTrains);

            /* Calculate the weighted average train in each direction. */
            if (weightedSimulation.Count() >= 2)
            {
                if (increasingCombined.Count() == 0)
                    averageTrains.Add(createZeroedAverageTrain(Category.Combined, direction.IncreasingKm));
                else
                    averageTrains.Add(processing.averageTrain(increasingCombined, weightedSimulation[0].journey, trackGeometry));

                if (decreasingCombined.Count() == 0)
                    averageTrains.Add(createZeroedAverageTrain(Category.Combined, direction.DecreasingKm));
                else
                    averageTrains.Add(processing.averageTrain(decreasingCombined, weightedSimulation[1].journey, trackGeometry));
            }
            else
            {
                string error = "There aren't enough weighted simulation files to proceed.";
                Console.WriteLine(error);

                throw new ArgumentException(error);
            }

            /* Write the averaged Data to file for inspection. */            
            FileOperations.wrtieAverageData(averageTrains, stats);
            
            return interpolatedTrains;
        }

        /// <summary>
        /// This function cleans the data from large gaps in the data and ensures the trains 
        /// are all travelling in a single direction with a minimum total distance.
        /// </summary>
        /// <param name="record">List of Train record objects</param>
        /// <param name="trackGeometry">A list of track Geometry objects</param>
        /// <returns>List of Train objects containing the journey details of each train.</returns>
        public static List<Train> CleanData(List<TrainRecord> record, List<TrackGeometry> trackGeometry)
        {
            /* Note: this function will not be needed when Enterprise Services delivers the interpolated 
             * date directly to the database. We can access this data directly, then analyse.
             */

            bool removeTrain = false;
            double distance = 0;
            double journeyDistance = 0;

            /* Create the lists for the processed train data. */
            List<Train> cleanTrainList = new List<Train>();
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

                    /* Populate the train parameters. */
                    Train item = new Train();
                    item.journey = journey;
                    item.trainDirection = processing.getTrainDirection(item);

                    /* remove the train if the direction is not valid. */
                    if (item.trainDirection == direction.Invalid)
                        removeTrain = true;

                    /* The end of the train journey has been reached. */
                    if (!removeTrain && journeyDistance > Settings.minimumJourneyDistance)
                    {
                        /* If all points are acceptable and the train travels the minimum distance, 
                         * add the train journey to the cleaned list. 
                         */
                        item.trainID = record[trainIndex - 1].trainID;
                        item.locoID = record[trainIndex - 1].locoID;
                        item.trainOperator = record[trainIndex - 1].trainOperator;
                        item.commodity = record[trainIndex - 1].commodity;
                        item.powerToWeight = record[trainIndex - 1].powerToWeight;

                        /* Determine the analysis Category. */
                        if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
                        {
                            if (item.powerToWeight > Settings.Category1LowerBound && item.powerToWeight <= Settings.Category1UpperBound)
                                item.Category = Category.Underpowered;
                            else if (item.powerToWeight > Settings.Category2LowerBound && item.powerToWeight <= Settings.Category2UpperBound)
                                item.Category = Category.Overpowered;
                            else
                                item.Category = Category.Actual;

                        }
                        else if (Settings.analysisCategory == analysisCategory.TrainOperator)
                        {
                            item.Category = convertTrainOperatorToCategory(item.trainOperator);
                        }
                        else
                        {
                            item.Category = convertCommodityToCategory(item.commodity);
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

                }

                /* The end of the records have been reached. */
                if (trainIndex == record.Count() - 1 && !removeTrain)
                {
                    /* Check uni directionality of the last train */
                    journey = processing.longestDistanceTravelledInOneDirection(journey, trackGeometry);
                    /* Calculate the total length of the journey */
                    journeyDistance = processing.calculateTrainJourneyDistance(journey);
                    
                    /* Populate the train parameters. */
                    Train lastItem = new Train();
                    lastItem.journey = journey;
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

                        /* Determine the analysis Category. */
                        if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
                        {
                            if (lastItem.powerToWeight > Settings.Category1LowerBound && lastItem.powerToWeight <= Settings.Category1UpperBound)
                                lastItem.Category = Category.Underpowered;
                            else if (lastItem.powerToWeight > Settings.Category2LowerBound && lastItem.powerToWeight <= Settings.Category2UpperBound)
                                lastItem.Category = Category.Overpowered;
                            else
                                lastItem.Category = Category.Actual;

                        }
                        else if (Settings.analysisCategory == analysisCategory.TrainOperator)
                        {
                            lastItem.Category = convertTrainOperatorToCategory(lastItem.trainOperator);
                        }
                        else
                        {
                            lastItem.Category = convertCommodityToCategory(lastItem.commodity);
                        }

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
        
        /// <summary>
        /// This function creates the individual train journeies and adds them to the list. The 
        /// function ensures the train is consistent and has a single direction of travel.
        /// </summary>
        /// <param name="record">List of Train record objects</param>
        /// <param name="trackGeometry">A list of track Geometry objects</param>
        /// <returns>List of Train objects containing the journey details of each train.</returns>
        public static List<Train> MakeTrains(List<TrainRecord> record, List<TrackGeometry> trackGeometry)
        {
            /* Note: this function is designed to replace the cleanTrains function when the 
             * interpolated data is delivered by Enterprise Services.
             */

            /* Create the lists for the processed train data. */
            List<Train> TrainList = new List<Train>();
            List<TrainJourney> journey = new List<TrainJourney>();
            
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

                }
                else
                {
                    /* The end of the train journey has been reached. */

                    /* Check uni directionality of the train */
                    journey = processing.longestDistanceTravelledInOneDirection(journey, trackGeometry);

                    /* Assign the train parameters. */
                    Train item = new Train();
                    item.journey = journey;
                    item.trainDirection = processing.getTrainDirection(item);
                    item.trainID = record[trainIndex - 1].trainID;
                    item.locoID = record[trainIndex - 1].locoID;
                    item.trainOperator = record[trainIndex - 1].trainOperator;
                    item.commodity = record[trainIndex - 1].commodity;
                    item.powerToWeight = record[trainIndex - 1].powerToWeight;

                    /* Determine the train Category. */
                    if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
                    {
                        if (item.powerToWeight > Settings.Category1LowerBound && item.powerToWeight <= Settings.Category1UpperBound)
                            item.Category = Category.Underpowered;
                        else if (item.powerToWeight > Settings.Category2LowerBound && item.powerToWeight <= Settings.Category2UpperBound)
                            item.Category = Category.Overpowered;
                        else
                            item.Category = Category.Actual;

                    }
                    else if (Settings.analysisCategory == analysisCategory.TrainOperator)
                    {
                        item.Category = convertTrainOperatorToCategory(item.trainOperator);
                    }
                    else
                    {
                        item.Category = convertCommodityToCategory(item.commodity);
                    }


                    /* Determine the actual km, and populate the loops information. */
                    processing.populateGeometryKm(item.journey, trackGeometry);
                    processing.populateLoopLocations(item.journey, trackGeometry);

                    /* Sort the journey in ascending order. */
                    item.journey = item.journey.OrderBy(t => t.kilometreage).ToList();

                    TrainList.Add(item);

                    /* Reset the parameters for the next train. */
                    journey.Clear();

                    /* Add the first record of the new train journey. */
                    journey.Add(new TrainJourney(record[trainIndex]));

                }

                /* The end of the records have been reached. */

                /* Check uni directionality of the last train */
                journey = processing.longestDistanceTravelledInOneDirection(journey, trackGeometry);

                /*  Assign the train parameters. */
                Train lastItem = new Train();

                lastItem.journey = journey;
                lastItem.trainDirection = processing.getTrainDirection(lastItem);

                lastItem.trainID = record[trainIndex - 1].trainID;
                lastItem.locoID = record[trainIndex - 1].locoID;
                lastItem.trainOperator = record[trainIndex - 1].trainOperator;
                lastItem.commodity = record[trainIndex - 1].commodity;
                lastItem.powerToWeight = record[trainIndex - 1].powerToWeight;

                /* Determine the train Category. */
                if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
                {
                    if (lastItem.powerToWeight > Settings.Category1LowerBound && lastItem.powerToWeight <= Settings.Category1UpperBound)
                        lastItem.Category = Category.Underpowered;
                    else if (lastItem.powerToWeight > Settings.Category2LowerBound && lastItem.powerToWeight <= Settings.Category2UpperBound)
                        lastItem.Category = Category.Overpowered;
                    else
                        lastItem.Category = Category.Actual;

                }
                else if (Settings.analysisCategory == analysisCategory.TrainOperator)
                {
                    lastItem.Category = convertTrainOperatorToCategory(lastItem.trainOperator);
                }
                else
                {
                    lastItem.Category = convertCommodityToCategory(lastItem.commodity);
                }

                /* Determine the actual km, and populate the loops information.  */
                processing.populateGeometryKm(lastItem.journey, trackGeometry);
                processing.populateLoopLocations(lastItem.journey, trackGeometry);

                /* Sort the journey in ascending order. */
                lastItem.journey = lastItem.journey.OrderBy(t => t.kilometreage).ToList();

                TrainList.Add(lastItem);

            }

            return TrainList;
        }

        /// <summary>
        /// Convert The analysis Category to the train Operator.
        /// </summary>
        /// <param name="Category">The analsyis Category.</param>
        /// <returns>The train operator corresponding to the analysis Category.</returns>
        private static trainOperator convertCategoryToTrainOperator(Category Category)
        {
            trainOperator trainOperator = trainOperator.Unknown;

            /* Extract the list of train operators. */
            List<trainOperator> operatorList = Enum.GetValues(typeof(trainOperator)).Cast<trainOperator>().ToList();

            /* Match the opertor to the Category. */
            foreach (trainOperator Operator in operatorList)
            {
                if (Operator.ToString().Equals(Category.ToString()))
                    trainOperator = Operator;
            }

            return trainOperator;
        }

        /// <summary>
        /// Convert The train Category to the train commodity.
        /// </summary>
        /// <param name="Category">The analsyis Category.</param>
        /// <returns>The train commodity corresponding to the analysis Category.</returns>
        private static trainCommodity convertCategoryToCommodity(Category Category)
        {
            trainCommodity trainCommodity = trainCommodity.Unknown;

            /* Extract the list of train operators. */
            List<trainCommodity> commodityList = Enum.GetValues(typeof(trainCommodity)).Cast<trainCommodity>().ToList();

            /* Match the opertor to the Category. */
            foreach (trainCommodity commodity in commodityList)
            {
                if (commodity.ToString().Equals(Category.ToString()))
                    trainCommodity = commodity;
            }

            return trainCommodity;
        }

        /// <summary>
        /// Convert the train operator to the analysis Category.
        /// </summary>
        /// <param name="trainOperator">The train operator.</param>
        /// <returns>The analysis Category corresponding to the train operator.</returns>
        private static Category convertTrainOperatorToCategory(trainOperator trainOperator)
        {
            Category trainCategory = Category.Unknown;

            /* Extract the list of Categories. */
            List<Category> CategoryList = Enum.GetValues(typeof(Category)).Cast<Category>().ToList();

            /* Match the Category to the opertor. */
            foreach (Category cat in CategoryList)
            {
                if (cat.ToString().Equals(trainOperator.ToString()))
                    trainCategory = cat;
            }

            return trainCategory;
        }

        /// <summary>
        /// Convert the train commodity to the analysis Category.
        /// </summary>
        /// <param name="trainOperator">The train operator.</param>
        /// <returns>The analysis Category corresponding to the train operator.</returns>
        private static Category convertCommodityToCategory(trainCommodity commodity)
        {
            Category trainCategory = Category.Unknown;

            /* Extract the list of Categories. */
            List<Category> CategoryList = Enum.GetValues(typeof(Category)).Cast<Category>().ToList();

            /* Match the Category to the opertor. */
            foreach (Category cat in CategoryList)
            {
                if (cat.ToString().Equals(commodity.ToString()))
                    trainCategory = cat;
            }

            return trainCategory;
        }

        /// <summary>
        /// Set the Train operator to the combination of the other Categories for full aggregation.
        /// </summary>
        /// <param name="combined">The list of trains to convert the operator to combined.</param>
        private static void setOperatorToCombined(List<Train> combined)
        {
            foreach (Train train in combined)
            {
                train.Category = Category.Combined;
            }
        }

        /// <summary>
        /// Set the Train operator to the group remaining Categories for full aggregation.
        /// </summary>
        /// <param name="combined">The list of trains to convert the operator to grouped.</param>
        private static void setOperatorToGrouped(List<Train> trains)
        {
            foreach (Train train in trains)
            {
                train.Category = Category.GroupRemaining;
            }
        }

        /// <summary>
        /// Creates an empty average train when there are no trains in the list to aggregate.
        /// </summary>
        /// <param name="trainCategory">The anlaysis Category where there are no trains in the list.</param>
        /// <param name="direction">The empty trains direction of travel.</param>
        /// <returns></returns>
        private static AverageTrain createZeroedAverageTrain(Category trainCategory, direction direction)
        {
            /* Determine the number of points in the average train journey. */
            int size = (int)((Settings.endKm - Settings.startKm) / (Settings.interval / 1000));

            int trainCount = 0;
            List<double> kilometreage = new List<double>(size);
            List<double> elevation = new List<double>(size);
            List<double> averageSpeed = new List<double>(size);
            List<bool> isInLoopBoundary = new List<bool>(size);
            List<bool> isInTSRboundary = new List<bool>(size);

            /* Set all properties to 0 or false. */
            for (int index = 0; index < size; index++)
            {
                kilometreage.Add(Settings.startKm + Settings.interval / 1000 * index);
                elevation.Add(0);
                averageSpeed.Add(0);
                isInLoopBoundary.Add(false);
                isInTSRboundary.Add(false);
            }

            return new AverageTrain(trainCategory, direction, trainCount, kilometreage, elevation, averageSpeed, isInLoopBoundary, isInTSRboundary);
        }

    } // Class Algorithm
}
