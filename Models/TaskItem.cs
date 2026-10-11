using System;
using System.Collections.Generic;
using System.Text;

namespace RealTimeAnalysis.Models
/// Описание одной задачи: имя, класс жёсткости, время выполнения C,
/// относительный дедлайн D и период T (в миллисекундах).
{
    public record TaskItem(string Name, Hardness Hardness, double C, double D, double T);
}
