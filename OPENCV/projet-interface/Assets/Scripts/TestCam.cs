using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCam : MonoBehaviour
{
    VideoCamReader cam;
    CircleDetector circ;

    public Image terrain = default;
    public RawImage screen = default;
    public RectTransform greenCircle;
    public RectTransform redCircle;
    Texture2D t;

    // Start is called before the first frame update
    void Start()
    {
        cam = VideoCamReader.Instance;
        Debug.Log(cam.Start());
        circ = new CircleDetector(cam);

        t = new Texture2D((int)screen.rectTransform.rect.width, (int)screen.rectTransform.rect.height, TextureFormat.RGBA32, false, false);
        screen.texture = t;
    }

    // Update is called once per frame
    void Update()
    {
        Mat frame;
        if(cam.IsOk && (frame = cam.GetData()).Size != new System.Drawing.Size(0, 0))
        {
            DisplayFrameOnPlane(frame);
            CircleDetector.DataCircle? data = circ.DetectColor(frame, Color.green);
            if(data is CircleDetector.DataCircle dataCir)
            {
                Vector3 pos = new(dataCir.center.x, -dataCir.center.y, 0);
                Vector2 frameSize = new(frame.Size.Width, frame.Size.Height);
                Vector3 posOnCanvas = screen.rectTransform.rect.size * pos / frameSize;



                greenCircle.localPosition = posOnCanvas;
                //circle.position = pos;
            }

            data = circ.DetectColor(frame, Color.red);
            if (data is CircleDetector.DataCircle dataCir2)
            {
                Vector3 pos = new(dataCir2.center.x, -dataCir2.center.y, 0);
                Vector2 frameSize = new(frame.Size.Width, frame.Size.Height);
                Vector3 posOnCanvas = screen.rectTransform.rect.size * pos / frameSize;
                redCircle.localPosition = posOnCanvas;
                //circle.position = pos;
            }
        }
    }

    private void DisplayFrameOnPlane(Mat frame)
    {
        Image<Bgr, byte> currentFrameImage = frame.ToImage<Bgr, byte>();
        Mat currentFrame = currentFrameImage.Mat;

        CvInvoke.Resize(currentFrame, currentFrame, new System.Drawing.Size(t.width, t.height));
        CvInvoke.CvtColor(currentFrame, currentFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Rgba);
        CvInvoke.Flip(currentFrame, currentFrame, Emgu.CV.CvEnum.FlipType.Vertical);

        t.LoadRawTextureData(currentFrame.DataPointer, currentFrame.Width * currentFrame.Height * 4);
        t.Apply();
    }
}
