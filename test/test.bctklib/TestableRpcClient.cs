using System;
using System.Collections.Generic;
using System.Linq;
using EpicChain.Json;

namespace test.bctklib
{
    class TestableRpcClient : EpicChain.Network.RPC.RpcClient
    {
        Queue<Func<JToken>> responseQueue = new();

        public TestableRpcClient(params Func<JToken>[] functions) : base(null)
        {
            foreach (var func in functions.Reverse())
            {
                responseQueue.Enqueue(func);
            }
        }

        public void QueueResource(string resourceName)
        {
            responseQueue.Enqueue(() => JToken.Parse(Utility.GetResource(resourceName)) ?? throw new NullReferenceException());
        }

        public override JToken RpcSend(string method, params JToken[] paraArgs)
        {
            return responseQueue.Dequeue()();
        }
    }
}
