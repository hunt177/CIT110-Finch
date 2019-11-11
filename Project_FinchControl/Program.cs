using FinchAPI;
using System;
using System.IO;
using System.Collections.Generic;

namespace Project_FinchControl
{

    // **************************************************
    //
    // Title: Finch Talent Show
    // Description: This program will have the Finch robot sing, dance, and light up. Note lengths are based on a tempo of 94 beats per minute
    // Application Type: Console
    // Author: Jordan Hunt
    // Dated Created: 2 October 2019
    // Last Modified: 6 November 2019
    //
    // **************************************************

    class Program
    {
        private enum Command
        {
            NONE,
            MOVEFORWARD,
            MOVEBACKWARD,
            STOPMOTORS,
            DELAY,
            TURNRIGHT,
            TURNLEFT,
            LEDON,
            LEDOFF,
            DONE

        }
        static void Main(string[] args)
        {
            displaySetTheme();

            DisplayWelcomeScreen();

            DisplayMainMenu();

            DisplayClosingScreen();
        }
        /// <summary>
        /// display main menu to user
        /// </summary>
        static void DisplayMainMenu()
        {
            //instantiate a Finch object
            Finch finchRobot = new Finch();
            bool finchRobotConnected = false;
            bool quitApplication = false;
            int numberAttempts = 0;
            finchRobot.setLED(0, 0, 0);
            do
            {
                DisplayScreenHeader("Main Menu");

                //get menu choice from user
                Console.WriteLine("A) Connect Finch Robot");
                Console.WriteLine("B) Talent Show");
                Console.WriteLine("C) Data Recorder");
                Console.WriteLine("D) Alarm System");
                Console.WriteLine("E) User Programming");
                Console.WriteLine("F) Disconnect Finch Robot");
                Console.WriteLine("G) User Theme");
                Console.WriteLine("Q) Quit");
                Console.Write("Enter Choice: ");

                var menuChoice = Console.ReadKey();
                //process menu choice
                switch (menuChoice.Key)
                {
                    case ConsoleKey.A:
                        if (finchRobotConnected)
                        {
                            Console.Clear();
                            Console.WriteLine("\n Finch robot already connected. Returning to main menu.");
                            DisplayContinuePrompt();
                        }
                        else
                        {
                            numberAttempts++;
                            if (numberAttempts > 3)
                            {
                                Console.Clear();
                                DisplayScreenHeader("Lockout Error");
                                Console.WriteLine("You have exceeded maximum number of attempts. Close the program and try again later.");
                                DisplayMainMenuPrompt();
                            }
                            else
                            {
                                finchRobotConnected = DisplayConnectFinchRobot(finchRobot);
                            }
                        }

                        break;

                    case ConsoleKey.B:
                        if (finchRobotConnected)
                        {
                            DisplayTalentShow(finchRobot);
                        }
                        else
                        {
                            DisplayErrorFinchNotConnected();
                        }

                        break;

                    case ConsoleKey.C:
                        if (finchRobotConnected)
                        {
                            DisplayDataRecorder(finchRobot);

                        }
                        else
                        {
                            DisplayErrorFinchNotConnected();
                        }

                        break;

                    case ConsoleKey.D:
                        if (finchRobotConnected)
                        {
                            DisplayAlarmSystem(finchRobot);
                        }
                        else
                        {
                            DisplayErrorFinchNotConnected();
                        }
                        break;

                    case ConsoleKey.E:
                        if (finchRobotConnected)
                        {
                            DisplayUserProgramming(finchRobot);
                        }
                        else
                        {
                            DisplayErrorFinchNotConnected();
                        }
                        break;

                    case ConsoleKey.F:
                        if (!finchRobotConnected)
                        {
                            DisplayErrorFinchNotConnected();
                        }
                        else
                        {
                            finchRobotConnected = DisplayDisconnectFinchRobot(finchRobot);
                        }
                        break;

                    case ConsoleKey.G:
                        DisplayChangeTheme();
                        break;
                    case ConsoleKey.Q:

                        if (finchRobotConnected)
                        {
                            Console.Clear();
                            Console.WriteLine("\nPlease disconnect Finch robot before exiting application.");
                            DisplayContinuePrompt();

                        }
                        else
                        {
                            quitApplication = true;
                        }


                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\t*********************************");
                        Console.WriteLine("\tPlease enter a valid menu choice");
                        Console.WriteLine("\t*********************************");
                        DisplayContinuePrompt();
                        break;

                }

            } while (!quitApplication);
        }

        static void DisplayUserProgramming(Finch finchRobot)
        {
            int motorSpeed;
            int ledBrightness;
            int waitSeconds;
            bool quitProgram = false;
            bool parametersSet = false;
            List<Command> commands = new List<Command>();

            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters;
            commandParameters.motorSpeed = 0;
            commandParameters.ledBrightness = 0;
            commandParameters.waitSeconds = 0;

            do
            {
                DisplayScreenHeader("User Programming");
                Console.WriteLine("A). Set Command Parameters");
                Console.WriteLine("B). Add Commands");
                Console.WriteLine("C). View Commands");
                Console.WriteLine("D). Execute Commands");
                Console.WriteLine("Q). Return to main menu");
                var menuChoice = Console.ReadKey();

                switch (menuChoice.Key)
                {
                    case ConsoleKey.A:
                        commandParameters = DisplayGetCommandParameters();
                        parametersSet = true;
                        break;

                    case ConsoleKey.B:
                        DisplayGetFinchCommands(commands);
                        break;

                    case ConsoleKey.C:
                        DisplayFinchCommands(commands);
                        break;

                    case ConsoleKey.D:
                        if (parametersSet == true)
                        {
                            bool continueExecuting = false;
                            do
                            {
                                DisplayExecuteFinchCommands(finchRobot, commands, commandParameters);

                                //ask user to repeat command execution
                                Console.WriteLine("Would you like to run commands again?");
                                string userResponse = Console.ReadLine().ToLower();
                                if (userResponse == "yes" || userResponse == "y")
                                {
                                    continueExecuting = true;
                                }
                                else
                                {
                                    continueExecuting = false;
                                }

                            } while (continueExecuting);

                        }
                        else
                        {
                            Console.WriteLine("\nParameters not set. Please set parameters to continue.");
                            DisplayContinuePrompt();
                        }
                        break;

                    case ConsoleKey.Q:
                        quitProgram = true;
                        break;

                    default:
                        Console.WriteLine("Unknown command. Please enter a proper command");
                        Console.ReadKey();
                        break;
                }

            } while (!quitProgram);
            DisplayMainMenuPrompt();
        }

