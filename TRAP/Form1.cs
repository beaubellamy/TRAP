/* uncomment when testing mutliple corridors overnight. */
//#define TESTING 

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Globalsettings;

/* Custome Libraries */
using TrainLibrary;
using IOLibrary;

namespace TRAP
{
    public partial class TrainPerformance : Form
    {

        /* Constant time factors. */
        public const double secPerHour = 3600;
        public const double secPerDay = 86400;
        public const double hoursPerDay = 24;
        public const double minutesPerHour = 60;
        public const double secPerMinute = 60;

        /* Timer parameters to keep track of execution time. */
        private int timeCounter = 0;
        private bool stopTheClock = false;

        

        public TrainPerformance()
        {
            /* initialise the form. */
            InitializeComponent();

            /* Set current version. */
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = "TRAP: "+version;

            /* When testing multiple corridors in one run. - Designed for overnight use */
#if (TESTING)
            object sender = new object();
            EventArgs e = new EventArgs();

            runTarcoolaToKalgoorlieBatch(sender, e);

            //runCulleranRanges(sender, e);               /* Insufficient TSR data */ // RunTime 00:09:34.91
            //runGunnedahBasin(sender, e);                  // Run Time: 01:22:06.31
            //runUlanLine(sender, e);                     // Run Time: 01:16:18.99
            //runPortKembla(sender, e);                   // Run Time: 00:11:46.52
            //runMacarthurToBotany(sender, e);            /* Insufficient TSR data */ // RunTime 00:07:02.58
            //runMelbourneToCootamundra(sender, e);       /* Insufficient TSR data */ // RunTime 00:49:15.08
            //runTarcoolaToKalgoorlie(sender, e);         /* Insufficient TSR data */ // RunTime 02:02:35.80
            //runSouthernHighlands(sender, e);            // Run Time: 01:13:03.57


#endif

        }

        /// <summary>
        /// Select the data file that requires processing.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectDataFile_Click(object sender, EventArgs e)
        {
            /* Select the data file. */
            //FileSettings.dataFile = tool.browseFile("Select the data file.");
            FileSettings.dataFile = Tools.selectDataFile(caption: "Select the data file.");
            
            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;
            
        }

        /// <summary>
        /// Select the geometry file required to process the train data.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectGeometryFile_Click(object sender, EventArgs e)
        {
            FileSettings.geometryFile = Tools.selectDataFile(caption: "Select the geometry file.");
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;
        }

        /// <summary>
        /// Select the Temporary Speed Restriction file required to process the train data.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectTSRFile_Click(object sender, EventArgs e)
        {
            FileSettings.temporarySpeedRestrictionFile = Tools.selectDataFile(caption: "Select the TSR file.");
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;
        }

        /// <summary>
        /// Select the file containing the list of trains to exclude from the processing.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectTrainFile_Click(object sender, EventArgs e)
        {
            FileSettings.trainListFile = Tools.selectDataFile(caption: "Select the train list file.");
            trainListFile.Text = Path.GetFileName(FileSettings.trainListFile);
            trainListFile.ForeColor = SystemColors.ActiveCaptionText;
        }

