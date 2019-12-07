using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace RoomFinishes
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            try
            {
                var panel = a.CreateRibbonPanel("ПИК");
                Icons.CreateIcons(panel);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;
    }

    internal class Icons
    {
        public static void CreateIcons(RibbonPanel panel)
        {
            //Retrive dll path
            var dllPath = Assembly.GetExecutingAssembly().Location;

            //Add RoomsFinishes Button
            var finishButtonData = new PushButtonData("Button", "Перекрасить смежные", dllPath, "RoomFinishes.Plugin")
            {
                ToolTip = "Перекрасить смежные",
                LargeImage = RetriveImage("RoomFinishes.Resources.RoomFinishLarge.png"),
                Image = RetriveImage("RoomFinishes.Resources.RoomFinishSmall.png")
            };
            var sbRoomData = new SplitButtonData("Button2", "Перекрасить смежные");
            var sbRoom = panel.AddItem(sbRoomData) as SplitButton;
            sbRoom?.AddPushButton(finishButtonData);
        }

        private static ImageSource RetriveImage(string imagePath)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imagePath);
            switch (imagePath.Substring(imagePath.Length - 3))
            {
                case "jpg":
                    var jpgDecoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.Default);
                    return jpgDecoder.Frames[0];
                case "bmp":
                    var bmpDecoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.Default);
                    return bmpDecoder.Frames[0];
                case "png":
                    var pngDecoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.Default);
                    return pngDecoder.Frames[0];
                case "ico":
                    var icoDecoder = new IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.Default);
                    return icoDecoder.Frames[0];
                default: return null;
            }
        }
    }
}