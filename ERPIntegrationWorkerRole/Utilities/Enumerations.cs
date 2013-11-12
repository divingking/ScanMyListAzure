using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPIntegrationWorkerRole.Utilities

{
    public enum RecordCategory
    {
        Order,
        Receipt,
        PhysicalInventory,
        CycleCount,
        Return,
        QualityIssue,
        PhysicalDamage,
        SalesSample,
        Stolen
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

    public enum AccountTier
    {
        sales,
        manager,
        ceo
    }

    public enum BusinessTier
    {
        free,
        standard,
        pro
    }
}