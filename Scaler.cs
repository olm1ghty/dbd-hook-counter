using Emgu.CV.CvEnum;
using Emgu.CV;
using System.Windows.Forms;
using System.Diagnostics;
using Properties = DBDtimer.Properties;

public class Scaler
{
    public List<string> aspectRatios = new() { "16:9", "16:10" };
    public List<int> HUDscales = new() { 100, 95, 90, 85, 80, 75, 70 };
    public List<int> MenuScales = new() { 100, 95, 90, 85, 80, 75, 70 };

    public string aspectRatio = "16:10";

    int blackBorderHeight = 80;
    int screenHeight;

    public float aspectRatioMod;
    public int blackBorderMod;
    public int blackBorderModHUD;
    public float HUDScale = 1f;
    public float MenuScale = 1f;

    public Scaler()
    {
        aspectRatio = Properties.Settings.Default.AspectRatio;
        MenuScale = Properties.Settings.Default.MenuScale;
        HUDScale = Properties.Settings.Default.HUDScale;

        screenHeight = Screen.PrimaryScreen.Bounds.Height;

        switch (aspectRatio)
        {
            case "16:9":
                aspectRatioMod = 0.75f;
                blackBorderMod = -blackBorderHeight;
                blackBorderModHUD = 0;
                break;

            case "16:10":
                aspectRatioMod = 1;
                blackBorderMod = 0;
                blackBorderModHUD = -blackBorderHeight;
                break;
        }
    }

    public Mat LoadScaledTemplate(Bitmap bmp)
    {
        Mat full = bmp.ToMat();                        // original size
        if (Math.Abs(aspectRatioMod * HUDScale - 1.0) < 0.0001) return full;

        Mat small = new Mat();
        CvInvoke.Resize(full, small, Size.Empty,       // Size.Empty → use fx/fy
                        aspectRatioMod * HUDScale, aspectRatioMod * HUDScale, Inter.Area);
        full.Dispose();
        return small;
    }

    public Mat LoadScaledTemplateMenu(Bitmap bmp)
    {
        Mat full = bmp.ToMat();                        // original size
        if (Math.Abs(aspectRatioMod - 1.0) < 0.0001) return full;

        Mat small = new Mat();
        CvInvoke.Resize(full, small, Size.Empty,       // Size.Empty → use fx/fy
                        aspectRatioMod, aspectRatioMod, Inter.Area);
        full.Dispose();
        return small;
    }

    public Rectangle Scale(Rectangle rect)
    {
        int heightBeforeHUDScale = (int)(rect.Height * aspectRatioMod);

        Rectangle newRect = new Rectangle(
            (int)(rect.X * aspectRatioMod * HUDScale),
            (int)((rect.Y + blackBorderMod) * aspectRatioMod),
            (int)(rect.Width * aspectRatioMod * HUDScale),
            (int)(heightBeforeHUDScale * HUDScale));

        newRect.Y = ScaleYforRect(heightBeforeHUDScale, newRect);

        return newRect;
    }

    int ScaleYforRect(int heightBeforeScale, Rectangle rect)
    {
        int distance = screenHeight + blackBorderModHUD - rect.Y - heightBeforeScale;
        distance = (int)(distance * HUDScale);

        return (screenHeight + blackBorderModHUD - rect.Height - distance);
    }

    int ScaleYforCoordinateHUD(int coordinate)
    {
        int distance = screenHeight + blackBorderModHUD - coordinate;
        distance = (int)(distance * HUDScale);

        return (screenHeight + blackBorderModHUD - distance);
    }

    public Point Scale(Point point)
    {
        return new Point(
            (int)(point.X * aspectRatioMod * HUDScale),
            (int)((point.Y + blackBorderMod) * aspectRatioMod));
    }

    public int Scale(int coordinate)
    {
        return (int)(coordinate * aspectRatioMod * HUDScale);
    }

    public int ScaleY(int coordinate)
    {
        coordinate = (int)((coordinate + blackBorderMod) * aspectRatioMod);

        return ScaleYforCoordinateHUD(coordinate);
    }

    public Rectangle ScaleMenu(Rectangle rect)
    {
        int heightBeforeMenuScale = (int)(rect.Height * aspectRatioMod);

        Rectangle newRect = new(
            (int)(rect.X * aspectRatioMod),
            (int)((rect.Y + 2 * blackBorderMod) * aspectRatioMod * MenuScale),
            (int)(rect.Width * aspectRatioMod * MenuScale),
            (int)(heightBeforeMenuScale * MenuScale));

        newRect.Y = ScaleYforRect(heightBeforeMenuScale, newRect);

        return newRect;
    }
}
