using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TheStage.Controls.GameController
{
    //Media element like from Silverlight (SL)
    sealed public class MediaElementSL : MediaElement
    {
        public MediaElementState CurrentState { get; private set; }

        public MediaElementSL() : base()
        {
            CurrentState = MediaElementState.Closed;
            base.BufferingStarted += (s, e) => CurrentState = MediaElementState.Buffering;
            base.BufferingEnded += (s, e) => CurrentState = MediaElementState.Playing;
        }

        public new void Play()
        {
            base.Play();
            CurrentState = MediaElementState.Playing;
        }

        public new void Stop()
        {
            base.Stop();
            CurrentState = MediaElementState.Stopped;
        }

        public new void Pause()
        {
            base.Pause();
            CurrentState = MediaElementState.Paused;
        }
    }

    public enum MediaElementState
    {
        Closed,
        //Opening,
        //Individualizing,
        //AcquiringLicense,
        Buffering,
        Playing,
        Paused,
        Stopped
    }
}
