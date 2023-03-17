using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace starlink_proto_dump.Extensions
{
    public static class EnumerableByteStringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <postedFrom>
        /// https://github.com/protocolbuffers/protobuf/issues/9431
        /// </postedFrom>
        public static IEnumerable<ByteString>? Sort(this IEnumerable<ByteString>? source)
        {
            if (source == null)
                return source;

            var protosLoaded = new Dictionary<string, (ByteString, FileDescriptorProto)>();
            var resolved = new HashSet<string>();
            var orderedList = new List<ByteString>();

            foreach (var buffer in source)
            {
                var proto = FileDescriptorProto.Parser.ParseFrom(buffer.ToByteArray());
                protosLoaded.TryAdd(proto.Name, (buffer, proto));
            }

            while (protosLoaded.Count > 0)
            {
                var (buffer, nextProto) = protosLoaded.Values.FirstOrDefault(x => x.Item2.Dependency.All(dep => resolved.Contains(dep)));
                if (nextProto == null)
                {
                    throw new InvalidOperationException($"Invalid proto dependencies. Unable to resolve remaining protos [{string.Join(",", protosLoaded.Values.Select(x => x.Item2.Name))}] that don't have all their dependencies available.");
                }

                resolved.Add(nextProto.Name);
                protosLoaded.Remove(nextProto.Name);
                orderedList.Add(buffer);
            }
            return orderedList;
        }
    }
}
