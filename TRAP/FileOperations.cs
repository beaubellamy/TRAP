using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globalsettings;

namespace TRAP
{
    class FileOperations
    {

        public static List<TrainRecord> readICEData(string filename, List<string> excludeTrainList)
        {
            /* Read all the lines of the data file. */
            isFileOpen(filename);

            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { ',', '\t' };

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            /* Initialise the fields of interest. */
            string subOperator = "";
            string TrainID = "none";
            string locoID = "none";
            trainOperator trainOperator = trainOperator.unknown;
            trainCommodity commodity = trainCommodity.unknown;
            double powerToWeight = 0.0;
            double speed = 0.0;
            double kmPost = 0.0;
            //double geometryKm = 0.0;
            double latitude = 0.0;
            double longitude = 0.0;
            DateTime dateTime = DateTime.MinValue;
            //double elevation = 0.0;

            bool header = true;
            bool includeTrain = true;

            /* List of all valid train data. */
            List<TrainRecord> IceRecord = new List<TrainRecord>();

            foreach (string line in lines)
            {
                if (header)
                    /* Ignore the header line. */
                    header = false;
                else
                {
                    /* Seperate each record into each field */
                    fields = line.Split(delimeters);

                    TrainID = fields[8];
                    locoID = fields[3];

                    if (fields[0].Count() >= 3) //operator
                        subOperator = fields[0].Substring(0, 3);

                    trainOperator = getOperator(subOperator);

                    /* Ensure values are valid while reading them out. */
                    double.TryParse(fields[14], out speed);
                    double.TryParse(fields[11], out kmPost);
                    double.TryParse(fields[2], out latitude);
                    double.TryParse(fields[4], out longitude);
                    DateTime.TryParse(fields[6], out dateTime);
                    double.TryParse(fields[18], out powerToWeight);

                    /* possible TSR information as well*/
                    /* TSR region
                     * Start km
                     * end km
                     * TSR issue Data
                     * TSR lift date
                     */

                    /* Check if the train is in the exclude list */
                    includeTrain = excludeTrainList.Contains(TrainID);

                    if (latitude < Settings.topLeftLocation.latitude && latitude > Settings.bottomRightLocation.latitude &&
                        longitude > Settings.topLeftLocation.longitude && longitude < Settings.bottomRightLocation.longitude &&
                        dateTime >= Settings.dateRange[0] && dateTime < Settings.dateRange[1] &&
                        !includeTrain)
                    {
                        TrainRecord record = new TrainRecord(TrainID, locoID, dateTime, new GeoLocation(latitude, longitude), trainOperator, commodity, kmPost, speed, powerToWeight);
                        IceRecord.Add(record);
                    }

                }
            }

            /* Return the list of records. */
            return IceRecord;
        }


        /// <summary>
        /// This function reads the file with the list of trains to exclude from the 
        /// data and stores the list in a managable list object.
        /// The file is assumed to have one train per line or have each train seperated 
        /// by a common delimiter [ , \ " \t \n]
        /// </summary>
        /// <param name="filename">The full path of the file containing the list of trains to exclude.</param>
        /// <returns>The populated list of all trains to exclude.</returns>
        public static List<string> readTrainList(string filename)
        {
            List<string> excludeTrainList = new List<string>();

            /* Read all the lines of the file. */
            isFileOpen(filename);

            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { ',', '\t', '\n' };

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            /* Add the trains to the list. */
            foreach (string line in lines)
                excludeTrainList.Add(line);

            return excludeTrainList;
        }

