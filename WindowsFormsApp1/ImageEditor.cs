using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    public class ImageEditor
    {
        public void ApplyColorSettings(ref Mat img, double hue, double saturation)
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
            ApplyWhiteBalance(ref img);
        }

        public void ApplyWhiteBalance(ref Mat img)
        {
            // Ваш код для применения белого баланса (просто пример, логика может быть другой)
            // Пример: клонирование изображения и присваивание оригинальному изображению результата клонирования
            Mat wbImg = img.Clone();
            img = wbImg;
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