        /// <summary>
        /// Calculate the average power to weight ratio required for the simulations.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void simulationPowerToWeightRatios_Click(object sender, EventArgs e)
        {
            /* Extract the form parameters. */
            populateFormParameters(this);
            
            /* Validate the form parameters. */
            if (!areFormParametersValid())
            {
                Tools.messageBox("One or more parameters are invalid.");
                return;
            }

            /* The data file and the geometry file are required for processing. */
            if (FileSettings.dataFile == null || FileSettings.geometryFile == null)
                return;

            /* Ensure there is a empty list of trains to exclude to start. */
            List<string> excludeTrainList = new List<string> { };

            /* Populate the exluded train list. */
            if (Settings.excludeListOfTrains)
                excludeTrainList = FileOperations.readTrainList(FileSettings.trainListFile);

            /* Read in the track gemoetry data. */
            List<TrackGeometry> trackGeometry = new List<TrackGeometry>();
            trackGeometry = FileOperations.readGeometryfile(FileSettings.geometryFile);

            /* Read the data. */
            List<TrainRecord> TrainRecords = new List<TrainRecord>();
            //TrainRecords = FileOperations.readICEData(FileSettings.dataFile, excludeTrainList, Settings.excludeListOfTrains, Settings.dateRange);
            //TrainRecords = FileOperations.readAzureICEData(FileSettings.dataFile, excludeTrainList, Settings.excludeListOfTrains, Settings.dateRange);
            TrainRecords = FileOperations.readAzureExtractICEData(FileSettings.dataFile, excludeTrainList, Settings.excludeListOfTrains, Settings.dateRange);


            if (TrainRecords.Count() == 0)
            {
                Tools.messageBox("There is no data within the specified boundaries.\nCheck the processing parameters.");
                return;
            }

            /* Set the power to weight ratios of the average trains that need to be simualted. */
            if (TrainRecords.Where(t => t.powerToWeight == 0).Count() == TrainRecords.Count())
            {
                Category1IncreasingPowerToWeightRatio.Text = "0";
                Category1DecreasingPowerToWeightRatio.Text = "0";

                Category2IncreasingPowerToWeightRatio.Text = "0";
                Category2DecreasingPowerToWeightRatio.Text = "0";

                combinedIncreasingPowerToWeightRatio.Text = "0";
                combinedDecreasingPowerToWeightRatio.Text = "0";

                SimulationP2WRatioLabel.Text = "The power to weight ratios for all trains are zero (0)";
            }
            else
            {
                /* Sort the data by [trainID, locoID, Date & Time, kmPost]. */
                List<TrainRecord> OrderdTrainRecords = new List<TrainRecord>();
                OrderdTrainRecords = TrainRecords.OrderBy(t => t.trainID).ThenBy(t => t.locoID).ThenBy(t => t.dateTime).ThenBy(t => t.kmPost).ToList();

                /* Clean data - remove trains with insufficient data. */
                /******** Should only be required while we are waiting for the data in the prefered format ********/
                List<Train> CleanTrainRecords = new List<Train>();
                CleanTrainRecords = Processing.CleanData(OrderdTrainRecords, trackGeometry, 
                    Settings.timeThreshold, Settings.distanceThreshold, Settings.minimumJourneyDistance, Settings.analysisCategory, 
                    Settings.Category1LowerBound, Settings.Category1UpperBound, Settings.Category2LowerBound, Settings.Category2UpperBound);
                /**************************************************************************************************/
                

                /* Calculate the avareage power to weight ratio for a given band and train direction. */
                Category1IncreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.Category1LowerBound, Settings.Category1UpperBound, direction.IncreasingKm));
                Category1DecreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.Category1LowerBound, Settings.Category1UpperBound, direction.DecreasingKm));

                Category2IncreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.Category2LowerBound, Settings.Category2UpperBound, direction.IncreasingKm));
                Category2DecreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.Category2LowerBound, Settings.Category2UpperBound, direction.DecreasingKm));

                combinedIncreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.Category1LowerBound, Settings.Category2UpperBound, direction.IncreasingKm));
                combinedDecreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.Category1LowerBound, Settings.Category2UpperBound, direction.DecreasingKm));


                /* Populate the counts for each train Category. */
                Category1IncreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category1UpperBound).Count().ToString();
                Category1DecreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category1UpperBound).Count().ToString();
                Category2IncreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category2LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();
                Category2DecreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category2LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();

                combinedIncreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();
                combinedDecreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();

                /* Need to run the simulaions based on the average power to weight ratios before continueing with the analysis. */
                SimulationP2WRatioLabel.Text = "Run Simulations based on these power to weight ratios";

            }

            
        }

        /// <summary>
        /// Select the simulation file for Category 1 characteristics in the increasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCategory1IncreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(Category1IncreasingSimulationFile, 0);
            
        }

        /// <summary>
        /// Select the simulation file for Category 1 characteristics in the decreasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCategory1DecreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(Category1DecreasingSimulationFile, 1);
        }

        /// <summary>
        /// Select the simulation file for Category 2 characteristics in the increasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCategory2IncreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(Category2IncreasingSimulationFile, 2);
        }

        /// <summary>
        /// Select the simulation file for Category 2 characteristics in the decreasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCategory2DecreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(Category2DecreasingSimulationFile, 3);
        }

        /// <summary>
        /// Select the simulation file for Category 3 characteristics in the increasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCategory3IncreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(Category3IncreasingSimulationFile, 4);
        }

        /// <summary>
        /// Select the simulation file for Category 3 characteristics in the decreasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCategory3DecreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(Category3DecreasingSimulationFile, 5);
        }
                
        /// <summary>
        /// Helper function to set the simulation file parameters and add the simulation file into the list in the correct index.
        /// </summary>
        /// <param name="simulationFile">Form object to populate.</param>
        /// <param name="index">Insertion index into the simulation file list.</param>
        private void setSimulationFile(TextBox simulationFile, int index)
        {
            string filename = null;
            string direction = null;
            /* Extract the simulation Category. */
            string Category = getSimulationCategory(index);
            
            /* Determine the direction of the simulation. */
            if ((index % 2) == 0)
                direction = "increasing";
            else
                direction = "decreasing";

            /* Create a meaningful string to help user identify the correct file. */
            string browseString = "Select the " + Category + " " + direction + " km simulation file.";

            /* Select the simulation file using the browser and insert into the simulation file list. */
            filename = Tools.selectDataFile(caption: browseString);
            FileSettings.simulationFiles[index] = filename;
            simulationFile.Text = Path.GetFileName(filename);
            simulationFile.ForeColor = SystemColors.ActiveCaptionText;
        }

        /// <summary>
        /// Identify the simulation Category based on the index in the list.
        /// </summary>
        /// <param name="index">Index of the simulation Category</param>
        /// <returns>A string identifying the simulation Category.</returns>
        private string getSimulationCategory(int index)
        {
            /* Identify which simulation Category is being selected. */
            if ((Operator1Category.SelectedItem != null || Operator2Category.SelectedItem != null || Operator3Category.SelectedItem != null) &&
                (Operator1Category.Text != "" || Operator2Category.Text != "" || Operator3Category.Text != ""))
            {
                /* Return the appropriate operator. */
                if (index < 2)
                    return Operator1Category.SelectedItem.ToString();
                else if (index < 4)
                    return Operator2Category.SelectedItem.ToString();
                else
                    return Operator3Category.SelectedItem.ToString();
            }
            else if ((Commodity1Category.SelectedItem != null || Commodity2Category.SelectedItem != null || Commodity3Category.SelectedItem != null) &&
                (Commodity1Category.Text != "" || Commodity2Category.Text != "" || Commodity3Category.Text != ""))
            {
                /* Return the appropriate Commodity. */
                if (index < 2)
                    return Commodity1Category.SelectedItem.ToString();
                else if (index < 4)
                    return Commodity2Category.SelectedItem.ToString();
                else
                    return Commodity3Category.SelectedItem.ToString();
            }
            else
            {
                /* Return the appropriate power to weight Category. */
                if (index < 2)
                    return "Underpowered";
                else if (index < 4)
                    return "Overpowered";
                else
                    return "Alternative";
            }

        }

        /// <summary>
        /// Set the destiantion directory for the aggregated results files.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void resultsDirectory_Click(object sender, EventArgs e)
        {
            /* Browse the folders for the desired desination folder. */
            FileSettings.aggregatedDestination = Tools.selectFolder();
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;
        }

        /// <summary>
        /// Perform the analysis based on the input parameters.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Execute_Click(object sender, EventArgs e)
        {
            /* Create a Timer. */
            Timer timer = new Timer();
            timer.Interval = 1000;                      // Set the tick interval to 1 second.
            timer.Enabled = true;                       // Set the time to be running.
            timer.Tag = executionTime;                  // Set the timer label
            timer.Tick += new EventHandler(tickTimer);  // Event handler function.

            /* Populate the parameters. */
            populateFormParameters(this);
            /* Validate the form parameters. */
            if (!areFormParametersValid())
            {
                Tools.messageBox("One or more parameters are invalid.");
                return;
            }

            /* Start the timer. */
            timer.Start();

            /* Set up the background threads to run asynchronously. */
            BackgroundWorker background = new BackgroundWorker();

            background.DoWork += (backgroundSender, backgroundEvents) =>
                {
            
                    /* Run the train performance analysis. */
                    List<Train> trains = new List<Train>();
                    trains = Algorithm.trainPerformance();

                    /* Populate the counts for each train Category. */
                    if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
                    {
                        Category1IncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                                        Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                        Where(t => t.powerToWeight <= Settings.Category1UpperBound).Count().ToString();
                        Category1DecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                                        Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                        Where(t => t.powerToWeight <= Settings.Category1UpperBound).Count().ToString();
                        Category2IncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                                        Where(t => t.powerToWeight > Settings.Category2LowerBound).
                                                        Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();
                        Category2DecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                                        Where(t => t.powerToWeight > Settings.Category2LowerBound).
                                                        Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();

                        combinedIncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                                        Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                        Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();
                        combinedDecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                                        Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                        Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();
                    }
                    
                    /*
                     * else if analysis catagory is train operator
                     *      set labels to appropriate opperators
                     *      set the counts of each operator
                     * else
                     *      set labels to appropriate commodity
                     *      set count of each commodity
                     */
                };

                background.RunWorkerCompleted += (backgroundSender, backgroundEvents) =>
                    {

                        /* When asynchronous execution complete, reset the timer counter ans stop the clock. */
                        timeCounter = 0;
                        stopTheClock = true;

                        Tools.messageBox("Program Complete.");
                    };

                background.RunWorkerAsync();



        }

        /// <summary>
        /// Allow the testing process to continue without asynchronous execution.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void testExecute(object sender, EventArgs e)
        {

            /* Populate the parameters. */
            populateFormParameters(this);
            /* Validate the form parameters. */
            if (!areFormParametersValid())
            {
                Tools.messageBox("One or more parameters are invalid.");
                return;
            }

            /* Run the train performance analysis. */
            List<Train> trains = new List<Train>();
            trains = Algorithm.trainPerformance();

            /* Populate the counts for each train Category. */
            if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
            {
                Category1IncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category1UpperBound).Count().ToString();
                Category1DecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category1UpperBound).Count().ToString();
                Category2IncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category2LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();
                Category2DecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category2LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();

                combinedIncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();
                combinedDecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.Category1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.Category2UpperBound).Count().ToString();
            }

            /*
             * else if analysis catagory is train operator
             *      set labels to appropriate opperators
             *      set the counts of each operator
             * else
             *      set labels to appropriate commodity
             *      set count of each commodity
             */




        }
        
        /// <summary>
        /// Calculate the average power to weight ratio of all trains within a band for a given direction of travel.
        /// </summary>
        /// <param name="trains">List of trains in the data.</param>
        /// <param name="lowerBound">The lower bound of the acceptable power to weight ratio.</param>
        /// <param name="upperBound">The upper bound of the acceptable power to weight ratio.</param>
        /// <param name="direction">The direction of the km of the train journey.</param>
        /// <returns>The average power to weight ratio for the trains in the specified direction.</returns>
        public double averagePowerToWeightRatio(List<Train> trains, double lowerBound, double upperBound, direction direction)
        {

            List<double> power2Weight = new List<double>();
            double power = 0;

            /* Clycle through each train. */
            foreach (Train train in trains)
            {
                if (train.trainDirection == direction)
                {
                    /* Extract the power to weight ratio for each train. */
                    power = train.powerToWeight;
                    if (power > lowerBound && power <= upperBound)
                        power2Weight.Add(power);
                }
            }

            /* Return the average power to weight ratio. */
            if (power2Weight.Count() == 0)
                return 0;
            else
                return power2Weight.Average();

        }

        /// <summary>
        /// Extract the date range for the data.
        /// </summary>
        /// <returns>A 2-element array containig the start and end date to consider.</returns>
        public DateTime[] getDateRange() { return new DateTime[2] { fromDate.Value, toDate.Value }; }

        /// <summary>
        /// Extract the value of the includeAListOfTrainsToExclude flag.
        /// </summary>
        /// <returns>The value of the boolean flag.</returns>
        public bool getTrainListExcludeFlag() { return excludeListOfTrains.Checked; }

        /// <summary>
        /// Extract the value of the ignore gaps during interpolation flag.
        /// </summary>
        /// <returns>The value of the boolean flag.</returns>
        public bool getIgnoreGapsFlag() { return IgnoreGaps.Checked; }

        /// <summary>
        /// Extract the value of the start km for interpolation.
        /// </summary>
        /// <returns>The start km.</returns>
        public double getStartKm()
        {
            double startKm;
            if (double.TryParse(startInterpolationKm.Text, out startKm))
                return startKm;

            return 0;
        }

        /// <summary>
        /// Extract the value of the end km for interpolation.
        /// </summary>
        /// <returns>The end km.</returns>
        public double getEndKm()
        {
            double endKm;
            if (double.TryParse(endInterpolationKm.Text, out endKm))
                return endKm;

            return 0;
        }

        /// <summary>
        /// Extract the value of the interpolation interval.
        /// </summary>
        /// <returns>The interpolation interval.</returns>
        public double getInterval()
        {
            double interval;
            if (double.TryParse(interpolationInterval.Text, out interval))
                return interval;

            return 0;
        }

        
        /// <summary>
        /// Extract the value of the minimum journey distance threshold.
        /// </summary>
        /// <returns>The minimum distance of the train journey.</returns>
        public double getJourneydistance()
        {
            double journeyDistance;
            if (double.TryParse(minimumJourneyDistance.Text, out journeyDistance))
                return journeyDistance * 1000;

            return 0;
        }

        /// <summary>
        /// Extract the value of the loop speed factor for comparison to the simulation data.
        /// </summary>
        /// <returns>The comparison factor.</returns>
        public double getLoopFactor()
        {
            double loopFactor;
            if (double.TryParse(loopSpeedThreshold.Text, out loopFactor))
                return loopFactor / 100.0;

            return 0;
        }

        /// <summary>
        /// Extract the value of the distance before and after a loop to be considered within the loop boundary
        /// </summary>
        /// <returns>The loop boundary threshold.</returns>
        public double getLoopBoundary()
        {
            double loopWindow;
            if (double.TryParse(loopBoundaryThreshold.Text, out loopWindow))
                return loopWindow;

            return 0;
        }

        /// <summary>
        /// Extract the value of the distance before and after a TSR location to be considered within the TSR boundary
        /// </summary>
        /// <returns>The TSR boundary window</returns>
        public double getTSRWindow()
        {
            double TSR;
            if (double.TryParse(TSRWindowBoundary.Text, out TSR))
                return TSR;

            return 0;
        }

        /// <summary>
        /// Extract the value of the time difference between succesive data points to be considered as seperate trains.
        /// </summary>
        /// <returns>The time difference in minutes.</returns>
        public double getTimeSeparation()
        {
            double timeDifference;
            if (double.TryParse(timeSeparation.Text, out timeDifference))
                return timeDifference * 60;

            return 0;
        }

        /// <summary>
        /// Extract the value of the minimum distance between successive data points.
        /// </summary>
        /// <returns>The minimum distance threshold.</returns>
        public double getDataSeparation()
        {
            double distance;
            if (double.TryParse(dataSeparation.Text, out distance))
                return distance * 1000;

            return 0;
        }

        /// <summary>
        /// Extract the lower bound value of the power to weight ratio for the underpowered trains.
        /// </summary>
        /// <returns>The lower bound power to weight ratio.</returns>
        public double getCategory1LowerBound()
        {
            double p2W;
            if (double.TryParse(Category1LowerBound.Text, out p2W))
                return p2W;

            return 0;
        }

        /// <summary>
        /// Extract the upper bound value of the power to weight ratio for the underpowered trains.
        /// </summary>
        /// <returns>The upper bound power to weight ratio.</returns>
        public double getCategory1UpperBound()
        {
            double p2W;
            if (double.TryParse(Category1UpperBound.Text, out p2W))
                return p2W;

            return 0;
        }

        /// <summary>
        /// Extract the lower bound value of the power to weight ratio for the overpowered trains.
        /// </summary>
        /// <returns>The lower bound power to weight ratio.</returns>
        public double getCategory2LowerBound()
        {
            double p2W;
            if (double.TryParse(Category2LowerBound.Text, out p2W))
                return p2W;

            return 0;
        }

        /// <summary>
        /// Extract the upper bound value of the power to weight ratio for the overpowered trains.
        /// </summary>
        /// <returns>The upper bound power to weight ratio.</returns>
        public double getCategory2UpperBound()
        {
            double p2W;
            if (double.TryParse(Category2UpperBound.Text, out p2W))
                return p2W;

            return 0;
        }

        /// <summary>
        /// Extract the value of the powerToWeightRatioAnalysis flag.
        /// </summary>
        /// <returns>The value of the boolean flag.</returns>
        public bool getPowerToWeightRatioAnalysis() { return powerToWeightRatioAnalysis.Checked; }
        
        /// <summary>
        /// Set the labels associated to the power to weight ratio analysis Category.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void powerToWeightRatioAnalysis_CheckedChanged(object sender, EventArgs e)
        {
            if (powerToWeightRatioAnalysis.Checked)
            {
                /* Set the default commodity and train operator Categories. */
                setDefaultCommodityAnalysisParameters();
                setDefaultOperatorAnalysisParameters();

                /* Set the power to weight ratio labels. */
                Category1SimulationLabel.Text = "Underpowered";
                Category2SimulationLabel.Text = "Overpowered";
                Category3SimulationLabel.Text = "Alternative";
            }


        }
        
        /// <summary>
        /// Extract the analysis Category from the Settings.
        /// </summary>
        /// <returns>The analysis Category</returns>
        public analysisCategory getAnalysisCategory()
        {
            if (powerToWeightRatioAnalysis.Checked)
                Settings.analysisCategory = analysisCategory.TrainPowerToWeight;
            else if (Operator1Category.SelectedItem != null)
                Settings.analysisCategory = analysisCategory.TrainOperator;
            else if (Commodity1Category.SelectedItem != null)
                Settings.analysisCategory = analysisCategory.TrainCommodity;
            else // trainType1.Text
                Settings.analysisCategory = analysisCategory.TrainType;

            return Settings.analysisCategory;
        }

        /// <summary>
        /// Extact the value for the operator 1 analysis Category.
        /// </summary>
        /// <returns>The train operator describing the first analysis Category.</returns>
        public trainOperator getOperator1Category()
        {
            /* Convert operator Category to train operator. */
            List<trainOperator> operatorList = Enum.GetValues(typeof(trainOperator)).Cast<trainOperator>().ToList();

            foreach (trainOperator Operator in operatorList)
            {
                if (Operator1Category.SelectedItem != null &&
                    Operator1Category.SelectedItem.ToString().Replace(" ", string.Empty).Equals(Operator.ToString()))
                    return Operator;
                
            }

            return trainOperator.Unknown;
        }

        /// <summary>
        /// Extact the value for the second operator analysis Category.
        /// </summary>
        /// <returns>The train operator describing the second analysis Category.</returns>
        public trainOperator getOperator2Category()
        {
            /* Convert operator Category to train operator. */
            List<trainOperator> operatorList = Enum.GetValues(typeof(trainOperator)).Cast<trainOperator>().ToList();

            foreach (trainOperator Operator in operatorList)
            {
                if (Operator2Category.SelectedItem != null &&
                    Operator2Category.SelectedItem.ToString().Replace(" ", string.Empty).Equals(Operator.ToString()))
                    return Operator;
            }

            return trainOperator.Unknown;
        }

        /// <summary>
        /// Extact the value for the third operator analysis Category.
        /// </summary>
        /// <returns>The train operator describing the third analysis Category.</returns>
        public trainOperator getOperator3Category()
        {
            /* Convert operator Category to train operator. */
            List<trainOperator> operatorList = Enum.GetValues(typeof(trainOperator)).Cast<trainOperator>().ToList();

            foreach (trainOperator Operator in operatorList)
            {
                if (Operator3Category.SelectedItem != null &&
                    Operator3Category.SelectedItem.ToString().Replace(" ",string.Empty).Equals(Operator.ToString()))
                    return Operator;
            }

            return trainOperator.Unknown;
        }

        /// <summary>
        /// Extact the value for the first commodity analysis Category.
        /// </summary>
        /// <returns>The train operator describing the first analysis Category.</returns>
        public trainCommodity getCommodity1Category()
        {
            /* Convert string to train operator. */
            List<trainCommodity> commodityList = Enum.GetValues(typeof(trainCommodity)).Cast<trainCommodity>().ToList();

            foreach (trainCommodity commodity in commodityList)
            {
                if (Commodity1Category.SelectedItem != null &&
                    Commodity1Category.SelectedItem.ToString().Replace(" ", string.Empty).Equals(commodity.ToString()))
                    return commodity;
            }
            return trainCommodity.Unknown;
        }

        /// <summary>
        /// Extact the value for the second commodity analysis Category.
        /// </summary>
        /// <returns>The train operator describing the second analysis Category.</returns>
        public trainCommodity getCommodity2Category()
        {
            /* Convert string to train operator. */
            List<trainCommodity> commodityList = Enum.GetValues(typeof(trainCommodity)).Cast<trainCommodity>().ToList();

            foreach (trainCommodity commodity in commodityList)
            {
                if (Commodity2Category.SelectedItem != null &&
                    Commodity2Category.SelectedItem.ToString().Replace(" ", string.Empty).Equals(commodity.ToString()))
                    return commodity;
            }
            return trainCommodity.Unknown;
        }

        /// <summary>
        /// Extact the value for the third commodity analysis Category.
        /// </summary>
        /// <returns>The train operator describing the third analysis Category.</returns>
        public trainCommodity getCommodity3Category()
        {
            /* Convert string to train operator. */
            List<trainCommodity> commodityList = Enum.GetValues(typeof(trainCommodity)).Cast<trainCommodity>().ToList();

            foreach (trainCommodity commodity in commodityList)
            {
                if (Commodity3Category.SelectedItem != null &&
                    Commodity3Category.SelectedItem.ToString().Replace(" ", string.Empty).Equals(commodity.ToString()))
                    return commodity;
            }
            return trainCommodity.Unknown;
        }

        /// <summary>
        /// Extract the value of the first train type
        /// </summary>
        /// <returns>The train type describing the first analysis category</returns>
        public trainType getTrainType1Category()
        {
            /* Convert string to train operator. */
            List<trainType> trainTypeList = Enum.GetValues(typeof(trainType)).Cast<trainType>().ToList();

            foreach (trainType trainType in trainTypeList)
            {
                if (trainType1.Text.Equals(trainType.ToString()))
                    return trainType;
            }
            return trainType.Unknown;
        }


        /// <summary>
        /// Extract the value of the third train type
        /// </summary>
        /// <returns>The train type describing the third analysis category</returns>
        public trainType getTrainType2Category()
        {
            /* Convert string to train operator. */
            List<trainType> trainTypeList = Enum.GetValues(typeof(trainType)).Cast<trainType>().ToList();

            foreach (trainType trainType in trainTypeList)
            {
                if (trainType2.Text.Equals(trainType.ToString()))
                    return trainType;
            }
            return trainType.Unknown;
        }

        /// <summary>
        /// Extract the value of the third train type
        /// </summary>
        /// <returns>The train type describing the third analysis category</returns>
        public trainType getTrainType3Category()
        {
            /* Convert string to train operator. */
            List<trainType> trainTypeList = Enum.GetValues(typeof(trainType)).Cast<trainType>().ToList();

            foreach (trainType trainType in trainTypeList)
            {
                if (trainType3.Text.Equals(trainType.ToString()))
                    return trainType;
            }
            return trainType.Unknown;
        }

        /// <summary>
        /// Convert the analysis Category to the appropriate train operator.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Operator1Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Set the analysis Category to train operator. */
            Settings.analysisCategory = analysisCategory.TrainOperator;

            /* Convert operator Category to train operator. */
            List<trainOperator> operatorList = Enum.GetValues(typeof(trainOperator)).Cast<trainOperator>().ToList();

            foreach (trainOperator Operator in operatorList)
            {
                /* Match the string of the analysis Category to the train operator. */
                if (Operator1Category.Text.ToString().Replace(" ", string.Empty).Equals(Operator.ToString()))
                    Settings.Category1Operator = Operator;
            }

            Category1SimulationLabel.Text = Settings.Category1Operator.ToString();

            /* Set other anaylsis parameters to default */
            powerToWeightRatioAnalysis.Checked = false;
            setDefaultCommodityAnalysisParameters();

        }

        /// <summary>
        /// Convert the analysis Category to the appropriate train operator.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Operator2Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Set the analysis Category to train operator. */
            Settings.analysisCategory = analysisCategory.TrainOperator;

            /* Convert operator Category to train operator. */
            List<trainOperator> operatorList = Enum.GetValues(typeof(trainOperator)).Cast<trainOperator>().ToList();

            foreach (trainOperator Operator in operatorList)
            {
                /* Match the string of the analysis Category to the train operator. */
                if (Operator2Category.Text.ToString().Replace(" ", string.Empty).Equals(Operator.ToString()))
                        Settings.Category2Operator = Operator;
            }
            Category2SimulationLabel.Text = Settings.Category2Operator.ToString();

            /* Set other anaylsis parameters to default */
            powerToWeightRatioAnalysis.Checked = false;
            setDefaultCommodityAnalysisParameters();

        }

        /// <summary>
        /// Convert the analysis Category to the appropriate train operator.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Operator3Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Set the analysis Category to train operator. */
            Settings.analysisCategory = analysisCategory.TrainOperator;

            /* Convert operator Category to train operator. */
            List<trainOperator> operatorList = Enum.GetValues(typeof(trainOperator)).Cast<trainOperator>().ToList();

            foreach (trainOperator Operator in operatorList)
            {
                /* Match the string of the analysis Category to the train operator. */
                if (Operator3Category.Text.ToString().Replace(" ", string.Empty).Equals(Operator.ToString()))
                    Settings.Category3Operator = Operator;
            }
            Category3SimulationLabel.Text = Settings.Category3Operator.ToString();

            /* Set other anaylsis parameters to default */
            powerToWeightRatioAnalysis.Checked = false;
            setDefaultCommodityAnalysisParameters();

        }

        /// <summary>
        /// Convert the analysis Category to the appropriate commodity.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Commodity1Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Set the analysis Category to train commodity. */
            Settings.analysisCategory = analysisCategory.TrainCommodity;

            /* Convert string to train operator. */
            List<trainCommodity> commodityList = Enum.GetValues(typeof(trainCommodity)).Cast<trainCommodity>().ToList();

            foreach (trainCommodity commodity in commodityList)
            {
                /* Match the string of the analysis Category to the commodity. */
                if (Commodity1Category.Text.ToString().Replace(" ", string.Empty).Equals(commodity.ToString()))
                    Settings.Category1Commodity = commodity;
            }
            Category1SimulationLabel.Text = Settings.Category1Commodity.ToString();

            /* Set other anaylsis parameters to default */
            powerToWeightRatioAnalysis.Checked = false;
            setDefaultOperatorAnalysisParameters();

        }

        /// <summary>
        /// Convert the analysis Category to the appropriate commodity.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Commodity2Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Set the analysis Category to train commodity. */
            Settings.analysisCategory = analysisCategory.TrainCommodity;

            /* Convert string to train operator. */
            List<trainCommodity> commodityList = Enum.GetValues(typeof(trainCommodity)).Cast<trainCommodity>().ToList();

            foreach (trainCommodity commodity in commodityList)
            {
                /* Match the string of the analysis Category to the commodity. */
                if (Commodity2Category.Text.ToString().Replace(" ", string.Empty).Equals(commodity.ToString()))
                    Settings.Category2Commodity = commodity;
            }
            Category2SimulationLabel.Text = Settings.Category2Commodity.ToString();

            /* Set other anaylsis parameters to default */
            powerToWeightRatioAnalysis.Checked = false;
            setDefaultOperatorAnalysisParameters();

        }

        /// <summary>
        /// Convert the analysis Category to the appropriate commodity.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Commodity3Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Set the analysis Category to train commodity. */
            Settings.analysisCategory = analysisCategory.TrainCommodity;

            /* Convert string to train operator. */
            List<trainCommodity> commodityList = Enum.GetValues(typeof(trainCommodity)).Cast<trainCommodity>().ToList();

            foreach (trainCommodity commodity in commodityList)
            {
                /* Match the string of the analysis Category to the commodity. */
                if (Commodity3Category.Text.ToString().Replace(" ", string.Empty).Equals(commodity.ToString()))
                    Settings.Category3Commodity = commodity;
            }
            Category3SimulationLabel.Text = Settings.Category3Commodity.ToString();

            /* Set other anaylsis parameters to default */
            powerToWeightRatioAnalysis.Checked = false;
            setDefaultOperatorAnalysisParameters();

        }

        /// <summary>
        /// Set Default commodity items.
        /// </summary>
        private void setDefaultCommodityAnalysisParameters()
        {
            Commodity1Category.SelectedItem = "";
            Commodity1Category.Text = "";

            Commodity2Category.SelectedItem = "";
            Commodity2Category.Text = "";

            Commodity3Category.SelectedItem = "";
            Commodity3Category.Text = "";
        }

        /// <summary>
        /// Set Default train operator items.
        /// </summary>
        private void setDefaultOperatorAnalysisParameters()
        {
            Operator1Category.SelectedItem = "";
            Operator1Category.Text = "";

            Operator2Category.SelectedItem = "";
            Operator2Category.Text = "";

            Operator3Category.SelectedItem = "";
            Operator3Category.Text = "";
        }

        /// <summary>
        /// Populate the Setting parameters from the form provided.
        /// </summary>
        /// <param name="form">The Form object containg the form parameters.</param>
        public void populateFormParameters(TrainPerformance form)
        {

            /* Extract the form parameters. */
            Settings.dateRange = form.getDateRange();
            Settings.excludeListOfTrains = form.getTrainListExcludeFlag();
            Settings.startKm = form.getStartKm();
            Settings.endKm = form.getEndKm();
            Settings.interval = form.getInterval();
            Settings.IgnoreGaps = form.getIgnoreGapsFlag();
            Settings.minimumJourneyDistance = form.getJourneydistance();
            Settings.loopSpeedThreshold = form.getLoopFactor();
            Settings.loopBoundaryThreshold = form.getLoopBoundary();
            Settings.TSRwindowBoundary = form.getTSRWindow();
            Settings.timeThreshold = form.getTimeSeparation();
            Settings.distanceThreshold = form.getDataSeparation();
            Settings.Category1LowerBound = form.getCategory1LowerBound();
            Settings.Category1UpperBound = form.getCategory1UpperBound();
            Settings.Category2LowerBound = form.getCategory2LowerBound();
            Settings.Category2UpperBound = form.getCategory2UpperBound();
            Settings.analysisCategory = form.getAnalysisCategory();
            Settings.Category1Commodity = form.getCommodity1Category();
            Settings.Category1Operator = form.getOperator1Category();
            Settings.Category2Commodity = form.getCommodity2Category();
            Settings.Category2Operator = form.getOperator2Category();
            Settings.Category3Commodity = form.getCommodity3Category();
            Settings.Category3Operator = form.getOperator3Category();
            Settings.Category1TrainType = form.getTrainType1Category();
            Settings.Category2TrainType = form.getTrainType2Category();
            Settings.Category3TrainType = form.getTrainType3Category();

        }

        /// <summary>
        /// Validate the form parameters are within logical boundaries.
        /// </summary>
        /// <returns>True if all parameters are valid.</returns>
        public bool areFormParametersValid()
        {

            if (Settings.dateRange == null ||
                Settings.dateRange[0] > DateTime.Today || Settings.dateRange[1] > DateTime.Today ||
                Settings.dateRange[0] > Settings.dateRange[1])
                return false;

            if (Settings.startKm < 0 || Settings.startKm > Settings.endKm)
                return false;

            if (Settings.endKm < 0 || Settings.endKm < Settings.startKm)
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

            if (Settings.Category1LowerBound < 0)
                return false;

            if (Settings.Category1UpperBound < 0)
                return false;

            if (Settings.Category2LowerBound < 0)
                return false;

            if (Settings.Category2UpperBound < 0)
                return false;

            return true;


        }

        /// <summary>
        /// This function sets all the testing parameters for the Cullerin Ranges data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setCulleranRangesParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Cullerin Ranges 2016-20170614.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Cullerin Ranges Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Cullerin Ranges TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Increasing 3.31_ThuW1.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Decreasing 3.33_TueW1.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Increasing 4.8_FriW1.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Decreasing 4.68_WedW1.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,6,1);

            /* Interpolation parameters */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "220";
            endInterpolationKm.Text = "320";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "80";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boundaries. */
            powerToWeightRatioAnalysis.Checked = true;
            Category1LowerBound.Text = "2";
            Category1UpperBound.Text = "4";
            Category2LowerBound.Text = "4";
            Category2UpperBound.Text = "6";

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainPowerToWeight;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
            
        }

        /// <summary>
        /// This function sets all the testing parameters for the Gunnedah Basin data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setGunnedahBasinParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Extract Gunnedah Basin 2018-Aug.txt";
            
            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Increasing-60.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2018, 5, 20);
            toDate.Value = new DateTime(2018, 8, 20);

            /* Interpolation Parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "280";
            endInterpolationKm.Text = "540";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "50";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "2";

            Category1LowerBound.Text = "0";
            Category1UpperBound.Text = "100";
            Category2LowerBound.Text = "100";
            Category2UpperBound.Text = "200";

            /* Anlaysis Parameters */
            powerToWeightRatioAnalysis.Checked = false;

            Settings.analysisCategory = analysisCategory.TrainOperator;

            Operator1Category.SelectedItem = "Pacific National";
            Settings.Category1Operator = trainOperator.PacificNational;
            Operator2Category.SelectedItem = "Aurizon";
            Settings.Category2Operator = trainOperator.Aurizon;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets all the testing parameters for the Macarthur to Botany data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setMacartur2BotanyParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany data.txt";
           
            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Botany to Macarthur - increasing - 3.33_ThuW1.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany - decreasing - 3.20_SatW1.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Botany to Macarthur - increasing - 7.87_ThuW1.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany - decreasing - 6.97_SatW1.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,2,1);

            /* Interpoaltion Parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "5";
            endInterpolationKm.Text = "70";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "40";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boundaries. */
            powerToWeightRatioAnalysis.Checked = true;

            Category1LowerBound.Text = "1.5";
            Category1UpperBound.Text = "4.5";
            Category2LowerBound.Text = "4.5";
            Category2UpperBound.Text = "11.5";

            /* Anlaysis Parameters */            
            Settings.analysisCategory = analysisCategory.TrainPowerToWeight;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets all the testing parameters for the Melbourne to Cootamundra data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setMelbourne2CootamundraParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Melbourne to Cootamundra data.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Melbourne to Cootamundra Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Melbourne to Cootamundra TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Increasing sim 3.5.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\decreasing sim 3.5.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Increasing sim 4.6.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\decreasing sim 4.6.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,4,1);

            /* Interpolation Parameters */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "5";
            endInterpolationKm.Text = "505";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "300";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boundaries. */
            powerToWeightRatioAnalysis.Checked = true;
            Category1LowerBound.Text = "2.5";
            Category1UpperBound.Text = "4";
            Category2LowerBound.Text = "4";
            Category2UpperBound.Text = "5.5";

            /* Anlaysis Parameters */            
            Settings.analysisCategory = analysisCategory.TrainPowerToWeight;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets all the testing parameters for the Tarcoola To Kalgoorlie data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setTarcoola2KalgoorlieParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie data.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\increasing 2.2_ThuW1.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\decreasing 2.6_SunW1.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\increasing 3.4_FriW1.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\decreasing 3.5_MonW1.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016, 6, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "950";
            endInterpolationKm.Text = "1400";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = true;
            Category1LowerBound.Text = "1.5";
            Category1UpperBound.Text = "3";
            Category2LowerBound.Text = "3";
            Category2UpperBound.Text = "4";

            /* Anlaysis Parameters */            
            Settings.analysisCategory = analysisCategory.TrainPowerToWeight;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets all the testing parameters for the full length of the 
        /// Tarcoola to Kalgoorlie data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieParameters2(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\GP1 SCT Vans - increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\GP1 SCT Vans - increasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP4 Intermodal - increasing.csv";
            //Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            //Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP4 Intermodal - increasing.csv";
            //Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            //Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP5 Intermodal - increasing.csv";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP5 Intermodal - increasing.csv";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 2, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;
            
            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            //TrainType1Category.SelectedItem = "Adelaide - Perth";
            //Settings.Category1TrainType = trainType.AdelaidePerth;
            //TrainType2Category.SelectedItem = "Melbourne - Perth";
            //Settings.Category2TrainType = trainType.MelbournePerth;
            //TrainType3Category.SelectedItem = "Perth - Sydney";
            //Settings.Category3TrainType = trainType.PerthSydney;

            trainType1.Text = "GP1";
            Settings.Category1TrainType = trainType.GP1;
            trainType2.Text = null;
            Settings.Category2TrainType = trainType.Unknown;
            trainType3.Text = null;
            Settings.Category3TrainType = trainType.Unknown;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets the first batch of Tarcoola to Kalgoorlie data for 
        /// processing the 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieBatch1(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\AP1 Steel - increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\AP1 Steel - increasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\AP2 Steel - increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\AP2 Steel - increasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\AP8 Indian Pacific - increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\AP8 Indian Pacific - increasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "8";
            loopSpeedThreshold.Text = "80";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            trainType1.Text = "AP1";
            Settings.Category1TrainType = trainType.AP1;
            trainType2.Text = "AP2";
            Settings.Category2TrainType = trainType.AP2;
            trainType3.Text = "AP8";
            Settings.Category3TrainType = trainType.AP8;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets the second batch of Tarcoola to Kalgoorlie data for 
        /// processing the 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieBatch2(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\GP1 SCT Vans - increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\GP1 SCT Vans - increasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP1 Intermodal - increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP1 Intermodal - increasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP2 Steel - increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP2 Steel - increasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "8";
            loopSpeedThreshold.Text = "80";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            trainType1.Text = "GP1";
            Settings.Category1TrainType = trainType.GP1;
            trainType2.Text = "MP1";
            Settings.Category2TrainType = trainType.MP1;
            trainType3.Text = "MP2";
            Settings.Category3TrainType = trainType.MP2;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets the third batch of Tarcoola to Kalgoorlie data for 
        /// processing the 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieBatch3(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP4 Intermodal - increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP4 Intermodal - increasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP5 Intermodal - increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP5 Intermodal - increasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP7 Express - increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP7 Express - increasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "8";
            loopSpeedThreshold.Text = "80";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            trainType1.Text = "MP4";
            Settings.Category1TrainType = trainType.MP4;
            trainType2.Text = "MP5";
            Settings.Category2TrainType = trainType.MP5;
            trainType3.Text = "MP7";
            Settings.Category3TrainType = trainType.MP7;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets the fourth batch of Tarcoola to Kalgoorlie data for 
        /// processing the 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieBatch4(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP9 Intermodal - increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\MP9 Intermodal - increasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\SP5 Intermodal - increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\SP5 Intermodal - increasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\SP7 Intermodal - increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\SP7 Intermodal - increasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "8";
            loopSpeedThreshold.Text = "80";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            trainType1.Text = "MP9";
            Settings.Category1TrainType = trainType.MP9;
            trainType2.Text = "SP5";
            Settings.Category2TrainType = trainType.SP5;
            trainType3.Text = "SP7";
            Settings.Category3TrainType = trainType.SP7;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets the fifth batch of Tarcoola to Kalgoorlie data for 
        /// processing the 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieBatch5(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PA8 Indian Pacific - decreasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PA8 Indian Pacific - decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PG1 SCT Vans - decreasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PG1 SCT Vans - decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM1 Intermodal - decreasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM1 Intermodal - decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "8";
            loopSpeedThreshold.Text = "80";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            trainType1.Text = "PA8";
            Settings.Category1TrainType = trainType.PA8;
            trainType2.Text = "PG1";
            Settings.Category2TrainType = trainType.PG1;
            trainType3.Text = "PM1";
            Settings.Category3TrainType = trainType.PM1;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets the six batch of Tarcoola to Kalgoorlie data for 
        /// processing the 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieBatch6(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM4 Steel - decreasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM4 Steel - decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM5 Intermodal - decreasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM5 Intermodal - decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM6 Intermodal - decreasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM6 Intermodal - decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "8";
            loopSpeedThreshold.Text = "80";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            trainType1.Text = "PM4";
            Settings.Category1TrainType = trainType.PM4;
            trainType2.Text = "PM5";
            Settings.Category2TrainType = trainType.PM5;
            trainType3.Text = "PM6";
            Settings.Category3TrainType = trainType.PM6;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets the seventh batch of Tarcoola to Kalgoorlie data for 
        /// processing the 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieBatch7(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM7 Express - decreasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM7 Express - decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM9 Intermodal - decreasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PM9 Intermodal - decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\GP1 SCT Vans - increasing.csv";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\GP1 SCT Vans - increasing.csv";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "8";
            loopSpeedThreshold.Text = "80";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            trainType1.Text = "PM7";
            Settings.Category1TrainType = trainType.PM7;
            trainType2.Text = "PM9";
            Settings.Category2TrainType = trainType.PM9;
            trainType3.Text = null;
            Settings.Category3TrainType = trainType.Unknown;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets the eightth batch of Tarcoola to Kalgoorlie data for 
        /// processing the 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setTarcoola2KalgoorlieBatch8(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie 2017.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR 2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PS6 Intermodal - decreasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PS6 Intermodal - decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PS7 Steel - decreasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PS7 Steel - decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PX4 Intermodal - decreasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\PX4 Intermodal - decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "500";
            endInterpolationKm.Text = "1700";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "8";
            loopSpeedThreshold.Text = "80";
            TSRWindowBoundary.Text = "1";

            /* Power to weight ratio boudnaries. */
            powerToWeightRatioAnalysis.Checked = false;

            /* Anlaysis Parameters */
            Settings.analysisCategory = analysisCategory.TrainType;

            trainType1.Text = "PS6";
            Settings.Category1TrainType = trainType.PS6;
            trainType2.Text = "PS7";
            Settings.Category2TrainType = trainType.PS7;
            trainType3.Text = "PX4";
            Settings.Category3TrainType = trainType.PX4;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function runs through the 8 batched settings to process all 22 known train types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runTarcoolaToKalgoorlieBatch(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setTarcoola2KalgoorlieBatch1(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);
            TimeSpan t1 = timer.Elapsed;

            setTarcoola2KalgoorlieBatch2(sender, e);
            testExecute(sender, e);
            TimeSpan t2 = timer.Elapsed - t1;

            setTarcoola2KalgoorlieBatch3(sender, e);
            testExecute(sender, e);
            TimeSpan t3 = timer.Elapsed - t2 - t1;

            setTarcoola2KalgoorlieBatch4(sender, e);
            testExecute(sender, e);
            TimeSpan t4 = timer.Elapsed - t3 - t2 - t1;

            setTarcoola2KalgoorlieBatch5(sender, e);
            testExecute(sender, e);
            TimeSpan t5 = timer.Elapsed - t4 - t3 - t2 - t1;

            setTarcoola2KalgoorlieBatch6(sender, e);
            testExecute(sender, e);
            TimeSpan t6 = timer.Elapsed - t5 - t4 - t3 - t2 - t1;

            setTarcoola2KalgoorlieBatch7(sender, e);
            testExecute(sender, e);
            TimeSpan t7 = timer.Elapsed - t6 - t5 - t4 - t3 - t2 - t1;

            setTarcoola2KalgoorlieBatch8(sender, e);
            testExecute(sender, e);
            TimeSpan t8 = timer.Elapsed - t7 - t6 - t5 - t4 - t3 - t2 - t1;

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string batch1Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t1.Hours, t1.Minutes, t1.Seconds, t1.Milliseconds / 10);
            string batch2Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t2.Hours, t2.Minutes, t2.Seconds, t2.Milliseconds / 10);
            string batch3Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t3.Hours, t3.Minutes, t3.Seconds, t3.Milliseconds / 10);
            string batch4Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t4.Hours, t4.Minutes, t4.Seconds, t4.Milliseconds / 10);
            string batch5Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t5.Hours, t5.Minutes, t5.Seconds, t5.Milliseconds / 10);
            string batch6Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t6.Hours, t6.Minutes, t6.Seconds, t6.Milliseconds / 10);
            string batch7Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t7.Hours, t7.Minutes, t7.Seconds, t7.Milliseconds / 10);
            string batch8Time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", t8.Hours, t8.Minutes, t8.Seconds, t8.Milliseconds / 10);

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}.{4:00}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine("Tarcoola to Kalgoorlie RunTime 1: " + batch1Time);
            Console.WriteLine("Tarcoola to Kalgoorlie RunTime 2: " + batch2Time);
            Console.WriteLine("Tarcoola to Kalgoorlie RunTime 3: " + batch3Time);
            Console.WriteLine("Tarcoola to Kalgoorlie RunTime 4: " + batch4Time);
            Console.WriteLine("Tarcoola to Kalgoorlie RunTime 5: " + batch5Time);
            Console.WriteLine("Tarcoola to Kalgoorlie RunTime 6: " + batch6Time);
            Console.WriteLine("Tarcoola to Kalgoorlie RunTime 7: " + batch7Time);
            Console.WriteLine("Tarcoola to Kalgoorlie RunTime 8: " + batch8Time);

            Console.WriteLine("Tarcoola to Kalgoorlie RunTime Total: " + elapsedTime);
        }

        /// <summary>
        /// This function sets all the testing parameters for the the Ulan line
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setUlanLineParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Extract Ulan Data 2018-Aug.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Ulan Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Ulan TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Pacific National - Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Pacific National - Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Aurizon - Increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Aurizon - Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Freightliner - Increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Freightliner - Decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2018, 5, 20);
            toDate.Value = new DateTime(2018,8,20);

            /* Interpolation Parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "280";
            endInterpolationKm.Text = "460";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "100";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            Category1LowerBound.Text = "0";
            Category1UpperBound.Text = "100";
            Category2LowerBound.Text = "100";
            Category2UpperBound.Text = "200";

            /* Anlaysis Parameters */
            powerToWeightRatioAnalysis.Checked = false;

            Settings.analysisCategory = analysisCategory.TrainOperator;

            Operator1Category.SelectedItem = "Pacific National";
            Settings.Category1Operator = trainOperator.PacificNational;
            Operator2Category.SelectedItem = "Aurizon";
            Settings.Category2Operator = trainOperator.Aurizon;
            Operator3Category.SelectedItem = "Freightliner";
            Settings.Category3Operator = trainOperator.Freightliner;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets all the testing parameters for the the Hunter line
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setHunterRegionParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\Extract Hunter Region 2018-Sept.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\KIY to WCK.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\Hunter Region TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\Pacific National - Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\Pacific National - Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\Aurizon - Increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\Aurizon - Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\Freightliner - Increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region\Freightliner - Decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Hunter Region";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2018, 1, 1);
            toDate.Value = new DateTime(2018, 7, 1);

            /* Interpolation Parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "160";
            endInterpolationKm.Text = "290";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "50";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            Category1LowerBound.Text = "0";
            Category1UpperBound.Text = "100";
            Category2LowerBound.Text = "100";
            Category2UpperBound.Text = "200";

            /* Anlaysis Parameters */
            powerToWeightRatioAnalysis.Checked = false;

            Settings.analysisCategory = analysisCategory.TrainOperator;

            Operator1Category.SelectedItem = "Pacific National";
            Settings.Category1Operator = trainOperator.PacificNational;
            Operator2Category.SelectedItem = "Aurizon";
            Settings.Category2Operator = trainOperator.Aurizon;
            Operator3Category.SelectedItem = "Freightliner";
            Settings.Category3Operator = trainOperator.Freightliner;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        }

        /// <summary>
        /// This function sets all the testing parameters for the Macarthur to Botany data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setSouthernHighlandsParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();
            
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Southern Highlands 2018-Aug.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Southern Highlands Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Southern Highlands TSR 2018-201806.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Grain-Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Grain-Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Minerals-Increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Minerals-Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\MB Superfreighter-Increasing.csv";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            //FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\MB Superfreighter-Decreasing.csv";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2018, 1, 1);
            toDate.Value = new DateTime(2018, 7, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "20";   // usaully 55
            endInterpolationKm.Text = "200";    // usually 145
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "20";     // Reduced from 50 to allow the TM coal trains partial journey to be included.
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            Category1LowerBound.Text = "0";
            Category1UpperBound.Text = "0";
            Category2LowerBound.Text = "0";
            Category2UpperBound.Text = "0";

            /* Anlaysis Parameters */
            powerToWeightRatioAnalysis.Checked = false;

            Settings.analysisCategory = analysisCategory.TrainCommodity;

            //trainType1.Text = "MB4";
            //Settings.Category1TrainType = trainType.MB4;
            //trainType2.Text = "BM4";
            //Settings.Category2TrainType = trainType.BM4;
            //trainType3.Text = null;
            //Settings.Category3TrainType = trainType.Unknown;

            //Operator1Category.SelectedItem = "City Rail";
            //Settings.Category1Operator = trainOperator.CityRail;
            //Operator2Category.SelectedItem = "Countrylink";
            //Settings.Category2Operator = trainOperator.Countrylink;
            //Operator3Category.SelectedItem = "Group Remaining";
            //Settings.Category3Operator = trainOperator.GroupRemaining;

            Commodity1Category.SelectedItem = "Grain";
            Settings.Category1Commodity = trainCommodity.Grain;
            Commodity2Category.SelectedItem = "Mineral";
            Settings.Category2Commodity = trainCommodity.Mineral;
            Commodity3Category.SelectedItem = null;
            Settings.Category3Commodity = trainCommodity.Unknown;
        
        }

        /// <summary>
        /// This function sets all the testing parameters for the Macarthur to Port Kembla
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setPortKemblaParameters(object sender, EventArgs e)
        {
            /* Reset default parameters before setting new scenario parameters. */
            resetDefaultParameters();

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Port Kembla to Moss Vale data.txt";
            
            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = SystemColors.ActiveCaptionText;
            simICEDataFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Port Kembla to Moss Vale Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = SystemColors.ActiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Port Kembla to  Moss Vale TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Coal-Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Coal-Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Grain-Increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Grain-Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Minerals-Increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Minerals-Decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = SystemColors.ActiveCaptionText;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2017, 6, 10);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "80";
            endInterpolationKm.Text = "160";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "50";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            Category1LowerBound.Text = "0";
            Category1UpperBound.Text = "0";
            Category2LowerBound.Text = "0";
            Category2UpperBound.Text = "0";

            /* Anlaysis Parameters */
            powerToWeightRatioAnalysis.Checked = false;

            Settings.analysisCategory = analysisCategory.TrainCommodity;

            Operator1Category.SelectedItem = null;
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = "Coal";
            Settings.Category1Commodity = trainCommodity.Coal;
            Commodity2Category.SelectedItem = "Grain";
            Settings.Category2Commodity = trainCommodity.Grain;
            Commodity3Category.SelectedItem = "Mineral";
            Settings.Category3Commodity = trainCommodity.Mineral;

        }

        /// <summary>
        /// Function determines if the testing parameters for Culleran Ranges need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void CulleranRanges_CheckedChanged(object sender, EventArgs e)
        {
            /* If Culleran Ranges tesging flag is checked, set the appropriate parameters. */
            if (CulleranRanges.Checked)
                setCulleranRangesParameters(sender, e);
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// Function determines if the testing parameters for Gunnedah Basin need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void GunnedahBasin_CheckedChanged(object sender, EventArgs e)
        {
            /* If Gunnedah Basin testing flag is checked, set the appropriate parameters. */
            if (GunnedahBasin.Checked)
                setGunnedahBasinParameters(sender, e);
            else
                resetDefaultParameters();
        }

        // <summary>
        /// Function determines if the testing parameters for the Ulan line need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void UlanLine_CheckedChanged(object sender, EventArgs e)
        {
            /* If Gunnedah Basin testing flag is checked, set the appropriate parameters. */
            if (UlanLine.Checked)
                setUlanLineParameters(sender, e);
            else
                resetDefaultParameters();
        }

        // <summary>
        /// Function determines if the testing parameters for the Hunter line need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Hunter_CheckedChanged(object sender, EventArgs e)
        {
            /* If Hunter testing flag is checked, set the appropriate parameters. */
            if (Hunter.Checked)
                setHunterRegionParameters(sender, e);
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// Function determines if the testing parameters for Macarthur to Botany need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Macarthur2Botany_CheckedChanged(object sender, EventArgs e)
        {
            /* If Macarthur to Botany testing flag is checked, set the appropriate parameters. */
            if (Macarthur2Botany.Checked)
                setMacartur2BotanyParameters(sender, e);
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// Function determines if the testing parameters for Melbourne to Cootamundra need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Melbourne2Cootamundra_CheckedChanged(object sender, EventArgs e)
        {
            /* If Melbourne to Cootamundra testing flag is checked, set the appropriate parameters. */
            if (Melbourne2Cootamundra.Checked)
                setMelbourne2CootamundraParameters(sender, e);
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// Function determines if the testing parameters for Tarcoola to Kalgoorlie need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Tarcoola2Kalgoorlie_CheckedChanged(object sender, EventArgs e)
        {
            /* If Tarcoola to Kalgoorlie testing flag is checked, set the appropriate parameters. */
            if (Tarcoola2Kalgoorlie.Checked)
                setTarcoola2KalgoorlieParameters2(sender, e);
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// Function determines if the testing parameters for the Southern Highlands need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void SouthernHighlands_CheckedChanged(object sender, EventArgs e)
        {
            /* If Tarcoola to Kalgoorlie testing flag is checked, set the appropriate parameters. */
            if (SouthernHighlands.Checked)
                setSouthernHighlandsParameters(sender, e);
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// Function determines if the testing parameters for Port Kembla need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void PortKembla_CheckedChanged(object sender, EventArgs e)
        {
            /* If Tarcoola to Kalgoorlie testing flag is checked, set the appropriate parameters. */
            if (PortKembla.Checked)
                setPortKemblaParameters(sender, e);
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// function resets the train performance analysis form to default settings.
        /// </summary>
        private void resetDefaultParameters()
        {

            /* Data File */
            FileSettings.dataFile = null;
            IceDataFile.Text = "<Required>";
            IceDataFile.ForeColor = SystemColors.InactiveCaptionText;
            simICEDataFile.Text = "Data File Loaded from FileSelection tab";
            simICEDataFile.ForeColor = SystemColors.InactiveCaptionText;
                        
            /* Geometry File */
            FileSettings.geometryFile = null;
            GeometryFile.Text = "<Required>";
            GeometryFile.ForeColor = SystemColors.InactiveCaptionText;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = null;
            temporarySpeedRestrictionFile.Text = "<Required>";
            temporarySpeedRestrictionFile.ForeColor = SystemColors.InactiveCaptionText;

            /* Simulation files */
            FileSettings.simulationFiles = new List<string>(new string[6]);
            Category1IncreasingSimulationFile.Text = "<Required>";
            Category1IncreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            Category1DecreasingSimulationFile.Text = "<Required>";
            Category1DecreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            Category2IncreasingSimulationFile.Text = "<Required>";
            Category2IncreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            Category2DecreasingSimulationFile.Text = "<Required>";
            Category2DecreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            Category3IncreasingSimulationFile.Text = "<Optional>";
            Category3IncreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            Category3DecreasingSimulationFile.Text = "<Optional>";
            Category3DecreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;



            /* Destination Folder */
            FileSettings.aggregatedDestination = null;
            resultsDestination.Text = "<Required>";
            resultsDestination.ForeColor = SystemColors.InactiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016, 2, 1);

            /* Interpolation Parameters */
            excludeListOfTrains.Checked = false;
            IgnoreGaps.Checked = false;

            startInterpolationKm.Text = "0";
            endInterpolationKm.Text = "100";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "80";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "2";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            Category1LowerBound.Text = "0";
            Category1UpperBound.Text = "0";
            Category2LowerBound.Text = "0";
            Category2UpperBound.Text = "0";

            /* Analysis parameters. */
            powerToWeightRatioAnalysis.Checked = false;

            Settings.analysisCategory = analysisCategory.Unknown;

            Operator1Category.SelectedItem = null;
            Operator1Category.Text = "";
            Settings.Category1Operator = trainOperator.Unknown;
            Operator2Category.SelectedItem = null;
            Operator2Category.Text = "";
            Settings.Category2Operator = trainOperator.Unknown;
            Operator3Category.SelectedItem = null;
            Operator3Category.Text = "";
            Settings.Category3Operator = trainOperator.Unknown;

            Commodity1Category.SelectedItem = null;
            Commodity1Category.Text = "";
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Commodity2Category.Text = "";
            Settings.Category2Commodity = trainCommodity.Unknown;
            Commodity3Category.SelectedItem = null;
            Commodity3Category.Text = "";
            Settings.Category3Commodity = trainCommodity.Unknown;

        }

        /// <summary>
        /// Set Cullerin Ranges parameters and Execute the analysis without the interacting with the form.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void runCulleranRanges(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();
            
            /* Set the analysis parameters. */
            setCulleranRangesParameters(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Culleran ranges RunTime " + elapsedTime);
        }

        /// <summary>
        /// Set Gunnedah Basin parameters and Execute the analysis without the interacting with the form.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void runGunnedahBasin(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setGunnedahBasinParameters(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Gunnedah basin RunTime " + elapsedTime);
        }

        /// <summary>
        /// Set Ulan Line parameters and Execute the analysis without the interacting with the form.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void runUlanLine(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setUlanLineParameters(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Ulan Line RunTime " + elapsedTime);
        }

        /// <summary>
        /// Set Macarthur to Port Botany parameters and Execute the analysis without the interacting with the form.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void runMacarthurToBotany(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setMacartur2BotanyParameters(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Macarthur to Port Botany RunTime " + elapsedTime);
        }

        /// <summary>
        /// Set Melbourne to Cootamundra parameters and Execute the analysis without the interacting with the form.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void runMelbourneToCootamundra(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setMelbourne2CootamundraParameters(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Melbourne to Cootamundra RunTime " + elapsedTime);
        }

        /// <summary>
        /// Set Tarcoola to Kalgoorlie parameters and Execute the analysis without the interacting with the form.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void runTarcoolaToKalgoorlie(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setTarcoola2KalgoorlieParameters2(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Tarcoola to Kalgoorlie RunTime " + elapsedTime);
        }

        /// <summary>
        /// Set Southern Highlands parameters and Execute the analysis without the interacting with the form.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void runSouthernHighlands(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setSouthernHighlandsParameters(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Southern Highlands RunTime " + elapsedTime);
        }

        /// <summary>
        /// Set port Kembla parameters and Execute the analysis without the interacting with the form.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void runPortKembla(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setPortKemblaParameters(sender, e);
            /* Simualte pressing execute button. */
            testExecute(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Port Kembla RunTime " + elapsedTime);
        }

        /// <summary>
        /// Event Handler function for the timeCounter. 
        /// This display the dynamic execution time of the program.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        void tickTimer(object sender, EventArgs e)
        {
            /* Stop the timer when stopTheClock is set to true. */
            if (stopTheClock)
            {
                ((Timer)sender).Stop();
                /* Reset the static timer properties. */
                timeCounter = 0;
                stopTheClock = false;
                return;
            }

            /* Increment the timer*/
            ++timeCounter;

            /* Convert the timeCounter to hours, minutes and seconds. */
            double hours = timeCounter / secPerHour;
            double minutes = (hours - (int)hours) * minutesPerHour;
            double seconds = (minutes - (int)minutes) * secPerMinute;

            /* Format a string for display on the form. */
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", (int)hours, (int)minutes, (int)seconds);
            ((Label)((Timer)sender).Tag).Text = elapsedTime;
        }

        


    }
}
