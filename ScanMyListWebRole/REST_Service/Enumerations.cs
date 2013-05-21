﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchWebRole.REST_Service
{
    public enum RecordCategory
    {
        Order,
        Receipt,
        Change
    }

    public enum RecordStatus
    {
        saved,
        sent,
        closed
    }

    public enum DeviceType
    {
        iPhone,
        Android,
        website
    }
}