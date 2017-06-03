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

namespace TRAP
{
    public partial class TrainPerformance : Form
    {
        
        public static Tools tool = new Tools();
        public static Processing processing = new Processing();
        public static TrackGeometry track = new TrackGeometry();

        public TrainPerformance()
        {
            InitializeComponent();
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
            if (Settings.includeAListOfTrainsToExclude)
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

            if (TrainRecords.Where(t => t.powerToWeight == 0).Count() == TrainRecords.Count())
            {
                catagory1IncreasingPowerToWeightRatio.Text = "0";
                catagory1DecreasingPowerToWeightRatio.Text = "0";

                catagory2IncreasingPowerToWeightRatio.Text = "0";
                catagory2DecreasingPowerToWeightRatio.Text = "0";

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
                catagory1IncreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.catagory1LowerBound, Settings.catagory1UpperBound, direction.IncreasingKm));
                catagory1DecreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.catagory1LowerBound, Settings.catagory1UpperBound, direction.DecreasingKm));

                catagory2IncreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.catagory2LowerBound, Settings.catagory2UpperBound, direction.IncreasingKm));
                catagory2DecreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.catagory2LowerBound, Settings.catagory2UpperBound, direction.DecreasingKm));

                combinedIncreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.catagory1LowerBound, Settings.catagory2UpperBound, direction.IncreasingKm));
                combinedDecreasingPowerToWeightRatio.Text = string.Format("{0:#.000}", averagePowerToWeightRatio(CleanTrainRecords, Settings.catagory1LowerBound, Settings.catagory2UpperBound, direction.DecreasingKm));


                /* Populate the counts for each train catagory. */
                catagory1IncreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.catagory1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.catagory1UpperBound).Count().ToString();
                catagory1DecreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.catagory1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.catagory1UpperBound).Count().ToString();
                catagory2IncreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.catagory2LowerBound).
                                                Where(t => t.powerToWeight <= Settings.catagory2UpperBound).Count().ToString();
                catagory2DecreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.catagory2LowerBound).
                                                Where(t => t.powerToWeight <= Settings.catagory2UpperBound).Count().ToString();

                combinedIncreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.IncreasingKm).
                                                Where(t => t.powerToWeight > Settings.catagory1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.catagory2UpperBound).Count().ToString();
                combinedDecreasingTrainCount.Text = CleanTrainRecords.Where(t => t.trainDirection == direction.DecreasingKm).
                                                Where(t => t.powerToWeight > Settings.catagory1LowerBound).
                                                Where(t => t.powerToWeight <= Settings.catagory2UpperBound).Count().ToString();

                /* Need to run the simulaions based on the average power to weight ratios before continueing with the analysis. */
                SimulationP2WRatioLabel.Text = "Run Simualtions based on these power to weight ratios";

            }

            
        }

        /// <summary>
        /// Select the simulation file for catagory 1 characteristics in the increasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCatagory1IncreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(catagory1IncreasingSimulationFile, 0);
            
        }

        /// <summary>
        /// Select the simulation file for catagory 1 characteristics in the decreasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCatagory1DecreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(catagory1DecreasingSimulationFile, 1);
        }

        /// <summary>
        /// Select the simulation file for catagory 2 characteristics in the increasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCatagory2IncreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(catagory2IncreasingSimulationFile, 2);
        }

        /// <summary>
        /// Select the simulation file for catagory 2 characteristics in the decreasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCatagory2DecreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(catagory2DecreasingSimulationFile, 3);
        }

        /// <summary>
        /// Select the simulation file for catagory 3 characteristics in the increasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCatagory3IncreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(catagory3IncreasingSimulationFile, 4);
        }

        /// <summary>
        /// Select the simulation file for catagory 3 characteristics in the decreasing km direction.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void selectCatagory3DecreasingSimulation_Click(object sender, EventArgs e)
        {
            /* Set the simulation file parameters and insert into the list at the correct index. */
            setSimulationFile(catagory3DecreasingSimulationFile, 5);
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
            /* Extract the simualtion catagory. */
            string catagory = getSimulationCatagory(index);
            
            /* Determine the direction of the simulation. */
            if ((index % 2) == 0)
                direction = "increasing";
            else
                direction = "decreasing";

            /* Create a meaningful string to help user identify the correct file. */
            string browseFile = "Select the " + catagory + " " + direction + " km simulation file.";

            /* Select the simulation file using the browser and insert into the simulation file list. */
            filename = tool.browseFile(browseFile);
            FileSettings.simulationFiles.Insert(index, filename);
            simulationFile.Text = Path.GetFileName(filename);
            simulationFile.ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Identify the simulation catagory based on the index in the list.
        /// </summary>
        /// <param name="index">Index of the simualtion catagory</param>
        /// <returns>A string identifying the simualtion catagory.</returns>
        private string getSimulationCatagory(int index)
        {
            /* This will need to be modified with the new features for operator and commodity. */

            /* Hunter Valley region requires the operators to be identified. */
            if (getHunterValleyRegion())
            {
                if (index < 2)
                    return "Pacific National";
                else if (index < 4)
                    return "Aurizon";
                else
                    return "Freightliner";
            }
            else
            {
                /* Standard interstate network catagories. */
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


            /* Populate the counts for each train catagory. */
            catagory1IncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                            Where(t => t.powerToWeight > Settings.catagory1LowerBound).
                                            Where(t => t.powerToWeight <= Settings.catagory1UpperBound).Count().ToString();
            catagory1DecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                            Where(t => t.powerToWeight > Settings.catagory1LowerBound).
                                            Where(t => t.powerToWeight <= Settings.catagory1UpperBound).Count().ToString();
            catagory2IncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                            Where(t => t.powerToWeight > Settings.catagory2LowerBound).
                                            Where(t => t.powerToWeight <= Settings.catagory2UpperBound).Count().ToString();
            catagory2DecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                            Where(t => t.powerToWeight > Settings.catagory2LowerBound).
                                            Where(t => t.powerToWeight <= Settings.catagory2UpperBound).Count().ToString();

            combinedIncreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.IncreasingKm).
                                            Where(t => t.powerToWeight > Settings.catagory1LowerBound).
                                            Where(t => t.powerToWeight <= Settings.catagory2UpperBound).Count().ToString();
            combinedDecreasingTrainCount.Text = trains.Where(t => t.trainDirection == direction.DecreasingKm).
                                            Where(t => t.powerToWeight > Settings.catagory1LowerBound).
                                            Where(t => t.powerToWeight <= Settings.catagory2UpperBound).Count().ToString();

            executionTime.Text = elapsedTime;

            tool.messageBox("Program Complete.");
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
        /// Extract the top left corner of the geographic box.
        /// </summary>
        /// <returns>A geographic location describing the top left corner of the box.</returns>
        public GeoLocation getTopLeftLocation()
        {
            double latitude, longitude;
            if (double.TryParse(fromLatitude.Text, out latitude) && (double.TryParse(fromLongitude.Text, out longitude)))
                return new GeoLocation(latitude, longitude);

            return new GeoLocation(0, 0);
        }

        /// <summary>
        /// Extract the bottom right corner of the geographic box.
        /// </summary>
        /// <returns>A geographic location describing the bottom right corner of the box.</returns>
        public GeoLocation getBottomRightLocation()
        {
            double latitude, longitude;
            if (double.TryParse(toLatitude.Text, out latitude) && (double.TryParse(toLongitude.Text, out longitude)))
                return new GeoLocation(latitude, longitude);

            return new GeoLocation(0, 0);
        }

        /// <summary>
        /// Extract the value of the includeAListOfTrainsToExclude flag.
        /// </summary>
        /// <returns>The value of the boolean flag.</returns>
        public bool getTrainListExcludeFlag() { return includeAListOfTrainsToExclude.Checked; }

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
        public double getCatagory1LowerBound()
        {
            double p2W;
            if (double.TryParse(catagory1LowerBound.Text, out p2W))
                return p2W;

            return 0;
        }

        /// <summary>
        /// Extract the upper bound value of the power to weight ratio for the underpowered trains.
        /// </summary>
        /// <returns>The upper bound power to weight ratio.</returns>
        public double getCatagory1UpperBound()
        {
            double p2W;
            if (double.TryParse(catagory1UpperBound.Text, out p2W))
                return p2W;

            return 0;
        }

        /// <summary>
        /// Extract the lower bound value of the power to weight ratio for the overpowered trains.
        /// </summary>
        /// <returns>The lower bound power to weight ratio.</returns>
        public double getCatagory2LowerBound()
        {
            double p2W;
            if (double.TryParse(catagory2LowerBound.Text, out p2W))
                return p2W;

            return 0;
        }

        /// <summary>
        /// Extract the upper bound value of the power to weight ratio for the overpowered trains.
        /// </summary>
        /// <returns>The upper bound power to weight ratio.</returns>
        public double getCatagory2UpperBound()
        {
            double p2W;
            if (double.TryParse(catagory2UpperBound.Text, out p2W))
                return p2W;

            return 0;
        }

        /// <summary>
        /// Extract the value of the getHunterValleyRegion flag.
        /// </summary>
        /// <returns>The value of the boolean flag.</returns>
        public bool getHunterValleyRegion() { return HunterValley.Checked; }

        /// <summary>
        /// Modify the simulation parameter labels based on the region under analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void HunterValley_CheckedChanged(object sender, EventArgs e)
        {
            if (HunterValley.Checked)
            {
                /* Set the train operators for the Hunter Valley region. */
                catagory1SimualtionLabel.Text = "Pacific National";
                catagory1Label.Text = "Pacific National:";
                simCatagory1Label.Text = "Pacific National:";

                catagory2SimualtionLabel.Text = "Aurizon";
                catagory2Label.Text = "Aurizon:";
                simCatagory2Label.Text = "Aurizon:";

                catagory3SimualtionLabel.Text = "Freightliner";

            }
            else
            {
                /* Set the simulation catagories for the interstate network. */
                catagory1SimualtionLabel.Text = "Underpowered";
                catagory1Label.Text = "Underpowered:";
                simCatagory1Label.Text = "Underpowered:";

                catagory2SimualtionLabel.Text = "Overpowered";
                catagory2Label.Text = "Overpowered:";
                simCatagory2Label.Text = "Overpowered:";

                catagory3SimualtionLabel.Text = "Alternative";
            }
        }

        /// <summary>
        /// This function sets all the testing parameters for the Cullerin Ranges data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setCulleranRangesParameters(object sender, EventArgs e)
        {
            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Cullerin Ranges test data.csv";

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
            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Increasing 3.31_ThuW1.csv");
            catagory1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            catagory1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Decreasing 3.33_TueW1.csv");
            catagory1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            catagory1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Increasing 4.8_FriW1.csv");
            catagory2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            catagory2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges\Decreasing 4.68_WedW1.csv");
            catagory2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            catagory2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //catagory3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //catagory3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Cullerin Ranges";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,4,1);

            fromLatitude.Text = "-10";
            toLatitude.Text = "-40";
            fromLongitude.Text = "110";
            toLongitude.Text = "152";

            includeAListOfTrainsToExclude.Checked = false;

            startInterpolationKm.Text = "220";
            endInterpolationKm.Text = "320";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "80";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            catagory1LowerBound.Text = "2";
            catagory1UpperBound.Text = "4";
            catagory2LowerBound.Text = "4";
            catagory2UpperBound.Text = "6";

            HunterValley.Checked = false;   // not required with new features.
            /* New features */
            powerToWeightRatioAnalysis.Checked = true;

            Operator1Catagory.Text = "Underpowered";
            Operator2Catagory.Text = "Overpowered";

            //Operator1Catagory.SelectedItem = Operator1Catagory.Items.IndexOf("Underpowered");
            //Operator2Catagory.SelectedItem = Operator1Catagory.Items.IndexOf("Overpowered");
            Operator3Catagory.SelectedItem = null;

            Commodity1Catagory.SelectedItem = null;
            Commodity2Catagory.SelectedItem = null;
            Commodity3Catagory.SelectedItem = null;


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
            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Increasing.csv");
            catagory1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            catagory1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Decreasing.csv");
            catagory1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            catagory1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Increasing-60.csv");
            catagory2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            catagory2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Decreasing.csv");
            catagory2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            catagory2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //catagory3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //catagory3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017,5,1);

            fromLatitude.Text = "-10";
            toLatitude.Text = "-40";
            fromLongitude.Text = "110";
            toLongitude.Text = "152";

            includeAListOfTrainsToExclude.Checked = false;

            startInterpolationKm.Text = "264";
            endInterpolationKm.Text = "541";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "250";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            catagory1LowerBound.Text = "0";
            catagory1UpperBound.Text = "100";
            catagory2LowerBound.Text = "100";
            catagory2UpperBound.Text = "200";

            HunterValley.Checked = true;

        }

        /// <summary>
        /// This function sets all the testing parameters for the Macarthur to Botany data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setMacartur2BotanyParameters(object sender, EventArgs e)
        {

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany test data.csv";

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
            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Botany to Macarthur - increasing - 3.33_ThuW1.csv");
            catagory1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            catagory1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany - decreasing - 3.20_SatW1.csv");
            catagory1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            catagory1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Botany to Macarthur - increasing - 7.87_ThuW1.csv");
            catagory2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            catagory2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany - decreasing - 6.97_SatW1.csv");
            catagory2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            catagory2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //catagory3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //catagory3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,2,1);

            fromLatitude.Text = "-10";
            toLatitude.Text = "-40";
            fromLongitude.Text = "110";
            toLongitude.Text = "152";

            includeAListOfTrainsToExclude.Checked = false;

            startInterpolationKm.Text = "5";
            endInterpolationKm.Text = "70";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "40";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            catagory1LowerBound.Text = "1.5";
            catagory1UpperBound.Text = "4.5";
            catagory2LowerBound.Text = "4.5";
            catagory2UpperBound.Text = "11.5";

            HunterValley.Checked = false;

        }

        /// <summary>
        /// This function sets all the testing parameters for the Melbourne to Cootamundra data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setMelbourne2CootamundraParameters(object sender, EventArgs e)
        {

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Melbourne to Cootamundra test data.csv";

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
            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Increasing sim 3.5.csv");
            catagory1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            catagory1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\decreasing sim 3.5.csv");
            catagory1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            catagory1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\Increasing sim 4.6.csv");
            catagory2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            catagory2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra\decreasing sim 4.6.csv");
            catagory2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            catagory2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //catagory3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //catagory3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Melbourne to Cootamundra";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016,4,1);

            fromLatitude.Text = "-10";
            toLatitude.Text = "-40";
            fromLongitude.Text = "110";
            toLongitude.Text = "152";

            includeAListOfTrainsToExclude.Checked = false;

            startInterpolationKm.Text = "5";
            endInterpolationKm.Text = "505";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "400";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            catagory1LowerBound.Text = "2.5";
            catagory1UpperBound.Text = "4";
            catagory2LowerBound.Text = "4";
            catagory2UpperBound.Text = "5.5";

            HunterValley.Checked = false;

        }

        /// <summary>
        /// This function sets all the testing parameters for the Tarcoola To Kalgoorlie data
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void setTarcoola2KalgoorlieParameters(object sender, EventArgs e)
        {

            /* Data File */
            FileSettings.dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\Tarcoola to Kalgoorlie test data.csv";

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
            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\increasing 2.2_ThuW1.csv");
            catagory1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            catagory1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\decreasing 2.6_SunW1.csv");
            catagory1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            catagory1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\increasing 3.4_FriW1.csv");
            catagory2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            catagory2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie\decreasing 3.5_MonW1.csv");
            catagory2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            catagory2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            //catagory3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            //FileSettings.simulationFiles.Add();
            //catagory3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            //catagory3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Tarcoola to Kalgoorlie";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016, 2, 1);

            fromLatitude.Text = "-10";
            toLatitude.Text = "-40";
            fromLongitude.Text = "110";
            toLongitude.Text = "152";

            includeAListOfTrainsToExclude.Checked = false;

            startInterpolationKm.Text = "950";
            endInterpolationKm.Text = "1400";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "350";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            catagory1LowerBound.Text = "1.5";
            catagory1UpperBound.Text = "3";
            catagory2LowerBound.Text = "3";
            catagory2UpperBound.Text = "4";

            HunterValley.Checked = false;

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
            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Pacific National - Increasing.csv");
            catagory1IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[0]);
            catagory1IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Pacific National - Decreasing.csv");
            catagory1DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[1]);
            catagory1DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Aurizon - Increasing.csv");
            catagory2IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[2]);
            catagory2IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Aurizon - Decreasing.csv");
            catagory2DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[3]);
            catagory2DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Freightliner - Increasing.csv");
            catagory3IncreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[4]);
            catagory3IncreasingSimulationFile.ForeColor = System.Drawing.Color.Black;

            FileSettings.simulationFiles.Add(@"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan\Freightliner - Decreasing.csv");
            catagory3DecreasingSimulationFile.Text = Path.GetFileName(FileSettings.simulationFiles[5]);
            catagory3DecreasingSimulationFile.ForeColor = System.Drawing.Color.Black;


            /* Destination Folder */
            FileSettings.aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan";
            resultsDestination.Text = FileSettings.aggregatedDestination;
            resultsDestination.ForeColor = System.Drawing.Color.Black;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 1);
            toDate.Value = new DateTime(2017,6,1);

            fromLatitude.Text = "-10";
            toLatitude.Text = "-40";
            fromLongitude.Text = "110";
            toLongitude.Text = "152";

            includeAListOfTrainsToExclude.Checked = false;

            startInterpolationKm.Text = "280";
            endInterpolationKm.Text = "460";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "100";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            catagory1LowerBound.Text = "0";
            catagory1UpperBound.Text = "100";
            catagory2LowerBound.Text = "100";
            catagory2UpperBound.Text = "200";

            HunterValley.Checked = true;

        }

        /// <summary>
        /// Function determines if the testing parameters for Culleran Ranges need 
        /// to be set or resets to default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void CulleranRanges_CheckedChanged(object sender, EventArgs e)
        {
            /* If Cullerin Ranges tesging flag is checked, set the appropriate parameters. */
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
            FileSettings.simulationFiles.Clear();
            catagory1IncreasingSimulationFile.Text = "<Required>";
            catagory1IncreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            catagory1DecreasingSimulationFile.Text = "<Required>";
            catagory1DecreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            catagory2IncreasingSimulationFile.Text = "<Required>";
            catagory2IncreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            catagory2DecreasingSimulationFile.Text = "<Required>";
            catagory2DecreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            catagory3IncreasingSimulationFile.Text = "<Optional>";
            catagory3IncreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;

            catagory3DecreasingSimulationFile.Text = "<Optional>";
            catagory3DecreasingSimulationFile.ForeColor = SystemColors.InactiveCaptionText;



            /* Destination Folder */
            FileSettings.aggregatedDestination = null;
            resultsDestination.Text = "<Required>";
            resultsDestination.ForeColor = SystemColors.InactiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016, 2, 1);

            /* Geographic box for Australia */
            fromLatitude.Text = "-10";
            toLatitude.Text = "-40";
            fromLongitude.Text = "110";
            toLongitude.Text = "152";

            includeAListOfTrainsToExclude.Checked = false;

            startInterpolationKm.Text = "0";
            endInterpolationKm.Text = "100";
            interpolationInterval.Text = "50";
            minimumJourneyDistance.Text = "80";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            loopBoundaryThreshold.Text = "1";
            loopSpeedThreshold.Text = "50";
            TSRWindowBoundary.Text = "1";

            catagory1LowerBound.Text = "0";
            catagory1UpperBound.Text = "0";
            catagory2LowerBound.Text = "0";
            catagory2UpperBound.Text = "0";

            HunterValley.Checked = false;


        }

        private void Operator1Catagory_SelectedValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine(Operator1Catagory.Name);
        }



        
    }
}
