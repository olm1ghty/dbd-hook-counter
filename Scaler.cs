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
    float referenceBlackBarOffset = 80f;

    int actualWidth;
    int actualHeight;
    float actualBlackBarOffset;

    public readonly float resolutionScaleX;
    public readonly float resolutionScaleY;

    public float HUDScale = 1f;
    public float MenuScale = 1f;

    float targetAspect;
    float actualAspect;

    public Scaler()
    {
        HUDScale = Properties.Settings.Default.HUDScale;
        MenuScale = Properties.Settings.Default.MenuScale;

        actualWidth = Screen.PrimaryScreen.Bounds.Width;
        actualHeight = Screen.PrimaryScreen.Bounds.Height;

        resolutionScaleX = (float)actualWidth / referenceWidth;
        resolutionScaleY = (float)actualHeight / referenceHeight;

        // Only compute actual black bars if aspect ratio is wider than 16:9
        targetAspect = 16f / 9f;
        actualAspect = (float)actualWidth / actualHeight;

        if (actualAspect < targetAspect)
        {
            int renderedHeight = (int)(actualWidth / targetAspect);
            int blackBarTotal = actualHeight - renderedHeight;
            actualBlackBarOffset = blackBarTotal / 2f;
        }
        else
        {
            actualBlackBarOffset = 0f;
        }
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

    private static void SaveImage(Mat image)
    {
        string folder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            "DebugCaptures");
        Directory.CreateDirectory(folder);

        string file = Path.Combine(
            folder,
            $"UI_{DateTime.Now:HH_mm_ss_fff}.png");

        image.Save(file);
    }

    public int ScaleYFromBottomAnchor(float originalY)
    {
        float referenceVisibleHeight = referenceHeight - 2 * referenceBlackBarOffset;
        float distanceFromBottom = referenceVisibleHeight - (originalY - referenceBlackBarOffset);
        float actualVisibleHeight = actualHeight - 2 * actualBlackBarOffset;
        float scaledDistance = distanceFromBottom * (actualVisibleHeight / referenceVisibleHeight);
        float finalY = actualHeight - actualBlackBarOffset - (scaledDistance * HUDScale);

        return (int)Math.Round(finalY);
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

        // X is anchored from the right
        float refRight = referenceWidth - rect.X;
        int scaledX = (int)(actualWidth - (refRight * resolutionScaleX * MenuScale));

        // Y uses linear top-left scale
        float refBottom = referenceHeight - rect.Y;
        float scaledBottom = refBottom * resolutionScaleY * MenuScale;
        int scaledY = (int)(actualHeight - scaledBottom);

        return new Rectangle(scaledX, scaledY, scaledWidth, scaledHeight);
    }

    public int ScaleOffsetX(int coordinate)
    {
        return (int)(coordinate * resolutionScaleX * HUDScale);
    }

    public int ScaleYOffsetHUD(int coordinate)
    {
        float adjustedReferenceBarOffset = 0f;
        float adjustedActualBarOffset = 0f;

        if (actualAspect < targetAspect)
        {
            // Letterboxing exists
            adjustedReferenceBarOffset = 80f;
            int renderedHeight = (int)(actualWidth / targetAspect);
            int blackBarTotal = actualHeight - renderedHeight;
            adjustedActualBarOffset = blackBarTotal / 2f;
        }

        float referenceVisibleHeight = referenceHeight - 2 * referenceBlackBarOffset;
        float resolutionScale = (float)(actualHeight - 2 * adjustedActualBarOffset) / referenceVisibleHeight;

        return (int)Math.Round(coordinate * resolutionScale * HUDScale);
    }
}
