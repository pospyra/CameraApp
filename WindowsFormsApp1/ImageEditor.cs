using OpenCvSharp;
using System;

namespace WindowsFormsApp1
{

    public class ImageEditor
    {
        public void ApplyColorSettings(ref Mat img, double hue, double saturation, bool whiteBalance)
        {
            if (img.Channels() == 1)
            {
                return;
            }

            Cv2.CvtColor(img, img, ColorConversionCodes.BGR2HSV);

            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    Vec3b pixel = img.At<Vec3b>(i, j);
                    pixel.Item1 = (byte)(pixel.Item1 * saturation);
                    pixel.Item0 = (byte)((pixel.Item0 + hue) % 180);
                    img.Set(i, j, pixel);
                }
            }

            Cv2.CvtColor(img, img, ColorConversionCodes.HSV2BGR);

            if (whiteBalance)
            {
                ApplyWhiteBalance(ref img);
            }
        }

        public void ApplyWhiteBalance(ref Mat img)
        {
            Cv2.CvtColor(img, img, ColorConversionCodes.BGR2Lab);

            Scalar meanLAB = Cv2.Mean(img);
            double meanL = meanLAB.Val0;
            double meanA = meanLAB.Val1;
            double meanB = meanLAB.Val2;

            double targetA = 128;
            double targetB = 128;

            double shiftA = targetA - meanA;
            double shiftB = targetB - meanB;

            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    Vec3b pixel = img.At<Vec3b>(i, j);

                    pixel.Item1 = Clamp((byte)(pixel.Item1 + shiftA), 0, 255);
                    pixel.Item2 = Clamp((byte)(pixel.Item2 + shiftB), 0, 255);
                    img.Set(i, j, pixel);
                }
            }

            Cv2.CvtColor(img, img, ColorConversionCodes.Lab2BGR);
        }

        public static byte Clamp(byte value, byte min, byte max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            return value;
        }


        public void ApplyExposureSettings(ref Mat img, int brightness, double contrast, double exposure)
        {
            if (img.Channels() == 1)
            {
                img.ConvertTo(img, -1, contrast, brightness);

                for (int i = 0; i < img.Rows; i++)
                {
                    for (int j = 0; j < img.Cols; j++)
                    {
                        byte pixel = img.At<byte>(i, j);
                        pixel = (byte)(pixel + (exposure * 128));
                        img.Set(i, j, pixel);
                    }
                }
            }
            else if (img.Channels() == 3)
            {
                img.ConvertTo(img, -1, contrast, brightness);

                Mat yuvImg = new Mat();
                Cv2.CvtColor(img, yuvImg, ColorConversionCodes.BGR2YUV);

                for (int i = 0; i < yuvImg.Rows; i++)
                {
                    for (int j = 0; j < yuvImg.Cols; j++)
                    {
                        Vec3b pixel = yuvImg.At<Vec3b>(i, j);
                        pixel.Item0 = (byte)(pixel.Item0 + (exposure * 128));
                        yuvImg.Set(i, j, pixel);
                    }
                }

                Cv2.CvtColor(yuvImg, img, ColorConversionCodes.YUV2BGR);
            }
        }

        public void ApplyImageSettings(ref Mat img, double sharpness, double gamma, double backlightCompensation)
        {
            int channels = img.Channels();

            if (channels == 1)
            {
                if (sharpness < 1.0)
                {
                    Mat laplacianImg = new Mat();
                    Cv2.Laplacian(img, laplacianImg, img.Depth());
                    img += (1.0 - sharpness) * laplacianImg;
                    laplacianImg.Dispose();
                }
            }
            else if (channels == 3)
            {
                if (sharpness > 1.0)
                {
                    int kernelSize = (int)(sharpness * 5);
                    if (kernelSize % 2 == 0)
                    {
                        kernelSize++;
                    }
                    Cv2.GaussianBlur(img, img, new OpenCvSharp.Size(kernelSize, kernelSize), 0);
                }
                else if (sharpness < 1.0)
                {
                    Mat laplacianImg = new Mat();
                    Cv2.Laplacian(img, laplacianImg, img.Depth());
                    img += (1.0 - sharpness) * laplacianImg;
                    laplacianImg.Dispose();
                }
            }

            if (gamma != 1.0)
            {
                Mat gammaImg = new Mat();
                img.ConvertTo(gammaImg, -1, 1, 0);
                for (int i = 0; i < img.Rows; i++)
                {
                    for (int j = 0; j < img.Cols; j++)
                    {
                        Vec3b pixel = gammaImg.At<Vec3b>(i, j);
                        for (int k = 0; k < channels; k++)
                        {
                            double normalized = pixel[k] / 255.0;
                            double corrected = Math.Pow(normalized, 1.0 / gamma);
                            pixel[k] = (byte)(corrected * 255);
                        }
                        gammaImg.Set(i, j, pixel);
                    }
                }
                img = gammaImg;
            }

            double backlightCompensationFactor = backlightCompensation - 0.5;

            if (backlightCompensationFactor != 0.0)
            {
                img += backlightCompensationFactor * 255;
            }
        }
    }
}