        /// <summary>
        /// Function reads in the track geometry data from file.
        /// </summary>
        /// <param name="filename">Full filename of the geometry file.</param>
        /// <returns>A list of track Geometry objects describing the track geometry.</returns>
        public static List<TrackGeometry> readGeometryfile(string filename)
        {
            Processing processing = new Processing();

            /* Create the list of track geometry objects. */
            List<TrackGeometry> trackGeometry = new List<TrackGeometry>();

            bool header = true;

            /* Read all the lines of the file. */
            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { ',','\t'}; 

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            bool firstPoint = true;

            /* Define the track geomerty parameters. */
            string geometryName = null;
            double latitude = 0.0;
            double longitude = 0.0;
            double elevation = 0.0;
            double kilometreage = 0.0;
            double virtualKilometreage = 0.0;
            bool isLoopHere = false;

            /* Define some additional helper parameters. */
            double distance = 0;
            direction direction = direction.notSpecified;
            double previousLat = 0;
            double previousLong = 0;
            double previouskm = 0;
            string loop;

            /* Add the trains to the list. */
            foreach (string line in lines)
            {
                if (header)
                    /* Ignore the header line. */
                    header = false;
                else
                {
                    /* Seperate each record into each field */
                    fields = line.Split(delimeters);
                    geometryName = fields[0];
                    double.TryParse(fields[1], out latitude);
                    double.TryParse(fields[2], out longitude);
                    double.TryParse(fields[3], out elevation);
                    double.TryParse(fields[4], out kilometreage);
                    loop = fields[6];

                    if (loop.Equals("loop", StringComparison.OrdinalIgnoreCase) || loop.Equals("true", StringComparison.OrdinalIgnoreCase))
                        isLoopHere = true;
                    else
                        isLoopHere = false;

                    /* The virtual kilometreage starts at the first kilometreage of the track. */
                    if (firstPoint)
                    {
                        virtualKilometreage = kilometreage;
                        /* Set the 'pervious' parameters. */
                        previousLat = latitude;
                        previousLong = longitude;
                        previouskm = kilometreage;
                        firstPoint = false;
                    }
                    else
                    {
                        /* Determine the direction for the track kilometreage. */
                        if (direction == direction.notSpecified)
                        {
                            if (kilometreage - previouskm > 0)
                                direction = direction.increasing;
                            else
                                direction = direction.decreasing;
                        }

                        /* Calcualte the distance between succesive points and increment the virtual kilometreage. */
                        distance = processing.calculateGreatCircleDistance(previousLat, previousLong, latitude, longitude);

                        if (direction == direction.increasing)
                            virtualKilometreage = virtualKilometreage + distance / 1000;

                        else
                            virtualKilometreage = virtualKilometreage - distance / 1000;

                        /* Set the 'previous' parameters. */
                        previousLat = latitude;
                        previousLong = longitude;

                    }

                    /* Add the geometry point to the list. */
                    TrackGeometry geometry = new TrackGeometry(0, geometryName, latitude, longitude, elevation, kilometreage, virtualKilometreage, isLoopHere);
                    trackGeometry.Add(geometry);
                    
                }
            }


            return trackGeometry;
        }

        /// <summary>
        /// Read the file containing the temporary speed restriction information and 
        /// store in a manalgable list of TSR objects, which contain all neccessary 
        /// information for each TSR.
        /// </summary>
        /// <param name="filename">TSR file</param>
        /// <returns>List of TSR objects contianting the parameters for each TSR.</returns>
        public static List<TSRObject> readTSRFile(string filename)
        {
            /* Read all the lines of the data file. */
            isFileOpen(filename);

            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { ',', '\t' };

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            /* Initialise the fields of interest. */
            string region = "none";
            DateTime issueDate = DateTime.MinValue;
            DateTime liftedDate = DateTime.MinValue;
            double startKm = 0.0;
            double endKm = 0.0;
            double speed = 0.0;

            bool header = true;

            /* List of all TSR details. */
            List<TSRObject> TSRList = new List<TSRObject>();

            foreach (string line in lines)
            {
                if (header)
                    /* Ignore the header line. */
                    header = false;
                else
                {
                    /* Seperate each record into each field */
                    fields = line.Split(delimeters);

                    region = fields[0];
                    /* needs to perform tests */
                    DateTime.TryParse(fields[1], out issueDate);
                    DateTime.TryParse(fields[2], out liftedDate);
                    double.TryParse(fields[10], out startKm);
                    double.TryParse(fields[11], out endKm);
                    double.TryParse(fields[5], out speed);

                    /* Set the lift date if the TSR applies the full time period. */
                    if (liftedDate == DateTime.MinValue)
                        liftedDate = Settings.dateRange[1];

                    /* Add the TSR properties that are within the period of analysis. */
                    if (issueDate < Settings.dateRange[1] && liftedDate >= Settings.dateRange[0])
                    {
                        TSRObject record = new TSRObject(region, issueDate, liftedDate, startKm, endKm, speed);
                        TSRList.Add(record);
                    }

                }
            }

            /* Return the list of TSR records. */
            return TSRList;
        }

        private static trainOperator getOperator(string shortOperator)
        {
            if (shortOperator.Equals("Aur", StringComparison.OrdinalIgnoreCase))
                return trainOperator.Aurizon;
            else if (shortOperator.Equals("Pac", StringComparison.OrdinalIgnoreCase))
                return trainOperator.PacificNational;
            else if (shortOperator.Equals("Fre",StringComparison.OrdinalIgnoreCase))
                return trainOperator.Freightliner;
            else 
                return trainOperator.unknown;
        
        }

        /// <summary>
        /// Determine if a file is already open before trying to read the file.
        /// </summary>
        /// <param name="filename">Filename of the file to be opened</param>
        /// <returns>True if the file is already open.</returns>
        public static void isFileOpen(string filename)
        {
            FileStream stream = null;

            /* Can the file be opened and read. */
            try
            {
                stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read);
            }
            catch (IOException e)
            {
                /* File is already opended and locked for reading. */
                //tool.messageBox(e.Message + ":\n\nClose the file and Start again.");
                Environment.Exit(0);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

        }


    }
}
