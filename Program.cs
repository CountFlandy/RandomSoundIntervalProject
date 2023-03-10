namespace RandomSountScareProject
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Media;

    /// <summary>
    /// This is a tiny personal project that has a simple premise
    /// 1: Generate a list of 10 increments of time ranging from 30 seconds to 2 hours
    /// 2: After the time has passed play a random audio file from the SoundFiles directory.
    /// 3: Then generate a new increment of time, and wait until it has passed and repeat.
    /// </summary>


    //I have no idea why I wanted to write this, but I did.
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Count Flandy's Random Sound Player Thing. I don't know, it just plays a random soundfile you provide after a random time.");
            Console.WriteLine("Select the min/max amount of time between plays.");
            Console.WriteLine("Please type ONLY the corresponding letter. IE: A");
            Console.WriteLine("A: 30 Seconds/2 Hours (Default)");
            Console.WriteLine("B: 30 Seconds/1 Hour");
            Console.WriteLine("C: 30 Seconds/30 Minutes");
            Console.WriteLine("D: 30 Seconds/10 Minutes");
            Console.WriteLine("E: 1 Hour/4 Hours");
            Console.WriteLine("F: 4 Hours/8 Hours");
            Console.WriteLine("G: 30 Seconds/30 Seconds");
            string userAnswer = Console.ReadLine();
            Console.WriteLine("Option " + userAnswer + "selected");

            // Define the range of time intervals based on user input
            int minSeconds, maxSeconds;
            switch (userAnswer.ToUpperInvariant())
            {
                case "A":
                    minSeconds = 30;
                    maxSeconds = 7200;
                    break;
                case "B":
                    minSeconds = 30;
                    maxSeconds = 3600;
                    break;
                case "C":
                    minSeconds = 30;
                    maxSeconds = 1800;
                    break;
                case "D":
                    minSeconds = 30;
                    maxSeconds = 600;
                    break;
                case "E":
                    minSeconds = 3600;
                    maxSeconds = 14400;
                    break;
                case "F":
                    minSeconds = 14400;
                    maxSeconds = 28800;
                    break;
                case "G":
                    minSeconds = 30;
                    maxSeconds = 30;
                    break;
                default:
                    minSeconds = 30;
                    maxSeconds = 7200;
                    break;
            }


            // Generate an array of 10 random increments of time within the defined range
            var random = new Random();
            var timeIncrements = Enumerable.Range(0, 10)
                .Select(i => TimeSpan.FromSeconds(random.Next(minSeconds, maxSeconds)))
                .ToArray();

            // Check for Soundfiles folder and create it if it doesn't exist
            var soundFilesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SoundFiles");
            if (!Directory.Exists(soundFilesDirectory))
            {
                Directory.CreateDirectory(soundFilesDirectory);
            }

            // Check if valid audio files exist in the Soundfiles directory
            var validAudioExtensions = new[] { ".wav", ".mp3", ".wma", ".mid", ".midi", ".aiff", ".aif", ".m4a" };
            var validAudioFiles = Directory.GetFiles(soundFilesDirectory)
                .Where(file => validAudioExtensions.Contains(Path.GetExtension(file)))
                .ToList();

            if (validAudioFiles.Count == 0)
            {
                Console.WriteLine("Warning: No valid audio files found in SoundFiles directory.");
                Console.WriteLine("Program will now stop. Please place valid audio files into the SoundFiles directory to start.");
                Console.WriteLine("Valid formats are: .wav, .mp3, .wma, .mid, .midi, .aiff, .aif, .m4a");
                Console.WriteLine("If you have a file that is an audio file in a different format than these please open up a Github issue with an example of this audio format.");
                Console.ReadLine();
                return;
            }
            
            // Now generate and start a new timer
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            var currentIndex = 0;
            while (true)
            {
                var nextIncrement = timeIncrements[currentIndex];
                if (timer.Elapsed >= nextIncrement)
                {
                    // Reset the timer back to 0
                    timer.Restart();
                    Console.WriteLine("You've been spooked! This increment of time was:" + nextIncrement);
                    // Generate a new random increment of time and add it to the end of the array
                    var newTimeIncrement = TimeSpan.FromSeconds(random.Next(minSeconds, maxSeconds));
                    timeIncrements[currentIndex] = newTimeIncrement;
                    currentIndex = (currentIndex + 1) % timeIncrements.Length;

                    // Play a random sound from all valid sounds in the Soundfiles directory
                    var randomAudioFile = validAudioFiles[random.Next(validAudioFiles.Count)];
                    Console.WriteLine("Now playing sound file: " + Path.GetFileName(randomAudioFile));
                    using var player = new SoundPlayer(randomAudioFile);
                    player.Play();
                    Console.WriteLine("Restarting Loop...");
                }
                else
                {
                    // Sleep for a short interval and then check again
                    System.Threading.Thread.Sleep(500);
                }
            }
        }
    }
}