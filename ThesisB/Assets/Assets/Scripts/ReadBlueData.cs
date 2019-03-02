using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TechTweaking.Bluetooth;
using UnityEngine.UI;

public class ReadBlueData : MonoBehaviour
{
	private  BluetoothDevice device;

	private int[] controllerData; 
	private string[] tempData;
	private bool down;


	#region Unity MonoBehaviour

	void Awake ()
	{
		BluetoothAdapter.enableBluetooth();//Force Enabling Bluetooth
		device = new BluetoothDevice();
		device.Name = "HC-06";
		device.setEndByte (10);
		device.ReadingCoroutine = ManageConnection;
		controllerData = new int[2];
		controllerData [0] = 9000;
		controllerData [1] = 9000;
		tempData = new string[2];
		down = false;
		DontDestroyOnLoad (this.gameObject);
		connect();
	}

	void Update()
	{
		if (GvrControllerInput.AppButtonDown) {
			disconnect ();
		} else if (GvrControllerInput.ClickButtonDown) {
			connect ();
		}
	}
	#endregion

	#region Connection

	private void reconnect()
	{
		StartCoroutine (restart ());
//		disconnect ();
//		if (!device.IsConnected) {
//			connect ();
//		}
	}

	IEnumerator restart()
	{
		disconnect ();
		yield return new WaitUntil (() => !device.IsConnected);
		connect ();
	}

	public void connect ()//Connect to the public global variable "device" if it's not null.
	{
		device.connect ();
	}

	public void disconnect ()//Disconnect the public global variable "device" if it's not null.
	{
		device.close ();

	}
	#endregion


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
					if (down) {
						if (controllerData [1] > 300) {
							down = false;
						}
					}
				}
			}
			yield return null;
		}
	}

	public bool buttonDown() {
		if (controllerData [1] < 300 && !down) {
			down = true;
			return true;
		}
		return false;
	}

	public int rotationInput() {
		return controllerData [0];
	}
}
