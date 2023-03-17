using Grpc.Net.Client;
using starlink_proto_dump;
using System.Diagnostics;
using System.Text;



if (isHelpArgs(args, new StringBuilder("usage: starlink-proto-dump [target path]")))
    System.Environment.Exit(1);

string? target = (args.Length > 0) ? args[0].Trim() : Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
string starlinkUri = @"http://192.168.100.1:9200";
string starlinkService = "SpaceX.API.Device.Device";

using (var channel = GrpcChannel.ForAddress(starlinkUri))
{
    Console.WriteLine($"Dump proto files.");
    Console.WriteLine($"Source: {starlinkUri}"); 
    Console.WriteLine($"Service: {starlinkService}");
    Console.WriteLine($"Target: {target}");

    Stopwatch watch = new Stopwatch();
    watch.Start();
    await ProtoHelper.Dump(
        channel, 
        starlinkService, 
        saveTo: target, 
        processing: (x) => Console.WriteLine($"Processing: {x}"));
    watch.Stop();
    Console.WriteLine($"Finish at {watch.Elapsed}.", ConsoleColor.Green);
}


static bool isHelpArgs(string[] args, StringBuilder helpMessage)
{
    string? command = args.Length > 0 ? args[0].Trim().ToLower() : string.Empty;

    bool result = command.Equals("-h", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("--help", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("help", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("/help", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("/h", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("/?", StringComparison.OrdinalIgnoreCase);

    if (result)
        Console.WriteLine(helpMessage.ToString());

    return result;
}
