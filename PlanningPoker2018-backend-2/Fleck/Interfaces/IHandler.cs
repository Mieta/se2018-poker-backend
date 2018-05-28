using System.Collections.Generic;

namespace PlanningPoker2018_backend_2.Fleck.Interfaces
{
    public interface IHandler
    {
        byte[] CreateHandshake(string subProtocol = null);
        void Receive(IEnumerable<byte> data);
        byte[] FrameText(string text);
        byte[] FrameBinary(byte[] bytes);
        byte[] FramePing(byte[] bytes);
        byte[] FramePong(byte[] bytes);
        byte[] FrameClose(int code);
    }
}