        static void DisplayExecuteFinchCommands(Finch finchRobot, List<Command> commands, (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters)
        {
            DisplayScreenHeader("Execute Finch Commands");
            Console.WriteLine("The Finch will now execute the commands that you have entered.");
            DisplayContinuePrompt();

            int motorSpeed = commandParameters.motorSpeed;
            int ledBrightness = commandParameters.ledBrightness;
            int waitSeconds = commandParameters.waitSeconds * 1000;

            foreach (Command command in commands)
            {
                Console.WriteLine(command);
                switch (command)
                {
                    case Command.NONE:
                        break;
                    case Command.MOVEFORWARD:
                        finchRobot.setMotors(motorSpeed, motorSpeed);
                        finchRobot.wait(waitSeconds);
                        finchRobot.setMotors(0, 0);
                        break;
                    case Command.MOVEBACKWARD:
                        finchRobot.setMotors(-motorSpeed, -motorSpeed);
                        finchRobot.wait(waitSeconds);
                        finchRobot.setMotors(0, 0);
                        break;
                    case Command.STOPMOTORS:
                        finchRobot.setMotors(0, 0);
                        finchRobot.wait(waitSeconds);
                        break;
                    case Command.DELAY:
                        finchRobot.wait(waitSeconds);
                        break;
                    case Command.TURNRIGHT:
                        finchRobot.setMotors(0, motorSpeed);
                        finchRobot.wait(waitSeconds);
                        finchRobot.setMotors(0, 0);
                        break;
                    case Command.TURNLEFT:
                        finchRobot.setMotors(motorSpeed, 0);
                        finchRobot.wait(waitSeconds);
                        finchRobot.setMotors(0, 0);
                        break;
                    case Command.LEDON:
                        finchRobot.setLED(ledBrightness, ledBrightness, ledBrightness);
                        finchRobot.wait(waitSeconds);
                        break;
                    case Command.LEDOFF:
                        finchRobot.setLED(0, 0, 0);
                        finchRobot.wait(waitSeconds);
                        break;
                    case Command.DONE:
                        break;
                    default:
                        break;
                }

            }
            finchRobot.setLED(0, 0, 0);
            finchRobot.setMotors(0, 0);

            DisplayContinuePrompt();
        }

        static void displaySetTheme()
        {
            string dataPath = @"Data\Theme.txt";
            string[] colors = File.ReadAllLines(dataPath);
            string backgroundColorString = colors[0];
            string foregroundColorString = colors[1];

            Console.BackgroundColor = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), colors[0]);
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colors[1]);

        }
        static void DisplayChangeTheme()
        {
            string dataPath = @"Data\Theme.txt";
            string userInput;
            string[] colors = new string[2];
            ConsoleColor backgroundColor;
            ConsoleColor foregroundColor;
            bool validColor = false;
            DisplayScreenHeader("Change User Theme");
            Console.WriteLine("Current background color: " + Console.BackgroundColor);
            Console.WriteLine("Current text color: " + Console.ForegroundColor);

            Console.WriteLine();
            Console.WriteLine("Would you like to update current theme?");
            userInput = Console.ReadLine().ToLower();
            if (userInput == "yes" || userInput == "y")
            {
                Console.WriteLine("Type 'colors' for list of available colors.");
                userInput = Console.ReadLine().ToLower();
                if (userInput == "colors")
                {
                    viewColorOptions();
                }

                do
                {
                    Console.WriteLine("Choose a new text color.");
                    colors[0] = Console.ReadLine();
                    validColor = Enum.TryParse<ConsoleColor>(colors[0], out backgroundColor);
                    if (!validColor)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid color name.");
                    }
                } while (!validColor);
                do
                {
                    Console.WriteLine("Choose a new background color.");
                    colors[1] = Console.ReadLine();
                    validColor = Enum.TryParse<ConsoleColor>(colors[1], out foregroundColor);
                    if (!validColor)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid color name.");
                    }
                
                } while (!validColor);
                Console.BackgroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colors[0]);
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colors[1]);
                Console.Clear();
                Console.WriteLine("New background color: " +  colors[0]);
                Console.WriteLine("New forground color: " + colors[1]);
                Console.WriteLine("Do you want to keep this color theme? [yes, no]");
                userInput = Console.ReadLine().ToLower(); 
                if (userInput == "yes")
                    {
                    File.WriteAllLines(dataPath, colors);
                    displaySetTheme();
                }
                else
                {
                    displaySetTheme();
                    Console.Clear();
                    Console.WriteLine("Theme reset to previous option.");
                    
                    DisplayMainMenuPrompt();
                }

                

            }
            else if (userInput == "no" || userInput == "n")
            {
                DisplayMainMenuPrompt();
            }
            
        }

        static void viewColorOptions()
        {
            DisplayScreenHeader("Available color options");
            Console.WriteLine("Please enter each color name exactly as it appears on screen.");
            foreach (ConsoleColor color in (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor)))
            {
                Console.WriteLine(color);
                
            }
        }

        static void DisplayFinchCommands(List<Command> commands)
        {
            DisplayScreenHeader("Display Finch Commands");
            foreach (Command command in commands)
            {
                Console.WriteLine(command);
            }
            DisplayContinuePrompt();
        }

        static void DisplayGetFinchCommands(List<Command> commands)
        {
            Command command = Command.NONE;
            string userResponse;

            DisplayScreenHeader("Get Finch Commands");

            Console.WriteLine("This program will allow you to choose the commands that you would like the Finch" +
                " to execute. \nBelow is the acceptable list of commands. Type DONE when you are finished adding commands." +
                "\nMOVEFORWARD \nMOVEBACKWARD \nSTOPMOTORS \nDELAY \nTURNRIGHT \nTURNLEFT \nLEDON \nLEDOFF \nDONE");
            do
            {
                userResponse = Console.ReadLine().ToUpper();
                Enum.TryParse(userResponse, out command);
                switch (command)
                {
                    case Command.MOVEFORWARD:
                        commands.Add(command);
                        break;
                    case Command.MOVEBACKWARD:
                        commands.Add(command);
                        break;
                    case Command.STOPMOTORS:
                        commands.Add(command);
                        break;
                    case Command.DELAY:
                        commands.Add(command);
                        break;
                    case Command.TURNRIGHT:
                        commands.Add(command);
                        break;
                    case Command.TURNLEFT:
                        commands.Add(command);
                        break;
                    case Command.LEDON:
                        commands.Add(command);
                        break;
                    case Command.LEDOFF:
                        commands.Add(command);
                        break;
                    default:
                        Console.WriteLine("Unknown command. Please enter one of the listed commands.");
                        break;
                    case Command.DONE:
                        break;

                }

            } while (command != Command.DONE);

            Console.WriteLine();
            Console.WriteLine("Here are the commands you entered.");
            foreach (Command value in commands)
            {
                Console.WriteLine(value);
            }
            DisplayContinuePrompt();
        }

        static (int motorSpeed, int ledBrightness, int waitSeconds) DisplayGetCommandParameters()
        {
            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters;
            DisplayScreenHeader("Get Command Parameters");
            Console.WriteLine("Please enter the motor speed (1-255)");
            int.TryParse(Console.ReadLine(), out commandParameters.motorSpeed);
            if (commandParameters.motorSpeed < 1)
            {
                commandParameters.motorSpeed = 1;
            }

            Console.WriteLine("Please enter LED brightness (1-255)");
            int.TryParse(Console.ReadLine(), out commandParameters.ledBrightness);
            if (commandParameters.ledBrightness < 1)
            {
                commandParameters.ledBrightness = 1;
            }

            Console.WriteLine("Please enter the time to wait in seconds");
            int.TryParse(Console.ReadLine(), out commandParameters.waitSeconds);
            if (commandParameters.waitSeconds < 1)
            {
                commandParameters.waitSeconds = 1;
            }
            DisplayContinuePrompt();
            return commandParameters;

        }

        static void DisplayAlarmSystem(Finch finchRobot)
        {
            string alarmType;
            int maxSeconds;
            double[] threshold;
            bool thresholdExceeded = false;

            DisplayScreenHeader("Alarm System");
            Console.WriteLine("This program will allow the finch robot to monitor either light levels or " +
                "\ntemperature levels for a specified number of seconds.");
            DisplayContinuePrompt();
            Console.WriteLine();
            alarmType = DisplayGetAlarmType();

            maxSeconds = DisplayGetMaxSeconds();

            threshold = DisplayGetThreshold(finchRobot, alarmType);


            //warn user and pause
            if (alarmType == "light")
            {
                thresholdExceeded = MonitorCurrentLightLevel(finchRobot, threshold[0], threshold[1], maxSeconds);
            }
            else if (alarmType == "temperature")
            {
                thresholdExceeded = MonitorCurrentTemperatureLevel(finchRobot, threshold[0], threshold[1], maxSeconds);
            }

            if (thresholdExceeded)
            {

                Console.WriteLine($"Maximum or minimum {alarmType} level exceeded.");

            }
            else
            {
                Console.WriteLine("Maximum seconds to monitor exceeded.");
            }

            //monitor current temperature level

            DisplayMainMenuPrompt();
        }
        static bool MonitorCurrentTemperatureLevel(Finch finchRobot, double upperThreshold, double lowerThreshold, int maxSeconds)
        {
            bool thresholdExceeded = false;
            double currentTemperatureLevel = finchRobot.getTemperature();
            double seconds = 0;
            double secondsRemaining = maxSeconds;
            int readingNumber = 0;
            finchRobot.setLED(0, 255, 0);


            while (!thresholdExceeded && seconds <= maxSeconds)
            {
                currentTemperatureLevel = finchRobot.getTemperature();
                DisplayScreenHeader("Monitoring temperature levels.");
                Console.WriteLine($"Reading number: {readingNumber}");
                Console.WriteLine($"Maximum threshold: {upperThreshold}°C");
                Console.WriteLine($"Minimum threshold: {lowerThreshold}°C");
                Console.WriteLine();
                Console.WriteLine($"Current temperature: {currentTemperatureLevel:#.##}°C");
                Console.WriteLine($"{secondsRemaining} seconds remaining.");

                if (currentTemperatureLevel > upperThreshold || currentTemperatureLevel < lowerThreshold)
                {
                    finchRobot.setLED(255, 0, 0);
                    finchRobot.noteOn(1000);
                    finchRobot.wait(1500);
                    finchRobot.noteOff();
                    thresholdExceeded = true;
                }
                else
                {
                    thresholdExceeded = false;
                }
                finchRobot.wait(500);
                seconds += .5;
                secondsRemaining = maxSeconds - seconds;
                readingNumber++;
                Console.Clear();

            }

            return thresholdExceeded;
        }
        static bool MonitorCurrentLightLevel(Finch finchRobot, double upperThreshold, double lowerThreshold, int maxSeconds)
        {
            bool thresholdExceeded = false;
            int currentLightLevel;
            double seconds = 0;
            double secondsRemaining = maxSeconds;
            int readingNumber = 0;
            finchRobot.setLED(0, 255, 0);


            while (!thresholdExceeded && seconds <= maxSeconds)
            {
                currentLightLevel = (finchRobot.getLeftLightSensor() + finchRobot.getRightLightSensor()) / 2;

                DisplayScreenHeader("Monitoring light levels.");
                Console.WriteLine($"Reading number: {readingNumber}");
                Console.WriteLine($"Maximum threshold: {upperThreshold}");
                Console.WriteLine($"Minimum threshold: {lowerThreshold}");
                Console.WriteLine();

                Console.WriteLine($"Current light level: {currentLightLevel}");
                Console.WriteLine($"{secondsRemaining} seconds remaining.");

                if (currentLightLevel > upperThreshold || currentLightLevel < lowerThreshold)
                {
                    finchRobot.setLED(255, 0, 0);
                    finchRobot.noteOn(1000);
                    finchRobot.wait(1500);
                    finchRobot.noteOff();
                    thresholdExceeded = true;
                }
                else
                {
                    thresholdExceeded = false;
                }
                finchRobot.wait(500);
                seconds += .5;
                secondsRemaining = maxSeconds - seconds;
                readingNumber++;
                Console.Clear();

            }

            return thresholdExceeded;
        }

        static double[] DisplayGetThreshold(Finch finchRobot, string alarmType)
        {

            double[] threshold = new double[2];
            bool validResponse = false;
            DisplayScreenHeader("Threshold Value");

            do
            {
                switch (alarmType)
                {
                    case "light":
                        Console.WriteLine($"Current Light Level: {(finchRobot.getLeftLightSensor() + finchRobot.getRightLightSensor()) / 2}");
                        Console.Write("Enter Maximum Light Level [0-255]: ");
                        double.TryParse(Console.ReadLine(), out threshold[0]);
                        Console.Write("Enter Minimum Light Level [0-255]: ");
                        double.TryParse(Console.ReadLine(), out threshold[1]);
                        if (threshold[0] < 0 || threshold[0] > 255 || threshold[1] < 0 || threshold[1] > 255 || threshold[1] > threshold[0])
                        {
                            Console.Clear();
                            Console.WriteLine("Selected threshold is out of bounds. Please enter a number between 0-255");
                            validResponse = false;
                        }
                        else
                        {
                            validResponse = true;
                        }
                        break;

                    case "temperature":
                        Console.WriteLine($"Current Temperature Level: {finchRobot.getTemperature():#.##} °C");
                        Console.Write("Enter Maximum Temperature Level in degrees celsius. ");
                        double.TryParse(Console.ReadLine(), out threshold[0]);
                        Console.Write("Enter Minimum Temperature Level in degrees celsius. ");
                        double.TryParse(Console.ReadLine(), out threshold[1]);

                        if (threshold[1] > threshold[0])
                        {
                            validResponse = false;

                        }
                        else
                        {
                            validResponse = true;
                        }
                        Console.Clear();
                        break;

                    default:
                        break;
                }
            } while (!validResponse);
            DisplayContinuePrompt();

            return threshold;

        }

        /// <summary>
        /// display max seconds to wait
        /// </summary>
        static int DisplayGetMaxSeconds()
        {
            bool validResponse = false;
            int secondsToMonitor = 0;
            do
            {

                Console.Write("Seconds to Monitor: ");

                //validate user response
                int.TryParse(Console.ReadLine(), out secondsToMonitor);

                if (secondsToMonitor > 0)
                {
                    validResponse = true;
                }

                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid response. Please enter a valid number of seconds.");
                    validResponse = false;

                }

            } while (!validResponse);
            Console.WriteLine($"The alarm will monitor for {secondsToMonitor} seconds");
            DisplayContinuePrompt();
            return secondsToMonitor;
        }

        /// <summary>
        /// get alarm type from user
        /// </summary>
        static string DisplayGetAlarmType()
        {
            bool validResponse;
            string userResponse;
            do
            {
                Console.Write("Alarm Type [light or temperature] ");
                userResponse = Console.ReadLine().ToLower();

                //validate userResponse
                switch (userResponse)
                {
                    case "light":
                    case "temperature":
                        Console.WriteLine($"You have chosen {userResponse} alarm.");
                        validResponse = true;
                        break;

                    default:
                        validResponse = false;
                        Console.Clear();
                        Console.WriteLine("Invalid response. Please choose a proper option.");

                        break;
                }


            } while (!validResponse);
            DisplayContinuePrompt();
            return userResponse;

        }
        /// <summary>
        /// get number of data points from user
        /// </summary>
        static int DisplayGetNumberOfDataPoints()
        {
            int numberDataPoints;
            bool validResponse = false;

            do
            {
                DisplayScreenHeader("Number of Data Points");

                Console.Write("Enter number of data points: ");
                bool success = int.TryParse(Console.ReadLine(), out numberDataPoints);

                if (success)
                {
                    validResponse = true;
                }
                else
                {

                    validResponse = false;

                }
            } while (!validResponse);
            DisplayContinuePrompt();

            return numberDataPoints;
        }

        /// <summary>
        /// display data recorder screen
        /// </summary>
        /// </summary>
        static void DisplayDataRecorder(Finch finchRobot)
        {
            double dataPointFrequency;
            int numberDataPoints;
            bool validResponse = true;

            DisplayScreenHeader("Data Recorder");

            //tell user what is going to happen
            Console.WriteLine("This program is going to use the finch robot to take data of either temperature or light." +
                "You will specify the amount of time you would like to wait between samples, as well as the number of samples to take.");



            do
            {
                Console.WriteLine("\nSelect your option: ");
                Console.WriteLine("A). Record Temperature");
                Console.WriteLine("B). Record Light");
                Console.WriteLine("Q). Quit to main menu.");
                var menuChoice = Console.ReadKey();

                switch (menuChoice.Key)
                {
                    case ConsoleKey.A:
                        dataPointFrequency = DisplayGetDataPointFrequency();
                        numberDataPoints = DisplayGetNumberOfDataPoints();
                        //instantiate the array
                        double[] temperatures = new double[numberDataPoints];
                        DisplayGetTemperature(finchRobot, numberDataPoints, dataPointFrequency, temperatures);
                        DisplayTemperatureData(temperatures);
                        validResponse = true;
                        break;

                    case ConsoleKey.B:

                        dataPointFrequency = DisplayGetDataPointFrequency();
                        numberDataPoints = DisplayGetNumberOfDataPoints();
                        //instantiate arrays
                        int[] leftLights = new int[numberDataPoints];
                        int[] rightLights = new int[numberDataPoints];

                        DisplayGetLeftLightSensor(finchRobot, numberDataPoints, dataPointFrequency, leftLights);
                        DisplayGetRightLightSensor(finchRobot, numberDataPoints, dataPointFrequency, rightLights);
                        DisplayLightData(leftLights, rightLights);
                        validResponse = true;
                        break;

                    case ConsoleKey.Q:
                        validResponse = true;
                        break;

                    default:
                        validResponse = false;
                        Console.Clear();

                        Console.WriteLine("Invalid Response. Please enter a proper choice.");

                        DisplayContinuePrompt();
                        Console.Clear();
                        break;

                }
            } while (!validResponse);

            DisplayMainMenuPrompt();
        }

        /// <summary>
        /// display light data from finch
        /// </summary>
        static void DisplayLightData(int[] leftLights, int[] rightLights)
        {
            DisplayScreenHeader("Light Data");
            Console.WriteLine("This program will display the light data based on the parameters you provided.");
            Console.WriteLine("Data ranges from 0(dark) to 255(bright)");
            DisplayContinuePrompt();

            for (int index = 0; index < leftLights.Length; index++)
            {
                int avgLight = (leftLights[index] + rightLights[index]) / (2);
                Console.WriteLine($"Left light sensor {index + 1}: {leftLights[index]}".PadRight(30) +
                $"Right light sensor {index + 1}: {rightLights[index]}".PadRight(30) +
                $"Average light sensor {index + 1}: {avgLight}".PadRight(30));

            }
            DisplayContinuePrompt();
        }

        /// <summary>
        /// get right light sensor data from finch
        /// </summary>
        static void DisplayGetRightLightSensor(Finch finchRobot, int numberDataPoints, double dataPointFrequency, int[] rightLights)
        {
            DisplayScreenHeader("Get Right Light Sensor Data");

            //provide the user info and prompt
            Console.WriteLine("Getting right light sensor data based on the parameters you provided.");

            for (int index = 0; index < numberDataPoints; index++)
            {
                //record data
                rightLights[index] = finchRobot.getRightLightSensor();
                int milliseconds = (int)(dataPointFrequency * 1000);
                finchRobot.wait(milliseconds);


                //echo data
                Console.WriteLine($"Right light {index + 1}: {rightLights[index]} ");
            }

        }

        /// <summary>
        /// get left light sensor data from finch
        /// </summary>
        static void DisplayGetLeftLightSensor(Finch finchRobot, int numberDataPoints, double dataPointFrequency, int[] leftLights)
        {
            DisplayScreenHeader("Get Left Light Sensor Data");

            //provide the user info and prompt
            Console.WriteLine("Getting left light sensor data based on the parameters you provided.");

            for (int index = 0; index < numberDataPoints; index++)
            {
                //record data
                leftLights[index] = finchRobot.getLeftLightSensor();
                int milliseconds = (int)(dataPointFrequency * 1000);
                finchRobot.wait(milliseconds);


                //echo data
                Console.WriteLine($"Left light {index + 1}: {leftLights[index]} ");
            }

        }

        /// <summary>
        /// convert degrees celsius to fahrenheit
        /// </summary>
        static double celsiusToFahrenheit(double temperatures)
        {
            double fahrenheit = ((temperatures * (9 / 5)) + 32);

            return fahrenheit;
        }

        /// <summary>
        /// get temperature from finch
        /// </summary>
        static void DisplayGetTemperature(Finch finchRobot, int numberDataPoints, double dataPointFrequency, double[] temperatures)
        {
            DisplayScreenHeader("Get Temperature Data");

            //provide the user info and prompt
            Console.WriteLine("Getting temperature data based on the parameters you provided.");

            for (int index = 0; index < numberDataPoints; index++)
            {
                //record data
                temperatures[index] = finchRobot.getTemperature();
                int milliseconds = (int)(dataPointFrequency * 1000);
                finchRobot.wait(milliseconds);


                //echo data
                Console.WriteLine($"Temperature {index + 1}: {temperatures[index]:#.##}°C ");
            }


        }

        /// <summary>
        /// display data to user
        /// </summary>
        static void DisplayTemperatureData(double[] temperatures)
        {
            DisplayScreenHeader("Temperature Data");

            double fahrenheit;
            double sumC = 0, sumF = 0;
            double averageC = 0, averageF = 0;
            for (int index = 0; index < temperatures.Length; index++)
            {
                fahrenheit = celsiusToFahrenheit((temperatures[index]));
                Console.WriteLine($"Temperature {index + 1}: {temperatures[index]:#.##}°C  {fahrenheit:#.##}°F".PadRight(15));
                sumC = sumC + temperatures[index];
                sumF = sumF + fahrenheit;

            }
            averageC = sumC / temperatures.Length;
            averageF = sumF / temperatures.Length;
            Console.WriteLine($"Average temperature: {averageC:#.##}°C {averageF:#.##}°F".PadRight(20));
            DisplayContinuePrompt();
        }

        /// <summary>
        /// get data point frequency from user
        /// </summary>
        static double DisplayGetDataPointFrequency()
        {
            double dataPointFrequency;
            bool validResponse = false;

            do
            {
                DisplayScreenHeader("Data Point Frequency");
                Console.WriteLine("Please enter how long in seconds that you want to wait before taking each reading.");
                Console.Write("Enter Data Point Frequency: ");
                bool success = double.TryParse(Console.ReadLine(), out dataPointFrequency);

                if (success)
                {
                    validResponse = true;
                }
                else
                {

                    validResponse = false;


                }
            } while (!validResponse);


            return dataPointFrequency;

        }

        /// <summary>
        /// display finch talent show
        /// </summary>
        static void DisplayTalentShow(Finch finchRobot)
        {

            DisplayScreenHeader("Talent Show");
            Console.WriteLine("Finch robot is ready to show your talent.");
            DisplayContinuePrompt();
            Console.Clear();
            Console.WriteLine("Do you like Queen? {y, n}");

            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.Clear();
                playQueen(finchRobot);
            }
            else
            {
                finchAngry(finchRobot);
            }

        }

        /// <summary>
        /// turns finch blue
        /// </summary>
        static void finchBlue(Finch finchRobot)
        {
            finchRobot.setLED(0, 0, 255);
        }

        /// <summary>
        /// turns finch red
        /// </summary>
        static void finchRed(Finch finchRobot)
        {
            finchRobot.setLED(255, 0, 0);
        }

        /// <summary>
        /// turns finch green
        /// </summary>
        static void finchGreen(Finch finchRobot)
        {
            finchRobot.setLED(0, 255, 0);
        }

        ///<summary>
        ///Finch is angry that you don't like Queen
        /// </summary>   
        static void finchAngry(Finch finchRobot)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("You are an uncultured swine.");
            for (int i = 12000; i > 1; i = i - 800)
            {
                finchRobot.setLED(255, 0, 0);
                finchRobot.wait(10);
                finchRobot.setLED(0, 0, 0);
                finchRobot.noteOn(i);
                finchRobot.wait(100);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            finchRobot.noteOff();
            DisplayContinuePrompt();
        }

        /// <summary>
        /// plays Queen's Don't Stop Me Now
        /// </summary>
        static void playQueen(Finch finchRobot)
        {
            //declare variables
            int BPM = 94;
            int qtNote = 60000 / BPM;
            int sixteenthNote = (60000 / BPM) / 2;
            int eighthNote = (60000 / BPM) / 4;
            int halfNote = (60000 / BPM) * 2;
            int wholeNote = (60000 / BPM) * 4;
            int rest = 0;
            int c5 = 523;
            int d4 = 294, d5 = 587;
            int e5 = 659;
            int f4 = 349, f5 = 698;
            int fSharp5 = 740;
            int g4 = 392, g5 = 784;
            int a4 = 440, a5 = 880;
            int aSharp4 = 466, aSharp5 = 932;

            playTone(g4, eighthNote, finchRobot);//to
            finchGoesForward(finchRobot);
            playTone(g4, eighthNote, finchRobot);
            finchGreen(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(f4, qtNote, finchRobot);//night
            finchRed(finchRobot);

            finchBacksUp(finchRobot);
            playTone(f4, eighthNote, finchRobot); //I'm
            finchGreen(finchRobot);

            finchStops(finchRobot);
            playTone(f4, eighthNote, finchRobot);
            finchGoesForward(finchRobot);
            playTone(a4, eighthNote, finchRobot);
            finchRed(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(c5, eighthNote, finchRobot);//have
            finchBacksUp(finchRobot);
            playTone(f5, eighthNote, finchRobot);
            finchGreen(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(e5, qtNote, finchRobot);//self
            finchGoesForward(finchRobot);
            playTone(rest, sixteenthNote, finchRobot);//rest
            finchRed(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(c5, eighthNote, finchRobot);//a
            finchSpinsLeft(finchRobot);
            playTone(a4, eighthNote, finchRobot);//real
            finchGreen(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(g4, qtNote, finchRobot);//good
            finchSpinsLeft(finchRobot);
            finchRed(finchRobot);

            playTone(f4, qtNote + sixteenthNote, finchRobot);//time
            finchSpinsLeft(finchRobot);
            playTone(rest, eighthNote + sixteenthNote, finchRobot);//rest
            finchGreen(finchRobot);

            finchRed(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(a4, sixteenthNote, finchRobot);//I
            finchSpinsLeft(finchRobot);
            finchBlue(finchRobot);

            playTone(g4, sixteenthNote + eighthNote, finchRobot);
            finchGreen(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(g4, sixteenthNote, finchRobot);//feel
            finchSpinsRight(finchRobot);
            playTone(f4, sixteenthNote, finchRobot);//a
            finchGreen(finchRobot);

            finchBacksUp(finchRobot);
            playTone(g4, qtNote, finchRobot);//live
            finchSpinsLeft(finchRobot);
            finchRed(finchRobot);

            playTone(d4, eighthNote, finchRobot);
            finchGoesForward(finchRobot);
            playTone(a4, qtNote, finchRobot);
            finchBlue(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(aSharp4, qtNote, finchRobot);
            finchBacksUp(finchRobot);
            finchGreen(finchRobot);

            playTone(c5, sixteenthNote + halfNote, finchRobot);
            finchSpinsRight(finchRobot);
            playTone(rest, qtNote, finchRobot);//rest
            finchGoesForward(finchRobot);
            finchBlue(finchRobot);

            playTone(d5, eighthNote, finchRobot);//and
            finchSpinsLeft(finchRobot);
            playTone(e5, eighthNote, finchRobot);//the
            finchGreen(finchRobot);

            finchRed(finchRobot);

            finchBacksUp(finchRobot);
            playTone(f5, halfNote + qtNote, finchRobot);//world
            finchSpinsLeft(finchRobot);
            playTone(rest, eighthNote, finchRobot);//rest
            finchBlue(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(f5, eighthNote, finchRobot);//it's
            finchGreen(finchRobot);

            finchBacksUp(finchRobot);
            finchRed(finchRobot);

            playTone(a5, qtNote, finchRobot);//turn
            finchSpinsLeft(finchRobot);
            finchBlue(finchRobot);

            playTone(aSharp5, eighthNote, finchRobot);//ing
            finchSpinsRight(finchRobot);
            playTone(a5, eighthNote + eighthNote, finchRobot);//in
            finchGreen(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(g5, qtNote, finchRobot);//side
            finchGoesForward(finchRobot);
            playTone(f5, eighthNote + qtNote + eighthNote, finchRobot);//out
            finchRed(finchRobot);

            finchGoesForward(finchRobot);
            playTone(d5, eighthNote + qtNote, finchRobot);//yeah
            finchBacksUp(finchRobot);
            playTone(rest, eighthNote, finchRobot);//rest
            finchGoesForward(finchRobot);
            finchBlue(finchRobot);

            playTone(f5, eighthNote, finchRobot);//I'm
            finchSpinsRight(finchRobot);
            playTone(a5, eighthNote, finchRobot);//floating
            finchGoesForward(finchRobot);
            playTone(a5, eighthNote, finchRobot);
            finchGreen(finchRobot);

            finchGoesForward(finchRobot);
            playTone(aSharp5, eighthNote, finchRobot);//a
            finchBacksUp(finchRobot);
            finchBlue(finchRobot);

            playTone(a5, eighthNote + qtNote, finchRobot);//round
            finchBacksUp(finchRobot);
            playTone(rest, eighthNote, finchRobot);//rest
            finchBacksUp(finchRobot);
            finchRed(finchRobot);


            playTone(g5, eighthNote, finchRobot);//in
            finchSpinsRight(finchRobot);
            playTone(fSharp5, eighthNote, finchRobot);//ec
            finchSpinsLeft(finchRobot);
            playTone(g5, qtNote, finchRobot);//sta
            finchStops(finchRobot);
            finchGreen(finchRobot);

            playTone(a5, eighthNote + qtNote, finchRobot);//sy
            finchSpinsLeft(finchRobot);
            playTone(d5, qtNote, finchRobot);//so
            finchBlue(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(aSharp5, eighthNote, finchRobot);//don't
            finchBacksUp(finchRobot);
            playTone(rest, eighthNote + eighthNote, finchRobot);
            finchSpinsLeft(finchRobot);
            playTone(a5, eighthNote, finchRobot);//stop
            finchRed(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(rest, eighthNote, finchRobot);
            finchGreen(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(g5, eighthNote, finchRobot);//me 
            finchSpinsRight(finchRobot);
            finchRed(finchRobot);

            playTone(rest, eighthNote, finchRobot);//rest
            finchSpinsRight(finchRobot);
            playTone(aSharp4, eighthNote + halfNote, finchRobot);//now 
            finchBlue(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(aSharp5, eighthNote, finchRobot);//don't
            finchBacksUp(finchRobot);
            finchGreen(finchRobot);

            playTone(rest, eighthNote + eighthNote, finchRobot);
            finchRed(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(a5, eighthNote, finchRobot);//stop
            finchSpinsLeft(finchRobot);
            playTone(rest, eighthNote, finchRobot);
            finchSpinsRight(finchRobot);
            finchRed(finchRobot);

            playTone(g5, eighthNote, finchRobot);//me
            finchSpinsRight(finchRobot);
            playTone(rest, eighthNote, finchRobot);//rest
            finchGreen(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(g5, eighthNote, finchRobot);//cause
            finchBlue(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(g5, eighthNote, finchRobot);//I'm
            finchSpinsRight(finchRobot);
            //change tempo
            BPM = 87;
            playTone(aSharp4, eighthNote, finchRobot);//hav
            finchRed(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(aSharp4, eighthNote, finchRobot);//ing
            finchGreen(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(aSharp4, eighthNote, finchRobot);//a
            finchSpinsLeft(finchRobot);
            playTone(c5, qtNote, finchRobot);//good
            finchRed(finchRobot);

            finchSpinsRight(finchRobot);
            playTone(d5, qtNote, finchRobot);//time
            finchSpinsRight(finchRobot);
            playTone(d5, eighthNote, finchRobot);//I
            finchSpinsLeft(finchRobot);
            finchGreen(finchRobot);

            playTone(e5, eighthNote, finchRobot);//don't
            finchSpinsRight(finchRobot);
            //change tempo
            BPM = 80;
            playTone(e5, sixteenthNote, finchRobot);//wan
            finchRed(finchRobot);

            finchGoesForward(finchRobot);
            playTone(e5, sixteenthNote, finchRobot);//na
            finchStops(finchRobot);
            finchBlue(finchRobot);

            playTone(f5, eighthNote, finchRobot);//stop
            finchGreen(finchRobot);

            finchSpinsLeft(finchRobot);
            finchRed(finchRobot);

            playTone(g5, qtNote, finchRobot);//at
            finchGreen(finchRobot);

            finchSpinsLeft(finchRobot);
            playTone(f5, wholeNote, finchRobot);//all
            finchBlue(finchRobot);

            finchStops(finchRobot);

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            finchRobot.noteOff();



        }

        /// <summary>
        /// finch stops moving when requested
        /// </summary>
        static void finchStops(Finch finchRobot)
        {
            finchRobot.setMotors(0, 0);
        }

        /// <summary>
        /// finch robot backs up during rests
        /// </summary>
        static void finchBacksUp(Finch finchRobot)
        {
            finchRobot.setMotors(-40, -40);


        }

        /// <summary>
        /// finch goes forward during rests
        /// </summary>
        static void finchGoesForward(Finch finchRobot)
        {
            finchRobot.setMotors(40, 40);


        }

        /// <summary>
        /// finch spins left during rests
        /// </summary>
        static void finchSpinsLeft(Finch finchRobot)
        {
            finchRobot.setMotors(40, -40);


        }

        /// <summary>
        /// finch spins right during rests
        /// </summary>
        static void finchSpinsRight(Finch finchRobot)
        {
            finchRobot.setMotors(-40, 40);


        }

        /// <summary>
        /// abbreviated code for playing tones
        /// </summary>
        static void playTone(int frequency, int duration, Finch finchRobot)
        {
            finchRobot.noteOn(frequency);
            finchRobot.wait(duration);
            finchRobot.noteOff();
        }

        /// <summary>
        /// tells the user that selected option is not yet implemented in program and returns to main menu
        /// </summary>
        static void DisplayFeatureNotImplemented()
        {
            Console.Clear();
            Console.WriteLine("\n\t\tOops!");
            Console.WriteLine("This feature is not yet implemented in this program.");
            Console.WriteLine("Please check back at a later date.");
            DisplayContinuePrompt();
        }

        /// <summary>
        /// display to the user that finch is not connected and return to main menu.
        /// </summary>
        static void DisplayErrorFinchNotConnected()
        {
            Console.Clear();
            Console.WriteLine("\nFinch robot is not connected. Returning to main menu.");
            DisplayMainMenuPrompt();
        }

        /// <summary>
        /// disconnect finch robot from application
        /// </summary>
        static bool DisplayDisconnectFinchRobot(Finch finchRobot)
        {
            bool finchRobotConnected;

            DisplayScreenHeader("Disconnect Finch Robot");

            Console.WriteLine("Are you sure you want to disconnect Finch robot? Type 'yes' to confirm.");
            string userInput = Console.ReadLine().ToLower();



            if (userInput == "yes")
            {
                finchRobot.setLED(0, 0, 0);
                finchRobot.noteOn(200);
                finchRobot.wait(1000);
                finchRobot.noteOff();
                finchRobot.disConnect();
                finchRobotConnected = false;
                Console.WriteLine();
                Console.WriteLine("Finch Robot is now disconnected.");


            }
            else
            {
                Console.WriteLine();

                Console.WriteLine("Finch robot not disconnected properly. Returning to main menu.");
                finchRobotConnected = true;
            }


            DisplayContinuePrompt();
            return finchRobotConnected;
        }

        /// <summary>
        /// Connect finch robot to application
        /// </summary>
        static bool DisplayConnectFinchRobot(Finch finchRobot)
        {
            bool finchRobotConnected;
            DisplayScreenHeader("Connect to Finch Robot");

            Console.WriteLine("Ready to connect to Finch robot. Be sure to connect the USB cable to the robot and the computer.");
            DisplayContinuePrompt();

            finchRobotConnected = finchRobot.connect();

            if (finchRobotConnected)
            {
                finchRobot.setLED(0, 255, 0);
                finchRobot.noteOn(15000);
                finchRobot.wait(1000);
                finchRobot.noteOff();
                Console.WriteLine();
                Console.WriteLine("Finch robot is now connected.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Unable to connect to the Finch robot.");

            }

            DisplayContinuePrompt();

            return finchRobotConnected;

        }

        /// <summary>
        /// display welcome screen
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tFinch Control");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display closing screen
        /// </summary>
        static void DisplayClosingScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tThank you for using Finch Control!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        #region HELPER METHODS

        ///<summary>
        ///display main menu prompt
        ///</summary>
        static void DisplayMainMenuPrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to return to main menu.");
            Console.ReadKey();
        }

        /// <summary>
        /// display continue prompt
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// display screen header
        /// </summary>
        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + headerText);
            Console.WriteLine();
        }

        #endregion
    }
}
