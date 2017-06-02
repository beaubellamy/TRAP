using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globalsettings;
using Microsoft.Office.Interop.Excel;

namespace TRAP
{
    class FileOperations
    {

        public static List<TrainRecord> readICEData(string filename, List<string> excludeTrainList)
        {
            /* Read all the lines of the data file. */
            isFileOpen(filename);

            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { '\t' };

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            /* Initialise the fields of interest. */
            string TrainID = "none";
            string locoID = "none";
            string subOperator = "";
            trainOperator trainOperator = trainOperator.Unknown;
            trainCommodity commodity = trainCommodity.Unknown;
            double powerToWeight = 0.0;
            double speed = 0.0;
            double kmPost = 0.0;
            //double geometryKm = 0.0;
            double latitude = 0.0;
            double longitude = 0.0;
            DateTime dateTime = DateTime.MinValue;
            //double elevation = 0.0;
            catagory catagory = catagory.Unknown;

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

                    TrainID = fields[6];
                    locoID = fields[1];

                    if (fields[4].Count() >= 3) //operator
                        subOperator = fields[4].Substring(0, 3);

                    trainOperator = getOperator(subOperator);

                    commodity = getCommodity(fields[5]);

                    /* Ensure values are valid while reading them out. */
                    double.TryParse(fields[9], out speed);
                    double.TryParse(fields[8], out kmPost);
                    double.TryParse(fields[0], out latitude);
                    double.TryParse(fields[2], out longitude);
                    DateTime.TryParse(fields[3], out dateTime);
                    double.TryParse(fields[7], out powerToWeight);

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
        /// This function reads the Traxim simulation files and populates the simualtedTrain 
        /// data for comparison to the averaged ICE data.
        /// </summary>
        /// <param name="filename">The simulation filename.</param>
        /// <returns>The list of data for the simualted train.</returns>
        public static Train readSimulationData(string filename, catagory simulationCatagory, direction direction)
        {

            /* Read all the lines of the data file. */
            isFileOpen(filename);

            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { ',', '\t' };

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            /* Initialise the fields of interest. */
            double kilometreage = 0;
            double latitude = 0;
            double longitude = 0;
            double elevation = 0;
            double time = 0;
            DateTime dateTime = DateTime.MinValue;
            double speed = 0;

            bool header = true;

            /* List of the simulated journey. */
            List<TrainJourney> simulatedJourney = new List<TrainJourney>();


            foreach (string line in lines)
            {
                /* Seperate each record into each field */
                fields = line.Split(delimeters);

                if (header)
                {
                    header = false;
                }
                else
                {
                    /* Add the properties to their respsective fields. */

                    double.TryParse(fields[3], out elevation);
                    double.TryParse(fields[9], out speed);
                    double.TryParse(fields[14], out kilometreage);
                    double.TryParse(fields[0], out latitude);
                    double.TryParse(fields[2], out longitude);


                    if (double.TryParse(fields[8], out time))
                    {
                        if (dateTime == DateTime.MinValue)
                            dateTime = new DateTime(2016, 1, 1, 0, 0, 0);
                        else
                            dateTime = dateTime.AddSeconds(time);
                    }
                    /* Add the record to the simulated journey. */
                    TrainJourney item = new TrainJourney(new GeoLocation(latitude, longitude), dateTime, speed, kilometreage, kilometreage, elevation);
                    simulatedJourney.Add(item);
                }
            }
            /* Create the simulated train. */
            Train simulatedTrain = new Train(simulatedJourney, simulationCatagory, direction);

            /* Return the list of records. */
            return simulatedTrain;
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
            char[] delimeters = { ',', '\t' };

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
            direction direction = direction.Unknown;
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
                        if (direction == direction.Unknown)
                        {
                            if (kilometreage - previouskm > 0)
                                direction = direction.IncreasingKm;
                            else
                                direction = direction.DecreasingKm;
                        }

                        /* Calcualte the distance between succesive points and increment the virtual kilometreage. */
                        distance = processing.calculateGreatCircleDistance(previousLat, previousLong, latitude, longitude);

                        if (direction == direction.IncreasingKm)
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

        /// <summary>
        /// This function writes each interpolated train journey to an individual column in excel.
        /// This can be used to compare against previously completed corridor analysis for validation.
        /// </summary>
        /// <param name="trainRecords">List of trains containing the interpolated data.</param>
        public static void writeTrainData(List<Train> trainRecords)
        {

            /* Create the microsfot excel references. */
            //Application excel;
            _Workbook workbook;
            _Worksheet worksheet;

            /* Start Excel and get Application object. */
            Application excel = new Application();

            /* Get the reference to the new workbook. */
            workbook = (Microsoft.Office.Interop.Excel._Workbook)(excel.Workbooks.Add(""));

            /* Create the header details. */
            //string[] headerString1 = { "km", "", "Trains:" };
            //string[] headerString2 = { "", "Train ID:" };
            //string[] headerString3 = { "", "Loco ID:" };
            //string[] headerString4 = { "", "Date:" };
            //string[] headerString5 = { "", "Power to Weight Ratio:" };
            //string[] headerString6 = { "", "Commodity:", };
            //string[] headerString7 = { "", "Direction:" };

            string[,] headerString = {{ "km", "", "Trains:" },
                                     { "", "Train ID:", "" },
                                     { "", "Loco ID:", "" },
                                     { "", "Date:", "" },
                                     { "", "Power to Weight Ratio:", "" },
                                     { "", "Commodity:", "" },
                                     { "", "Direction:", "" }};


            /* Pagenate the data for writing to excel. */
            int excelPageSize = 1000000;        /* Page size of the excel worksheet. */
            int excelPages = 1;                 /* Number of Excel pages to write. */
            int headerOffset = 9;

            /* Adjust the excel page size or the number of pages to write. */
            if (trainRecords.Count() < excelPageSize)
                excelPageSize = trainRecords.Count();
            else
                excelPages = (int)Math.Round((double)trainRecords.Count() / excelPageSize + 0.5);

            //int middle = (int)trainRecords[0].TrainJourney.Count() / 2;
            /* Deconstruct the train details into excel columns. */
            string[,] TrainID = new string[1, trainRecords.Count()];
            string[,] LocoID = new string[1, trainRecords.Count()];
            double[,] powerToWeight = new double[1, trainRecords.Count()];
            string[,] commodity = new string[1, trainRecords.Count()];
            string[,] direction = new string[1, trainRecords.Count()];
            DateTime[,] dateTime = new DateTime[1, trainRecords[0].journey.Count()];
            double[,] kilometerage = new double[trainRecords[0].journey.Count(), 1];

            double[,] speed = new double[trainRecords[0].journey.Count(), trainRecords.Count()];

            int headerRows = headerString.GetLength(0); //7
            int headerColumns = headerString.GetLength(1); //3

            /* Loop through the excel pages. */
            for (int excelPage = 0; excelPage < excelPages; excelPage++)
            {
                /* Set the active worksheet. */
                worksheet = workbook.Sheets[excelPage + 1];
                workbook.Sheets[excelPage + 1].Activate();
                Range topLeft = worksheet.Cells[1, 1];
                Range bottomRight = worksheet.Cells[headerRows, headerColumns];
                worksheet.get_Range(topLeft, bottomRight).Value2 = headerString;

                /* Loop through the data for each excel page. */
                for (int trainIdx = 0; trainIdx < trainRecords.Count(); trainIdx++)
                {

                    TrainID[0, trainIdx] = trainRecords[trainIdx].trainID;
                    LocoID[0, trainIdx] = trainRecords[trainIdx].locoID;

                    /* Extract the earliest date in the journey to represent the train date. */
                    dateTime[0, trainIdx] = trainRecords[trainIdx].journey.Where(t => t.dateTime > DateTime.MinValue).ToList().Min(t => t.dateTime);

                    powerToWeight[0, trainIdx] = trainRecords[trainIdx].powerToWeight;

                    commodity[0, trainIdx] = trainRecords[trainIdx].commodity.ToString();

                    direction[0, trainIdx] = trainRecords[trainIdx].trainDirection.ToString();

                    for (int journeyIdx = 0; journeyIdx < trainRecords[trainIdx].journey.Count(); journeyIdx++)
                    {
                        kilometerage[journeyIdx, 0] = Settings.startKm + Settings.interval / 1000 * journeyIdx;

                        speed[journeyIdx, trainIdx] = trainRecords[trainIdx].journey[journeyIdx].speed;

                    }
                }

                /* Write the data to the active excel workseet. */
                worksheet.Range[worksheet.Cells[2, 3], worksheet.Cells[2, trainRecords.Count() + 2]].Value2 = TrainID;
                worksheet.Range[worksheet.Cells[3, 3], worksheet.Cells[3, trainRecords.Count() + 2]].Value2 = LocoID;
                worksheet.Range[worksheet.Cells[4, 3], worksheet.Cells[4, trainRecords.Count() + 2]].Value2 = dateTime;
                worksheet.Range[worksheet.Cells[5, 3], worksheet.Cells[5, trainRecords.Count() + 2]].Value2 = powerToWeight;
                worksheet.Range[worksheet.Cells[6, 3], worksheet.Cells[6, trainRecords.Count() + 2]].Value2 = commodity;
                worksheet.Range[worksheet.Cells[7, 3], worksheet.Cells[7, trainRecords.Count() + 2]].Value2 = direction;

                worksheet.Range[worksheet.Cells[headerOffset, 1], worksheet.Cells[headerOffset + trainRecords[0].journey.Count() - 1, 1]].Value2 = kilometerage;
                worksheet.Range[worksheet.Cells[headerOffset, 3], worksheet.Cells[headerOffset + trainRecords[0].journey.Count() - 1, 3 + trainRecords.Count() - 1]].Value2 = speed;

            }

            /* Generate the resulting file name and location to save to. */
            string savePath = FileSettings.aggregatedDestination;
            string saveFilename = savePath + @"\ICEData_InterpolatedTrains" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            /* Check the file does not exist yet. */
            if (File.Exists(saveFilename))
            {
                isFileOpen(saveFilename);
                File.Delete(saveFilename);
            }


            /* Save the excel file. */
            excel.UserControl = false;
            workbook.SaveAs(saveFilename, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            workbook.Close();

            return;
        }

        public static void wrtieAverageData(List<AverageTrain> averageTrains, List<Statistics> stats)
        {
            /* Start Excel and get the references to the workbook and worksheet. */
            Application excel = new Application();
            _Workbook workbook = excel.Workbooks.Add("");
            _Worksheet worksheet;


            /* Extract the statistics */
            /* Note: there is no check to confimr the order in which the statistics values are listed. */
            string[,] statisticsHeader = { { "Statistics:" }, { "Number Of Trains" }, { "Average Distance Travelled" }, { "Average Speed" }, { "Average P/W Ratio" }, { "P/W standard Deviation" } };
            string[,] totalStatistics = new string[statisticsHeader.GetLength(0), stats.Count()];


            for (int index = 0; index < stats.Count(); index++)
            {
                totalStatistics[0, index] = stats[index].catagory;
                totalStatistics[1, index] = stats[index].numberOfTrains.ToString();
                totalStatistics[2, index] = stats[index].averageDistanceTravelled.ToString();
                totalStatistics[3, index] = stats[index].averageSpeed.ToString();
                totalStatistics[4, index] = stats[index].averagePowerToWeightRatio.ToString();
                totalStatistics[5, index] = stats[index].standardDeviationP2W.ToString();

            }

            /* Determine the number of different catagories in both direction. */
            /* Ignore the combined trains as they are an amalgamation of each catagory in each direction. */
            //int catagories = (averageTrains.Count() - 2) / 2;

            


            /* Create the header details. */
            //string[] headerString = new string[] { };
            //if (Settings.HunterValleyRegion)
            //{
            //    if (catagories == 2)
            //        headerString = new string[] { "Kilometreage", "Elevation", "Pacific National Increasing km", "Pacific National Decreasing km", 
            //            "Aurizon Increasing km", "Aurizon Decreasing km", "Weighted Average Increasing km", "Weighted Average Decreasing km", "Loop", "TSRs" };

            //    if (catagories == 3)
            //        headerString = new string[] { "Kilometreage", "Elevation", "Pacific National Increasing km", "Pacific National Decreasing km", 
            //            "Aurizon Increasing km", "Aurizon Decreasing km", "Freightliner Increasing km", "Freightliner Decreasing km", 
            //            "Weighted Average Increasing km", "Weighted Average Decreasing km", "Loop", "TSRs" };
            //}
            //else
            //    headerString = new string[] { "Kilometreage", "Elevation", "Underpowered Increasing km", "Underpowered Decreasing km", "Overpowered Increasing km", 
            //                            "Overpowered Decreasing km", "Weighted Average Increasing km", "Weighted Average Decreasing km", "Loop", "TSRs" };


            List<string> headerString = new List<string>();
            headerString.Add("kilometerage");
            headerString.Add("Elevation");

            for (int trainIdx = 0; trainIdx < averageTrains.Count(); trainIdx++)
            {
                headerString.Add(averageTrains[trainIdx].trainCatagory.ToString() + " " +averageTrains[trainIdx].direction.ToString());
            }
            headerString.Add("Loops");
            headerString.Add("TSRs");
            
            /* Pagenate the data for writing to excel. */
            //int excelPageSize = 1000000;        /* Page size of the excel worksheet. */
            int numberOfPoints = averageTrains[0].kilometreage.Count();
            //int excelPages = 1;                 /* Number of Excel pages to write. */
            int headerOffset = statisticsHeader.GetLength(0) + 4;

            /* Adjust the excel page size or the number of pages to write. */
            //if (averageTrains[0].kilometreage.Count() < excelPageSize)
            //    excelPageSize = averageTrains[0].kilometreage.Count();
            //else
            //    excelPages = (int)Math.Round((double)averageTrains[0].kilometreage.Count() / excelPageSize + 0.5);


            /* Deconstruct the train details into excel columns. */
            double[,] kilometerage = new double[numberOfPoints, 1];
            double[,] elevation = new double[numberOfPoints, 1];
            //double[,] catagory1Increasing = new double[numberOfPoints, 1];   // Pacific National
            //double[,] catagory1Decreasing = new double[numberOfPoints, 1];   // Pacific National
            //double[,] catagory2Increasing = new double[numberOfPoints, 1];    // Aurizon
            //double[,] catagory2Decreasing = new double[numberOfPoints, 1];    // Aurizon
            //double[,] catagory3Increasing = new double[numberOfPoints, 1];    // Freightliner
            //double[,] catagory3Decreasing = new double[numberOfPoints, 1];    // Freightliner

            //double[,] totalIncreasingSpeed = new double[numberOfPoints, 1];
            //double[,] totalDecreasingSpeed = new double[numberOfPoints, 1];
            string[,] isLoophere = new string[numberOfPoints, 1];
            string[,] isTSRhere = new string[numberOfPoints, 1];

            double[,] averageSpeedArray = new double[numberOfPoints, averageTrains.Count()];


            /* Set the active worksheet. */
            worksheet = (_Worksheet)workbook.Sheets[1];
            workbook.Sheets[1].Activate();

            /* Loop through the data for each excel page. */
            for (int i = 0; i < numberOfPoints; i++)
            {
                /* Check we dont try to read more data than there really is. */
                //int checkIdx = j + 0 * numberOfPoints;

                kilometerage[i, 0] = averageTrains[0].kilometreage[i];
                elevation[i, 0] = averageTrains[0].elevation[i];

                if (averageTrains[0].isInLoopBoundary[i])
                    isLoophere[i, 0] = "Loop Boundary";

                if (averageTrains[0].isInTSRboundary[i])
                    isTSRhere[i, 0] = "TSR Boundary";


                for (int j = 0; j < averageTrains.Count(); j++)
                {
                    averageSpeedArray[i, j] = averageTrains[j].averageSpeed[i];
                }

            }

            /* Display the statistics for each catagory. */
            int column = 3;
            Range topLeft = worksheet.Cells[statisticsHeader.GetLength(1), column];
            Range bottomRight = worksheet.Cells[statisticsHeader.GetLength(0), column + totalStatistics.GetLength(1) - 1];

            /* Set statistics. */
            worksheet.get_Range("A1", "A6").Value2 = statisticsHeader;
            worksheet.get_Range(topLeft, bottomRight).Value2 = totalStatistics;


            /* Generalise the row and columns */
            /* Set the data header. */
            topLeft = worksheet.Cells[headerOffset, 1];
            bottomRight = worksheet.Cells[headerOffset, averageTrains.Count() +4];
            worksheet.get_Range(topLeft, bottomRight).Value2 = headerString.ToArray();
            //"A9", "L9"

            int dataOffset = headerOffset+1;
            /* Write the data to the active excel workseet. */
            worksheet.get_Range("A" + dataOffset, "A" + (dataOffset + numberOfPoints-1)).Value2 = kilometerage;
            worksheet.get_Range("B" + dataOffset, "B" + (dataOffset + numberOfPoints-1)).Value2 = elevation;

            topLeft = worksheet.Cells[dataOffset, column];
            bottomRight = worksheet.Cells[dataOffset + numberOfPoints-1, column + averageTrains.Count() - 1];
            worksheet.get_Range(topLeft, bottomRight).Value2 = averageSpeedArray;

            /* Increment the column. */
            topLeft = worksheet.Cells[dataOffset, column + averageTrains.Count()];
            bottomRight = worksheet.Cells[dataOffset + numberOfPoints-1, column + averageTrains.Count()];
            worksheet.get_Range(topLeft, bottomRight).Value2 = isLoophere;

            /* Increment the column. */
            topLeft = worksheet.Cells[dataOffset, column + averageTrains.Count() + 1];
            bottomRight = worksheet.Cells[dataOffset + numberOfPoints-1, column + averageTrains.Count() + 1];
            worksheet.get_Range(topLeft, bottomRight).Value2 = isTSRhere;



            //worksheet.get_Range("A" + headerOffset, "A" + (headerOffset + excelPageSize - 1)).Value2 = kilometerage;
            //worksheet.get_Range("B" + headerOffset, "B" + (headerOffset + excelPageSize - 1)).Value2 = elevation;
            //worksheet.get_Range("C" + headerOffset, "C" + (headerOffset + excelPageSize - 1)).Value2 = catagory1Increasing;     // Pacific National
            //worksheet.get_Range("D" + headerOffset, "D" + (headerOffset + excelPageSize - 1)).Value2 = catagory1Decreasing;     // Pacific National
            //worksheet.get_Range("E" + headerOffset, "E" + (headerOffset + excelPageSize - 1)).Value2 = catagory2Increasing;      // Aurizon
            //worksheet.get_Range("F" + headerOffset, "F" + (headerOffset + excelPageSize - 1)).Value2 = catagory2Decreasing;      // Aurizon
            //if (catagories == 3)
            //{
            //    worksheet.get_Range("G" + headerOffset, "G" + (headerOffset + excelPageSize - 1)).Value2 = catagory3Increasing;      // Freightliner
            //    worksheet.get_Range("H" + headerOffset, "H" + (headerOffset + excelPageSize - 1)).Value2 = catagory3Decreasing;      // Freightliner
            //    worksheet.get_Range("I" + headerOffset, "I" + (headerOffset + excelPageSize - 1)).Value2 = totalIncreasingSpeed;
            //    worksheet.get_Range("J" + headerOffset, "J" + (headerOffset + excelPageSize - 1)).Value2 = totalDecreasingSpeed;
            //    worksheet.get_Range("K" + headerOffset, "K" + (headerOffset + excelPageSize - 1)).Value2 = isLoophere;
            //    worksheet.get_Range("L" + headerOffset, "L" + (headerOffset + excelPageSize - 1)).Value2 = isTSRhere;
            //}
            //else
            //{
            //    worksheet.get_Range("G" + headerOffset, "G" + (headerOffset + excelPageSize - 1)).Value2 = totalIncreasingSpeed;
            //    worksheet.get_Range("H" + headerOffset, "H" + (headerOffset + excelPageSize - 1)).Value2 = totalDecreasingSpeed;
            //    worksheet.get_Range("I" + headerOffset, "I" + (headerOffset + excelPageSize - 1)).Value2 = isLoophere;
            //    worksheet.get_Range("J" + headerOffset, "J" + (headerOffset + excelPageSize - 1)).Value2 = isTSRhere;
            //}





            /* Generate the resulting file name and location to save to. */
            string savePath = FileSettings.aggregatedDestination;
            string saveFilename = savePath + @"\AverageSpeed_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            /* Check the file does not exist yet. */
            if (File.Exists(saveFilename))
            {
                isFileOpen(saveFilename);
                File.Delete(saveFilename);
            }

            /* Save the excel file. */
            excel.UserControl = false;
            workbook.SaveAs(saveFilename, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            workbook.Close();

            return;



        }

        private static trainOperator getOperator(string shortOperator)
        {
            if (shortOperator.Equals("Aus", StringComparison.OrdinalIgnoreCase))
                return trainOperator.ARTC;
            else if (shortOperator.Equals("Aur", StringComparison.OrdinalIgnoreCase))
                return trainOperator.Aurizon;
            else if (shortOperator.Equals("Cit", StringComparison.OrdinalIgnoreCase))
                return trainOperator.CityRail;
            else if (shortOperator.Equals("Cou", StringComparison.OrdinalIgnoreCase))
                return trainOperator.CountryLink;
            else if (shortOperator.Equals("Fre", StringComparison.OrdinalIgnoreCase))
                return trainOperator.Freightliner;
            else if (shortOperator.Equals("Gre", StringComparison.OrdinalIgnoreCase))
                return trainOperator.GreatSouthernRail;
            else if (shortOperator.Equals("int", StringComparison.OrdinalIgnoreCase))
                return trainOperator.Interail;
            else if (shortOperator.Equals("Lau", StringComparison.OrdinalIgnoreCase))
                return trainOperator.LauchlanValleyRailSociety;
            else if (shortOperator.Equals("Pac", StringComparison.OrdinalIgnoreCase))
                return trainOperator.PacificNational;
            else if (shortOperator.Equals("QUB", StringComparison.OrdinalIgnoreCase))
                return trainOperator.QUBE;
            else if (shortOperator.Equals("Rai", StringComparison.OrdinalIgnoreCase))
                return trainOperator.RailCorp;
            else if (shortOperator.Equals("SCT", StringComparison.OrdinalIgnoreCase))
                return trainOperator.SCT;
            else if (shortOperator.Equals("Sou", StringComparison.OrdinalIgnoreCase))
                return trainOperator.SouthernShorthaulRail;
            else if (shortOperator.Equals("Syd", StringComparison.OrdinalIgnoreCase))
                return trainOperator.SydneyRailService;
            else if (shortOperator.Equals("The", StringComparison.OrdinalIgnoreCase))
                return trainOperator.TheRailMotorService;
            else if (shortOperator.Equals("V L", StringComparison.OrdinalIgnoreCase))
                return trainOperator.VLinePassenger;
            else
                return trainOperator.Unknown;

        }

        private static trainCommodity getCommodity(string commodity)
        {
            string[] Clinker = { "Clinker" };
            string[] Coal = { "Coal Export", "Containersied Coal" };
            string[] Freight = { "General Freight" };
            string[] Grain = { "Grain" };
            string[] Intermodal = { "Intermodal" };
            string[] Minerals = { "Minerals" };
            string[] Passenger = { "Passenger" };
            string[] Steel = { "Steel" };
            string[] Work = { "Unspecified Commodity" };


            if (Clinker.Contains(commodity))
                return trainCommodity.Clinker;
            else if (Coal.Contains(commodity))
                return trainCommodity.Coal;
            else if (Freight.Contains(commodity))
                return trainCommodity.Freight;
            else if (Grain.Contains(commodity))
                return trainCommodity.Grain;
            else if (Intermodal.Contains(commodity))
                return trainCommodity.Intermodal;
            else if (Minerals.Contains(commodity))
                return trainCommodity.Mineral;
            else if (Passenger.Contains(commodity))
                return trainCommodity.Passenger;
            else if (Steel.Contains(commodity))
                return trainCommodity.Steel;
            else if (Work.Contains(commodity))
                return trainCommodity.Work;
            else
                return trainCommodity.Unknown;
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
