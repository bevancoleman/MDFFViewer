using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using MDFFParserLibrary;
using MDFFParserLibrary.Models.Enums;
using ReactiveUI;
using SkiaSharp;

namespace MDFFViewer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public string FileName => @"test file.csv";
    
    public ReactiveCommand<Unit, Unit> BtnImportFileAction { get; }

    
 /*   public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 2, 1, 3, 5, 3, 4, 6 },
            Fill = null
        }
    };*/

 public IList<ISeries> Series { get; set; } = new List<ISeries>();

    public LabelVisual Title { get; set; } =
        new LabelVisual
        {
            Text = "My chart title",
            TextSize = 25,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };

    public MainViewModel()
    {
        BtnImportFileAction = ReactiveCommand.Create(() =>
        {
            var parser = new Parser();
            var results = parser.ReadFile(FileName);

            var seriesData = parser.ConvertToSeries(results, new DateTime(2023, 04, 01), new DateTime(2023, 06, 30), 30, DataUnitOfMeasure.kWh);

 
            foreach (var dataSeries in seriesData.DataSeries)
            {
                Series.Add(new LineSeries<decimal>() {
                    Values = dataSeries.Value.Values,
                    Fill = null,
                    Name = dataSeries.Value.Name,
                    GeometrySize = 5,
                    LineSmoothness = 0,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 1 }
                });
            }
        });
    }
}