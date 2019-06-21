using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Globalsettings;
using System.Diagnostics;


/* Custom Libraries */
using TrainLibrary;
using IOLibrary;
using Statistics;

namespace TRAP
{

    class Algorithm
    {

        /// <summary>
        /// Determine the average train performance in both directions based on the supplied 
        /// actual train data. The form allows the user to specify which parameters will be 
        /// used to analyse the data. These can be the train operator, the power to weight 
        /// ratios, and the commodity the train carries.
        /// 
        /// The loop locations and TSR information is also used to extract the data that 
        /// corresponds to a train that enters a loop and is bound by a TSR. If the train 
        /// is within the 'loop bounday threshold' and is deemed to be stopping in the loop, 
        /// the data at this location is not included. The train is deemed to be stopping in 
        /// a loop if the train speed drops below the simulated speed multiplied by the 
        /// 'loop speed factor'. If the train is within the 'TSR window', the data at this 
        /// location is ignored as the train is bound by the TSR at the location. The average 
        /// train is then determined from the included train data.
        /// 
        /// For the purposes of the Hunter Valley region a Signal is treated as a loop. However, 
        /// each signal must be identified in the geometry file.
        /// 
        /// This function produces a file containing the interpolated data for each train 
        /// and a file containing the aggregated information for each analysis Category.
        /// 
        /// Authour:
        /// B. Bellamy
        /// 
        /// Version 1.1.0.0
        /// - Updated application to use the custom software libraries, which includes 
        /// changes to the interpolation implementation
        /// - The interpolation now allows the train journey's with small gaps in the data 
        /// to be retained. The interpolated values within these gaps are not used in any 
        /// aggregation. This allows for more trains to be counted towards the average 
        /// performance between the gaps.
        /// 
        /// Version 1.2.0.0
        /// - Added sample count and standard deviation of the average speed values, 
        /// which are written to the output file.
        /// 
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
            TSRs = FileOperations.readTSRFile(FileSettings.temporarySpeedRestrictionFile, Settings.dateRange);
            
            /* Read the data. */
            List<TrainRecord> TrainRecords = new List<TrainRecord>();

            TrainRecords = FileOperations.readAzureICEData(FileSettings.dataFile, excludeTrainList, Settings.excludeListOfTrains, Settings.dateRange);
            
            if (TrainRecords.Count() == 0)
            {
                //tool.messageBox("There are no records in the list to analyse.", "No trains available.");
                return new List<Train>();
            }

            TrainRecords = TrainRecords.Where(t => t.commodity != trainCommodity.Passenger).ToList();

            /* Identify the number of operators. */
            List<trainOperator> operators = TrainRecords.Select(t => t.trainOperator).Distinct().ToList();
            operators.Remove(trainOperator.Unknown);
            int numberOfOperators = operators.Count();

            List<string> locos = TrainRecords.Where(t => t.trainOperator == trainOperator.PacificNational).Select(t => t.locoID).Distinct().ToList();

            List<trainCommodity> Commodities = TrainRecords.Select(t => t.commodity).Distinct().ToList();
            int numberOfCommodities = Commodities.Count();

            int recordsNotPNorQR = TrainRecords.Where(t => t.trainOperator != trainOperator.PacificNational).Where(t => t.trainOperator != trainOperator.Aurizon).Count();

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
                    simCategories.Add(Processing.convertTrainOperatorToCategory(Settings.Category1Operator));

                if (Settings.Category2Operator != trainOperator.Unknown)
                    simCategories.Add(Processing.convertTrainOperatorToCategory(Settings.Category2Operator));

