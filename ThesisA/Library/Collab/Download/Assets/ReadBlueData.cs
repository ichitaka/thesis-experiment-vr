using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TechTweaking.Bluetooth;
using UnityEngine.UI;
using UnityEditor.Hardware;

public class ReadBlueData : MonoBehaviour
{
	private  BluetoothDevice device;
	public Text dataToSend;
	//public LabManager lab;

	private int[] controllerData; 
	private string[] tempData;
	//private int[] tempRotData;

	void Awake ()
	{
		BluetoothAdapter.enableBluetooth();//Force Enabling Bluetooth
		device = new BluetoothDevice();
		device.Name = "HC-06";
		device.setEndByte (10);
		device.ReadingCoroutine = ManageConnection;
		controllerData = new int[2];
		controllerData [0] = 0;
		controllerData [1] = 0;
		tempData = new string[2];
		//tempRotData = new int[10]; 
		connect();
	}
		
	public void connect ()//Connect to the public global variable "device" if it's not null.
	{
		device.connect ();
		Debug.Log ("Trying to connect");
	}

	public void disconnect ()//Disconnect the public global variable "device" if it's not null.
	{
		device.close ();
	}

	public void send ()
	{		
		if (device != null && !string.IsNullOrEmpty (dataToSend.text)) {
			device.send (System.Text.Encoding.ASCII.GetBytes (dataToSend.text + (char)10));//10 is our seperator Byte (sepration between packets)
		}
	}


	//############### Reading Data  #####################
	//Please note that you don't have to use Couroutienes, you can just put your code in the Update() method
	//If you want to achieve a minimum delay please check the "High Bit Rate Terminal" demo
	IEnumerator  ManageConnection (BluetoothDevice device)
	{//Manage Reading Coroutine


		while (device.IsReading) {

			//polll all available packets
			BtPackets packets = device.readAllPackets ();

			if (packets != null) {

				/*
				 * parse packets, packets are ordered by indecies (0,1,2,3 ... N),
				 * where Nth packet is the latest packet and 0th is the oldest/first arrived packet.
				 * 
				 */

				for (int i=0; i<packets.Count; i++) {

					//packets.Buffer contains all the needed packets plus a header of meta data (indecies and sizes) 
					//To parse a packet we need the INDEX and SIZE of that packet.
					int indx = packets.get_packet_offset_index (i);
					int size = packets.get_packet_size (i);

					string content = System.Text.ASCIIEncoding.ASCII.GetString (packets.Buffer, indx, size);
					tempData = content.Split (',');
					controllerData [0] = int.Parse (tempData [0]);
					controllerData [1] = int.Parse (tempData [1]);
					dataToSend.text = controllerData[0] + " @ " + controllerData[1];

				}
			}


			yield return null;
		}
	}

	public bool buttonDown() {
		if (controllerData [1] < 300) {
			return true;
		}
		return false;
	}

	public void measureInput() {
		StartCoroutine (ManageInput());
	}

	IEnumerator ManageInput() {
		yield return new WaitUntil (() => buttonDown());
		//lab.setInput (controllerData[0]);
		//lab.notWaitingAnymore ();
	}

}
