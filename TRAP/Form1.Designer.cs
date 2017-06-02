namespace TRAP
{
    partial class TrainPerformance
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.simulationTab = new System.Windows.Forms.TabControl();
            this.fileSelectionTab = new System.Windows.Forms.TabPage();
            this.UlanLine = new System.Windows.Forms.CheckBox();
            this.Tarcoola2Kalgoorlie = new System.Windows.Forms.CheckBox();
            this.Melbourne2Cootamundra = new System.Windows.Forms.CheckBox();
            this.Macarthur2Botany = new System.Windows.Forms.CheckBox();
            this.GunnedahBasin = new System.Windows.Forms.CheckBox();
            this.CulleranRanges = new System.Windows.Forms.CheckBox();
            this.TestLabel = new System.Windows.Forms.Label();
            this.includeAListOfTrainsToExclude = new System.Windows.Forms.CheckBox();
            this.HunterValley = new System.Windows.Forms.CheckBox();
            this.trainListFile = new System.Windows.Forms.TextBox();
            this.selectTrainFile = new System.Windows.Forms.Button();
            this.temporarySpeedRestrictionFile = new System.Windows.Forms.TextBox();
            this.selectTSRFile = new System.Windows.Forms.Button();
            this.GeometryFile = new System.Windows.Forms.TextBox();
            this.selectGeometryFile = new System.Windows.Forms.Button();
            this.IceDataFile = new System.Windows.Forms.TextBox();
            this.selectDataFile = new System.Windows.Forms.Button();
            this.processingTab = new System.Windows.Forms.TabPage();
            this.toDate = new System.Windows.Forms.DateTimePicker();
            this.fromDate = new System.Windows.Forms.DateTimePicker();
            this.timeSeparation = new System.Windows.Forms.TextBox();
            this.TSRWindowBoundary = new System.Windows.Forms.TextBox();
            this.dataSeparation = new System.Windows.Forms.TextBox();
            this.loopSpeedThreshold = new System.Windows.Forms.TextBox();
            this.minimumJourneyDistance = new System.Windows.Forms.TextBox();
            this.loopBoundaryThreshold = new System.Windows.Forms.TextBox();
            this.interpolationInterval = new System.Windows.Forms.TextBox();
            this.endInterpolationKm = new System.Windows.Forms.TextBox();
            this.startInterpolationKm = new System.Windows.Forms.TextBox();
            this.toLongitude = new System.Windows.Forms.TextBox();
            this.toLatitude = new System.Windows.Forms.TextBox();
            this.fromLongitude = new System.Windows.Forms.TextBox();
            this.fromLatitude = new System.Windows.Forms.TextBox();
            this.leftLongitudeLabel = new System.Windows.Forms.Label();
            this.leftLatitudeLabel = new System.Windows.Forms.Label();
            this.toLabel = new System.Windows.Forms.Label();
            this.fromLabel = new System.Windows.Forms.Label();
            this.loopSpeedLabel = new System.Windows.Forms.Label();
            this.TSRLabel = new System.Windows.Forms.Label();
            this.leftLocationLabel = new System.Windows.Forms.Label();
            this.rightLocationLabel = new System.Windows.Forms.Label();
            this.startKmLabel = new System.Windows.Forms.Label();
            this.minDistanceLabel = new System.Windows.Forms.Label();
            this.dataSeparationLabel = new System.Windows.Forms.Label();
            this.timeSeparationLabel = new System.Windows.Forms.Label();
            this.endKmLabel = new System.Windows.Forms.Label();
            this.label3loopBoundarylabel = new System.Windows.Forms.Label();
            this.interpolationLabel = new System.Windows.Forms.Label();
            this.GeographicBoxLabel = new System.Windows.Forms.Label();
            this.DateRangeLabel = new System.Windows.Forms.Label();
            this.simulationParametersTab = new System.Windows.Forms.TabPage();
            this.SimulationP2WRatioLabel = new System.Windows.Forms.Label();
            this.combinedDecreasingTrainCount = new System.Windows.Forms.Label();
            this.catagory2DecreasingTrainCount = new System.Windows.Forms.Label();
            this.catagory1DecreasingTrainCount = new System.Windows.Forms.Label();
            this.combinedDecreasingPowerToWeightRatio = new System.Windows.Forms.Label();
            this.catagory2DecreasingPowerToWeightRatio = new System.Windows.Forms.Label();
            this.catagory1DecreasingPowerToWeightRatio = new System.Windows.Forms.Label();
            this.combinedIncreasingTrainCount = new System.Windows.Forms.Label();
            this.catagory2IncreasingTrainCount = new System.Windows.Forms.Label();
            this.catagory1IncreasingTrainCount = new System.Windows.Forms.Label();
            this.combinedIncreasingPowerToWeightRatio = new System.Windows.Forms.Label();
            this.catagory2IncreasingPowerToWeightRatio = new System.Windows.Forms.Label();
            this.catagory1IncreasingPowerToWeightRatio = new System.Windows.Forms.Label();
            this.countLabel2 = new System.Windows.Forms.Label();
            this.PWRatioLabel2 = new System.Windows.Forms.Label();
            this.countLabel1 = new System.Windows.Forms.Label();
            this.PWRatioLabel1 = new System.Windows.Forms.Label();
            this.simulationPowerToWeightRatios = new System.Windows.Forms.Button();
            this.catagory2UpperBound = new System.Windows.Forms.TextBox();
            this.catagory2LowerBound = new System.Windows.Forms.TextBox();
            this.catagory1UpperBound = new System.Windows.Forms.TextBox();
            this.catagory1LowerBound = new System.Windows.Forms.TextBox();
            this.powerToWeightLabel = new System.Windows.Forms.Label();
            this.simICEDataFile = new System.Windows.Forms.TextBox();
            this.combinedLabel = new System.Windows.Forms.Label();
            this.simCatagory2Label = new System.Windows.Forms.Label();
            this.simCatagory1Label = new System.Windows.Forms.Label();
            this.decreasingLabel = new System.Windows.Forms.Label();
            this.increasingLabel = new System.Windows.Forms.Label();
            this.upperBoundLabel = new System.Windows.Forms.Label();
            this.lowerBoundLabel = new System.Windows.Forms.Label();
            this.catagory2Label = new System.Windows.Forms.Label();
            this.catagory1Label = new System.Windows.Forms.Label();
            this.dataFileLabel = new System.Windows.Forms.Label();
            this.simualtionFileTab = new System.Windows.Forms.TabPage();
            this.Execute = new System.Windows.Forms.Button();
            this.executionTime = new System.Windows.Forms.Label();
            this.ExecitionTimeLabel = new System.Windows.Forms.Label();
            this.resultsDestination = new System.Windows.Forms.TextBox();
            this.ResultsDirectoryLabel = new System.Windows.Forms.Label();
            this.resultsDirectory = new System.Windows.Forms.Button();
            this.selectCatagory3DecreasingSimulation = new System.Windows.Forms.Button();
            this.catagory3DecreasingSimulationFile = new System.Windows.Forms.TextBox();
            this.selectCatagory3IncreasingSimulation = new System.Windows.Forms.Button();
            this.catagory3SimualtionLabel = new System.Windows.Forms.Label();
            this.catagory3IncreasingSimulationFile = new System.Windows.Forms.TextBox();
            this.selectCatagory2DecreasingSimulation = new System.Windows.Forms.Button();
            this.catagory2DecreasingSimulationFile = new System.Windows.Forms.TextBox();
            this.selectCatagory2IncreasingSimulation = new System.Windows.Forms.Button();
            this.catagory2SimualtionLabel = new System.Windows.Forms.Label();
            this.catagory2IncreasingSimulationFile = new System.Windows.Forms.TextBox();
            this.selectCatagory1DecreasingSimulation = new System.Windows.Forms.Button();
            this.catagory1DecreasingSimulationFile = new System.Windows.Forms.TextBox();
            this.selectCatagory1IncreasingSimulation = new System.Windows.Forms.Button();
            this.catagory1SimualtionLabel = new System.Windows.Forms.Label();
            this.catagory1IncreasingSimulationFile = new System.Windows.Forms.TextBox();
            this.OperatorLabel = new System.Windows.Forms.Label();
            this.CommodityLabel = new System.Windows.Forms.Label();
            this.Operator1Catagory = new System.Windows.Forms.ComboBox();
            this.Commodity1Catagory = new System.Windows.Forms.ComboBox();
            this.Commodity2Catagory = new System.Windows.Forms.ComboBox();
            this.Operator2Catagory = new System.Windows.Forms.ComboBox();
            this.Commodity3Catagory = new System.Windows.Forms.ComboBox();
            this.Operator3Catagory = new System.Windows.Forms.ComboBox();
            this.simulationTab.SuspendLayout();
            this.fileSelectionTab.SuspendLayout();
            this.processingTab.SuspendLayout();
            this.simulationParametersTab.SuspendLayout();
            this.simualtionFileTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // simulationTab
            // 
            this.simulationTab.Controls.Add(this.fileSelectionTab);
            this.simulationTab.Controls.Add(this.processingTab);
            this.simulationTab.Controls.Add(this.simulationParametersTab);
            this.simulationTab.Controls.Add(this.simualtionFileTab);
            this.simulationTab.Location = new System.Drawing.Point(4, 18);
            this.simulationTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.simulationTab.Name = "simulationTab";
            this.simulationTab.SelectedIndex = 0;
            this.simulationTab.Size = new System.Drawing.Size(1626, 794);
            this.simulationTab.TabIndex = 0;
            // 
            // fileSelectionTab
            // 
            this.fileSelectionTab.Controls.Add(this.UlanLine);
            this.fileSelectionTab.Controls.Add(this.Tarcoola2Kalgoorlie);
            this.fileSelectionTab.Controls.Add(this.Melbourne2Cootamundra);
            this.fileSelectionTab.Controls.Add(this.Macarthur2Botany);
            this.fileSelectionTab.Controls.Add(this.GunnedahBasin);
            this.fileSelectionTab.Controls.Add(this.CulleranRanges);
            this.fileSelectionTab.Controls.Add(this.TestLabel);
            this.fileSelectionTab.Controls.Add(this.includeAListOfTrainsToExclude);
            this.fileSelectionTab.Controls.Add(this.HunterValley);
            this.fileSelectionTab.Controls.Add(this.trainListFile);
            this.fileSelectionTab.Controls.Add(this.selectTrainFile);
            this.fileSelectionTab.Controls.Add(this.temporarySpeedRestrictionFile);
            this.fileSelectionTab.Controls.Add(this.selectTSRFile);
            this.fileSelectionTab.Controls.Add(this.GeometryFile);
            this.fileSelectionTab.Controls.Add(this.selectGeometryFile);
            this.fileSelectionTab.Controls.Add(this.IceDataFile);
            this.fileSelectionTab.Controls.Add(this.selectDataFile);
            this.fileSelectionTab.Location = new System.Drawing.Point(4, 29);
            this.fileSelectionTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fileSelectionTab.Name = "fileSelectionTab";
            this.fileSelectionTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fileSelectionTab.Size = new System.Drawing.Size(1618, 761);
            this.fileSelectionTab.TabIndex = 0;
            this.fileSelectionTab.Text = "File Selection";
            this.fileSelectionTab.UseVisualStyleBackColor = true;
            // 
            // UlanLine
            // 
            this.UlanLine.AutoSize = true;
            this.UlanLine.Location = new System.Drawing.Point(1297, 362);
            this.UlanLine.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UlanLine.Name = "UlanLine";
            this.UlanLine.Size = new System.Drawing.Size(102, 24);
            this.UlanLine.TabIndex = 16;
            this.UlanLine.Text = "Ulan Line";
            this.UlanLine.UseVisualStyleBackColor = true;
            this.UlanLine.CheckedChanged += new System.EventHandler(this.UlanLine_CheckedChanged);
            // 
            // Tarcoola2Kalgoorlie
            // 
            this.Tarcoola2Kalgoorlie.AutoSize = true;
            this.Tarcoola2Kalgoorlie.Location = new System.Drawing.Point(1297, 326);
            this.Tarcoola2Kalgoorlie.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Tarcoola2Kalgoorlie.Name = "Tarcoola2Kalgoorlie";
            this.Tarcoola2Kalgoorlie.Size = new System.Drawing.Size(187, 24);
            this.Tarcoola2Kalgoorlie.TabIndex = 15;
            this.Tarcoola2Kalgoorlie.Text = "Tarcoola to Kalgoorlie";
            this.Tarcoola2Kalgoorlie.UseVisualStyleBackColor = true;
            this.Tarcoola2Kalgoorlie.CheckedChanged += new System.EventHandler(this.Tarcoola2Kalgoorlie_CheckedChanged);
            // 
            // Melbourne2Cootamundra
            // 
            this.Melbourne2Cootamundra.AutoSize = true;
            this.Melbourne2Cootamundra.Location = new System.Drawing.Point(1297, 291);
            this.Melbourne2Cootamundra.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Melbourne2Cootamundra.Name = "Melbourne2Cootamundra";
            this.Melbourne2Cootamundra.Size = new System.Drawing.Size(229, 24);
            this.Melbourne2Cootamundra.TabIndex = 14;
            this.Melbourne2Cootamundra.Text = "Melbourne to Cootamundra";
            this.Melbourne2Cootamundra.UseVisualStyleBackColor = true;
            this.Melbourne2Cootamundra.CheckedChanged += new System.EventHandler(this.Melbourne2Cootamundra_CheckedChanged);
            // 
            // Macarthur2Botany
            // 
            this.Macarthur2Botany.AutoSize = true;
            this.Macarthur2Botany.Location = new System.Drawing.Point(1297, 256);
            this.Macarthur2Botany.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Macarthur2Botany.Name = "Macarthur2Botany";
            this.Macarthur2Botany.Size = new System.Drawing.Size(179, 24);
            this.Macarthur2Botany.TabIndex = 13;
            this.Macarthur2Botany.Text = "Macarthur to Botany";
            this.Macarthur2Botany.UseVisualStyleBackColor = true;
            this.Macarthur2Botany.CheckedChanged += new System.EventHandler(this.Macarthur2Botany_CheckedChanged);
            // 
            // GunnedahBasin
            // 
            this.GunnedahBasin.AutoSize = true;
            this.GunnedahBasin.Location = new System.Drawing.Point(1297, 220);
            this.GunnedahBasin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GunnedahBasin.Name = "GunnedahBasin";
            this.GunnedahBasin.Size = new System.Drawing.Size(155, 24);
            this.GunnedahBasin.TabIndex = 12;
            this.GunnedahBasin.Text = "Gunnedah Basin";
            this.GunnedahBasin.UseVisualStyleBackColor = true;
            this.GunnedahBasin.CheckedChanged += new System.EventHandler(this.GunnedahBasin_CheckedChanged);
            // 
            // CulleranRanges
            // 
            this.CulleranRanges.AutoSize = true;
            this.CulleranRanges.Location = new System.Drawing.Point(1297, 185);
            this.CulleranRanges.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CulleranRanges.Name = "CulleranRanges";
            this.CulleranRanges.Size = new System.Drawing.Size(153, 24);
            this.CulleranRanges.TabIndex = 11;
            this.CulleranRanges.Text = "Culleran Ranges";
            this.CulleranRanges.UseVisualStyleBackColor = true;
            this.CulleranRanges.CheckedChanged += new System.EventHandler(this.CulleranRanges_CheckedChanged);
            // 
            // TestLabel
            // 
            this.TestLabel.AutoSize = true;
            this.TestLabel.Location = new System.Drawing.Point(1293, 148);
            this.TestLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TestLabel.Name = "TestLabel";
            this.TestLabel.Size = new System.Drawing.Size(176, 20);
            this.TestLabel.TabIndex = 10;
            this.TestLabel.Text = "Set Testing Parameters";
            // 
            // includeAListOfTrainsToExclude
            // 
            this.includeAListOfTrainsToExclude.AutoSize = true;
            this.includeAListOfTrainsToExclude.Location = new System.Drawing.Point(26, 437);
            this.includeAListOfTrainsToExclude.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.includeAListOfTrainsToExclude.Name = "includeAListOfTrainsToExclude";
            this.includeAListOfTrainsToExclude.Size = new System.Drawing.Size(173, 24);
            this.includeAListOfTrainsToExclude.TabIndex = 9;
            this.includeAListOfTrainsToExclude.Text = "Exclude trains in list";
            this.includeAListOfTrainsToExclude.UseVisualStyleBackColor = true;
            // 
            // HunterValley
            // 
            this.HunterValley.AutoSize = true;
            this.HunterValley.Location = new System.Drawing.Point(26, 99);
            this.HunterValley.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.HunterValley.Name = "HunterValley";
            this.HunterValley.Size = new System.Drawing.Size(185, 24);
            this.HunterValley.TabIndex = 8;
            this.HunterValley.Text = "Hunter Valley Region";
            this.HunterValley.UseVisualStyleBackColor = true;
            this.HunterValley.CheckedChanged += new System.EventHandler(this.HunterValley_CheckedChanged);
            // 
            // trainListFile
            // 
            this.trainListFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.trainListFile.Location = new System.Drawing.Point(267, 389);
            this.trainListFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trainListFile.Name = "trainListFile";
            this.trainListFile.Size = new System.Drawing.Size(858, 26);
            this.trainListFile.TabIndex = 7;
            this.trainListFile.Text = "<Optional>";
            // 
            // selectTrainFile
            // 
            this.selectTrainFile.Location = new System.Drawing.Point(26, 378);
            this.selectTrainFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectTrainFile.Name = "selectTrainFile";
            this.selectTrainFile.Size = new System.Drawing.Size(232, 49);
            this.selectTrainFile.TabIndex = 6;
            this.selectTrainFile.Text = "Select Train List File";
            this.selectTrainFile.UseVisualStyleBackColor = true;
            this.selectTrainFile.Click += new System.EventHandler(this.selectTrainFile_Click);
            // 
            // temporarySpeedRestrictionFile
            // 
            this.temporarySpeedRestrictionFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.temporarySpeedRestrictionFile.Location = new System.Drawing.Point(267, 297);
            this.temporarySpeedRestrictionFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.temporarySpeedRestrictionFile.Name = "temporarySpeedRestrictionFile";
            this.temporarySpeedRestrictionFile.Size = new System.Drawing.Size(858, 26);
            this.temporarySpeedRestrictionFile.TabIndex = 5;
            this.temporarySpeedRestrictionFile.Text = "<Required>";
            // 
            // selectTSRFile
            // 
            this.selectTSRFile.Location = new System.Drawing.Point(26, 286);
            this.selectTSRFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectTSRFile.Name = "selectTSRFile";
            this.selectTSRFile.Size = new System.Drawing.Size(232, 49);
            this.selectTSRFile.TabIndex = 4;
            this.selectTSRFile.Text = "Select TSR File";
            this.selectTSRFile.UseVisualStyleBackColor = true;
            this.selectTSRFile.Click += new System.EventHandler(this.selectTSRFile_Click);
            // 
            // GeometryFile
            // 
            this.GeometryFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.GeometryFile.Location = new System.Drawing.Point(267, 220);
            this.GeometryFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GeometryFile.Name = "GeometryFile";
            this.GeometryFile.Size = new System.Drawing.Size(858, 26);
            this.GeometryFile.TabIndex = 3;
            this.GeometryFile.Text = "<Required>";
            // 
            // selectGeometryFile
            // 
            this.selectGeometryFile.Location = new System.Drawing.Point(26, 209);
            this.selectGeometryFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectGeometryFile.Name = "selectGeometryFile";
            this.selectGeometryFile.Size = new System.Drawing.Size(232, 49);
            this.selectGeometryFile.TabIndex = 2;
            this.selectGeometryFile.Text = "Select Geometry File";
            this.selectGeometryFile.UseVisualStyleBackColor = true;
            this.selectGeometryFile.Click += new System.EventHandler(this.selectGeometryFile_Click);
            // 
            // IceDataFile
            // 
            this.IceDataFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.IceDataFile.Location = new System.Drawing.Point(267, 51);
            this.IceDataFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.IceDataFile.Name = "IceDataFile";
            this.IceDataFile.Size = new System.Drawing.Size(858, 26);
            this.IceDataFile.TabIndex = 1;
            this.IceDataFile.Text = "<Required>";
            // 
            // selectDataFile
            // 
            this.selectDataFile.Location = new System.Drawing.Point(26, 40);
            this.selectDataFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectDataFile.Name = "selectDataFile";
            this.selectDataFile.Size = new System.Drawing.Size(232, 49);
            this.selectDataFile.TabIndex = 0;
            this.selectDataFile.Text = "Select Data File";
            this.selectDataFile.UseVisualStyleBackColor = true;
            this.selectDataFile.Click += new System.EventHandler(this.selectDataFile_Click);
            // 
            // processingTab
            // 
            this.processingTab.Controls.Add(this.toDate);
            this.processingTab.Controls.Add(this.fromDate);
            this.processingTab.Controls.Add(this.timeSeparation);
            this.processingTab.Controls.Add(this.TSRWindowBoundary);
            this.processingTab.Controls.Add(this.dataSeparation);
            this.processingTab.Controls.Add(this.loopSpeedThreshold);
            this.processingTab.Controls.Add(this.minimumJourneyDistance);
            this.processingTab.Controls.Add(this.loopBoundaryThreshold);
            this.processingTab.Controls.Add(this.interpolationInterval);
            this.processingTab.Controls.Add(this.endInterpolationKm);
            this.processingTab.Controls.Add(this.startInterpolationKm);
            this.processingTab.Controls.Add(this.toLongitude);
            this.processingTab.Controls.Add(this.toLatitude);
            this.processingTab.Controls.Add(this.fromLongitude);
            this.processingTab.Controls.Add(this.fromLatitude);
            this.processingTab.Controls.Add(this.leftLongitudeLabel);
            this.processingTab.Controls.Add(this.leftLatitudeLabel);
            this.processingTab.Controls.Add(this.toLabel);
            this.processingTab.Controls.Add(this.fromLabel);
            this.processingTab.Controls.Add(this.loopSpeedLabel);
            this.processingTab.Controls.Add(this.TSRLabel);
            this.processingTab.Controls.Add(this.leftLocationLabel);
            this.processingTab.Controls.Add(this.rightLocationLabel);
            this.processingTab.Controls.Add(this.startKmLabel);
            this.processingTab.Controls.Add(this.minDistanceLabel);
            this.processingTab.Controls.Add(this.dataSeparationLabel);
            this.processingTab.Controls.Add(this.timeSeparationLabel);
            this.processingTab.Controls.Add(this.endKmLabel);
            this.processingTab.Controls.Add(this.label3loopBoundarylabel);
            this.processingTab.Controls.Add(this.interpolationLabel);
            this.processingTab.Controls.Add(this.GeographicBoxLabel);
            this.processingTab.Controls.Add(this.DateRangeLabel);
            this.processingTab.Location = new System.Drawing.Point(4, 29);
            this.processingTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.processingTab.Name = "processingTab";
            this.processingTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.processingTab.Size = new System.Drawing.Size(1618, 761);
            this.processingTab.TabIndex = 1;
            this.processingTab.Text = "Processing Parameters";
            this.processingTab.UseVisualStyleBackColor = true;
            // 
            // toDate
            // 
            this.toDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.toDate.Location = new System.Drawing.Point(722, 71);
            this.toDate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toDate.Name = "toDate";
            this.toDate.Size = new System.Drawing.Size(148, 26);
            this.toDate.TabIndex = 31;
            this.toDate.Value = new System.DateTime(2017, 6, 1, 0, 0, 0, 0);
            // 
            // fromDate
            // 
            this.fromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromDate.Location = new System.Drawing.Point(430, 71);
            this.fromDate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fromDate.Name = "fromDate";
            this.fromDate.Size = new System.Drawing.Size(148, 26);
            this.fromDate.TabIndex = 30;
            this.fromDate.Value = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            // 
            // timeSeparation
            // 
            this.timeSeparation.Location = new System.Drawing.Point(430, 462);
            this.timeSeparation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.timeSeparation.Name = "timeSeparation";
            this.timeSeparation.Size = new System.Drawing.Size(148, 26);
            this.timeSeparation.TabIndex = 29;
            this.timeSeparation.Text = "10";
            // 
            // TSRWindowBoundary
            // 
            this.TSRWindowBoundary.Location = new System.Drawing.Point(992, 462);
            this.TSRWindowBoundary.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TSRWindowBoundary.Name = "TSRWindowBoundary";
            this.TSRWindowBoundary.Size = new System.Drawing.Size(148, 26);
            this.TSRWindowBoundary.TabIndex = 28;
            this.TSRWindowBoundary.Text = "1";
            // 
            // dataSeparation
            // 
            this.dataSeparation.Location = new System.Drawing.Point(430, 422);
            this.dataSeparation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataSeparation.Name = "dataSeparation";
            this.dataSeparation.Size = new System.Drawing.Size(148, 26);
            this.dataSeparation.TabIndex = 27;
            this.dataSeparation.Text = "4";
            // 
            // loopSpeedThreshold
            // 
            this.loopSpeedThreshold.Location = new System.Drawing.Point(992, 422);
            this.loopSpeedThreshold.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.loopSpeedThreshold.Name = "loopSpeedThreshold";
            this.loopSpeedThreshold.Size = new System.Drawing.Size(148, 26);
            this.loopSpeedThreshold.TabIndex = 26;
            this.loopSpeedThreshold.Text = "50";
            // 
            // minimumJourneyDistance
            // 
            this.minimumJourneyDistance.Location = new System.Drawing.Point(430, 382);
            this.minimumJourneyDistance.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.minimumJourneyDistance.Name = "minimumJourneyDistance";
            this.minimumJourneyDistance.Size = new System.Drawing.Size(148, 26);
            this.minimumJourneyDistance.TabIndex = 25;
            this.minimumJourneyDistance.Text = "100";
            // 
            // loopBoundaryThreshold
            // 
            this.loopBoundaryThreshold.Location = new System.Drawing.Point(992, 382);
            this.loopBoundaryThreshold.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.loopBoundaryThreshold.Name = "loopBoundaryThreshold";
            this.loopBoundaryThreshold.Size = new System.Drawing.Size(148, 26);
            this.loopBoundaryThreshold.TabIndex = 24;
            this.loopBoundaryThreshold.Text = "1";
            // 
            // interpolationInterval
            // 
            this.interpolationInterval.Location = new System.Drawing.Point(430, 342);
            this.interpolationInterval.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.interpolationInterval.Name = "interpolationInterval";
            this.interpolationInterval.Size = new System.Drawing.Size(148, 26);
            this.interpolationInterval.TabIndex = 23;
            this.interpolationInterval.Text = "50";
            // 
            // endInterpolationKm
            // 
            this.endInterpolationKm.Location = new System.Drawing.Point(992, 302);
            this.endInterpolationKm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.endInterpolationKm.Name = "endInterpolationKm";
            this.endInterpolationKm.Size = new System.Drawing.Size(148, 26);
            this.endInterpolationKm.TabIndex = 22;
            this.endInterpolationKm.Text = "460";
            // 
            // startInterpolationKm
            // 
            this.startInterpolationKm.Location = new System.Drawing.Point(430, 302);
            this.startInterpolationKm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.startInterpolationKm.Name = "startInterpolationKm";
            this.startInterpolationKm.Size = new System.Drawing.Size(148, 26);
            this.startInterpolationKm.TabIndex = 21;
            this.startInterpolationKm.Text = "280";
            // 
            // toLongitude
            // 
            this.toLongitude.Location = new System.Drawing.Point(722, 211);
            this.toLongitude.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toLongitude.Name = "toLongitude";
            this.toLongitude.Size = new System.Drawing.Size(148, 26);
            this.toLongitude.TabIndex = 20;
            this.toLongitude.Text = "152";
            // 
            // toLatitude
            // 
            this.toLatitude.Location = new System.Drawing.Point(430, 211);
            this.toLatitude.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toLatitude.Name = "toLatitude";
            this.toLatitude.Size = new System.Drawing.Size(148, 26);
            this.toLatitude.TabIndex = 19;
            this.toLatitude.Text = "-40";
            // 
            // fromLongitude
            // 
            this.fromLongitude.Location = new System.Drawing.Point(722, 171);
            this.fromLongitude.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fromLongitude.Name = "fromLongitude";
            this.fromLongitude.Size = new System.Drawing.Size(148, 26);
            this.fromLongitude.TabIndex = 18;
            this.fromLongitude.Text = "110";
            // 
            // fromLatitude
            // 
            this.fromLatitude.Location = new System.Drawing.Point(430, 171);
            this.fromLatitude.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fromLatitude.Name = "fromLatitude";
            this.fromLatitude.Size = new System.Drawing.Size(148, 26);
            this.fromLatitude.TabIndex = 17;
            this.fromLatitude.Text = "-10";
            // 
            // leftLongitudeLabel
            // 
            this.leftLongitudeLabel.AutoSize = true;
            this.leftLongitudeLabel.Location = new System.Drawing.Point(717, 135);
            this.leftLongitudeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.leftLongitudeLabel.Name = "leftLongitudeLabel";
            this.leftLongitudeLabel.Size = new System.Drawing.Size(84, 20);
            this.leftLongitudeLabel.TabIndex = 16;
            this.leftLongitudeLabel.Text = "Longitude:";
            // 
            // leftLatitudeLabel
            // 
            this.leftLatitudeLabel.AutoSize = true;
            this.leftLatitudeLabel.Location = new System.Drawing.Point(426, 135);
            this.leftLatitudeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.leftLatitudeLabel.Name = "leftLatitudeLabel";
            this.leftLatitudeLabel.Size = new System.Drawing.Size(71, 20);
            this.leftLatitudeLabel.TabIndex = 15;
            this.leftLatitudeLabel.Text = "Latitude:";
            // 
            // toLabel
            // 
            this.toLabel.AutoSize = true;
            this.toLabel.Location = new System.Drawing.Point(717, 28);
            this.toLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(31, 20);
            this.toLabel.TabIndex = 14;
            this.toLabel.Text = "To:";
            // 
            // fromLabel
            // 
            this.fromLabel.AutoSize = true;
            this.fromLabel.Location = new System.Drawing.Point(426, 28);
            this.fromLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(50, 20);
            this.fromLabel.TabIndex = 13;
            this.fromLabel.Text = "From:";
            // 
            // loopSpeedLabel
            // 
            this.loopSpeedLabel.AutoSize = true;
            this.loopSpeedLabel.Location = new System.Drawing.Point(717, 426);
            this.loopSpeedLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.loopSpeedLabel.Name = "loopSpeedLabel";
            this.loopSpeedLabel.Size = new System.Drawing.Size(198, 20);
            this.loopSpeedLabel.TabIndex = 12;
            this.loopSpeedLabel.Text = "Loop Speed Threshold (%)";
            // 
            // TSRLabel
            // 
            this.TSRLabel.AutoSize = true;
            this.TSRLabel.Location = new System.Drawing.Point(717, 466);
            this.TSRLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TSRLabel.Name = "TSRLabel";
            this.TSRLabel.Size = new System.Drawing.Size(136, 20);
            this.TSRLabel.TabIndex = 11;
            this.TSRLabel.Text = "TSR Window (km)";
            // 
            // leftLocationLabel
            // 
            this.leftLocationLabel.AutoSize = true;
            this.leftLocationLabel.Location = new System.Drawing.Point(144, 175);
            this.leftLocationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.leftLocationLabel.Name = "leftLocationLabel";
            this.leftLocationLabel.Size = new System.Drawing.Size(137, 20);
            this.leftLocationLabel.TabIndex = 10;
            this.leftLocationLabel.Text = "Top Left Location:";
            // 
            // rightLocationLabel
            // 
            this.rightLocationLabel.AutoSize = true;
            this.rightLocationLabel.Location = new System.Drawing.Point(144, 215);
            this.rightLocationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rightLocationLabel.Name = "rightLocationLabel";
            this.rightLocationLabel.Size = new System.Drawing.Size(172, 20);
            this.rightLocationLabel.TabIndex = 9;
            this.rightLocationLabel.Text = "Bottom Right Location:";
            // 
            // startKmLabel
            // 
            this.startKmLabel.AutoSize = true;
            this.startKmLabel.Location = new System.Drawing.Point(46, 306);
            this.startKmLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.startKmLabel.Name = "startKmLabel";
            this.startKmLabel.Size = new System.Drawing.Size(176, 20);
            this.startKmLabel.TabIndex = 8;
            this.startKmLabel.Text = "Start Kilometreage (km)";
            // 
            // minDistanceLabel
            // 
            this.minDistanceLabel.AutoSize = true;
            this.minDistanceLabel.Location = new System.Drawing.Point(46, 386);
            this.minDistanceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.minDistanceLabel.Name = "minDistanceLabel";
            this.minDistanceLabel.Size = new System.Drawing.Size(220, 20);
            this.minDistanceLabel.TabIndex = 7;
            this.minDistanceLabel.Text = "Minimum Travel Distance (km)";
            // 
            // dataSeparationLabel
            // 
            this.dataSeparationLabel.AutoSize = true;
            this.dataSeparationLabel.Location = new System.Drawing.Point(46, 426);
            this.dataSeparationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.dataSeparationLabel.Name = "dataSeparationLabel";
            this.dataSeparationLabel.Size = new System.Drawing.Size(161, 20);
            this.dataSeparationLabel.TabIndex = 6;
            this.dataSeparationLabel.Text = "Data Separation (km)";
            // 
            // timeSeparationLabel
            // 
            this.timeSeparationLabel.AutoSize = true;
            this.timeSeparationLabel.Location = new System.Drawing.Point(46, 466);
            this.timeSeparationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.timeSeparationLabel.Name = "timeSeparationLabel";
            this.timeSeparationLabel.Size = new System.Drawing.Size(148, 20);
            this.timeSeparationLabel.TabIndex = 5;
            this.timeSeparationLabel.Text = "Time Separation (h)";
            // 
            // endKmLabel
            // 
            this.endKmLabel.AutoSize = true;
            this.endKmLabel.Location = new System.Drawing.Point(717, 306);
            this.endKmLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.endKmLabel.Name = "endKmLabel";
            this.endKmLabel.Size = new System.Drawing.Size(164, 20);
            this.endKmLabel.TabIndex = 4;
            this.endKmLabel.Text = "End interpolation (km)";
            // 
            // label3loopBoundarylabel
            // 
            this.label3loopBoundarylabel.AutoSize = true;
            this.label3loopBoundarylabel.Location = new System.Drawing.Point(717, 386);
            this.label3loopBoundarylabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3loopBoundarylabel.Name = "label3loopBoundarylabel";
            this.label3loopBoundarylabel.Size = new System.Drawing.Size(226, 20);
            this.label3loopBoundarylabel.TabIndex = 3;
            this.label3loopBoundarylabel.Text = "Loop Boundary Threshold (km)";
            // 
            // interpolationLabel
            // 
            this.interpolationLabel.AutoSize = true;
            this.interpolationLabel.Location = new System.Drawing.Point(46, 346);
            this.interpolationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.interpolationLabel.Name = "interpolationLabel";
            this.interpolationLabel.Size = new System.Drawing.Size(181, 20);
            this.interpolationLabel.TabIndex = 2;
            this.interpolationLabel.Text = "Interpolation Interval (m)";
            // 
            // GeographicBoxLabel
            // 
            this.GeographicBoxLabel.AutoSize = true;
            this.GeographicBoxLabel.Location = new System.Drawing.Point(46, 135);
            this.GeographicBoxLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GeographicBoxLabel.Name = "GeographicBoxLabel";
            this.GeographicBoxLabel.Size = new System.Drawing.Size(191, 20);
            this.GeographicBoxLabel.TabIndex = 1;
            this.GeographicBoxLabel.Text = "Geographic Confinement:";
            // 
            // DateRangeLabel
            // 
            this.DateRangeLabel.AutoSize = true;
            this.DateRangeLabel.Location = new System.Drawing.Point(46, 80);
            this.DateRangeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DateRangeLabel.Name = "DateRangeLabel";
            this.DateRangeLabel.Size = new System.Drawing.Size(100, 20);
            this.DateRangeLabel.TabIndex = 0;
            this.DateRangeLabel.Text = "Date Range:";
            // 
            // simulationParametersTab
            // 
            this.simulationParametersTab.Controls.Add(this.SimulationP2WRatioLabel);
            this.simulationParametersTab.Controls.Add(this.combinedDecreasingTrainCount);
            this.simulationParametersTab.Controls.Add(this.catagory2DecreasingTrainCount);
            this.simulationParametersTab.Controls.Add(this.catagory1DecreasingTrainCount);
            this.simulationParametersTab.Controls.Add(this.combinedDecreasingPowerToWeightRatio);
            this.simulationParametersTab.Controls.Add(this.catagory2DecreasingPowerToWeightRatio);
            this.simulationParametersTab.Controls.Add(this.catagory1DecreasingPowerToWeightRatio);
            this.simulationParametersTab.Controls.Add(this.combinedIncreasingTrainCount);
            this.simulationParametersTab.Controls.Add(this.catagory2IncreasingTrainCount);
            this.simulationParametersTab.Controls.Add(this.catagory1IncreasingTrainCount);
            this.simulationParametersTab.Controls.Add(this.combinedIncreasingPowerToWeightRatio);
            this.simulationParametersTab.Controls.Add(this.catagory2IncreasingPowerToWeightRatio);
            this.simulationParametersTab.Controls.Add(this.catagory1IncreasingPowerToWeightRatio);
            this.simulationParametersTab.Controls.Add(this.countLabel2);
            this.simulationParametersTab.Controls.Add(this.PWRatioLabel2);
            this.simulationParametersTab.Controls.Add(this.countLabel1);
            this.simulationParametersTab.Controls.Add(this.PWRatioLabel1);
            this.simulationParametersTab.Controls.Add(this.simulationPowerToWeightRatios);
            this.simulationParametersTab.Controls.Add(this.catagory2UpperBound);
            this.simulationParametersTab.Controls.Add(this.catagory2LowerBound);
            this.simulationParametersTab.Controls.Add(this.catagory1UpperBound);
            this.simulationParametersTab.Controls.Add(this.catagory1LowerBound);
            this.simulationParametersTab.Controls.Add(this.powerToWeightLabel);
            this.simulationParametersTab.Controls.Add(this.simICEDataFile);
            this.simulationParametersTab.Controls.Add(this.combinedLabel);
            this.simulationParametersTab.Controls.Add(this.simCatagory2Label);
            this.simulationParametersTab.Controls.Add(this.simCatagory1Label);
            this.simulationParametersTab.Controls.Add(this.decreasingLabel);
            this.simulationParametersTab.Controls.Add(this.increasingLabel);
            this.simulationParametersTab.Controls.Add(this.upperBoundLabel);
            this.simulationParametersTab.Controls.Add(this.lowerBoundLabel);
            this.simulationParametersTab.Controls.Add(this.catagory2Label);
            this.simulationParametersTab.Controls.Add(this.catagory1Label);
            this.simulationParametersTab.Controls.Add(this.dataFileLabel);
            this.simulationParametersTab.Location = new System.Drawing.Point(4, 29);
            this.simulationParametersTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.simulationParametersTab.Name = "simulationParametersTab";
            this.simulationParametersTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.simulationParametersTab.Size = new System.Drawing.Size(1618, 761);
            this.simulationParametersTab.TabIndex = 2;
            this.simulationParametersTab.Text = "Simulation Parameters";
            this.simulationParametersTab.UseVisualStyleBackColor = true;
            // 
            // SimulationP2WRatioLabel
            // 
            this.SimulationP2WRatioLabel.AutoSize = true;
            this.SimulationP2WRatioLabel.ForeColor = System.Drawing.Color.Red;
            this.SimulationP2WRatioLabel.Location = new System.Drawing.Point(508, 646);
            this.SimulationP2WRatioLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SimulationP2WRatioLabel.Name = "SimulationP2WRatioLabel";
            this.SimulationP2WRatioLabel.Size = new System.Drawing.Size(51, 20);
            this.SimulationP2WRatioLabel.TabIndex = 33;
            this.SimulationP2WRatioLabel.Text = "label1";
            // 
            // combinedDecreasingTrainCount
            // 
            this.combinedDecreasingTrainCount.AutoSize = true;
            this.combinedDecreasingTrainCount.Location = new System.Drawing.Point(736, 526);
            this.combinedDecreasingTrainCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.combinedDecreasingTrainCount.Name = "combinedDecreasingTrainCount";
            this.combinedDecreasingTrainCount.Size = new System.Drawing.Size(18, 20);
            this.combinedDecreasingTrainCount.TabIndex = 32;
            this.combinedDecreasingTrainCount.Text = "0";
            // 
            // catagory2DecreasingTrainCount
            // 
            this.catagory2DecreasingTrainCount.AutoSize = true;
            this.catagory2DecreasingTrainCount.Location = new System.Drawing.Point(736, 478);
            this.catagory2DecreasingTrainCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory2DecreasingTrainCount.Name = "catagory2DecreasingTrainCount";
            this.catagory2DecreasingTrainCount.Size = new System.Drawing.Size(18, 20);
            this.catagory2DecreasingTrainCount.TabIndex = 31;
            this.catagory2DecreasingTrainCount.Text = "0";
            // 
            // catagory1DecreasingTrainCount
            // 
            this.catagory1DecreasingTrainCount.AutoSize = true;
            this.catagory1DecreasingTrainCount.Location = new System.Drawing.Point(736, 438);
            this.catagory1DecreasingTrainCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory1DecreasingTrainCount.Name = "catagory1DecreasingTrainCount";
            this.catagory1DecreasingTrainCount.Size = new System.Drawing.Size(18, 20);
            this.catagory1DecreasingTrainCount.TabIndex = 30;
            this.catagory1DecreasingTrainCount.Text = "0";
            // 
            // combinedDecreasingPowerToWeightRatio
            // 
            this.combinedDecreasingPowerToWeightRatio.AutoSize = true;
            this.combinedDecreasingPowerToWeightRatio.Location = new System.Drawing.Point(600, 526);
            this.combinedDecreasingPowerToWeightRatio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.combinedDecreasingPowerToWeightRatio.Name = "combinedDecreasingPowerToWeightRatio";
            this.combinedDecreasingPowerToWeightRatio.Size = new System.Drawing.Size(18, 20);
            this.combinedDecreasingPowerToWeightRatio.TabIndex = 29;
            this.combinedDecreasingPowerToWeightRatio.Text = "0";
            // 
            // catagory2DecreasingPowerToWeightRatio
            // 
            this.catagory2DecreasingPowerToWeightRatio.AutoSize = true;
            this.catagory2DecreasingPowerToWeightRatio.Location = new System.Drawing.Point(600, 478);
            this.catagory2DecreasingPowerToWeightRatio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory2DecreasingPowerToWeightRatio.Name = "catagory2DecreasingPowerToWeightRatio";
            this.catagory2DecreasingPowerToWeightRatio.Size = new System.Drawing.Size(18, 20);
            this.catagory2DecreasingPowerToWeightRatio.TabIndex = 28;
            this.catagory2DecreasingPowerToWeightRatio.Text = "0";
            // 
            // catagory1DecreasingPowerToWeightRatio
            // 
            this.catagory1DecreasingPowerToWeightRatio.AutoSize = true;
            this.catagory1DecreasingPowerToWeightRatio.Location = new System.Drawing.Point(600, 438);
            this.catagory1DecreasingPowerToWeightRatio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory1DecreasingPowerToWeightRatio.Name = "catagory1DecreasingPowerToWeightRatio";
            this.catagory1DecreasingPowerToWeightRatio.Size = new System.Drawing.Size(18, 20);
            this.catagory1DecreasingPowerToWeightRatio.TabIndex = 27;
            this.catagory1DecreasingPowerToWeightRatio.Text = "0";
            // 
            // combinedIncreasingTrainCount
            // 
            this.combinedIncreasingTrainCount.AutoSize = true;
            this.combinedIncreasingTrainCount.Location = new System.Drawing.Point(408, 526);
            this.combinedIncreasingTrainCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.combinedIncreasingTrainCount.Name = "combinedIncreasingTrainCount";
            this.combinedIncreasingTrainCount.Size = new System.Drawing.Size(18, 20);
            this.combinedIncreasingTrainCount.TabIndex = 26;
            this.combinedIncreasingTrainCount.Text = "0";
            // 
            // catagory2IncreasingTrainCount
            // 
            this.catagory2IncreasingTrainCount.AutoSize = true;
            this.catagory2IncreasingTrainCount.Location = new System.Drawing.Point(408, 478);
            this.catagory2IncreasingTrainCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory2IncreasingTrainCount.Name = "catagory2IncreasingTrainCount";
            this.catagory2IncreasingTrainCount.Size = new System.Drawing.Size(18, 20);
            this.catagory2IncreasingTrainCount.TabIndex = 25;
            this.catagory2IncreasingTrainCount.Text = "0";
            // 
            // catagory1IncreasingTrainCount
            // 
            this.catagory1IncreasingTrainCount.AutoSize = true;
            this.catagory1IncreasingTrainCount.Location = new System.Drawing.Point(408, 438);
            this.catagory1IncreasingTrainCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory1IncreasingTrainCount.Name = "catagory1IncreasingTrainCount";
            this.catagory1IncreasingTrainCount.Size = new System.Drawing.Size(18, 20);
            this.catagory1IncreasingTrainCount.TabIndex = 24;
            this.catagory1IncreasingTrainCount.Text = "0";
            // 
            // combinedIncreasingPowerToWeightRatio
            // 
            this.combinedIncreasingPowerToWeightRatio.AutoSize = true;
            this.combinedIncreasingPowerToWeightRatio.Location = new System.Drawing.Point(279, 526);
            this.combinedIncreasingPowerToWeightRatio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.combinedIncreasingPowerToWeightRatio.Name = "combinedIncreasingPowerToWeightRatio";
            this.combinedIncreasingPowerToWeightRatio.Size = new System.Drawing.Size(18, 20);
            this.combinedIncreasingPowerToWeightRatio.TabIndex = 23;
            this.combinedIncreasingPowerToWeightRatio.Text = "0";
            // 
            // catagory2IncreasingPowerToWeightRatio
            // 
            this.catagory2IncreasingPowerToWeightRatio.AutoSize = true;
            this.catagory2IncreasingPowerToWeightRatio.Location = new System.Drawing.Point(279, 478);
            this.catagory2IncreasingPowerToWeightRatio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory2IncreasingPowerToWeightRatio.Name = "catagory2IncreasingPowerToWeightRatio";
            this.catagory2IncreasingPowerToWeightRatio.Size = new System.Drawing.Size(18, 20);
            this.catagory2IncreasingPowerToWeightRatio.TabIndex = 22;
            this.catagory2IncreasingPowerToWeightRatio.Text = "0";
            // 
            // catagory1IncreasingPowerToWeightRatio
            // 
            this.catagory1IncreasingPowerToWeightRatio.AutoSize = true;
            this.catagory1IncreasingPowerToWeightRatio.Location = new System.Drawing.Point(279, 438);
            this.catagory1IncreasingPowerToWeightRatio.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory1IncreasingPowerToWeightRatio.Name = "catagory1IncreasingPowerToWeightRatio";
            this.catagory1IncreasingPowerToWeightRatio.Size = new System.Drawing.Size(18, 20);
            this.catagory1IncreasingPowerToWeightRatio.TabIndex = 21;
            this.catagory1IncreasingPowerToWeightRatio.Text = "0";
            // 
            // countLabel2
            // 
            this.countLabel2.AutoSize = true;
            this.countLabel2.Location = new System.Drawing.Point(736, 403);
            this.countLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.countLabel2.Name = "countLabel2";
            this.countLabel2.Size = new System.Drawing.Size(52, 20);
            this.countLabel2.TabIndex = 20;
            this.countLabel2.Text = "Count";
            // 
            // PWRatioLabel2
            // 
            this.PWRatioLabel2.AutoSize = true;
            this.PWRatioLabel2.Location = new System.Drawing.Point(600, 403);
            this.PWRatioLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PWRatioLabel2.Name = "PWRatioLabel2";
            this.PWRatioLabel2.Size = new System.Drawing.Size(73, 20);
            this.PWRatioLabel2.TabIndex = 19;
            this.PWRatioLabel2.Text = "P/W ratio";
            // 
            // countLabel1
            // 
            this.countLabel1.AutoSize = true;
            this.countLabel1.Location = new System.Drawing.Point(408, 403);
            this.countLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.countLabel1.Name = "countLabel1";
            this.countLabel1.Size = new System.Drawing.Size(52, 20);
            this.countLabel1.TabIndex = 18;
            this.countLabel1.Text = "Count";
            // 
            // PWRatioLabel1
            // 
            this.PWRatioLabel1.AutoSize = true;
            this.PWRatioLabel1.Location = new System.Drawing.Point(279, 403);
            this.PWRatioLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PWRatioLabel1.Name = "PWRatioLabel1";
            this.PWRatioLabel1.Size = new System.Drawing.Size(73, 20);
            this.PWRatioLabel1.TabIndex = 17;
            this.PWRatioLabel1.Text = "P/W ratio";
            // 
            // simulationPowerToWeightRatios
            // 
            this.simulationPowerToWeightRatios.Location = new System.Drawing.Point(74, 311);
            this.simulationPowerToWeightRatios.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.simulationPowerToWeightRatios.Name = "simulationPowerToWeightRatios";
            this.simulationPowerToWeightRatios.Size = new System.Drawing.Size(204, 78);
            this.simulationPowerToWeightRatios.TabIndex = 16;
            this.simulationPowerToWeightRatios.Text = "Generate Simulation Power to Weight Ratios";
            this.simulationPowerToWeightRatios.UseVisualStyleBackColor = true;
            this.simulationPowerToWeightRatios.Click += new System.EventHandler(this.simulationPowerToWeightRatios_Click);
            // 
            // catagory2UpperBound
            // 
            this.catagory2UpperBound.Location = new System.Drawing.Point(604, 226);
            this.catagory2UpperBound.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory2UpperBound.Name = "catagory2UpperBound";
            this.catagory2UpperBound.Size = new System.Drawing.Size(148, 26);
            this.catagory2UpperBound.TabIndex = 15;
            this.catagory2UpperBound.Text = "11.5";
            // 
            // catagory2LowerBound
            // 
            this.catagory2LowerBound.Location = new System.Drawing.Point(284, 226);
            this.catagory2LowerBound.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory2LowerBound.Name = "catagory2LowerBound";
            this.catagory2LowerBound.Size = new System.Drawing.Size(148, 26);
            this.catagory2LowerBound.TabIndex = 14;
            this.catagory2LowerBound.Text = "4.5";
            // 
            // catagory1UpperBound
            // 
            this.catagory1UpperBound.Location = new System.Drawing.Point(604, 186);
            this.catagory1UpperBound.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory1UpperBound.Name = "catagory1UpperBound";
            this.catagory1UpperBound.Size = new System.Drawing.Size(148, 26);
            this.catagory1UpperBound.TabIndex = 13;
            this.catagory1UpperBound.Text = "4.5";
            // 
            // catagory1LowerBound
            // 
            this.catagory1LowerBound.Location = new System.Drawing.Point(284, 186);
            this.catagory1LowerBound.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory1LowerBound.Name = "catagory1LowerBound";
            this.catagory1LowerBound.Size = new System.Drawing.Size(148, 26);
            this.catagory1LowerBound.TabIndex = 12;
            this.catagory1LowerBound.Text = "1.5";
            // 
            // powerToWeightLabel
            // 
            this.powerToWeightLabel.AutoSize = true;
            this.powerToWeightLabel.Location = new System.Drawing.Point(69, 125);
            this.powerToWeightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.powerToWeightLabel.Name = "powerToWeightLabel";
            this.powerToWeightLabel.Size = new System.Drawing.Size(183, 20);
            this.powerToWeightLabel.TabIndex = 11;
            this.powerToWeightLabel.Text = "Power To Weight Ratios:";
            // 
            // simICEDataFile
            // 
            this.simICEDataFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.simICEDataFile.Location = new System.Drawing.Point(186, 43);
            this.simICEDataFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.simICEDataFile.Name = "simICEDataFile";
            this.simICEDataFile.Size = new System.Drawing.Size(700, 26);
            this.simICEDataFile.TabIndex = 10;
            this.simICEDataFile.Text = "Data File Loaded from File Selection tab";
            // 
            // combinedLabel
            // 
            this.combinedLabel.AutoSize = true;
            this.combinedLabel.Location = new System.Drawing.Point(69, 526);
            this.combinedLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.combinedLabel.Name = "combinedLabel";
            this.combinedLabel.Size = new System.Drawing.Size(85, 20);
            this.combinedLabel.TabIndex = 9;
            this.combinedLabel.Text = "Combined:";
            // 
            // simCatagory2Label
            // 
            this.simCatagory2Label.AutoSize = true;
            this.simCatagory2Label.Location = new System.Drawing.Point(69, 478);
            this.simCatagory2Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.simCatagory2Label.Name = "simCatagory2Label";
            this.simCatagory2Label.Size = new System.Drawing.Size(107, 20);
            this.simCatagory2Label.TabIndex = 8;
            this.simCatagory2Label.Text = "Overpowered:";
            // 
            // simCatagory1Label
            // 
            this.simCatagory1Label.AutoSize = true;
            this.simCatagory1Label.Location = new System.Drawing.Point(69, 438);
            this.simCatagory1Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.simCatagory1Label.Name = "simCatagory1Label";
            this.simCatagory1Label.Size = new System.Drawing.Size(118, 20);
            this.simCatagory1Label.TabIndex = 7;
            this.simCatagory1Label.Text = "Underpowered:";
            // 
            // decreasingLabel
            // 
            this.decreasingLabel.AutoSize = true;
            this.decreasingLabel.Location = new System.Drawing.Point(600, 369);
            this.decreasingLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.decreasingLabel.Name = "decreasingLabel";
            this.decreasingLabel.Size = new System.Drawing.Size(186, 20);
            this.decreasingLabel.TabIndex = 6;
            this.decreasingLabel.Text = "Decreasing km Direction:";
            // 
            // increasingLabel
            // 
            this.increasingLabel.AutoSize = true;
            this.increasingLabel.Location = new System.Drawing.Point(279, 369);
            this.increasingLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.increasingLabel.Name = "increasingLabel";
            this.increasingLabel.Size = new System.Drawing.Size(179, 20);
            this.increasingLabel.TabIndex = 5;
            this.increasingLabel.Text = "Increasing km Direction:";
            // 
            // upperBoundLabel
            // 
            this.upperBoundLabel.AutoSize = true;
            this.upperBoundLabel.Location = new System.Drawing.Point(600, 152);
            this.upperBoundLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.upperBoundLabel.Name = "upperBoundLabel";
            this.upperBoundLabel.Size = new System.Drawing.Size(108, 20);
            this.upperBoundLabel.TabIndex = 4;
            this.upperBoundLabel.Text = "Upper Bound:";
            // 
            // lowerBoundLabel
            // 
            this.lowerBoundLabel.AutoSize = true;
            this.lowerBoundLabel.Location = new System.Drawing.Point(279, 152);
            this.lowerBoundLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lowerBoundLabel.Name = "lowerBoundLabel";
            this.lowerBoundLabel.Size = new System.Drawing.Size(107, 20);
            this.lowerBoundLabel.TabIndex = 3;
            this.lowerBoundLabel.Text = "Lower Bound:";
            // 
            // catagory2Label
            // 
            this.catagory2Label.AutoSize = true;
            this.catagory2Label.Location = new System.Drawing.Point(69, 231);
            this.catagory2Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory2Label.Name = "catagory2Label";
            this.catagory2Label.Size = new System.Drawing.Size(107, 20);
            this.catagory2Label.TabIndex = 2;
            this.catagory2Label.Text = "Overpowered:";
            // 
            // catagory1Label
            // 
            this.catagory1Label.AutoSize = true;
            this.catagory1Label.Location = new System.Drawing.Point(69, 191);
            this.catagory1Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory1Label.Name = "catagory1Label";
            this.catagory1Label.Size = new System.Drawing.Size(118, 20);
            this.catagory1Label.TabIndex = 1;
            this.catagory1Label.Text = "Underpowered:";
            // 
            // dataFileLabel
            // 
            this.dataFileLabel.AutoSize = true;
            this.dataFileLabel.Location = new System.Drawing.Point(69, 48);
            this.dataFileLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.dataFileLabel.Name = "dataFileLabel";
            this.dataFileLabel.Size = new System.Drawing.Size(108, 20);
            this.dataFileLabel.TabIndex = 0;
            this.dataFileLabel.Text = "ICE Data File:";
            // 
            // simualtionFileTab
            // 
            this.simualtionFileTab.Controls.Add(this.Commodity3Catagory);
            this.simualtionFileTab.Controls.Add(this.Operator3Catagory);
            this.simualtionFileTab.Controls.Add(this.Commodity2Catagory);
            this.simualtionFileTab.Controls.Add(this.Operator2Catagory);
            this.simualtionFileTab.Controls.Add(this.Commodity1Catagory);
            this.simualtionFileTab.Controls.Add(this.Operator1Catagory);
            this.simualtionFileTab.Controls.Add(this.CommodityLabel);
            this.simualtionFileTab.Controls.Add(this.OperatorLabel);
            this.simualtionFileTab.Controls.Add(this.Execute);
            this.simualtionFileTab.Controls.Add(this.executionTime);
            this.simualtionFileTab.Controls.Add(this.ExecitionTimeLabel);
            this.simualtionFileTab.Controls.Add(this.resultsDestination);
            this.simualtionFileTab.Controls.Add(this.ResultsDirectoryLabel);
            this.simualtionFileTab.Controls.Add(this.resultsDirectory);
            this.simualtionFileTab.Controls.Add(this.selectCatagory3DecreasingSimulation);
            this.simualtionFileTab.Controls.Add(this.catagory3DecreasingSimulationFile);
            this.simualtionFileTab.Controls.Add(this.selectCatagory3IncreasingSimulation);
            this.simualtionFileTab.Controls.Add(this.catagory3SimualtionLabel);
            this.simualtionFileTab.Controls.Add(this.catagory3IncreasingSimulationFile);
            this.simualtionFileTab.Controls.Add(this.selectCatagory2DecreasingSimulation);
            this.simualtionFileTab.Controls.Add(this.catagory2DecreasingSimulationFile);
            this.simualtionFileTab.Controls.Add(this.selectCatagory2IncreasingSimulation);
            this.simualtionFileTab.Controls.Add(this.catagory2SimualtionLabel);
            this.simualtionFileTab.Controls.Add(this.catagory2IncreasingSimulationFile);
            this.simualtionFileTab.Controls.Add(this.selectCatagory1DecreasingSimulation);
            this.simualtionFileTab.Controls.Add(this.catagory1DecreasingSimulationFile);
            this.simualtionFileTab.Controls.Add(this.selectCatagory1IncreasingSimulation);
            this.simualtionFileTab.Controls.Add(this.catagory1SimualtionLabel);
            this.simualtionFileTab.Controls.Add(this.catagory1IncreasingSimulationFile);
            this.simualtionFileTab.Location = new System.Drawing.Point(4, 29);
            this.simualtionFileTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.simualtionFileTab.Name = "simualtionFileTab";
            this.simualtionFileTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.simualtionFileTab.Size = new System.Drawing.Size(1618, 761);
            this.simualtionFileTab.TabIndex = 3;
            this.simualtionFileTab.Text = "Simulation File Selection";
            this.simualtionFileTab.UseVisualStyleBackColor = true;
            // 
            // Execute
            // 
            this.Execute.Location = new System.Drawing.Point(1264, 574);
            this.Execute.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Execute.Name = "Execute";
            this.Execute.Size = new System.Drawing.Size(220, 71);
            this.Execute.TabIndex = 20;
            this.Execute.Text = "Execute";
            this.Execute.UseVisualStyleBackColor = true;
            this.Execute.Click += new System.EventHandler(this.Execute_Click);
            // 
            // executionTime
            // 
            this.executionTime.AutoSize = true;
            this.executionTime.Location = new System.Drawing.Point(1413, 669);
            this.executionTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.executionTime.Name = "executionTime";
            this.executionTime.Size = new System.Drawing.Size(39, 20);
            this.executionTime.TabIndex = 19;
            this.executionTime.Text = "time";
            // 
            // ExecitionTimeLabel
            // 
            this.ExecitionTimeLabel.AutoSize = true;
            this.ExecitionTimeLabel.Location = new System.Drawing.Point(1260, 669);
            this.ExecitionTimeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ExecitionTimeLabel.Name = "ExecitionTimeLabel";
            this.ExecitionTimeLabel.Size = new System.Drawing.Size(121, 20);
            this.ExecitionTimeLabel.TabIndex = 18;
            this.ExecitionTimeLabel.Text = "Execution Time:";
            // 
            // resultsDestination
            // 
            this.resultsDestination.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.resultsDestination.Location = new System.Drawing.Point(344, 543);
            this.resultsDestination.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resultsDestination.Name = "resultsDestination";
            this.resultsDestination.Size = new System.Drawing.Size(865, 26);
            this.resultsDestination.TabIndex = 17;
            this.resultsDestination.Text = "<Required>";
            // 
            // ResultsDirectoryLabel
            // 
            this.ResultsDirectoryLabel.AutoSize = true;
            this.ResultsDirectoryLabel.Location = new System.Drawing.Point(48, 574);
            this.ResultsDirectoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ResultsDirectoryLabel.Name = "ResultsDirectoryLabel";
            this.ResultsDirectoryLabel.Size = new System.Drawing.Size(134, 20);
            this.ResultsDirectoryLabel.TabIndex = 16;
            this.ResultsDirectoryLabel.Text = "Resutls Directory:";
            // 
            // resultsDirectory
            // 
            this.resultsDirectory.Location = new System.Drawing.Point(80, 526);
            this.resultsDirectory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resultsDirectory.Name = "resultsDirectory";
            this.resultsDirectory.Size = new System.Drawing.Size(255, 62);
            this.resultsDirectory.TabIndex = 15;
            this.resultsDirectory.Text = "Select AggregatedDestination Directory";
            this.resultsDirectory.UseVisualStyleBackColor = true;
            this.resultsDirectory.Click += new System.EventHandler(this.resultsDirectory_Click);
            // 
            // selectCatagory3DecreasingSimulation
            // 
            this.selectCatagory3DecreasingSimulation.Location = new System.Drawing.Point(80, 443);
            this.selectCatagory3DecreasingSimulation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectCatagory3DecreasingSimulation.Name = "selectCatagory3DecreasingSimulation";
            this.selectCatagory3DecreasingSimulation.Size = new System.Drawing.Size(255, 46);
            this.selectCatagory3DecreasingSimulation.TabIndex = 14;
            this.selectCatagory3DecreasingSimulation.Text = "Select Decreasing Simulation";
            this.selectCatagory3DecreasingSimulation.UseVisualStyleBackColor = true;
            this.selectCatagory3DecreasingSimulation.Click += new System.EventHandler(this.selectCatagory3DecreasingSimulation_Click);
            // 
            // catagory3DecreasingSimulationFile
            // 
            this.catagory3DecreasingSimulationFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.catagory3DecreasingSimulationFile.Location = new System.Drawing.Point(344, 452);
            this.catagory3DecreasingSimulationFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory3DecreasingSimulationFile.Name = "catagory3DecreasingSimulationFile";
            this.catagory3DecreasingSimulationFile.Size = new System.Drawing.Size(865, 26);
            this.catagory3DecreasingSimulationFile.TabIndex = 13;
            this.catagory3DecreasingSimulationFile.Text = "<Optional>";
            // 
            // selectCatagory3IncreasingSimulation
            // 
            this.selectCatagory3IncreasingSimulation.Location = new System.Drawing.Point(80, 382);
            this.selectCatagory3IncreasingSimulation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectCatagory3IncreasingSimulation.Name = "selectCatagory3IncreasingSimulation";
            this.selectCatagory3IncreasingSimulation.Size = new System.Drawing.Size(255, 46);
            this.selectCatagory3IncreasingSimulation.TabIndex = 12;
            this.selectCatagory3IncreasingSimulation.Text = "Select Increasing Simulation";
            this.selectCatagory3IncreasingSimulation.UseVisualStyleBackColor = true;
            this.selectCatagory3IncreasingSimulation.Click += new System.EventHandler(this.selectCatagory3IncreasingSimulation_Click);
            // 
            // catagory3SimualtionLabel
            // 
            this.catagory3SimualtionLabel.AutoSize = true;
            this.catagory3SimualtionLabel.Location = new System.Drawing.Point(48, 357);
            this.catagory3SimualtionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory3SimualtionLabel.Name = "catagory3SimualtionLabel";
            this.catagory3SimualtionLabel.Size = new System.Drawing.Size(88, 20);
            this.catagory3SimualtionLabel.TabIndex = 11;
            this.catagory3SimualtionLabel.Text = "Alternative:";
            // 
            // catagory3IncreasingSimulationFile
            // 
            this.catagory3IncreasingSimulationFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.catagory3IncreasingSimulationFile.Location = new System.Drawing.Point(344, 391);
            this.catagory3IncreasingSimulationFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory3IncreasingSimulationFile.Name = "catagory3IncreasingSimulationFile";
            this.catagory3IncreasingSimulationFile.Size = new System.Drawing.Size(865, 26);
            this.catagory3IncreasingSimulationFile.TabIndex = 10;
            this.catagory3IncreasingSimulationFile.Text = "<Optional>";
            // 
            // selectCatagory2DecreasingSimulation
            // 
            this.selectCatagory2DecreasingSimulation.Location = new System.Drawing.Point(80, 288);
            this.selectCatagory2DecreasingSimulation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectCatagory2DecreasingSimulation.Name = "selectCatagory2DecreasingSimulation";
            this.selectCatagory2DecreasingSimulation.Size = new System.Drawing.Size(255, 46);
            this.selectCatagory2DecreasingSimulation.TabIndex = 9;
            this.selectCatagory2DecreasingSimulation.Text = "Select Decreasing Simulation";
            this.selectCatagory2DecreasingSimulation.UseVisualStyleBackColor = true;
            this.selectCatagory2DecreasingSimulation.Click += new System.EventHandler(this.selectCatagory2DecreasingSimulation_Click);
            // 
            // catagory2DecreasingSimulationFile
            // 
            this.catagory2DecreasingSimulationFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.catagory2DecreasingSimulationFile.Location = new System.Drawing.Point(344, 297);
            this.catagory2DecreasingSimulationFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory2DecreasingSimulationFile.Name = "catagory2DecreasingSimulationFile";
            this.catagory2DecreasingSimulationFile.Size = new System.Drawing.Size(865, 26);
            this.catagory2DecreasingSimulationFile.TabIndex = 8;
            this.catagory2DecreasingSimulationFile.Text = "<Required>";
            // 
            // selectCatagory2IncreasingSimulation
            // 
            this.selectCatagory2IncreasingSimulation.Location = new System.Drawing.Point(80, 226);
            this.selectCatagory2IncreasingSimulation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectCatagory2IncreasingSimulation.Name = "selectCatagory2IncreasingSimulation";
            this.selectCatagory2IncreasingSimulation.Size = new System.Drawing.Size(255, 46);
            this.selectCatagory2IncreasingSimulation.TabIndex = 7;
            this.selectCatagory2IncreasingSimulation.Text = "Select Increasing Simulation";
            this.selectCatagory2IncreasingSimulation.UseVisualStyleBackColor = true;
            this.selectCatagory2IncreasingSimulation.Click += new System.EventHandler(this.selectCatagory2IncreasingSimulation_Click);
            // 
            // catagory2SimualtionLabel
            // 
            this.catagory2SimualtionLabel.AutoSize = true;
            this.catagory2SimualtionLabel.Location = new System.Drawing.Point(48, 202);
            this.catagory2SimualtionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory2SimualtionLabel.Name = "catagory2SimualtionLabel";
            this.catagory2SimualtionLabel.Size = new System.Drawing.Size(107, 20);
            this.catagory2SimualtionLabel.TabIndex = 6;
            this.catagory2SimualtionLabel.Text = "Overpowered:";
            // 
            // catagory2IncreasingSimulationFile
            // 
            this.catagory2IncreasingSimulationFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.catagory2IncreasingSimulationFile.Location = new System.Drawing.Point(344, 235);
            this.catagory2IncreasingSimulationFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory2IncreasingSimulationFile.Name = "catagory2IncreasingSimulationFile";
            this.catagory2IncreasingSimulationFile.Size = new System.Drawing.Size(865, 26);
            this.catagory2IncreasingSimulationFile.TabIndex = 5;
            this.catagory2IncreasingSimulationFile.Text = "<Required>";
            // 
            // selectCatagory1DecreasingSimulation
            // 
            this.selectCatagory1DecreasingSimulation.Location = new System.Drawing.Point(80, 134);
            this.selectCatagory1DecreasingSimulation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectCatagory1DecreasingSimulation.Name = "selectCatagory1DecreasingSimulation";
            this.selectCatagory1DecreasingSimulation.Size = new System.Drawing.Size(255, 46);
            this.selectCatagory1DecreasingSimulation.TabIndex = 4;
            this.selectCatagory1DecreasingSimulation.Text = "Select Decreasing Simulation";
            this.selectCatagory1DecreasingSimulation.UseVisualStyleBackColor = true;
            this.selectCatagory1DecreasingSimulation.Click += new System.EventHandler(this.selectCatagory1DecreasingSimulation_Click);
            // 
            // catagory1DecreasingSimulationFile
            // 
            this.catagory1DecreasingSimulationFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.catagory1DecreasingSimulationFile.Location = new System.Drawing.Point(344, 143);
            this.catagory1DecreasingSimulationFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory1DecreasingSimulationFile.Name = "catagory1DecreasingSimulationFile";
            this.catagory1DecreasingSimulationFile.Size = new System.Drawing.Size(865, 26);
            this.catagory1DecreasingSimulationFile.TabIndex = 3;
            this.catagory1DecreasingSimulationFile.Text = "<Required>";
            // 
            // selectCatagory1IncreasingSimulation
            // 
            this.selectCatagory1IncreasingSimulation.Location = new System.Drawing.Point(80, 72);
            this.selectCatagory1IncreasingSimulation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.selectCatagory1IncreasingSimulation.Name = "selectCatagory1IncreasingSimulation";
            this.selectCatagory1IncreasingSimulation.Size = new System.Drawing.Size(255, 46);
            this.selectCatagory1IncreasingSimulation.TabIndex = 2;
            this.selectCatagory1IncreasingSimulation.Text = "Select Increasing Simulation";
            this.selectCatagory1IncreasingSimulation.UseVisualStyleBackColor = true;
            this.selectCatagory1IncreasingSimulation.Click += new System.EventHandler(this.selectCatagory1IncreasingSimulation_Click);
            // 
            // catagory1SimualtionLabel
            // 
            this.catagory1SimualtionLabel.AutoSize = true;
            this.catagory1SimualtionLabel.Location = new System.Drawing.Point(48, 48);
            this.catagory1SimualtionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.catagory1SimualtionLabel.Name = "catagory1SimualtionLabel";
            this.catagory1SimualtionLabel.Size = new System.Drawing.Size(118, 20);
            this.catagory1SimualtionLabel.TabIndex = 1;
            this.catagory1SimualtionLabel.Text = "Underpowered:";
            // 
            // catagory1IncreasingSimulationFile
            // 
            this.catagory1IncreasingSimulationFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.catagory1IncreasingSimulationFile.Location = new System.Drawing.Point(344, 82);
            this.catagory1IncreasingSimulationFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.catagory1IncreasingSimulationFile.Name = "catagory1IncreasingSimulationFile";
            this.catagory1IncreasingSimulationFile.Size = new System.Drawing.Size(865, 26);
            this.catagory1IncreasingSimulationFile.TabIndex = 0;
            this.catagory1IncreasingSimulationFile.Text = "<Required>";
            // 
            // OperatorLabel
            // 
            this.OperatorLabel.AutoSize = true;
            this.OperatorLabel.Location = new System.Drawing.Point(1260, 30);
            this.OperatorLabel.Name = "OperatorLabel";
            this.OperatorLabel.Size = new System.Drawing.Size(111, 20);
            this.OperatorLabel.TabIndex = 21;
            this.OperatorLabel.Text = "Train Operator";
            // 
            // CommodityLabel
            // 
            this.CommodityLabel.AutoSize = true;
            this.CommodityLabel.Location = new System.Drawing.Point(1413, 29);
            this.CommodityLabel.Name = "CommodityLabel";
            this.CommodityLabel.Size = new System.Drawing.Size(88, 20);
            this.CommodityLabel.TabIndex = 22;
            this.CommodityLabel.Text = "Commodity";
            // 
            // Operator1Catagory
            // 
            this.Operator1Catagory.FormattingEnabled = true;
            this.Operator1Catagory.Items.AddRange(new object[] {
            "Pacific Nationl",
            "Aurizon",
            "Freightliner",
            "ARTC",
            "QUBE"});
            this.Operator1Catagory.Location = new System.Drawing.Point(1250, 106);
            this.Operator1Catagory.Name = "Operator1Catagory";
            this.Operator1Catagory.Size = new System.Drawing.Size(121, 28);
            this.Operator1Catagory.TabIndex = 23;
            // 
            // Commodity1Catagory
            // 
            this.Commodity1Catagory.FormattingEnabled = true;
            this.Commodity1Catagory.Items.AddRange(new object[] {
            "Steel",
            "Minerals",
            "General Freight",
            "Intermodal",
            "Coal"});
            this.Commodity1Catagory.Location = new System.Drawing.Point(1417, 106);
            this.Commodity1Catagory.Name = "Commodity1Catagory";
            this.Commodity1Catagory.Size = new System.Drawing.Size(121, 28);
            this.Commodity1Catagory.TabIndex = 24;
            // 
            // Commodity2Catagory
            // 
            this.Commodity2Catagory.FormattingEnabled = true;
            this.Commodity2Catagory.Items.AddRange(new object[] {
            "Steel",
            "Minerals",
            "General Freight",
            "Intermodal",
            "Coal"});
            this.Commodity2Catagory.Location = new System.Drawing.Point(1417, 262);
            this.Commodity2Catagory.Name = "Commodity2Catagory";
            this.Commodity2Catagory.Size = new System.Drawing.Size(121, 28);
            this.Commodity2Catagory.TabIndex = 26;
            // 
            // Operator2Catagory
            // 
            this.Operator2Catagory.FormattingEnabled = true;
            this.Operator2Catagory.Items.AddRange(new object[] {
            "Pacific Nationl",
            "Aurizon",
            "Freightliner",
            "ARTC",
            "QUBE"});
            this.Operator2Catagory.Location = new System.Drawing.Point(1250, 262);
            this.Operator2Catagory.Name = "Operator2Catagory";
            this.Operator2Catagory.Size = new System.Drawing.Size(121, 28);
            this.Operator2Catagory.TabIndex = 25;
            // 
            // Commodity3Catagory
            // 
            this.Commodity3Catagory.FormattingEnabled = true;
            this.Commodity3Catagory.Items.AddRange(new object[] {
            "Steel",
            "Minerals",
            "General Freight",
            "Intermodal",
            "Coal"});
            this.Commodity3Catagory.Location = new System.Drawing.Point(1417, 422);
            this.Commodity3Catagory.Name = "Commodity3Catagory";
            this.Commodity3Catagory.Size = new System.Drawing.Size(121, 28);
            this.Commodity3Catagory.TabIndex = 28;
            // 
            // Operator3Catagory
            // 
            this.Operator3Catagory.FormattingEnabled = true;
            this.Operator3Catagory.Items.AddRange(new object[] {
            "Pacific Nationl",
            "Aurizon",
            "Freightliner",
            "ARTC",
            "QUBE"});
            this.Operator3Catagory.Location = new System.Drawing.Point(1250, 422);
            this.Operator3Catagory.Name = "Operator3Catagory";
            this.Operator3Catagory.Size = new System.Drawing.Size(121, 28);
            this.Operator3Catagory.TabIndex = 27;
            // 
            // TrainPerformance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1648, 831);
            this.Controls.Add(this.simulationTab);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "TrainPerformance";
            this.Text = "Form1";
            this.simulationTab.ResumeLayout(false);
            this.fileSelectionTab.ResumeLayout(false);
            this.fileSelectionTab.PerformLayout();
            this.processingTab.ResumeLayout(false);
            this.processingTab.PerformLayout();
            this.simulationParametersTab.ResumeLayout(false);
            this.simulationParametersTab.PerformLayout();
            this.simualtionFileTab.ResumeLayout(false);
            this.simualtionFileTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl simulationTab;
        private System.Windows.Forms.TabPage fileSelectionTab;
        private System.Windows.Forms.CheckBox includeAListOfTrainsToExclude;
        private System.Windows.Forms.CheckBox HunterValley;
        private System.Windows.Forms.TextBox trainListFile;
        private System.Windows.Forms.Button selectTrainFile;
        private System.Windows.Forms.TextBox temporarySpeedRestrictionFile;
        private System.Windows.Forms.Button selectTSRFile;
        private System.Windows.Forms.TextBox GeometryFile;
        private System.Windows.Forms.Button selectGeometryFile;
        private System.Windows.Forms.TextBox IceDataFile;
        private System.Windows.Forms.Button selectDataFile;
        private System.Windows.Forms.TabPage processingTab;
        private System.Windows.Forms.TabPage simulationParametersTab;
        private System.Windows.Forms.TabPage simualtionFileTab;
        private System.Windows.Forms.Label leftLocationLabel;
        private System.Windows.Forms.Label rightLocationLabel;
        private System.Windows.Forms.Label startKmLabel;
        private System.Windows.Forms.Label minDistanceLabel;
        private System.Windows.Forms.Label dataSeparationLabel;
        private System.Windows.Forms.Label timeSeparationLabel;
        private System.Windows.Forms.Label endKmLabel;
        private System.Windows.Forms.Label label3loopBoundarylabel;
        private System.Windows.Forms.Label interpolationLabel;
        private System.Windows.Forms.Label GeographicBoxLabel;
        private System.Windows.Forms.Label DateRangeLabel;
        private System.Windows.Forms.Label leftLongitudeLabel;
        private System.Windows.Forms.Label leftLatitudeLabel;
        private System.Windows.Forms.Label toLabel;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.Label loopSpeedLabel;
        private System.Windows.Forms.Label TSRLabel;
        private System.Windows.Forms.TextBox timeSeparation;
        private System.Windows.Forms.TextBox TSRWindowBoundary;
        private System.Windows.Forms.TextBox dataSeparation;
        private System.Windows.Forms.TextBox loopSpeedThreshold;
        private System.Windows.Forms.TextBox minimumJourneyDistance;
        private System.Windows.Forms.TextBox loopBoundaryThreshold;
        private System.Windows.Forms.TextBox interpolationInterval;
        private System.Windows.Forms.TextBox endInterpolationKm;
        private System.Windows.Forms.TextBox startInterpolationKm;
        private System.Windows.Forms.TextBox toLongitude;
        private System.Windows.Forms.TextBox toLatitude;
        private System.Windows.Forms.TextBox fromLongitude;
        private System.Windows.Forms.TextBox fromLatitude;
        private System.Windows.Forms.DateTimePicker toDate;
        private System.Windows.Forms.DateTimePicker fromDate;
        private System.Windows.Forms.Label combinedDecreasingTrainCount;
        private System.Windows.Forms.Label catagory2DecreasingTrainCount;
        private System.Windows.Forms.Label catagory1DecreasingTrainCount;
        private System.Windows.Forms.Label combinedDecreasingPowerToWeightRatio;
        private System.Windows.Forms.Label catagory2DecreasingPowerToWeightRatio;
        private System.Windows.Forms.Label catagory1DecreasingPowerToWeightRatio;
        private System.Windows.Forms.Label combinedIncreasingTrainCount;
        private System.Windows.Forms.Label catagory2IncreasingTrainCount;
        private System.Windows.Forms.Label catagory1IncreasingTrainCount;
        private System.Windows.Forms.Label combinedIncreasingPowerToWeightRatio;
        private System.Windows.Forms.Label catagory2IncreasingPowerToWeightRatio;
        private System.Windows.Forms.Label catagory1IncreasingPowerToWeightRatio;
        private System.Windows.Forms.Label countLabel2;
        private System.Windows.Forms.Label PWRatioLabel2;
        private System.Windows.Forms.Label countLabel1;
        private System.Windows.Forms.Label PWRatioLabel1;
        private System.Windows.Forms.Button simulationPowerToWeightRatios;
        private System.Windows.Forms.TextBox catagory2UpperBound;
        private System.Windows.Forms.TextBox catagory2LowerBound;
        private System.Windows.Forms.TextBox catagory1UpperBound;
        private System.Windows.Forms.TextBox catagory1LowerBound;
        private System.Windows.Forms.Label powerToWeightLabel;
        private System.Windows.Forms.TextBox simICEDataFile;
        private System.Windows.Forms.Label combinedLabel;
        private System.Windows.Forms.Label simCatagory2Label;
        private System.Windows.Forms.Label simCatagory1Label;
        private System.Windows.Forms.Label decreasingLabel;
        private System.Windows.Forms.Label increasingLabel;
        private System.Windows.Forms.Label upperBoundLabel;
        private System.Windows.Forms.Label lowerBoundLabel;
        private System.Windows.Forms.Label catagory2Label;
        private System.Windows.Forms.Label catagory1Label;
        private System.Windows.Forms.Label dataFileLabel;
        private System.Windows.Forms.Button selectCatagory1DecreasingSimulation;
        private System.Windows.Forms.TextBox catagory1DecreasingSimulationFile;
        private System.Windows.Forms.Button selectCatagory1IncreasingSimulation;
        private System.Windows.Forms.Label catagory1SimualtionLabel;
        private System.Windows.Forms.TextBox catagory1IncreasingSimulationFile;
        private System.Windows.Forms.Button selectCatagory3DecreasingSimulation;
        private System.Windows.Forms.TextBox catagory3DecreasingSimulationFile;
        private System.Windows.Forms.Button selectCatagory3IncreasingSimulation;
        private System.Windows.Forms.Label catagory3SimualtionLabel;
        private System.Windows.Forms.TextBox catagory3IncreasingSimulationFile;
        private System.Windows.Forms.Button selectCatagory2DecreasingSimulation;
        private System.Windows.Forms.TextBox catagory2DecreasingSimulationFile;
        private System.Windows.Forms.Button selectCatagory2IncreasingSimulation;
        private System.Windows.Forms.Label catagory2SimualtionLabel;
        private System.Windows.Forms.TextBox catagory2IncreasingSimulationFile;
        private System.Windows.Forms.Button Execute;
        private System.Windows.Forms.Label executionTime;
        private System.Windows.Forms.Label ExecitionTimeLabel;
        private System.Windows.Forms.TextBox resultsDestination;
        private System.Windows.Forms.Label ResultsDirectoryLabel;
        private System.Windows.Forms.Button resultsDirectory;
        private System.Windows.Forms.Label SimulationP2WRatioLabel;
        private System.Windows.Forms.CheckBox UlanLine;
        private System.Windows.Forms.CheckBox Tarcoola2Kalgoorlie;
        private System.Windows.Forms.CheckBox Melbourne2Cootamundra;
        private System.Windows.Forms.CheckBox Macarthur2Botany;
        private System.Windows.Forms.CheckBox GunnedahBasin;
        private System.Windows.Forms.CheckBox CulleranRanges;
        private System.Windows.Forms.Label TestLabel;
        private System.Windows.Forms.ComboBox Operator1Catagory;
        private System.Windows.Forms.Label CommodityLabel;
        private System.Windows.Forms.Label OperatorLabel;
        private System.Windows.Forms.ComboBox Commodity3Catagory;
        private System.Windows.Forms.ComboBox Operator3Catagory;
        private System.Windows.Forms.ComboBox Commodity2Catagory;
        private System.Windows.Forms.ComboBox Operator2Catagory;
        private System.Windows.Forms.ComboBox Commodity1Catagory;

    }
}

