using Emgu.CV.CvEnum;
using Emgu.CV;
using System.Windows.Forms;
using System.Drawing;
using Properties = DBD_Hook_Counter.Properties;
using ExCSS;
using Point = System.Drawing.Point;
using DBD_Hook_Counter;

public class Scaler
{
    public List<int> HUDscales = new() { 100, 95, 90, 85, 80, 75, 70 };
    public List<int> MenuScales = new() { 100, 95, 90, 85, 80, 75, 70 };

    int referenceWidth = 2560;
    int referenceHeight = 1600;

    int actualWidth;
    int actualHeight;

    float resolutionScaleX;
    float resolutionScaleY;

    float blackBarOffsetY = 80f;
    //float referenceBarOffsetY = 80f; // Because base rects were measured with black bars present

    public float HUDScale = 1f;
    public float MenuScale = 1f;

    public Scaler()
    {
        HUDScale = Properties.Settings.Default.HUDScale;
        MenuScale = Properties.Settings.Default.MenuScale;

        actualWidth = Screen.PrimaryScreen.Bounds.Width;
        actualHeight = Screen.PrimaryScreen.Bounds.Height;

        resolutionScaleX = (float)actualWidth / referenceWidth;
        resolutionScaleY = (float)actualHeight / referenceHeight;

        // Detect black bars if display is wider than 16:9 (i.e., 16:10 displaying a 16:9 render)
        float targetAspect = 16f / 9f;
        float displayAspect = (float)actualWidth / actualHeight;
    }

    private int AdjustReferenceY(int y)
    {
        return (int)(y - blackBarOffsetY);  // Adjust Y by removing the top black bar offset (80px)
    }

    int ScaleY(int originalY)
    {
        int adjustedY = AdjustReferenceY(originalY);  // removes 80 from top black bar
        float scaleFactor = (float)actualHeight / (referenceHeight - 2 * blackBarOffsetY); // scaling visible height
        return (int)(adjustedY * scaleFactor * HUDScale);  // ✅ apply HUD scale to match actual position
    }

    int ScaleYMenu(int originalY)
    {
        int adjustedY = (int)(originalY - (blackBarOffsetY * 2));  // Adjust the Y-coordinate by removing the black bar offset
        float scaleFactor = (float)actualHeight / (referenceHeight - 2 * blackBarOffsetY);  // Scaling based on visible area height (1440px)
        return (int)(adjustedY * scaleFactor);  // Apply the scaling factor
    }

    public int ScaleYFromBottomAnchor(float originalY)
    {
        float referenceBarOffsetY = 80f;
        float visibleReferenceHeight = referenceHeight - 2 * referenceBarOffsetY;
        float adjustedY = originalY - referenceBarOffsetY;

        float scale = (float)actualHeight / visibleReferenceHeight;
        float scaledYFullHUD = adjustedY * scale;

        float visibleTargetHeight = actualHeight;
        float anchoredY = visibleTargetHeight - ((visibleTargetHeight - scaledYFullHUD) * HUDScale);

        return (int)Math.Round(anchoredY);
    }

    public Mat LoadScaledTemplate(Bitmap bmp, bool isMenu = false)
    {
        float fx = resolutionScaleX * (isMenu ? MenuScale : HUDScale);
        float fy = resolutionScaleY * (isMenu ? MenuScale : HUDScale);

        if (Math.Abs(fx - 1.0f) < 0.0001f && Math.Abs(fy - 1.0f) < 0.0001f)
            return bmp.ToMat();

        Mat resized = new();
        CvInvoke.Resize(bmp.ToMat(), resized, Size.Empty, fx, fy, Inter.Area);
        //SaveImage(resized);

        return resized;
    }

    private static void SaveImage(Mat resized)
    {
        string folder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            "DebugCaptures");
        Directory.CreateDirectory(folder);

        string file = Path.Combine(
            folder,
            $"UI_{DateTime.Now:HH_mm_ss_fff}.png");

        //resized.Save(file);
    }

    public Rectangle Scale(Rectangle rect)
    {
        int scaledWidth = (int)(rect.Width * resolutionScaleX * HUDScale);
        int scaledHeight = (int)(rect.Height * resolutionScaleY * HUDScale);
        int scaledX = (int)(rect.X * resolutionScaleX * HUDScale);
        int scaledY = ScaleYFromBottomAnchor((float)rect.Y);

        return new Rectangle(scaledX, scaledY, scaledWidth, scaledHeight);
    }


    public Rectangle ScaleMenu(Rectangle rect)
    {
        int scaledWidth = (int)(rect.Width * resolutionScaleX * MenuScale);
        int scaledHeight = (int)(rect.Height * resolutionScaleY * MenuScale);
        int scaledX = (int)(rect.X * resolutionScaleX * MenuScale);
        int scaledY = (int)(ScaleYMenu(rect.Y) * MenuScale);

        return new Rectangle(scaledX, scaledY, scaledWidth, scaledHeight);
    }

    public Point Scale(Point point)
    {
        int adjustedY = point.Y;

        int scaledX = (int)(point.X * resolutionScaleX * HUDScale);
        int scaledY = (int)(ScaleY(point.Y) * HUDScale);

        return new Point(scaledX, scaledY);
    }

    public int ScaleOffsetX(int coordinate)
    {
        return (int)(coordinate * resolutionScaleX * HUDScale);
    }

    public int ScaleYOffsetHUD(int coordinate)
    {
        float referenceBarOffsetY = 80f;
        float referenceVisibleHeight = referenceHeight - 2 * referenceBarOffsetY;

        float resolutionScale = (float)actualHeight / referenceVisibleHeight;

        return (int)Math.Round(GameSettings.hookStageCounterOffset * resolutionScale * HUDScale);
    }
}
