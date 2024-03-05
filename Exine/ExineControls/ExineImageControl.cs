using Exine.ExineGraphics;

namespace Exine.ExineControls
{
    public class ExineImageControl : ExineControl
    {
        #region Tint Colour k333123
        private Color _tintColour;
        public Color TintColour
        {
            get { return _tintColour; }
            set
            {
                if (_tintColour == value)
                    return;
                _tintColour = value;
                //OnForeColourChanged();
            }
        }
        #endregion

        public override Point DisplayLocation { get { return UseOffSet ? base.DisplayLocation.Add(Library.GetOffSet(Index)) : base.DisplayLocation; } }
        public Point DisplayLocationWithoutOffSet { get { return base.DisplayLocation; } }

        #region Auto Size
        private bool _autoSize;
        public bool AutoSize
        {
            get { return _autoSize; }
            set
            {
                if (_autoSize == value)
                    return;
                _autoSize = value;
                OnAutoSizeChanged(EventArgs.Empty);
            }
        }
        public event EventHandler AutoSizeChanged;
        private void OnAutoSizeChanged(EventArgs e)
        {
            TextureValid = false;
            if (AutoSizeChanged != null)
                AutoSizeChanged.Invoke(this, e);
        }
        #endregion

        #region DrawImage
        private bool _drawImage;
        public bool DrawImage
        {
            get { return _drawImage; }
            set
            {
                if (_drawImage == value)
                    return;
                _drawImage = value;
                OnDrawImageChanged();
            }
        }
        public event EventHandler DrawImageChanged;
        private void OnDrawImageChanged()
        {
            Redraw();
            if (DrawImageChanged != null)
                DrawImageChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Index
        private int _index;
        public virtual int Index
        {
            get { return _index; }
            set
            {
                if (_index == value)
                    return;
                _index = value;
                OnIndexChanged();
            }
        }
        public event EventHandler IndexChanged;
        protected void OnIndexChanged()
        {
            OnSizeChanged();
            if (IndexChanged != null)
                IndexChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Library
        private MLibrary _library;
        public MLibrary Library
        {
            get { return _library; }
            set
            {
                if (_library == value)
                    return;
                _library = value;
                OnLibraryChanged();
            }
        }
        public event EventHandler LibraryChanged;
        private void OnLibraryChanged()
        {
            OnSizeChanged();
            if (LibraryChanged != null)
                LibraryChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region PixelDetect
        private bool _pixelDetect;
        protected bool PixelDetect
        {
            set
            {
                if (_pixelDetect == value)
                    return;
                _pixelDetect = value;
                OnPixelDetectChanged();
            }
        }
        public event EventHandler PixelDetectChanged;
        private void OnPixelDetectChanged()
        {
            Redraw();
            if (PixelDetectChanged != null)
                PixelDetectChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region UseOffset
        private bool _useOffSet;
        public bool UseOffSet
        {
            protected get { return _useOffSet; }
            set
            {
                if (_useOffSet == value)
                    return;
                _useOffSet = value;
                OnUseOffSetChanged();
            }
        }
        public event EventHandler UseOffSetChanged;
        private void OnUseOffSetChanged()
        {
            OnLocationChanged();
            if (UseOffSetChanged != null)
                UseOffSetChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Size
        public override Size Size
        {
            set { base.Size = value; }
            get
            {
                if (AutoSize && Library != null && Index >= 0)
                    return Library.GetTrueSize(Index);
                return base.Size;
            }
        }

        public override Size TrueSize
        {
            get
            {
                if (Library != null && Index >= 0)
                    return Library.GetTrueSize(Index);
                return base.TrueSize;
            }
        }

        #endregion

        public ExineImageControl()
        {
            _drawImage = true;
            _index = -1;
            ForeColour = Color.White;
            _autoSize = true;
            TintColour = Color.Red; //k333123
            BorderColour = Color.Transparent;//k333123 240305
        }

        protected internal override void DrawControl()
        {
            base.DrawControl();

            if (DrawImage && Library != null)
            {
                bool oldGray = DXManager.GrayScale;

                if (GrayScale)
                {
                    DXManager.SetGrayscale(true);
                }

                if (Blending)
                    Library.ExineDrawTinted(Index, DisplayLocation, ForeColour, TintColour, true,true,1.0f);//k333123

                else
                    Library.DrawTinted(Index, DisplayLocation, ForeColour, TintColour, true);//k333123

                if (GrayScale) DXManager.SetGrayscale(oldGray);

            }
        }

        public override bool IsMouseOver(Point p)
        {
            return base.IsMouseOver(p) && (!_pixelDetect || Library.VisiblePixel(Index, p.Subtract(DisplayLocation), true) || Moving);
        }

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            DrawImageChanged = null;
            _drawImage = false;

            IndexChanged = null;
            _index = 0;

            LibraryChanged = null;
            Library = null;

            PixelDetectChanged = null;
            _pixelDetect = false;

            UseOffSetChanged = null;
            _useOffSet = false;
        }
        #endregion
    }
}