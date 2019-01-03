using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

interface IUsable
{
    bool IsCtrlling { get; set; }

    void OnBeginCtrl();

    void DoCtrl();

    void OnStopCtrl();
}