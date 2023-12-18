using Emgu.CV.Structure;
using Emgu.CV;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV.CvEnum;
using System.Drawing;
using Emgu.CV.Util;
using System.Linq;

public class CircleDetector
{
    public struct DataCircle
    {
        public Vector2 center;
        public float radius;
    }

    public DataCircle? DetectColor(Mat frame, UnityEngine.Color col)
    {
        Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();
        return DetectColor(image, col);
    }

    public DataCircle? DetectColor(Image<Bgr, byte> image, UnityEngine.Color col)
    {
        Hsv inf = new();
        Hsv sup = new();
        if (col == UnityEngine.Color.red)
        {
            inf = new Hsv(0, 70, 50);
            sup = new Hsv(10, 255, 255);
        }

        if (col == UnityEngine.Color.green)
        {
            inf = new Hsv(36, 25, 25);
            sup = new Hsv(70, 255, 255);
        }



        Image<Hsv, byte> hsvImage = image.Convert<Hsv, byte>();

        Image<Gray, byte> mask = hsvImage.InRange(inf, sup);


        Image<Gray, byte> circleGray = mask;
        CvInvoke.MedianBlur(circleGray, circleGray, 11);

        Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(5, 5), new Point(-1, -1));
        CvInvoke.MorphologyEx(circleGray, circleGray, MorphOp.Open, kernel, new Point(-1, -1), 10, BorderType.Default, default);

        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        CvInvoke.FindContours(circleGray, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

        if (contours.Size < 1) return null;
        VectorOfPoint largestContour = new(
            contours.ToArrayOfArray()
            .Where(a =>
                CvInvoke.ContourArea(new VectorOfPoint(a))
                ==
                contours.ToArrayOfArray()
                    .Max(b => CvInvoke.ContourArea(new VectorOfPoint(b))
                )
            ).First());


        VectorOfPoint c = largestContour;

        double perimeter = CvInvoke.ArcLength(c, true);
        VectorOfPoint approx = new VectorOfPoint();
        CvInvoke.ApproxPolyDP(c, approx, 0.04 * perimeter, true);
        double area = CvInvoke.ContourArea(c);
        if (approx.Size > 5 && area > 1000 && area < 500000)
        {
            CircleF cir = CvInvoke.MinEnclosingCircle(c);
            DataCircle data = new();
            data.center = new Vector2(
                cir.Center.X / image.Width,
                cir.Center.Y / image.Height
            );
            data.radius = cir.Radius;

            //CvInvoke.Circle(image, Point.Round(cir.Center), (int)cir.Radius, new MCvScalar(255, 0, 0), 2);
            return data;
        }
        return null;
    }



    VideoCamReader cam;

    // Start is called before the first frame update
    public CircleDetector(VideoCamReader cam)
    {
        this.cam = cam;
    }

}
