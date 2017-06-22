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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Globalsettings;
using System.Reflection;

namespace TRAP
{
    public partial class TrainPerformance : Form
    {

        public static Tools tool = new Tools();
        public static Processing processing = new Processing();
        public static TrackGeometry track = new TrackGeometry();

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

            //runCulleranRanges(sender, e);             /* Insufficient TSR data */ 
            runGunnedBasin(sender, e);                  // Run Time: 01:06:43.43
            runUlanLine(sender, e);                     // Run Time: 01:20:22.35
            //runMacarthurToBotany(sender, e);          /* Insufficient TSR data */ 
            //runMelbourneToCootamundra(sender, e);     /* Insufficient TSR data */ 
            //runTarcoolaToKalgoorlie(sender, e);       /* Insufficient TSR data */ 
            runSouthernHighlands(sender, e);            // Run Time: 01:15:35.47
            runPortKembla(sender, e);                   // Run Time: 00:11:44.02
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
            FileSettings.dataFile = tool.browseFile("Select the data file.");
            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;
            
        }

        /// <summary>
        /// Select the geometry file required to process the train data.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectGeometryFile_Click(object sender, EventArgs e)
        {
            FileSettings.geometryFile = tool.browseFile("Select the geometry file.");
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Select the Temporary Speed Restriction file required to process the train data.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectTSRFile_Click(object sender, EventArgs e)
        {
            FileSettings.temporarySpeedRestrictionFile = tool.browseFile("Select the TSR file.");
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Select the file containing the list of trains to exclude from the processing.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectTrainFile_Click(object sender, EventArgs e)
        {
            FileSettings.trainListFile = tool.browseFile("Select the train list file.");
            trainListFile.Text = Path.GetFileName(FileSettings.trainListFile);
            trainListFile.ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Calculate the average power to weight ratio required for the simulations.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void simulationPowerToWeightRatios_Click(object sender, EventArgs e)
        {
            /* Extract the form parameters. */
            processing.populateFormParameters(this);

            /* Validate the form parameters. */
            if (!processing.areFormParametersValid())
            {
                tool.messageBox("One or more parameters are invalid.");
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
            TrainRecords = FileOperations.readICEData(FileSettings.dataFile, excludeTrainList);


            if (TrainRecords.Count() == 0)
            {
                tool.messageBox("There is no data within the specified boundaries.\nCheck the processing parameters.");
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
                CleanTrainRecords = Algorithm.CleanData(OrderdTrainRecords, trackGeometry);
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
            filename = tool.browseFile(browseString);
            FileSettings.simulationFiles[index] = filename;
            simulationFile.Text = Path.GetFileName(filename);
            simulationFile.ForeColor = System.Drawing.Color.Black;
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
            FileSettings.aggregatedDestination = tool.selectFolder();
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Perform the analysis based on the input parameters.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Execute_Click(object sender, EventArgs e)
        {
            /* Populate the parameters. */
            processing.populateFormParameters(this);
            /* Validate the form parameters. */
            if (!processing.areFormParametersValid())
            {
                tool.messageBox("One or more parameters are invalid.");
                return;
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Run the train performance analysis. */
            List<Train> trains = new List<Train>();
            trains = Algorithm.trainPerformance();

            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);


            /* Populate the counts for each train Category. */
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

            executionTime.Text = elapsedTime;

#if (!TESTING)
            tool.messageBox("Program Complete.");      
#endif

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
        /// Extract the analysis Category.
        /// </summary>
        /// <returns>The analysis Category</returns>
        public analysisCategory getAnalysisCategory()
        {
            if (getPowerToWeightRatioAnalysis())
                return analysisCategory.TrainPowerToWeight;
            
            if (getOperator1Category() != trainOperator.Unknown)
                return analysisCategory.TrainOperator;

            if (getCommodity1Category() != trainCommodity.Unknown)
                return analysisCategory.TrainCommodity;

            return analysisCategory.Unknown;
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
        /// Convert the analysis Category to the appropriate train operator.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Operator1Category_SelectedValueChanged(object sender, EventArgs e)
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
        private void Operator2Category_SelectedValueChanged(object sender, EventArgs e)
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
        /// This function sets all the testing parameters for the Cullerin Ranges data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setCulleranRangesParameters(object sender, EventArgs e)
        {
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Cullerin Ranges 2016-20170614.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Cullerin Ranges Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Cullerin Ranges TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Increasing 3.31_ThuW1.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Decreasing 3.33_TueW1.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Increasing 4.8_FriW1.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Decreasing 4.68_WedW1.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,4,1);

            /* Interpolation parameters */
            excludeListOfTrains.Checked = false;

            startInterpolationKm.Text = "220";
            endInterpolationKm.Text = "320";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "80";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
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
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin Data 2017-20170529.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Increasing-60.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017,5,1);

            /* Interpolation Parameters. */
            excludeListOfTrains.Checked = false;

            startInterpolationKm.Text = "264";
            endInterpolationKm.Text = "541";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "250";
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

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany data.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Botany to Macarthur - increasing - 3.33_ThuW1.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany - decreasing - 3.20_SatW1.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Botany to Macarthur - increasing - 7.87_ThuW1.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany - decreasing - 6.97_SatW1.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,2,1);

            /* Interpoaltion Parameters. */
            excludeListOfTrains.Checked = false;

            startInterpolationKm.Text = "5";
            endInterpolationKm.Text = "70";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "40";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
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

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Melbourne to Cootamundra data.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Melbourne to Cootamundra Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Melbourne to Cootamundra TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Increasing sim 3.5.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\decreasing sim 3.5.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Increasing sim 4.6.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\decreasing sim 4.6.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,4,1);

            /* Interpolation Parameters */
            excludeListOfTrains.Checked = false;

            startInterpolationKm.Text = "5";
            endInterpolationKm.Text = "505";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "400";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
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

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie data.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\increasing 2.2_ThuW1.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\decreasing 2.6_SunW1.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\increasing 3.4_FriW1.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\decreasing 3.5_MonW1.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[4] = "";
            //Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //Category3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles[5] = "";
            //Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //Category3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016, 2, 1);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;

            startInterpolationKm.Text = "950";
            endInterpolationKm.Text = "1400";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
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
        /// This function sets all the testing parameters for the the Ulan line
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setUlanLineParameters(object sender, EventArgs e)
        {
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Ulan Data 2017-20170531.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Ulan Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Ulan TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Pacific National - Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Pacific National - Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Aurizon - Increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Aurizon - Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Freightliner - Increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Freightliner - Decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017,6,10);

            /* Interpolation Parameters. */
            excludeListOfTrains.Checked = false;

            startInterpolationKm.Text = "280";
            endInterpolationKm.Text = "460";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "100";
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

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Southern Highlands Data 2016-201706.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Southern Highlands Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\Southern Highlands TSR 2016-2017.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\CityRail Passenger-Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\CityRail Passenger-Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\CountryLink Passenger-Increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\CountryLink Passenger-Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\MB Superfreighter-Increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands\MB Superfreighter-Decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Southern Highlands";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017, 6, 10);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;

            startInterpolationKm.Text = "55";
            endInterpolationKm.Text = "145";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "50";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            Category1LowerBound.Text = "0";
            Category1UpperBound.Text = "0";
            Category2LowerBound.Text = "0";
            Category2UpperBound.Text = "0";

            /* Anlaysis Parameters */
            powerToWeightRatioAnalysis.Checked = false;

            Settings.analysisCategory = analysisCategory.TrainOperator;

            Operator1Category.SelectedItem = "City Rail";
            Settings.Category1Operator = trainOperator.CityRail;
            Operator2Category.SelectedItem = "Countrylink";
            Settings.Category2Operator = trainOperator.Countrylink;
            Operator3Category.SelectedItem = "Group Remaining";
            Settings.Category3Operator = trainOperator.GroupRemaining;

            Commodity1Category.SelectedItem = null;
            Settings.Category1Commodity = trainCommodity.Unknown;
            Commodity2Category.SelectedItem = null;
            Settings.Category2Commodity = trainCommodity.Unknown;
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

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Port Kembla to Moss Vale data.txt";

            IceDataFile.Text = Path.GetFileName(FileSettings.dataFile);
            simICEDataFile.Text = Path.GetFileName(FileSettings.dataFile);

            IceDataFile.ForeColor = System.Drawing.Color.Black;
            simICEDataFile.ForeColor = System.Drawing.Color.Black;

            /* Geometry File */
            FileSettings.geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Port Kembla to Moss Vale Geometry.csv";
            GeometryFile.Text = Path.GetFileName(FileSettings.geometryFile);
            GeometryFile.ForeColor = System.Drawing.Color.Black;

            /* TSR File */
            FileSettings.temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Port Kembla to  Moss Vale TSR.csv";
            temporarySpeedRestrictionFile.Text = Path.GetFileName(FileSettings.temporarySpeedRestrictionFile);
            temporarySpeedRestrictionFile.ForeColor = System.Drawing.Color.Black;

            /* Simulation files */
            FileSettings.simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Coal-Increasing.csv";
            Category1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            Category1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Coal-Decreasing.csv";
            Category1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            Category1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Grain-Increasing.csv";
            Category2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            Category2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Grain-Decreasing.csv";
            Category2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            Category2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[4] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Minerals-Increasing.csv";
            Category3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            Category3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles[5] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale\Minerals-Decreasing.csv";
            Category3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            Category3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Port Kembla to Moss Vale";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2017, 6, 10);

            /* Interpolation parameters. */
            excludeListOfTrains.Checked = false;

            startInterpolationKm.Text = "80";
            endInterpolationKm.Text = "160";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "50";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            Category1LowerBound.Text = "0";
            Category1UpperBound.Text = "0";
            Category2LowerBound.Text = "0";
            Category2UpperBound.Text = "0";

            /* Anlaysis Parameters */
            powerToWeightRatioAnalysis.Checked = false;

            Settings.analysisCategory = analysisCategory.TrainOperator;

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
                setTarcoola2KalgoorlieParameters(sender, e);
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

            startInterpolationKm.Text = "0";
            endInterpolationKm.Text = "100";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "80";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
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
            Execute_Click(sender, e);

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
        private void runGunnedBasin(object sender, EventArgs e)
        {
            /* Start a timer */
            Stopwatch timer = new Stopwatch();
            timer.Start();

            /* Set the analysis parameters. */
            setGunnedahBasinParameters(sender, e);
            /* Simualte pressing execute button. */
            Execute_Click(sender, e);

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
            Execute_Click(sender, e);

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
            Execute_Click(sender, e);

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
            Execute_Click(sender, e);

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
            setTarcoola2KalgoorlieParameters(sender, e);
            /* Simualte pressing execute button. */
            Execute_Click(sender, e);

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
            Execute_Click(sender, e);

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
            Execute_Click(sender, e);

            timer.Stop();

            /* Display the run time. */
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Port Kembla RunTime " + elapsedTime);
        }



    }
}
