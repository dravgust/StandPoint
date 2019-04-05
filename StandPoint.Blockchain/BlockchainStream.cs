using System;
using System.IO;
using System.Linq;
using System.Reflection;
using StandPoint.Utilities;

namespace StandPoint.Blockchain
{
    public class BlockchainStream : ApplicationStream
    {
        private static readonly MethodInfo ReadTypes = typeof(BlockchainStream)
            .GetTypeInfo()
            .DeclaredMethods
            .Where(m => m.Name == "Read")
            .Where(m => m.IsGenericMethodDefinition)
            .Where(m => m.GetParameters().Length == 1)
            .First(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.HasElementType && !p.ParameterType.GetElementType().IsArray));

        private static readonly MethodInfo WriteTypes = typeof(BlockchainStream)
            .GetTypeInfo()
            .DeclaredMethods
            .Where(m => m.Name == "Write")
            .Where(m => m.IsGenericMethodDefinition)
            .Where(m => m.GetParameters().Length == 1)
            .First(m => m.GetParameters().Any(p => p.ParameterType.HasElementType && !p.ParameterType.GetElementType().IsArray));

        public ProtocolVersion ProtocolVersion { get; set; } = ProtocolVersion.PROTOCOL_VERSION;

        public int MaxArraySize { get; set; } = 1024 * 1024;

        public BlockchainStream(Stream inner) : base(inner)
        {
        }

        public BlockchainStream(byte[] bytes) : base(bytes)
        {

        }

        public void Read(Type type, ref object obj)
        {
            try
            {
                var parameters = new [] { obj };
                ReadTypes.MakeGenericMethod(type).Invoke(this, parameters);
                obj = parameters[0];
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public void Write(Type type, object obj)
        {
            try
            {
                var parameters = new[] { obj };
                WriteTypes.MakeGenericMethod(type).Invoke(this, parameters);

            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }
}