                if (Settings.Category3Operator != trainOperator.Unknown)
                    simCategories.Add(Processing.convertTrainOperatorToCategory(Settings.Category3Operator));

            }
            else if (Settings.analysisCategory == analysisCategory.TrainType)
            {

                if (Settings.Category1TrainType != trainType.Unknown)
                    simCategories.Add(Processing.convertTrainTypeToCategory(Settings.Category1TrainType));
                
                if (Settings.Category2TrainType != trainType.Unknown)
                    simCategories.Add(Processing.convertTrainTypeToCategory(Settings.Category2TrainType));

                if (Settings.Category3TrainType != trainType.Unknown)
                    simCategories.Add(Processing.convertTrainTypeToCategory(Settings.Category3TrainType));

            }
            else
            {
                /* analysisCategory is commodities. */
                if (Settings.Category1Commodity != trainCommodity.Unknown)
                    simCategories.Add(Processing.convertCommodityToCategory(Settings.Category1Commodity));

                if (Settings.Category2Commodity != trainCommodity.Unknown)
                    simCategories.Add(Processing.convertCommodityToCategory(Settings.Category2Commodity));

                if (Settings.Category3Commodity != trainCommodity.Unknown)
                    simCategories.Add(Processing.convertCommodityToCategory(Settings.Category3Commodity));

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
            interpolatedSimulations = Processing.interpolateTrainData(simulatedTrains, trackGeometry, Settings.startKm, Settings.endKm, Settings.interval);

            /* Sort the data by [trainID, locoID, Date & Time, kmPost]. */
            List<TrainRecord> OrderdTrainRecords = new List<TrainRecord>();
            OrderdTrainRecords = TrainRecords.OrderBy(t => t.trainID).ThenBy(t => t.locoID).ThenBy(t => t.dateTime).ThenBy(t => t.kmPost).ToList();

            /* Clean the data */
            List<Train> CleanTrainRecords = new List<Train>();
            CleanTrainRecords = Processing.CleanData(OrderdTrainRecords, trackGeometry,
                Settings.timeThreshold, Settings.distanceThreshold, Settings.minimumJourneyDistance, Settings.analysisCategory,
                Settings.Category1LowerBound, Settings.Category1UpperBound, Settings.Category2LowerBound, Settings.Category2UpperBound);

            List<Train> trainsNotPNorQR = CleanTrainRecords.Where(t => t.trainOperator != trainOperator.PacificNational).Where(t => t.trainOperator != trainOperator.Aurizon).ToList();

            /* Write the raw train data to file. */
            //FileOperations.writeRawTrainDataWithTime(CleanTrainRecords, FileSettings.aggregatedDestination);

            /* Place holders for train counts of each comodity. */
            //List<Train> steel = CleanTrainRecords.Where(t => t.commodity.Equals(trainCommodity.Steel)).ToList();
            //List<Train> grain = CleanTrainRecords.Where(t => t.commodity.Equals(trainCommodity.Grain)).ToList();
            //List<Train> mineral = CleanTrainRecords.Where(t => t.commodity.Equals(trainCommodity.Mineral)).ToList();
            //List<Train> clinker = CleanTrainRecords.Where(t => t.commodity.Equals(trainCommodity.Clinker)).ToList();
            //List<Train> passenger = CleanTrainRecords.Where(t => t.commodity.Equals(trainCommodity.Passenger)).ToList();
            //List<Train> general = CleanTrainRecords.Where(t => t.commodity.Equals(trainCommodity.GeneralFreight)).ToList(); //******
            //List<Train> coal = CleanTrainRecords.Where(t => t.commodity.Equals(trainCommodity.Coal)).ToList();
            //List<Train> work = CleanTrainRecords.Where(t => t.commodity.Equals(trainCommodity.Work)).ToList();

            /* Interpolate data */
            List<Train> interpolatedTrains = new List<Train>();
            if (!Settings.IgnoreGaps)
                /* Standard interpolation method */
                interpolatedTrains = Processing.interpolateTrainData(CleanTrainRecords, trackGeometry, Settings.startKm, Settings.endKm, Settings.interval);
            else
                /* Interpolation method does not interpolate through the gaps. (typically used with MakeTrains function) */
                interpolatedTrains = Processing.interpolateTrainDataWithGaps(CleanTrainRecords, trackGeometry, Settings.startKm, Settings.endKm, Settings.interval);
           
            /* Populate the trains TSR values after interpolation to gain more granularity with TSR boundary. */
            Processing.populateAllTrainsTemporarySpeedRestrictions(interpolatedTrains, TSRs);

            List<processTrainDataPoint> processedTrains = new List<processTrainDataPoint>();
            
            /* Write the interpolated data to file.
             * These trains still contain the affect of TSR's and loops.
             */
            FileOperations.writeTrainData(interpolatedTrains, Settings.startKm, Settings.interval, FileSettings.aggregatedDestination);
            
            /* Create the list of averaged trains */
            List<AverageTrain> averageTrains = new List<AverageTrain>();
            /* Create a sublist of trains for each direction. */
            List<Train> increasingTrainCategory = new List<Train>();
            List<Train> decreasingTrainCategory = new List<Train>();

            List<TrainStatistics> stats = new List<TrainStatistics>();

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
                    trainOperator operatorCategory = Processing.convertCategoryToTrainOperator(simCategories[index]);

                    /* Create a list for each operator. */
                    if (operatorCategory != trainOperator.GroupRemaining)
                    {
                        increasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == operatorCategory).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        decreasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == operatorCategory).Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                        /******************************************************************************************/
                        /* Hack to get seperate the PN train by loco in Gunnedah */

