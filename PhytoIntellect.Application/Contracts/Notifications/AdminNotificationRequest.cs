using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Notifications
{
    public class AdminNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// يحدد الدور المستهدف: "Herbalist" أو "Patient" أو "All"
        /// </summary>
        public string TargetRole { get; set; } = "All";
    }
}
