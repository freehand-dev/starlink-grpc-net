using Grpc.Net.Client;
using SpaceX.API.Device;
using System.Text;
using System.Text.Json;


if (isHelpArgs(args, new StringBuilder("usage: dish-control reboot|stow|unstow")))
    System.Environment.Exit(1);

using (var channel = GrpcChannel.ForAddress("http://192.168.100.1:9200"))
{
    var client = new Device.DeviceClient(channel);

    switch (args[0].Trim().ToLower())
    {
        case "reboot":
            var callReboot = await client.HandleAsync(new Request() { Reboot = new RebootRequest() });
            break;
        case "stow":
            var callStow = await client.HandleAsync(new Request() { DishStow = new DishStowRequest() });
            break;
        case "unstow":
            var callUnstow = await client.HandleAsync(new Request() { DishStow = new DishStowRequest() { Unstow = true } });
            break;
        default:
            var callInfo = await client.HandleAsync(new Request() { DishGetConfig = new DishGetConfigRequest() });
            Console.WriteLine(JsonSerializer.Serialize(callInfo.DishGetConfig));
            break;
    }
}





static bool isHelpArgs(string[] args, StringBuilder helpMessage)
{
    string? command = args.Length > 0 ? args[0].Trim().ToLower() : default;
    bool result = string.IsNullOrEmpty(command) ||
        command.Equals("-h", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("--help", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("help", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("/help", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("/h", StringComparison.OrdinalIgnoreCase) ||
        command.Equals("/?", StringComparison.OrdinalIgnoreCase);

    if (result)
        Console.WriteLine(helpMessage.ToString());

    return result;
}