using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MGT.Utilities.EventHandlers;
using MGT.ECG_Signal_Generator;
using System.Threading;
using System.Media;
using System.IO;
using System.Reflection;

namespace MGT.Cardia
{
    class ECGSound : IDisposable
    {
        private Cardia cardia;

        private bool active = true;

        private bool playBeat = true;
        private bool playAlarm = true;
        private int volume = 10;

        private bool edge = true;
        private bool alarm = false;
        private bool alarmPlaying = false;

        Thread worker;

        public ECGSound(Cardia cardia)
        {
            this.cardia = cardia;

            worker = new Thread(DoWork);
            worker.Name = "ECG Sound Worker";
            worker.Start();
            
            cardia.SignalGenerated += cardia_SignalGenerated;
            cardia.AlarmTripped += cardia_AlarmTripped;
            cardia.PlayBeatChanged += cardia_PlayBeatChanged;
            cardia.PlayAlarmChanged += cardia_PlayAlarmChanged;
            cardia.VolumeChanged += cardia_VolumeChanged;
        }

        void cardia_PlayBeatChanged(object sender, bool arg)
        {
            playBeat = arg;
        }

        void cardia_PlayAlarmChanged(object sender, bool arg)
        {
            playAlarm = arg;
            if (alarmPlaying)
            {
                StopAlarm();
                PlayAlarm();
            }
        }

        void cardia_SignalGenerated(object sender, SignalGeneratedEventArgs e)
        {
            if (e.Buffer[0].Beat && edge)
            {
                if (edge)
                    edge = false;

                lock (this)
                    Monitor.Pulse(this);
            }
            else
            {
                edge = true;
            }
        }

        void cardia_AlarmTripped(object sender, bool arg)
        {
            alarm = arg;

            if (arg)
                lock (this)
                    Monitor.Pulse(this);
            else
                StopAlarm();
        }

        void cardia_VolumeChanged(object sender, int arg)
        {
            volume = arg;

            if (alarmPlaying)
            {
                StopAlarm();
                PlayAlarm();
            }

        }

        private void DoWork()
        {
            while (active)
            {
                lock (this)
                    Monitor.Wait(this);

                if (active)
                    PlaySound();
            }
        }

        private void PlaySound()
        {
            if (alarm && playAlarm)
            {
                PlayAlarm();
            }
            else
            {
                StopAlarm();
                PlayBeep();
            }
        }

        private void PlayAlarm()
        {
            if (!playAlarm)
                return;

            if (alarmPlaying)
                return;

            switch (volume)
            {
            }

            alarmPlaying = true;
        }

        private void StopAlarm()
        {


            alarmPlaying = false;
        }

        private void PlayBeep()
        {
            if (!playBeat)
                return;

            switch (volume)
            {

            }
        }

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                active = false;

                lock (this)
                    Monitor.Pulse(this);

                worker.Abort();

            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
