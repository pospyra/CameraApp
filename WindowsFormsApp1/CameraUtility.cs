using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DirectShowLib;
using OpenCvSharp;

public class CameraUtility
{
    private static CameraUtility _instance;

    private static Dictionary<int, string> cameraDict;

    private CameraUtility()
    {
        if (cameraDict == null)
        {
            InitializeCameras();
        }
    }

    public static CameraUtility Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CameraUtility();
            }
            return _instance;
        }
    }

    private void InitializeCameras()
    {
        DsDevice[] videoDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

        cameraDict = new Dictionary<int, string>();

        for (int i = 0; i < videoDevices.Length; i++)
        {
            cameraDict.Add(i, videoDevices[i].Name);
        }

        foreach (var device in videoDevices)
        {
            device.Dispose();
        }
    }

    public Dictionary<int, string> GetAvailableCameras()
    {
        return cameraDict;
    }

    public void ReleaseCameraAndTimer(Timer cameraTimer, VideoCapture videoCapture)
    {
        if (cameraTimer != null)
        {
            cameraTimer.Stop();
            cameraTimer.Dispose();
            cameraTimer = null;
        }

        if (videoCapture != null)
        {
            if (!videoCapture.IsDisposed)
            {
                videoCapture.Release();
            }
            videoCapture.Dispose();
        }
    }
}
