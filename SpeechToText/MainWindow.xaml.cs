#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Windows;
using System.Diagnostics;
using System.Text;
using System.IO.Ports;
using System.ComponentModel;
using System.Data;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Timers;
#endregion



namespace SpeechToText
{
    /// <summary>
    /// MainWindow class
    /// </summary>
  
    public partial class MainWindow : Window
    {

        static Boolean first = true;

        static HeartRateInfo last = null;

        private static Task ReceiveFromServerAsync()
        {
            var dbContext = new IvaDatabase2Entities();


            SpeechSynthesizer reader_warning = new SpeechSynthesizer();

            if (last == null)
            {
                first = false;
                last = dbContext.HeartRateInfo.OrderByDescending(hr => hr.Id).FirstOrDefault();

                if (last.heartRate > 100 || last.heartRate < 70)
                {
                    Console.WriteLine(last.heartRate);
                    Console.WriteLine(dbContext.HeartRateInfo.OrderByDescending(hr => hr.Id).FirstOrDefault().heartRate);
                    reader_warning.SpeakAsync("Warning! Core condition in danger!");
                }
            }


               if( dbContext.HeartRateInfo.OrderByDescending(hr => hr.Id).FirstOrDefault().heartRate != last.heartRate)
            {
                last = dbContext.HeartRateInfo.OrderByDescending(hr => hr.Id).FirstOrDefault();

                if (last.heartRate > 100 || last.heartRate < 70)
                {
                    Console.WriteLine(last.heartRate);
                    Console.WriteLine(dbContext.HeartRateInfo.OrderByDescending(hr => hr.Id).FirstOrDefault().heartRate);
                    reader_warning.Speak("Warning! Core condition in danger!");
                }

                Console.WriteLine(last.heartRate);
            }



               Console.ReadLine();
            return new Task(new Action(pula));
       }
        
        static void pula()
        {

        }


        /// <summary>
        /// the engine
        /// </summary>
        SpeechRecognitionEngine speechRecognitionEngine = null;

        /// <summary>
        /// list of predefined commands
        /// </summary>
        List<Word> words = new List<Word>();





        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += async (sender, e) => await ReceiveFromServerAsync();
            timer.Start();
            InitializeComponent();

            initializeEngine("//example.txt");
        }

        private void initializeEngine(String file)
        {
            try
            {
                // create the engine
                speechRecognitionEngine = createSpeechEngine("en-US");

                // hook to events
                speechRecognitionEngine.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(engine_AudioLevelUpdated);
                if (file.Equals("//example.txt"))
                {
                    speechRecognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(engine_SpeechRecognized);
                }
                else
                {
                    speechRecognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(boolean_SpeechRecognized);
                }
                // load dictionary
                loadGrammarAndCommands(file);

                // use the system's default microphone
                speechRecognitionEngine.SetInputToDefaultAudioDevice();

                // start listening
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Voice recognition failed");
            }
        }





        /// <summary>
        /// Creates the speech engine.
        /// </summary>
        /// <param name="preferredCulture">The preferred culture.</param>
        /// <returns></returns>
        private SpeechRecognitionEngine createSpeechEngine(string preferredCulture)
        {
            foreach (RecognizerInfo config in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (config.Culture.ToString() == preferredCulture)
                {
                    speechRecognitionEngine = new SpeechRecognitionEngine(config);
                    break;
                }
            }

            // if the desired culture is not found, then load default
            if (speechRecognitionEngine == null)
            {
                MessageBox.Show("The desired culture is not installed on this machine, the speech-engine will continue using "
                    + SpeechRecognitionEngine.InstalledRecognizers()[0].Culture.ToString() + " as the default culture.",
                    "Culture " + preferredCulture + " not found!");
                speechRecognitionEngine = new SpeechRecognitionEngine(SpeechRecognitionEngine.InstalledRecognizers()[0]);
            }

            return speechRecognitionEngine;
        }

