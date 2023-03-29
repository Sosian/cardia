﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using MGT.Utilities.EventHandlers;

namespace MGT.HRM.HRP
{
    public abstract class BtHrpFileLogger : BtHrpLogger, IHRMFileLogger
    {
        //public event GenericEventHandler<string> FileNameChanged; TODO

        protected string? fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                if (running)
                    throw new Exception("Filename cannot be changed when the logger is running");

                string bck = fileName;

                fileName = value;
                // if (bck != value)
                    // if (FileNameChanged != null)
                    //     FileNameChanged(this, value);
            }
        }

        public override void ResetSubscriptions()
        {
            base.ResetSubscriptions();
            // FileNameChanged = null;
        }
    }
}
