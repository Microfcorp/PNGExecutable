using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNGExecutable
{
    public static class Externals
    {
        /// <summary>
        /// Возвращает, высокая ли яркость у изображения
        /// </summary>
        /// <param name="img">Исходное изображение</param>
        /// <returns></returns>
        public static bool IsHighBrightnes(this Image imga)
        {
            var Color = AverageBrightnes(imga);
            //return (Color[0] >= 254 && Color[1] >= 254 && Color[2] >= 254);
            return Color >= 254;
        }

        /// <summary>
        /// Средняя яркость изображения
        /// </summary>
        /// <param name="img">Исходное изображение</param>
        /// <returns></returns>
        public static int[] AverageBrightnesRGB(this Image imga)
        {
            Bitmap img = (Bitmap)imga;
            int[] Color = { 0, 0, 0 };
            for (int i = 0; i < img.Width; i++)
            {
                for (int ia = 0; ia < img.Height; ia++)
                {
                    var p = img.GetPixel(i, ia);
                    Color[0] = (p.R + Color[0]) / 2;
                    Color[1] = (p.G + Color[1]) / 2;
                    Color[2] = (p.B + Color[2]) / 2;
                }
            }
            return Color;
        }

        /// <summary>
        /// Средняя яркость изображения
        /// </summary>
        /// <param name="img">Исходное изображение</param>
        /// <returns></returns>
        public static float AverageBrightnes(this Image imga)
        {
            Bitmap img = (Bitmap)imga;
            float Color = 0;
            for (int i = 0; i < img.Width; i++)
            {
                for (int ia = 0; ia < img.Height; ia++)
                {
                    var p = img.GetPixel(i, ia);
                    Color = ((float)p.GetBrightness()*254 + Color) / 2;
                }
            }
            return Color;
        }

        /// <summary>
        /// Пропорционально маштабирует изображение
        /// </summary>
        /// <param name="image">Изображение исходное</param>
        /// <param name="maxHeight">Максимальная высота</param>
        /// <returns></returns>
        public static Image ScaleImage(this Image image, int maxWidth)
        {
            var ratio = (double)maxWidth / image.Width - 1;

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
        /// <summary>
        /// Преобразует массив байт в изображение
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public static Image ToImage(this byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage;
            try
            {
                returnImage = Image.FromStream(ms);
            }
            catch 
            {
                returnImage = Properties.Resources.месяц;
            }
            return returnImage;
        }

        /// <summary>
        /// Преобразует изображение в массив байт
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        /// <summary>
        /// Делает изображение с черным фоном
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Image ToNight(this Image img)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gp = Graphics.FromImage(bmp);
            gp.Clear(Color.Black);
            gp.DrawImage(img, 0, 0);
            return bmp;
        }
        /// <summary>
        /// Поиск последовательности байтов в массиве
        /// </summary>
        /// <param name="Source">Источник</param>
        /// <param name="Pattern">Паттерн для поиска</param>
        /// <param name="Start">Смещение поиска</param>
        /// <returns></returns>
        public static List<int> SearchBytePattern(this byte[] Source, byte[] Pattern, int Start)
        {
            int SourceLen = Source.Length - Pattern.Length + 1;//Получаем длину исходного массива.
            int j;
            List<int> positions = new List<int>();//Переменная для хранения списка результатов поиска.

            for (int i = Start; i < SourceLen; i++)
            {
                if (Source[i] != Pattern[0]) continue;//Сравниваем первый искомый байт и если он совпадает, то проверяем остальные байты за ним идущие, иначе идем дальше по массиву.
                for (j = Pattern.Length - 1; j >= 1 && Source[i + j] == Pattern[j]; j--) ;//Сравниваем остальные байты с нашим значением.
                if (j == 0)//Переменная будет равна нулю, если все байты совпали
                {
                    positions.Add(i);//Добавляем адрес начала совпадения первого байта в список
                    i += Pattern.Length - 1;//Увеличиваем значение переменной на величину искомого, так как мы его уже проверили и это ускоряет поиск.
                }
            }
            return positions;//Отдаем список адресов, которые совпадают с искомым значением.
        }

        /// <summary>
        /// Преобразует байты в читаемый вид
        /// </summary>
        /// <param name="size">Размер в байтах</param>
        /// <returns></returns>
        public static string formatFileSize(this long size)
        {
            //size /= 1024;
            var a = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            int pos = 0;
            while (size >= 1024)
            {
                size /= 1024;
                pos++;
            }
            return Math.Round((double)size, 2) + " " + a[pos];
        }
    }
}
