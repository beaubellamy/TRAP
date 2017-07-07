using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRAP;

namespace Globalsettings
{
    /* A list of available analysis seperation Categories. */
    public enum analysisCategory { TrainOperator, TrainCommodity, TrainPowerToWeight , Unknown};

    public static class FileSettings
    {
        /* Filenames for each required file. */
        public static string dataFile = null;
        public static string geometryFile = null;
        public static string temporarySpeedRestrictionFile = null;
        public static string trainListFile = null;                  /* File only required if includeAListOfTrainsToExclude is TRUE. */
        /* Default number of simulation Categories is 3, hence, the default number of simulation files is 6, one for each direction */
        public static List<string> simulationFiles = new List<string>(new string[6]);
        public static string aggregatedDestination = null;

    }

    public static class Settings
    {
        
        /* Data boundaries */
        public static DateTime[] dateRange;                 /* Date range of data to include. */
        public static GeoLocation topLeftLocation;          /* Top left corner of the geographic box describing the included data. */
        public static GeoLocation bottomRightLocation;      /* Bottom right corner of the geographic box describing the included data. */
        public static bool excludeListOfTrains;             /* Is a list of trains that are to be excluded available. */

        /* Corridor dependant / Analysis parameters */
        public static double startKm;                       /* Start km for interpoaltion data. */
        public static double endKm;                         /* End km for interpolation data. */
        public static double interval;                      /* Interpolation interval (metres). */
        public static double minimumJourneyDistance;        /* Minimum distance of a train journey to be considered valid. */

        /* Processing parameters */
        public static double loopSpeedThreshold;            /* Cuttoff for the simulation speed, when comparing the train to the simualted train. */
        public static double loopBoundaryThreshold;         /* Distance either side of the loop to be considered within the loop boundary (km). */
        public static double TSRwindowBoundary;             /* Distance either side of the TSR location to be considered within the TSR boundary (km). */
        public static double timeThreshold;                 /* Minimum time between data points to be considered a seperate train. */
        public static double distanceThreshold;             /* Minimum distance between successive data points. */

        /* Simulation Parameters */
        public static double Category1LowerBound;           /* The lower bound cuttoff for the underpowered trains. */
        public static double Category1UpperBound;           /* The upper bound cuttoff for the underpowered trains. */
        public static double Category2LowerBound;           /* The lower bound cuttoff for the overpowered trains. */
        public static double Category2UpperBound;           /* The upper bound cuttoff for the overpowered trains. */

        public static double proRataTSRRatio;               /* The ratio to apply to the simulated speed to replace the actual speed for TSR locations. */

        public static analysisCategory analysisCategory;
        public static trainOperator Category1Operator = trainOperator.Unknown;
        public static trainOperator Category2Operator = trainOperator.Unknown;
        public static trainOperator Category3Operator = trainOperator.Unknown;
        public static trainCommodity Category1Commodity = trainCommodity.Unknown;
        public static trainCommodity Category2Commodity = trainCommodity.Unknown;
        public static trainCommodity Category3Commodity = trainCommodity.Unknown;

        
    }



}