                        ///* PN - TT */
                        //if (operatorCategory == trainOperator.PacificNational)
                        //{
                        //    increasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == operatorCategory).
                        //        Where(t => t.locoID.StartsWith("TT")).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        //    decreasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == operatorCategory).
                        //        Where(t => t.locoID.StartsWith("TT")).Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                        //}
                        ///* PN - 90 */
                        //if (operatorCategory == trainOperator.Freightliner)
                        //{
                        //    increasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == trainOperator.PacificNational).
                        //        Where(t => t.locoID.StartsWith("9")).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        //    decreasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == trainOperator.PacificNational).
                        //        Where(t => t.locoID.StartsWith("9")).Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                        //}
                        ///* QR - all */
                        //if (operatorCategory == trainOperator.Aurizon)
                        //{
                        //    increasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == operatorCategory).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        //    decreasingTrainCategory = interpolatedTrains.Where(t => t.trainOperator == operatorCategory).Where(t => t.trainDirection == direction.DecreasingKm).ToList();

                        //}
                        /*********************************************************************************************/
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
                                operatorCategory = Processing.convertCategoryToTrainOperator(simCategories[groupIdx]);
                                increasingTrainCategory = increasingTrainCategory.Where(t => t.trainOperator != operatorCategory).ToList();
                                decreasingTrainCategory = decreasingTrainCategory.Where(t => t.trainOperator != operatorCategory).ToList();
                            }


                        }
                        /* Reset the operator to grouped for the analysis */
                        Processing.setOperatorToGrouped(increasingTrainCategory);
                        Processing.setOperatorToGrouped(decreasingTrainCategory);
                    }

                }
                else if (Settings.analysisCategory == analysisCategory.TrainType)
                {
                    /* Convert the train category to the train operator. */
                    trainType trainType = Processing.convertCategoryToTrainType(simCategories[index]);

                    /* Create a list for each operator. */
                    if (trainType != trainType.GroupRemaining)
                    { 
                        increasingTrainCategory = interpolatedTrains.Where(t => t.trainType == trainType).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        decreasingTrainCategory = interpolatedTrains.Where(t => t.trainType == trainType).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
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
                                /* Remove the specified train types from the list so they aren't counted twice. */
                                trainType = Processing.convertCategoryToTrainType(simCategories[groupIdx]);
                                increasingTrainCategory = increasingTrainCategory.Where(t => t.trainType != trainType).ToList();
                                decreasingTrainCategory = decreasingTrainCategory.Where(t => t.trainType != trainType).ToList();
                            }
                        }
                        /* Reset the operator to grouped for the analysis */
                        Processing.setOperatorToGrouped(increasingTrainCategory);
                        Processing.setOperatorToGrouped(decreasingTrainCategory);
                    }
                }
                else
                {
                    /* Convert the train category to the commodity. */
                    trainCommodity commodity = Processing.convertCategoryToCommodity(simCategories[index]);

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
                                commodity = Processing.convertCategoryToCommodity(simCategories[groupIdx]);
                                increasingTrainCategory = increasingTrainCategory.Where(t => t.commodity != commodity).ToList();
                                decreasingTrainCategory = decreasingTrainCategory.Where(t => t.commodity != commodity).ToList();
                            }
                        }
                        /* Reset the operator to grouped for the analysis */
                        Processing.setOperatorToGrouped(increasingTrainCategory);
                        Processing.setOperatorToGrouped(decreasingTrainCategory);
                    }

                }

                /* Generate statistics for the lists. */
                stats.Add(TrainStatistics.generateStats(increasingTrainCategory));
                stats.Add(TrainStatistics.generateStats(decreasingTrainCategory));

                if (increasingTrainCategory.Count() == 0 || decreasingTrainCategory.Count() == 0)
                {
                    stats[stats.Count() - 1].Category = simCategories[index].ToString() + " " + direction.DecreasingKm.ToString();
                    stats[stats.Count() - 2].Category = simCategories[index].ToString() + " " + direction.IncreasingKm.ToString();
                }

                /* Aggregate the train lists into an average train consistent with the specified Category. */
                if (increasingTrainCategory.Count() > 0)
                {
                    if (Settings.trainsStoppingAtLoops)
                        averageTrains.Add(Processing.averageTrainStoppingAtLoops(increasingTrainCategory, interpolatedSimulations[index * 2].journey, trackGeometry, Settings.startKm, Settings.endKm, Settings.interval, Settings.loopSpeedThreshold, Settings.loopBoundaryThreshold, Settings.TSRwindowBoundary));
                    else
                        averageTrains.Add(Processing.averageTrain(increasingTrainCategory, interpolatedSimulations[index * 2].journey, trackGeometry, Settings.startKm, Settings.endKm, Settings.interval, Settings.loopSpeedThreshold, Settings.loopBoundaryThreshold, Settings.TSRwindowBoundary));

                    processedTrains.AddRange(Processing.processTrainData(increasingTrainCategory, interpolatedSimulations[index * 2].journey, averageTrains[index * 2], Settings.TSRwindowBoundary, Settings.loopBoundaryThreshold));

                }
                else
                {
                    averageTrains.Add(Processing.createZeroedAverageTrain(simCategories[index], direction.IncreasingKm, Settings.startKm, Settings.endKm, Settings.interval));
                    processedTrains.AddRange(Processing.processTrainData(interpolatedSimulations[index * 2].journey, averageTrains[index * 2], direction.IncreasingKm));
                }

                if (decreasingTrainCategory.Count() > 0)
                {
                    if (Settings.trainsStoppingAtLoops)
                        averageTrains.Add(Processing.averageTrainStoppingAtLoops(decreasingTrainCategory, interpolatedSimulations[index * 2 + 1].journey, trackGeometry, Settings.startKm, Settings.endKm, Settings.interval, Settings.loopSpeedThreshold, Settings.loopBoundaryThreshold, Settings.TSRwindowBoundary));
                    else
                        averageTrains.Add(Processing.averageTrain(decreasingTrainCategory, interpolatedSimulations[index * 2 + 1].journey, trackGeometry, Settings.startKm, Settings.endKm, Settings.interval, Settings.loopSpeedThreshold, Settings.loopBoundaryThreshold, Settings.TSRwindowBoundary));

                    processedTrains.AddRange(Processing.processTrainData(decreasingTrainCategory, interpolatedSimulations[index * 2 + 1].journey, averageTrains[index * 2 + 1], Settings.TSRwindowBoundary, Settings.loopBoundaryThreshold));

                }
                else
                {
                    averageTrains.Add(Processing.createZeroedAverageTrain(simCategories[index], direction.DecreasingKm, Settings.startKm, Settings.endKm, Settings.interval));
                    processedTrains.AddRange(Processing.processTrainData(interpolatedSimulations[index * 2 + 1].journey, averageTrains[index * 2 + 1], direction.DecreasingKm));
                }
            
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

                Processing.setOperatorToCombined(increasingCombined);
                Processing.setOperatorToCombined(decreasingCombined);
                
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
                        increasingSubList = interpolatedTrains.Where(t => t.trainOperator == Processing.convertCategoryToTrainOperator(simCategory)).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        increasingCombined.AddRange(increasingSubList);
                        decreasingSubList = interpolatedTrains.Where(t => t.trainOperator == Processing.convertCategoryToTrainOperator(simCategory)).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                        decreasingCombined.AddRange(decreasingSubList);
                    }
                }
                Processing.setOperatorToCombined(increasingCombined);
                Processing.setOperatorToCombined(decreasingCombined);

            }
            else if (Settings.analysisCategory == analysisCategory.TrainType)
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
                        increasingSubList = interpolatedTrains.Where(t => t.trainType == Processing.convertCategoryToTrainType(simCategory)).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        increasingCombined.AddRange(increasingSubList);
                        decreasingSubList = interpolatedTrains.Where(t => t.trainType == Processing.convertCategoryToTrainType(simCategory)).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                        decreasingCombined.AddRange(decreasingSubList);
                    }
                }
                Processing.setOperatorToCombined(increasingCombined);
                Processing.setOperatorToCombined(decreasingCombined);

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
                        increasingSubList = interpolatedTrains.Where(t => t.commodity == Processing.convertCategoryToCommodity(simCategory)).Where(t => t.trainDirection == direction.IncreasingKm).ToList();
                        increasingCombined.AddRange(increasingSubList);
                        decreasingSubList = interpolatedTrains.Where(t => t.commodity == Processing.convertCategoryToCommodity(simCategory)).Where(t => t.trainDirection == direction.DecreasingKm).ToList();
                        decreasingCombined.AddRange(decreasingSubList);
                    }
                }
                Processing.setOperatorToCombined(increasingCombined);
                Processing.setOperatorToCombined(decreasingCombined);

            }

            /* Generate statistics for the weighted average trains. */
            stats.Add(TrainStatistics.generateStats(increasingCombined));
            stats.Add(TrainStatistics.generateStats(decreasingCombined));

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
                    averageTrains.Add(Processing.createZeroedAverageTrain(Category.Combined, direction.IncreasingKm, Settings.startKm, Settings.endKm, Settings.interval));
                else
                {
                    if (Settings.trainsStoppingAtLoops)
                        averageTrains.Add(Processing.averageTrainStoppingAtLoops(increasingCombined, weightedSimulation[0].journey, trackGeometry,
                            Settings.startKm, Settings.endKm, Settings.interval, Settings.loopSpeedThreshold, Settings.loopBoundaryThreshold, Settings.TSRwindowBoundary));
                    else
                        averageTrains.Add(Processing.averageTrain(increasingCombined, weightedSimulation[0].journey, trackGeometry,
                        Settings.startKm, Settings.endKm, Settings.interval, Settings.loopSpeedThreshold, Settings.loopBoundaryThreshold, Settings.TSRwindowBoundary));
                }
                if (decreasingCombined.Count() == 0)
                    averageTrains.Add(Processing.createZeroedAverageTrain(Category.Combined, direction.DecreasingKm, Settings.startKm, Settings.endKm, Settings.interval));
                else
                {
                    if (Settings.trainsStoppingAtLoops)
                        averageTrains.Add(Processing.averageTrainStoppingAtLoops(decreasingCombined, weightedSimulation[1].journey, trackGeometry,
                            Settings.startKm, Settings.endKm, Settings.interval, Settings.loopSpeedThreshold, Settings.loopBoundaryThreshold, Settings.TSRwindowBoundary));
                    else
                        averageTrains.Add(Processing.averageTrain(decreasingCombined, weightedSimulation[1].journey, trackGeometry,
                            Settings.startKm, Settings.endKm, Settings.interval, Settings.loopSpeedThreshold, Settings.loopBoundaryThreshold, Settings.TSRwindowBoundary));
                 }
                averageTrains.Add(weightedSimulation[0].ToAverageTrain());
                averageTrains.Add(weightedSimulation[1].ToAverageTrain());

                /* Generate statistics for the weighted average trains. */
                stats.Add(TrainStatistics.generateStats(weightedSimulation[0]));
                stats.Add(TrainStatistics.generateStats(weightedSimulation[1]));

            }
            else
            {
                string error = "There aren't enough weighted simulation files to proceed.";
                Console.WriteLine(error);

                throw new ArgumentException(error);
            }

            Dictionary<FieldInfo, object> settings = ReadStaticFields(typeof(Globalsettings.Settings), typeof(Globalsettings.FileSettings));


            /* Write the averaged Data to file for inspection. */
            FileOperations.writeAverageData(averageTrains, stats, FileSettings.aggregatedDestination, settings);
            //FileOperations.writeProcessTrainDataPoints(processedTrains, FileSettings.aggregatedDestination);

            return interpolatedTrains;
        }

        public static Dictionary<FieldInfo, object> ReadStaticFields(params Type[] types)
        {
            return types
                .SelectMany
                (
                    t => t.GetFields(BindingFlags.Public | BindingFlags.Static)
                )
                .ToDictionary(f => f, f => f.GetValue(null));
        }

    }

    
}