        /// <summary>
        /// Loads the grammar and commands.
        /// </summary>
        private void loadGrammarAndCommands(String file)
        {
            try
            {
                Choices texts = new Choices();
                string[] lines = File.ReadAllLines(Environment.CurrentDirectory + file);
                foreach (string line in lines)
                {
                    // skip commentblocks and empty lines..
                    if (line.StartsWith("--") || line == String.Empty) continue;

                    // split the line
                    var parts = line.Split(new char[] { '|' });

                    // add commandItem to the list for later lookup or execution
                    words.Add(new Word() { Text = parts[0], AttachedText = parts[1], IsShellCommand = (parts[2] == "true") });

                    // add the text to the known choices of speechengine
                    texts.Add(parts[0]);
                }
                Grammar wordsList = new Grammar(new GrammarBuilder(texts));
                speechRecognitionEngine.LoadGrammar(wordsList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        Boolean aborted = false;
        /// <summary>
        /// Gets the known command.
        /// </summary>
        /// <param name="command">The order.</param>
        /// <returns></returns>
        private string getKnownTextOrExecute(string command)
        {
            try
            {
                var cmd = words.Where(c => c.Text == command).First();
  
                if (cmd.IsShellCommand)
                {
                    Process proc = new Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = cmd.AttachedText;
                    proc.Start();
                    return "you just started : " + cmd.AttachedText;
                }
                else
                {
                    SerialPort port;
                    SpeechSynthesizer reader = new SpeechSynthesizer();
                    String s = cmd.AttachedText;
                    String s1 = cmd.Text;

                    port = new SerialPort("COM3", 9600);
                    port.Open();       

                    Console.Write(s);
                    if (!aborted)
                        moveCommands(port, s, reader);
                    if (s.Equals("Hello, my name is Iva!"))
                    {
                        reader.SpeakAsync(s);
                        aborted = false;
                    }
                    if (s.Equals("Shutting down the movement system"))
                    {
                        reader.SpeakAsync(s);
                        aborted = true;
                    }
                    if (s1.Equals("Iva do you love me"))
                    {
                        reader.SpeakAsync(s);
                        
                    }
                    if(s1.Equals("Iva who are you"))
                    {
                        reader.SpeakAsync(s);
                    }
                    if (s1.Equals("Iva tell them"))
                    {
                        reader.SpeakAsync(s);
                    }
                    if (s.Equals("Do you want me to open skype.exe?"))
                    {
                        waitForResponse();
                        reader.SpeakAsync(s);
                        Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaa");

                        //   return "Do you want me to open Skype?";
                    }
                    if(s.Equals("Closing firefox"))
                    {
                        

                        foreach (Process proc in Process.GetProcessesByName("firefox")) {
                            proc.Kill();
                            proc.WaitForExit();
                        }
                    }                    
                port.Close();

                    return cmd.AttachedText;
                }
            }
            catch (Exception)
            {
                return command;
            }
        }

        private void moveCommands(SerialPort port, String s, SpeechSynthesizer reader)
        {
            if (s.Equals("go"))
            {
                reader.SpeakAsync("Engine started");
                port.Write("w" + '\n');

            }
            if (s.Equals("back"))
            {
                port.Write("s" + '\n');
            }
            if (s.Equals("left"))
            {
                port.Write("a" + '\n');
            }
            if (s.Equals("right"))
            {
                port.Write("d" + '\n');
            }
            if (s.Equals("stop"))
            {
                port.Write("o" + '\n');
                reader.SpeakAsync("Engine Stopped");
            }
            if (s.Equals("half right"))
            {
                port.Write("e" + "\n");
                port.Write("o" + "\n");
            }
            if (s.Equals("half left"))
            {
                port.Write("q" + "\n");
                port.Write("o" + "\n");
            }
        }

        private void waitForResponse()
        {
            speechRecognitionEngine.RecognizeAsyncStop();
            speechRecognitionEngine.Dispose();
            initializeEngine("//booleans.txt");
          
        }

        private void returnToExample()
        {
            speechRecognitionEngine.RecognizeAsyncStop();
            speechRecognitionEngine.Dispose();
            initializeEngine("//example.txt");

        }



        /// <summary>
        /// Handles the SpeechRecognized event of the engine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Speech.Recognition.SpeechRecognizedEventArgs"/> instance containing the event data.</param>
        void engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            txtSpoken.Text += "\r" + getKnownTextOrExecute(e.Result.Text);
            scvText.ScrollToEnd();
        }

        private void boolean_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            txtSpoken.Text += "\r" + getAnswer(e.Result.Text);
            Console.WriteLine("In method boolean_SpeechRecognized");
            scvText.ScrollToEnd();
        }

        private String getAnswer(String command)
        {
            try
            {
                var cmd = words.Where(c => c.Text == command).First();

                String s = cmd.Text;
                Boolean started = false;
                if (s.Equals("yes"))
                {
                    Console.Write("yes");
                    Process proc = new Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = "skype.exe";
                    proc.Start();
                    started = true;
                }
                else
                {
                    Console.Write("no");
                }


                returnToExample();
                if (started)
                    return "you just started " + "skype";
                else
                    return s;

            }
            catch (Exception)
            {
                return command;
            }
        }

        /// <summary>
        /// Handles the AudioLevelUpdated event of the engine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Speech.Recognition.AudioLevelUpdatedEventArgs"/> instance containing the event data.</param>
        void engine_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            prgLevel.Value = e.AudioLevel;
        }


        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // unhook events
            speechRecognitionEngine.RecognizeAsyncStop();
            // clean references
            speechRecognitionEngine.Dispose();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
