using Emgu.CV;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCamReader
{
    #region Singleton
    private static VideoCamReader instance;

    public static VideoCamReader Instance {
        get
        {
            if(instance == null)
            {
                instance = new VideoCamReader();
            }
            return instance;
        }

        private set { instance = value; }
    }
    #endregion

    readonly VideoCapture videoCam = null;
    readonly Mat videoFrame = null;
    public bool IsOk { get; private set; }

    public bool Start()
    {
        if(videoCam != null)
        {
            try
            {
                videoCam.Start();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
        IsOk = videoCam.IsOpened;
        return videoCam.IsOpened;
    }

    private VideoCamReader()
    {
        videoCam = new VideoCapture(0);
        videoFrame = new Mat();

        videoCam.ImageGrabbed += new System.EventHandler(HandleWebcamQueryFrame);
    }

    private void HandleWebcamQueryFrame(object sender, System.EventArgs e)
    {
        if (videoCam != null && videoCam.Ptr != System.IntPtr.Zero)
        {
            videoCam.Retrieve(videoFrame);
        }
    }

    private void OnDestroy()
    {
        videoCam.Stop();
        videoCam.Dispose();
    }

    public Mat GetData()
    {
        IsOk = videoCam.IsOpened;
        if (videoCam.IsOpened)
        {
            //CvInvoke.Flip(videoFrame, videoFrame, Emgu.CV.CvEnum.FlipType.Vertical);
            return videoFrame;
        }
        return null;
    }
}
