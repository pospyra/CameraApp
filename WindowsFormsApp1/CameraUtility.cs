using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DirectShowLib;

public class CameraUtility
{

    public static Dictionary<int, string> GetAvailableCameras()
    {
        // Словарь для хранения индексов камер и их имен
        Dictionary<int, string> cameraDict = new Dictionary<int, string>();

        // Список для хранения информации о доступных устройствах
        DsDevice[] videoDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

        // Перебор всех доступных устройств и добавление их в словарь
        for (int i = 0; i < videoDevices.Length; i++)
        {
            cameraDict.Add(i, videoDevices[i].Name);
        }

        // Освобождение ресурсов
        foreach (var device in videoDevices)
        {
            device.Dispose();
        }

        return cameraDict;
    }

    // Метод для получения доступных форматов видео из выбранной камеры
    public static List<string> GetAvailableVideoFormats(int cameraIndex)
    {
        // Список для хранения доступных форматов видео
        List<string> videoFormats = new List<string>();

        // Получаем список доступных устройств
        DsDevice[] videoDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

        // Проверяем, существует ли камера с указанным индексом
        if (cameraIndex >= 0 && cameraIndex < videoDevices.Length)
        {
            // Получаем выбранное устройство камеры
            DsDevice camera = videoDevices[cameraIndex];

            // Создаем графический менеджер для управления потоками
            IFilterGraph2 graphBuilder = (IFilterGraph2)new FilterGraph();

            // Создаем устройство камеры и добавляем его в графический менеджер
            object cameraObject;
            camera.Mon.BindToObject(null, null, typeof(IBaseFilter).GUID, out cameraObject);
            IBaseFilter cameraFilter = cameraObject as IBaseFilter;

            if (cameraFilter != null)
            {
                // Добавляем фильтр камеры в графический менеджер
                graphBuilder.AddFilter(cameraFilter, "Camera");

                // Получаем конфигурацию потока камеры
                IAMStreamConfig streamConfig = cameraFilter as IAMStreamConfig;

                if (streamConfig != null)
                {
                    // Получаем количество форматов
                    int count, size;
                    streamConfig.GetNumberOfCapabilities(out count, out size);

                    // Перебор всех доступных форматов
                    for (int i = 0; i < count; i++)
                    {
                        // Создайте экземпляры для хранения значений типа медиа и структур SCCap
                        AMMediaType mediaType;
                        IntPtr pSCCap = IntPtr.Zero;

                        // Получите информацию о формате
                        int hr = streamConfig.GetStreamCaps(i, out mediaType,  pSCCap);

                        // Проверьте результат выполнения метода
                        if (hr == 0)
                        {
                            // Получите информацию о медиа-типе
                            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));

                            // Создайте строку для описания формата видео
                            string videoFormat = $"{videoInfoHeader.BmiHeader.Width}x{videoInfoHeader.BmiHeader.Height} @ {videoInfoHeader.AvgTimePerFrame}ns";

                            // Добавьте формат в список
                            videoFormats.Add(videoFormat);
                        }

                        // Освобождение памяти
                        if (mediaType.formatPtr != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(mediaType.formatPtr);
                            mediaType.formatPtr = IntPtr.Zero;
                        }
                    }
                }

                // Удаление фильтра камеры из графического менеджера
                graphBuilder.RemoveFilter(cameraFilter);
            }

            // Освобождение ресурсов камеры
            cameraFilter = null;
        }

        // Освобождение ресурсов
        foreach (var device in videoDevices)
        {
            device.Dispose();
        }

        return videoFormats;
    }
}
