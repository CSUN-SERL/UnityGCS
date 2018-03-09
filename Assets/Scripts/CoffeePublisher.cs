using UnityEngine;
using System.Collections;
using ROSBridgeLib.std_msgs;
public class CoffeePublisher
{
	public static string GetMessageType()
    {
        return StringMsg.GetMessageType();
    }

    public static string GetMessageTopic()
    {
        return "coffee";
    }
}
