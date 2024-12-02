using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using Accord.MachineLearning;
using Accord;
using System.Linq;

namespace Video_text
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.CreateDirectory("Videos");
            System.IO.Directory.CreateDirectory("Completed");

            //making a json file to store previous settings and that lot

            videoprocessor Animate = new videoprocessor();

            Animate.main();

            Console.ReadLine();
        }
    }

    class videoprocessor
    {
        string videodirec = Directory.GetCurrentDirectory() + "\\Videos";           //directory for images
        string completeddirec = Directory.GetCurrentDirectory() + "\\Completed";

        //THESE ARE ALL DEFAULT VALUES, IF A JSON IS PRESENT WITH DIFFERENT AND COMPATIBLE SETTINGS IT WILL USE THOSE

        //settings
        int fps = 30;

        int ThreadCount = 6;

        // the default values
        int WidthDe = 230;
        int HeightDe = 80;

        // the values that are used (these are updated)
        int Width = 230;
        int Height = 80;

        // the custom values when experimental mode is on
        int CustomWidth = 230;
        int CustomHeight = 80;

        //bool values for certain modes
        bool DynamicBrightness = true;
        bool ExperimentalMode = false;

        //a list of characters that are displayed on screen
        Char[] SymbolList = {'@','0','#','%','/',';',':','.'};
        

        //the type of colourprocessor
        int colourProcessor = 0;
        string[] processorTypeString = { "Single point rotation", "Floating gradiant", "Double point rotation", "Double point rotation combined" };

        List<MakeImageThreads> ObjectList = new List<MakeImageThreads>(); //this so each thread have their own object 

        List<string> AsciiNumbers = new List<string>();
        //string[] AsciiNumberFile;
                        

        //-------------------------------------

        public videoprocessor()
        {
            //fancy numbers to display fps innit

        //    AsciiNumberFile = File.ReadAllLines("AsciiNumbers.txt");
            
        //    int counter = 0;
        //    string AsciiNumber = "";
        //    foreach (string a in AsciiNumberFile)
        //    {
        //        AsciiNumber += a + "\n";

        //        counter++;

        //        if (counter%5 == 0)
        //        {
        //            //is divisble by 5                    
        //            AsciiNumbers.Add(AsciiNumber);
        //            AsciiNumber = "";
        //        }             
        //    }

        //    Console.WriteLine(AsciiNumbers[2]);
        //    Console.WriteLine(AsciiNumbers[7]);

            Console.ReadLine();           
        }

        public void main()
        {
            for (bool Menu = true; Menu == true;)
            {
                //this checks the setting file for settings
                JsonSettings(false);

                Console.Clear();
                Console.SetCursorPosition(0, 0);

                //settting custom variables if experimental mode is on

                if (ExperimentalMode == true)
                {
                    Width = CustomWidth;
                    Height = CustomHeight;
                }

                else
                {
                    Width = WidthDe;
                    Height = HeightDe;
                }

                Console.WriteLine("--UNICODE VIDEO CONSTRUCTOR--\n");

                Console.WriteLine(" - Play an existing text video (1) \n - Make a new text video (2)\n - Compress a video (3)\n - Settings (4)\n\n - RealTimeCapture <Experimental> (5)");
                Console.Write("Input decision number: ");
                string Menuquery = Console.ReadLine();

                if (Menuquery == "1")
                {
                    this.PlayVideo();
                }

                else if (Menuquery == "2")
                {
                    this.MakeVideo();
                }

                else if (Menuquery == "3")
                {
                    this.CompressVideo(); 
                }

                else if (Menuquery == "4")
                {
                    this.Settings();
                }

                else if (Menuquery == "5")
                {
                    this.RealTimeVideoCapture();
                }

                else
                {
                    Console.WriteLine("bruh");
                }
            }
        }

        public void Settings()
        {          
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(" - Settings - \n");
            Console.WriteLine("Settings Marked experimental only work if experimental mode is on \notherwise they use the default settings\n ");

            Console.WriteLine("Fps (1) \n - Currently: {0} Fps \n", fps);
            Console.WriteLine("Resolution *EXPERIMENTAL* (2) \n - Currently {0}*{1} characters \n", CustomWidth, CustomHeight);
            Console.WriteLine("DynamicBrightness (3) \n - Currently: {0} \n", DynamicBrightness);
            Console.WriteLine("DynamicBrightness Settings *EXPERIMENTAL* (4) \n");
            Console.WriteLine("Thread Count (5) \n - Currently: {0}\n", ThreadCount);
            Console.WriteLine("Experimental Mode (6) \n - Currently : {0}\n", ExperimentalMode);
            Console.WriteLine("< Return to menu - (7) >\n");
            Console.Write("Input decision number: ");

            string MenuQuery = Console.ReadLine();

            if (MenuQuery == "1")
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("\n - Change Fps - ");
                Console.Write("{0} Current Fps | New fps: " ,fps);
                try
                {
                    fps = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Invalid Input!");
                }
            }

            else if (MenuQuery == "2")
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("This will only effect the making of videos and not the resolution they are played \nat unless they are processed in real time");
                Console.WriteLine("The Default values are x: {0} and y: {1}" ,WidthDe ,HeightDe);
                Console.WriteLine("The Custom values are x: {0} and y: {1}" ,CustomWidth ,CustomHeight);

                try
                {
                    Console.Write("Input custom X Resolution: ");
                    CustomWidth = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Input custom Y Resolution: ");
                    CustomHeight = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Invalid Input!");
                }
            }

            else if (MenuQuery == "3")
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("DynamicBrightness adjusts the colour palette depending on whats on the screen\nThis makes it easier to see videos but makes colour less consistent");
                Console.WriteLine("DynamicBrightness is currently {0}", DynamicBrightness);
                Console.WriteLine("and you are currently using: {0}", processorTypeString[colourProcessor]);
                Console.Write("Make true? (y/n): ");
                string MenuQuery3 = Console.ReadLine();

                if (MenuQuery3 == "y")
                {
                    DynamicBrightness = true;
                }
                else if (MenuQuery3 == "n")
                {
                    DynamicBrightness = false;
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }
            }

            else if (MenuQuery == "4")
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                int i = 1;

                Console.WriteLine("This will choose what colour processor you will use");
                Console.WriteLine("Your current processor is {0} \n", processorTypeString[colourProcessor]);

                foreach (string a in processorTypeString)
                {
                    Console.WriteLine("{0} ({1})" , a, i);
                    i++;
                }

                Console.Write("Input decision number: ");
                string menuQuery4 = Console.ReadLine();

                i = 0;
                foreach (string a in processorTypeString)
                {
                    
                    try
                    {
                        if (Convert.ToInt32(menuQuery4) == i+1)
                        {
                            colourProcessor = i;

                            Console.WriteLine("Colour processor is now {0}", processorTypeString[i]);
                            
                        }
                    }

                    catch
                    {
                        Console.WriteLine("Invalid Input");
                    }

                    i++;
                }

                Console.ReadLine();
            }

            else if(MenuQuery == "5")
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("This will determine how many threads are used when making the videos");
                Console.WriteLine("and possibly real time processing in real time");
                Console.Write("ThreadCount: ");
               
                string MenuQuery5 = Console.ReadLine();

                try
                {
                    ThreadCount = Convert.ToInt32(MenuQuery5);
                }
                catch
                {
                    Console.WriteLine("Invalid input!");
                }
            }
                
            else if(MenuQuery == "6")
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("Experimental mode is usually only effective when making new videos");
                Console.WriteLine("Experimental mode will enable custom resolutions and the ability to edit the DynamicBrightness settings \nThis can cause considerable problems");
                Console.WriteLine("Experimental mode is currently {0}", ExperimentalMode);
                Console.Write("Make true? (y/n): ");
                string MenuQuery6 = Console.ReadLine();

                if (MenuQuery6 == "y")
                {
                    ExperimentalMode = true;
                }
                else if (MenuQuery6 == "n")
                {
                    ExperimentalMode = false;
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }
            }

            else if (MenuQuery == "7")
            {
                return;
            }

            else
            {
                Console.WriteLine("Incorrect input!");
            }

            this.JsonSettings(true);

        }

        public void JsonSettings(bool UpdateFile)
        {
            string JsonDirec = System.IO.Directory.GetCurrentDirectory() + "/Settings.json";
            string JsonContents = "";

            //this will hold the values in the object to be saved
            JsonObject JsonVariables = new JsonObject();           

            if (UpdateFile == true)
            {
                //make changes to the json file

                JsonVariables.fps = fps;
                JsonVariables.threads = ThreadCount;
                JsonVariables.customwidth = CustomWidth;
                JsonVariables.customheight = CustomHeight;
                JsonVariables.DynamicBrightness = DynamicBrightness;
                JsonVariables.ExperimentalMode = ExperimentalMode;
                JsonVariables.colourProcessor = colourProcessor;

                File.WriteAllText(JsonDirec, JsonConvert.SerializeObject(JsonVariables));
            }


            else
            {
                //try find a file if not make a new one with the default values
                //this happens when the program starts

                if (File.Exists(JsonDirec) == true)
                {
                    if (File.ReadAllText(JsonDirec) == "")
                    {
                        //file empty

                        JsonVariables.fps = fps;
                        JsonVariables.customwidth = CustomWidth;
                        JsonVariables.customheight = CustomHeight;
                        JsonVariables.DynamicBrightness = DynamicBrightness;
                        JsonVariables.ExperimentalMode = ExperimentalMode;
                        JsonVariables.colourProcessor = colourProcessor;

                        File.WriteAllText(JsonDirec, JsonConvert.SerializeObject(JsonVariables));
                    }

                    else
                    {
                        //file exists and has contents inside

                        try
                        {
                            //updating settings with settings in file

                            JsonContents = File.ReadAllText(JsonDirec);
                            JsonVariables = JsonConvert.DeserializeObject<JsonObject>(JsonContents);
                            fps = JsonVariables.fps;
                            ThreadCount = JsonVariables.threads;
                            CustomWidth = JsonVariables.customwidth;
                            CustomHeight = JsonVariables.customheight;
                            DynamicBrightness = JsonVariables.DynamicBrightness;
                            ExperimentalMode = JsonVariables.ExperimentalMode;
                            colourProcessor = JsonVariables.colourProcessor;
                        }
                        catch
                        {
                            Console.WriteLine("Settings File Corrupted \nmaking new file");
                          
                            JsonVariables.fps = fps;
                            JsonVariables.threads = ThreadCount;
                            JsonVariables.customwidth = CustomWidth;
                            JsonVariables.customheight = CustomHeight;
                            JsonVariables.DynamicBrightness = DynamicBrightness;
                            JsonVariables.ExperimentalMode = ExperimentalMode;
                            JsonVariables.colourProcessor = colourProcessor;

                            File.WriteAllText(JsonDirec, JsonConvert.SerializeObject(JsonVariables));
                        }
                    }
                }

                else
                {
                    //file doesnt exist

                    JsonVariables.fps = fps;
                    JsonVariables.threads = ThreadCount;
                    JsonVariables.customwidth = CustomWidth;
                    JsonVariables.customheight = CustomHeight;
                    JsonVariables.DynamicBrightness = DynamicBrightness;
                    JsonVariables.ExperimentalMode = ExperimentalMode;
                    JsonVariables.colourProcessor = colourProcessor;

                    File.WriteAllText(JsonDirec, JsonConvert.SerializeObject(JsonVariables));
                }
            }

        }

        public void PlayVideo()
        {

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Stopwatch stopwatch = new Stopwatch();           

            int menuQuery = 0;   //video texts to export
            List<string> videos = new List<string>();

            for (bool importmenu = true; importmenu == true;)
            {
                videos = new List<string>();
                string[] videosWpath = Directory.GetFiles(completeddirec);           //finds available images with paths

                for (int i = 0; i < videosWpath.Length; i++)         //parse directories to only obatain file name and ignore any other filetypes like txt and put into list
                {
                    string[] CurrentFile = (((videosWpath[i].Split('\\'))[videosWpath[i].Split('\\').Length - 1]).Split('.')); //file without directory

                    if (CurrentFile[1] == "txt")
                    {
                        videos.Add(CurrentFile[0] + ".txt");
                    }

                    else
                    {

                    }
                }

                Console.WriteLine("--Select Video--\n");

                Console.WriteLine("< Refresh Files - 0 >");

                for (int i = 0; i < videos.Count; i++)
                {
                    Console.WriteLine(videos[i] + " - {0}", i + 1);
                }

                Console.WriteLine("< Return to menu - {0} >", videos.Count + 1);

                Console.Write("\nInput decision number: ");
                menuQuery = Convert.ToInt32(Console.ReadLine());

                if (menuQuery == 0)
                {
                    Console.WriteLine("Refreshed Files\n");
                }

                else if (menuQuery >= videos.Count + 1)
                {
                    Console.WriteLine("Invalid Input");
                }

                else
                {
                    break;
                }

            }

            string VideoDirec = Directory.GetCurrentDirectory() + "\\Completed\\" + videos[menuQuery - 1];

            StreamReader fs = new StreamReader(VideoDirec, Encoding.UTF8);

            //gonna find the meta data by reading characters until it gets to a \n
            //then parsing the string of characters to find the data
            //this way it will continue reading the stream 

            bool metalookup = true;
            string metastring = "";
            string[] metadata = new string[2];

            while (metalookup == true)
            {
                char metaread = (char)fs.Read();

                if (metaread == '\n')
                {
                    metalookup = false;
                }
                else
                {
                    metastring += metaread;
                }
            }

            metadata = metastring.Split(' ');

            int playWidth = 0;
            int playHeight = 0;
            int playFps = 0;
            int totalFrames = 0;
            bool compressed = false;

            try
            {
                playWidth = Convert.ToInt32(metadata[0]);
                playHeight = Convert.ToInt32(metadata[1]);
                playFps = Convert.ToInt32(metadata[2]);
                totalFrames = Convert.ToInt32(metadata[3]);
                if (metadata[4] == "1")
                {
                    compressed = true;
                    Console.WriteLine("Compressed vidoe");
                }
                else
                {
                    compressed = false;
                }
            }
            catch
            {
                Console.WriteLine("Metadata in textfile corrupted, aborting");
                Console.Write(" - Press enter to continue - ");
                Console.ReadLine();
                return;
            }

            float timing = Convert.ToInt32(Math.Round(Convert.ToDouble(1000 / playFps)));    //average time to delay a frame

            Console.WriteLine("Total Frames: {0} \nResolution: {1} x {2} \nFps: {3}", totalFrames, playWidth, playHeight, playFps);
            if (compressed == true)
            {
                Console.WriteLine("-- Compressed Video Type--");
            }

            else
            {

            }

            Console.WriteLine(" - Press enter to play - ");
            Console.ReadLine();

            Console.SetCursorPosition(0, 0);
            Console.Clear();

            string templine = "";
            string number = "";
            int numberout;

            char temp = ' ';
            int pausetime = 0;

            int testint;

            //string Digit;
            //string fpsstring;

            //making the progression bar 
            //int completion = 0;
            //should make a bar that goes across the entire distance of the video playing

            int y = 0;
            int x = 0;

            if (compressed == false)
            {

                for (int i = 0; i < totalFrames; i++)
                {
                    stopwatch.Start();


                    for (y = 0; y < playHeight; y++)
                    {

                        for (x = 0; x < playWidth + 2; x++)
                        {
                            templine += (char)fs.Read();

                        }

                        Console.Write(templine);
                        templine = "";

                    }


                    //old fps display
                    //time it takes to display the text
                    //fpsstring = Convert.ToString(stopwatch.ElapsedMilliseconds);
                    //for (int d = 0; d < fpsstring.Length; d++)
                    //{
                    //    Digit = Convert.ToString(fpsstring[d]);
                    //    Digit = Convert.ToString(AsciiNumbers[Convert.ToInt32(Digit)]);
                    //    Console.WriteLine(Digit);
                    //}

                    Console.WriteLine();

                    //starting to write the progression bar 

                    //Console.Write('|');
                    //completion = Convert.ToInt32(Math.Round((playWidth-2) * (Convert.ToDouble(i) / totalFrames)));

                    //int bar = 0;

                    //for (; bar < completion; bar++)
                    //{
                    //    Console.Write('#');
                    //}

                    //for (; bar < playWidth-2; bar++)
                    //{
                    //    Console.Write(' ');
                    //}
                    //Console.Write("|");

                    //end of the progression bar 

                    stopwatch.Stop();

                    pausetime = Convert.ToInt32(timing - stopwatch.ElapsedMilliseconds);
                    if (pausetime <= 0)
                    {
                        pausetime = 1;
                    }

                    new System.Threading.ManualResetEvent(false).WaitOne(pausetime);

                    Console.SetCursorPosition(0, 0);

                    stopwatch.Reset();
                }
            }

            else
            {
                

                for (int i = 0; i < totalFrames; i++)
                {
                    stopwatch.Start();


                    for (y = 0; y < playHeight; y++)
                    {
                        bool line = true;
                        while (line == true)  //the +2 is for the /r/n at the end of eachline 
                        {
                            temp = (char)fs.Read();

                            if (temp == '\n')
                            {
                                number = "1";
                                line = false;
                            }

                            else if (temp == '\r')
                            {
                                temp = ' ';
                            }
                           
                            bool canConvert = int.TryParse(Convert.ToString(temp) ,out testint);
                            

                            if (canConvert == true)
                            {
                                //is an integer
                                number += temp;
                                //Console.WriteLine("Collecting number {0}  Total {1}", temp ,number);
                                
                                //Console.ReadLine();
                            }
                            else
                            {
                                //is not a integer
                                //Console.WriteLine("Its a symbol!");

                                int.TryParse(number, out numberout);

                                //Console.WriteLine(numberout);

                                for (int a = 0; a < numberout; a++)
                                {
                                    templine += temp;
                                }

                                number = "";

                                //Console.WriteLine(templine);
                                //Console.ReadLine();
                            }
                        }

                        Console.Write(templine);
                        templine = "";

                    }

                    //time it takes to display the text
                    //fpsstring = Convert.ToString(stopwatch.ElapsedMilliseconds);
                    //for (int d = 0; d < fpsstring.Length; d++)
                    //{
                    //    Digit = Convert.ToString(fpsstring[d]);
                    //    Digit = Convert.ToString(AsciiNumbers[Convert.ToInt32(Digit)]);
                    //    Console.WriteLine(Digit);
                    //}

                    stopwatch.Stop();

                    pausetime = Convert.ToInt32(timing - stopwatch.ElapsedMilliseconds);
                    if (pausetime <= 0)
                    {
                        pausetime = 1;
                    }

                    new System.Threading.ManualResetEvent(false).WaitOne(pausetime);

                    Console.SetCursorPosition(0, 0);

                    stopwatch.Reset();
                }
            }

            fs.Close();
        }

        public void MakeVideo()
        {

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            int menuQuery = 0;   //videos to export
            List<string> videos = new List<string>();

            
            for (bool importmenu = true; importmenu == true;)
            {
                string[] videosWpath = Directory.GetFiles(videodirec);           //finds available videos with paths

                for (int i = 0; i < videosWpath.Length; i++)         //parse directories to only obatain file name and ignore any other filetypes like txt and put into list
                {
                    string[] CurrentFile = (((videosWpath[i].Split('\\'))[videosWpath[i].Split('\\').Length - 1]).Split('.'));        //file without directory

                    if (CurrentFile[1] == "mp4")
                    {
                        videos.Add(CurrentFile[0] + "." + CurrentFile[1]);
                    }

                    else
                    {

                    }
                }
                

                Console.WriteLine("\n--Select Video--\n");

                Console.WriteLine("< Refresh Files - 0 >");

                for (int i = 0; i < videos.Count; i++)
                {
                    Console.WriteLine(videos[i] + " - {0}", i + 1);
                }

                Console.WriteLine("< Return to menu - {0} >", videos.Count+1);

                Console.Write("\nInput decision number: ");

                try
                { 
                    menuQuery = Convert.ToInt32(Console.ReadLine());

                    if (menuQuery == 0)
                    {
                        Console.WriteLine("Refreshed Files\n");
                        videos.Clear();
                        Console.Clear();
                        Console.SetCursorPosition(0, 0);
                    }

                    else if (menuQuery == videos.Count + 1)
                    {
                        return; //goes back to menu
                    }

                    else if (menuQuery > videos.Count + 1)
                    {
                        Console.WriteLine("Invalid Input");
                    }

                    else
                    {
                        break;
                    }
                }

                catch
                {
                    Console.WriteLine("Invalid Input!");
                }    
                
            }

            string importDirec = Directory.GetCurrentDirectory() + "\\Videos\\" + videos[menuQuery - 1];

            Console.Write("Input name of file: ");
            string filename = Directory.GetCurrentDirectory() + "\\Completed\\" + Console.ReadLine() + ".txt";

            List<Thread> ThreadList = new List<Thread>();               //IMPORTANTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT THREAD COUNT THINGY

            //just making new threads which get assigned differently 
            //main does nothing when assigned to these threads.

            for (int i = 0; i < ThreadCount; i++)    //Threadcount is found in settings
            {
                ThreadList.Add(new Thread(main));     //MEANS literally nothing the main or does anything in this context/situation. just used to add threads to the list whchi will them ne changed
            }

            var vFreader = new VideoFileReader();
            vFreader.Open(importDirec);
            Double Frames = Convert.ToDouble(vFreader.FrameCount);
            vFreader.Close();

            //figuring out the chunk size for each thread to process 
            //get total frames and divide by chunks round down and then add excess to last chunk

            //using a 2D list to hold the chunk size and the amount it needs to skip

            int ChunkSize = Convert.ToInt32(Math.Floor(Frames / ThreadCount));

            List<List<int>> ChunkSizes = new List<List<int>>();
            int Skipped = 0;

            //mnakingf list of objects for each thread and finding chunk sizes

            ObjectList.Clear();
            for (int i = 0; i < ThreadCount; i++)
            {
                //calculating chunk sizes

                if (i == ThreadCount - 1)
                {
                    //this will calculate the final chunk which also has all the excess frames

                    ChunkSizes.Add(new List<int>());
                    ChunkSizes[i].Add(0);
                    ChunkSizes[i].Add(0);

                    ChunkSizes[i][0] = Convert.ToInt32(Frames) - (ChunkSize * (ThreadCount - 1));
                    ChunkSizes[i][1] = Convert.ToInt32(Frames) - ChunkSizes[i][0];
                }

                else
                {
                    //calcualting chunk sizes and the amoutn to skip for normal chunks

                    ChunkSizes.Add(new List<int>());
                    ChunkSizes[i].Add(0);
                    ChunkSizes[i].Add(0);

                    Skipped = i * ChunkSize;

                    ChunkSizes[i][0] = ChunkSize;
                    ChunkSizes[i][1] = Skipped;
                }


                ObjectList.Add(new MakeImageThreads(Width, Height, DynamicBrightness, ChunkSizes[i], i, SymbolList, colourProcessor));

                //defining the height and widt h of the new video 
                //the size of this specific chunk that its going to process
                //whether dynamic brightness is on
                //the size of the chunk
                //the number/chunk ID
                //and normal chunk size


                //i may reduce it to where it only includes the amount to skip and the amount it needs to do

                Console.WriteLine("I made an object yay!  There skipping {0} Frames! Id never skip a chance with you tho hehe" ,ChunkSizes[i][1]);
            }


            //displaying information
            Console.WriteLine("\nTotal Threads in use {0}", ThreadCount);
            Console.WriteLine("Threads processing {0} frames each with the last thread processing {1} frames extra", ChunkSize, (Convert.ToInt32(Frames) - (ChunkSize * (ThreadCount - 1))));
            Console.WriteLine("Total Frames to process {0}\n", Frames);

            Console.Write("The video is being processed at the resolution {0} x {1} \n using the {2} colour processor     *This cannot be changed* \nContinue? (y/n): ",Width ,Height, processorTypeString[colourProcessor]);

            bool Query = true;
            while (Query == true)
            {
                string ContinueQuery = Console.ReadLine();
                ContinueQuery.ToLower();

                if (ContinueQuery == "y")
                {
                    Console.Write("\n - Press enter to start - ");
                    Console.ReadLine();
                    Query = false;
                }

                else if (ContinueQuery == "n")
                {
                    Console.Write("\n - Aborting process - \n - Press enter to continue - ");
                    Console.ReadLine();
                    return;
                }

                else
                {
                    Console.Write("\n - Invalid input! - \n");
                }
            }         

            for (int currentchunk = 0; currentchunk < ThreadCount; currentchunk++)   //duplicating the video file so that the threads can access data at the same time
            {
                try
                {
                    System.IO.File.Copy(importDirec, Directory.GetCurrentDirectory() + "\\Videos\\tempvideo" + Convert.ToInt32(currentchunk) + ".mp4");
                }
                catch
                {
                    System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\Videos\\tempvideo" + Convert.ToInt32(currentchunk) + ".mp4");

                    System.IO.File.Copy(importDirec, Directory.GetCurrentDirectory() + "\\Videos\\tempvideo" + Convert.ToInt32(currentchunk) + ".mp4");
                }

                Console.WriteLine("I managed all the files u wanted >.< <3");
            }


            int counter = 0;
            foreach (var threadObject in ObjectList)
            {
                ThreadList[counter] = new Thread(new ThreadStart(threadObject.MakeChunk));

                ThreadList[counter].Start();
              
                counter++;

                Console.WriteLine("Yay a thread started :3. Praise me uwu");
            }

            ProgressionCheck();

            foreach (var thread in ThreadList)
            {
                Console.WriteLine("Im making friends just like you asked hehe");
                thread.Join();        
            }
            
            //now need to combine all the text files together

            //meta data of the file at the top line
            string metadata = (Width + " " + Height + " " + fps + " " + Frames + " " + "0" + "\n");  //0 is saying its not compressed 
            File.WriteAllText(filename ,metadata);

            for (int i = 0; i < ThreadCount; i++)
            {
                string file = (Directory.GetCurrentDirectory() + "\\Completed\\"+"tempID" + i + ".txt");
                string[] tempfile = File.ReadAllLines(file);
                File.AppendAllLines(filename ,tempfile);    
                File.Delete(file);
            }
        }

        public void ProgressionCheck()
        {
            string coolbar = "          "; //10 spaces

            bool Incomplete = true;

            //Console.Clear();
            int cursurtop = Console.CursorTop;

            Console.CursorVisible = false;

            while (Incomplete == true)
            {
                Console.SetCursorPosition(0, cursurtop);
                int progress = 0;

                for (int i = 0; i < ThreadCount; i++)
                {
                    //uses the method in each of the objects for multithreading to find the progression
                    progress = ObjectList[i].ProgressionQuery();

                    if (progress == 100)
                    {
                        Console.WriteLine("Thread {0}: {1}% |{2}| Complete!", i, progress, coolbar);
                    }

                    else if (progress == 420)
                    {
                        Console.WriteLine("Thread {0}: 100% |##########| Complete!", i);
                    }

                    else
                    {
                        coolbar = "";

                        //Finding how long to make the bar 
                        for (int barcounter = 0; barcounter < Convert.ToInt32(progress / 10); barcounter++)
                        {
                            coolbar += "#";
                        }

                        for (int coolbarexcess = 0; coolbarexcess < (10 - Convert.ToInt32(progress / 10)); coolbarexcess++)
                        {
                            coolbar += " ";
                        }


                        Console.WriteLine("Thread{0}: {1}% |{2}|", i, progress, coolbar);
                    }

                    System.Threading.Thread.Sleep(20);
                    //thought it would be dumb to be updating all the time
                }               

                if (progress >= 90)
                {
                    Incomplete = false;
                }

                else
                {

                }

                cursurtop = Console.CursorTop - ThreadCount;


                Console.CursorVisible = true;
            }
        } 

        public void CompressVideo()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            int menuQuery = 0;   //video texts to export
            List<string> videos = new List<string>();

            for (bool importmenu = true; importmenu == true;)
            {
                string[] videosWpath = Directory.GetFiles(completeddirec);           //finds available images with paths

                for (int i = 0; i < videosWpath.Length; i++)         //parse directories to only obatain file name and ignore any other filetypes like txt and put into list
                {
                    string[] CurrentFile = (((videosWpath[i].Split('\\'))[videosWpath[i].Split('\\').Length - 1]).Split('.')); //file without directory

                    if (CurrentFile[1] == "txt")
                    {
                        videos.Add(CurrentFile[0] + "." + CurrentFile[1]);
                    }

                    else
                    {

                    }
                }

                Console.WriteLine("--Select Video--\n");

                Console.WriteLine("< Refresh Files - 0 >");

                for (int i = 0; i < videos.Count; i++)
                {
                    Console.WriteLine(videos[i] + " - {0}", i + 1);
                }

                Console.WriteLine("< Return to menu - {0} >", videos.Count + 1);

                Console.Write("\nInput decision number: ");
                menuQuery = Convert.ToInt32(Console.ReadLine());

                if (menuQuery == 0)
                {
                    Console.WriteLine("Refreshed Files\n");
                }

                else if (menuQuery == videos.Count + 1)
                {
                    return;
                }

                else
                {
                    break;
                }

            }

            string VideoLocation = Directory.GetCurrentDirectory() + "\\Completed\\" + videos[menuQuery - 1];

            //basically yeah compress the text
            //it will get the file read each characters until it reaches a /n while counting the values untill one changes and appending to a new file
            //meta data must still be added and new parameter of compressed must be added so that when played it can read it properly
            //all that really needs to input is the direc of the file

            int Lines = File.ReadAllLines(VideoLocation).Length;
            Console.WriteLine("Lines to process; {0}", Lines);

            string SaveLocation = Directory.GetCurrentDirectory() + "\\Completed\\" + (VideoLocation.Split('\\')[VideoLocation.Split('\\').Length - 1]).Split('.')[0] + " - Comp.txt";

            StreamReader FileReader = new StreamReader(VideoLocation ,Encoding.UTF8);

            try
            {
                File.Delete(SaveLocation);
            }
            catch
            {

            }

            //Setting meta data from old to new compressed file

            bool metalookup = true;
            string metastring = "";

            while (metalookup == true)
            {
                char metaread = (char)FileReader.Read();

                if (metaread == '\n')
                {
                    metalookup = false;
                }
                else
                {
                    metastring += metaread;
                }
            }

            metastring = metastring.Split(' ')[0] + " " + metastring.Split(' ')[1] + " " + metastring.Split(' ')[2] + " " + metastring.Split(' ')[3] + " 1";
            

            File.AppendAllText(SaveLocation, metastring);

            //Finsihed meta data append

            //start compression

            char CurrentCharacter = ' ';
            char PreviousCharacter = ' ';
            bool newline = false;
            bool FoundSymbol = false;
            string CurrentLine = "";

            int Counter = 1;

            for (int y = 1; y < Lines-1; y++) //minus one line as one is meta data
            {
                newline = false;
                CurrentCharacter = ' ';
                PreviousCharacter = 'n'; //n just means ignore character before basically this is appropiate when a new line happens or a different symbol
                CurrentLine = "\n";

                //Console.WriteLine("New Line starting");
                while (newline == false)
                {
                    //reading lines

                    FoundSymbol = false;

                    //Console.WriteLine("Finding new Symbol");
                    Counter = 1;

                    while (FoundSymbol == false)
                    {
                        CurrentCharacter = (char)FileReader.Read();

                        if (CurrentCharacter == PreviousCharacter)
                        {
                            //same character ++ to counter
                            Counter++;

                            //Console.WriteLine("Same Character: {0} {1}" ,Counter ,CurrentCharacter);
                            PreviousCharacter = CurrentCharacter;
                        }

                        else if (PreviousCharacter == 'n')  //is previous character as this what its comparing it too
                        {
                            //nothing its just reading the first character of the new symbol
                            //theres probably a better way than using n as a placeholder
                            //Console.WriteLine("Placeholder Previous");
                            //Console.WriteLine("Current Char: {0}", CurrentCharacter);
                            PreviousCharacter = CurrentCharacter;
                        }

                        else if (CurrentCharacter == '\r')
                        {
                            //Console.WriteLine("Ignore");
                        }

                        else if (CurrentCharacter == '\n')
                        {
                            //theres a new line so create symbol for current character and count and reset loop
                            CurrentLine += Convert.ToString(Counter) + PreviousCharacter;
                                                       
                            newline = true;
                            FoundSymbol = true;

                            //Console.WriteLine("End of Line");
                        }

                        else if (CurrentCharacter != PreviousCharacter)
                        {
                            CurrentLine += Convert.ToString(Counter) + PreviousCharacter;
                            //are not the same so new character symbol is added to list
                            //Console.WriteLine("Different Symbol: {0} Test: {1}", CurrentCharacter ,CurrentCharacter);

                            PreviousCharacter = CurrentCharacter;
                            FoundSymbol = true;
                        }

                        else
                        {
                            //Console.WriteLine("What");
                        }

                        //Console.WriteLine("New Previous character set: {0}" ,PreviousCharacter);

                        //Console.ReadLine();
                    }
               
                }

                //Console.WriteLine("Finished Line {0}: {1}", y, CurrentLine);
                //Console.ReadLine();

                File.AppendAllText(SaveLocation ,CurrentLine);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        public void RealTimeVideoCapture()
        {
            //choose video input
            //get frames 
            //convert to video 
            //display

            Console.WriteLine("Choose your device");

            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            int MenuAnswer = 0;

            bool menuQuery = true;
            while (menuQuery == true)
            {
                int counter = 1;
                foreach (var devices in videoDevices)
                {
                    Console.WriteLine("Device {0}: ({1})", videoDevices[counter - 1].Name ,counter);
                    counter++;
                }

                Console.Write("Input your decision number: ");
                string MenuInput = Console.ReadLine();

                try
                {
                    MenuAnswer = Convert.ToInt32(MenuInput);

                    if (MenuAnswer > counter)
                    {
                        Console.WriteLine("Invalid Input!\n");
                    }

                    else
                    {
                        Console.WriteLine("Selected: {0}", videoDevices[MenuAnswer - 1].Name);
                        menuQuery = false;
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid Input!\n");
                }
            }

            var videoSource = new VideoCaptureDevice(videoDevices[MenuAnswer-1].MonikerString);

            VideoCapture LivePlayer = new VideoCapture(videoSource ,Width ,Height, SymbolList, colourProcessor);
        }

       
        public int monoValue(Byte R, Byte G, Byte B)
        {
            int monoValue = Convert.ToInt32((R + G + B) / 3.00);
            return monoValue;
        } 
    }

    public class MakeImageThreads
    {
        private string videolocation;
        private int scalex;
        private int scaley;
        private bool DynamicBrightness;
        private List<int> ChunkInfo;
        private int ChunkID;
        private Double Progression;
        private string ChunkLocation;
        private char[] SymbolList;
        private int colourProcessor;

        //idk why these are private i do not know the advantages

        public MakeImageThreads(int width, int height, bool Dynamic, List<int> chunksize, int ChunkNumber,char[] SymbolListImp, int colourProcessorImp)
        {
            videolocation = Directory.GetCurrentDirectory() + "\\Videos\\" + "tempvideo" + Convert.ToString(ChunkNumber) + ".mp4";
            scalex = width;
            scaley = height;
            DynamicBrightness = Dynamic;
            ChunkInfo = chunksize;
            ChunkID = ChunkNumber;
            SymbolList = SymbolListImp;
            colourProcessor = colourProcessorImp;

            ChunkLocation = Directory.GetCurrentDirectory() + "\\Completed\\" + "tempID" + Convert.ToString(ChunkID) + ".txt";
        }

        public void MakeChunk()
        {
            var vFreader = new VideoFileReader();
            vFreader.Open(videolocation);

            //finding and deleting old chunk file if it exists.
            try
            {
                File.Delete(ChunkLocation);
            }
            catch
            { 

            }

            //making the object for handling colours and symbols
            var ColourManager = new FindSymbol(SymbolList, colourProcessor);

            Bitmap imageBM;

            //skipping frames before the frames that need to be processed if they exist 
            for (int i = 0; i < ChunkInfo[1]; i++)
            {
                imageBM = vFreader.ReadVideoFrame();
                imageBM.Dispose();
            }

            //making list to old finsihed list until it is needed to be saved

            List<string> TextFile = new List<string>();

            for (int a = 0; a < ChunkInfo[0] - 1; a++)
            {
                //Console.WriteLine("\r {0}  ChunkID {1}  Chunk size {2}", a ,ChunkID ,Chunk);

                imageBM = vFreader.ReadVideoFrame();

                if (imageBM == null)
                {
                    break;
                }
                else
                {

                }

                //finding limits of file 
                double imagex = imageBM.Width;
                double imagey = imageBM.Height;

                //colour object
                Color pixel = new Color();
                double MonoBrightness = 0;

                double xFetch = 0;
                double yFetch = 0;

                double xFetchRounded = 0;
                double yFetchRounded = 0;

                //finding the average brightness
                // i am going to collect six spread out points from the image to find the average brightness
                //hopefully this should impact performance that much 

                double brightness = 0;   //this will represent the brightness 0-1
                

                if (DynamicBrightness == true)
                {
                    int brightnessCounter = 0;

                    for (double x = 0.1; x <= 0.9; x = x + 0.1)    //loop for the first pixel the middle pixel and last in y direction
                    {
                        brightnessCounter++;

                        for (double y = 0.1; y <= 0.9; y = y + 0.1) //loop for the different rows
                        {
                            int xint = Convert.ToInt32(Math.Round(x * imagex));
                            int yint = Convert.ToInt32(Math.Round(y * imagey));

                            pixel = imageBM.GetPixel(xint, yint);
                            MonoBrightness = Convert.ToInt32(pixel.R) + Convert.ToInt32(pixel.G) + Convert.ToInt32(pixel.B);
                            MonoBrightness = MonoBrightness / 256;
                            MonoBrightness = MonoBrightness / 3;

                            brightness = brightness + MonoBrightness;
                        }
                    }

                    brightness = brightness / brightnessCounter;
                }
                else
                {
                    brightness = 1F;
                }

                ColourManager.UpdateValues(brightness);

                //obtaining all the pixels for the new image at the new resolution
                //and then assiging the new character for each monovalue

                
                int pixelmono = 0;

                for (float y = 0; y < scaley; y++)
                {

                    string TempLine = "";

                    for (float x = 0; x < scalex; x++)
                    {


                        xFetch = imagex * (x / scalex);
                        yFetch = imagey * (y / scaley);


                        xFetchRounded = Convert.ToInt32(Math.Floor(xFetch));
                        yFetchRounded = Convert.ToInt32(Math.Floor(yFetch));

                        pixel = imageBM.GetPixel(Convert.ToInt32(xFetchRounded), Convert.ToInt32(yFetchRounded));

                        pixelmono = monoValue(pixel.R, pixel.G, pixel.B);

                        TempLine += ColourManager.Symbol(pixelmono);
                    }

                    

                    TextFile.Add(TempLine);
                    //adds finished line                    
                }  // actually making the text things on the screen

                imageBM.Dispose();

                //finds the current progression by doing the framesprocessed/theamount it needs to do 
                Progression = Convert.ToDouble(a) / ChunkInfo[0];
            }
            
            File.WriteAllLines(ChunkLocation,TextFile);
            TextFile.Clear();

            vFreader.Close();
            System.IO.File.Delete(videolocation);
        }

        public int ProgressionQuery()
        {
            return Convert.ToInt32(Progression * 100);
        }

        public int monoValue(Byte R, Byte G, Byte B)
        {
            int monoValue = Convert.ToInt32((R + G + B) / 3.00);
            return monoValue;
        }
    }

    public class VideoCapture
    {
        int counter = 0;
        double brightness = 0.5;
        char[] SymbolList;

        Bitmap Frame;
        VideoCaptureDevice VideoSource;
        FindSymbol ColourManager;
        
        int Width;
        int Height;
        double imagex;
        double imagey;

        int colourProcessor;

        Color pixel = new Color();

        public VideoCapture(VideoCaptureDevice VideoInput, int WidthIn, int HeightIn, char[] SymbolListImp, int colourProcessorImp)
        {          
            VideoSource = VideoInput;
            Width = WidthIn;
            Height = HeightIn;
            SymbolList = SymbolListImp;
            colourProcessor = colourProcessorImp;
            ColourManager = new FindSymbol(SymbolList, colourProcessor);


            File.Delete("Logs.txt");
       
            Console.WriteLine(Width);
            Console.WriteLine(Height);

            Console.ReadLine();

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            VideoSource.Start();
            Thread.Sleep(2000);

            VideoSource.NewFrame += new Accord.Video.NewFrameEventHandler(video_NewFrame);

            Console.Read();

            VideoSource.Stop();
        }

        public void video_NewFrame(object sender, Accord.Video.NewFrameEventArgs EventArgs)
        {
            if (EventArgs.Frame != null)
            {
                

                Console.SetCursorPosition(0, 0);

                Frame = EventArgs.Frame;

                //finding resolution of original image 
                imagex = Frame.Width;
                imagey = Frame.Height;

                double xFetch = 0;
                double yFetch = 0;

                double xFetchRounded = 0;
                double yFetchRounded = 0;

                int pixelmono = 0;

                if (counter % 100 == 0)  //will find new brightness values every 100 Frames
                {
                    brightness = FindBrightness(Frame);
                    ColourManager.UpdateValues(brightness);
                    string adjustedBrightness = Convert.ToString(0.95f - ((0.95f - brightness) / 1.2f));

                    string logtext = "Brightess: " + brightness + "| AdjustedBrightness: " + adjustedBrightness + " | Resolution " + Convert.ToString(Width) + Convert.ToString(Height) + "\n";
                    File.AppendAllText("Logs.txt" , logtext);
                }

                for (float y = 0; y < Height; y++)
                {

                    string TempLine = "";

                    for (float x = 0; x < Width; x++)
                    {
                        xFetch = imagex * (x / Width);
                        yFetch = imagey * (y / Height);

                        xFetchRounded = Convert.ToInt32(Math.Floor(xFetch));
                        yFetchRounded = Convert.ToInt32(Math.Floor(yFetch));

                        pixel = Frame.GetPixel(Convert.ToInt32(xFetchRounded), Convert.ToInt32(yFetchRounded));

                        pixelmono = monoValue(pixel.R, pixel.G, pixel.B);

                        TempLine += ColourManager.Symbol(pixelmono);
                    }

                    Console.WriteLine(TempLine);
                    //adds finished line                    
                }   //this is processing the frames

                double barlength = imagex;
                Console.WriteLine();
                Console.Write("|");
                int brightnessAmount = Convert.ToInt32(Math.Round(brightness * (barlength - 2d)));
                for (int x = 0; x < Width-2; x++)
                {   
                    if (x <= brightnessAmount)
                    {
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine("|");

                foreach (int x in ColourManager.adjustedvalues)
                {
                    Console.Write("{0} ", x);
                }
                Console.WriteLine("| Brightness {0}\ncolour parametres\n\nDouble line colour processor visualiser", brightness);

                //this is where i make a cool graph
                //each line is going to equal 5 and each space will equal five

                if (counter % 100 == 0)
                {
                    int line1;
                    int line2;

                    List<List<string>> graph = new List<List<string>>();

                    for (int y = 0; y <= 51; y += 1)
                    {
                        graph.Add(new List<string>());

                        for (int x = 0; x <= 51; x += 1)
                        {
                            graph[y].Add(" ");
                        }
                    }

                    for (int x = 0; x <= 255; x += 5)
                    {
                        line1 = Convert.ToInt32(Math.Round((x - 255d) * (2d * (1 - brightness)) + 255d));
                        line2 = Convert.ToInt32(Math.Round(x * (2d * brightness)));

                        line1 = Convert.ToInt32(Math.Round(line1 / 5d));
                        line2 = Convert.ToInt32(Math.Round(line2 / 5d));   //this will make it a multiple of 5 and i wont need to dividde it again in the index

                        if (line1 <= 0)
                        {
                            line1 = 0;
                        }
                        if (line2 <= 0)
                        {
                            line2 = 0;
                        }

                        if (line1 > 51)
                        {
                            line1 = 51;
                        }
                        if(line2 > 51)
                        {
                            line2 = 51;
                        }

                        graph[Math.Abs(line1 - 51)][x/5] = "#";
                        graph[Math.Abs(line2 - 51)][x/5] = "#";
                    }

                    foreach (List<string> a in graph)
                    {
                        foreach (string s in a)
                        {
                            Console.Write(s);
                        }
                        Console.WriteLine();
                    }
                }

                Frame.Dispose();

                try
                {
                    counter++;
                }
                catch //just in case overflow error after running for long periods
                {
                    counter = 0;
                }
            }

            else
            {
                Console.WriteLine("nothing bro");
            }
        }

        public double FindBrightness(Bitmap Frame)
        {
            double imagex = Frame.Width;
            double imagey = Frame.Height;
            int LocalCounter = 1;

            //colour object
            Color pixel = new Color();
            double MonoBrightness = 0;

            for (double x = 0.1; x <= 0.9; x = x + 0.1)    //loop for the first pixel the middle pixel and last in y direction
            {
                for (double y = 0.1; y <= 0.9; y = y + 0.1) //loop for the different rows
                {
                    LocalCounter++;

                    int xint = Convert.ToInt32(Math.Round(x * imagex));
                    int yint = Convert.ToInt32(Math.Round(y * imagey));

                    pixel = Frame.GetPixel(xint, yint);
                    MonoBrightness = Convert.ToInt32(pixel.R) + Convert.ToInt32(pixel.G) + Convert.ToInt32(pixel.B);
                    MonoBrightness = MonoBrightness / 3;
                    MonoBrightness = MonoBrightness / 255;

                    brightness = brightness + MonoBrightness;                
                }
            }
            brightness = brightness / LocalCounter;

            return brightness;
        }

        public int monoValue(Byte R, Byte G, Byte B)
        {
            int monoValue = Convert.ToInt32((R + G + B) / 3.00);
            return monoValue;
        }
    }

    public class FindSymbol
    {
        public double brightness;
        public List<int> adjustedvalues = new List<int>();
        public List<int> defaultvalues = new List<int>();

        char[] SymbolList;
        int colourProcessor;

        public FindSymbol(char[] SymbolListImp, int colourProcessorImp)
        {
            SymbolList = SymbolListImp;
            colourProcessor = colourProcessorImp;

            //setting the default brightness values 
            //using 255 as the max as there cannot be anything higher than 256

            for (double i = SymbolList.Length; i > 0; i--)
            {
                double temp = Math.Round(255d * (i / SymbolList.Length));
                defaultvalues.Add(Convert.ToInt32(temp));
              
            }

            for (int i = SymbolList.Length; i > 0; i--)
            {
                adjustedvalues.Add(255);
            }
        }

        public void UpdateValues(double brightnessInp)
        {
            brightness = brightnessInp;
            bool top;

            //this is for the double line method
            if (brightness > 0.5)
            {
                top = true;
            }
            else
            {
                top = false;
            }

            for (int i = 0; i < defaultvalues.Count; i++)
            {
                if (colourProcessor == 0)
                {
                    //dynamic brightness method
                    brightness = 0.95F - ((0.95F - brightness) / 1.2);
                    
                    adjustedvalues[i] = Convert.ToInt32(defaultvalues[i] * brightness);                    
                }

                else if (colourProcessor == 1)
                {
                    //floating line method
                    double temp;

                    temp = Math.Round((defaultvalues[i] * 0.6) + (102 * brightness));
                    adjustedvalues[i] = Convert.ToInt32(temp);
                }
                

                else if (colourProcessor == 2)
                {
                    //double line method
                    if (top == true)
                    {
                        adjustedvalues[i] = Convert.ToInt32(Math.Round((defaultvalues[i] - 255d) * (2d * (1 - brightness)) + 255d));
                    }
                    //this method uses 2 different lines that pivot on different points one being 0,0 for brightness below 0.5 and the other 255,255 for brightness above 
                    //0.5
                    else
                    {
                        adjustedvalues[i] = Convert.ToInt32(Math.Round(defaultvalues[i] * (2d * brightness)));
                    }
                }

                else if (colourProcessor == 3)
                {
                    //double line method combined
                    //this method is very simlar but instead it combines the 2 lines 
                    //this may result in better contrast but may also reduce clarity in primarily dark or light enviroments 
                    //suited towards videos with a changing amount of light and lots of contrast

                    if (defaultvalues[i] > 128) // this is the half way mark between all colours this will use the lower equation aka bottom or not top
                    {
                        adjustedvalues[i] = Convert.ToInt32(Math.Round((defaultvalues[i] - 255d) * (2d * (1 - brightness)) + 255d));
                    }
                    else
                    {
                        adjustedvalues[i] = Convert.ToInt32(Math.Round(defaultvalues[i] * (2d * brightness)));
                    }
                }
                
            }
        }

        public char Symbol(int value)
        {
            for (int i = 0; i < SymbolList.Length; i++)
            {    
                if (value > adjustedvalues[i])
                {
                    return SymbolList[i];
                }

                else
                {

                }
            }

            return ' ';
        }
    }

    public class JsonObject
    {
        public int fps { get; set; }
        public int threads { get; set; }

        public int customwidth { get; set; }
        public int customheight { get; set; }

        public bool DynamicBrightness { get; set; }
        public bool ExperimentalMode { get; set; }

        public int colourProcessor { get; set; }
    }
}
