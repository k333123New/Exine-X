﻿using Exine.ExineGraphics;
using Exine.ExineScenes;
//

namespace Exine.ExineObjects
{
    public class DecoObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Deco; }
        }

        public override bool Blocking
        {
            get { return false; }
        }

        public int Image;

        public DecoObject(uint objectID)
            : base(objectID)
        {
        }

        public void Load(ServerPacket.ObjectDeco info)
        {
            CurrentLocation = info.Location;
            MapLocation = info.Location;
            ExineMainScene.Scene.MapControl.AddObject(this);
            Image = info.Image;

            BodyLibrary = Libraries.Deco;
        }
        public override void Process()
        {
            DrawLocation = new Point((CurrentLocation.X - User.Movement.X + MapControl.OffSetX) * MapControl.CellWidth, (CurrentLocation.Y - User.Movement.Y + MapControl.OffSetY) * MapControl.CellHeight);
            DrawLocation.Offset(GlobalDisplayLocationOffset);
            DrawLocation.Offset(User.OffSetMove);
        }

        public override void Draw()
        {
            BodyLibrary.Draw(Image, DrawLocation, DrawColour, true);
        }

        public override bool MouseOver(Point p)
        {
            return false;
        }

        public override void DrawBehindEffects(bool effectsEnabled)
        {
        }

        public override void DrawEffects(bool effectsEnabled)
        {
        }
    }
}
