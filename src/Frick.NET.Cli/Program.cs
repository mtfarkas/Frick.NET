using Frick.NET;

try
{
    string? bfSource = null;

    if (args.Length > 0)
    {
        string argFlag = args[0].Trim();
        if (argFlag == "-i")
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("The -i flag requires a file to be specified. Please pass a file name.");
                return -1;
            }
            bfSource = File.ReadAllText(args[1]);
        }
        else
        {
            Console.Error.WriteLine("Unknown argument. Please use -i <FILENAME> if you want to run a Brainfuck program from a file, or pass the source code using STDIN.");
            return -1;
        }
    }
    else
    {
        if (Console.IsInputRedirected)
        {
            using var inputStream = Console.OpenStandardInput();
            using var streamReader = new StreamReader(inputStream);
            bfSource = streamReader.ReadToEnd();
        }
        else
        {
            bfSource = Console.ReadLine();
        }
    }

    if (string.IsNullOrWhiteSpace(bfSource))
    {
        Console.Error.WriteLine("Brainfuck source code was empty. Please provide the source either using -i <FILENAME> or using STDIN");
        return -1;
    }

    FrickInterpreter interpreter = new();

    interpreter.Run(bfSource);

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error while running the CLI: {ex.Message}");
    return -1;
}