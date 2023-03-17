using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using SpaceX.API.Device;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace dish_status_watcher.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        // options default
        private const string _fieldsDef = "DeviceState.UptimeS, PopPingDropRate, PopPingLatencyMs, SecondsToFirstNonemptySlot, UplinkThroughputBps, DownlinkThroughputBps, GpsStats.GpsValid, GpsStats.NoSatsAfterTtff, GpsStats.GpsSats, ObstructionStats.CurrentlyObstructed, ObstructionStats.TimeObstructed, ObstructionStats.ValidS";
        private const string _intervalDef = "1000";

        // options
        private int _interval;
        private HashSet<string> _fields;
        private string _starlinkDishUri = "http://192.168.100.1:9200";

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // parse options
            Int32.TryParse(_configuration["interval"] ?? _intervalDef, out _interval);
            _fields = (_configuration["fields"] ?? _fieldsDef).Split(',').Select(p => p.Trim()).ToHashSet();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer generator = new PeriodicTimer(TimeSpan.FromMilliseconds(_interval));

            using GrpcChannel channel = GrpcChannel.ForAddress(_starlinkDishUri);

            var client = new Device.DeviceClient(channel);

            
            while (!stoppingToken.IsCancellationRequested && await generator.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var call = await client.HandleAsync(new Request() { GetStatus = new GetStatusRequest() }, cancellationToken: stoppingToken, deadline: DateTime.UtcNow.AddSeconds(1));
                    var dishStatus = call.DishGetStatus;


                    List<string> csv = new List<string>();

                    foreach (string field in _fields)
                    {
                        try
                        {
                            string fieldValue = GetPropValue(dishStatus, field) ?? string.Empty;
                            csv.Add(fieldValue);
                        }
                        catch { }
                    }
                    _logger.LogInformation(String.Join(',', csv));
                } 
                catch (Grpc.Core.RpcException rpcException)
                {
                    _logger.LogError($"{rpcException.Status.StatusCode}{(!String.IsNullOrEmpty(rpcException.Status.Detail) ? $": {rpcException.Status.Detail}" : string.Empty)}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }

        }

        public static string? GetPropValue(Object? obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }
                obj = obj.GetType()?.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.GetValue(obj, null);
            }
            return Convert.ToString(obj);
        }
    }
}