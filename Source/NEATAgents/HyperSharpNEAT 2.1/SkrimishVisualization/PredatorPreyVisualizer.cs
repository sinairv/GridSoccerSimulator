using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpNeatLib.Experiments;
using SharpNeatLib.Xml;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeuralNetwork;
using System.Xml;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.CPPNs;
using System.IO;
using SharpNeatLib.Evolution;

namespace SkirmishVisualization
{
    public partial class Form1 : Form
    {

        World w=new World();
        bool drawPie = true;
        bool isMulti = true;
        INetwork network=null;
        NeatGenome seedGenome = null;
        SkirmishSubstrate substrate;
        int timer = 0;

        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.Size = new Size((int)w.height, (int)w.width);
            SkirmishExperiment.multiple = true;
            isMulti = true;
            this.toggleMultiToolStripMenuItem.Checked = isMulti;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            w.drawWorld(e.Graphics,drawPie);
            e.Graphics.DrawString(timer.ToString(), new Font("Tahoma", 40), Brushes.Black, 50, 50);
            string s="";
            if (isMulti && w.bigBrain != null)
            {
                for (int j = 0; j < 5; j++)
                {

                    s += w.bigBrain.GetOutputSignal(j * 3);
                    s += " " + w.bigBrain.GetOutputSignal(j * 3 + 1);
                    s += " " + w.bigBrain.GetOutputSignal(j * 3 + 2);
                    s += Environment.NewLine;
                }
            }
            e.Graphics.DrawString(s, new Font("Tahoma", 12), Brushes.Black, 50, 100);
           
            //this.menuStrip1.Visible = false;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'm')
                drawPie = !drawPie;
            else if (e.KeyChar == '/')
            {
                if (isMulti)
                    w.goMulti(100);
                else
                    w.go(100);

                timer += 100;
            }
            else if (e.KeyChar == '1')
            {
                w = SkirmishNetworkEvaluator.world1(network);
                timer = 0;
            }
            else if (e.KeyChar == '2')
            {
                w = SkirmishNetworkEvaluator.world2(network);
                timer = 0;
            }
            else if (e.KeyChar == '3')
            {
                w = SkirmishNetworkEvaluator.world3(network);
                timer = 0;
            }
            else if (e.KeyChar == '4')
            {
                w = SkirmishNetworkEvaluator.world4(network);
                timer = 0;
            }
            else if (e.KeyChar == '5')
            {
                w = SkirmishNetworkEvaluator.world5(network);
                timer = 0;
            }
            else if (e.KeyChar == 'q')
            {
                w = SkirmishNetworkEvaluator.pointWorldVar(network, 3*(float)Math.PI / 8.0f);
                timer = 0;
            }
            else if (e.KeyChar == 'w')
            {
                w = SkirmishNetworkEvaluator.pointWorldVar(network, (float)Math.PI/4.0f);
                timer = 0;
            }
            else if (e.KeyChar == 'e')
            {
                w = SkirmishNetworkEvaluator.pointWorldVar(network, (float)Math.PI / 8.0f);
                timer = 0;
            }
            else if (e.KeyChar == 'a')
            {
                w = SkirmishNetworkEvaluator.diamondWorldVar(network, 75);
                timer = 0;
            }
            else if (e.KeyChar == 's')
            {
                w = SkirmishNetworkEvaluator.diamondWorldVar(network, 100);
                timer = 0;
            }
            else if (e.KeyChar == 'd')
            {
                w = SkirmishNetworkEvaluator.diamondWorldVar(network, 125);
                timer = 0;
            }
            else if (e.KeyChar == 'z')
            {
                w = SkirmishNetworkEvaluator.squareWorldVar(network, 75);
                timer = 0;
            }
            else if (e.KeyChar == 'x')
            {
                w = SkirmishNetworkEvaluator.squareWorldVar(network, 100);
                timer = 0;
            }
            else if (e.KeyChar == 'c')
            {
                w = SkirmishNetworkEvaluator.squareWorldVar(network, 125);
                timer = 0;
            }
            else if (e.KeyChar == 'r')
            {
                w = SkirmishNetworkEvaluator.lWorldVar(network, 75);
                timer = 0;
            }
            else if (e.KeyChar == 't')
            {
                w = SkirmishNetworkEvaluator.lWorldVar(network, 100);
                timer = 0;
            }
            else if (e.KeyChar == 'y')
            {
                w = SkirmishNetworkEvaluator.lWorldVar(network, 125);
                timer = 0;
            }
            else
            {
                if (isMulti)
                    w.timeStepMulti();
                else
                    w.timeStep();
                timer++;
            }
                
