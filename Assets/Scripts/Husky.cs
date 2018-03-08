// The purpose of Husky.cs is to get keyboard input from the arrow keys. 
// That input is then manipulated to be the movement of the Husky bot. 
// We can change the Husky's velocity thorugh this script to our needs.
// The data in this script is sent though to ROSBridge and is being published 
// to a topic at real-time.

using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using UnityEngine.UI;
using System.Threading;
using System;

public class Husky : MonoBehaviour  {
	private ROSBridgeWebSocketConnection _ros = null;	
	private ROSBridgeWebSocketConnection _ros1 = null;	
	private ROSBridgeWebSocketConnection _ros4 = null;	



	private bool _useJoysticks;

	// Will create bot instances
	public Toggle Isbot1;
	public Toggle Isbot2;
	public Toggle Isbot3;
	public Toggle Isbot4;
	private string _topic = "/robot1/cmd_vel";
	private string _aws = "ws:ubuntu@13.57.99.200";
	private string _station4 = "ws:ubuntu@192.168.1.43";
	private string _station1 = "ws:ubuntu@192.168.1.161";
	

	// The critical thing here is to define our subscribers, publishers and service response handlers.
	void Start ()
	{
        
		_useJoysticks = Input.GetJoystickNames ().Length > 0;
		
		// ros will be a node with said connection below... To our AWS server.
        _ros = new ROSBridgeWebSocketConnection (_station1, 9090);
        //_ros1 = new ROSBridgeWebSocketConnection (_station1, 9090); 
        //_ros4 = new ROSBridgeWebSocketConnection (_station4, 9090);

        _ros.AddPublisher(typeof(CoffeePublisher));

		// Gives a live connection to ROS via ROSBridge.
		_ros.Connect ();
		//_ros1.Connect ();	
		//_ros4.Connect ();
        //Thread.Sleep(3000);
        //_ros.Advertise("coffee", StringMsg.GetMessageType());


	}

	// Extremely important to disconnect from ROS. OTherwise packets continue to flow.
	void OnApplicationQuit() {
		if(_ros!=null)
			_ros.Disconnect ();
		if(_ros1!=null)
			_ros1.Disconnect ();
		if(_ros4!=null)
			_ros4.Disconnect ();
	}
	
	// Update is called once per frame in Unity. We use the joystick or cursor keys to generate teleoperational commands
	// that are sent to the ROS world, which drives the robot which ...
	void Update()
	{
		// Instantiates variables with keyboad input (Lines 44 - 62).
		float _dx, _dy;

		if (_useJoysticks)
		{
			_dx = Input.GetAxis("Joy0X");
			_dy = Input.GetAxis("Joy0Y");
		}
		else
		{
			_dx = Input.GetAxis("Horizontal");
			_dy = Input.GetAxis("Vertical");
		}
		
		// Multiplying _dy or _dx by a larger value, increases "speed".
		// Linear is responsibile for forward and backward movment.
		var linear = _dy * 3.0f; 
		//angular is responsible for rotaion.
		var angular = -_dx * 2.0f; 

		// Create a ROS Twist message from the keyboard input. This input/twist message, creates the data that will in turn move the 
		// bot on the ground.
		var msg = new TwistMsg(new Vector3Msg(linear, 0.0, 0.0), new Vector3Msg(0.0, 0.0, angular));
		_topic = "";
		ActiveToggle();             ///////\\\\\\\\\ Need to call func to get appropriate topic 
                                    // name for the correct selected bot. This is for testing of 
                                    // the 4 bot environment!!!!!!

        // Publishes the TwistMsg values over to the /cmd_vel topic in ROS.
        //_ros.Publish("/cmd_vel", msg);

        //string STR;
        //STR = ROSBridgeMsg.Advertise("/coffee", "std_msgs/String");
        //        Debug.Log(STR);
        //var str = new StringMsg("hello Miguel");

        var str = new StringMsg("Hello Miguel");
        Debug.Log(str);
        _ros.Publish("/coffee", str);




        //_ros.Publish(_topic, msg);

		//_ros.Publish("/robot1/cmd_vel", msg);	/////////\\\\\\\\\\ this is for testing of the 4 bots 
									// environment!!!	

		_ros.Render ();
       
	}

	void ActiveToggle()
	{
		if (Isbot1.isOn)
		{
			_topic = "/robot1/cmd_vel";
			//_ros = _ros1;
		}
		if (Isbot2.isOn)
		{
			_topic = "/robot2/cmd_vel";
			//_ros = _ros1;
		}
		if (Isbot3.isOn)
		{
			_topic = "/robot3/cmd_vel";
			//_ros = _ros1;
		}
		if (Isbot4.isOn)
		{
			_topic = "/robot4/cmd_vel";
			//_ros = _ros1;
		}


	}
}

