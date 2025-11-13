using Infrastructure.Services.Gameplay;

namespace Hexes
{
    public class Hex
    {
        public ColorID ColorID;
        public HexModelView HexModelView;

        public Hex(HexModelView hexModelView, ColorID colorID)
        {
            ColorID = colorID;
            HexModelView = hexModelView;
        }
    }
}