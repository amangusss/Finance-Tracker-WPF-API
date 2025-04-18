using System.Globalization;
using System.Windows.Data;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Finance_Tracker_WPF_API.UI.Converters
{
    public class IncomeExpenseBarChartConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<object> items)
            {
                var grouped = items.GroupBy(item => ((DateTime)item.GetType().GetProperty("Date")?.GetValue(item)).Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Income = Math.Round(g.Where(t => t.GetType().GetProperty("Type")?.GetValue(t)?.ToString() == "Income").Sum(t => (decimal)(t.GetType().GetProperty("AmountInSelectedCurrency")?.GetValue(t) ?? 0m)), 2),
                        Expense = Math.Round(g.Where(t => t.GetType().GetProperty("Type")?.GetValue(t)?.ToString() == "Expense").Sum(t => (decimal)(t.GetType().GetProperty("AmountInSelectedCurrency")?.GetValue(t) ?? 0m)), 2)
                    }).ToList();

                var incomeSeries = new ColumnSeries<decimal>
                {
                    Name = "Income",
                    Values = grouped.Select(x => x.Income).ToArray(),
                    Fill = new SolidColorPaint(SKColors.Green)
                };
                var expenseSeries = new ColumnSeries<decimal>
                {
                    Name = "Expense",
                    Values = grouped.Select(x => x.Expense).ToArray(),
                    Fill = new SolidColorPaint(SKColors.Red)
                };
                return new ISeries[] { incomeSeries, expenseSeries };
            }
            return new ISeries[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}