            Invalidate();
        }


        private void start100ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlayN(100);
        }

        private void play1000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayN(1000);
        }

        private void play2000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayN(2000);
        }

        private void play10000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayN(10000);
        }

        private void PlayN(int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (isMulti)
                    w.timeStepMulti();
                else
                    w.timeStep();
                timer++;

                Invalidate();

                Application.DoEvents();
            }
        }


        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            w.addEnemy(new Prey(e.X, e.Y, w.agentSize, w.agentSize));
            Invalidate();
        }


        private void loadGenomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory; 
            // Path.Combine(Environment.CurrentDirectory, @"..\..\..\SeedGenomes"); 

            DialogResult res = openFileDialog1.ShowDialog(this);
            if (res == DialogResult.OK  || res==DialogResult.Yes)
            {
                string filename = openFileDialog1.FileName;
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filename);
                    seedGenome = XmlNeatGenomeReaderStatic.Read(doc);
                    setupSubstrate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }

        private void loadCCEANeatGenomesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                dlg.Filter = "*.xml|*.xml";
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                var fileNames = dlg.FileNames;
                if (fileNames.Length != 5)
                {
                    MessageBox.Show("You must select 5 files!");
                    return;
                }

                Array.Sort(fileNames);

                INetwork[] predNetworks = new INetwork[5];

                for (int i = 0; i < predNetworks.Length; i++)
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(fileNames[i]);
                        var genome = XmlNeatGenomeReaderStatic.Read(doc);
                        predNetworks[i] = genome.Decode(HyperNEATParameters.substrateActivationFunction);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }

                this.isMulti = false;
                SkirmishNetworkEvaluator.NetworkPerPred = predNetworks;
            }
        }



        private void loadCCEAGenomesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                dlg.Filter = "*.xml|*.xml";
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                var fileNames = dlg.FileNames;
                if (fileNames.Length != 5)
                {
                    MessageBox.Show("You must select 5 files!");
                    return;
                }

                Array.Sort(fileNames);

                INetwork[] predNetworks = new INetwork[5];

                for (int i = 0; i < predNetworks.Length; i++)
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(fileNames[i]);
                        var genome = XmlNeatGenomeReaderStatic.Read(doc);

                        var curSubstrate = new SkirmishSubstrate(5, 3, 5, HyperNEATParameters.substrateActivationFunction);
                        predNetworks[i] = curSubstrate.GenerateGenome(genome.Decode(null)).Decode(null);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }

                this.isMulti = false;
                SkirmishNetworkEvaluator.NetworkPerPred = predNetworks;
            }
        }


        private void toggleMultiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (seedGenome == null)
            {
                MessageBox.Show("You must first load a genome!");
                return;
            }

            isMulti = !isMulti;
            SkirmishExperiment.multiple = isMulti;
            toggleMultiToolStripMenuItem.Checked = isMulti;
            setupSubstrate();
            
        }

        private void setupSubstrate()
        {
            if (isMulti)
            {
                substrate = new SkirmishSubstrate(25, 15, 25, HyperNEATParameters.substrateActivationFunction);
                network = substrate.generateMultiGenomeModulus(seedGenome.Decode(null), 5).Decode(null);
            }
            else
            {
                substrate = new SkirmishSubstrate(5, 3, 5, HyperNEATParameters.substrateActivationFunction);
                network = substrate.GenerateGenome(seedGenome.Decode(null)).Decode(null);
            }
        }

        // Attempting to output to folder
        private string EvoOutputLogFolder = "";

        // Attempting to use seed from file
        private string EvoSeedFileName = null;

        // Attempting to do experiment with shape
        private string EvoShape = "triangle";

        //Experiment is heterogeneous?
        private bool EvoIsMulti = false;

        
        private void startEvolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NeatGenome seedGenome = null;

            if (EvoSeedFileName != null)
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(EvoSeedFileName);
                    seedGenome = XmlNeatGenomeReaderStatic.Read(document);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Problem loading genome. \n" + ex.Message);
                }
            }

            double maxFitness = 0;
            int maxGenerations = 1000;
            int populationSize = 150;
            int inputs = 4;
            IExperiment exp = new SkirmishExperiment(inputs, 1, EvoIsMulti, EvoShape);

            w = SkirmishNetworkEvaluator.lWorldVar(network, 125);
            timer = 0;

            StreamWriter SW;
            SW = File.CreateText(EvoOutputLogFolder + "logfile.txt");
            XmlDocument doc;
            FileInfo oFileInfo;
            IdGenerator idgen;
            EvolutionAlgorithm ea;
            if (seedGenome == null)
            {
                idgen = new IdGenerator();
                ea = new EvolutionAlgorithm(
                    new Population(idgen, 
                        GenomeFactory.CreateGenomeList(
                            exp.DefaultNeatParameters, idgen, 
                            exp.InputNeuronCount, exp.OutputNeuronCount, 
                            exp.DefaultNeatParameters.pInitialPopulationInterconnections, 
                            populationSize)),
                    exp.PopulationEvaluator, exp.DefaultNeatParameters);
            }
            else
            {
                idgen = new IdGeneratorFactory().CreateIdGenerator(seedGenome);
                ea = new EvolutionAlgorithm(new Population(idgen, GenomeFactory.CreateGenomeList(seedGenome, populationSize, exp.DefaultNeatParameters, idgen)), exp.PopulationEvaluator, exp.DefaultNeatParameters);
            }
            for (int j = 0; j < maxGenerations; j++)
            {
                DateTime dt = DateTime.Now;
                ea.PerformOneGeneration();
                if (ea.BestGenome.Fitness > maxFitness)
                {
                    maxFitness = ea.BestGenome.Fitness;
                    doc = new XmlDocument();
                    XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome);
                    oFileInfo = new FileInfo(EvoOutputLogFolder + "bestGenome" + j.ToString() + ".xml");
                    doc.Save(oFileInfo.FullName);

                    // This will output the substrate, uncomment if you want that
                    /* doc = new XmlDocument();
                     XmlGenomeWriterStatic.Write(doc, (NeatGenome) SkirmishNetworkEvaluator.substrate.generateMultiGenomeModulus(ea.BestGenome.Decode(null),5));
                     oFileInfo = new FileInfo(folder + "bestNetwork" + j.ToString() + ".xml");
                     doc.Save(oFileInfo.FullName);
                     */


                }
                Console.WriteLine(ea.Generation.ToString() + " " + ea.BestGenome.Fitness + " " + (DateTime.Now.Subtract(dt)));
                //Do any post-hoc stuff here

                SW.WriteLine(ea.Generation.ToString() + " " + (maxFitness).ToString());

            }
            SW.Close();

            doc = new XmlDocument();
            XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, ActivationFunctionFactory.GetActivationFunction("NullFn"));
            oFileInfo = new FileInfo(EvoOutputLogFolder + "bestGenome.xml");
            doc.Save(oFileInfo.FullName);
        }

        
    }
}