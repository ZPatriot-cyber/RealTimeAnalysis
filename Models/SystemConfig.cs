using System;
using System.Collections.Generic;
using System.Text;

namespace RealTimeAnalysis.Models
{
    /// Параметры вычислительной системы: число процессоров и наличие сети.
    public record SystemConfig(int CpuCount, bool HasNetwork);
 
}
