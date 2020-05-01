using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Linq;

public class HeartRateManager : MonoBehaviour
{
    const int queueSize = 10;

    private readonly SerialPort sp = new SerialPort("/dev/cu.usbmodem141201", 9600);
    private readonly Queue<int> calibrationQueue = new Queue<int>(queueSize);

    public int restingHeartRate;
    public int currentHeartRate;
    [Range(0, 4)]
    public int intensity;
    public float readInterval = 1;

    public TextMeshProUGUI currentHRText;
    public TextMeshProUGUI restingHRText;
    public Button calibrateButton;
    public bool isCalibrating;

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();
        sp.ReadTimeout = (int)(readInterval * 1000);

        StartCoroutine(ReadPort());

        calibrateButton.onClick.AddListener(() =>
        {
            calibrationQueue.Clear();
            isCalibrating = true;
        });
    }

    private void OnDestroy()
    {
        calibrateButton.onClick.RemoveAllListeners();
        sp.Close();
        StopCoroutine(ReadPort());
    }

    // Update is called once per frame
    void Update()
    {
        calibrateButton.interactable = !isCalibrating;

        currentHRText.text = $"Current Heart Rate: {currentHeartRate}";
        restingHRText.text = $"Resting Heart Rate: {restingHeartRate}";

        intensity = Mathf.RoundToInt(intensity);

        if (currentHeartRate <= restingHeartRate)
        {
            intensity = 0;
        }

        if (currentHeartRate >= restingHeartRate + 2)
        {
            intensity = 1;
        }

        if (currentHeartRate >= restingHeartRate + 4)
        {
            intensity = 2;
        }

        if (currentHeartRate >= restingHeartRate + 6)
        {
            intensity = 3;
        }

        if (currentHeartRate >= restingHeartRate + 8)
        {
            intensity = 4;
        }
    }

    private IEnumerator ReadPort()
    {
        yield return new WaitForSeconds(readInterval);

        if (sp.IsOpen)
        {
            var value = sp.ReadByte();
            currentHeartRate = value;

            if(isCalibrating)
            {
                Calibrate(currentHeartRate);
            }
        }

        StartCoroutine(ReadPort());

    }

    private void Calibrate(int heartrate)
    {
        if(calibrationQueue.Count == queueSize)
        {
            calibrationQueue.Dequeue();
            isCalibrating = false;
        }
        calibrationQueue.Enqueue(heartrate);
        restingHeartRate = calibrationQueue.Sum() / calibrationQueue.Count;
    }
}
