public class UIScaler
{
    List<string> resolutions = new() { "1920 x 1080" };
    List<string> aspectRatios = new() { "16 x 9", "16 x 10" };
    string resolution;
    string aspectRatio = "16 x 9";
    int blackBorderHeight = 80;

    public float aspectRatioMod;
    public int blackBorderMod;

    public UIScaler()
    {
        switch (aspectRatio)
        {
            case "16 x 9":
                aspectRatioMod = 0.75f;
                blackBorderMod = -blackBorderHeight;
                break;

            case "16 x 10":
                aspectRatioMod = 1;
                blackBorderMod = 0;
                break;
        }
    }

    public Rectangle Scale(Rectangle rect)
    {
        return new Rectangle(
            (int)(rect.X * aspectRatioMod),
            (int)((rect.Y + blackBorderMod) * aspectRatioMod),
            (int)(rect.Width * aspectRatioMod),
            (int)(rect.Height * aspectRatioMod));
    }

    public Rectangle ScaleMenu(Rectangle rect)
    {
        return new Rectangle(
            (int)(rect.X * aspectRatioMod),
            (int)((rect.Y + 2 * blackBorderMod) * aspectRatioMod),
            (int)(rect.Width * aspectRatioMod),
            (int)(rect.Height * aspectRatioMod));
    }

    public Point Scale(Point point)
    {
        return new Point(
            (int)(point.X * aspectRatioMod),
            (int)((point.Y + blackBorderMod) * aspectRatioMod));
    }

    public int Scale(int coordinate)
    {
        return (int)(coordinate * aspectRatioMod);
    }

    public int ScaleY(int coordinate)
    {
        return (int)((coordinate + blackBorderMod) * aspectRatioMod);
    }
}
