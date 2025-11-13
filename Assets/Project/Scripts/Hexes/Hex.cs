namespace Infrastructure
{
    public class Hex
    {
        public HexModelView HexModelView;
        public ColorID ColorID;

        public Hex(HexModelView hexModelView, ColorID colorID)
        {
            ColorID = colorID;
            HexModelView = hexModelView;
        }
    }
}