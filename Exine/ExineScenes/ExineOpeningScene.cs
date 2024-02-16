using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineSounds;
using S = ServerPackets;
using C = ClientPackets;

namespace Exine.ExineScenes
{
    public sealed class ExineOpeningScene : ExineScene
    {
        private ExineAnimatedControl _background;
        
        public ExineOpeningScene()
        {
            SoundManager.PlayMusic(SoundList.ExineIntroMusic, true);
            SoundManager.PlaySound(SoundList.ExineIntroWind, false);

            Disposing += (o, e) =>
            {
                SoundManager.StopMusic();
                SoundManager.StopSound(SoundList.ExineIntroWind);
            };

            _background = new ExineAnimatedControl
            {
                Animated = true,
                AnimationCount = 300,
                AnimationDelay = 34,
                Index = 0,
                Library = Libraries.ExineOpening,
                Loop = false,
                Parent = this,
                Visible = true,
                Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
            };
            /*
            _background.AfterAnimation += (o, e) =>
            { 
                MirScene.ActiveScene = new ExineLoginScene();
                Dispose();
            };*/

            _background.Click += (o, e) =>
            { 
                ExineScene.ActiveScene = new ExineLoginScene();
                Dispose();
            }; 
        }

        public override void Process()
        {
           
        }

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _background = null;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
